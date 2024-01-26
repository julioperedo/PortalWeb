using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class OpenBoxController : BaseController
    {
        #region Constructores

        public OpenBoxController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(long? IdSubsidiary, string Product, bool Enabled, bool WithStock)
        {
            string message = "";
            try
            {
                var lstItems = GetItems(new Models.OpenBoxFilters { IdSubsidiary = IdSubsidiary, Product = Product, Enabled = Enabled, WithStock = WithStock });
                var items = lstItems.Select(x => new { x.Id, Subsidiary = x.Subsidiary.Name, x.Warehouse, x.Product.ItemCode, ItemName = x.Product.Name, x.Quantity, x.Enabled, x.WithStock, x.Price });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Edit(long Id)
        {
            BEP.OpenBox beItem;
            if (Id == 0)
            {
                beItem = new BEP.OpenBox { Enabled = true };
            }
            else
            {
                BCP.OpenBox bcOBox = new BCP.OpenBox();
                beItem = bcOBox.Search(Id);
            }
            return PartialView(beItem);
        }

        public IActionResult GetProducts()
        {
            BCP.Product bcProduct = new();
            List<Field> filters = new() { new Field("Enabled", true) };
            var products = bcProduct.List(filters, "1");
            return Json(products.Select(x => new { Id = x.Id.ToString(), Name = $"{x.ItemCode} - {x.Name}" }));
        }

        public IActionResult GetWarehouses2(string Subsidiary)
        {
            Subsidiary = $"'{Subsidiary}'";
            string strMessage = "";
            IEnumerable<BEA.Item> lstItems = Enumerable.Empty<BEA.Item>();
            try
            {
                if (CardCode == HomeCardCode)
                {
                    BCA.Warehouse bcStorehouse = new();
                    lstItems = bcStorehouse.ListIn(Subsidiary);
                }
                else
                {
                    BCP.WarehouseAllowed bcWharehouse = new();
                    IEnumerable<BEP.WarehouseAllowed> lstWharehouses = bcWharehouse.List("Name");
                    lstItems = (from i in lstWharehouses
                                where Subsidiary.ToUpper().Replace("'", "").Split(',').Contains(i.Subsidiary.ToUpper()) & i.ClientVisible
                                select new BEA.Item { Code = i.Code, Name = i.Name }).ToList();
                }
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            return Json(lstItems.OrderBy(x => x.Name).Select(x => new { x.Code, x.Name }).Distinct());
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BEP.OpenBox Item, Models.OpenBoxFilters Filters)
        {
            string message = "";
            try
            {
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;

                BCP.OpenBox bcOBox = new BCP.OpenBox();
                bcOBox.Save(ref Item);

                var lstItems = GetItems(Filters);
                var items = lstItems.Select(x => new { x.Id, Subsidiary = x.Subsidiary.Name, x.Warehouse, x.Product.ItemCode, ItemName = x.Product.Name, x.Quantity, x.Enabled, x.WithStock, x.Price });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete(long Id, Models.OpenBoxFilters Filters)
        {
            string message = "";
            try
            {
                BEP.OpenBox Item = new BEP.OpenBox { StatusType = BEntities.StatusType.Delete, Id = Id, LogDate = DateTime.Now };

                BCP.OpenBox bcOBox = new BCP.OpenBox();
                bcOBox.Save(ref Item);

                var lstItems = GetItems(Filters);
                var items = lstItems.Select(x => new { x.Id, Subsidiary = x.Subsidiary.Name, x.Warehouse, x.Product.ItemCode, ItemName = x.Product.Name, x.Quantity, x.Enabled, x.WithStock, x.Price });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Metodos Privados

        private List<BEP.OpenBox> GetItems(Models.OpenBoxFilters itemFilters = null)
        {
            if (itemFilters == null) itemFilters = new Models.OpenBoxFilters { Enabled = true, WithStock = true };
            BCP.OpenBox bcOpen = new BCP.OpenBox();
            List<Field> filters = new List<Field> { new Field("t0.Enabled", itemFilters.Enabled ? 1 : 0) };
            if (itemFilters.IdSubsidiary.HasValue && itemFilters.IdSubsidiary.Value > 0)
            {
                filters.Add(new Field("IdSubsidiary", itemFilters.IdSubsidiary.Value));
            }
            if (!string.IsNullOrWhiteSpace(itemFilters.Product))
            {
                filters.AddRange(new[] {
                    new Field("t1.Name", itemFilters.Product.Trim(), Operators.Likes), new Field("t1.Description", itemFilters.Product.Trim(), Operators.Likes), new Field("t1.ItemCode", itemFilters.Product.Trim(), Operators.Likes),
                    new Field(LogicalOperators.Or), new Field(LogicalOperators.Or)
                });
            }
            CompleteFilters(ref filters);
            List<BEP.OpenBox> lstItems = bcOpen.ListByProduct(filters, "1", BEP.relOpenBox.Product, BEP.relOpenBox.Subsidiary);

            if (lstItems?.Count > 0)
            {
                BCA.ProductStock bcInventory = new BCA.ProductStock();
                filters = new List<Field> { new Field("ItemCode", string.Join(",", lstItems.Select(x => $"'{x.Product.ItemCode}'")), Operators.In) };
                var stockItems = bcInventory.List(filters, "1");

                lstItems.ForEach(x => x.WithStock = (from s in stockItems where s.Subsidiary.ToLower() == x.Subsidiary.Name.ToLower() & s.Warehouse.ToLower() == x.Warehouse.ToLower() & s.ItemCode.ToLower() == x.Product.ItemCode.ToLower() select s.Stock).Sum() > 0);
            }
            lstItems = lstItems.FindAll(x => x.WithStock == itemFilters.WithStock);
            return lstItems;
        }

        #endregion

    }
}