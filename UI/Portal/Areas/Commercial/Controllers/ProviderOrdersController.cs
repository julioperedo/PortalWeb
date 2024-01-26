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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEM = Portal.Areas.Commercial.Models;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class ProviderOrdersController : BaseController
    {
        #region Constructores

        public ProviderOrdersController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public IActionResult GetProviders()
        {
            BCA.ProviderOrder bcShopping = new();
            IEnumerable<BEA.Item> lstProviders = bcShopping.ListProviders();
            return Json(lstProviders.Select(x => new { x.Code, x.Name }));
        }

        public IActionResult Filter(string Subsidiaries, string Warehouses, int? OrderNumber, string ProviderCode, string ProductCode, DateTime? SinceDate, DateTime? UntilDate, string State, string Line, string Category, string Subcategory)
        {
            string message = "";
            List<BEM.ProviderOrder> items = new();
            try
            {
                items = GetItems(Subsidiaries, Warehouses, OrderNumber, ProviderCode, ProductCode, SinceDate, UntilDate, State, Line, Category, Subcategory);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var beResult = new { message, items };
            return Json(beResult);
        }

        public IActionResult Detail(string Subsidiary, int OrderNumber)
        {
            string message = "";
            IEnumerable<BEA.ProviderOrderItem> items = new List<BEA.ProviderOrderItem>();
            try
            {
                BCA.ProviderOrder bcDetail = new();
                List<Field> lstFilter = new() { new Field("Subsidiary", Subsidiary), new Field("DocNumber", OrderNumber), new Field(LogicalOperators.And) };
                items = bcDetail.ListItems(lstFilter, "ItemCode");
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var beResult = new { message, items };
            return Json(beResult);
        }

        public IActionResult ExportExcel(string Subsidiaries, string StoreHouses, int? NumOrder, string ProviderCode, string ProductCode, DateTime? SinceDate, DateTime? UntilDate, string State, string Line, string Category, string Subcategory)
        {
            List<BEM.ProviderOrder> lstItems = GetItems(Subsidiaries, StoreHouses, NumOrder, ProviderCode, ProductCode, SinceDate, UntilDate, State, Line, Category, Subcategory);
            using (ExcelPackage objExcel = new())
            {
                ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
                FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

                wsMain.Name = "Resumen Compras";
                wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
                wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
                wsMain.Cells.Style.Font.Size = 9;

                wsMain.Cells[4, 1].Value = $"Fecha: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";
                wsMain.Cells[1, 1].Value = "CONSULTA DE COMPRAS";
                wsMain.Cells[1, 1].Style.Font.Size = 12;
                wsMain.Cells[1, 1].Style.Font.Bold = true;
                wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsMain.Cells[1, 1, 1, 10].Merge = true;

                var imgLogo = wsMain.Drawings.AddPicture("logo", logo);
                imgLogo.SetPosition(5, 5);

                wsMain.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsMain.Column(5).Style.Numberformat.Format = "dd/MM/yyyy";
                wsMain.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsMain.Column(6).Style.Numberformat.Format = "dd/MM/yyyy";
                wsMain.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsMain.Column(8).Style.Numberformat.Format = "#,##0.00";
                wsMain.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsMain.Column(9).Style.Numberformat.Format = "#,##0.00";

                wsMain.Cells[6, 1].Value = "Sucursal";
                wsMain.Cells[6, 2].Value = "Almacén";
                wsMain.Cells[6, 3].Value = "Proveedor";
                wsMain.Cells[6, 4].Value = "# Orden";
                wsMain.Cells[6, 5].Value = "F. Orden Compra";
                wsMain.Cells[6, 6].Value = "F. Estimada";
                wsMain.Cells[6, 7].Value = "Términos Pago";
                wsMain.Cells[6, 8].Value = "Gastos Adicionales";
                wsMain.Cells[6, 9].Value = "Total";

                wsMain.Cells[6, 1, 6, 9].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells[6, 1, 6, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
                wsMain.Cells[6, 1, 6, 9].Style.Font.Color.SetColor(System.Drawing.Color.White);
                wsMain.Cells[6, 1, 6, 9].Style.Font.Bold = true;

                int intFila;

                if (lstItems != null && lstItems.Count > 0)
                {
                    intFila = 7;
                    foreach (var item in lstItems)
                    {
                        wsMain.Cells[intFila, 1].Value = item.Subsidiary;
                        wsMain.Cells[intFila, 2].Value = item.Warehouse;
                        wsMain.Cells[intFila, 3].Value = item.ProviderName;
                        wsMain.Cells[intFila, 4].Value = item.DocNumber;
                        wsMain.Cells[intFila, 5].Value = item.DocDate;
                        wsMain.Cells[intFila, 6].Value = item.EstimatedDate;
                        wsMain.Cells[intFila, 7].Value = item.Terms;
                        wsMain.Cells[intFila, 8].Value = item.OtherCosts;
                        wsMain.Cells[intFila, 9].Value = item.Total;
                        intFila += 1;
                    }
                }

                wsMain.Cells.AutoFitColumns();
                wsMain.Cells.Style.WrapText = true;
                wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                wsMain.View.FreezePanes(7, 1);
                wsMain.PrinterSettings.LeftMargin = 0.2m;
                wsMain.PrinterSettings.RightMargin = 0.2m;
                wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:6"];
                wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:I"];
                //.PrinterSettings.Orientation = eOrientation.Landscape

                string strFileName = $"Resumen-Pedidos-{DateTime.Now.ToString("yyyyMMdd-HHmm")}.xlsx";
                byte[] objData = objExcel.GetAsByteArray();
                objExcel.Dispose();
                return File(objData, "application/xlsx", strFileName);
            }
        }

        public IActionResult DownloadFile(string FilePath, string FileName)
        {
            try
            {
                string fullName = $@"{FilePath}\\{FileName}";
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string extension = FileName.Contains(".") ? FileName.Split(".").Last() : "";
                string contentType = extension switch
                {
                    "htm" or "html" => "text/HTML",
                    "txt" => "text/plain",
                    "doc" or "rtf" or "docx" => "Application/msword",
                    "xls" or "xlsx" => "Application/x-msexcel",
                    "jpg" or "jpeg" => "image/jpeg",
                    "gif" => "image/GIF",
                    "pdf" => "application/pdf",
                    "msg" => "application/vnd.ms-outlook",
                    _ => "application/octet-stream",
                };
                return File(file, contentType, $"{FileName}");
            }
            catch (FileNotFoundException)
            {
                string fullName = Path.Combine(rootDirectory, "wwwroot", "images", "DMC-404.jpg");
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string contentType = "image/jpeg";
                return File(file, contentType, "DMC-404.jpg");
            }
        }

        #endregion

        #region Métodos Privados

        private static List<BEM.ProviderOrder> GetItems(string Subsidiaries, string Warehouses, int? NumOrder, string ProviderCode, string ProductCode, DateTime? SinceDate, DateTime? UntilDate, string State, string Line, string Category, string Subcategory)
        {
            List<Field> filters = new();
            if (!string.IsNullOrWhiteSpace(Subsidiaries))
            {
                filters.Add(new Field("LOWER(Subsidiary)", Subsidiaries.ToLower(), Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(Warehouses))
            {
                filters.Add(new Field("LOWER(Warehouse)", Warehouses.ToLower(), Operators.In));
            }
            if (NumOrder.HasValue)
            {
                filters.Add(new Field("DocNumber", NumOrder.Value, Operators.Likes));
            }
            if (!string.IsNullOrWhiteSpace(ProviderCode))
            {
                filters.Add(new Field("LOWER(ProviderCode)", ProviderCode.ToLower(), Operators.In));
            }
            if (SinceDate.HasValue)
            {
                filters.Add(new Field("DocDate", SinceDate.Value.ToString("yyyy/MM/dd"), Operators.HigherOrEqualThan));
                filters.Add(new Field("EstimatedDate", SinceDate.Value.ToString("yyyy/MM/dd"), Operators.HigherOrEqualThan));
                filters.Add(new Field(LogicalOperators.Or));
            }
            if (UntilDate.HasValue)
            {
                filters.Add(new Field("DocDate", UntilDate.Value.ToString("yyyy/MM/dd"), Operators.LowerOrEqualThan));
                filters.Add(new Field("EstimatedDate", UntilDate.Value.ToString("yyyy/MM/dd"), Operators.LowerOrEqualThan));
                filters.Add(new Field(LogicalOperators.Or));
            }
            if (!string.IsNullOrWhiteSpace(State))
            {
                filters.Add(new Field("LOWER(State)", State == "C" ? "cerrado" : "abierto"));
            }
            if (!string.IsNullOrWhiteSpace(ProductCode))
            {
                filters.Add(new Field("LOWER(ItemCode)", ProductCode.ToLower(), Operators.Likes));
                filters.Add(new Field("LOWER(ItemName)", ProductCode.ToLower(), Operators.Likes));
                filters.Add(new Field(LogicalOperators.Or));
            }
            if (!string.IsNullOrWhiteSpace(Line))
            {
                filters.Add(new Field("LOWER(ItemLine)", Line.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(Category))
            {
                filters.Add(new Field("LOWER(ItemCategory)", Category.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(Subcategory))
            {
                filters.Add(new Field("LOWER(ItemSubcategory)", Subcategory.ToLower()));
            }
            CompleteFilters(ref filters);

            BCA.ProviderOrder bcShopping = new();
            IEnumerable<BEA.ProviderOrder> lstItems = bcShopping.ListFull(filters, "Subsidiary, DocDate");

            return (from i in lstItems
                    group i by new { i.Subsidiary, i.DocNumber } into g
                    select new BEM.ProviderOrder
                    {
                        Subsidiary = g.Key.Subsidiary,
                        DocNumber = g.Key.DocNumber,
                        DocDate = g.FirstOrDefault().DocDate,
                        EstimatedDate = g.FirstOrDefault().EstimatedDate,
                        ProviderCode = g.FirstOrDefault().ProviderCode,
                        ProviderName = g.FirstOrDefault().ProviderName,
                        Warehouse = g.FirstOrDefault().Warehouse,
                        ReferenceOrder = g.FirstOrDefault().ReferenceOrder,
                        SellerCode = g.FirstOrDefault().SellerCode,
                        Terms = g.FirstOrDefault().Terms,
                        OtherCosts = g.FirstOrDefault().OtherCosts,
                        Total = g.FirstOrDefault().Total,
                        State = g.FirstOrDefault().State,
                        OpenQuantity = g.FirstOrDefault().OpenQuantity,
                        Quantity = g.FirstOrDefault().Quantity,
                        Items = from x in g
                                where x.BillNumber.HasValue && x.BillNumber.Value > 0
                                select new BEM.ProviderOrderItem { BillNumber = x.BillNumber.Value, BillDate = x.BillDate, FilePath = x.FilePath, FileName = x.FileName } //g.Select(x => new BEM.ProviderOrderItem { BillNumber = x.BillNumber, BillDate = x.BillDate })
                    }).ToList();
        }

        #endregion
    }
}