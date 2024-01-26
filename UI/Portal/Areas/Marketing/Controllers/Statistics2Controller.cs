using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using BCP = BComponents.Product;
using BCY = BComponents.PiggyBank;
using BEP = BEntities.Product;
using BEY = BEntities.PiggyBank;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class Statistics2Controller : BaseController
    {
        #region Constructors

        public Statistics2Controller(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Filter()
        {
            string message = "";
            try
            {
                BCY.Serial bcSerial = new();
                List<Field> filters = new() { new Field("State", "V") };
                var serials = bcSerial.List(filters, "1", BEY.relSerial.User);
                var items = serials.Select(x => new { x.Id, x.IdUser, x.RegisterDate.Date, x.CardCode, x.ItemCode, x.Points, UserName = x.User.Name, x.User.StoreName, City = ToTitle(x.User.City.Trim()) });

                var clients = serials.Select(x => new { x.CardCode, x.CardName }).Distinct();

                BCP.Product bcProduct = new();
                IEnumerable<BEP.Product> lstProducts = bcProduct.ListForPiggyBank("1");
                var products = lstProducts.Select(x => new { x.Id, x.ItemCode, x.Name, x.Line });

                return Json(new { message, items, products, clients });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private int GetQuarter(DateTime Item) => (Item.Month - 1) / 3 + 1;

        #endregion
    }
}
