using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using BCC = BComponents.CIESD;
using BCP = BComponents.Product;
using BEC = BEntities.CIESD;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class MicrosoftProductController : BaseController
    {
        #region Constructores

        public MicrosoftProductController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs
        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                if (GetPermission("MicrosoftProducts") > 0)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Denied");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter()
        {
            string message = "";
            try
            {
                BCC.Product bcProduct = new();
                IEnumerable<BEC.Product> products = bcProduct.List("1", BEC.relProduct.Prices);

                BCP.Price bcPrice = new();
                string codes = string.Join(",", from p in products where !string.IsNullOrEmpty(p.ItemCode) select $"'{p.ItemCode}'");
                IEnumerable<BEP.Price> prices = bcPrice.ListByCode(codes, "1", BEP.relPrice.Product);

                var items = from p in products
                            join r in prices on p.ItemCode.ToLower() equals r.Product.ItemCode.ToLower() into lfPrices
                            from l in lfPrices.DefaultIfEmpty()
                            select new { p.Id, p.Sku, p.ItemCode, p.Name, p.Description, p.ReturnType, p.FulfillmentType, p.Enabled, Price = l?.Regular, Cost = p.ListPrices?.Last().Amount };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetPrices(long Id, string ItemCode)
        {
            string message = "";
            try
            {
                BCC.Price bcPriceX = new();
                List<Field> filters = new() { new Field("IdProduct", Id) };
                IEnumerable<BEC.Price> items = bcPriceX.List(filters, "1");

                BCP.Price bcPrice = new();
                BEP.Price price = bcPrice.Search(ItemCode, 1);
                if (price == null || price.IdProduct == 0)
                {
                    BCP.Product bcProduct = new();
                    var product = bcProduct.Search(ItemCode);
                    price = new BEP.Price { IdProduct = product.Id, IdSudsidiary = 1 };
                }
                return Json(new { message, items, price });
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
        public IActionResult Edit(BEP.Price Item, IList<BEC.Price> Prices)
        {
            string message = "";
            try
            {
                DateTime now = DateTime.Now;

                BCP.Price bcPrice = new();
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogUser = UserCode;
                Item.LogDate = now;
                bcPrice.Save(ref Item);

                BCC.Price bcPriceX = new();
                foreach (var item in Prices)
                {
                    item.LogUser = UserCode;
                    item.LogDate = now;
                }
                bcPriceX.Save(ref Prices);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Metodos Privados

        #endregion
    }
}
