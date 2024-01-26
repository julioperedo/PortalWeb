using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System.Collections.Generic;
using System;
using System.IO;
using BCA = BComponents.SAP;
using BCH = BComponents.HumanResources;
using BEA = BEntities.SAP;
using BEH = BEntities.HumanResources;
using BEntities.Filters;
using System.Linq;
using BEntities;
using System.Threading.Tasks;

namespace Portal.Areas.HumanResources.Controllers
{
    [Area("HumanResources")]
    [Authorize]
    public class EmployeeController : BaseController
    {
        #region Gobal Variables

        private readonly IWebHostEnvironment _env;

        #endregion

        #region Constructors

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment hEnviroment) : base(configuration, hEnviroment)
        {
            _env = hEnviroment;
        }

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

        public IActionResult Edit(long Id = -1)
        {
            BCH.Employee bcEmployee = new();
            List<Field> filters = new() { Field.New("Id", Id, Operators.Different) };
            var lstManagers = bcEmployee.List(filters, "Name");
            var managers = lstManagers.Select(x => new { x.Id, x.Name, x.Enabled });

            BCH.EmployeeDepartment bc = new();
            filters = new() { Field.New("IdEmployee", Id) };
            var lstItems = bc.List(filters, "1", BEH.relEmployeeDepartment.Department, BEH.relEmployeeDepartment.Manager);
            var items = lstItems.Select(x => new { x.Id, x.IdDepartment, Department = x.Department.Name, x.IdManager, Manager = x.Manager.Name, x.StatusType });

            return Json(new { managers, items });
        }

        public IActionResult Filter(string FilterData)
        {
            string message = "";
            try
            {
                var items = GetItems(FilterData);
                //var items = employees.Select(x => new { x.Id, x.Name, x.Position, x.Photo, x.Enabled});
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetDepartments()
        {
            BCH.Department bc = new();
            IEnumerable<BEH.Department> lstDepartments = bc.List("1");
            var items = lstDepartments.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetManagers(long IdEmployee)
        {
            BCH.Employee bcEmployee = new();
            List<Field> filters = new() { Field.New("Id", IdEmployee, Operators.Different) };
            var managers = bcEmployee.List(filters, "Name");
            var items = managers.Select(x => new { x.Id, x.Name, x.Enabled });
            return Json(items);
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public async Task<ActionResult> EditAsync(BEH.Employee Item, List<BEH.EmployeeDepartment> Departments)
        {
            string message = "";
            try
            {
                BCH.Employee bc = new();
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;
                Item.StatusType = Item.Id > 0 ? StatusType.Update : StatusType.Insert;
                foreach (var item in Departments)
                {
                    item.LogUser = UserCode;
                    item.LogDate = DateTime.Now;
                }
                Item.ListEmployeeDepartment_Employees = Departments;

                await bc.SaveAsync(Item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost()]
        public IActionResult Sync()
        {
            var message = "";
            try
            {
                BCA.Employee bcEmpSAP = new();
                var sapEmployees = bcEmpSAP.List(null, "1");

                BCH.Employee bcEmp = new();
                var employees = bcEmp.List("1");

                IList<BEH.Employee> items = new List<BEH.Employee>();

                foreach (var e in sapEmployees)
                {
                    var emp = employees.FirstOrDefault(x => x.Id == e.Id);
                    if (emp != null)
                    {
                        emp.Name = e.Name;
                        emp.Enabled = e.IsAvailable;
                        emp.Position = e.Position;
                        emp.ShortName = e.ShortName;
                        emp.Phone = string.IsNullOrEmpty(e.Phone) ? null : int.Parse(e.Phone);
                        emp.Mail = e.Email;
                        emp.StatusType = StatusType.Update;
                    }
                    else
                    {
                        emp = new()
                        {
                            Id = e.Id,
                            Name = e.Name,
                            Position = e.Position,
                            Enabled = e.IsAvailable,
                            ShortName = e.ShortName,
                            Phone = string.IsNullOrEmpty(e.Phone) ? null : int.Parse(e.Phone),
                            Mail = e.Email,
                            StatusType = StatusType.Insert
                        };
                        if (!string.IsNullOrEmpty(e.Picture))
                        {
                            try
                            {
                                string fileName = e.Picture.Replace(" ", "-"), fullPath = Path.Combine(_env.WebRootPath, "images", "humanresources", fileName);
                                System.IO.File.Copy(e.Path + e.Picture, fullPath);
                                emp.Photo = fileName;
                            }
                            catch (Exception) { }
                        }
                    }
                    emp.LogUser = UserCode;
                    emp.LogDate = DateTime.Now;

                    items.Add(emp);
                }

                bcEmp.Save(ref items);

                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEH.Employee> GetItems(string filterData)
        {
            BCH.Employee bc = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(filterData))
            {
                lstFilter.Add(Field.New("Name", filterData, Operators.Likes));
            }
            IEnumerable<BEH.Employee> items = bc.List(lstFilter, "1");
            return items;
        }

        #endregion
    }
}
