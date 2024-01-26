using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BEntities.Filters;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCS = BComponents.Security;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BES = BEntities.Security;
using Microsoft.AspNetCore.Hosting;

namespace Portal.Areas.Visits.Controllers
{
    [Area("Visits")]
    [Authorize]
    public class GuardsRouteController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration config;
        #endregion

        #region Constructores

        public GuardsRouteController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { config = configuration; }

        #endregion

        #region GETs

        public ActionResult Index()
        {
            if (IsAllowed("Visits", "GuardsRoute", "Index"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public ActionResult GetPoints(long GuardId, DateTime? Since, DateTime? Until)
        {
            string message = "";
            try
            {
                BCD.CheckPoint bcPoints = new();
                List<Field> lstFilters = new() { };
                string dateFormat = "yyyy-MM-dd HH:mm:ss";
                if (GuardId != 0)
                {
                    lstFilters.Add(new Field("IdGuard", GuardId));
                }
                if (Since.HasValue)
                {
                    lstFilters.Add(new Field("CheckDate", Since.Value.ToString(dateFormat), Operators.HigherOrEqualThan));
                }
                if (Until.HasValue)
                {
                    lstFilters.Add(new Field("CheckDate", Until.Value.ToString(dateFormat), Operators.LowerOrEqualThan));
                }
                CompleteFilters(ref lstFilters);
                IEnumerable<BED.CheckPoint> lstPoints = bcPoints.List(lstFilters, "CheckDate");
                var items = from c in lstPoints orderby c.CheckDate select new { c.Latitude, c.Longitude, Date = c.CheckDate, c.Type, c.Accuracy, c.Provider, c.Altitude };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public ActionResult GetGuards()
        {
            BCS.User bcUser = new();
            List<BES.User> lstUsers = bcUser.ListWithCheckpoints(null, null);
            var items = lstUsers.Select(u => new { u.Id, u.Name });
            return Json(items);
        }

        #endregion
    }
}