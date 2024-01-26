using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Portal.Controllers;
using BCF = BComponents.Staff;
using BCP = BComponents.Product;
using BEF = BEntities.Staff;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class LinesController : BaseController
    {
        #region Constructores

        public LinesController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            short intPermission = GetPermission("Lineas");
            if (IsAllowed(this))
            {
                ViewData["Permission"] = intPermission;
                IEnumerable<BEP.Line> lstLines = GetItems("");
                return View(lstLines);
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetStaff()
        {
            IEnumerable<BEF.Member> members = new List<BEF.Member>();
            try
            {
                BCF.Member bcMember = new();
                members = bcMember.List("Name");
            }
            catch (Exception) { }
            var items = members.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult Edit(int id = -1)
        {
            if (id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Models.Line beLine = new();
                if (id > 0)
                {
                    BCP.Line bcLine = new();
                    BEP.Line Line = bcLine.Search(id);
                    BCP.LineDetail bcDetail = new();
                    IEnumerable<BEP.LineDetail> lstItems = bcDetail.List(new List<Field> { new Field("IdLine", id) }, "1");
                    beLine = new Models.Line(Line) { SAPLines = lstItems.Select(x => x.SAPLine).ToList() };
                }
                return PartialView(beLine);
            }
        }

        public IActionResult Filter(string filterData)
        {
            string strMessage = "";
            IEnumerable<BEP.Line> lstItems = Enumerable.Empty<BEP.Line>();
            try
            {
                lstItems = GetItems(filterData);
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            return Json(new { Message = strMessage, Items = lstItems });
        }

        public async Task<IActionResult> UploadImage(IEnumerable<IFormFile> files)
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
                        var physicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "lines", fileName);
                        using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                }
            }
            catch (Exception) { }
            // Return an empty string to signify success
            return Content("");
        }

        public IActionResult DeleteImage(string[] fileNames)
        {
            foreach (var strFileName in fileNames)
            {
                string strPhysicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "lines", strFileName);
                if (System.IO.File.Exists(strPhysicalPath))
                {
                    System.IO.File.Delete(strPhysicalPath);
                }
            }
            return Content("");
        }

        public IActionResult GetSAPLines(long? LineId)
        {
            IEnumerable<string> lstLines = new List<string>();
            try
            {
                BCP.Product bcProduct = new();
                lstLines = bcProduct.ListSAPLines(LineId);
            }
            catch (Exception)
            {
            }
            return Json(lstLines.Select(x => new { Id = x, Name = x }));
        }

        #endregion

        #region POSTs

        // POST: Administration/Profile/Edit/5
        [HttpPost()]
        public IActionResult Edit(Models.Line Line)
        {
            string strMessage = "";
            IEnumerable<BEP.Line> lstItems = Enumerable.Empty<BEP.Line>();
            try
            {
                DateTime logDate = DateTime.Now;
                BCP.Line bcLine = new();
                BEP.Line beLine = Line.ToEntity(UserCode, logDate);
                beLine.StatusType = Line.Id > 0 ? StatusType.Update : StatusType.Insert;

                BCP.LineDetail bcDetail = new();
                IEnumerable<BEP.LineDetail> lstOlds = bcDetail.List(new List<Field> { new Field("IdLine", Line.Id) }, "1");
                if (Line.SAPLines?.Count > 0 || lstOlds?.Count() > 0)
                {
                    foreach (var item in lstOlds)
                    {
                        if (Line.SAPLines == null || !Line.SAPLines.Contains(item.SAPLine))
                        {
                            beLine.ListLineDetails.Add(new BEP.LineDetail { Id = item.Id, LogDate = logDate, StatusType = StatusType.Delete });
                        }
                    }
                    if (Line.SAPLines?.Count > 0)
                    {
                        foreach (var item in Line.SAPLines)
                        {
                            if ((from o in lstOlds select o.SAPLine).Contains(item) == false)
                            {
                                beLine.ListLineDetails.Add(new BEP.LineDetail { StatusType = StatusType.Insert, SAPLine = item, LogUser = UserCode, LogDate = logDate });
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(Line?.NewImageURL))
                {
                    beLine.ImageURL = Line.NewImageURL;
                    DeleteImage(Line.ImageURL); //Borramos el anterior archivo si existiece
                }
                else
                {
                    beLine.ImageURL = Line.ImageURL;
                }
                bcLine.Save(ref beLine);
                lstItems = GetItems("");
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            var beResult = new { Message = strMessage, Items = lstItems };
            return Json(beResult);
        }

        // POST: Administration/Profile/Delete/5
        [HttpPost()]
        public IActionResult Delete(int id)
        {
            string strMessage = "";
            IEnumerable<BEP.Line> lstItems = Enumerable.Empty<BEP.Line>();
            try
            {
                BEP.Line beLine = new() { Id = id, StatusType = StatusType.Delete, LogDate = DateTime.Now };
                BCP.Line bcLine = new();
                bcLine.Save(ref beLine);
                lstItems = GetItems("");
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            var beResult = new { Message = strMessage, Items = lstItems };
            return Json(beResult);
        }

        [HttpPost()]
        public IActionResult DeleteImg(string ImgName)
        {
            string strMessage = "";
            try
            {
                DeleteImage(ImgName);
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            var beResult = new { Message = strMessage };
            return Json(beResult);
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEP.Line> GetItems(string filter)
        {
            BCP.Line bcLine = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                lstFilter.AddRange(new[] { new Field("Name", filter, Operators.Likes), new Field("Description", filter, Operators.Likes), new Field(LogicalOperators.Or) });
            }
            IEnumerable<BEP.Line> lstItems = bcLine.ListExtended(lstFilter, "Name");
            return lstItems;
        }

        private void DeleteImage(string FileName)
        {
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                var physicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "lines", FileName);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
        }

        #endregion
    }
}