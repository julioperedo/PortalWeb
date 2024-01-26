using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Style;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class AuditLogsController : BaseController
    {
        #region Constructores

        public AuditLogsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewBag.CardCode = CardCode;
                return View(CardCode == HomeCardCode ? "Index" : "IndexClient");
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetUsers(string CardCode)
        {
            string message = "";
            IEnumerable<BES.User> users = default;
            try
            {
                BCS.User bcUser = new();
                List<Field> lstFilters = new() { new Field("CardCode", CardCode, Operators.In) };
                users = bcUser.List(lstFilters, "FirstName, LastName");
                //if (users == null || users.Count == 0) message = "Ese Cliente no tiene Usuarios.";
                if (users?.Count() > 0)
                {
                    var items = (from u in users
                                 group u by u.CardCode into g
                                 select new { client = g.Key, users = g.Select(x => new { x.Id, x.Name }) });
                    return Json(new { message, items });
                }
                else
                {
                    message = "Ese Cliente no tiene Usuarios.";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Filter(string Users, DateTime? Since, DateTime? Until)
        {
            string message = "";
            IEnumerable<BES.SessionHistory> items = new List<BES.SessionHistory>();
            try
            {
                items = getItems(Users, Since, Until);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items = items.Select(x => new { UserName = x.User.Name, x.Description, ClientCode = x.User.CardCode, x.LogDate }) });
        }

        public IActionResult ExportExcel(string Users, DateTime? Since, DateTime? Until)
        {
            using (OfficeOpenXml.ExcelPackage objExcel = new OfficeOpenXml.ExcelPackage())
            {
                OfficeOpenXml.ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");

                wsMain.Name = "Sesiones";
                wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
                wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
                wsMain.Cells.Style.Font.Size = 9;

                wsMain.Cells[2, 1].Value = $"Fecha: {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}";
                wsMain.Cells[1, 1].Value = "CONSULTA DE SESIONES";
                wsMain.Cells[1, 1].Style.Font.Size = 12;
                wsMain.Cells[1, 1].Style.Font.Bold = true;
                wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsMain.Cells[1, 1, 1, 3].Merge = true;

                IEnumerable<BES.SessionHistory> lstItems = getItems(Users, Since, Until);
                var items = (from i in lstItems
                             orderby i.User.CardCode, i.User.Name, i.LogDate descending
                             select new { i.User.CardCode, i.User.Name, i.Description, i.LogDate }).ToList();

                int col = 1;
                if (CardCode == HomeCardCode)
                {
                    wsMain.Cells[3, col++].Value = "Cliente";
                }
                var colDate = wsMain.Column(CardCode == HomeCardCode ? 4 : 3);
                colDate.Style.Numberformat.Format = "dd-MM-yyyy HH:mm";
                colDate.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                wsMain.Cells[3, col++].Value = "Nombre";
                wsMain.Cells[3, col++].Value = "Descripción";
                wsMain.Cells[3, col].Value = "Fecha";

                wsMain.Cells[3, 1, 3, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells[3, 1, 3, col].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                wsMain.Cells[3, 1, 3, col].Style.Font.Color.SetColor(Color.White);
                wsMain.Cells[3, 1, 3, col].Style.Font.Bold = true;

                int intRow = 4;
                foreach (var item in items)
                {
                    col = 1;
                    if (CardCode == HomeCardCode)
                    {
                        wsMain.Cells[intRow, col++].Value = item.CardCode;
                    }
                    wsMain.Cells[intRow, col++].Value = item.Name;
                    wsMain.Cells[intRow, col++].Value = item.Description;
                    wsMain.Cells[intRow, col].Value = item.LogDate;
                    intRow++;
                }
                wsMain.Cells.AutoFitColumns();

                wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", OfficeOpenXml.ExcelHeaderFooter.PageNumber, OfficeOpenXml.ExcelHeaderFooter.NumberOfPages);
                wsMain.View.FreezePanes(4, 1);
                wsMain.PrinterSettings.LeftMargin = 0.2m;
                wsMain.PrinterSettings.RightMargin = 0.2m;
                wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:3"];
                wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:D"];

                string strFileName = "Consulta de Sesiones " + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
                byte[] objData = objExcel.GetAsByteArray();
                objExcel.Dispose();
                return File(objData, "application/xlsx", strFileName);
            }
        }

        #endregion

        #region POSTs

        #endregion

        #region Private Methods

        private IEnumerable<BES.SessionHistory> getItems(string Users, DateTime? Since, DateTime? Until)
        {
            BCS.SessionHistory bcLog = new();
            List<Field> filters = new();
            if (!string.IsNullOrWhiteSpace(Users))
            {
                filters.Add(new Field("IdUser", Users, Operators.In));
            }
            if (Since.HasValue)
            {
                filters.Add(new Field("CAST(LogDate AS DATE)", Since.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
            }
            if (Until.HasValue)
            {
                filters.Add(new Field("CAST(LogDate AS DATE)", Until.Value.ToString("yyy-MM-dd"), Operators.LowerOrEqualThan));
            }
            CompleteFilters(ref filters);
            IEnumerable<BES.SessionHistory> lstResult = bcLog.List(filters, "LogDate", BES.relSessionHistory.User);
            return lstResult;
        }

        #endregion
    }
}