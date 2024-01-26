using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCP = BComponents.Product;
using BCM = BComponents.Marketing;
using BEM = BEntities.Marketing;
using Microsoft.AspNetCore.Hosting;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class ConfigOffersMailController : BaseController
    {
        #region Constructores

        public ConfigOffersMailController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        // GET: Marketing/OffersMailConfig
        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                BCP.Line bcLine = new();
                BCM.OffersMailConfig bcConfig = new();
                Models.OffersMailConfig beData = new() { Lines = bcLine.ListForOffers(), Asigned = bcConfig.ListWithOffers("1", BEM.relOffersMailConfig.Line) };
                return View(beData);
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Save(IList<BEM.OffersMailConfig> Items)
        {
            string strMessage = "";
            try
            {
                long UserId = UserCode;
                var now = DateTime.Now;
                foreach (var item in Items)
                {
                    item.LogUser = UserId;
                    item.LogDate = now;
                    item.StatusType = item.WeekDay == 0 ? BEntities.StatusType.Delete : (item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update);
                }
                BCM.OffersMailConfig bcConfig = new();
                bcConfig.Save(ref Items);
                return Json(new { Message = strMessage, Items });
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            return Json(new { Message = strMessage });
        }

        #endregion

        #region Private Methods

        #endregion
    }
}