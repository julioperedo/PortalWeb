using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System.Collections.Generic;
using System;
using BCH = BComponents.HumanResources;
using BEH = BEntities.HumanResources;
using BEntities.Filters;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using BEntities;
using System.Threading.Tasks;

namespace Portal.Areas.HumanResources.Controllers
{
    [Area("HumanResources")]
    [Authorize]
    public class DepartmentController : BaseController
    {
        #region Constructors

        public DepartmentController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                //ViewData["Permission"] = GetPermission("Perfiles");
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Edit(int Id = -1)
        {
            if (Id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                string message = "";
                BEH.Department item = default;
                try
                {
                    if (Id == 0)
                    {
                        item = new BEH.Department {  };
                    }
                    else
                    {
                        BCH.Department bcDepartment = new();
                        //item = bcDepartment.Search(Id, BEH.relDepartment.Employees);
                    }
                }
                catch (Exception ex)
                {
                    message = GetError(ex);
                }
                return Json(new { message, item });
            }
        }

        public IActionResult Filter(string FilterData)
        {
            string message = "";
            try
            {
                var departments = GetItems(FilterData);
                var items = departments.Select(x => new { x.Id, x.Name, x.Enabled });
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
        public async Task<ActionResult> EditAsync(BEH.Department Item)
        {
            string message = "";
            try
            {
                BCH.Department bc = new();
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;
                Item.StatusType = Item.Id > 0 ? StatusType.Update : StatusType.Insert;

                long id = await bc.SaveAsync(Item);
                return Json(new { message, id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEH.Department> GetItems(string filterData)
        {
            BCH.Department bc = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(filterData))
            {
                lstFilter.Add(Field.New("Name", filterData, Operators.Likes));
            }
            IEnumerable<BEH.Department> items = bc.List(lstFilter, "1");
            return items;
        }

        #endregion
    }
}
