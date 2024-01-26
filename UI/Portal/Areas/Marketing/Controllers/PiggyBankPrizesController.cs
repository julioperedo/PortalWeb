using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Portal.Controllers;
using BCY = BComponents.PiggyBank;
using BEY = BEntities.PiggyBank;
using System.Linq;
using BEntities;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class PiggyBankPrizesController : BaseController
    {

        #region Constructors

        public PiggyBankPrizesController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Filter()
        {
            string message = "";
            try
            {
                BCY.Prizes bcPrizes = new();
                var prizes = bcPrizes.List("1");
                var items = prizes.Select(x => new { x.Id, x.Name, x.Description, x.ImageUrl, x.Points, x.Enabled });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult RemoveImage(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            foreach (var strFileName in fileNames)
            {
                string strPhysicalPath = Path.Combine(rootDirectory, "wwwroot", "files", "product-docs", strFileName);
                if (System.IO.File.Exists(strPhysicalPath))
                    System.IO.File.Delete(strPhysicalPath);
            }
            return Content("");
        }

        public async Task<IActionResult> SaveImage(IEnumerable<IFormFile> files)   
        {
            // The Name of the Upload component is "files"
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                        var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                        var physicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "piggybank", "prizes", fileName);
                        using var fileStream = new FileStream(physicalPath, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception) { }            
            return Content("");  // Return an empty string to signify success
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BEY.Prizes Item)
        {
            string message = "";
            try
            {
                BCY.Prizes bcPrizes = new();
                Item.StatusType = Item.Id > 0 ? StatusType.Update : StatusType.Insert;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;

                bcPrizes.Save(ref Item);
                return Json(new { message, id = Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            string message = "";
            try
            {
                BEY.Prizes Item = new() { Id = Id, LogDate = DateTime.Now, StatusType = StatusType.Delete };
                BCY.Prizes bcPrize = new();
                bcPrize.Save(ref Item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

    }
}
