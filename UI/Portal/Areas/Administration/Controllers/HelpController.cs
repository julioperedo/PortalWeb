using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class HelpController : BaseController
    {
        #region Constructores

        public HelpController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            int intPermission = GetPermission("Notificaciones");
            if (IsAllowed(this))
            {
                ViewData["Permission"] = intPermission;
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter()
        {
            string message = "";
            IEnumerable<BEB.UserHelp> items = new List<BEB.UserHelp>();
            try
            {
                items = GetItems();
            }
            catch (Exception ex)
            {
                message = "Se ha producido el siguiente error al traer los datos: <br />" + GetError(ex);
            }
            return Json(new { message, items });
        }

        public IActionResult Edit(long Id = -1)
        {
            if (Id >= 0)
            {
                BEB.UserHelp beItem;
                if (Id == 0)
                {
                    beItem = new BEB.UserHelp();
                }
                else
                {
                    BCB.UserHelp bcHelp = new BCB.UserHelp();
                    beItem = bcHelp.Search(Id);
                    if (!string.IsNullOrWhiteSpace(beItem.Value))
                    {
                        beItem.Value = SetHTMLSafe(beItem.Value);
                    }
                }
                Models.UserHelp item = new Models.UserHelp(beItem);
                return PartialView(item);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public IActionResult Edit(BEB.UserHelp Item)
        {
            string message = "";
            IEnumerable<BEB.UserHelp> items = new List<BEB.UserHelp>();
            try
            {
                Item.StatusType = Item.Id > 0 ? StatusType.Update : StatusType.Insert;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;

                BCB.UserHelp bcNotification = new BCB.UserHelp();
                bcNotification.Save(ref Item);
                items = GetItems();
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        [HttpPost()]
        public IActionResult Delete(long Id)
        {
            string message = "";
            IEnumerable<BEB.UserHelp> items = new List<BEB.UserHelp>();
            DateTime objDate = DateTime.Now;
            try
            {
                BEB.UserHelp item = new() { Id = Id, StatusType = StatusType.Delete, LogDate = objDate };
                BCB.UserHelp bcHelp = new();
                bcHelp.Save(ref item);
                items = GetItems();
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region Metodos Privados

        private IEnumerable<BEB.UserHelp> GetItems()
        {
            BCB.UserHelp bcHelp = new();
            IEnumerable<BEB.UserHelp> lstItems = bcHelp.List("1");
            return lstItems;
        }

        #endregion
    }
}
