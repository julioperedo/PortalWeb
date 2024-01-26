using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BEntities.Enums;
using BEntities.Filters;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUglify.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Areas.Product.Models;
using Portal.Controllers;
using Portal.Misc;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCF = BComponents.Staff;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEF = BEntities.Staff;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class PriceListController : BaseController
    {
        #region Constructores

        public PriceListController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewBag.LocalUser = CardCode == HomeCardCode ? "Y" : "N";

                BCP.ClientAllowed bcClientAllowed = new();
                ViewBag.SeeStock = bcClientAllowed.IsAllowed(CardCode) ? "Y" : "N";
                ViewBag.SeeProviderOrders = GetPermission("PriceList-SeeProviderOrders") > 0 ? "Y" : "N";
                ViewBag.ProductRequirements = GetPermission("PriceList-ProductRequest") > 0 ? "Y" : "N";

                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetAvailablesLines()
        {
            string message = "";
            try
            {
                BCS.User bcUser = new();
                BES.User user = bcUser.Search(UserCode);

                BCP.Line bcLine = new();
                List<Line> lstResult;
                IEnumerable<BEP.Line> lstLines = bcLine.ListForPriceList(CardCode == HomeCardCode, "Name");

                BCS.LineNotAllowed bcClient = new();
                List<Field> lstFilter = new() { new Field("CardCode", CardCode) };
                var lstClientExceptions = bcClient.List(lstFilter, "1");

                lstResult = (from l in lstLines
                             where CardCode == HomeCardCode || user.AllowLinesBlocked || l.WhenFilteredShowInfo || ((l.FilterType == "AllBut" & !(from a in lstClientExceptions select a.IdLine).Contains(l.Id)) || (l.FilterType == "NoneBut" & (from a in lstClientExceptions select a.IdLine).Contains(l.Id)))
                             group l by l.ImageURL into g
                             select new Line { Id = g.First().Id, Name = g.First().Name, ImageURL = g.Key, SubLines = (from d in g select new Models.Line { Id = d.Id, Name = d.Name }).ToList() }).ToList();

                BCP.Product bcProduct = new();
                int countWithOffer = bcProduct.CountWithOffer(new List<Field>());
                if (countWithOffer > 0)
                {
                    lstResult.Add(new Line { Id = 0, Name = "DMC Ofertas", ImageURL = "ofertas.png", SubLines = new List<Line> { new Line(-10, "Todas las Ofertas"), new Line(-11, "Ofertas Santa Cruz"), new Line(-13, "Ofertas Iquique"), new Line(-12, "Ofertas Miami") } });
                }
                lstResult.Add(new Line { Id = -2, Name = "Productos Nuevos", ImageURL = "Nuevo.png", SubLines = new List<Line>() });
                BCP.OpenBox bcOBox = new();
                var lstOpenBox = bcOBox.List(new List<Field> { new Field("Enabled", 1) }, "1", BEP.relOpenBox.Product, BEP.relOpenBox.Subsidiary);

                if (lstOpenBox?.Count() > 0)
                {
                    //BCA.ProductStock bcInventory = new BCA.ProductStock();
                    //var filters = new List<Field> { new Field("ItemCode", string.Join(",", (from x in lstOpenBox where !string.IsNullOrEmpty(x.Product?.ItemCode) select $"'{x.Product.ItemCode}'")), Operators.In) };
                    //List<BEA.ProductStock> stockItems = new List<BEA.ProductStock>();
                    //try
                    //{
                    //    stockItems = bcInventory.List(filters, "1");
                    //}
                    //catch (Exception)
                    //{
                    //    stockItems = new List<BEA.ProductStock>();
                    //}
                    //lstOpenBox.ForEach(x => x.WithStock = (from s in stockItems where s.Subsidiary.ToLower() == x.Subsidiary.Name.ToLower() & s.Warehouse.ToLower() == x.Warehouse.ToLower() & s.ItemCode.ToLower() == x.Product?.ItemCode?.ToLower() select s.Stock).Sum() > 0);
                    //if (lstOpenBox.Count(x => x.WithStock) > 0)
                    //{
                    //    lstResult.Add(new Models.Line { Id = -3, Name = "Productos Abiertos", ImageURL = "openbox.png", SubLines = new List<Models.Line>() });
                    //}
                    lstResult.Add(new Line { Id = -3, Name = "Productos Abiertos", ImageURL = "openbox.png", SubLines = new List<Models.Line>() });
                }
                lstResult.Add(new Line { Id = -1, Name = "DMC Staff", ImageURL = "staff.png", SubLines = new List<Line>() });
                var items = lstResult.Select(x => new { x.Id, x.Name, x.ImageURL, SubLines = x.SubLines.Select(i => new { i.Id, i.Name }) });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetStaff()
        {
            BCF.Department bcDepartment = new();
            IEnumerable<BEF.Department> lstDepartments = bcDepartment.List("Name", BEF.relDepartment.Members);

            Misc.Models.Staff objStaff = new()
            {
                Departments = (from d in lstDepartments
                               orderby d.Order
                               select new Misc.Models.Department
                               {
                                   Name = d.Name,
                                   ClassName = d.Id == 1 ? "" : "managers",
                                   Managers = (from m in d.ListMembers
                                               where m.Manager
                                               orderby m.Order
                                               select new Misc.Models.Contact { Name = m.Name, Position = m.Position, Mail = m.Mail, Photo = m.Photo, Phone = m.Phone }).ToList(),
                                   Members = (from m in d.ListMembers
                                              where !m.Manager
                                              orderby m.Order
                                              select new Misc.Models.Contact { Name = m.Name, Position = m.Position, Mail = m.Mail, Photo = m.Photo, Phone = m.Phone }).ToList()
                               }).ToList()
            };
            return Json(objStaff);
        }

        public IActionResult Lines(long? IdLine, string Category, string Subcategory, string Filter)
        {
            string message = "";
            try
            {
                IEnumerable<LineShort> items = GetItems2(IdLine, Category, Subcategory, Filter);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetStock(string ItemCodes)
        {
            string message = "";
            try
            {
                BCA.ProductStock bcStock = new();
                List<Field> filters = new()
                {
                    new Field("LOWER(ItemCode)", ItemCodes.ToLower(), Operators.In),
                    new Field("Stock", 0, Operators.HigherThan),
                    new Field("Requested", 0, Operators.HigherThan),
                    new Field("Reserved", 0, Operators.HigherThan),
                    new Field(LogicalOperators.Or),
                    new Field(LogicalOperators.Or),
                    new Field(LogicalOperators.And)
                };
                var stockItems = bcStock.List(filters, "1");

                BCP.WarehouseAllowed bcWarehouse = new();
                var allowedWarehouses = bcWarehouse.List("1");

                filters = new() {
                    Field.New("LOWER(ItemCode)", ItemCodes.ToLower(), Operators.In), Field.New(@"IFNULL(ArrivalDate, '')", "", Operators.Different), Field.New("Stock", 0, Operators.HigherThan),
                    Field.New("Warehouse", "TRANSITO", Operators.Likes), Field.LogicalAnd(), Field.LogicalAnd(), Field.LogicalAnd()
                };
                var newArrivals = bcStock.ListBalance(filters, "1");
                bool isLocalUser = CardCode == HomeCardCode;

                var codes = ItemCodes.Replace("'", "").Split(',');
                var tempItems = from i in stockItems
                                    //where (from a in allowedWarehouses where a.Subsidiary.ToLower() == i.Subsidiary.ToLower() & (a.ClientVisible | isLocalUser) select a.Name.ToLower()).Contains(i.Warehouse.ToLower())
                                group i by i.ItemCode.ToUpper() into g
                                select new
                                {
                                    ItemCode = g.Key,
                                    Subsidiaries = from s in g
                                                   group s by s.Subsidiary into sg
                                                   select new
                                                   {
                                                       Name = ToTitle(sg.Key),
                                                       Items = sg.Select(x => new
                                                       {
                                                           Warehouse = ToTitle(x.Warehouse),
                                                           x.Stock,
                                                           x.Requested,
                                                           x.Reserved,
                                                           x.Available,
                                                           x.Available2,
                                                           Percentage = GetPercentage(x.Rotation, x.Stock, x.Reserved),
                                                           clientVisible = allowedWarehouses.FirstOrDefault(a => a.Subsidiary.ToLower() == sg.Key.ToLower() && a.Name.ToLower() == x.Warehouse.ToLower())?.ClientVisible ?? false,
                                                           arrivalDate = newArrivals.FirstOrDefault(a => a.ItemCode.ToUpper() == g.Key & a.Subsidiary.ToLower() == x.Subsidiary.ToLower() & a.Warehouse.ToLower() == x.Warehouse.ToLower())?.ArrivalDate,
                                                       })
                                                   }
                                };
                var items = from c in codes
                            join i in tempItems on c.ToLower() equals i.ItemCode.ToLower() into lj
                            from l in lj.DefaultIfEmpty()
                            select new { ItemCode = c, l?.Subsidiaries };

                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetOpenBoxes()
        {
            string message = "";
            try
            {
                BCP.OpenBox bcOBox = new();
                IEnumerable<BEP.OpenBox> openBoxes = bcOBox.List(new List<Field> { new Field("Enabled", true) }, "IdSubsidiary", BEP.relOpenBox.Product, BEP.relOpenBox.Subsidiary);

                BCA.ProductStock bcInventory = new();
                var filters = new List<Field> { new Field("ItemCode", string.Join(",", (from o in openBoxes where !string.IsNullOrEmpty(o.Product?.ItemCode) select $"'{o.Product.ItemCode}'")), Operators.In) };
                var stockItems = bcInventory.List(filters, "1");

                openBoxes.ForEach(x => x.WithStock = (from s in stockItems
                                                      where s.Subsidiary.ToLower() == x.Subsidiary.Name.ToLower() & s.Warehouse.ToLower() == x.Warehouse.ToLower() & s.ItemCode.ToLower() == x.Product?.ItemCode?.ToLower()
                                                      select s.Stock).Sum() > 0);

                IEnumerable<BES.LineNotAllowed> lstNotAllowed = new List<BES.LineNotAllowed>();
                if (CardCode != HomeCardCode)
                {
                    BCS.LineNotAllowed bcClient = new();
                    filters = new List<Field> { new Field("CardCode", CardCode) };
                    lstNotAllowed = bcClient.List(filters, "1", BES.relLineNotAllowed.Line, BEP.relLine.LineDetails);
                }
                List<string> linesNotAllowed = new();
                lstNotAllowed.ForEach(item => item.Line.ListLineDetails.ForEach(x => linesNotAllowed.Add(x.SAPLine.ToLower())));

                BCS.User bcUser = new();
                BES.User user = bcUser.Search(UserCode);

                var items = from x in openBoxes
                            where x.WithStock & (!string.IsNullOrEmpty(x.Product?.ItemCode) && (!linesNotAllowed.Contains(x.Product.Line.ToLower()) || user.AllowLinesBlocked))
                            select new Models.OpenBox
                            {
                                Id = x.IdProduct,
                                Subsidiary = x.Subsidiary.Name,
                                ItemCode = x.Product.ItemCode,
                                ProductName = x.Product.Name,
                                ProductDesc = x.Product.Description?.Replace("\r", "<br />") ?? "",
                                Comments = x.Comments?.Replace("\r", "<br />") ?? "",
                                Quantity = x.Quantity,
                                Price = x.Price,
                                ImageUrl = x.Product.ImageURL,
                                Link = x.Product.Link ?? "",
                                ProductComments = x.Product.Commentaries?.Replace("\r", "<br />") ?? ""
                            };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetReservedItems(string Subsidiary, string Warehouse, string ItemCode)
        {
            string message = "";
            try
            {
                bool seeReservedIQ = GetPermission("SeeReservedIQ") > 0, seeReservedLA = GetPermission("SeeReservedLA") > 0, seeReservedSC = GetPermission("SeeReservedSC") > 0;
                BCS.UserData bcUserData = new();
                var beUserData = bcUserData.SearchByUser(UserCode);
                string currentCode = beUserData?.SellerCode ?? "";
                decimal? decNull = null;

                bool seeReserved(string subsidiary, string code)
                {
                    bool result = false;
                    if (subsidiary.ToLower() == "santa cruz" & (seeReservedSC | code == currentCode))
                    {
                        result = true;
                    }
                    if (subsidiary.ToLower() == "iquique" & (seeReservedIQ | code == currentCode))
                    {
                        result = true;
                    }
                    if (subsidiary.ToLower() == "miami" & (seeReservedLA | code == currentCode))
                    {
                        result = true;
                    }
                    return result;
                }

                BCA.Order bcOrder = new();
                var reserved = bcOrder.ListReserved(Subsidiary, Warehouse, ItemCode);
                BCA.OrderFile bcFile = new();
                IEnumerable<BEA.OrderFile> files = bcFile.List((from o in reserved select o.DocEntry.ToString()).ToList());
                var lstReFiles = from f in files
                                 group f by new { f.Subsidiary, f.DocEntry } into g
                                 select new { g.Key.Subsidiary, g.Key.DocEntry, Count = g.Count(), Files = (from d in g select $"{d.FileName}.{d.FileExt}").ToList() };
                var items = from o in reserved
                            join f in lstReFiles on new { o.DocEntry, Subsidiary = o.Subsidiary.ToLower() } equals new { f.DocEntry, Subsidiary = f.Subsidiary.ToLower() } into ljFiles
                            from lf in ljFiles.DefaultIfEmpty()
                            orderby o.DocNum
                            select new
                            {
                                o.DocNum,
                                ClientCode = seeReserved(Subsidiary, o.SellerCode) ? o.ClientCode : "",
                                ClientName = seeReserved(Subsidiary, o.SellerCode) ? o.ClientName : "",
                                o.DocDate,
                                Price = seeReserved(Subsidiary, o.SellerCode) ? o.Price : decNull,
                                o.Quantity,
                                SellerCode = o.SellerCode ?? "",
                                SellerName = o.SellerName ?? "",
                                o.Subsidiary,
                                o.DocEntry,
                                o.Authorized,
                                Correlative = o.Correlative ?? "",
                                HasFiles = lf != null && lf.Count > 0,
                                Files = lf != null ? lf.Files : new List<string>()
                            };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetItems(string Ids)
        {
            string message = "";
            try
            {
                BCP.ClientAllowed bcClientAllowed = new();
                IEnumerable<Models.LineShort> lstLines = GetItems2(null, null, null, null, Ids); //GetItemsByIds(Ids);
                var lines = (from l in lstLines
                             select new
                             {
                                 l.Name,
                                 Categories = l.Categories.Select(c => new
                                 {
                                     c.Name,
                                     Subcategories = c.Subcategories.Select(s => new
                                     {
                                         s.Name,
                                         Products = s.Products.Select(p => new
                                         {
                                             p.ItemCode,
                                             Name = $"{p.Name} <br />{p.Description}",
                                             Warranty = p.Warranty ?? "",
                                             Prices = p.Prices.Select(r => new { r.Subsidiary, r.Regular, r.Offer, Volume = r.Volume.Select(v => new { v.Price, v.Quantity }) })
                                         })
                                     })
                                 })
                             }).ToList();
                List<string> productCodesList = new();
                foreach (var line in lstLines)
                {
                    foreach (var category in line.Categories)
                    {
                        foreach (var subcategory in category.Subcategories)
                        {
                            productCodesList.AddRange(subcategory.Products.Select(x => x.ItemCode));
                        }
                    }
                }

                string productCodes = string.Join(",", productCodesList.Select(x => $"'{x}'"));
                BCA.ProductStock bcStock = new();
                var filters = new List<Field> {
                    new Field("ItemCode", productCodes, Operators.In), new Field("Stock", 0, Operators.HigherThan), new Field("Requested", 0, Operators.HigherThan), new Field("Reserved", 0, Operators.HigherThan),
                    new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.And)
                };
                IEnumerable<BEA.ProductStock> stockList = bcStock.List(filters, "1");

                BCP.WarehouseAllowed bcWarehouse = new();
                var allowedWarehouses = bcWarehouse.List("1");

                var stockItems = from i in stockList
                                 where (from a in allowedWarehouses where a.Subsidiary.ToLower() == i.Subsidiary.ToLower() & (a.ClientVisible | CardCode == HomeCardCode) select a.Name.ToLower()).Contains(i.Warehouse.ToLower())
                                 select new { i.ItemCode, i.Subsidiary, i.Warehouse, i.Stock, i.Reserved, i.Available, i.Available2, i.Requested, i.Percentage };
                return Json(new { message, lines, stockItems });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ExportExcel(string Ids, bool WithDetail = false, bool WithTransit = false)
        {
            using ExcelPackage objExcel = new();
            BCP.ClientAllowed bcClientAllowed = new();
            bool boSeeStock = bcClientAllowed.IsAllowed(CardCode), boLocal = CardCode == HomeCardCode;
            IEnumerable<Models.LineShort> lstLines = GetItems2(null, null, null, null, Ids); //GetItemsByIds(Ids);
            List<string> productCodesList = new();
            foreach (var line in lstLines)
            {
                foreach (var category in line.Categories)
                {
                    foreach (var subcategory in category.Subcategories)
                    {
                        productCodesList.AddRange(subcategory.Products.Select(x => x.ItemCode));
                    }
                }
            }

            string productCodes = string.Join(",", productCodesList.Select(x => $"'{x}'"));
            BCA.ProductStock bcStock = new();
            var filters = new List<Field> {
                new Field("ItemCode", productCodes, Operators.In), new Field("Stock", 0, Operators.HigherThan), new Field("Requested", 0, Operators.HigherThan), new Field("Reserved", 0, Operators.HigherThan),
                new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.And)
            };
            IEnumerable<BEA.ProductStock> stockList = new List<BEA.ProductStock>();
            try
            {
                stockList = bcStock.List(filters, "1");
            }
            catch (Exception) { }

            BCP.WarehouseAllowed bcWarehouse = new();
            var allowedWarehouses = bcWarehouse.List("1");

            //var stockItems = from i in stockList
            //                 where (from a in allowedWarehouses where a.Subsidiary.ToLower() == i.Subsidiary.ToLower() & (a.ClientVisible | CardCode == HomeCardCode) select a.Name.ToLower()).Contains(i.Warehouse.ToLower())
            //                 select new { i.ItemCode, i.Subsidiary, i.Warehouse, i.Stock, i.Reserved, i.Available, i.Available2, i.Requested, Percentage = GetPercentage(i.Rotation, i.Stock, i.Reserved) };
            bool isLocalUser = CardCode == HomeCardCode;
            var stockItems = from i in stockList
                             where (from a in allowedWarehouses where a.Subsidiary.ToLower() == i.Subsidiary.ToLower() & (a.ClientVisible | CardCode == HomeCardCode) select a.Name.ToLower()).Contains(i.Warehouse.ToLower())
                             select new { i.ItemCode, i.Subsidiary, i.Warehouse, i.Stock, i.Reserved, i.Available, i.Available2, i.Requested, Percentage = isLocalUser || i.Available2 <= 50 ? (i.Available2 > 0 ? i.Available2.ToString() : "0") : "+50" };
            SetCultureUI();

            long IdManager = 0;
            if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
            {
                BCF.Member bcMember = new();
                var member = bcMember.SearchByMail(EMail);
                IdManager = member?.Id ?? 0;
            }

            ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
            FileInfo logoDMC = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));
            var imgLogoDMC = wsMain.Drawings.AddPicture("logoDMC", logoDMC);
            imgLogoDMC.From.Column = 0;
            imgLogoDMC.From.Row = 0;
            imgLogoDMC.From.ColumnOff = 1;
            imgLogoDMC.From.RowOff = 1;
            imgLogoDMC.SetSize(120, 47);
            wsMain.Row(1).Height = 50;

            wsMain.Cells.Style.Font.Size = 10;
            wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
            wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
            wsMain.Cells[2, 1].Value = $"Precios Válidos para el {DateTime.Now:dd/MM/yyyy}";
            wsMain.Cells[2, 1].Style.Font.Bold = true;
            wsMain.Cells[2, 1].Style.Font.Size = 20;
            wsMain.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[2, 1, 2, 17].Merge = true;
            wsMain.Cells[2, 1, 2, 17].Style.Fill.BackgroundColor.SetColor(Color.Black);
            wsMain.Cells[2, 1, 2, 17].Style.Font.Color.SetColor(Color.White);

            List<string> lstLogos = (from l in lstLines select l.ImageURL).Distinct().ToList();
            int intRow = 3, intCol = 1, tempRow, rowHeight = 0;
            wsMain.Column(1).Width = 20;
            wsMain.Column(3).Width = 20;
            wsMain.Column(5).Width = 20;
            wsMain.Column(7).Width = 20;
            wsMain.Column(9).Width = 20;
            wsMain.Column(11).Width = 20;
            decimal maxHeight = 90, maxWidth = 120, logoHeight, logoWidth;
            foreach (var strLogo in lstLogos)
            {
                List<Models.LineShort> lstTLines = lstLines.Where(x => x.ImageURL == strLogo).ToList();
                string lineLogoPath = Path.Combine(rootDirectory, "wwwroot", "images", "lines", strLogo);
                FileInfo logo = new(lineLogoPath);
                OfficeOpenXml.Drawing.ExcelPicture imgMainLogo;
                if (lstTLines.Count == 1)
                {
                    ExcelHyperLink lnkLine = new($"#'{ExcelSafeName(lstTLines.First().Name)}'!E1", UriKind.Relative);
                    imgMainLogo = wsMain.Drawings.AddPicture($"logo_{strLogo.Split('.').First()}", logo, lnkLine);
                }
                else
                {
                    imgMainLogo = wsMain.Drawings.AddPicture($"logo_{strLogo.Split('.').First()}", logo);
                }

                int tempWidth, tempHeight;
                using (var image = SixLabors.ImageSharp.Image.Load(lineLogoPath))
                {
                    tempWidth = image.Width;
                    tempHeight = image.Height;
                }

                imgMainLogo.From.Column = intCol - 1;
                imgMainLogo.From.Row = intRow - 1;

                logoHeight = tempHeight;
                logoWidth = tempHeight;
                if (logoWidth > maxWidth)
                {
                    logoWidth = maxWidth;
                    logoHeight = (maxWidth * tempHeight) / tempWidth;
                }
                if (logoHeight > maxHeight)
                {
                    logoHeight = maxHeight;
                    logoWidth = (maxHeight * tempWidth) / tempHeight;
                }

                imgMainLogo.SetSize(decimal.ToInt32(logoWidth), decimal.ToInt32(logoHeight));

                imgMainLogo.AdjustPositionAndSize();
                wsMain.Row(intRow).Height = 70;

                tempRow = intRow;
                if (lstTLines.Count > rowHeight) rowHeight = lstTLines.Count;
                if (lstTLines.Count > 1)
                {
                    foreach (var line in lstTLines)
                    {
                        tempRow++;
                        wsMain.Cells[tempRow, intCol].Hyperlink = new ExcelHyperLink($"'{ExcelSafeName(line.Name)}'!E1", line.Name);
                    }
                }
                if (intCol == 11)
                {
                    intCol = 1;
                    intRow += 2 + rowHeight;
                    rowHeight = 0;
                }
                else
                {
                    intCol += 2;
                }
            }
            intRow += 8;
            tempRow = intRow;
            wsMain.Cells[intRow, 1, intRow + 5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[intRow, 1, intRow + 5, 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            wsMain.Cells[intRow, 1, intRow, 3].Merge = true;
            wsMain.Cells[intRow++, 1].Value = "DMC S.A.";
            wsMain.Cells[intRow, 1, intRow, 3].Merge = true;
            wsMain.Cells[intRow++, 1].Value = "Av. Grigota # 3800";
            wsMain.Cells[intRow, 1, intRow, 3].Merge = true;
            wsMain.Cells[intRow++, 1].Value = "Teléfono: 354 3000";
            wsMain.Cells[intRow, 1, intRow, 3].Merge = true;
            wsMain.Cells[intRow++, 1].Value = "Fax: 354 3637";
            wsMain.Cells[intRow, 1, intRow, 3].Merge = true;
            wsMain.Cells[intRow++, 1].Value = "Santa Cruz - Bolivia";
            wsMain.Cells[intRow, 1, intRow, 3].Merge = true;
            wsMain.Cells[intRow++, 1].Value = "Contacto Comercial: Departamento de Ventas";

            intRow = tempRow;
            wsMain.Cells[intRow, 5, intRow + 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[intRow, 5, intRow + 5, 5].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
            wsMain.Cells[intRow, 5, intRow, 7].Merge = true;
            wsMain.Cells[intRow++, 5].Value = "DMC LATIN AMERICA INC.";
            wsMain.Cells[intRow, 5, intRow, 7].Merge = true;
            wsMain.Cells[intRow++, 5].Value = "9935 NW 88 Ave.";
            wsMain.Cells[intRow, 5, intRow, 7].Merge = true;
            wsMain.Cells[intRow++, 5].Value = "Miami, FL 73178";
            wsMain.Cells[intRow, 5, intRow, 7].Merge = true;
            wsMain.Cells[intRow++, 5].Value = "Contacto Comercial: Receiving Department";
            wsMain.Cells[intRow, 5, intRow, 7].Merge = true;
            wsMain.Cells[intRow++, 5].Value = "Teléfono: 786 245 4457";
            wsMain.Cells[intRow, 5, intRow, 7].Merge = true;
            wsMain.Cells[intRow++, 5].Value = "ENTREGAS FOB MIAMI se realizan todos los dias excepto Viernes.";

            intRow = tempRow;
            wsMain.Cells[intRow, 9, intRow + 5, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[intRow, 9, intRow + 5, 9].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            wsMain.Cells[intRow, 9, intRow, 11].Merge = true;
            wsMain.Cells[intRow++, 9].Value = "DMC LATIN AMERICA INC.";
            wsMain.Cells[intRow, 9, intRow, 11].Merge = true;
            wsMain.Cells[intRow++, 9].Value = "9935 NW 88 Ave.";
            wsMain.Cells[intRow, 9, intRow, 11].Merge = true;
            wsMain.Cells[intRow++, 9].Value = "Miami, FL 33178";
            wsMain.Cells[intRow, 9, intRow, 11].Merge = true;
            wsMain.Cells[intRow++, 9].Value = "Contacto Comercial: Receiving Department";
            wsMain.Cells[intRow, 9, intRow, 11].Merge = true;
            wsMain.Cells[intRow++, 9].Value = "Teléfono: 786 245 4457";
            wsMain.Cells[intRow, 9, intRow, 11].Merge = true;
            wsMain.Cells[intRow++, 9].Value = "ENTREGAS FOB MIAMI se realizan todos los dias excepto Viernes.";

            foreach (Models.LineShort line in lstLines)
            {
                List<string> subsidiaries = new();
                foreach (var tempCat in line.Categories)
                {
                    foreach (var tempSub in tempCat.Subcategories)
                    {
                        foreach (var tempProd in tempSub.Products)
                        {
                            foreach (var tempPrice in tempProd.Prices)
                            {
                                if (!subsidiaries.Contains(tempPrice.Subsidiary))
                                {
                                    subsidiaries.Add(tempPrice.Subsidiary);
                                }
                            }
                        }
                    }
                }
                ExcelWorksheet wsLine = objExcel.Workbook.Worksheets.Add(line.Id.ToString());
                wsLine.DefaultColWidth = 15;
                if (line.ImageURL != null && line.ImageURL.Trim().Length > 0)
                {
                    string logoLinePath = Path.Combine(rootDirectory, "wwwroot", "images", "lines", line.ImageURL);
                    FileInfo logoLine = new(logoLinePath);

                    var imgLogo = wsLine.Drawings.AddPicture("logo", logoLine);
                    imgLogo.From.Column = 0;
                    imgLogo.From.Row = 0;

                    int tempWidth, tempHeight;
                    using (var image = SixLabors.ImageSharp.Image.Load(logoLinePath))
                    {
                        tempWidth = image.Width;
                        tempHeight = image.Height;
                    }

                    logoHeight = tempHeight;
                    logoWidth = tempHeight;
                    if (logoWidth > maxWidth)
                    {
                        logoWidth = maxWidth;
                        logoHeight = (maxWidth * tempHeight) / tempWidth;
                    }
                    if (logoHeight > maxHeight)
                    {
                        logoHeight = maxHeight;
                        logoWidth = (maxHeight * tempWidth) / tempHeight;
                    }
                    imgLogo.SetSize(decimal.ToInt32(logoWidth), decimal.ToInt32(logoHeight));
                }
                wsLine.Column(1).Width = 20;
                wsLine.Column(2).Width = 90;
                //wsLine.Column(5).Width = 22;
                //wsLine.Column(7).Width = 22;
                //wsLine.Column(9).Width = 22;

                int intPrices = line.Categories.Max(x => x.Subcategories.Max(y => y.Products.Max(z => z.Prices.Count))); //line.Subsidiaries.Count;
                wsLine.Name = ExcelSafeName(line.Name);

                wsLine.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsLine.Cells.Style.Fill.PatternColor.SetColor(Color.White);
                wsLine.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
                wsLine.Cells.Style.Font.Size = 8;

                //ExcelHyperLink lnkLine = new ExcelHyperLink("#'Main'!A1", UriKind.Relative);
                //Image logo = Image.FromFile(Path.Combine(rootDirectory, "wwwroot", "images", "lines", strLogo));
                //Image logoBack = Image.FromFile(Server.MapPath("~/Content/img/Volver.png"));
                //OfficeOpenXml.Drawing.ExcelPicture imgBackLogo = wsLine.Drawings.AddPicture($"logo_Back_{line.Id}", logoBack, lnkLine);
                //imgBackLogo.From.Column = 4;
                //imgBackLogo.From.Row = 0;
                //imgBackLogo.From.ColumnOff = 1;
                //imgBackLogo.From.RowOff = 2;
                //imgBackLogo.SetSize(100, 48);

                intRow = 8;
                foreach (var category in line.Categories)
                {
                    wsLine.Cells[intRow, 1].Value = category.Name;
                    wsLine.Cells[intRow, 1].Style.Font.Bold = true;
                    wsLine.Cells[intRow, 1, intRow, 3 + intPrices * 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsLine.Cells[intRow, 1, intRow, 3 + intPrices * 2].Style.Fill.PatternColor.SetColor(Color.White);
                    wsLine.Cells[intRow, 1, intRow, 3 + intPrices * 2].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                    wsLine.Cells[intRow, 1, intRow, 3 + intPrices * 2].Style.Font.Color.SetColor(Color.White);
                    intRow++;

                    foreach (var subcategory in category.Subcategories)
                    {
                        wsLine.Cells[intRow, 1].Value = subcategory.Name;
                        wsLine.Cells[intRow, 1].Style.Font.Bold = true;
                        intRow++;
                        wsLine.Cells[intRow, 1].Value = "Código";
                        wsLine.Cells[intRow, 2].Value = "Nombre";
                        wsLine.Cells[intRow, 3].Value = "Garantía";

                        intCol = 4;
                        foreach (string subsidiary in subsidiaries)
                        {
                            if (boLocal | boSeeStock)
                            {
                                wsLine.Cells[intRow - 1, intCol].Value = subsidiary;
                                wsLine.Cells[intRow - 1, intCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                wsLine.Cells[intRow, intCol].Value = "Precio";
                                wsLine.Cells[intRow, intCol++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                wsLine.Cells[intRow, intCol].Value = "Stock";
                                wsLine.Cells[intRow, intCol++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                wsLine.Cells[intRow - 1, intCol - 2, intRow - 1, intCol - 1].Merge = true;
                                //wsLine.Cells[intRow - 1, intCol - 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                            else
                            {
                                wsLine.Cells[intRow, intCol].Value = subsidiary;
                                wsLine.Cells[intRow, intCol, intRow, intCol + 1].Merge = true;
                                //wsLine.Cells[intRow, intCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                intCol += 2;
                            }
                        }
                        wsLine.Cells[intRow, 4, intRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        wsLine.Cells[intRow - 1, 1, intRow, 3 + intPrices * 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        wsLine.Cells[intRow - 1, 1, intRow, 3 + intPrices * 2].Style.Fill.PatternColor.SetColor(Color.White);
                        wsLine.Cells[intRow - 1, 1, intRow, 3 + intPrices * 2].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
                        intRow++;

                        foreach (Models.ProductShort item in subcategory.Products)
                        {
                            wsLine.Cells[intRow, 1].Value = item.ItemCode;
                            wsLine.Cells[intRow, 2].Value = item.Name;
                            if (!string.IsNullOrEmpty(item.Description) & WithDetail)
                            {
                                wsLine.Cells[intRow, 2].Value += "\r\n" + item.Description.Replace("<br />", "\r\n");
                            }
                            if (!string.IsNullOrEmpty(item.Commentaries) & WithDetail)
                            {
                                wsLine.Cells[intRow, 2].Value += "\r\n" + item.Commentaries.Replace("<br />", "\r\n");
                            }
                            wsLine.Cells[intRow, 3].Value = item.Warranty;
                            intCol = 4;
                            foreach (string subsidiary in subsidiaries)
                            {
                                var tempStock = stockItems.Where(x => x.ItemCode.ToLower() == item.ItemCode.ToLower() & x.Subsidiary.ToLower() == subsidiary.ToLower() & (!x.Warehouse.ToLower().Contains("transito") | WithTransit));
                                var tempPrice = (from p in item.Prices where p.Subsidiary.ToLower() == subsidiary.ToLower() select p).FirstOrDefault();
                                if (tempPrice?.Regular > 0 | tempStock?.Count() > 0)
                                {
                                    string strValue = "";
                                    if (tempPrice?.Regular > 0)
                                    {
                                        if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
                                        {
                                            strValue = line.Manager.Id == IdManager & (tempPrice.Offer.HasValue && tempPrice.Offer.Value > 0) ? $"{tempPrice.Offer.Value:N2} $Us" : $"{tempPrice.Regular:N2} $Us";
                                        }
                                        else
                                        {
                                            strValue = tempPrice.Offer.HasValue && tempPrice.Offer.Value > 0 ? $"{tempPrice.Offer.Value:N2} $Us" : $"{tempPrice.Regular:N2} $Us";
                                        }
                                    }
                                    wsLine.Cells[intRow, intCol].Value = strValue;
                                    if ((tempPrice != null && tempPrice.Offer != null && tempPrice.Offer.HasValue && tempPrice.Offer.Value > 0) | tempPrice?.Volume?.Count > 0)
                                    {
                                        string comment = "";
                                        if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
                                        {
                                            if (line.Manager.Id == IdManager)
                                            {
                                                comment += tempPrice.Offer.HasValue && tempPrice.Offer.Value > 0 ? $"Precio Regular: {tempPrice.Regular:N2} $Us \r\n  Oferta: {tempPrice.Offer.Value:N2} $Us" : "";
                                                if (tempPrice.Volume?.Count > 0)
                                                    comment += string.Join("\r\n", tempPrice.Volume.Select(x => $"{x.Quantity} items    \t\t {x.Price:N2} $Us"));
                                            }
                                        }
                                        else
                                        {
                                            comment = tempPrice.Offer.HasValue && tempPrice.Offer.Value > 0 ? $"Precio Regular: {tempPrice.Regular:N2} $Us \r\n  Oferta: {tempPrice.Offer.Value:N2} $Us" : "";
                                            if (tempPrice.Volume?.Count > 0 & CardCode == HomeCardCode)
                                                comment += string.Join("\r\n", tempPrice.Volume.Select(x => $"{x.Quantity} items    \t\t {x.Price:N2} $Us"));
                                        }
                                        if (!string.IsNullOrEmpty(comment)) wsLine.Cells[intRow, intCol].AddComment(comment, "Soporte Portal");
                                    }
                                    wsLine.Cells[intRow, intCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    intCol++;
                                    if (tempStock?.Count() > 0)
                                    {
                                        if (boLocal | boSeeStock)
                                        {
                                            wsLine.Cells[intRow, intCol].Value = tempStock.Sum(x => x.Available2);
                                            wsLine.Cells[intRow, intCol].AddComment(string.Join("\r\n", tempStock.Select(x => $"{ToTitle(x.Warehouse)}  {x.Available2} {(WithTransit & x.Requested > 0 ? $" + {x.Requested}" : "")}")), "Soporte Portal");
                                        }
                                        else
                                        {
                                            wsLine.Cells[intRow, intCol].Value = string.Join("\r\n", tempStock.Select(x => $"{ToTitle(x.Warehouse)}  {x.Percentage}"));
                                            wsLine.Cells[intRow, intCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }
                                    }
                                }
                                else
                                {
                                    if (boSeeStock | boLocal) intCol++;
                                }
                                intCol++;
                            }
                            if (intRow % 2 == 1)
                            {
                                wsLine.Cells[intRow, 1, intRow, intCol - 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 240, 240));
                            }
                            intRow++;
                        }
                    }
                    intRow++;
                }
                wsLine.Cells[intRow, 1].Value = "La lista de precios DMC S.A. está sujeta a cambios de precios, así como variación en su disponibilidad y términos de ofertas, garantías y otros aspectos. Por tal razón DMC S.A. no se hace responsable por errores u omisiones que se den en la presente, pues estarían exentos a la responsabilidad directa por los mismos.";
                wsLine.Cells[intRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                wsLine.Cells[intRow, 1].Style.WrapText = true;
                wsLine.Cells[intRow, 1].Style.Fill.BackgroundColor.SetColor(Color.Black);
                wsLine.Cells[intRow, 1].Style.Font.Color.SetColor(Color.White);
                wsLine.Cells[intRow, 1, intRow, 3 + intPrices * 2].Merge = true;
                wsLine.Row(intRow).Height = 38;

                wsLine.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            string strFileName = $"Lista de Precios DMC {DateTime.Now:yyyyMMdd-HHmm}.xlsx";
            byte[] objData = objExcel.GetAsByteArray();
            objExcel.Dispose();
            return File(objData, "application/xlsx", strFileName);
        }

        public IActionResult GetProviderOrders(string ItemCode, string Subsidiary, string Warehouse)
        {
            string message = "";
            try
            {
                BCA.ProviderOrder bcOrder = new();
                var orders = bcOrder.ListByItemRequested(ItemCode, Subsidiary, Warehouse);
                var details = bcOrder.ListByItemRequestedDetails(ItemCode, Subsidiary, Warehouse);
                if (orders?.Count() > 0)
                {
                    var items = from o in orders
                                orderby o.DocDate
                                join d in details on new { Subsidiary = o.Subsidiary.ToLower(), o.DocNumber } equals new { Subsidiary = d.Subsidiary.ToLower(), d.DocNumber }
                                select new { o.DocNumber, o.DocDate, o.EstimatedDate, o.ReferenceOrder, o.ProviderCode, o.ProviderName, d.Quantity, d.OpenQuantity, d.Price, d.DeliveredQuantity, d.Subtotal, d.OpenSubtotal };
                    return Json(new { message, items });
                }
                else
                {
                    return Json(new { message, items = Array.Empty<string>() });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public async Task<IActionResult> GetClientsByListAsync(string ListName, long LineId)
        {
            string message = "";
            try
            {
                IEnumerable<BEA.Client> clients = Enumerable.Empty<BEA.Client>();
                BCP.PriceGroupClient bc = new();
                var clientCodes = await bc.ListClientsAsync(ListName, LineId);

                if (clientCodes.Any())
                {
                    BCA.Client bcClient = new();
                    List<Field> filters = new() { Field.New("LOWER(CardCode)", string.Join(",", clientCodes.Select(x => $"'{x.ToLower()}'")), Operators.In) };
                    clients = await bcClient.ListShortAsync(filters, "1");
                }

                var items = clients.Select(x => new { Code = x.CardCode, Name = x.CardName });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Metodos Privados

        private IEnumerable<LineShort> GetItems2(long? IdLine, string Category, string Subcategory, string Filter, string Ids = "")
        {
            List<string> filters = !string.IsNullOrWhiteSpace(Filter) ? Filter.Split(',').ToList() : new List<string>();
            List<LineShort> lstLines = new();

            bool boLocal = CardCode == HomeCardCode, boPM = (long)BEE.Types.Profile.ProductManagement == ProfileCode;
            long IdManager = 0;
            if (boPM)
            {
                BCF.Member bcMember = new();
                var manager = bcMember.SearchByMail(EMail);
                IdManager = manager?.Id ?? 0;
            }

            BCP.Line bcLine = new();
            BCP.Product bcProduct = new();
            BCP.Price bcPrice = new();
            BCP.PriceOffer bcOffer = new();
            IEnumerable<BEP.Line> lstTemp;
            IEnumerable<BEP.Product> lstProducts;
            List<Field> lstFilter = new();
            IEnumerable<string> lstCategories, lstSubCategories, lstProductIds;
            bool boSC = true, boIQ = true, boLA = true;

            BCP.PriceGroupLine bcGroupLine = new();
            BCP.PriceGroup bcGroup = new();

            IEnumerable<BEP.PriceGroup> groupLists = bcGroup.List("1");
            IEnumerable<BEP.PriceGroupLine> groupLineList = bcGroupLine.List("1", BEP.relPriceGroupLine.Group);

            if (IsClient)
            {
                BCB.Classifier bcClassifier = new();
                var classifiers = bcClassifier.List((long)Classifiers.PriceListMinimunAmounts, "1");
                decimal minSC, minIQ, minLA, totalSC, totalIQ, totalLA;
                int months = int.Parse(classifiers.First(x => x.Name == "PL-MIN-MONTHS").Value);
                minSC = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-SC").Value);
                minIQ = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-IQ").Value);
                minLA = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-LA").Value);

                BCA.Client bcClientSAP = new();
                var amounts = bcClientSAP.ListResumeAmounts(CardCode, DateTime.Today.AddMonths(-months));
                totalSC = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "santa cruz")?.Balance ?? 0;
                totalIQ = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "iquique")?.Balance ?? 0;
                totalLA = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "miami")?.Balance ?? 0;

                boSC = totalSC >= minSC || GetPermission("SeePricesSA") > 0;
                boIQ = totalIQ >= minIQ || GetPermission("SeePricesIQ") > 0;
                boLA = totalLA >= minLA || GetPermission("SeePricesLA") > 0;

                groupLineList = bcGroupLine.ListByClient(CardCode, "1", BEP.relPriceGroupLine.Group);
                groupLists = bcGroup.ListByClient(CardCode, "1");
            }

            if (string.IsNullOrEmpty(Ids))
            {
                lstFilter.AddRange(new[] { Field.New("Enabled", true), Field.New("ISNULL(Category, '')", "", Operators.Different), Field.LogicalAnd() });
                if (IdLine.HasValue && IdLine <= 0)
                {
                    lstProducts = IdLine switch
                    {
                        0 => bcProduct.ListWithOffer(lstFilter, "ItemCode"),
                        -11 => bcProduct.ListWithOfferSA(lstFilter, "ItemCode"),
                        -12 => bcProduct.ListWithOfferLA(lstFilter, "ItemCode"),
                        -13 => bcProduct.ListWithOfferIQ(lstFilter, "ItemCode"),
                        -2 => bcProduct.ListWithPricesAndNew(30, lstFilter, "1"),
                        _ => bcProduct.ListWithOffer(lstFilter, "ItemCode"),
                    };
                }
                else
                {
                    if (IdLine.HasValue)
                    {
                        lstFilter.AddRange(new[] { Field.New("ISNULL(Line, '')", $"SELECT SAPLine FROM Product.LineDetail WHERE IdLine = {IdLine.Value}", Operators.In), Field.LogicalAnd() });
                    }
                    if (!string.IsNullOrWhiteSpace(Category))
                    {
                        lstFilter.AddRange(new[] { Field.New("Category", Category), Field.LogicalAnd() });
                    }
                    if (!string.IsNullOrWhiteSpace(Subcategory))
                    {
                        lstFilter.AddRange(new[] { Field.New("SubCategory", Subcategory), Field.LogicalAnd() });
                    }
                    if (filters?.Count > 0)
                    {
                        foreach (var item in filters)
                        {
                            lstFilter.AddRange(new[] {
                                Field.New("Name", item, Operators.Likes), Field.New("Description", item, Operators.Likes), Field.New("ItemCode", item, Operators.Likes), Field.New("Category", item, Operators.Likes),
                                Field.New("SubCategory", item, Operators.Likes)
                            });
                            for (int i = 1; i < 5; i++)
                            {
                                lstFilter.Add(Field.LogicalOr());
                            }
                            lstFilter.Add(Field.LogicalAnd());
                        }
                    }
                    lstProducts = bcProduct.ListWithPrices(lstFilter, "1");
                }
            }
            else
            {
                lstFilter.Add(Field.New("Id", Ids, Operators.In));
                lstProducts = bcProduct.List(lstFilter, "ItemCode");
            }

            BCS.LineNotAllowed bcClient = new();
            List<Field> lstFilter2 = new() { Field.New("CardCode", CardCode) };
            var lstClientExceptions = bcClient.List(lstFilter2, "1");

            if (lstProducts?.Count() > 0)
            {
                lstProductIds = (from p in lstProducts where !string.IsNullOrWhiteSpace(p.Line) & (p.ShowTeamOnly == false | boLocal) select p.Id.ToString()).Distinct().ToList();
                BCP.PriceHistory bcHistory = new();
                List<long> lstProductIds2 = (from p in lstProducts where !string.IsNullOrWhiteSpace(p.Line) select p.Id).Distinct().ToList();
                IEnumerable<BEP.PriceHistory> lstHistory = lstProductIds2?.Count > 0 ? bcHistory.ListFirst(lstProductIds2) : Enumerable.Empty<BEP.PriceHistory>();

                BCB.Classifier bcClassifier = new();
                List<BEB.Classifier> subsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");

                BCS.User bcUser = new();
                BES.User user = bcUser.Search(UserCode);

                lstTemp = lstProductIds?.Count() > 0 ? bcLine.ListForPriceList(string.Join(",", lstProductIds), "2", BEP.relLine.LineDetails, BEP.relLine.Manager) : new List<BEP.Line>();
                lstLines = (from l in lstTemp
                            where boLocal || user.AllowLinesBlocked || l.WhenFilteredShowInfo || ((l.FilterType == "AllBut" & !(from a in lstClientExceptions select a.IdLine).Contains(l.Id)) || (l.FilterType == "NoneBut" & (from a in lstClientExceptions select a.IdLine).Contains(l.Id)))
                            select new LineShort
                            {
                                Id = l.Id,
                                Name = l.Name,
                                Description = l.Description.ToWebSafe(),
                                Footer = l.Footer.ToWebSafe(),
                                Header = l.Header.ToWebSafe(),
                                ImageURL = l.ImageURL,
                                SAPLines = l.ListLineDetails.Select(x => x.SAPLine).ToList(),
                                Manager = l.Manager != null ? new Manager(l.Manager) : null,
                                Percentage = groupLineList.FirstOrDefault(x => x.IdLine == l.Id)?.Percentage ?? 0
                            }).ToList();

                BCP.VolumePricing bcVolumen = new();
                lstFilter = new List<Field> { new Field("IdProduct", string.Join(",", lstProductIds), Operators.In) };
                IEnumerable<BEP.VolumePricing> lstVolumen = lstProductIds?.Count() > 0 ? bcVolumen.List(lstFilter, "1") : new List<BEP.VolumePricing>();

                bool seeStockPermission = GetPermission("Pricelist-SeeStock") > 0;
                foreach (var item in lstLines)
                {
                    bool canSeeOffers = !boPM | (boPM & item.Manager?.Id == IdManager);
                    var line = lstTemp.FirstOrDefault(x => x.Id == item.Id);
                    bool boShowPrice = boLocal || user.AllowLinesBlocked || (line.FilterType == "NoneBut" & lstClientExceptions.Any(x => x.IdLine == line.Id)) || (line.FilterType == "AllBut" & !lstClientExceptions.Any(x => x.IdLine == line.Id));

                    List<BEP.Price> lstPrices = bcPrice.List2(item.Id, "1", BEP.relPrice.Sudsidiary) ?? new List<BEP.Price>();
                    IEnumerable<BEP.PriceOffer> lstOffers = bcOffer.ListByLine(item.Id, "Price") ?? new List<BEP.PriceOffer>();

                    IEnumerable<BEP.Product> lstProds = lstProducts.Where(i => item.SAPLines.Contains(i.Line) & (i.ShowTeamOnly == false | boLocal));
                    lstCategories = (from p in lstProds orderby p.Category select p.Category).Distinct().ToList();

                    long intCat = 1, intSub = 1;
                    foreach (var cat in lstCategories)
                    {
                        Models.Category beCat = new() { Name = cat, Id = intCat++ };
                        lstSubCategories = (from p in lstProds where p.Category == cat orderby p.SubCategory select p.SubCategory).Distinct().ToList();
                        foreach (var subcat in lstSubCategories)
                        {
                            Subcategory beSubCat;
                            beSubCat = new()
                            {
                                Name = subcat,
                                Id = intSub++,
                                Products = (from p in lstProds
                                            join h in lstHistory on p.Id equals h.IdProduct into lj
                                            from l in lj.DefaultIfEmpty()
                                            where p.Category == cat & p.SubCategory == subcat
                                            orderby p.Name
                                            select new ProductShort
                                            {
                                                Id = p.Id,
                                                Name = p.Name,
                                                Description = p.Description.ToWebSafe(),
                                                Commentaries = p.Commentaries.ToWebSafe(),
                                                ItemCode = p.ItemCode,
                                                Consumables = p.Consumables.ToWebSafe(),
                                                Link = p.Link,
                                                ExtraComments = !string.IsNullOrWhiteSpace(p.ExtraComments) ? SetHTMLSafe(p.ExtraComments.ToWebSafe()) : "",
                                                ShowTeamOnly = p.ShowTeamOnly,
                                                ShowAlways = p.ShowAlways ? "Y" : "N",
                                                Warranty = p.Warranty,
                                                ImageURL = p.ImageURL,
                                                IsDigital = p.IsDigital,
                                                IsNew = l != null && l.LogDate.AddDays(30) >= DateTime.Now,
                                                Prices = (from s in subsidiaries
                                                          join r in lstPrices on new { IdSudsidiary = s.Id, IdProduct = p.Id } equals new { r.IdSudsidiary, r.IdProduct } into ljPrices
                                                          from lp in ljPrices.DefaultIfEmpty()
                                                          where boShowPrice & ((s.Id == 1 & boSC) || (s.Id == 3 & boIQ) || (s.Id == 2 & boLA))
                                                          select new Price
                                                          {
                                                              Subsidiary = ToTitle(s.Name),
                                                              Regular = (groupLineList.Any(z => z.IdLine == item.Id) || groupLists.Any()) & p?.Cost > 0 & item.Percentage >= 0 & s.Id == 1 ? p.Cost.Value * (100 + item.Percentage) / 100 / 0.84m : (lp?.Regular ?? 0),
                                                              Offer = canSeeOffers ? lstOffers?.Where(o => o.IdProduct == p.Id && o.IdSubsidiary == s.Id)?.FirstOrDefault()?.Price ?? 0 : 0,
                                                              OfferSince = canSeeOffers ? lstOffers?.FirstOrDefault(o => o.IdProduct == p.Id && o.IdSubsidiary == s.Id)?.Since ?? null : null,
                                                              OfferUntil = canSeeOffers ? lstOffers?.FirstOrDefault(o => o.IdProduct == p.Id && o.IdSubsidiary == s.Id)?.Until ?? null : null,
                                                              Suggested = lp?.ClientSuggested ?? 0,
                                                              OfferDescription = canSeeOffers ? lstOffers?.Where(o => o.IdProduct == p.Id && o.IdSubsidiary == s.Id)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Description ?? "" : "",
                                                              Observations = lp?.Observations.ToWebSafe(),
                                                              Commentaries = lp?.Commentaries.ToWebSafe(),
                                                              Volume = (from v in lstVolumen
                                                                        where (v.IdProduct == p.Id & v.IdSubsidiary == s.Id & v.Quantity > 0 & v.Price > 0 & boLocal) & (!boPM | (boPM & item.Manager?.Id == IdManager))
                                                                        select new VolumePrice { Price = v.Price, Quantity = v.Quantity, Observations = v.Observations }).ToList(),
                                                              OtherPrices = IsClient ? new List<ListName>() : (from g in groupLists
                                                                                                               where g.Percentage >= 0 & (p.Cost.HasValue && p.Cost.Value > 0) & s.Id == 1
                                                                                                               join gl in groupLineList on new { IdGroup = g.Id, IdLine = item.Id } equals new { gl.IdGroup, gl.IdLine } into ljGroupList
                                                                                                               from lj2 in ljGroupList.DefaultIfEmpty()
                                                                                                               select new ListName(g.Name, p.Cost.Value * (100 + (lj2?.Percentage ?? g.Percentage)) / 100 / 0.84m)).ToList()
                                                          }).ToList()
                                            }).ToList()
                            };

                            if (beSubCat.Products?.Count > 0)
                            {
                                beCat.Subcategories.Add(beSubCat);
                            }
                        }
                        if (beCat.Subcategories?.Count(x => x.Products?.Count > 0) > 0)
                        {
                            item.Categories.Add(beCat);
                            foreach (var s in beCat.Subcategories)
                            {
                                foreach (var p in s.Products)
                                {
                                    foreach (var r in p.Prices)
                                    {
                                        foreach (var o in r.OtherPrices)
                                        {
                                            if (!item.UsedGroups.Contains(o.Name)) item.UsedGroups.Add(o.Name);
                                        }
                                    }
                                }
                            }
                            //item.UsedGroups.Sort();
                        }
                    }
                }
                lstLines = lstLines.Where(x => x.Categories?.Count > 0).ToList();
            }
            return lstLines;
        }

        private static string ExcelSafeName(string worksheetName)
        {
            const string invalidCharsRegex = @"[/\\*'?[\]:]+";
            const int maxLength = 31;

            string safeName = Regex.Replace(worksheetName, invalidCharsRegex, "").Replace("  ", " ").Trim();
            if (string.IsNullOrWhiteSpace(safeName))
            {
                safeName = "Default";   // cannot be empty
            }
            else if (safeName.Length > maxLength)
            {
                safeName = safeName.Substring(0, maxLength);
            }
            return safeName;
        }

        #endregion
    }
}