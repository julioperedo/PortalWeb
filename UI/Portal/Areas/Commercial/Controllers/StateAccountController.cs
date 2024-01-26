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
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public class StateAccountController : BaseController
    {
        #region Constructores

        public StateAccountController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                if (CardCode == HomeCardCode)
                {
                    bool seeMargin = GetPermission("VerMargen") > 0, seeAllClients = GetPermission("SeeAllClients") > 0;
                    BCS.UserData bcData = new();
                    BES.UserData data = bcData.SearchByUser(UserCode);
                    ViewBag.Permissions = new { seeMargin, seeAllClients, sellerCode = data?.SellerCode ?? "" };
                    return View();
                }
                else
                {
                    ViewData["Title"] = $"Estado de Cuenta : {CardName}";
                    ViewBag.CardCode = CardCode;
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Detail(string id)
        {
            if (IsAllowed("Commercial", "StateAccount", "Index"))
            {
                if (CardCode == HomeCardCode)
                {
                    ViewBag.CardCode = id;
                    return View("Index");
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
                    catch (Exception)
                    {

                    }
                    ViewData["Title"] = $"Estado de Cuenta : {clientName}";
                    ViewBag.CardCode = CardCode;
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        // GET: Customer/StateAccount/Filter
        public IActionResult Filter(string CardCode, string Subsidiary, string SalesMan)
        {
            string message = "";
            IEnumerable<BEA.StateAccountItem> items = new List<BEA.StateAccountItem>();
            IEnumerable<BEA.Client> lstResume = new List<BEA.Client>();
            try
            {
                items = GetResults(CardCode, Subsidiary, SalesMan);
                lstResume = GetResume(CardCode, Subsidiary, SalesMan);
                var resume = lstResume.Select(x => new
                {
                    clientCode = x.CardCode,
                    clientName = x.CardName,
                    x.Subsidiary,
                    x.CreditLimit,
                    x.Balance,
                    x.OrdersBalance,
                    x.Terms,
                    availableBalance = x.CreditLimit - x.Balance,
                    dueAmount = (from i in items where i.Days > 0 & x.Subsidiary.ToLower() == i.Subsidiary.ToLower() & i.State?.ToLower() == "en mora" select i.Balance).Sum(),
                    dueItems = (from i in items where i.Days > 0 & x.Subsidiary.ToLower() == i.Subsidiary.ToLower() & i.State?.ToLower() == "en mora" select new { i.Days, i.Balance }).ToList()
                });
                return Json(new { message, items, resume });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ExportExcel(string CardCode, string Subsidiary, string SalesMan)
        {
            string seeMargin = GetPermission("VerMargen") > 0 ? "Y" : "N";
            bool localUser = HomeCardCode == CardCode;
            using ExcelPackage objExcel = new();
            FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));
            IEnumerable<BEA.StateAccountItem> lstItems = GetResults(CardCode, Subsidiary, SalesMan);
            IEnumerable<BEA.Client> lstResume = GetResume(CardCode, Subsidiary, SalesMan);

            ExcelWorksheet wsDetail = objExcel.Workbook.Worksheets.Add("Detail");
            wsDetail.Name = "Estado de Cuenta";
            wsDetail.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsDetail.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
            wsDetail.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            wsDetail.Cells.Style.Font.Size = 9;

            wsDetail.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsDetail.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsDetail.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsDetail.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsDetail.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsDetail.Column(14).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsDetail.Column(15).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsDetail.Column(16).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            if (localUser | seeMargin == "Y") wsDetail.Column(17).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            wsDetail.Column(5).Style.Numberformat.Format = "dd/MM/yyyy";
            wsDetail.Column(10).Style.Numberformat.Format = "dd/MM/yyyy";
            wsDetail.Column(13).Style.Numberformat.Format = "dd/MM/yyyy";
            wsDetail.Column(14).Style.Numberformat.Format = "#,##0.00";
            wsDetail.Column(15).Style.Numberformat.Format = "#,##0.00";
            if (localUser | seeMargin == "Y") wsDetail.Column(16).Style.Numberformat.Format = "#,##0.00 %";

            wsDetail.Cells[1, 1].Style.Font.Size = 12;
            wsDetail.Cells[1, 1].Style.Font.Bold = true;
            wsDetail.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsDetail.Cells[1, 1, 1, 17].Merge = true;
            wsDetail.Cells[4, 1].Value = $"Fecha: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";

            wsDetail.Cells[6, 1].Value = "Cliente";
            wsDetail.Cells[6, 2].Value = "Sucursal";
            wsDetail.Cells[6, 3].Value = "Almacén";
            wsDetail.Cells[6, 4].Value = "Tipo Doc.";
            wsDetail.Cells[6, 5].Value = "Fehca Doc.";
            wsDetail.Cells[6, 6].Value = "Nro. Doc.";
            wsDetail.Cells[6, 7].Value = "Vendedor";
            wsDetail.Cells[6, 8].Value = "Nota Venta";
            wsDetail.Cells[6, 9].Value = "Orden Venta";
            wsDetail.Cells[6, 10].Value = "Fecha Retiro";
            wsDetail.Cells[6, 11].Value = "Orden Cliente";
            wsDetail.Cells[6, 12].Value = "Término";
            wsDetail.Cells[6, 13].Value = "Fecha Venc.";
            wsDetail.Cells[6, 14].Value = "Total";
            wsDetail.Cells[6, 15].Value = "Balance";
            if (localUser | seeMargin == "Y")
            {
                wsDetail.Cells[6, 16].Value = "Margen";
                wsDetail.Cells[6, 17].Value = "Días";
                wsDetail.Cells[6, 18].Value = "Estado";
            }
            else
            {
                wsDetail.Cells[6, 16].Value = "Días";
                wsDetail.Cells[6, 17].Value = "Estado";
            }

            wsDetail.Cells[6, 1, 6, 18].Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsDetail.Cells[6, 1, 6, 18].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
            wsDetail.Cells[6, 1, 6, 18].Style.Font.Color.SetColor(Color.White);
            wsDetail.Cells[6, 1, 6, 18].Style.Font.Bold = true;

            if (lstItems?.Count() > 0)
            {
                wsDetail.Cells[1, 1].Value = "ESTADO DE CUENTA ";

                int intFila = 7;
                foreach (var item in lstItems)
                {
                    wsDetail.Cells[intFila, 1].Value = item.ClientName;
                    wsDetail.Cells[intFila, 2].Value = item.Subsidiary;
                    wsDetail.Cells[intFila, 3].Value = item.Warehouse;
                    wsDetail.Cells[intFila, 4].Value = item.Type;
                    wsDetail.Cells[intFila, 5].Value = item.DocDate;
                    wsDetail.Cells[intFila, 6].Value = item.DocNum;
                    wsDetail.Cells[intFila, 7].Value = item.SellerName;
                    wsDetail.Cells[intFila, 8].Value = item.SaleNote != 0 ? item.SaleNote.ToString() : "";
                    wsDetail.Cells[intFila, 9].Value = item.SaleOrder;
                    wsDetail.Cells[intFila, 10].Value = item.PickupDate;
                    wsDetail.Cells[intFila, 11].Value = item.ClientOrder;
                    wsDetail.Cells[intFila, 12].Value = item.Terms;
                    wsDetail.Cells[intFila, 13].Value = item.DueDate;
                    wsDetail.Cells[intFila, 14].Value = item.Total;
                    wsDetail.Cells[intFila, 15].Value = item.Balance;
                    if (localUser | seeMargin == "Y")
                    {
                        wsDetail.Cells[intFila, 16].Value = item.PercetageMargin;
                        wsDetail.Cells[intFila, 17].Value = item.Days;
                        wsDetail.Cells[intFila, 18].Value = item.State;
                    }
                    else
                    {
                        wsDetail.Cells[intFila, 16].Value = item.Days;
                        wsDetail.Cells[intFila, 17].Value = item.State;
                    }

                    intFila += 1;
                }
                wsDetail.Cells[intFila, 15].Value = (from i in lstItems select i.Balance).Sum();
                wsDetail.Cells[intFila, 15].Style.Numberformat.Format = "#,##0.00";

                wsDetail.Cells[intFila, 1, intFila, 18].Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsDetail.Cells[intFila, 1, intFila, 18].Style.Fill.BackgroundColor.SetColor(Color.DimGray);
                wsDetail.Cells[intFila, 1, intFila, 18].Style.Font.Color.SetColor(Color.White);
                wsDetail.Cells[intFila, 1, intFila, 18].Style.Font.Bold = true;
            }
            else
            {
                wsDetail.Cells[1, 1].Value = "ESTADO DE CUENTA";
            }

            wsDetail.Cells.AutoFitColumns();
            wsDetail.View.FreezePanes(7, 1);
            wsDetail.PrinterSettings.Orientation = eOrientation.Landscape;
            wsDetail.PrinterSettings.LeftMargin = 0.3m;
            wsDetail.PrinterSettings.RightMargin = 0.3m;
            wsDetail.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            wsDetail.PrinterSettings.RepeatRows = wsDetail.Cells["1:6"];
            wsDetail.PrinterSettings.RepeatColumns = wsDetail.Cells["A:N"];

            var imgLogo = wsDetail.Drawings.AddPicture("logo", logo);
            imgLogo.SetPosition(5, 5);
            imgLogo.SetSize(90);

            ExcelWorksheet wsResume = objExcel.Workbook.Worksheets.Add("Resume");

            wsResume.Name = "Resumen";

            wsResume.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsResume.Cells.Style.Fill.PatternColor.SetColor(Color.White);
            wsResume.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
            wsResume.Cells.Style.Font.Size = 9;

            wsResume.Cells[1, 1].Style.Font.Size = 12;
            wsResume.Cells[1, 1].Style.Font.Bold = true;
            wsResume.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsResume.Cells[1, 1, 1, 14].Merge = true;

            wsResume.Cells[5, 1].Value = "Cliente";
            wsResume.Cells[5, 2].Value = "Límite de Crédito";
            wsResume.Cells[5, 3].Value = "Saldo Disponible";
            wsResume.Cells[5, 4].Value = "Balance";
            wsResume.Cells[5, 8].Value = "Pedidos";
            wsResume.Cells[5, 12].Value = "Términos";
            wsResume.Cells[6, 4].Value = "Total";
            wsResume.Cells[6, 5].Value = "DMC SA";
            wsResume.Cells[6, 6].Value = "DMC LA";
            wsResume.Cells[6, 7].Value = "DMC IQQ";
            wsResume.Cells[6, 8].Value = "Total";
            wsResume.Cells[6, 9].Value = "DMC SA";
            wsResume.Cells[6, 10].Value = "DMC LA";
            wsResume.Cells[6, 11].Value = "DMC IQQ";
            wsResume.Cells[6, 12].Value = "DMC SA";
            wsResume.Cells[6, 13].Value = "DMC LA";
            wsResume.Cells[6, 14].Value = "DMC IQQ";
            wsResume.Cells[5, 1, 6, 1].Merge = true;
            wsResume.Cells[5, 2, 6, 2].Merge = true;
            wsResume.Cells[5, 3, 6, 3].Merge = true;
            wsResume.Cells[5, 4, 5, 7].Merge = true;
            wsResume.Cells[5, 8, 5, 11].Merge = true;
            wsResume.Cells[5, 12, 5, 14].Merge = true;
            wsResume.Cells[5, 4, 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsResume.Cells[6, 4, 6, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            wsResume.Cells[5, 1, 6, 14].Style.Font.Size = 10;
            wsResume.Cells[5, 1, 6, 14].Style.Font.Bold = true;

            int intRow = 6;
            //if (lstResume?.Count > 0)
            //{
            //    wsResume.Cells[1, 1].Value = $"RESUMEN DE ESTADO DE CUENTA {lstResume[0].ClientName}";
            //    foreach (var beItem in lstResume)
            //    {
            //        intRow += 1;

            //        wsResume.Cells[intRow, 1].Value = beItem.ClientName;
            //        wsResume.Cells[intRow, 2].Value = beItem.CreditLimit;
            //        wsResume.Cells[intRow, 3].Value = beItem.AvailableBalance;
            //        wsResume.Cells[intRow, 4].Value = beItem.BalanceTotal;
            //        wsResume.Cells[intRow, 5].Value = beItem.BalanceSA;
            //        wsResume.Cells[intRow, 6].Value = beItem.BalanceLA;
            //        wsResume.Cells[intRow, 7].Value = beItem.BalanceIQQ;
            //        wsResume.Cells[intRow, 8].Value = beItem.OrdersTotal;
            //        wsResume.Cells[intRow, 9].Value = beItem.OrdersSA;
            //        wsResume.Cells[intRow, 10].Value = beItem.OrdersLA;
            //        wsResume.Cells[intRow, 11].Value = beItem.OrdersIQQ;
            //        wsResume.Cells[intRow, 12].Value = beItem.TermsSA;
            //        wsResume.Cells[intRow, 13].Value = beItem.TermsLA;
            //        wsResume.Cells[intRow, 14].Value = beItem.TermsIQQ;
            //    }

            //    wsResume.Cells[6, 2, intRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //    wsResume.Cells[7, 2, intRow, 11].Style.Numberformat.Format = "#,##0.00";
            //}
            //else
            //{
            //    wsResume.Cells[1, 1].Value = "RESUMEN DE ESTADO DE CUENTA ";
            //}
            wsResume.Cells[5, 1, 6, 14].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            wsResume.Cells[5, 1, intRow, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            wsResume.Cells.AutoFitColumns();
            wsResume.PrinterSettings.LeftMargin = 0.3m;
            wsResume.PrinterSettings.RightMargin = 0.3m;
            wsResume.PrinterSettings.Orientation = eOrientation.Landscape;
            wsResume.PrinterSettings.RepeatRows = wsResume.Cells["1:6"];
            wsResume.PrinterSettings.RepeatColumns = wsResume.Cells["A:N"];
            wsResume.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            var imgLogo2 = wsResume.Drawings.AddPicture("logo", logo);
            imgLogo2.SetPosition(5, 5);
            imgLogo2.SetSize(90);

            byte[] objData = objExcel.GetAsByteArray();

            objExcel.Dispose();
            return File(objData, "application/xlsx", $"Estado-de-Cuentas-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx");
        }

        public IActionResult SaleNote(string Subsidiary, int Number, string ClientCode)
        {
            if (IsAllowed("Commercial", "StateAccount", "Index"))
            {
                if (CardCode == ClientCode | CardCode == HomeCardCode)
                {
                    Models.SaleNoteReport beData = new() { Subsidiary = ToTitle(Subsidiary), SaleNote = Number, User = UserName, CardCode = ClientCode };
                    return View(beData);
                }
                else
                {
                    return RedirectToAction("Index", "Orders");
                }
            }
            else
            {
                return RedirectToAction("Denied", "Home", new { area = "" });
            }
        }

        public IActionResult GetBankAccounts()
        {
            string message = "";
            try
            {
                var items = GetBankAccountsItems();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetOpenOrders(string CardCode, string Subsidiary)
        {
            string message = "";
            try
            {
                BCA.Order bcOrders = new();
                List<Field> filters = new() {
                    new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field("LOWER(ClientCode)", CardCode.ToLower()), new Field("State", "Abierto"),
                    new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                };
                var orders = bcOrders.List2(filters, "1");
                return Json(new { message, orders });
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

        private static IEnumerable<BEA.StateAccountItem> GetResults(string CardCode, string Subsidiary, string SalesMan)
        {
            IEnumerable<BEA.StateAccountItem> lstItems = Enumerable.Empty<BEA.StateAccountItem>();
            BCA.StateAccount bcAccount = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrEmpty(CardCode)) lstFilter.Add(new Field("ClientCode", CardCode));
            if (!string.IsNullOrEmpty(Subsidiary)) lstFilter.Add(new Field("LOWER(Subsidiary)", Subsidiary.ToLower(), Operators.In));
            if (!string.IsNullOrEmpty(SalesMan)) lstFilter.AddRange(new[] {
                new Field("SellerCode", SalesMan), new Field("LOWER(SellerCode)", "dmc"), new Field("LOWER(IFNULL(SellerCode, ''))", ""), 
                new Field(LogicalOperators.Or), new Field(LogicalOperators.Or)
            });

            int intQuantity = lstFilter.Count - 1;
            CompleteFilters(ref lstFilter);

            lstItems = bcAccount.List(lstFilter, "CAST(DocDate AS DATE)");
            lstItems.ForEach(i =>
            {
                i.ClientName = $"{i.ClientCode} - {i.ClientName}";
                i.PercetageMargin = (i.Margin / (i.TaxlessToral != 0 ? i.TaxlessToral : 1)) * 100;
                i.Header = i.Header?.Replace("\r", "<br />") ?? "";
                i.Footer = i.Footer?.Replace("\r", "<br />") ?? "";
                i.SellerName = ToTitle(i.SellerName ?? "");
                i.Warehouse = ToTitle(i.Warehouse ?? "");
                //if(i.DIAS >= 15) {
                //    i.ESTADO += " Intereses = " + (i.DIAS * i.TOTAL * 0.001m).ToString("N2");
                //}
            });
            lstItems = from i in lstItems orderby i.ClientName, i.DocDate, i.DocNum select i;
            return lstItems;
        }

        private static IEnumerable<BEA.Client> GetResume(string CardCode, string Subsidiaries, string SalesMan)
        {
            List<Field> lstFilter = new();
            BCA.Client bcClient = new();
            if (!string.IsNullOrWhiteSpace(CardCode)) lstFilter.Add(new Field("CardCode", CardCode));
            if (!string.IsNullOrWhiteSpace(Subsidiaries)) lstFilter.Add(new Field("LOWER(Subsidiary)", Subsidiaries.ToLower(), Operators.In));
            if (!string.IsNullOrWhiteSpace(SalesMan)) lstFilter.Add(new Field("SellerCode", SalesMan));
            CompleteFilters(ref lstFilter);
            IEnumerable<BEA.Client> lstTemp = bcClient.ListBalance(lstFilter, "1");
            return lstTemp;
        }

        private static List<Administration.Models.BankAccountGroup> GetBankAccountsItems()
        {
            BCL.BankAccount bcAccount = new();
            IEnumerable<BEL.BankAccount> lstAccounts = bcAccount.List("1");
            var lstItems = (from a in lstAccounts
                            group a by a.Subsidiary into g
                            select new Administration.Models.BankAccountGroup
                            {
                                Name = g.Key,
                                Items = g.Select(x => new Administration.Models.BankAccount(x)).ToList()
                            }).ToList();
            return lstItems;
        }

        #endregion
    }
}