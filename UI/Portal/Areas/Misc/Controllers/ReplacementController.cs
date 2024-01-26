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
using BCF = BComponents.Staff;
using BCS = BComponents.Security;
using BEF = BEntities.Staff;
using BES = BEntities.Security;

namespace Portal.Areas.Misc.Controllers
{
    [Area("Misc")]
    [Authorize]
    public class ReplacementController : BaseController
    {
        #region Constructores

        public ReplacementController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                int permission = GetPermission("Reemplazos");
                ViewData["Permission"] = permission;
                if (permission > 0)
                {
                    var items = GetItems(DateTime.Today, null, null, null);
                    return View("IndexComplete", items);
                }
                else
                {
                    Models.Replacement data = new();
                    string sellerCode = GetSellerCode();
                    if (!string.IsNullOrEmpty(sellerCode))
                    {
                        data.ValidSeller = true;
                        data.SellerCode = sellerCode;
                        data.Replacements = GetItems(DateTime.Today, null, sellerCode, null).ToList();
                    }
                    return View(data);
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(DateTime? InitialDate, DateTime? FinalDate, string SellerCode, string ReplacementCode)
        {
            string message = "";
            try
            {
                var items = GetItems(InitialDate, FinalDate, SellerCode, ReplacementCode);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Edit(int Id = -1)
        {
            if (Id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                BEF.Replace item;
                if (Id == 0)
                {
                    string sellerCode = GetSellerCode();
                    item = new BEF.Replace { InitialDate = DateTime.Today.AddDays(1), FinalDate = DateTime.Today.AddDays(3), SellerCode = sellerCode };
                }
                else
                {
                    BCF.Replace bcReplace = new BCF.Replace();
                    item = bcReplace.Search(Id);
                }
                return PartialView(item);
            }
        }

        #endregion

        #region POSTs

        [HttpPost]
        public ActionResult Edit(BEF.Replace Item, Models.ReplacementFilters Filters)
        {
            string message = "";
            try
            {
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;

                BCF.Replace bcReplace = new BCF.Replace();
                bcReplace.Save(ref Item);

                var items = GetItems(Filters.Initialdate, Filters.FinalDate, Filters.SellerCode, Filters.ReplacementCode);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public ActionResult Delete(Models.ReplacementFilters Filters, int Id = -1)
        {
            if (Id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                string message = "";
                try
                {
                    BEF.Replace item = new BEF.Replace { InitialDate = DateTime.Today, FinalDate = DateTime.Today, LogDate = DateTime.Today, Id = Id, StatusType = BEntities.StatusType.Delete };
                    BCF.Replace bcReplace = new BCF.Replace();
                    bcReplace.Save(ref item);

                    var items = GetItems(Filters.Initialdate, Filters.FinalDate, Filters.SellerCode, Filters.ReplacementCode);
                    return Json(new { message, items });
                }
                catch (Exception ex)
                {
                    message = GetError(ex);
                }
                return Json(new { message });
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEF.Replace> GetItems(DateTime? InitialDate, DateTime? FinalDate, string SellerCode, string ReplacementCode)
        {
            BCF.Replace bcReplace = new();
            List<Field> filters = new();
            if (InitialDate.HasValue)
            {
                filters.Add(new Field("FinalDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
            }
            if (FinalDate.HasValue)
            {
                filters.Add(new Field("InitialDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
            }
            if (!string.IsNullOrWhiteSpace(SellerCode))
            {
                filters.Add(new Field("SellerCode", SellerCode));
            }
            if (!string.IsNullOrWhiteSpace(ReplacementCode))
            {
                filters.Add(new Field("ReplaceCode", ReplacementCode));
            }
            CompleteFilters(ref filters);
            IEnumerable<BEF.Replace> items = bcReplace.List(filters, "1");
            if (items?.Count() > 0)
            {
                BCS.UserData bcData = new();
                List<string> codes = items.Select(x => x.ReplaceCode).Distinct().ToList();
                codes.AddRange(items.Select(x => x.SellerCode).Distinct());
                var replacementsData = bcData.List(new List<Field> { new Field("SellerCode", string.Join(",", codes.Select(x => $"'{x}'")), Operators.In) }, "1", BES.relUserData.User);
                foreach (var x in items)
                {
                    x.SellerName = (from s in replacementsData where s.SellerCode == x.SellerCode select ToTitle(s.User.Name)).FirstOrDefault() ?? "";
                    x.ReplacementName = (from s in replacementsData where s.SellerCode == x.ReplaceCode select ToTitle(s.User.Name)).FirstOrDefault() ?? "";
                }
            }
            return items;
        }

        private string GetSellerCode()
        {
            BCS.UserData bcData = new();
            BES.UserData userData = bcData.SearchByUser(UserCode);
            return userData?.SellerCode ?? "";
        }

        #endregion
    }
}