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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class StateAccountHistoryController : BaseController
    {
        #region Constructores

        public StateAccountHistoryController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                if (CardCode == HomeCardCode)
                {
                    ViewBag.SeeMargin = GetPermission("VerMargen") > 0 ? "Y" : "N";
                    return View();
                }
                else
                {
                    ViewBag.CardCode = CardCode;
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetClientsBySeller()
        {
            IEnumerable<BEA.Client> clients;
            try
            {
                BCS.UserData bcData = new();
                var data = bcData.SearchByUser(UserCode);
                if (!string.IsNullOrEmpty(data?.SellerCode))
                {
                    List<Field> filters = new() { new Field("SellerCode", data.SellerCode) };
                    BCA.Client bcClient = new();
                    clients = bcClient.ListShort(filters, "CardName");
                }
                else
                {
                    clients = new List<BEA.Client>();
                }
            }
            catch (Exception)
            {
                clients = new List<BEA.Client>();
            }
            return Json(clients.Select(x => new { x.CardCode, CardName = $"{x.CardCode} - {x.CardName}" }));
        }

        public IActionResult Filter(string ClientCode, DateTime InitialDate, DateTime FinalDate)
        {
            string message = "";
            try
            {
                var (detail, resumeBefore, balance) = getItems(ClientCode, InitialDate, FinalDate);

                var items = detail.Select(x => new { x.Subsidiary, x.Type, x.TypeId, x.DocNum, x.DocDate, x.Terms, x.DocTotal, x.SellerCode, x.SellerName });
                var before = resumeBefore.Select(x => new { x.Type, Total = x.DocTotal });
                var selected = (from d in detail
                                group d by d.Type into g
                                select new { Type = g.Key, Total = g.Sum(x => x.DocTotal) });

                return Json(new { message, items, before, balance, selected });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ExportExcel(string ClientCode, DateTime InitialDate, DateTime FinalDate)
        {
            string numberFormat = "#,##0.00", dateFormat = "dd/MM/yyyy";
            using (ExcelPackage objExcel = new())
            {
                ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
                wsMain.Cells.Style.Font.Size = 10;
                wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                //System.Drawing.Image logo = System.Drawing.Image.FromFile(Server.MapPath("~/Content/img/logo2.jpg"));

                var (detail, resumeBefore, balance) = getItems(ClientCode, InitialDate, FinalDate);
                var selected = (from d in detail
                                group d by d.Type into g
                                select new { Type = g.Key, DocTotal = g.Sum(x => x.DocTotal) });

                int col = 4, row = 1;
                decimal total = 0;
                var styledCells = wsMain.Cells[1, 1, 1, 2];
                styledCells.Style.Font.Bold = true;
                styledCells.Style.Font.Size = 11;

                wsMain.Cells[row, 1].Value = "Período";
                wsMain.Cells[row, 2].Value = $"antes de {InitialDate:dd/MM/yyyy}";
                wsMain.Cells[row++, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                foreach (var item in resumeBefore)
                {
                    wsMain.Cells[row, 1].Value = item.Type;
                    wsMain.Cells[row, 2].Value = item.DocTotal;
                    wsMain.Cells[row++, 2].Style.Numberformat.Format = numberFormat;
                    total += item.DocTotal;
                }
                //wsMain.Cells[row, 1].Value = "Total Período";
                //wsMain.Cells[row, 2].Value = total;
                //wsMain.Cells[row++, 2].Style.Numberformat.Format = numberFormat;

                //styledCells = wsMain.Cells[row - 1, 1, row - 1, 2];
                //styledCells.Style.Font.Bold = true;
                //styledCells.Style.Font.Size = 12;

                styledCells = wsMain.Cells[1, 1, row - 1, 2];
                styledCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                styledCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FCF8E3"));
                styledCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#8A6D3B"));
                //cellsBefore.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                //cellsBefore.Style.Border.Left.Color.SetColor(ColorTranslator.FromHtml("#FAEBCC"));

                row++;
                total = 0;
                int tempRow = row;

                styledCells = wsMain.Cells[row, 1, row, 2];
                styledCells.Style.Font.Bold = true;
                styledCells.Style.Font.Size = 11;

                wsMain.Cells[row, 1].Value = "Período";
                wsMain.Cells[row, 2].Value = $"{InitialDate:dd/MM/yyyy} - {FinalDate:dd/MM/yyyy}";
                wsMain.Cells[row++, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                foreach (var item in selected)
                {
                    wsMain.Cells[row, 1].Value = item.Type;
                    wsMain.Cells[row, 2].Value = item.DocTotal;
                    wsMain.Cells[row++, 2].Style.Numberformat.Format = numberFormat;
                    total += item.DocTotal;
                }
                wsMain.Cells[row, 1].Value = "Total Período";
                wsMain.Cells[row, 2].Value = total;
                wsMain.Cells[row++, 2].Style.Numberformat.Format = numberFormat;

                styledCells = wsMain.Cells[row - 1, 1, row - 1, 2];
                styledCells.Style.Font.Bold = true;
                styledCells.Style.Font.Size = 12;

                styledCells = wsMain.Cells[tempRow, 1, row - 1, 2];
                styledCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                styledCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#DFF0D8"));
                styledCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#3C763D"));

                row++;
                wsMain.Cells[row, 1].Value = "BALANCE";
                wsMain.Cells[row, 2].Value = balance;
                wsMain.Cells[row++, 2].Style.Numberformat.Format = numberFormat;

                styledCells = wsMain.Cells[row - 1, 1, row - 1, 2];
                styledCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                styledCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#D9EDF7"));
                styledCells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#31708F"));
                styledCells.Style.Font.Bold = true;
                styledCells.Style.Font.Size = 16;

                wsMain.Cells[row, 1].Value = "Los negativos son en favor del cliente.";
                wsMain.Cells[row, 1, row, 2].Merge = true;

                row = 1;
                wsMain.Cells[row, col++].Value = "Sucursal";
                wsMain.Cells[row, col++].Value = "Tipo";
                wsMain.Cells[row, col++].Value = "No. Doc.";
                wsMain.Cells[row, col].Value = "Fecha";
                wsMain.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsMain.Cells[row, col].Value = "Monto";
                wsMain.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsMain.Cells[row, col++].Value = "Referencia";
                wsMain.Cells[row, col].Value = "Ejecutivo de Ventas";

                styledCells = wsMain.Cells[row, 4, row++, col];
                styledCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                styledCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#999999"));
                styledCells.Style.Font.Bold = true;
                styledCells.Style.Font.Size = 11;

                foreach (var item in detail)
                {
                    col = 4;
                    wsMain.Cells[row, col++].Value = item.Subsidiary;
                    wsMain.Cells[row, col++].Value = item.Type;
                    wsMain.Cells[row, col++].Value = item.DocNum;
                    wsMain.Cells[row, col].Value = item.DocDate;
                    wsMain.Cells[row, col].Style.Numberformat.Format = dateFormat;
                    wsMain.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsMain.Cells[row, col].Value = item.DocTotal;
                    wsMain.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsMain.Cells[row, col++].Value = item.Terms;
                    wsMain.Cells[row++, col].Value = item.SellerName;

                    if (row % 2 == 1)
                    {
                        styledCells = wsMain.Cells[row - 1, 4, row - 1, col];
                        styledCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        styledCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#F0F0F0"));
                    }
                }

                wsMain.Column(1).Width = 14;
                wsMain.Column(2).Width = 22;
                wsMain.Column(3).Width = 5;
                wsMain.Column(4).Width = 10;
                wsMain.Column(5).Width = 14;
                wsMain.Column(6).Width = 9;
                wsMain.Column(7).Width = 14;
                wsMain.Column(8).Width = 12;
                wsMain.Column(9).Width = 10;
                wsMain.Column(10).Width = 30;

                //wsMain.Row(4).Style.Fill.PatternType = ExcelFillStyle.DarkDown;
                //wsMain.Row(4).Style.Fill.BackgroundColor.SetColor(Color.Orange);

                wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                //wsMain.View.FreezePanes(8, 1);
                wsMain.PrinterSettings.LeftMargin = 0.2m;
                wsMain.PrinterSettings.RightMargin = 0.2m;
                //wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:7"];
                //wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:N"];

                string strFileName = $"Historico-Estado-Cuentas-{DateTime.Now.ToString("yyyyMMdd-HHmm")}.xlsx";
                byte[] objData = objExcel.GetAsByteArray();
                objExcel.Dispose();
                return File(objData, "application/xlsx", strFileName);
            }
        }

        public IActionResult GetPaymentReceipt(string Subsidiary, long DocNum)
        {
            string message = "";
            try
            {
                BCA.Payment bcPayment = new();
                List<Field> filters = new() { new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field("DocNumber", DocNum), new Field(LogicalOperators.And) };
                IEnumerable<BEA.Payment> payments = bcPayment.List(filters, "1");
                var item = (from i in payments
                            orderby i.DocDate descending
                            group i by i.DocNumber into g
                            select new
                            {
                                DocNumber = g.Key,
                                g.First().Subsidiary,
                                g.First().DocDate,
                                g.First().State,
                                g.First().ClientCode,
                                g.First().ClientName,
                                g.First().TotalReceipt,
                                g.First().OnAccount,
                                g.First().NotAppliedTotal,
                                g.First().Comments,
                                NoteNumbers = String.Join(", ", (from n in g where n.NoteNumber > 0 select n.NoteNumber).ToArray()),
                                DueDays = string.Join(", ", (from d in g select d.DueDays).ToArray()),
                                TotalDueDays = (from d in g select d.DueDays).Sum(),
                                TotalBills = g.Count(),
                                PastDue = (from d in g where d.DueDays >= 10 select d).Count() > 0,
                                Notes = (from d in g select new { d.NoteNumber, d.NotePaidAmount, d.Total, d.DueDays, d.NoteDate, d.Terms }).ToList()
                            }).FirstOrDefault();
                return Json(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        #endregion

        #region Métodos Privados

        private (IEnumerable<BEA.StateAccountDetail>, IEnumerable<BEA.StateAccountDetail>, decimal) getItems(string ClientCode, DateTime InitialDate, DateTime FinalDate)
        {
            IEnumerable<BEA.StateAccountDetail> detail, resumeBefore;
            IEnumerable<BEA.Client> clientDetails;
            BCA.StateAccountDetail bcStateAccount = new BCA.StateAccountDetail();
            BCA.Client bcClient = new BCA.Client();

            List<Field> filters = new List<Field> { 
                new Field("CardCode", ClientCode), new Field("DocDate", InitialDate.ToString("yyy-MM-dd"), Operators.HigherOrEqualThan), new Field("DocDate", FinalDate.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan), 
                new Field(LogicalOperators.And), new Field(LogicalOperators.And) 
            };
            detail = bcStateAccount.List2(filters, "DocDate");

            filters = new List<Field> { new Field("CardCode", ClientCode), new Field("DocDate", InitialDate.ToString("yyyy-MM-dd"), Operators.LowerThan), new Field(LogicalOperators.And) };
            resumeBefore = bcStateAccount.ListResume(filters, "1");

            filters = new List<Field> { new Field("CardCode", ClientCode) };
            clientDetails = bcClient.ListBalance(filters, "1");
            decimal balance = clientDetails.Sum(x => x.Balance);

            return (detail, resumeBefore, balance);
        }

        #endregion
    }
}