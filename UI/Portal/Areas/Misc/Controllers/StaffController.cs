using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BEF = BEntities.Staff;
using BCF = BComponents.Staff;
using Microsoft.AspNetCore.Hosting;

namespace Portal.Areas.Misc.Controllers
{
    [Area("Misc")]
    [Authorize]
    public class StaffController : BaseController
    {
        #region Constructores

        public StaffController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public IActionResult GetStaff()
        {
            BCF.Department bcDepartment = new();
            IEnumerable<BEF.Department> lstDepartments = bcDepartment.List("Name", BEF.relDepartment.Members);

            Models.Staff objStaff = new()
            {
                Departments = (from d in lstDepartments
                               orderby d.Order
                               select new Models.Department
                               {
                                   Name = d.Name,
                                   ClassName = d.Id == 1 ? "" : "managers",
                                   Managers = (from m in d.ListMembers
                                               where m.Manager
                                               orderby m.Order
                                               select new Models.Contact { Name = m.Name, Position = m.Position, Mail = m.Mail, Photo = m.Photo, Phone = m.Phone }).ToList(),
                                   Members = (from m in d.ListMembers
                                              where !m.Manager
                                              orderby m.Order
                                              select new Models.Contact { Name = m.Name, Position = m.Position, Mail = m.Mail, Photo = m.Photo, Phone = m.Phone }).ToList()
                               }).ToList()
            };
            return Json(objStaff);
        }

        #endregion

        #region Private Methods

        #endregion

    }
}