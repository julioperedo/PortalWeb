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
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    public class UnattendedProductsController : BaseController
    {

        #region Variables Globales
        private readonly IConfiguration config;
        #endregion

        #region Constructores

        public UnattendedProductsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { config = configuration; }

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

        //public IActionResult GetProductManagers()
        //{
        //    BCA.Seller bcSalesMan = new BCA.Seller();
        //    List<BEA.Seller> lstItems = bcSalesMan.ProductManagers(new List<Field>(), "Name");
        //    return Json(lstItems);
        //}

        public IActionResult Filter(string Enabled, string WithStock, string InTransit, string WithPrice, string WithCommentaries, string ProductManager, string InWeb, string WithImage, string WithLink, string WithRotation)
        {
            string message = "";
            IEnumerable<BEP.Product> items = new List<BEP.Product>();
            try
            {
                items = GetProducts(Enabled, WithStock, InTransit, WithPrice, WithCommentaries, ProductManager, InWeb, WithImage, WithLink, WithRotation);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        public IActionResult Export(string Enabled, string WithStock, string InTransit, string WithPrice, string WithCommentaries, string ProductManager, string InWeb, string WithImage, string WithLink, string WithRotation)
        {
            using (ExcelPackage objExcel = new())
            {
                IEnumerable<BEP.Product> lstTemp, lstItems = GetProducts(Enabled, WithStock, InTransit, WithPrice, WithCommentaries, ProductManager, InWeb, WithImage, WithLink, WithRotation);
                List<string> lstLines, lstCategories, lstSubcategories, lstManagers;
                lstManagers = (from i in lstItems orderby i.ProductManager select i.ProductManager).Distinct().ToList();

                ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
                FileInfo logoDMC = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo.jpg"));
                var imgLogoDMC = wsMain.Drawings.AddPicture("logoDMC", logoDMC);
                imgLogoDMC.From.Column = 0;
                imgLogoDMC.From.Row = 0;
                imgLogoDMC.From.ColumnOff = 1;
                imgLogoDMC.From.RowOff = 1;
                imgLogoDMC.SetSize(120, 47);
                wsMain.Row(1).Height = 50;

                List<string> lstTitle = new List<string>();
                if (Enabled == "Y") { lstTitle.Add("Habilitados"); }
                if (Enabled == "N") { lstTitle.Add("Deshabilitados"); }
                if (WithStock == "Y") { lstTitle.Add("Con Stock"); }
                if (WithStock == "N") { lstTitle.Add("Sin Stock"); }
                if (InTransit == "Y") { lstTitle.Add("Con Tránsito"); }
                if (InTransit == "N") { lstTitle.Add("Sin Tránsito"); }
                if (WithPrice == "Y") { lstTitle.Add("Con Precio"); }
                if (WithPrice == "N") { lstTitle.Add("Sin Precio"); }
                if (WithCommentaries == "Y") lstTitle.Add("Con Comentarios");
                if (WithCommentaries == "N") lstTitle.Add("Sin Comentarios");
                if (InWeb == "Y") lstTitle.Add("Visible en Web");
                if (InWeb == "N") lstTitle.Add("No visible en Web");
                if (!string.IsNullOrEmpty(WithImage)) { lstTitle.Add(WithImage == "Y" ? "Con Imagen" : "Sin Imagen"); }
                if (!string.IsNullOrEmpty(WithLink)) { lstTitle.Add(WithLink == "Y" ? "Con Vínculo" : "Sin Vínculo"); }
                if (!string.IsNullOrEmpty(WithRotation)) { lstTitle.Add(WithRotation == "Y" ? "Con Rotación" : "Sin Rotación"); }
                if (!string.IsNullOrEmpty(ProductManager)) { lstTitle.Add("De " + ProductManager); }

                wsMain.Cells.Style.Font.Size = 9;
                wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
                wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
                wsMain.Cells[2, 1].Value = "Productos Desatendidos" + (lstTitle.Count > 0 ? (" - " + string.Join(", ", lstTitle)) : "");
                wsMain.Cells[2, 1].Style.Font.Bold = true;
                wsMain.Cells[2, 1].Style.Font.Size = 20;
                wsMain.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsMain.Cells[2, 1, 2, 19].Merge = true;
                wsMain.Cells[2, 1, 2, 19].Style.Fill.BackgroundColor.SetColor(Color.Black);
                wsMain.Cells[2, 1, 2, 19].Style.Font.Color.SetColor(Color.White);

                int intRow = 4;
                foreach (var manager in lstManagers)
                {
                    wsMain.Cells[intRow, 1].Value = "PM: " + manager + " ( Total: " + (from i in lstItems where i.ProductManager == manager select i).Count() + " )";
                    wsMain.Cells[intRow, 1].Style.Font.Bold = true;
                    wsMain.Cells[intRow, 1].Style.Font.Size = 12;
                    wsMain.Cells[intRow, 1, intRow, 19].Merge = true;
                    wsMain.Cells[intRow, 1, intRow, 19].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
                    wsMain.Cells[intRow, 1, intRow, 19].Style.Font.Color.SetColor(Color.White);
                    intRow++;
                    lstLines = (from i in lstItems where i.ProductManager == manager orderby i.Line select i.Line).Distinct().ToList();
                    foreach (var line in lstLines)
                    {
                        wsMain.Cells[intRow, 1].Value = "Línea: " + line + " ( Total: " + (from i in lstItems where i.Line == line select i).Count() + " )";
                        wsMain.Cells[intRow, 1].Style.Font.Bold = true;
                        wsMain.Cells[intRow, 1].Style.Font.Size = 12;
                        wsMain.Cells[intRow, 1, intRow, 19].Merge = true;
                        wsMain.Cells[intRow, 1, intRow, 19].Style.Fill.BackgroundColor.SetColor(Color.Gray);
                        wsMain.Cells[intRow, 1, intRow, 19].Style.Font.Color.SetColor(Color.White);
                        intRow++;
                        lstCategories = (from i in lstItems where i.Line == line orderby i.Category select i.Category).Distinct().ToList();
                        foreach (var category in lstCategories)
                        {
                            wsMain.Cells[intRow, 1].Value = "Categoría: " + category + " ( Total: " + (from i in lstItems where i.Line == line & i.Category == category select i).Count() + " )";
                            wsMain.Cells[intRow, 1].Style.Font.Bold = true;
                            wsMain.Cells[intRow, 1].Style.Font.Size = 11;
                            wsMain.Cells[intRow, 1, intRow, 19].Merge = true;
                            wsMain.Cells[intRow, 1, intRow, 19].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
                            wsMain.Cells[intRow, 1, intRow, 19].Style.Font.Color.SetColor(Color.White);
                            intRow++;
                            lstSubcategories = (from i in lstItems where i.Line == line & i.Category == category orderby i.SubCategory select i.SubCategory).Distinct().ToList();
                            foreach (var subcategory in lstSubcategories)
                            {
                                wsMain.Cells[intRow, 1].Value = "Subcategoría: " + subcategory + " ( Total: " + (from i in lstItems where i.Line == line & i.Category == category & i.SubCategory == subcategory select i).Count() + " )";
                                wsMain.Cells[intRow, 1].Style.Font.Bold = true;
                                wsMain.Cells[intRow, 1].Style.Font.Size = 10;
                                wsMain.Cells[intRow, 1, intRow, 19].Merge = true;
                                wsMain.Cells[intRow, 1, intRow, 19].Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
                                //wsMain.Cells[intRow, 1, intRow, 13].Style.Font.Color.SetColor(System.Drawing.Color.DarkGray);
                                intRow++;
                                wsMain.Cells[intRow, 8].Value = "Precios";
                                wsMain.Cells[intRow, 8, intRow, 10].Merge = true;
                                wsMain.Cells[intRow, 11].Value = "Stock";
                                wsMain.Cells[intRow, 11, intRow, 13].Merge = true;
                                wsMain.Cells[intRow, 14].Value = "Tránsito";
                                wsMain.Cells[intRow, 14, intRow, 16].Merge = true;
                                wsMain.Cells[intRow, 17].Value = "Rotación";
                                wsMain.Cells[intRow, 17, intRow, 19].Merge = true;
                                intRow++;
                                wsMain.Cells[intRow, 1].Value = "Item";
                                wsMain.Cells[intRow, 2].Value = "Nombre";
                                wsMain.Cells[intRow, 3].Value = "Habilitado";
                                wsMain.Cells[intRow, 4].Value = "En Web";
                                wsMain.Cells[intRow, 5].Value = "Comentarios";
                                wsMain.Cells[intRow, 6].Value = "Imagen";
                                wsMain.Cells[intRow, 7].Value = "Link";
                                wsMain.Cells[intRow, 8].Value = "Central";
                                wsMain.Cells[intRow, 9].Value = "LA";
                                wsMain.Cells[intRow, 10].Value = "Iquique";
                                wsMain.Cells[intRow, 11].Value = "Central";
                                wsMain.Cells[intRow, 12].Value = "LA";
                                wsMain.Cells[intRow, 13].Value = "Iquique";
                                wsMain.Cells[intRow, 14].Value = "Central";
                                wsMain.Cells[intRow, 15].Value = "LA";
                                wsMain.Cells[intRow, 16].Value = "Iquique";
                                wsMain.Cells[intRow, 17].Value = "Central";
                                wsMain.Cells[intRow, 18].Value = "LA";
                                wsMain.Cells[intRow, 19].Value = "Iquique";
                                wsMain.Cells[intRow - 1, 1, intRow, 19].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                wsMain.Cells[intRow - 1, 3, intRow - 1, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                wsMain.Cells[intRow, 8, intRow, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                intRow++;
                                lstTemp = (from i in lstItems where i.Line == line & i.Category == category & i.SubCategory == subcategory orderby i.ItemCode select i).ToList();
                                foreach (var item in lstTemp)
                                {
                                    wsMain.Cells[intRow, 1].Value = item.ItemCode;
                                    wsMain.Cells[intRow, 2].Value = item.Name;
                                    wsMain.Cells[intRow, 3].Value = item.Enabled ? "Si" : "No";
                                    wsMain.Cells[intRow, 4].Value = item.ShowInWeb ? "Si" : "No";
                                    wsMain.Cells[intRow, 5].Value = !string.IsNullOrWhiteSpace(item.Commentaries) ? "Si" : "No";
                                    wsMain.Cells[intRow, 6].Value = !string.IsNullOrWhiteSpace(item.ImageURL) ? "Si" : "No";
                                    wsMain.Cells[intRow, 7].Value = !string.IsNullOrWhiteSpace(item.Link) ? "Si" : "No";
                                    wsMain.Cells[intRow, 8].Value = item.PriceSantaCruz;
                                    wsMain.Cells[intRow, 8].Style.Numberformat.Format = "#,##0.00";
                                    wsMain.Cells[intRow, 9].Value = item.PriceMiami;
                                    wsMain.Cells[intRow, 9].Style.Numberformat.Format = "#,##0.00";
                                    wsMain.Cells[intRow, 10].Value = item.PriceIquique;
                                    wsMain.Cells[intRow, 10].Style.Numberformat.Format = "#,##0.00";
                                    wsMain.Cells[intRow, 11].Value = item.SantaCruz + (item.ReservedSantaCruz.HasValue && item.ReservedSantaCruz.Value > 0 ? "( -" + item.ReservedSantaCruz + ")" : "");
                                    wsMain.Cells[intRow, 12].Value = item.Miami + (item.ReservedMiami.HasValue && item.ReservedMiami.Value > 0 ? "( -" + item.ReservedMiami + ")" : "");
                                    wsMain.Cells[intRow, 13].Value = item.Iquique + (item.ReservedIquique.HasValue && item.ReservedIquique.Value > 0 ? "( -" + item.ReservedIquique + ")" : "");
                                    wsMain.Cells[intRow, 14].Value = item.TransitSantaCruz;
                                    wsMain.Cells[intRow, 15].Value = item.TransitMiami;
                                    wsMain.Cells[intRow, 16].Value = item.TransitIquique;
                                    wsMain.Cells[intRow, 17].Value = item.RotationSantaCruz;
                                    wsMain.Cells[intRow, 18].Value = item.RotationMiami;
                                    wsMain.Cells[intRow, 19].Value = item.RotationIquique;
                                    intRow++;
                                }
                            }
                        }
                    }
                }

                wsMain.Cells.AutoFitColumns();

                string strFileName = $"Productos Desatendidos {DateTime.Now:yyyy-MM-dd HHmm}.xlsx";
                byte[] objData = objExcel.GetAsByteArray();
                objExcel.Dispose();
                return File(objData, "application/xlsx", strFileName);
            }
        }

        #endregion

        #region Private Methods

        private static IEnumerable<BEP.Product> GetProducts(string Enabled, string WithStock, string InTransit, string WithPrice, string WithCommentaries, string ProductManager, string InWeb, string WithImage, string WithLink, string WithRotation)
        {
            IEnumerable<BEP.Product> lstProducts, lstItems = new List<BEP.Product>();

            BCP.Product bcProduct = new();
            BCA.ProductStock bcInventory = new();

            List<Field> filters = new()
            {
                new Field("Warehouse", "RESPUESTOS", Operators.NotLikes),
                new Field("Warehouse", "RMA", Operators.NotLikes),
                new Field("Warehouse", "OFFSHORE", Operators.NotLikes),
                new Field("Warehouse", "DEMO", Operators.NotLikes),
                new Field("Warehouse", "NO USAR", Operators.NotLikes),
                new Field("Line", "BLUUMI", Operators.Different),
                new Field("Category", "'REPUESTOS', '__NO_VENDIBLE', 'Z_OTROS'", Operators.NotIn),
                new Field("Stock", 0, Operators.HigherThan),
                new Field("Requested", 0, Operators.HigherThan),
                new Field(LogicalOperators.Or)
            };
            if (!string.IsNullOrEmpty(ProductManager))
            {
                filters.AddRange(new[] { new Field("ProductManager", ProductManager), new Field(LogicalOperators.And) });
            }
            CompleteFilters(ref filters);
            IEnumerable<BEA.ProductStock> lstTempInventory = bcInventory.List(filters, "1");
            var lstInventory = from i in lstTempInventory
                               group i by i.ItemCode into g
                               select new
                               {
                                   ItemCode = g.Key,
                                   StockSA = (from d in g where d.Subsidiary == "Santa Cruz" & !d.Warehouse.ToLower().Contains("transito") select d.Stock).Sum(),
                                   StockLA = (from d in g where d.Subsidiary == "Miami" & !d.Warehouse.ToLower().Contains("transito") select d.Stock).Sum(),
                                   StockIQ = (from d in g where d.Subsidiary == "Iquique" & !d.Warehouse.ToLower().Contains("transito") select d.Stock).Sum(),
                                   TransitSA = (from d in g where d.Subsidiary == "Santa Cruz" select d.Warehouse.ToLower().Contains("transito") ? d.Stock : d.Requested).Sum(),
                                   TransitLA = (from d in g where d.Subsidiary == "Miami" select d.Warehouse.ToLower().Contains("transito") ? d.Stock : d.Requested).Sum(),
                                   TransitIQ = (from d in g where d.Subsidiary == "Iquique" select d.Warehouse.ToLower().Contains("transito") ? d.Stock : d.Requested).Sum(),
                                   ReservedSA = (from d in g where d.Subsidiary == "Santa Cruz" select d.Reserved).Sum(),
                                   ReservedLA = (from d in g where d.Subsidiary == "Miami" select d.Reserved).Sum(),
                                   ReservedIQ = (from d in g where d.Subsidiary == "Iquique" select d.Reserved).Sum(),
                                   RotationSA = (from d in g where d.Subsidiary == "Santa Cruz" select d.Rotation).FirstOrDefault(),
                                   RotationIQ = (from d in g where d.Subsidiary == "Iquique" select d.Rotation).FirstOrDefault(),
                                   RotationLA = (from d in g where d.Subsidiary == "Miami" select d.Rotation).FirstOrDefault()
                               };

            BCP.Price bcPrice = new();
            filters = new List<Field> { new Field("Regular", 0, Operators.HigherThan) };
            IEnumerable<BEP.Price> lstTempPrices = bcPrice.List(filters, "1", BEP.relPrice.Product);

            BCP.PriceOffer bcOffer = new();
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            filters = new List<Field> { new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) };
            CompleteFilters(ref filters);
            IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

            var lstPrices = from p in lstTempPrices
                            group p by p.IdProduct into g
                            select new
                            {
                                IdProduct = g.Key,
                                g.First().Product.ItemCode,
                                SA = offers.Any(x => x.IdProduct == g.Key & x.IdSubsidiary == 1 & x.Price > 0) ? offers.First(x => x.IdProduct == g.Key & x.IdSubsidiary == 1 & x.Price > 0).Price : (g.FirstOrDefault(x => x.IdSudsidiary == 1)?.Regular ?? 0),
                                LA = offers.Any(x => x.IdProduct == g.Key & x.IdSubsidiary == 2 & x.Price > 0) ? offers.First(x => x.IdProduct == g.Key & x.IdSubsidiary == 2 & x.Price > 0).Price : (g.FirstOrDefault(x => x.IdSudsidiary == 2)?.Regular ?? 0),
                                IQ = offers.Any(x => x.IdProduct == g.Key & x.IdSubsidiary == 3 & x.Price > 0) ? offers.First(x => x.IdProduct == g.Key & x.IdSubsidiary == 3 & x.Price > 0).Price : (g.FirstOrDefault(x => x.IdSudsidiary == 3)?.Regular ?? 0)
                            };

            filters = new List<Field> { new Field("Line", "BLUUMI", Operators.Different), new Field("Category", "'REPUESTOS', '__NO_VENDIBLE', 'Z_OTROS'", Operators.NotIn) };
            if (!string.IsNullOrWhiteSpace(ProductManager))
            {
                filters.AddRange(new[] { new Field("ProductManager", ProductManager), new Field(LogicalOperators.And) });
            }
            filters.Add(new Field(LogicalOperators.And));
            BCA.Product bcSProduct = new();
            IEnumerable<BEA.Product> lstSProducts = bcSProduct.ListCatalog(filters, "1");

            filters = new List<Field>();
            lstProducts = bcProduct.List(filters, "1");

            decimal? decNull = null;
            lstItems = from s in lstSProducts
                       join p in lstProducts on s.Code.Trim().ToLower() equals p.ItemCode.Trim().ToLower() into ljProduct
                       from lp in ljProduct.DefaultIfEmpty()
                       join i in lstInventory on s.Code.Trim().ToLower() equals i.ItemCode.Trim().ToLower() into ljInventory
                       from li in ljInventory.DefaultIfEmpty()
                       join r in lstPrices on s.Code.Trim().ToLower() equals r.ItemCode.Trim().ToLower() into ljPrice
                       from lr in ljPrice.DefaultIfEmpty()
                       select new BEP.Product
                       {
                           ItemCode = s.Code,
                           Name = s.Name,
                           Line = s.Line,
                           Category = s.Category,
                           SubCategory = s.Subcategory,
                           ProductManager = s.ProductManager,
                           Commentaries = lp?.Commentaries ?? "",
                           Enabled = lp?.Enabled ?? false,
                           ShowInWeb = lp?.ShowInWeb ?? false,
                           ImageURL = lp?.ImageURL ?? "",
                           Link = lp?.Link ?? "",
                           RotationSantaCruz = li?.RotationSA ?? "",
                           RotationIquique = li?.RotationIQ ?? "",
                           RotationMiami = li?.RotationLA ?? "",
                           SantaCruz = (li != null ? (li.StockSA != 0 ? li.StockSA : decNull) : decNull),
                           Miami = (li != null ? (li.StockLA != 0 ? li.StockLA : decNull) : decNull),
                           Iquique = (li != null ? (li.StockIQ != 0 ? li.StockIQ : decNull) : decNull),
                           TransitSantaCruz = (li != null ? (li.TransitSA != 0 ? li.TransitSA : decNull) : decNull),
                           TransitMiami = (li != null ? (li.TransitLA != 0 ? li.TransitLA : decNull) : decNull),
                           TransitIquique = (li != null ? (li.TransitIQ != 0 ? li.TransitIQ : decNull) : decNull),
                           PriceSantaCruz = (lr != null ? (lr.SA != 0 ? lr.SA : decNull) : decNull),
                           PriceMiami = (lr != null ? (lr.LA != 0 ? lr.LA : decNull) : decNull),
                           PriceIquique = (lr != null ? (lr.IQ != 0 ? lr.IQ : decNull) : decNull),
                           ReservedSantaCruz = (li != null ? (li?.ReservedSA != 0 ? li.ReservedSA : decNull) : decNull),
                           ReservedIquique = (li != null ? (li?.ReservedIQ != 0 ? li.ReservedIQ : decNull) : decNull),
                           ReservedMiami = (li != null ? (li?.ReservedLA != 0 ? li.ReservedLA : decNull) : decNull)
                       };

            if (Enabled == "Y")
            {
                lstItems = lstItems.Where(i => i.Enabled);
            }
            else if (Enabled == "N")
            {
                lstItems = lstItems.Where(i => !i.Enabled);
            }
            if (WithStock == "Y")
            {
                lstItems = lstItems.Where(i => (i.SantaCruz.HasValue && i.SantaCruz.Value > 0) | (i.Miami.HasValue && i.Miami.Value > 0) | (i.Iquique.HasValue && i.Iquique.Value > 0));
            }
            else if (WithStock == "N")
            {
                lstItems = lstItems.Where(i => (!i.SantaCruz.HasValue || i.SantaCruz.Value <= 0) & (!i.Miami.HasValue || i.Miami.Value <= 0) & (!i.Iquique.HasValue || i.Iquique.Value <= 0));
            }
            if (InTransit == "Y")
            {
                lstItems = (from i in lstItems where (i.TransitSantaCruz.HasValue && i.TransitSantaCruz.Value > 0) | (i.TransitMiami.HasValue && i.TransitMiami.Value > 0) | (i.TransitIquique.HasValue && i.TransitIquique.Value > 0) select i).ToList();
            }
            else if (InTransit == "N")
            {
                lstItems = (from i in lstItems where i.TransitSantaCruz.HasValue == false & i.TransitMiami.HasValue == false & i.TransitIquique.HasValue == false select i).ToList();
            }
            if (WithPrice == "Y")
            {
                lstItems = (from i in lstItems where i.PriceSantaCruz.HasValue | i.PriceMiami.HasValue | i.PriceIquique.HasValue select i).ToList();
            }
            else if (WithPrice == "N")
            {
                lstItems = (from i in lstItems where i.PriceSantaCruz.HasValue == false & i.PriceMiami.HasValue == false & i.PriceIquique.HasValue == false select i).ToList();
            }
            if (WithCommentaries == "Y")
            {
                lstItems = lstItems.Where(i => !string.IsNullOrWhiteSpace(i.Commentaries));
            }
            else if (WithCommentaries == "N")
            {
                lstItems = lstItems.Where(i => string.IsNullOrWhiteSpace(i.Commentaries));
            }
            if (InWeb == "Y")
            {
                lstItems = lstItems.Where(i => i.ShowInWeb == true);
            }
            else if (InWeb == "N")
            {
                lstItems = lstItems.Where(i => i.ShowInWeb == false);
            }
            if (WithImage == "Y")
            {
                lstItems = lstItems.Where(x => !string.IsNullOrWhiteSpace(x.ImageURL));
            }
            else if (WithImage == "N")
            {
                lstItems = lstItems.Where(x => string.IsNullOrWhiteSpace(x.ImageURL));
            }
            if (WithLink == "Y")
            {
                lstItems = lstItems.Where(x => !string.IsNullOrWhiteSpace(x.Link));
            }
            else if (WithLink == "N")
            {
                lstItems = lstItems.Where(x => string.IsNullOrWhiteSpace(x.Link));
            }
            if (WithRotation == "Y")
            {
                lstItems = lstItems.Where(x => !string.IsNullOrWhiteSpace(x.RotationSantaCruz) & !string.IsNullOrWhiteSpace(x.RotationIquique) & !string.IsNullOrWhiteSpace(x.RotationMiami));
            }
            else if (WithRotation == "N")
            {
                lstItems = lstItems.Where(x => string.IsNullOrWhiteSpace(x.RotationSantaCruz) | string.IsNullOrWhiteSpace(x.RotationIquique) | string.IsNullOrWhiteSpace(x.RotationMiami));
            }
            return lstItems;
        }

        #endregion

    }
}