using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUglify.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BEA = BEntities.SAP;
using BEM = Portal.Areas.Commercial.Models;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class PaymentsController : BaseController
    {
        #region Constructores

        public PaymentsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                if (CardCode == HomeCardCode)
                {
                    ViewData["Title"] = "Histórico de Pagos";
                    return View();
                }
                else
                {
                    string clientName = CardCode;
                    try
                    {
                        BCA.Client bcClient = new BCA.Client();
                        BEA.Client beClient = bcClient.Search(CardCode);
                        clientName = beClient.CardName;
                    }
                    catch (Exception) { }
                    ViewData["Title"] = $"Histórico de Pagos : {clientName}";
                    ViewBag.CardCode = CardCode;
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(string ClientCode, DateTime? InitialDate, DateTime? FinalDate, int? ReceiptCode, int? NoteCode)
        {
            string message = "";
            List<BEM.Receipt> items = new();
            try
            {
                items = GetItems(ClientCode, InitialDate, FinalDate, ReceiptCode, NoteCode);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var beResult = new { message, items };
            return Json(beResult);
        }

        public IActionResult ExportExcel(string ClientCode, DateTime? InitialDate, DateTime? FinalDate, int? ReceiptCode, int? NoteCode)
        {
            List<BEM.Receipt> lstItems = GetItems(ClientCode, InitialDate, FinalDate, ReceiptCode, NoteCode);
            using ExcelPackage objExcel = new();
            ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
            FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

            wsMain.Name = "Pagos";
            wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
            wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
            wsMain.Cells.Style.Font.Size = 9;

            wsMain.Cells[4, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            wsMain.Cells[1, 1].Value = "HISTORICO DE PAGOS";
            if (InitialDate.HasValue | FinalDate.HasValue)
            {
                wsMain.Cells[2, 1].Value = (InitialDate.HasValue ? "Desde: " + InitialDate.Value.ToString("dd/MM/yyyy") : "") + (FinalDate.HasValue ? " - Hasta: " + FinalDate.Value.ToString("dd/MM/yyyy") : "");
            }
            wsMain.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[2, 1, 2, 10].Merge = true;
            wsMain.Cells[1, 1].Style.Font.Size = 12;
            wsMain.Cells[1, 1].Style.Font.Bold = true;
            wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[1, 1, 1, 10].Merge = true;

            var imgLogo = wsMain.Drawings.AddPicture("logo", logo);
            imgLogo.SetPosition(5, 5);

            wsMain.Cells[6, 4].Value = "Fecha";
            wsMain.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[6, 5].Value = "Recibo";
            wsMain.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[6, 6].Value = "Estado Recibo";
            wsMain.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[6, 7].Value = "# Notas de Venta";
            wsMain.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[6, 8].Value = "Pago a Cuenta";
            wsMain.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsMain.Cells[6, 9].Value = "Total Recibo";
            wsMain.Cells[6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsMain.Cells[6, 10].Value = "Total Sin Aplicar";
            wsMain.Cells[6, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsMain.Cells[6, 11].Value = "Referencia";
            wsMain.Cells[6, 12].Value = "Días Mora";
            wsMain.Cells[6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsMain.Cells[6, 1, 6, 12].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells[6, 1, 6, 12].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
            wsMain.Cells[6, 1, 6, 12].Style.Font.Color.SetColor(Color.White);
            wsMain.Cells[6, 1, 6, 12].Style.Font.Bold = true;

            if (lstItems != null && lstItems.Count > 0)
            {
                List<string> lstClientes = (from i in lstItems orderby i.ClientName select i.ClientName).Distinct().ToList();

                int intFila = 7;
                foreach (var cliente in lstClientes)
                {
                    List<string> lstSucursales = (from i in lstItems where i.ClientName == cliente orderby i.Subsidiary select i.Subsidiary).Distinct().ToList();

                    int totalDueDays = (from i in lstItems where i.ClientName == cliente select i.TotalDueDays).Sum(), totalBilled = (from i in lstItems where i.ClientName == cliente select i.TotalBilled).Sum();

                    wsMain.Cells[intFila, 1].Value = "Cliente: " + cliente;
                    wsMain.Cells[intFila, 1, intFila, 12].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    wsMain.Cells[intFila, 1, intFila, 12].Style.Font.Bold = true;
                    wsMain.Cells[intFila, 1, intFila, 8].Merge = true;
                    wsMain.Cells[intFila, 9].Value = (from i in lstItems where i.ClientName == cliente select i.TotalReceipt).Sum();
                    wsMain.Cells[intFila, 9].Style.Numberformat.Format = "#,##0.00";
                    wsMain.Cells[intFila, 12].Value = totalBilled > 0 ? (decimal)totalDueDays / totalBilled : "";
                    wsMain.Cells[intFila, 12].Style.Numberformat.Format = "#,##0.00";
                    intFila += 1;
                    foreach (var sucursal in lstSucursales)
                    {
                        var lstSubItems = (from i in lstItems where i.Subsidiary == sucursal & i.ClientName == cliente select i).ToList();

                        wsMain.Cells[intFila, 2].Value = "Sucursal: " + sucursal;
                        wsMain.Cells[intFila, 1, intFila, 12].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        wsMain.Cells[intFila, 1, intFila, 12].Style.Font.Bold = true;
                        wsMain.Cells[intFila, 2, intFila, 8].Merge = true;
                        wsMain.Cells[intFila, 9].Value = (from i in lstItems where i.Subsidiary == sucursal & i.ClientName == cliente select i.TotalReceipt).Sum();
                        wsMain.Cells[intFila, 9].Style.Numberformat.Format = "#,##0.00";
                        var x = (from i in lstItems where i.Subsidiary == sucursal & i.ClientName == cliente select i.TotalDueDays).Sum();
                        var y = (from i in lstItems where i.Subsidiary == sucursal & i.ClientName == cliente select i.TotalBilled).Sum();
                        wsMain.Cells[intFila, 12].Value = y == 0 ? (decimal)0.0 : (decimal)x / y;
                        wsMain.Cells[intFila, 12].Style.Numberformat.Format = "#,##0.00";
                        intFila += 1;

                        foreach (var item in lstSubItems)
                        {
                            wsMain.Cells[intFila, 4].Value = item.DocDate;
                            wsMain.Cells[intFila, 4].Style.Numberformat.Format = "dd/MM/yyyy";
                            wsMain.Cells[intFila, 5].Value = item.DocNumber;
                            wsMain.Cells[intFila, 6].Value = item.State;
                            wsMain.Cells[intFila, 7].Value = string.Join(", ", (from i in item.Notes where i.NoteNumber > 0 select i.NoteNumber));
                            wsMain.Cells[intFila, 8].Value = item.OnAccount;
                            wsMain.Cells[intFila, 8].Style.Numberformat.Format = "#,##0.00";
                            wsMain.Cells[intFila, 9].Value = item.TotalReceipt;
                            wsMain.Cells[intFila, 9].Style.Numberformat.Format = "#,##0.00";
                            wsMain.Cells[intFila, 10].Value = item.NotAppliedTotal;
                            wsMain.Cells[intFila, 10].Style.Numberformat.Format = "#,##0.00";
                            wsMain.Cells[intFila, 11].Value = item.Comments;
                            wsMain.Cells[intFila, 12].Value = item.TotalDueDays;
                            intFila += 1;
                        }
                    }
                }
                wsMain.Cells[intFila, 9].Value = (from i in lstItems select i.TotalReceipt).Sum();
                wsMain.Cells[intFila, 9].Style.Numberformat.Format = "#,##0.00";
                wsMain.Cells[intFila, 1, intFila, 12].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                wsMain.Cells[intFila, 1, intFila, 12].Style.Font.Color.SetColor(Color.White);
                wsMain.Cells[intFila, 1, intFila, 12].Style.Font.Bold = true;
            }

            wsMain.Cells.AutoFitColumns();
            wsMain.Column(1).Width = 3;
            wsMain.Column(2).Width = 3;
            wsMain.Column(3).Width = 3;
            wsMain.Column(7).Width = 50;
            wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            wsMain.View.FreezePanes(7, 1);
            wsMain.PrinterSettings.Orientation = eOrientation.Landscape;
            wsMain.PrinterSettings.LeftMargin = 0.2m;
            wsMain.PrinterSettings.RightMargin = 0.2m;
            wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:6"];
            wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:L"];

            byte[] objData = objExcel.GetAsByteArray();
            objExcel.Dispose();
            return File(objData, "application/xlsx", $"Historico-Pagos-{DateTime.Now.ToString("yyyyMMdd-HHmm")}.xlsx");
        }

        public IActionResult GetAdjustmentItems(string Subsidiary, int DocNum)
        {
            string message = "";
            IEnumerable<BEA.PaymentItem> items = default;
            try
            {
                BCA.Payment bcPayment = new();
                items = bcPayment.ListAdjustmentItems(Subsidiary, DocNum, "LineNum");
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region Métodos Privados

        private List<BEM.Receipt> GetItems(string ClientCode, DateTime? InitialDate, DateTime? FinalDate, int? ReceiptCode, int? NoteCode)
        {
            BCA.Payment bcPayment = new();
            IEnumerable<BEA.Payment> lstTemp = bcPayment.List(ClientCode, InitialDate, FinalDate, ReceiptCode, NoteCode, "3 DESC");
            List<Field> filters = new();
            if (!string.IsNullOrWhiteSpace(ClientCode)) filters.Add(new Field("ClientCode", ClientCode.Trim()));
            if (InitialDate.HasValue) filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
            if (FinalDate.HasValue) filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
            if (ReceiptCode.HasValue && ReceiptCode.Value > 0) filters.Add(new Field("Id", ReceiptCode));
            CompleteFilters(ref filters);
            IEnumerable<BEA.Payment> lstAdjust = bcPayment.ListAdjustments(filters, "DocDate");
            List<BEM.Receipt> lstItems = new();
            lstItems = (from i in lstTemp
                        orderby i.DocDate descending
                        group i by i.DocNumber into g
                        select new BEM.Receipt
                        {
                            DocNumber = g.Key,
                            Subsidiary = ToTitle(g.First().Subsidiary),
                            DocDate = g.First().DocDate,
                            State = g.First().State,
                            ClientCode = g.First().ClientCode,
                            ClientName = $"{g.First().ClientCode} - {g.First().ClientName}",
                            TotalReceipt = g.First().TotalReceipt,
                            OnAccount = g.First().OnAccount,
                            NotAppliedTotal = g.First().NotAppliedTotal,
                            Comments = g.First().Comments,
                            TotalDueDays = (from d in g select d.DueDays).Sum(),
                            TotalBilled = g.Count(),
                            InDue = (from d in g where d.DueDays >= 10 select d).Any(),
                            Notes = (from d in g
                                     where d.NoteNumber.HasValue && d.NoteNumber.Value > 0
                                     select new BEM.Note { NoteNumber = d.NoteNumber.Value, AmountPaid = d.NotePaidAmount, Total = d.Total, Days = d.DueDays, DocDate = d.NoteDate, Terms = d.Terms, IsDelivery = d.IsDeliveryNote }).ToList()
                        }).ToList();
            if (lstAdjust?.Count() > 0)
            {
                lstAdjust.ForEach(x => x.ClientName = $"{x.ClientCode} - {x.ClientName}");
                lstItems.AddRange(lstAdjust.Select(i => new BEM.Receipt(i) { Subsidiary = ToTitle(i.Subsidiary) }));
            }
            return lstItems;
        }

        #endregion
    }
}