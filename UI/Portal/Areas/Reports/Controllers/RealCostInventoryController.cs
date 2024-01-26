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
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;

namespace Portal.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    public class RealCostInventoryController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration config;
        #endregion

        #region Constructores

        public RealCostInventoryController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { config = configuration; }

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

        public IActionResult Filter(string Subsidiaries, string StoreHouses, string ItemCode, string Description, string Category, string Subcategory, string Line, string Blocked)
        {
            string message = "";
            IEnumerable<BEA.ProductStock> lstItems = new List<BEA.ProductStock>();
            try
            {
                lstItems = GetItems(Subsidiaries, StoreHouses, ItemCode, Description, Category, Subcategory, Line, Blocked);
                var items = lstItems.Select(x => new { x.Subsidiary, x.Warehouse, x.Product.Category, x.Product.Subcategory, x.Product.Line, x.ItemCode, ItemName = x.Product.Name, x.Stock, x.Reserved, x.Requested, x.Available, x.Warning, x.PriceReal, x.TotalReal, x.PriceModified, x.TotalModified });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Detail(string ItemCode, string Subsidiary)
        {
            BCA.Product bcItem = new();
            BEA.Product beItem = Subsidiary.ToLower() == "santa cruz" ? bcItem.SearchSC(ItemCode) : (Subsidiary.ToLower() == "iquique" ? bcItem.SearchIQQ(ItemCode) : bcItem.SearchLA(ItemCode));
            if (beItem != null)
            {
                beItem.Description = SetHTMLSafe(beItem.Description);
                beItem.Commentaries = SetHTMLSafe(beItem.Commentaries);
            }
            return Json(beItem);
        }

        public IActionResult Export(string Subsidiaries, string StoreHouses, string ItemCode, string Description, string Category, string Subcategory, string Line, string Blocked)
        {
            try
            {
                IEnumerable<BEA.ProductStock> lstItems = GetItems(Subsidiaries, StoreHouses, ItemCode, Description, Category, Subcategory, Line, Blocked);
                using (ExcelPackage objExcel = new())
                {
                    ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
                    FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

                    wsMain.Name = "Inventario";
                    wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsMain.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
                    wsMain.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    wsMain.Cells.Style.Font.Size = 9;

                    wsMain.Cells[4, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    wsMain.Cells[1, 1].Value = "INVENTARIO";
                    wsMain.Cells[1, 1].Style.Font.Size = 12;
                    wsMain.Cells[1, 1].Style.Font.Bold = true;
                    wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int row = 6, finalColumn = CardCode == HomeCardCode ? 16 : 8;

                    wsMain.Cells[1, 1, 1, finalColumn].Merge = true;

                    var logoImage = wsMain.Drawings.AddPicture("logo", logo);
                    logoImage.SetPosition(5, 5);

                    wsMain.Cells[row, 1].Value = "Categoría";
                    wsMain.Cells[row, 2].Value = "Subcategoría";
                    wsMain.Cells[row, 3].Value = "Sucursal";
                    wsMain.Cells[row, 4].Value = "Almacén";
                    wsMain.Cells[row, 5].Value = "Item";
                    wsMain.Cells[row, 6].Value = "Descripción";
                    wsMain.Cells[row, 7].Value = "Línea";
                    if (CardCode == HomeCardCode)
                    {
                        wsMain.Cells[row, 8].Value = "Stock";
                        wsMain.Cells[row, 9].Value = "Reserva";
                        wsMain.Cells[row, 10].Value = "Pedido";
                        wsMain.Cells[row, 11].Value = "Disponibilidad";
                        wsMain.Cells[row, 12].Value = "Aviso";
                        wsMain.Cells[row, 13].Value = "Unitario";
                        wsMain.Cells[row, 14].Value = "Total";
                        wsMain.Cells[row, 15].Value = "Unitario Modificado";
                        wsMain.Cells[row, 16].Value = "Total Modificado";

                        wsMain.Cells[row, 8, row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsMain.Cells[row, 13, row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        wsMain.Cells[row, 8].Value = "Disponibilidad";
                        wsMain.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    wsMain.Cells[row, 1, row, finalColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsMain.Cells[row, 1, row, finalColumn].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                    wsMain.Cells[row, 1, row, finalColumn].Style.Font.Color.SetColor(Color.White);
                    wsMain.Cells[row, 1, row, finalColumn].Style.Font.Bold = true;

                    if (lstItems?.Count() > 0)
                    {
                        foreach (var item in lstItems)
                        {
                            row += 1;
                            wsMain.Cells[row, 1].Value = item.Product.Category;
                            wsMain.Cells[row, 2].Value = item.Product.Subcategory;
                            wsMain.Cells[row, 3].Value = item.Subsidiary;
                            wsMain.Cells[row, 4].Value = item.Warehouse;
                            wsMain.Cells[row, 5].Value = item.ItemCode;
                            wsMain.Cells[row, 6].Value = item.Product.Name;
                            wsMain.Cells[row, 7].Value = item.Product.Line;
                            if (CardCode == HomeCardCode)
                            {
                                wsMain.Cells[row, 8].Value = item.Stock;
                                wsMain.Cells[row, 9].Value = item.Reserved;
                                wsMain.Cells[row, 10].Value = item.Requested;
                                wsMain.Cells[row, 11].Value = item.Available;
                                wsMain.Cells[row, 12].Value = item.Warning;
                                wsMain.Cells[row, 13].Value = item.PriceReal;
                                wsMain.Cells[row, 14].Value = item.TotalReal;
                                wsMain.Cells[row, 15].Value = item.PriceModified;
                                wsMain.Cells[row, 16].Value = item.TotalModified;

                                wsMain.Cells[row, 8, row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                wsMain.Cells[row, 8, row, 11].Style.Numberformat.Format = "#,##0.00";
                                wsMain.Cells[row, 13, row, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                wsMain.Cells[row, 13, row, 16].Style.Numberformat.Format = "#,##0.00";
                            }
                        }
                    }
                    wsMain.Cells.AutoFitColumns();
                    wsMain.Cells.Style.WrapText = true;
                    wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                    wsMain.View.FreezePanes(7, 1);
                    wsMain.PrinterSettings.LeftMargin = 0.2m;
                    wsMain.PrinterSettings.RightMargin = 0.2m;
                    wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:6"];
                    wsMain.PrinterSettings.RepeatColumns = wsMain.Cells[$"A:{(CardCode == HomeCardCode ? "P" : "H")}"];

                    string strFileName = "Inventario-Costo-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
                    byte[] objData = objExcel.GetAsByteArray();
                    objExcel.Dispose();
                    return File(objData, "application/xlsx", strFileName);
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = GetError(ex) });
            }
        }

        #endregion

        #region Private Methods

        private IEnumerable<BEA.ProductStock> GetItems(string Subsidiaries, string StoreHouses, string ItemCode, string Description, string Category, string Subcategory, string Line, string Blocked)
        {
            BCA.ProductStock bcInventory = new BCA.ProductStock();
            List<Field> lstFilter = new List<Field> { new Field { Name = "LOWER(Subsidiary)", Value = Subsidiaries.ToLower().Trim(), Operator = Operators.In } };
            if (!string.IsNullOrWhiteSpace(Blocked))
            {
                if (Blocked == "S")
                {
                    lstFilter.Add(new Field("Blocked", "Y", Operators.Equal));
                }
                else
                {
                    lstFilter.Add(new Field("Blocked", "N", Operators.Different));
                }
            }
            if (!string.IsNullOrWhiteSpace(StoreHouses))
            {
                lstFilter.Add(new Field("LOWER(Warehouse)", StoreHouses.ToLower().Trim(), Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(ItemCode))
            {
                lstFilter.Add(new Field("LOWER(ItemCode)", ItemCode.ToLower(), Operators.Likes));
            }
            if (!string.IsNullOrWhiteSpace(Description))
            {
                lstFilter.Add(new Field("LOWER(ItemName)", Description.ToLower(), Operators.Likes));
            }
            if (!string.IsNullOrWhiteSpace(Category))
            {
                lstFilter.Add(new Field("LOWER(Category)", Category.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(Subcategory))
            {
                lstFilter.Add(new Field("LOWER(Subcategory)", Subcategory.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(Line))
            {
                lstFilter.Add(new Field("LOWER(Line)", Line.ToLower(), Operators.In));
            }

            CompleteFilters(ref lstFilter);
            IEnumerable<BEA.ProductStock> lstItems = bcInventory.ListWithCost(lstFilter, "Subsidiary, ItemCode, ItemName", BEA.relProductStock.Product);
            return lstItems;
        }

        #endregion

    }
}