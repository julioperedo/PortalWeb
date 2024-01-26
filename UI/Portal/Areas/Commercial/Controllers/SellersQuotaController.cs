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
using BCL = BComponents.Sales;
using BEA = BEntities.SAP;
using BEL = BEntities.Sales;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class SellersQuotaController : BaseController
    {
        #region Constructores

        public SellersQuotaController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public IActionResult Filter(int Year, int Month)
        {
            string message = "";
            List<Models.SellersQuota> items = new List<Models.SellersQuota>();
            try
            {
                items = GetItems(Year, Month);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Save(List<BEL.SellersQuota> Items, int Year, int Month)
        {
            string message = "";
            List<Models.SellersQuota> items = new();
            try
            {
                long intUserID = UserCode;
                BCL.SellersQuota bcQuota = new();
                List<Field> lstFilter = new();
                IList<BEL.SellersQuota> lstFinal = new List<BEL.SellersQuota>();

                lstFilter = new List<Field> { new Field("[Year]", Year), new Field("[Month]", Month), new Field(LogicalOperators.And) };
                IEnumerable<BEL.SellersQuota> lstQuota = bcQuota.List(lstFilter, "2");

                foreach (var item in lstQuota)
                {
                    BEL.SellersQuota beQuota = (from i in Items where i.SalesmanCode == item.SalesmanCode & i.Amount > 0 select i).FirstOrDefault();
                    if (beQuota == null)
                    {
                        item.StatusType = BEntities.StatusType.Delete;
                        lstFinal.Add(item);
                    }
                }
                foreach (BEL.SellersQuota item in Items)
                {
                    BEL.SellersQuota beQuota = (from q in lstQuota where q.SalesmanCode == item.SalesmanCode select q).FirstOrDefault();
                    if (beQuota != null)
                    {
                        if (beQuota.Amount != item.Amount | beQuota.Commentaries != item.Commentaries)
                        {
                            if (item.Amount > 0)
                            {
                                beQuota.StatusType = BEntities.StatusType.Update;
                                beQuota.Amount = item.Amount;
                                beQuota.Commentaries = item.Commentaries;
                            }
                            else
                            {
                                beQuota.StatusType = BEntities.StatusType.Delete;
                            }
                            beQuota.LogUser = intUserID;
                            beQuota.LogDate = DateTime.Now;
                            lstFinal.Add(beQuota);
                        }
                    }
                    else
                    {
                        if (item.Amount > 0)
                        {
                            beQuota = new BEL.SellersQuota
                            {
                                StatusType = BEntities.StatusType.Insert,
                                Year = Year,
                                Month = Month,
                                SalesmanCode = item.SalesmanCode,
                                Amount = item.Amount,
                                Commentaries = item.Commentaries,
                                LogUser = intUserID,
                                LogDate = DateTime.Now
                            };
                            lstFinal.Add(beQuota);
                        }
                    }
                }
                bcQuota.Save(ref lstFinal);
                items = GetItems(Year, Month);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region Métodos Privados

        private List<Models.SellersQuota> GetItems(int Year, int Month)
        {
            List<Models.SellersQuota> lstItems = new();
            BCA.Seller bcSellers = new();
            BCL.SellersQuota bcQuota = new();
            List<Field> lstFilter = new() { new Field("Active", "Y") };
            IEnumerable<BEA.Seller> lstSellers = bcSellers.List(lstFilter, "2");

            lstFilter = new List<Field> { new Field("[Year]", Year), new Field("[Month]", Month), new Field(LogicalOperators.And) };
            IEnumerable<BEL.SellersQuota> lstQuota = bcQuota.List(lstFilter, "2");

            lstItems = (from s in lstSellers
                        join q in lstQuota on s.ShortName.ToLower() equals q.SalesmanCode.ToLower() into g
                        from lf in g.DefaultIfEmpty()
                        select new Models.SellersQuota { Id = lf?.Id ?? 0, Code = s.ShortName, Name = s.Name, Year = Year, Month = Month, Amount = lf?.Amount ?? 0, Commentaries = lf?.Commentaries ?? "" }).ToList();
            return lstItems;
        }

        #endregion
    }
}