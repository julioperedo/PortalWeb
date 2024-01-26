using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Controllers;
using SixLabors.ImageSharp;
using BCD = BComponents.AppData;
using BCV = BComponents.Visits;
using BED = BEntities.AppData;
using BEV = BEntities.Visits;

namespace Portal.Areas.Visits.Controllers
{
    [Area("Visits")]
    [Authorize]
    public class VisitsController : BaseController
    {
        #region Variables Globales
        private readonly IWebHostEnvironment localEnv;
        #endregion

        #region Constructores

        public VisitsController(IConfiguration Configuration, IWebHostEnvironment env) : base(Configuration, env) { localEnv = env; }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed("Visits", "Visits", "Index"))
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
            var bcStaff = new BCD.StaffMembers();
            var localStaff = bcStaff.List("1");
            var lstItems = from d in localStaff where d.Active orderby d.FullName select new { d.Id, Name = d.FullName };
            return Json(lstItems);
        }

        public IActionResult Filter(string CardCode, string Visitor, long Staff, DateTime? Since, DateTime? Until, bool NotFinished)
        {
            string message = "";
            try
            {
                var items = GetItems(CardCode, Visitor, Staff, Since, Until, NotFinished);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Edit(long Id)
        {
            BEV.Visit beVisit;
            if (Id == 0)
            {
                beVisit = new BEV.Visit { InitialDate = DateTime.Now };
            }
            else
            {
                BCV.Visit bcVisit = new BCV.Visit();
                beVisit = bcVisit.Search2(Id, BEV.relVisit.Staff, BEV.relVisit.Visitor);
            }
            string form = beVisit.Editable ? "Edit" : "See";

            return PartialView(form, beVisit);
        }

        public IActionResult ExportExcel(string CardCode, string Visitor, long Staff, DateTime? Since, DateTime? Until, bool NotFinished)
        {
            var lstItems = GetItems(CardCode, Visitor, Staff, Since, Until, NotFinished);
            using ExcelPackage objExcel = new();
            ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
            FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

            wsMain.Name = "Visitas";
            wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
            wsMain.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            wsMain.Cells.Style.Font.Size = 9;

            List<string> filters = new();
            if (!string.IsNullOrEmpty(CardCode))
            {
                filters.Add($"Cliente: {CardCode}");
            }
            if (!string.IsNullOrEmpty(Visitor))
            {
                filters.Add($"Visitante: {Visitor}");
            }
            if (Staff != 0)
            {
                BCD.StaffMembers bcStaff = new();
                var beStaff = bcStaff.Search(Staff);
                filters.Add($"Visitado: {beStaff.FullName}");
            }
            if (Since.HasValue)
            {
                filters.Add($"Desde: {Since.Value:dd-MM-yyyy}");
            }
            if (Until.HasValue)
            {
                filters.Add($"Hasta: {Until.Value:dd-MM-yyyy}");
            }

            wsMain.Cells[1, 1].Value = "CONSULTA DE VISITAS";
            wsMain.Cells[1, 1].Style.Font.Size = 12;
            wsMain.Cells[1, 1].Style.Font.Bold = true;
            wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[1, 1, 1, 13].Merge = true;
            wsMain.Cells[4, 1, 4, 13].Merge = true;
            wsMain.Cells[4, 1].Value = string.Join(", ", filters.ToArray());

            var imgLogo = wsMain.Drawings.AddPicture("logo", logo);
            imgLogo.SetPosition(5, 5);

            wsMain.Cells[6, 1].Value = "Cliente";
            wsMain.Cells[6, 2].Value = "Nombre";
            wsMain.Cells[6, 3].Value = "Doc. Identidad";
            wsMain.Cells[6, 4].Value = "Personal Visitado";
            wsMain.Cells[6, 5].Value = "Desde";
            wsMain.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[6, 6].Value = "Hasta";
            wsMain.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsMain.Cells[6, 1, 6, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells[6, 1, 6, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
            wsMain.Cells[6, 1, 6, 6].Style.Font.Color.SetColor(System.Drawing.Color.White);
            wsMain.Cells[6, 1, 6, 6].Style.Font.Bold = true;

            int intFila = 7;
            if (lstItems?.Count() > 0)
            {
                foreach (var item in lstItems)
                {
                    wsMain.Cells[intFila, 1].Value = item.ClientName;
                    wsMain.Cells[intFila, 2].Value = item.Visitor.FullName;
                    wsMain.Cells[intFila, 3].Value = item.Visitor.DocumentId;
                    wsMain.Cells[intFila, 4].Value = item.Staff.FullName;
                    wsMain.Cells[intFila, 5].Value = item.InitialDate;
                    wsMain.Cells[intFila, 5].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    wsMain.Cells[intFila, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsMain.Cells[intFila, 6].Value = item.FinalDate;
                    wsMain.Cells[intFila, 6].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                    wsMain.Cells[intFila, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    intFila++;
                }
            }
            wsMain.Column(1).Width = 25;
            wsMain.Column(2).Width = 25;
            wsMain.Column(3).Width = 12;
            wsMain.Column(4).Width = 32;
            wsMain.Column(5).Width = 18;
            wsMain.Column(6).Width = 18;

            //wsMain.Cells.Style.WrapText = true;
            //wsMain.Cells.AutoFitColumns();

            wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            wsMain.View.FreezePanes(7, 1);
            wsMain.PrinterSettings.LeftMargin = 0.2m;
            wsMain.PrinterSettings.RightMargin = 0.2m;
            wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:6"];
            wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:F"];

            string strFileName = "Listado-Visitas-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
            byte[] objData = objExcel.GetAsByteArray();
            objExcel.Dispose();
            return File(objData, "application/xlsx", strFileName);
        }

        public IActionResult GetPersonImages(long Id)
        {
            string message = "";
            try
            {
                BCV.Picture bcPicture = new();
                IEnumerable<BEV.Picture> lstPictures = bcPicture.List(new List<Field> { new Field("IdPerson", Id) }, "Type");
                return Json(new { message, photo = lstPictures?.FirstOrDefault(p => p.Type == 1)?.Name ?? "", ci = lstPictures?.FirstOrDefault(p => p.Type == 2)?.Name ?? "", cir = lstPictures?.FirstOrDefault(p => p.Type == 3)?.Name ?? "" });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult AddVisitor(string DocumentId, string FirstName, string LastName, string Phone, string PhotoImage, string DocumentIdImage, string DocumentIdReverseImage)
        {
            string message = "";
            try
            {
                BCV.Person bcPerson = new();
                BEV.Person bePerson = new() { StatusType = StatusType.Insert, DocumentId = DocumentId, FirstName = FirstName, LastName = LastName, Phone = Phone, LogUser = UserCode, LogDate = DateTime.Now };
                bcPerson.Save(ref bePerson);

                BCV.Picture bcPicture = new();
                if (!string.IsNullOrWhiteSpace(PhotoImage))
                {
                    BEV.Picture bePhoto = new() { StatusType = StatusType.Insert, IdPerson = bePerson.Id, Name = PhotoImage, Type = 1, LogUser = UserCode, LogDate = DateTime.Now };
                    bcPicture.Save(ref bePhoto);
                }
                if (!string.IsNullOrWhiteSpace(DocumentIdImage))
                {
                    BEV.Picture beImage = new() { StatusType = StatusType.Insert, IdPerson = bePerson.Id, Name = DocumentIdImage, Type = 2, LogUser = UserCode, LogDate = DateTime.Now };
                    bcPicture.Save(ref beImage);
                }
                if (!string.IsNullOrWhiteSpace(DocumentIdReverseImage))
                {
                    BEV.Picture beImage = new() { StatusType = StatusType.Insert, IdPerson = bePerson.Id, Name = DocumentIdReverseImage, Type = 3, LogUser = UserCode, LogDate = DateTime.Now };
                    bcPicture.Save(ref beImage);
                }
                return Json(new { message, id = bePerson.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Camera()
        {
            return View();
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult SaveImageBase64(string ImageBase64)
        {
            string message = "", fileName = "", filePath = "";
            try
            {
                string strImage = ImageBase64.Split(',')[1];
                filePath = Path.Combine(rootDirectory, "wwwroot", "images", "person");
                fileName = $"{UserCode}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string fullPath = Path.Combine(filePath, fileName);
                byte[] bytes = Convert.FromBase64String(strImage);          
                using Image image = Image.Load(bytes);
                image.Save(fullPath);
            }
            catch (Exception ex)
            {
                message = filePath + "   " + GetError(ex);
            }
            return Json(new { message, fileName });
        }

        [HttpPost]
        public IActionResult SaveImageBase64ToPerson(long PersonId, string ImageBase64)
        {
            string message = "", fileName = "";
            try
            {
                string strImage = ImageBase64.Split(',')[1];
                string filePath = Path.Combine(rootDirectory, "wwwroot", "images", "person");
                fileName = $"{PersonId}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string fullPath = Path.Combine(filePath, fileName);
                byte[] bytes = Convert.FromBase64String(strImage);
                using Image image = Image.Load(bytes);
                image.Save(fullPath);

                BCV.Picture bcPicture = new();
                IEnumerable<BEV.Picture> lstPictures = bcPicture.List(new List<Field> { new Field("IdPerson", PersonId) }, "1");
                BEV.Picture bePicture = (from p in lstPictures where p.Type == 1 select p).FirstOrDefault() ?? new BEV.Picture { IdPerson = PersonId, Type = 1 };
                bePicture.StatusType = bePicture.Id == 0 ? StatusType.Insert : StatusType.Update;
                bePicture.Name = fileName;
                bePicture.LogUser = UserCode;
                bePicture.LogDate = DateTime.Now;
                bcPicture.Save(ref bePicture);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, fileName });
        }

        [HttpPost]
        public IActionResult Edit(BEV.Visit Item)
        {
            string message = "";
            try
            {
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;

                BCV.Visit bcVisit = new();
                bcVisit.Save(ref Item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult FinishVisit(long Id)
        {
            string message = "";
            try
            {
                BCV.Visit bcVisit = new();
                BEV.Visit beVisit = bcVisit.Search(Id);
                beVisit.StatusType = StatusType.Update;
                beVisit.FinalDate = DateTime.Now;
                beVisit.LogDate = DateTime.Now;
                beVisit.LogUser = UserCode;
                bcVisit.Save(ref beVisit);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private static List<BEV.Visit> GetItems(string CardCode, string Visitor, long Staff, DateTime? Since, DateTime? Until, bool NotFinished)
        {
            BCV.Visit bcVisit = new();
            List<Field> lstFilters = new();
            int cont = 0;
            if (Since.HasValue)
            {
                lstFilters.Add(new Field { Name = "CAST(ISNULL(v.FinalDate, v.InitialDate) AS DATE)", Value = Since.Value.ToString("yyyy-MM-dd"), Operator = Operators.HigherOrEqualThan });
                cont++;
            }
            if (Until.HasValue)
            {
                lstFilters.Add(new Field { Name = "CAST(v.InitialDate AS DATE)", Value = Until.Value.ToString("yyyy-MM-dd"), Operator = Operators.LowerOrEqualThan });
                cont++;
            }
            if (!string.IsNullOrWhiteSpace(CardCode))
            {
                lstFilters.Add(new Field { Name = "v.CardCode", Value = CardCode });
                cont++;
            }
            if (NotFinished)
            {
                lstFilters.Add(new Field { Name = "v.FinalDate", Value = "NULL", Operator = Operators.IsNull });
                cont++;
            }
            if (Staff != 0)
            {
                lstFilters.Add(new Field { Name = "v.StaffId", Value = Staff });
                cont++;
            }
            if (!string.IsNullOrWhiteSpace(Visitor))
            {
                lstFilters.Add(new Field { Name = "p.DocumentId", Value = Visitor, Operator = Operators.Likes });
                lstFilters.Add(new Field { Name = "p.FirstName", Value = Visitor, Operator = Operators.Likes });
                lstFilters.Add(new Field { Name = "p.LastName", Value = Visitor, Operator = Operators.Likes });
                lstFilters.Add(new Field { LogicalOperator = LogicalOperators.Or });
                lstFilters.Add(new Field { LogicalOperator = LogicalOperators.Or });
                cont++;
            }
            for (int i = 1; i < cont; i++)
            {
                lstFilters.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            List<BEV.Visit> lstItems = bcVisit.List2(lstFilters, "v.InitialDate", BEV.relVisit.Staff, BEV.relVisit.Visitor);
            return lstItems;
        }

        #endregion

    }
}