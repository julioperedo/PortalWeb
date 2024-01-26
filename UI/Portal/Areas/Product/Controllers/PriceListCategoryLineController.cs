using BEntities;
using BEntities.Filters;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using Field = BEntities.Filters.Field;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class PriceListCategoryLineController : BaseController
    {
        #region Constructors

        public PriceListCategoryLineController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetGroups()
        {
            string message = "";
            try
            {
                BCP.PriceGroup bcGroup = new();
                var groups = bcGroup.List("1");
                var items = groups.Select(x => new { x.Id, x.Name, x.Percentage });
                return Json(new { message, items });
            }
            catch (System.Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetProducts(long LineId)
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                var tempProds = bcProduct.ListByLine(LineId, "Category, ItemCode", BEP.relProduct.Prices);
                string prodCodes = string.Join(",", tempProds.Select(x => $"'{x.ItemCode.ToLower()}'"));
                BCA.ProductStock bcInventory = new();
                var filters = new List<Field> {
                    new Field("LOWER(ItemCode)", prodCodes, Operators.In), new Field("Subsidiary", "Santa Cruz"), new Field("Stock", 0, Operators.HigherThan),
                    new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                };
                IEnumerable<BEA.ProductStock> lstFifo;
                try
                {
                    lstFifo = bcInventory.ListBalance(filters, "1");
                }
                catch (System.Exception)
                {
                    lstFifo = new List<BEA.ProductStock>();
                }

                var products = from x in tempProds
                               select new
                               {
                                   x.Id,
                                   x.ItemCode,
                                   Name = SetHTMLSafe(x.Name),
                                   x.Category,
                                   Cost = x.Cost ?? lstFifo.FirstOrDefault(i => i.ItemCode.ToLower() == x.ItemCode.ToLower())?.PriceReal ?? 0,
                                   IdPrice = x.ListPrices.FirstOrDefault(i => i.IdSudsidiary == (long)BEE.Subsidiaries.SantaCruz)?.Id,
                                   Price = x.ListPrices.FirstOrDefault(i => i.IdSudsidiary == (long)BEE.Subsidiaries.SantaCruz)?.Regular
                               };

                BCP.PriceGroupLine bcGroup = new();
                var tempGroups = bcGroup.ListByLine(LineId, "1");
                var groups = tempGroups.Select(x => new { x.Id, x.IdLine, x.IdGroup, x.Percentage });

                return Json(new { message, products, groups });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetGroupsByline(long LineId)
        {
            string message = "";
            try
            {
                BCP.PriceGroupLine bcGroup = new();
                var groupList = bcGroup.ListByLine(LineId, "1", BEP.relPriceGroupLine.Group);
                var groups = groupList.Where(x => x.IdGroup != 1).Select(x => new { x.Id, x.Group.Name });

                BCP.PriceGroupLineClient bcClient = new();
                var clientList = bcClient.ListByLine(LineId, "1", BEP.relPriceGroupLineClient.GroupLine, BEP.relPriceGroupLine.Group);

                string codes = string.Join(",", clientList.Select(x => $"'{x.CardCode.ToLower()}'"));
                List<Field> filters = new() { new Field("LOWER(CardCode)", codes, Operators.In) };
                BCA.Client bcClientSAP = new();
                var clientsSAP = !string.IsNullOrEmpty(codes) ? bcClientSAP.List(filters, "1") : new List<BEA.Client>();

                var clients = clientList.Select(x => new { x.CardCode, CardName = clientsSAP.FirstOrDefault(i => i.CardCode.ToLower() == x.CardCode.ToLower())?.CardName ?? "", x.IdGroupLine, GroupName = x.GroupLine.Group.Name });

                return Json(new { message, groups, clients });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Save(IList<BEP.PriceGroupLine> Groups, IList<BEP.Product> Products, IList<BEP.Price> Prices)
        {
            string message = "";
            try
            {
                DateTime now = DateTime.Now;
                foreach (var g in Groups)
                {
                    g.StatusType = g.Id > 0 ? StatusType.Update : StatusType.Insert;
                    g.LogUser = UserCode;
                    g.LogDate = now;
                }
                BCP.PriceGroupLine bcGroup = new();
                bcGroup.Save(ref Groups);

                BCP.Product bcProduct = new();
                foreach (var p in Products)
                {
                    p.LogUser = UserCode;
                    p.LogDate = now;
                }
                bcProduct.UpdateCosts(Products);

                foreach (var p in Prices)
                {
                    p.StatusType = p.Id > 0 ? StatusType.Update : StatusType.Insert;
                    p.LogUser = UserCode;
                    p.LogDate = now;
                }
                BCP.Price bcPrice = new();
                bcPrice.Save(ref Prices);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SaveClient(long GroupLineId, string CardCode)
        {
            string message = "";
            try
            {
                BCP.PriceGroupLineClient bcGroup = new();
                BEP.PriceGroupLineClient item = new() { CardCode = CardCode, IdGroupLine = GroupLineId, LogUser = UserCode, LogDate = DateTime.Now, StatusType = StatusType.Insert };
                bcGroup.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteClient(long GroupLineId, string CardCode)
        {
            string message = "";
            try
            {
                BCP.PriceGroupLineClient bcGroup = new();
                bcGroup.Delete(GroupLineId, CardCode);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
