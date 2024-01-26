using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System.Collections.Generic;
using BCP = BComponents.Product;
using BEP = BEntities.Product;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class PiggyBankConfigController : BaseController
    {
        #region Constructors

        public PiggyBankConfigController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetProducts()
        {
            BCP.Product bcProduct = new();
            var items = bcProduct.ListWithPrices(new List<Field>(), "ItemCode");
            return Json(items);
        }

        #endregion

        #region POSTs

        #endregion

        #region Private Methods

        #endregion
    }
}
