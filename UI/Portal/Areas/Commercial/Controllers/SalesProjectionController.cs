using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCL = BComponents.Sales;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEL = BEntities.Sales;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class SalesProjectionController : BaseController
    {
        #region Constructores

        public SalesProjectionController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public IActionResult Filter(int Year, int Month)
        {
            string message = "";
            List<Models.SalesProjection> items = new List<Models.SalesProjection>();
            try
            {
                items = GetItems(Year, Month);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Save(IList<BEL.Projection> Items, int Year, int Month)
        {
            string message = "";
            List<Models.SalesProjection> items = new();
            try
            {
                BCL.Projection bcProyection = new();
                foreach (var item in Items)
                {
                    item.StatusType = item.Id > 0 ? BEntities.StatusType.Update : BEntities.StatusType.Insert;
                    item.LogDate = DateTime.Now;
                    item.LogUser = UserCode;
                }
                bcProyection.Save(ref Items);
                items = GetItems(Year, Month);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region Métodos Privados

        private List<Models.SalesProjection> GetItems(int Year, int Month)
        {
            List<Models.SalesProjection> items = new();

            List<Field> filters = new() { new Field("[Year]", Year), new Field("[Month]", Month), new Field(LogicalOperators.And) };
            BCL.Projection bcProyection = new();
            IEnumerable<BEL.Projection> projections = bcProyection.List(filters, "1");
            List<string> divisions = new() { "Consumer", "Enterprise", "Mobile" };
            BCB.Classifier bcClassifier = new();
            List<BEB.Classifier> subsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "Value");

            foreach (var s in subsidiaries)
            {
                foreach (var d in divisions)
                {
                    var item = projections.FirstOrDefault(x => x.Subsidiary.ToLower() == s.Name.ToLower() & x.Division.ToLower() == d.ToLower());
                    if (item == null)
                    {
                        items.Add(new Models.SalesProjection(0, ToTitle(s.Name), d, Year, Month, 0));
                    }
                    else
                    {
                        items.Add(new Models.SalesProjection(item));
                    }
                }
            }

            return items;
        }

        #endregion

    }
}
