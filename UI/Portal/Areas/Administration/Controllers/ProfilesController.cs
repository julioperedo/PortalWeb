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
using BCS = BComponents.Security;
using BEB = BEntities.Base;
using BEM = Portal.Areas.Administration.Models;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class ProfilesController : BaseController
    {
        #region Constructores

        public ProfilesController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewData["Permission"] = GetPermission("Perfiles");
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        // GET: Administration/Profile/Edit/5
        public IActionResult Edit(int Id = -1)
        {
            if (Id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                BES.Profile profile;
                if (Id == 0)
                {
                    profile = new BES.Profile { ListProfileActivitys = new List<BES.ProfileActivity>(), ListProfilePages = new List<BES.ProfilePage>(), ListProfileCharts = new List<BES.ProfileChart>() };
                }
                else
                {
                    BCS.Profile bcProfile = new BCS.Profile();
                    profile = bcProfile.Search(Id);
                }
                BCS.ProfileActivity bcActivity = new BCS.ProfileActivity();
                profile.ListProfileActivitys = bcActivity.ListByProfile(Id, "1, 2");
                BCS.ProfilePage bcPage = new BCS.ProfilePage();
                profile.ListProfilePages = bcPage.ListByProfile(Id, "1, 2");
                BCS.ProfileChart bcChart = new BCS.ProfileChart();
                profile.ListProfileCharts = bcChart.ListByProfile(Id, "1, 2");
                return PartialView(profile);
            }
        }

        public IActionResult Filter(string FilterData)
        {
            string message = "";
            List<BEM.Profile> items = new List<BEM.Profile>();
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

        #endregion

        #region POSTs

        // POST: Administration/Profile/Edit/5
        [HttpPost()]
        public ActionResult Edit(BES.Profile Item, string FilterData)
        {
            string message = "";
            List<BEM.Profile> items = new List<BEM.Profile>();
            DateTime objDate = DateTime.Now;
            try
            {
                BCS.Profile bcProfile = new BCS.Profile();
                Item.LogDate = objDate;
                Item.LogUser = UserCode;
                Item.StatusType = Item.Id > 0 ? StatusType.Update : StatusType.Insert;
                if (Item.ListProfileActivitys != null)
                {
                    foreach (var activity in Item.ListProfileActivitys)
                    {
                        activity.LogUser = UserCode;
                        activity.LogDate = objDate;
                        activity.Permission = 0;
                        if (activity.Insert) activity.Permission += 1;
                        if (activity.Update) activity.Permission += 2;
                        if (activity.Delete) activity.Permission += 4;
                        //beActivity.StatusType = beActivity.Id > 0 ? StatusType.Update : StatusType.Insert;
                    }
                }
                if (Item.ListProfilePages != null)
                {
                    foreach (var page in Item.ListProfilePages)
                    {
                        page.LogUser = UserCode;
                        page.LogDate = objDate;
                    }
                }
                if (Item.ListProfileCharts != null)
                {
                    foreach (var chart in Item.ListProfileCharts)
                    {
                        chart.LogUser = UserCode;
                        chart.LogDate = objDate;
                    }
                }
                bcProfile.Save(ref Item);
                items = GetItems(FilterData);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var beResult = new { message, items };
            return Json(beResult);
        }

        // POST: Administration/Profile/Delete/5
        [HttpPost()]
        public ActionResult Delete(int Id, string filterData)
        {
            string message = "";
            List<BEM.Profile> items = new List<BEM.Profile>();
            DateTime objDate = DateTime.Now;
            try
            {
                BES.Profile beProfile = new BES.Profile { Id = Id, StatusType = StatusType.Delete, LogDate = objDate };
                BCS.Profile bcProfile = new BCS.Profile();
                bcProfile.Save(ref beProfile);
                items = GetItems(filterData);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var beResult = new { message, items };
            return Json(beResult);
        }

        #endregion

        #region Private Methods

        private List<BEM.Profile> GetItems(string filterData)
        {
            BCS.Profile bcProfile = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(filterData))
            {
                lstFilter.AddRange(new[] { new Field("Name", filterData, Operators.Likes), new Field("Description", filterData, Operators.Likes), new Field(LogicalOperators.Or) });
            }
            IEnumerable<BES.Profile> lstItems = bcProfile.List(lstFilter, "1");
            List<BEM.Profile> profiles = lstItems.Select(x => new BEM.Profile { Id = x.Id, Description = x.Description, Name = x.Name, Type = x.IsExternalCapable ? "Para Clientes" : "Para Personal" }).ToList();
            return profiles;
        }

        #endregion
    }
}