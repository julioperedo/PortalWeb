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
using BCB = BComponents.Base;
using BEB = BEntities.Base;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class ClassifiersController : BaseController
    {
        #region Constructores

        public ClassifiersController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewData["Permission"] = GetPermission("Clasificadores");
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(string FilterData)
        {
            string message = "";
            IEnumerable<BEB.ClassifierType> items = new List<BEB.ClassifierType>();
            try
            {
                items = GetItems(FilterData);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var beResult = new { message, items };
            return Json(beResult);
        }

        // GET: Base/Classifier/Edit/5
        public IActionResult Edit(int Id)
        {
            BCB.ClassifierType bcType = new BCB.ClassifierType();
            BEB.ClassifierType beType = bcType.Search(Id, BEB.relClassifierType.Classifiers) ?? new BEB.ClassifierType { ListClassifiers = new List<BEB.Classifier>() };
            return PartialView(beType);
        }

        #endregion

        #region POSTs

        //POST: Base/Classifier/Edit/5
        [HttpPost()]
        public ActionResult Edit(BEB.ClassifierType Item, string FilterData)
        {
            string message = "";
            try
            {
                DateTime logDate = DateTime.Now;
                Item.StatusType = Item.Id == 0 ? StatusType.Insert : StatusType.Update;
                Item.LogDate = logDate;
                Item.LogUser = UserCode;
                if(Item.ListClassifiers?.Count > 0)
                {
                    foreach (var item in Item.ListClassifiers)
                    {
                        item.LogUser = UserCode;
                        item.LogDate = logDate;
                    }
                }                

                BCB.ClassifierType bcType = new();
                bcType.Save(ref Item);

                var items = GetItems(FilterData);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        // POST: Base/Classifier/Delete/5
        [HttpPost()]
        public ActionResult Delete(int Id, string FilterData)
        {
            string message = "";
            IEnumerable<BEB.ClassifierType> items = new List<BEB.ClassifierType>();
            try
            {
                BEB.ClassifierType beClassifierType = new() { Id = Id, LogDate = DateTime.Now, StatusType = StatusType.Delete };
                BCB.ClassifierType bcClassifierType = new();
                bcClassifierType.Save(ref beClassifierType);
                items = GetItems(FilterData);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEB.ClassifierType> GetItems(string FilterData)
        {
            BCB.ClassifierType bcType = new();
            List<Field> lstFilters = new();
            if (!string.IsNullOrWhiteSpace(FilterData))
            {
                lstFilters.AddRange(new[] { new Field("Name", FilterData, Operators.Likes), new Field("Description", FilterData, Operators.Likes), new Field(LogicalOperators.Or) });
            }
            IEnumerable<BEB.ClassifierType> items = bcType.List(lstFilters, "1");
            return items;
        }

        #endregion
    }
}