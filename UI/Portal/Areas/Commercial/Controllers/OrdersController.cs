using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
using BCL = BComponents.Sales;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEL = BEntities.Sales;
using BES = BEntities.Security;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class OrdersController : BaseController
    {
        #region Constructores

        public OrdersController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                string seeMargin = GetPermission("VerMargen") > 0 ? "Y" : "N";
                ViewData["SeeMargin"] = seeMargin;
                if (CardCode == HomeCardCode)
                {
                    BCS.UserData bcData = new();
                    BES.UserData data = bcData.SearchByUser(UserCode);
                    ViewBag.Sellercode = data?.SellerCode ?? "";
                    ViewBag.SeeAllClients = GetPermission("SeeAllClients") > 0 ? "Y" : "N";
                    return View();
                }
                else
                {
                    ViewData["Title"] = $"Ordenes/Notas : {CardName}";
                    ViewBag.CardCode = CardCode;
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(string SearchType, string ClientCode, int? DocNumber, DateTime? InitialDate, DateTime? FinalDate, string SellerCode, string ProductManager, string State, string Complete, string ClientOrder, string ItemCode, string Category, string Subcategory, string Line, string Subsidiary, string Warehouse)
        {
            //Trace.TraceInformation(Newtonsoft.Json.JsonConvert.SerializeObject(new { SearchType, ClientCode, DocNumber, InitialDate, FinalDate, SellerCode, ProductManager, State, Complete, ClientOrder, ItemCode, Category, Subcategory, Line, Subsidiary, Warehouse }));
            string message = "";
            List<BEA.Document> items;
            try
            {
                items = SearchType switch
                {
                    "SN" => GetNotes(ClientCode, DocNumber, InitialDate, FinalDate, SellerCode, ProductManager, ItemCode, Category, Subcategory, Line, Subsidiary, Warehouse),
                    "DN" => GetDeliveryNotes(ClientCode, DocNumber, InitialDate, FinalDate, SellerCode, ProductManager, ItemCode, Category, Subcategory, Line, Subsidiary, Warehouse),
                    _ => GetOrders(ClientCode, DocNumber, InitialDate, FinalDate, SellerCode, ProductManager, State, Complete, ClientOrder, ItemCode, Category, Subcategory, Line, Subsidiary, Warehouse),
                };
            }
            catch (Exception ex)
            {
                items = new List<BEA.Document>();
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        public IActionResult Detail(string Subsidiary, int Id, string State, string Type)
        {
            string message = "";
            try
            {
                List<BEA.DocumentItem> items = new();
                decimal? decNull = null;
                if (Type == "Orden de Venta")
                {
                    BCA.Order bcOrder = new();
                    IEnumerable<BEA.OrderItem> lstOrderItems = bcOrder.ListItems(Subsidiary, Id, "LineNum", BEA.relOrderItem.Product) ?? new List<BEA.OrderItem>();
                    items = lstOrderItems.Select(x => new BEA.DocumentItem
                    {
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        Warehouse = x.Warehouse,
                        Line = x.Line,
                        Quantity = x.Quantity,
                        OpenQuantity = x.OpenQuantity,
                        Stock = x.Stock,
                        Price = x.Price,
                        ItemTotal = x.ItemTotal,
                        Margin0100 = (decimal)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : 0),
                        Complete = State != "Abierto" || x.OpenQuantity <= x.Stock
                    }).ToList();
                }
                if (Type == "Nota de Venta")
                {
                    BCA.Note bcNote = new();
                    List<Field> filters = new() { new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field("Id", Id), new Field(LogicalOperators.And) };
                    var noteItems = bcNote.ListItems(filters, "LineNum", BEA.relNoteItem.Product);
                    items = noteItems.Select(x => new BEA.DocumentItem { ItemCode = x.ItemCode, ItemName = x.ItemName, Warehouse = x.Warehouse, Line = x.Line, Quantity = x.Quantity, Price = x.Price, ItemTotal = x.ItemTotal, Margin0100 = (decimal)(x.CalculedTotal != 0 ? x.Margin / x.CalculedTotal : decNull) }).ToList();
                }
                if (Type == "Nota de Entrega")
                {
                    BCA.DeliveryNote bcNote = new();
                    var filters = new[] { $"'{Subsidiary.ToLower()}-{Id}'" };
                    var noteItems = bcNote.ListItems2(filters, "LineNum");
                    items = noteItems.Select(x => new BEA.DocumentItem { ItemCode = x.ItemCode, ItemName = x.ItemName, Warehouse = x.Warehouse, Line = x.Line, Quantity = x.Quantity, Price = x.Price, ItemTotal = x.Total, Margin0100 = (decimal)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : decNull) }).ToList();
                }
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult DownloadFile(string FilePath, string FileName, string FileExt)
        {
            try
            {
                string fullName = $@"{FilePath}\\{FileName}.{FileExt}";
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string contentType = FileExt switch
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
                return File(file, contentType, $"{FileName}.{FileExt}");
            }
            catch (FileNotFoundException)
            {
                string fullName = Path.Combine(rootDirectory, "wwwroot", "images", "DMC-404.jpg");
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string contentType = "image/jpeg";
                return File(file, contentType, "DMC-404.jpg");
            }
        }

        public IActionResult GetStockDetail(string Subsidiary, string Warehouse, string ItemCode)
        {
            BCA.ProductStock bcInventory = new();
            List<Field> lstFilters = new() { new Field("ItemCode", ItemCode), new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field("LOWER(Warehouse)", Warehouse.ToLower()), new Field(LogicalOperators.And), new Field(LogicalOperators.And) };
            IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(lstFilters, "1");
            lstInventory.ForEach(x => x.Warehouse = ToTitle(x.Warehouse));
            return PartialView(lstInventory);
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
                var lstReFiles = (from f in files
                                  group f by new { f.Subsidiary, f.DocEntry } into g
                                  select new { g.Key.Subsidiary, g.Key.DocEntry, Count = g.Count(), Files = (from d in g select $"{d.FileName}.{d.FileExt}").ToList() }).ToList();
                var items = (from o in reserved
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
                             }).ToList();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ExportExcel(string SearchType, string ClientCode, int? DocNumber, DateTime? InitialDate, DateTime? FinalDate, string SellerCode, string ProductManager, string State, string Complete, string ClientOrder, string ItemCode, string Category, string Subcategory, string Line, string Subsidiary, string Warehouse, bool Detailed)
        {
            List<BEA.Document> items = SearchType switch
            {
                "SN" => GetNotes(ClientCode, DocNumber, InitialDate, FinalDate, SellerCode, ProductManager, ItemCode, Category, Subcategory, Line, Subsidiary, Warehouse),
                "DN" => GetDeliveryNotes(ClientCode, DocNumber, InitialDate, FinalDate, SellerCode, ProductManager, ItemCode, Category, Subcategory, Line, Subsidiary, Warehouse),
                _ => GetOrders(ClientCode, DocNumber, InitialDate, FinalDate, SellerCode, ProductManager, State, Complete, ClientOrder, ItemCode, Category, Subcategory, Line, Subsidiary, Warehouse),
            };
            bool seeMargin = GetPermission("VerMargen") > 0, localUser = HomeCardCode == CardCode;
            using ExcelPackage objExcel = new();
            ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
            FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

            List<BEA.DocumentItem> lstDetails = new();
            if (Detailed)
            {
                var codes = items.Select(x => $"'{x.Subsidiary.ToLower()}-{x.Id}'");
                if (SearchType == "SO")
                {
                    BCA.Order bcOrder = new();
                    var lstOrderItems = bcOrder.ListItems(codes, BEA.relOrderItem.Product) ?? new List<BEA.OrderItem>(); lstDetails = lstOrderItems.Select(x => new BEA.DocumentItem { Id = x.OrderId, ItemCode = x.ItemCode, ItemName = x.ItemName, Subsidiary = x.Subsidiary, Warehouse = x.Warehouse, Line = x.Line, Quantity = x.Quantity, OpenQuantity = x.OpenQuantity, Stock = x.Stock, Price = x.Price, ItemTotal = x.ItemTotal, Margin0100 = (decimal)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : -100), Complete = State != "Abierto" || x.OpenQuantity <= x.Stock }).ToList();
                }
                if (SearchType == "SN")
                {
                    BCA.Note bcNote = new();
                    var noteItems = bcNote.ListItems(codes, BEA.relNoteItem.Product) ?? new List<BEA.NoteItem>();
                    lstDetails = noteItems.Select(x => new BEA.DocumentItem { Id = x.NoteId, ItemCode = x.ItemCode, ItemName = x.ItemName, Subsidiary = x.Subsidiary, Warehouse = x.Warehouse, Line = x.Line, Quantity = x.Quantity, Price = x.Price, ItemTotal = x.ItemTotal, Margin0100 = (decimal)(x.CalculedTotal != 0 ? x.Margin / x.CalculedTotal : -100) }).ToList();
                }
                if (SearchType == "DN")
                {
                    BCA.DeliveryNote bcNote = new();
                    var noteItems = bcNote.ListItems2(codes, "LineNum") ?? new List<BEA.DeliveryNoteItem>();
                    lstDetails = noteItems.Select(x => new BEA.DocumentItem { Id = x.Id, ItemCode = x.ItemCode, ItemName = x.ItemName, Subsidiary = x.Subsidiary, Warehouse = x.Warehouse, Line = x.Line, Quantity = x.Quantity, Price = x.Price, ItemTotal = x.Total, Margin0100 = (decimal)(x.TaxlessTotal != 0 ? x.Margin / x.TaxlessTotal : -100) }).ToList();
                }
            }
            else
            {
                lstDetails.Add(new BEA.DocumentItem());
            }

            wsMain.Name = "Resumen Pedidos";
            wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
            wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
            wsMain.Cells.Style.Font.Size = 9;

            wsMain.Cells[1, 1].Value = "CONSULTA DE PEDIDOS";
            wsMain.Cells[1, 1].Style.Font.Size = 14;
            wsMain.Cells[1, 1].Style.Font.Bold = true;
            wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[1, 1, 1, 14].Merge = true;
            wsMain.Cells[4, 1, 4, 14].Merge = true;
            wsMain.Cells[4, 1].Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";

            var imgLogo = wsMain.Drawings.AddPicture("logo", logo);
            imgLogo.SetPosition(5, 5);

            int col = 1;
            wsMain.Cells[6, col++].Value = "Sucursal";
            wsMain.Cells[6, col++].Value = "Almacén";
            wsMain.Cells[6, col++].Value = "Cliente";
            wsMain.Cells[6, col++].Value = "Vendedor";
            wsMain.Cells[6, col++].Value = "Orden Compra Cliente";
            wsMain.Cells[6, col++].Value = "Tipo";
            wsMain.Cells[6, col].Value = "Número";
            wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsMain.Cells[6, col].Value = "Total($us)";
            wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            if (SearchType == "SO")
            {
                wsMain.Cells[6, col].Value = "Abierto($us)";
                wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }
            if (seeMargin)
            {
                wsMain.Cells[6, col].Value = "Margen";
                wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }
            wsMain.Cells[6, col].Value = "Fecha";
            wsMain.Cells[6, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            if (SearchType == "SO")
            {
                col++;
                wsMain.Cells[6, col].Value = "Estado";
            }
            if (localUser & SearchType == "SO")
            {
                col++;
                wsMain.Cells[6, col++].Value = "Autorizado";
                wsMain.Cells[6, col].Value = "Completo";
            }
            if (Detailed)
            {
                col++;
                wsMain.Cells[6, col++].Value = "Cod.";
                wsMain.Cells[6, col++].Value = "Nombre";
                wsMain.Cells[6, col++].Value = "Línea";
                wsMain.Cells[6, col].Value = "Cantidad";
                wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                if (SearchType == "SO")
                {
                    wsMain.Cells[6, col].Value = "Cant. Abierta";
                    wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
                if (localUser & SearchType == "SO")
                {
                    wsMain.Cells[6, col].Value = "Stock";
                    wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
                wsMain.Cells[6, col].Value = "Precio";
                wsMain.Cells[6, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                wsMain.Cells[6, col].Value = "Subtotal";
                wsMain.Cells[6, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                if (seeMargin)
                {
                    col++;
                    wsMain.Cells[6, col].Value = "Margen Item";
                    wsMain.Cells[6, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
            }

            wsMain.Cells[6, 1, 6, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells[6, 1, 6, col].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
            wsMain.Cells[6, 1, 6, col].Style.Font.Color.SetColor(Color.White);
            wsMain.Cells[6, 1, 6, col].Style.Font.Bold = true;

            int intFila;
            if (items?.Count > 0)
            {
                List<string> lstSucursales = (from i in items orderby i.Subsidiary select i.Subsidiary).Distinct().ToList();

                intFila = 7;
                foreach (var sucursal in lstSucursales)
                {
                    List<string> lstAlmacenes = (from i in items where i.Subsidiary == sucursal orderby i.Warehouse select i.Warehouse).Distinct().ToList();
                    foreach (var almacen in lstAlmacenes)
                    {
                        List<string> lstClientes = (from i in items where i.Subsidiary == sucursal & i.Warehouse == almacen orderby i.ClientName select i.ClientName).Distinct().ToList();
                        foreach (var cliente in lstClientes)
                        {
                            List<BEA.Document> lstSubItems = (from i in items where i.Subsidiary == sucursal & i.Warehouse == almacen & i.ClientName == cliente orderby i.DocNumber select i).ToList();
                            foreach (var item in lstSubItems)
                            {
                                List<BEA.DocumentItem> tempDetails = new() { new BEA.DocumentItem { ItemCode = "XXX1", ItemName = "Name 1", Line = "Line1", Quantity = 1, OpenQuantity = 0, Stock = 10, Price = (decimal)120.5, ItemTotal = (decimal)120.5 } };
                                if (Detailed)
                                {
                                    tempDetails = lstDetails.FindAll(x => x.Subsidiary.ToLower() == item.Subsidiary.ToLower() & x.Id == item.Id);
                                }

                                foreach (var detail in tempDetails)
                                {
                                    col = 1;
                                    wsMain.Cells[intFila, col++].Value = item.Subsidiary;
                                    wsMain.Cells[intFila, col++].Value = item.Warehouse;
                                    wsMain.Cells[intFila, col++].Value = item.ClientName;
                                    wsMain.Cells[intFila, col++].Value = item.SellerName;
                                    wsMain.Cells[intFila, col++].Value = item.ClientOrder;
                                    wsMain.Cells[intFila, col++].Value = item.DocType;
                                    wsMain.Cells[intFila, col++].Value = item.DocNumber;
                                    wsMain.Cells[intFila, col].Value = item.Total;
                                    wsMain.Cells[intFila, col++].Style.Numberformat.Format = "#,##0.00";
                                    if (SearchType == "SO")
                                    {
                                        wsMain.Cells[intFila, col].Value = item.OpenAmount;
                                        wsMain.Cells[intFila, col++].Style.Numberformat.Format = "#,##0.00";
                                    }
                                    if (seeMargin)
                                    {
                                        wsMain.Cells[intFila, col].Value = item.Margin0100 / 100;
                                        wsMain.Cells[intFila, col++].Style.Numberformat.Format = "0.00 %";
                                    }
                                    wsMain.Cells[intFila, col].Value = item.DocDate;
                                    wsMain.Cells[intFila, col].Style.Numberformat.Format = "dd/MM/yyyy";
                                    if (SearchType == "SO")
                                    {
                                        col++;
                                        wsMain.Cells[intFila, col].Value = item.State;
                                    }
                                    if (localUser & SearchType == "SO")
                                    {
                                        col++;
                                        wsMain.Cells[intFila, col++].Value = item.Authorized ? "Si" : "No";
                                        wsMain.Cells[intFila, col].Value = item.Complete ? "Si" : "No";
                                    }
                                    if (Detailed)
                                    {
                                        col++;
                                        wsMain.Cells[intFila, col++].Value = detail.ItemCode;
                                        wsMain.Cells[intFila, col++].Value = detail.ItemName;
                                        wsMain.Cells[intFila, col++].Value = detail.Line;
                                        wsMain.Cells[intFila, col].Value = detail.Quantity;
                                        wsMain.Cells[intFila, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        if (SearchType == "SO")
                                        {
                                            wsMain.Cells[intFila, col].Value = detail.OpenQuantity;
                                            wsMain.Cells[intFila, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }
                                        if (localUser & SearchType == "SO")
                                        {
                                            wsMain.Cells[intFila, col].Value = detail.Stock;
                                            wsMain.Cells[intFila, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }
                                        wsMain.Cells[intFila, col].Value = detail.Price;
                                        wsMain.Cells[intFila, col].Style.Numberformat.Format = "#,##0.00";
                                        wsMain.Cells[intFila, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        wsMain.Cells[intFila, col].Value = detail.ItemTotal;
                                        wsMain.Cells[intFila, col].Style.Numberformat.Format = "#,##0.00";
                                        wsMain.Cells[intFila, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        if (seeMargin)
                                        {
                                            col++;
                                            wsMain.Cells[intFila, col].Value = detail.Margin0100;
                                            wsMain.Cells[intFila, col].Style.Numberformat.Format = "0.00 %";
                                            wsMain.Cells[intFila, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                        }
                                    }

                                    if (CardCode == HomeCardCode)
                                    {
                                        wsMain.Cells[intFila, col + 1].Value = item.Header?.Replace("<br />", Environment.NewLine) + (item.Footer?.Trim().Length > 0 ? (Environment.NewLine + item.Footer.Replace("<br />", Environment.NewLine)) : "");
                                    }
                                    intFila += 1;
                                }
                            }
                        }
                    }
                }
                wsMain.Cells[intFila, 8].Value = (from i in items select i.Total).Sum();
                wsMain.Cells[intFila, 8].Style.Numberformat.Format = "#,##0.00";
                if (SearchType == "SO")
                {
                    wsMain.Cells[intFila, 9].Value = (from i in items select i.OpenAmount).Sum();
                    wsMain.Cells[intFila, 9].Style.Numberformat.Format = "#,##0.00";
                }
                if (seeMargin)
                {
                    decimal taxlessTotal = (from i in items select i.TaxlessTotal).Sum(), margin = (from i in items select i.Margin).Sum();
                    wsMain.Cells[intFila, SearchType == "SO" ? 10 : 9].Value = taxlessTotal != 0 ? margin / taxlessTotal : 0;
                    wsMain.Cells[intFila, SearchType == "SO" ? 10 : 9].Style.Numberformat.Format = "0.00 %";
                }
                wsMain.Cells[intFila, 1, intFila, col].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                wsMain.Cells[intFila, 1, intFila, col].Style.Font.Color.SetColor(Color.White);
                wsMain.Cells[intFila, 1, intFila, col].Style.Font.Bold = true;
            }

            wsMain.Cells.AutoFitColumns();
            wsMain.Cells.Style.WrapText = true;
            wsMain.Column(6).Width = 15;

            wsMain.Column(1).Width = 10;
            wsMain.Column(2).Width = 11;
            wsMain.Column(3).Width = 40;
            wsMain.Column(8).Width = 18;

            wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            wsMain.View.FreezePanes(7, 1);
            wsMain.PrinterSettings.LeftMargin = 0.2m;
            wsMain.PrinterSettings.RightMargin = 0.2m;
            wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:7"];
            wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:N"];
            wsMain.PrinterSettings.Orientation = eOrientation.Landscape;

            string strFileName = $"Resumen-Pedidos-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
            byte[] objData = objExcel.GetAsByteArray();
            objExcel.Dispose();
            return File(objData, "application/xlsx", strFileName);
        }

        #endregion

        #region Private Methods

        private List<BEA.Document> GetOrders(string ClientCode, int? DocNumber, DateTime? InitialDate, DateTime? FinalDate, string SellerCode, string ProductManager, string State, string Complete, string ClientOrder, string ItemCode, string Category, string Subcategory, string Line, string Subsidiary, string Warehouse)
        {
            List<BEA.Document> items = new();
            BCA.Order bcOrder = new();
            List<Field> filters = new(), itemFilters = new();
            if (!string.IsNullOrEmpty(ClientCode))
            {
                filters.Add(new Field("LOWER(ClientCode)", ClientCode.ToLower()));
            }
            if (ProfileCode == (long)BEE.Types.Profile.Sales || ProfileCode == (long)BEE.Types.Profile.SpecialSales)
            {
                if (!string.IsNullOrEmpty(SellerCode))
                {
                    filters.AddRange(new[] { new Field("LOWER(SellerCode)", SellerCode.ToLower()), new Field("LOWER(SellerCode)", "dmc"), new Field("SlpCode", -1), new Field(LogicalOperators.Or), new Field(LogicalOperators.Or) });
                }
            }
            if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
            {
                BCS.UserData bcData = new();
                BES.UserData data = bcData.SearchByUser(UserCode);
                if (!string.IsNullOrEmpty(data?.SellerCode))
                {
                    itemFilters.Add(new Field("LOWER(U_CODGRT)", data.SellerCode.ToLower()));
                }
            }
            if (DocNumber.HasValue && DocNumber.Value > 0)
            {
                filters.Add(new Field("DocNumber", DocNumber.Value));
            }
            else
            {
                if (!string.IsNullOrEmpty(SellerCode) && ProfileCode != (long)BEE.Types.Profile.Sales && ProfileCode != (long)BEE.Types.Profile.SpecialSales && ProfileCode != (long)BEE.Types.Profile.ProductManagement)
                {
                    filters.Add(new Field("LOWER(SellerCode)", SellerCode.ToLower()));
                }
                if (!string.IsNullOrEmpty(Subsidiary))
                {
                    filters.Add(new Field("LOWER(Subsidiary)", Subsidiary.ToLower()));
                }
                if (!string.IsNullOrEmpty(Warehouse))
                {
                    filters.Add(new Field("LOWER(Warehouse)", Warehouse.ToLower()));
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (!string.IsNullOrEmpty(ProductManager))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_GPRODUCT)", ProductManager.ToLower()));
                }
                if (!string.IsNullOrEmpty(State))
                {
                    filters.Add(new Field("State", State == "O" ? "Abierto" : "Cerrado"));
                }
                if (!string.IsNullOrEmpty(Complete))
                {
                    List<string> codes = Complete.Split(',').ToList();
                    if (codes.Count == 2)
                    {
                        if (codes.Contains("I") & codes.Contains("PI"))
                        {
                            filters.AddRange(new[] { new Field("NonCompleteItem", 0, Operators.HigherThan) });
                        }
                        if (codes.Contains("I") & codes.Contains("C"))
                        {
                            filters.AddRange(new[] { new Field("NonCompleteItem", 0), new Field("ItemsCount - NonCompleteItem", 0), new Field(LogicalOperators.Or) });
                        }
                        if (codes.Contains("PI") & codes.Contains("C"))
                        {
                            filters.AddRange(new[] { new Field("NonCompleteItem", 0), new Field("ItemsCount - NonCompleteItem", 0, Operators.Different), new Field(LogicalOperators.Or) });
                        }
                    }
                    if (codes.Count == 1)
                    {
                        if (codes.Contains("I")) filters.AddRange(new[] { new Field("NonCompleteItem", 0, Operators.HigherThan), new Field("ItemsCount - NonCompleteItem", 0), new Field(LogicalOperators.And) });
                        if (codes.Contains("PI")) filters.AddRange(new[] { new Field("NonCompleteItem", 0, Operators.HigherThan), new Field("ItemsCount - NonCompleteItem", 0, Operators.Different), new Field(LogicalOperators.And) });
                        if (codes.Contains("C")) filters.AddRange(new[] { new Field("NonCompleteItem", 0) });
                    }
                }
                if (!string.IsNullOrEmpty(ClientOrder))
                {
                    filters.Add(new Field("LOWER(ClientOrder)", ClientOrder.ToLower(), Operators.Likes));
                }
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    itemFilters.AddRange(new[] { new Field("LOWER(s1.ItemCode)", ItemCode.ToLower(), Operators.Likes), new Field("LOWER(s1.ItemName)", ItemCode.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                }
                if (!string.IsNullOrEmpty(Category))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_CATEGORIA)", Category.ToLower()));
                }
                if (!string.IsNullOrEmpty(Subcategory))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_SUBCATEG)", Subcategory.ToLower()));
                }
                if (!string.IsNullOrEmpty(Line))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_LINEA)", Line.ToLower()));
                }
            }
            CompleteFilters(ref filters);
            CompleteFilters(ref itemFilters);
            var orders = bcOrder.List4(filters, itemFilters, "1", BEA.relOrder.Files);
            if (orders?.Count() > 0)
            {
                string ids = "";
                List<BEA.DocumentRelated> relatedDocs = new();
                ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "santa cruz" select o.Id);
                relatedDocs.AddRange(bcOrder.ListRelatedDocuments("Santa Cruz", ids));
                ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "iquique" select o.Id);
                relatedDocs.AddRange(bcOrder.ListRelatedDocuments("Iquique", ids));
                ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "miami" select o.Id);
                relatedDocs.AddRange(bcOrder.ListRelatedDocuments("Miami", ids));

                ids = string.Join(",", from o in orders where o.Subsidiary.ToLower() == "santa cruz" select o.DocNumber);
                BCL.Transport bcTransport = new();
                var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();

                items = orders.Select(x => new BEA.Document
                {
                    Id = x.Id,
                    Authorized = x.Authorized == "Y",
                    ClientCode = x.ClientCode ?? "",
                    ClientName = x.ClientName ?? "",
                    ClientOrder = x.ClientOrder ?? "",
                    Header = x.Header?.Trim() ?? "",
                    Footer = x.Footer?.Trim() ?? "",
                    Complete = x.NonCompleteItem == 0,
                    DocDate = x.DocDate,
                    DocNumber = x.DocNumber,
                    DocType = "Orden de Venta",
                    Files = x.Files.Select(i => new BEA.Attachment { Path = i.Path, FileName = i.FileName, FileExt = i.FileExt }),
                    Margin = x.Margin,
                    TaxlessTotal = x.TaxlessTotal,
                    Margin0100 = x.TaxlessTotal > 0 ? 100 * (x.Margin / x.TaxlessTotal) : 0,
                    OpenAmount = x.OpenAmount,
                    SellerName = x.SellerName ?? "",
                    State = x.State,
                    Subsidiary = ToTitle(x.Subsidiary),
                    Warehouse = ToTitle(x.Warehouse ?? ""),
                    Total = x.Total,
                    RelatedDocs = relatedDocs.FindAll(r => r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower()),
                    Transport = (from t in transportList
                                 where t.StringValues == x.DocNumber.ToString() & x.Subsidiary.ToLower() == "santa cruz"
                                 select new BEA.TransportOrder
                                 {
                                     DocNumber = t.DocNumber,
                                     Date = t.Date,
                                     Source = t.Source.Name,
                                     Destination = t.Destination.Name,
                                     Transporter = t.Transporter.Name,
                                     DeliveryTo = t.DeliveryTo,
                                     Observations = t.Observations ?? "",
                                     Weight = t.Weight,
                                     QuantityPieces = t.QuantityPieces,
                                     RemainingAmount = t.RemainingAmount,
                                     StringValues = t.StringValues
                                 }).Distinct()
                }).ToList();
            }
            return items;
        }

        private List<BEA.Document> GetNotes(string ClientCode, int? DocNumber, DateTime? InitialDate, DateTime? FinalDate, string SellerCode, string ProductManager, string ItemCode, string Category, string Subcategory, string Line, string Subsidiary, string Warehouse)
        {
            List<BEA.Document> items = new();
            BCA.Note bcNote = new();
            List<Field> filters = new(), itemFilters = new();
            if (!string.IsNullOrEmpty(ClientCode))
            {
                filters.Add(new Field("LOWER(ClientCode)", ClientCode.ToLower()));
            }
            if (ProfileCode == (long)BEE.Types.Profile.Sales || ProfileCode == (long)BEE.Types.Profile.SpecialSales)
            {
                if (!string.IsNullOrEmpty(SellerCode))
                {
                    filters.AddRange(new[] { new Field("LOWER(SellerCode)", SellerCode.ToLower()), new Field("LOWER(SellerCode)", "dmc"), new Field("SlpCode", -1), new Field(LogicalOperators.Or), new Field(LogicalOperators.Or) });
                }
            }
            if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
            {
                BCS.UserData bcData = new();
                BES.UserData data = bcData.SearchByUser(UserCode);
                if (!string.IsNullOrEmpty(data?.SellerCode))
                {
                    itemFilters.Add(new Field("LOWER(U_CODGRT)", data.SellerCode.ToLower()));
                }
            }
            if (DocNumber.HasValue && DocNumber.Value > 0)
            {
                filters.Add(new Field("DocNumber", DocNumber.Value));
            }
            else
            {
                if (!string.IsNullOrEmpty(SellerCode) && ProfileCode != (long)BEE.Types.Profile.Sales && ProfileCode != (long)BEE.Types.Profile.SpecialSales && ProfileCode != (long)BEE.Types.Profile.ProductManagement)
                {
                    filters.Add(new Field("LOWER(SellerCode)", SellerCode.ToLower()));
                }
                if (!string.IsNullOrEmpty(Subsidiary))
                {
                    filters.Add(new Field("LOWER(Subsidiary)", Subsidiary.ToLower()));
                }
                if (!string.IsNullOrEmpty(Warehouse))
                {
                    filters.Add(new Field("LOWER(Warehouse)", Warehouse.ToLower()));
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (!string.IsNullOrEmpty(ProductManager))
                {
                    itemFilters.Add(new Field("LOWER(t2.U_GPRODUCT)", ProductManager.ToLower()));
                }
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    itemFilters.AddRange(new[] { new Field("LOWER(t2.ItemCode)", ItemCode.ToLower(), Operators.Likes), new Field("LOWER(t2.ItemName)", ItemCode.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                }
                if (!string.IsNullOrEmpty(Category))
                {
                    itemFilters.Add(new Field("LOWER(t2.U_CATEGORIA)", Category.ToLower()));
                }
                if (!string.IsNullOrEmpty(Subcategory))
                {
                    itemFilters.Add(new Field("LOWER(t2.U_SUBCATEG)", Subcategory.ToLower()));
                }
                if (!string.IsNullOrEmpty(Line))
                {
                    itemFilters.Add(new Field("LOWER(t2.U_LINEA)", Line.ToLower()));
                }
            }
            CompleteFilters(ref filters);
            CompleteFilters(ref itemFilters);
            var notes = bcNote.List(filters, itemFilters, "1", BEA.relNote.Files);
            if (notes?.Count() > 0)
            {
                string ids = "";
                List<BEA.DocumentRelated> relatedDocs = new();
                ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "santa cruz" select o.Id);
                relatedDocs.AddRange(bcNote.ListRelatedDocuments("Santa Cruz", ids));
                ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "iquique" select o.Id);
                relatedDocs.AddRange(bcNote.ListRelatedDocuments("Iquique", ids));
                ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "miami" select o.Id);
                relatedDocs.AddRange(bcNote.ListRelatedDocuments("Miami", ids));

                ids = string.Join(",", from o in relatedDocs where o.Subsidiary.ToLower() == "santa cruz" & o.DocType == "Orden de Venta" select o.DocNumber);
                BCL.Transport bcTransport = new();
                var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();

                items = notes.Select(x => new BEA.Document
                {
                    Id = x.Id,
                    ClientCode = x.ClientCode ?? "",
                    ClientName = x.ClientName ?? "",
                    DocDate = x.DocDate,
                    DocNumber = x.DocNumber,
                    DocType = "Nota de Venta",
                    Files = x.Files.Select(i => new BEA.Attachment { Path = i.Path, FileName = i.FileName, FileExt = i.FileExt }),
                    Margin = x.Margin,
                    TaxlessTotal = x.TaxlessTotal,
                    Margin0100 = x.TaxlessTotal > 0 ? 100 * (x.Margin / x.TaxlessTotal) : 0,
                    SellerName = x.SellerName ?? "",
                    Subsidiary = ToTitle(x.Subsidiary),
                    Warehouse = ToTitle(x.Warehouse ?? ""),
                    Total = x.Total,
                    RelatedDocs = relatedDocs.FindAll(r => r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower()),
                    Series = x.Series,
                    Transport = (from t in transportList
                                 where (from r in relatedDocs where r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower() & r.DocType == "Orden de Venta" select r.DocNumber.ToString()).Contains(t.StringValues)
                                 select new BEA.TransportOrder
                                 {
                                     DocNumber = t.DocNumber,
                                     Date = t.Date,
                                     Source = t.Source.Name,
                                     Destination = t.Destination.Name,
                                     Transporter = t.Transporter.Name,
                                     DeliveryTo = t.DeliveryTo,
                                     Observations = t.Observations ?? "",
                                     Weight = t.Weight,
                                     QuantityPieces = t.QuantityPieces,
                                     RemainingAmount = t.RemainingAmount,
                                     StringValues = t.StringValues
                                 }).Distinct()
                }).ToList();
            }
            return items;
        }

        private List<BEA.Document> GetDeliveryNotes(string ClientCode, int? DocNumber, DateTime? InitialDate, DateTime? FinalDate, string SellerCode, string ProductManager, string ItemCode, string Category, string Subcategory, string Line, string Subsidiary, string Warehouse)
        {
            List<BEA.Document> items = new();
            BCA.DeliveryNote bcNote = new();
            List<Field> filters = new(), itemFilters = new();
            if (!string.IsNullOrEmpty(ClientCode))
            {
                filters.Add(new Field("LOWER(ClientCode)", ClientCode.ToLower()));
            }
            if (ProfileCode == (long)BEE.Types.Profile.Sales || ProfileCode == (long)BEE.Types.Profile.SpecialSales)
            {
                if (!string.IsNullOrEmpty(SellerCode))
                {
                    filters.AddRange(new[] { new Field("LOWER(SellerCode)", SellerCode.ToLower()), new Field("LOWER(SellerCode)", "dmc"), new Field("SlpCode", -1), new Field(LogicalOperators.Or), new Field(LogicalOperators.Or) });
                }
            }
            if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
            {
                BCS.UserData bcData = new();
                BES.UserData data = bcData.SearchByUser(UserCode);
                if (!string.IsNullOrEmpty(data?.SellerCode))
                {
                    itemFilters.Add(new Field("LOWER(U_CODGRT)", data.SellerCode.ToLower()));
                }
            }
            if (DocNumber.HasValue && DocNumber.Value > 0)
            {
                filters.Add(new Field("DocNumber", DocNumber.Value));
            }
            else
            {
                if (!string.IsNullOrEmpty(SellerCode) && ProfileCode != (long)BEE.Types.Profile.Sales && ProfileCode != (long)BEE.Types.Profile.SpecialSales && ProfileCode != (long)BEE.Types.Profile.ProductManagement)
                {
                    filters.Add(new Field("LOWER(SellerCode)", SellerCode.ToLower()));
                }
                if (!string.IsNullOrEmpty(Subsidiary))
                {
                    filters.Add(new Field("LOWER(Subsidiary)", Subsidiary.ToLower()));
                }
                if (!string.IsNullOrEmpty(Warehouse))
                {
                    filters.Add(new Field("LOWER(Warehouse)", Warehouse.ToLower()));
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (!string.IsNullOrEmpty(ProductManager))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_GPRODUCT)", ProductManager.ToLower()));
                }
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    itemFilters.AddRange(new[] { new Field("LOWER(s1.ItemCode)", ItemCode.ToLower(), Operators.Likes), new Field("LOWER(s1.ItemName)", ItemCode.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
                }
                if (!string.IsNullOrEmpty(Category))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_CATEGORIA)", Category.ToLower()));
                }
                if (!string.IsNullOrEmpty(Subcategory))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_SUBCATEG)", Subcategory.ToLower()));
                }
                if (!string.IsNullOrEmpty(Line))
                {
                    itemFilters.Add(new Field("LOWER(s1.U_LINEA)", Line.ToLower()));
                }
            }
            CompleteFilters(ref filters);
            CompleteFilters(ref itemFilters);
            var notes = bcNote.List(filters, itemFilters, "1", BEA.relDeliveryNote.Files);
            if (notes?.Count() > 0)
            {
                string ids = "";
                List<BEA.DocumentRelated> relatedDocs = new();
                ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "santa cruz" select o.Id);
                if (!string.IsNullOrEmpty(ids)) relatedDocs.AddRange(bcNote.ListRelatedDocuments("Santa Cruz", ids));
                ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "iquique" select o.Id);
                if (!string.IsNullOrEmpty(ids)) relatedDocs.AddRange(bcNote.ListRelatedDocuments("Iquique", ids));
                ids = string.Join(",", from o in notes where o.Subsidiary.ToLower() == "miami" select o.Id);
                if (!string.IsNullOrEmpty(ids)) relatedDocs.AddRange(bcNote.ListRelatedDocuments("Miami", ids));

                ids = string.Join(",", from o in relatedDocs where o.Subsidiary.ToLower() == "santa cruz" & o.DocType == "Orden de Venta" select o.DocNumber);
                BCL.Transport bcTransport = new();
                var transportList = !string.IsNullOrEmpty(ids) ? bcTransport.List(ids, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter) : new List<BEL.Transport>();

                items = notes.Select(x => new BEA.Document
                {
                    Id = x.Id,
                    ClientCode = x.ClientCode,
                    ClientName = x.ClientName,
                    DocDate = x.DocDate,
                    DocNumber = x.DocNumber,
                    DocType = "Nota de Entrega",
                    Files = x.Files.Select(i => new BEA.Attachment { Path = i.Path, FileName = i.FileName, FileExt = i.FileExt }),
                    Margin = x.Margin,
                    TaxlessTotal = x.TaxlessTotal,
                    Margin0100 = x.TaxlessTotal > 0 ? 100 * (x.Margin / x.TaxlessTotal) : 0,
                    SellerName = x.SellerName,
                    Subsidiary = ToTitle(x.Subsidiary),
                    Warehouse = ToTitle(x.Warehouse),
                    Total = x.Total,
                    RelatedDocs = relatedDocs.FindAll(r => r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower()),
                    Transport = (from t in transportList
                                 where (from r in relatedDocs where r.BaseId == x.Id & r.Subsidiary.ToLower() == x.Subsidiary.ToLower() & r.DocType == "Orden de Venta" select r.DocNumber.ToString()).Contains(t.StringValues)
                                 select new BEA.TransportOrder
                                 {
                                     DocNumber = t.DocNumber,
                                     Date = t.Date,
                                     Source = t.Source.Name,
                                     Destination = t.Destination.Name,
                                     Transporter = t.Transporter.Name,
                                     DeliveryTo = t.DeliveryTo,
                                     Observations = t.Observations ?? "",
                                     Weight = t.Weight,
                                     QuantityPieces = t.QuantityPieces,
                                     RemainingAmount = t.RemainingAmount,
                                     StringValues = t.StringValues
                                 }).Distinct()
                }).ToList();
            }
            return items;
        }

        #endregion
    }
}