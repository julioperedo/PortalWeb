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
using BCB = BComponents.Base;
using BEB = BEntities.Base;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class EventsController : BaseController
    {
        #region Constructores

        public EventsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public IActionResult Next(int Initial)
        {
            BCB.Event bcEvent = new BCB.Event();
            List<BEB.Event> lstItems = bcEvent.List(Initial, Initial + 10, "Date DESC");
            Models.EventsPage bePage = new Models.EventsPage
            {
                Next = lstItems.Count == 11 ? (Initial + 10) : 0,
                Items = (from i in lstItems select new Models.Event { Id = i.Id, Name = i.Name, Description = i.Description, Date = i.Date, Detail = !string.IsNullOrWhiteSpace(i.Detail) }).Take(10).ToList()
            };
            return Json(bePage);
        }

        public IActionResult Detail(long id)
        {
            BCB.Event bcEvent = new BCB.Event();
            BEB.Event beItem = bcEvent.Search(id);
            Models.Event beEvent = new Models.Event { Id = beItem.Id, Name = beItem.Name, Description = SetHTMLSafe(beItem.Detail), Date = beItem.Date };
            return PartialView(beEvent);
        }

        public IActionResult Admin()
        {
            IEnumerable<BEB.Event> lstItems = GetItems(DateTime.Today.AddDays(-10), null);
            return View(lstItems);
        }

        public IActionResult Filter(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            IEnumerable<BEB.Event> items = new List<BEB.Event>();
            try
            {
                items = GetItems(InitialDate, FinalDate);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        public IActionResult Edit(long Id = -1)
        {
            if (Id >= 0)
            {
                BEB.Event beItem;
                if (Id == 0)
                {
                    beItem = new BEB.Event { Date = DateTime.Today, LogDate = DateTime.Now };
                }
                else
                {
                    BCB.Event bcEvent = new BCB.Event();
                    beItem = bcEvent.Search(Id);
                    if (!string.IsNullOrWhiteSpace(beItem.Detail))
                    {
                        beItem.Detail = SetHTMLSafe(beItem.Detail);
                    }
                }
                return PartialView(beItem);
            }
            else
            {
                return RedirectToAction("Admin");
            }
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public IActionResult Delete(long Id, DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            IEnumerable<BEB.Event> items = new List<BEB.Event>();
            try
            {
                BCB.Event bcEvent = new BCB.Event();
                BEB.Event beItem = new BEB.Event { Id = Id, StatusType = BEntities.StatusType.Delete, Date = DateTime.Now, LogDate = DateTime.Now };
                bcEvent.Save(ref beItem);
                items = GetItems(InitialDate, FinalDate);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        [HttpPost()]
        public IActionResult Edit(BEB.Event Item, DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            IEnumerable<BEB.Event> items = new List<BEB.Event>();
            try
            {
                BCB.Event bcEvent = new BCB.Event();
                Item.StatusType = Item.Id > 0 ? BEntities.StatusType.Update : BEntities.StatusType.Insert;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;
                bcEvent.Save(ref Item);
                items = GetItems(InitialDate, FinalDate);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEB.Event> GetItems(DateTime? InitialDate, DateTime? FinalDate)
        {
            BCB.Event bcEvent = new();
            List<Field> lstFilter = new();
            if (InitialDate.HasValue)
            {
                lstFilter.Add(new Field("CAST(Date AS DATE)", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
            }
            if (FinalDate.HasValue)
            {
                lstFilter.Add(new Field("CAST(Date AS DATE)", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                if (lstFilter.Count > 1)
                {
                    lstFilter.Add(new Field(LogicalOperators.And));
                }
            }
            IEnumerable<BEB.Event> lstItems = bcEvent.List(lstFilter, "Date");
            return lstItems;
        }

        #endregion
    }
}