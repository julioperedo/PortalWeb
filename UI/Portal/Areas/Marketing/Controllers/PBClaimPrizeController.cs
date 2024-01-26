using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCI = BComponents.PiggyBank;
using BEI = BEntities.PiggyBank;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class PBClaimPrizeController : BaseController
    {
        #region Constructors

        public PBClaimPrizeController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

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

        public IActionResult Edit(int Id = -1)
        {
            if (Id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                string message = "";
                BEI.ClaimedPrize item = default;
                try
                {
                    if (Id == 0)
                    {
                        item = new BEI.ClaimedPrize();
                    }
                    else
                    {
                        BCI.ClaimedPrize bc = new();
                        item = bc.Search(Id);
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
                var items = departments.Select(x => new { x.Id, x.ClaimDate, x.IdPrize, Prize = x.Prize.Name, x.IdUser, User = x.User.Name, x.Quantity, x.Points });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetPrizes()
        {
            BCI.Prizes bc = new();
            var claims = bc.List("1");
            var items = from i in claims where i.Enabled select new { i.Id, i.Name, i.Points, i.ImageUrl };
            return Json(items);
        }

        public IActionResult GetUsers()
        {
            BCI.User bc = new();
            var users = bc.ListResume("1");
            var items = users.Select(i => new { i.Id, i.Name, i.Points });
            return Json(items);
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public async Task<ActionResult> EditAsync(BEI.ClaimedPrize Item)
        {
            string message = "";
            try
            {
                BCI.ClaimedPrize bc = new();
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

        [HttpPost()]
        public async Task<IActionResult> DeleteAsync(long Id)
        {
            string message = "";
            try
            {
                BCI.ClaimedPrize bc = new();
                BEI.ClaimedPrize item = new() { Id = Id, LogDate = DateTime.Now, ClaimDate = DateTime.Now, StatusType = StatusType.Delete };

                await bc.SaveAsync(item);
                return Json(new { message });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEI.ClaimedPrize> GetItems(string filterData)
        {
            BCI.ClaimedPrize bc = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(filterData))
            {
                lstFilter.Add(Field.New("Name", filterData, Operators.Likes));
            }
            IEnumerable<BEI.ClaimedPrize> items = bc.List(lstFilter, "1", BEI.relClaimedPrize.User, BEI.relClaimedPrize.Prize);
            return items;
        }



        #endregion
    }
}
