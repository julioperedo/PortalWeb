using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Areas.Product.Models;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class WarehousesController : BaseController
    {

        #region Constructores

        public WarehousesController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            short intPermission = GetPermission("Almacenes");
            if (IsAllowed(this))
            {
                ViewData["Permission"] = intPermission;
                BCA.Warehouse bcStorehouse = new();
                IEnumerable<BEA.Item> lstStoreHouses = bcStorehouse.List();

                BCP.WarehouseAllowed bcWarehouse = new();
                IEnumerable<BEP.WarehouseAllowed> lstWarehouses = bcWarehouse.List("Name");
                var lstResult = from s in lstStoreHouses
                                group s by s.Parent into g
                                select new Subsidiary
                                {
                                    Name = g.Key,
                                    Warehouses = (from d in g
                                                  join w in lstWarehouses on new { w = d.Name.ToLower(), s = d.Parent.ToLower() } equals new { w = w.Name.ToLower(), s = w.Subsidiary.ToLower() } into ljWarehouses
                                                  from lj in ljWarehouses.DefaultIfEmpty()
                                                  select new AvailableWarehouse { Id = lj?.Id ?? 0, Name = ToTitle(d.Name), Code = d.Code, Selected = lj?.Id > 0, ClientVisible = lj?.ClientVisible ?? false }).ToList()
                                };
                return View(lstResult);
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public ActionResult Edit(List<BEP.WarehouseAllowed> items)
        {
            string strMessage = "";
            try
            {
                BCP.WarehouseAllowed bcWharehouse = new();
                IEnumerable<BEP.WarehouseAllowed> lstWharehouses = bcWharehouse.List("Name");
                foreach (var item in items)
                {
                    item.StatusType = item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                    item.LogDate = DateTime.Now;
                    item.LogUser = UserCode;
                }
                foreach (var item in lstWharehouses)
                {
                    if (!items.Select(x => x.Id).Contains(item.Id))
                    {
                        item.StatusType = BEntities.StatusType.Delete;
                        items.Add(item);
                    }
                }
                IList<BEP.WarehouseAllowed> lstItems = items.Where(x => x.StatusType != BEntities.StatusType.NoAction).ToList();
                bcWharehouse.Save(ref lstItems);
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            var beResult = new { Message = strMessage };
            return Json(beResult);
        }

        #endregion
    }
}