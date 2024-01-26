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
using BCS = BComponents.Security;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class BlackListController : BaseController
    {
        #region Constructores

        public BlackListController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewBag.CardCode = CardCode;
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetBlacklist()
        {
            string message = "";
            IEnumerable<string> items = null;
            try
            {
                BCS.BlackList bcBlacklist = new();
                IEnumerable<BES.BlackList> lstItems = bcBlacklist.List("CardCode");
                items = lstItems.Select(x => x.CardCode);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        public IActionResult FilterMails(string Filter)
        {
            string message = "";
            try
            {
                BCS.MailBlacklist bcBlacklist = new();
                List<Field> filters = new() { new Field("EMail", Filter, Operators.Likes) };
                var lstItems = bcBlacklist.List(filters, "EMail");
                string userCodes = string.Join(",", lstItems.Select(x => x.LogUser));
                BCS.User bcUser = new();
                filters = new List<Field> { new Field("Id", userCodes, Operators.In) };
                IEnumerable<BES.User> lstUsers = lstItems?.Count() > 0 ? bcUser.List(filters, "1") : new List<BES.User>();
                var items = (from i in lstItems
                             join u in lstUsers on i.LogUser equals u.Id into g
                             from lu in g.DefaultIfEmpty()
                             select new { i.EMail, UserName = lu?.Name ?? "", Date = i.LogDate }).ToList();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public IActionResult AddMailBlacklist(string EMail)
        {
            string message = "", error = "N";
            try
            {
                EMail = EMail.Trim();
                BCS.MailBlacklist bcBlacklist = new();
                List<Field> filters = new() { new Field("EMail", EMail) };
                var list = bcBlacklist.List(filters, "1");
                if (list?.Count() > 0)
                {
                    message = "Ese correo electrónico ya está registrado en la lista negra.";
                }
                else
                {
                    BES.MailBlacklist item = new() { EMail = EMail, LogUser = UserCode, LogDate = DateTime.Now, StatusType = StatusType.Insert };
                    bcBlacklist.Save(ref item);
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
                error = "Y";
            }
            return Json(new { message, error });
        }

        [HttpPost()]
        public IActionResult SaveClients(IEnumerable<string> Clients)
        {
            string message = "";
            try
            {
                List<string> lstNew = new();
                List<BES.BlackList> lstDelete = new();
                BCS.BlackList bcBlackList = new();
                IEnumerable<BES.BlackList> lstItems = bcBlackList.List("1");
                lstNew = (from c in Clients where !lstItems.Select(x => x.CardCode).Contains(c) select c).ToList();
                lstDelete = (from a in lstItems where !Clients.Contains(a.CardCode) select a).ToList();
                long intUserID = UserCode;

                IList<BES.BlackList> lstBlackList = new List<BES.BlackList>();
                foreach (string code in lstNew)
                {
                    lstBlackList.Add(new BES.BlackList { StatusType = StatusType.Insert, CardCode = code, LogDate = DateTime.Now, LogUser = intUserID });
                }
                foreach (BES.BlackList item in lstDelete)
                {
                    item.StatusType = StatusType.Delete;
                    lstBlackList.Add(item);
                }
                if (lstBlackList.Count > 0)
                {
                    bcBlackList.Save(ref lstBlackList);
                }
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