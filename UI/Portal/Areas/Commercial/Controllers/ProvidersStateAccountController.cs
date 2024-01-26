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

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class ProvidersStateAccountController : BaseController
    {

        #region Constructores

        public ProvidersStateAccountController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public new IActionResult GetProductManagers()
        {
            BCA.StateAccount bcState = new();
            IEnumerable<BEA.Seller> lstManagers = bcState.ListProductManagers();
            var lstItems = lstManagers.Select(x => new { Code = x.ShortName, x.Name }); //(from m in lstManagers select new { GerenteProducto = m.GerenteProducto, GerenteProductoShort = m.GerenteProductoShort }).ToList();
            return Json(lstItems);
        }

        public IActionResult Filter(string Provider, string Subsidiary, string ProductManager)
        {
            string message = "";
            try
            {
                var items = GetItems(Provider, Subsidiary, ProductManager);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ExportExcel(string Provider, string Subsidiary, string ProductManager)
        {
            using (ExcelPackage objExcel = new())
            {
                FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));
                IEnumerable<BEA.ProviderStateAccountItem> lstItems = GetItems(Provider, Subsidiary, ProductManager);

                ExcelWorksheet wsDetail = objExcel.Workbook.Worksheets.Add("Detail");
                wsDetail.Name = "Estado de Cuenta";
                wsDetail.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsDetail.Cells.Style.Fill.PatternColor.SetColor(Color.White);
                wsDetail.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
                wsDetail.Cells.Style.Font.Size = 9;

                wsDetail.Cells[1, 1].Style.Font.Size = 12;
                wsDetail.Cells[1, 1].Style.Font.Bold = true;
                wsDetail.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsDetail.Cells[1, 1, 1, 16].Merge = true;
                wsDetail.Cells[4, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                wsDetail.Cells[6, 1].Value = "Sucursal";
                wsDetail.Cells[6, 2].Value = "Ciudad";
                wsDetail.Cells[6, 3].Value = "Proveedor";
                wsDetail.Cells[6, 4].Value = "Encargado";
                wsDetail.Cells[6, 5].Value = "Fehca Doc.";
                wsDetail.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsDetail.Cells[6, 6].Value = "Fehca Doc.";
                wsDetail.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsDetail.Cells[6, 7].Value = "Tipo Doc.";
                wsDetail.Cells[6, 8].Value = "Nro. Doc.";
                wsDetail.Cells[6, 9].Value = "Orden Compra";
                wsDetail.Cells[6, 10].Value = "Fact. Proveedor";
                wsDetail.Cells[6, 11].Value = "Fact. Fabricante";
                wsDetail.Cells[6, 12].Value = "Término";
                wsDetail.Cells[6, 13].Value = "Total";
                wsDetail.Cells[6, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsDetail.Cells[6, 14].Value = "Balance";
                wsDetail.Cells[6, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsDetail.Cells[6, 15].Value = "Días";
                wsDetail.Cells[6, 16].Value = "Estado";

                wsDetail.Cells[6, 1, 6, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsDetail.Cells[6, 1, 6, 16].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                wsDetail.Cells[6, 1, 6, 16].Style.Font.Color.SetColor(Color.White);
                wsDetail.Cells[6, 1, 6, 16].Style.Font.Bold = true;

                if (lstItems != null && lstItems.Any())
                {
                    wsDetail.Cells[1, 1].Value = "ESTADO DE CUENTA ";

                    int intFila = 7;
                    foreach (var item in lstItems)
                    {
                        wsDetail.Cells[intFila, 1].Value = item.Subsidiary;
                        wsDetail.Cells[intFila, 2].Value = "";
                        wsDetail.Cells[intFila, 3].Value = item.ProviderName;
                        wsDetail.Cells[intFila, 4].Value = item.ProductManager;
                        wsDetail.Cells[intFila, 5].Value = item.DocDate;
                        wsDetail.Cells[intFila, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        wsDetail.Cells[intFila, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                        wsDetail.Cells[intFila, 6].Value = item.DocDueDate;
                        wsDetail.Cells[intFila, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        wsDetail.Cells[intFila, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                        wsDetail.Cells[intFila, 7].Value = item.Type;
                        wsDetail.Cells[intFila, 8].Value = item.DocNum;
                        wsDetail.Cells[intFila, 9].Value = item.DocBase;
                        if (item.BillProvider != 0)
                        {
                            wsDetail.Cells[intFila, 10].Value = item.BillProvider;
                        }
                        wsDetail.Cells[intFila, 11].Value = item.BillNumber;
                        wsDetail.Cells[intFila, 12].Value = item.Terms;
                        wsDetail.Cells[intFila, 13].Value = item.DocTotal;
                        wsDetail.Cells[intFila, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsDetail.Cells[intFila, 13].Style.Numberformat.Format = "#,##0.00";
                        wsDetail.Cells[intFila, 14].Value = item.Balance;
                        wsDetail.Cells[intFila, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsDetail.Cells[intFila, 14].Style.Numberformat.Format = "#,##0.00";
                        wsDetail.Cells[intFila, 15].Value = item.Days;
                        wsDetail.Cells[intFila, 16].Value = item.State;

                        intFila += 1;
                    }
                    wsDetail.Cells[intFila, 14].Value = (from i in lstItems select i.Balance).Sum();
                    wsDetail.Cells[intFila, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsDetail.Cells[intFila, 14].Style.Numberformat.Format = "#,##0.00";

                    wsDetail.Cells[intFila, 1, intFila, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsDetail.Cells[intFila, 1, intFila, 16].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                    wsDetail.Cells[intFila, 1, intFila, 16].Style.Font.Color.SetColor(Color.White);
                    wsDetail.Cells[intFila, 1, intFila, 16].Style.Font.Bold = true;
                }
                else
                {
                    wsDetail.Cells[1, 1].Value = "ESTADO DE CUENTA";
                }

                wsDetail.View.FreezePanes(7, 1);
                wsDetail.PrinterSettings.Orientation = eOrientation.Landscape;
                wsDetail.PrinterSettings.LeftMargin = 0.3m;
                wsDetail.PrinterSettings.RightMargin = 0.3m;
                wsDetail.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                wsDetail.PrinterSettings.RepeatRows = wsDetail.Cells["1:6"];
                wsDetail.PrinterSettings.RepeatColumns = wsDetail.Cells["A:P"];

                var imgLogo = wsDetail.Drawings.AddPicture("logo", logo);
                imgLogo.SetPosition(5, 5);
                imgLogo.SetSize(90);

                byte[] objData = objExcel.GetAsByteArray();
                objExcel.Dispose();
                return File(objData, "application/xlsx", "Estado-de-Cuenta-Compras-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".xlsx");
            }
        }

        #endregion

        #region Private Methods

        private static IEnumerable<BEA.ProviderStateAccountItem> GetItems(string Provider, string Subsidiary, string ProductManager)
        {
            BCA.StateAccount bcState = new();
            List<Field> lstFilter = new() { new Field("Subsidiary", Subsidiary, Operators.In) };
            if (!string.IsNullOrWhiteSpace(Provider))
            {
                lstFilter.AddRange(new[] { new Field("ProviderCode", Provider, Operators.In), new Field(LogicalOperators.And) });
            }
            if (!string.IsNullOrWhiteSpace(ProductManager))
            {
                lstFilter.AddRange(new[] { new Field("ProductManagerShort", ProductManager, Operators.In), new Field(LogicalOperators.And) });
            }
            var items = bcState.ListStateAccountProvider(lstFilter, "Days");

            return items;
        }

        #endregion

    }
}