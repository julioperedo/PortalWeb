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
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class LinesNotAllowedController : BaseController
    {
        #region Constructores

        public LinesNotAllowedController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public IActionResult Detail(string CardCode)
        {
            string message = "";
            try
            {
                BCS.LineNotAllowed bcClient = new();
                List<BES.LineNotAllowed> lstLines = bcClient.ListSelected(CardCode, "l.Name");
                lstLines.Insert(0, new BES.LineNotAllowed { IdLine = 0, LineName = "TODOS" });
                return Json(new { message, items = lstLines });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetFilteredClients(bool All = true)
        {
            BCS.LineNotAllowed bcNotAllowed = new();
            List<BES.LineNotAllowed> lstNotAllowed = bcNotAllowed.ListClients();

            BCA.Client bcClients = new();
            IEnumerable<BEA.Client> lstClients = bcClients.List(new List<Field>(), "CardName");

            var lstItems = from c in lstClients
                           where (All || (from n in lstNotAllowed select n.CardCode).Contains(c.CardCode))
                           select new { c.CardCode, CardName = $"{c.CardCode} - {c.CardName}" };
            return Json(lstItems);
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public ActionResult Edit(string CardCode, List<long> Codes)
        {
            string strMessage = "";
            try
            {
                IList<BES.LineNotAllowed> lstLines, lstFinal = new List<BES.LineNotAllowed>();
                BCS.LineNotAllowed bcClient = new();
                List<Field> lstFilter = new() { new Field("CardCode", CardCode) };
                lstLines = bcClient.List(lstFilter, "1")?.ToList() ?? new List<BES.LineNotAllowed>();
                if (Codes == null) Codes = new List<long>();
                DateTime objDate = DateTime.Now;
                foreach (var item in (from l in lstLines where !Codes.Contains(l.IdLine) select l).ToList())
                {
                    item.StatusType = BEntities.StatusType.Delete;
                    item.LogUser = UserCode;
                    item.LogDate = objDate;
                    lstFinal.Add(item);
                }
                foreach (var item in (from c in Codes where !(from l in lstLines select l.IdLine).Contains(c) select c).ToList())
                {
                    lstFinal.Add(new BES.LineNotAllowed { StatusType = BEntities.StatusType.Insert, CardCode = CardCode, IdLine = item, LogUser = UserCode, LogDate = objDate });
                }
                bcClient.Save(ref lstFinal);
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