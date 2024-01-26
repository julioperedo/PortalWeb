using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BEntities.Filters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class InventoryController : BaseController
    {

        #region Constructores

        public InventoryController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            short intPermission = GetPermission("Inventario");
            if (IsAllowed(this))
            {
                ViewData["Permission"] = intPermission;

                string strLocal = "N", strClientAllowed = "N", strCardCode = CardCode;
                if (strCardCode == HomeCardCode)
                {
                    strLocal = "Y";
                }
                else
                {
                    BCP.ClientAllowed bcAllowed = new();
                    IEnumerable<BEP.ClientAllowed> lstAlloweds = bcAllowed.List("1");
                    //if ((from a in lstAlloweds select a.CardCode).Contains(strCardCode))
                    if (lstAlloweds.Any(x => x.CardCode == strCardCode))
                    {
                        strClientAllowed = "Y";
                    }
                }
                ViewBag.Local = strLocal;
                ViewBag.ClientAllowed = strClientAllowed;

                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(string Subsidiaries, string WareHouses, string ItemCode, string Description, string Category, string Subcategory, string Line, bool Available, bool Stock, bool Blocked = false)
        {
            string strMessage = "";
            try
            {
                IEnumerable<BEA.ProductStock> lstItems = GetItems(Subsidiaries, WareHouses, ItemCode, Description, Category, Subcategory, Line, Available, Stock, Blocked);
                var items = lstItems.Select(x => new
                {
                    x.Product.Category,
                    x.Product.Subcategory,
                    Subsidiary = ToTitle(x.Subsidiary),
                    Warehouse = ToTitle(x.Warehouse),
                    x.ItemCode,
                    ItemName = x.Product.Name,
                    x.Stock,
                    x.Reserved,
                    x.Requested,
                    x.Available,
                    x.Available2,
                    Percentage = GetPercentage(x.Product.Rotation, x.Stock, x.Reserved)
                });

                return Json(new { Message = strMessage, Items = items });
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            return Json(new { Message = strMessage });
        }

        public IActionResult Detail(string ItemCode, string Subsidiary)
        {
            BCA.Product bcItem = new BCA.Product();
            BEA.Product beItem;
            if (Subsidiary.ToLower() == "miami")
            {
                beItem = bcItem.SearchLA(ItemCode);
            }
            else if (Subsidiary.ToLower() == "iquique")
            {
                beItem = bcItem.SearchIQQ(ItemCode);
            }
            else
            {
                beItem = bcItem.SearchSC(ItemCode);
            }
            beItem.Description = SetHTMLSafe(beItem.Description);
            beItem.Commentaries = SetHTMLSafe(beItem.Commentaries);
            return PartialView(beItem);
        }

        public ActionResult ExportExcel(string Subsidiaries, string WareHouses, string ItemCode, string Description, string Category, string Subcategory, string Line, bool Available, bool Stock, bool Blocked = false)
        {
            IEnumerable<BEA.ProductStock> lstItems = GetItems(Subsidiaries, WareHouses, ItemCode, Description, Category, Subcategory, Line, Available, Stock, Blocked);
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
                if (CardCode == HomeCardCode)
                {
                    wsMain.Cells[1, 1, 1, 11].Merge = true;
                }
                else
                {
                    wsMain.Cells[1, 1, 1, 8].Merge = true;
                }

                var imgLogo = wsMain.Drawings.AddPicture("logo", logo);
                imgLogo.SetPosition(5, 5);

                int intFila = 6, intFinalColumn;

                wsMain.Cells[intFila, 1].Value = "Categoría";
                wsMain.Cells[intFila, 2].Value = "Subcategoría";
                wsMain.Cells[intFila, 3].Value = "Sucursal";
                wsMain.Cells[intFila, 4].Value = "Almacén";
                wsMain.Cells[intFila, 5].Value = "Item";
                wsMain.Cells[intFila, 6].Value = "Descripción";
                wsMain.Cells[intFila, 7].Value = "Línea";
                if (CardCode == HomeCardCode)
                {
                    wsMain.Cells[intFila, 8].Value = "Stock";
                    wsMain.Cells[intFila, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsMain.Cells[intFila, 9].Value = "Reserva";
                    wsMain.Cells[intFila, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsMain.Cells[intFila, 10].Value = "Pedido";
                    wsMain.Cells[intFila, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsMain.Cells[intFila, 11].Value = "Disponibilidad";
                    wsMain.Cells[intFila, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    intFinalColumn = 11;
                }
                else
                {
                    wsMain.Cells[intFila, 8].Value = "Disponibilidad";
                    wsMain.Cells[intFila, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    intFinalColumn = 8;
                }

                wsMain.Cells[intFila, 1, intFila, intFinalColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells[intFila, 1, intFila, intFinalColumn].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
                wsMain.Cells[intFila, 1, intFila, intFinalColumn].Style.Font.Color.SetColor(System.Drawing.Color.White);
                wsMain.Cells[intFila, 1, intFila, intFinalColumn].Style.Font.Bold = true;

                if (lstItems != null && lstItems.Any())
                {
                    foreach (var item in lstItems)
                    {
                        intFila += 1;
                        wsMain.Cells[intFila, 1].Value = item.Product.Category;
                        wsMain.Cells[intFila, 2].Value = item.Product.Subcategory;
                        wsMain.Cells[intFila, 3].Value = item.Subsidiary;
                        wsMain.Cells[intFila, 4].Value = item.Warehouse;
                        wsMain.Cells[intFila, 5].Value = item.ItemCode;
                        wsMain.Cells[intFila, 6].Value = item.Product.Name;
                        wsMain.Cells[intFila, 7].Value = item.Product.Line;
                        if (CardCode == HomeCardCode)
                        {
                            wsMain.Cells[intFila, 8].Value = item.Stock;
                            wsMain.Cells[intFila, 9].Value = item.Reserved;
                            wsMain.Cells[intFila, 10].Value = item.Requested;
                            wsMain.Cells[intFila, 11].Value = item.Available;
                        }
                        else
                        {
                            BCP.ClientAllowed bcAllowed = new();
                            string strCardCode = CardCode;
                            IEnumerable<BEP.ClientAllowed> lstAlloweds = bcAllowed.List("1");
                            if (lstAlloweds.Any(x => x.CardCode == strCardCode))
                            {
                                wsMain.Cells[intFila, 8].Value = item.Available2;
                            }
                            else
                            {
                                wsMain.Cells[intFila, 8].Value = GetPercentage(item.Product.Rotation, item.Stock, item.Reserved); //item.Porcentaje.HasValue ? item.Porcentaje.Value.ToString("N2") + " %" : "";
                            }

                            wsMain.Cells[intFila, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
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
                wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:H"];

                string strFileName = "Inventario-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
                byte[] objData = objExcel.GetAsByteArray();
                objExcel.Dispose();
                return File(objData, "application/xlsx", strFileName);
            }
        }

        #endregion

        #region Métodos Privados

        private IEnumerable<BEA.ProductStock> GetItems(string Subsidiaries, string StoreHouses, string ItemCode, string Description, string Category, string Subcategory, string Line, bool Available, bool Stock, bool Blocked)
        {
            BCA.ProductStock bcInventory = new();
            List<Field> lstFilter = new() { new Field { Name = "LOWER(Subsidiary)", Value = Subsidiaries.Trim().ToLower(), Operator = Operators.In } };
            if (Blocked)
            {
                lstFilter.Add(new Field { Name = "Blocked", Value = "Y", Operator = Operators.Equal });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            else
            {
                lstFilter.Add(new Field { Name = "Blocked", Value = "Y", Operator = Operators.Different });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            if (StoreHouses != null && StoreHouses.Trim().Length > 0)
            {
                lstFilter.Add(new Field { Name = "LOWER(Warehouse)", Value = StoreHouses.Trim().ToLower(), Operator = Operators.In });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            if (ItemCode != null && ItemCode.Length > 0)
            {
                lstFilter.Add(new Field { Name = "LOWER(ItemCode)", Value = ItemCode.ToLower(), Operator = Operators.Likes });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            if (Description != null && Description.Length > 0)
            {
                lstFilter.Add(new Field { Name = "LOWER(ItemName)", Value = Description.ToLower(), Operator = Operators.Likes });
                //lstFilter.Add(new Field { Name = "Detalle", Value = Description, Operator = Operators.Likes });
                //lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            if (Category != null && Category.Length > 0)
            {
                lstFilter.Add(new Field { Name = "LOWER(Category)", Value = Category.ToLower() });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            if (Subcategory != null && Subcategory.Length > 0)
            {
                lstFilter.Add(new Field { Name = "LOWER(Subcategory)", Value = Subcategory.ToLower() });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            if (Line != null && Line.Length > 0)
            {
                lstFilter.Add(new Field { Name = "LOWER(Line)", Value = Line.ToLower(), Operator = Operators.In });
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
            string strCardCode = CardCode;
            if (Available)
            {
                if (strCardCode == HomeCardCode)
                {
                    lstFilter.Add(new Field { Name = "( Stock + Reserved + Requested )", Value = 0, Operator = Operators.HigherThan });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                }
                else
                {
                    lstFilter.Add(new Field { Name = "Available2", Value = 0, Operator = Operators.HigherThan });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                }
            }
            if (Stock)
            {
                if (strCardCode == HomeCardCode)
                {
                    lstFilter.Add(new Field { Name = "Stock", Value = 0, Operator = Operators.HigherThan });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                }
            }

            IEnumerable<BEA.ProductStock> lstItems = bcInventory.List(lstFilter, "Subsidiary, ItemCode", BEA.relProductStock.Product) ?? new List<BEA.ProductStock>();
            return lstItems;
        }

        private new static string GetPercentage(string Rotation, decimal Stock, decimal Reserved)
        {
            decimal? decResult = 0;
            if (!string.IsNullOrWhiteSpace(Rotation))
            {
                decResult = (Stock - Reserved) * 100;
                switch (Rotation.ToLower())
                {
                    case "baja":
                        decResult /= 10;
                        break;
                    case "media":
                        decResult /= 50;
                        break;
                    case "intermedia":
                        decResult /= 100;
                        break;
                    case "alta":
                        decResult /= 500;
                        break;
                    default:
                        decResult = null;
                        break;
                }
                if (decResult.HasValue && decResult < 0) decResult = 0;
                if (decResult.HasValue && decResult > 100) decResult = 100;
            }
            string strResult = decResult.HasValue ? decResult.Value.ToString("N0") + "%" : "N/D";
            return strResult;
        }

        #endregion

    }
}