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
using BCS = BComponents.Security;
using BCV = BComponents.Visits;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEV = BEntities.Visits;

namespace Portal.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    public class LessActiveUsersController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration config;
        #endregion

        #region Constructores

        public LessActiveUsersController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { config = configuration; }

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

        public IActionResult GetSalesman()
        {
            BCA.Seller bcSalesman = new();
            var lstSalesmen = bcSalesman.ListWithClient(new List<Field>(), "1");
            var lstItems = (from s in lstSalesmen select new { Id = s.ShortName, s.Name }).Distinct();
            return Json(lstItems);
        }

        public IActionResult Filter(string SellerCode, string ClientName)
        {
            string message = "";
            List<BES.LastLogs> items = new();
            try
            {
                items = GetItems(SellerCode, ClientName);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        public IActionResult Export(string SellerCode, string ClientName)
        {
            List<BES.LastLogs> lstTemp, lstItems = GetItems(SellerCode, ClientName);
            using ExcelPackage objExcel = new();
            ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
            FileInfo logoDMC = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo.jpg"));
            var imgLogoDMC = wsMain.Drawings.AddPicture("logoDMC", logoDMC);
            imgLogoDMC.From.Column = 0;
            imgLogoDMC.From.Row = 0;
            imgLogoDMC.From.ColumnOff = 1;
            imgLogoDMC.From.RowOff = 1;
            imgLogoDMC.SetSize(120, 47);
            wsMain.Row(1).Height = 50;

            wsMain.Cells.Style.Font.Size = 10;
            wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
            wsMain.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            wsMain.Cells[2, 1].Value = "Clientes Menos Activos al " + DateTime.Now.ToString("dd/MM/yyyy");
            wsMain.Cells[2, 1].Style.Font.Bold = true;
            wsMain.Cells[2, 1].Style.Font.Size = 20;
            wsMain.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[2, 1, 2, 4].Merge = true;
            wsMain.Cells[2, 1, 2, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
            wsMain.Cells[2, 1, 2, 4].Style.Font.Color.SetColor(System.Drawing.Color.White);

            //foreach(var item in lstItems) {
            //    item.Period = showPeriod(item.Days);
            //}

            int intRow = 4;
            List<string> lstPeriods = (from i in lstItems select i.Period).Distinct().ToList();
            foreach (string period in lstPeriods)
            {
                lstTemp = (from i in lstItems where i.Period == period select i).ToList();
                wsMain.Cells[intRow, 1].Value = period + "     ( Total: " + lstTemp.Count() + " )";
                wsMain.Cells[intRow, 1, intRow, 4].Merge = true;
                wsMain.Cells[intRow, 1, intRow, 4].Style.Font.Bold = true;
                wsMain.Cells[intRow, 1, intRow, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                wsMain.Cells[intRow, 1, intRow, 4].Style.Font.Color.SetColor(System.Drawing.Color.White);
                intRow++;
                wsMain.Cells[intRow, 1].Value = "Cliente";
                wsMain.Cells[intRow, 2].Value = "Correo";
                wsMain.Cells[intRow, 3].Value = "Vendedor";
                wsMain.Cells[intRow, 4].Value = "Ultimo Ingreso";
                wsMain.Cells[intRow, 1, intRow, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                intRow++;
                foreach (var item in lstTemp)
                {
                    wsMain.Cells[intRow, 1].Value = item.ClientName;
                    wsMain.Cells[intRow, 2].Value = item.EMail;
                    wsMain.Cells[intRow, 3].Value = item.Seller;
                    wsMain.Cells[intRow, 4].Value = item.LastLog != null ? item.LastLog.Value.ToString("dd/MM/yyyy HH:mm:ss") : "";
                    intRow++;
                }
            }
            wsMain.Cells.AutoFitColumns();

            string strFileName = "Clientes Menos Activos al " + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
            byte[] objData = objExcel.GetAsByteArray();
            objExcel.Dispose();
            return File(objData, "application/xlsx", strFileName);
        }

        public IActionResult DetailSessions(string CardCode, DateTime LogDate)
        {
            string message = "";
            try
            {
                BCS.SessionHistory bcSessions = new();
                List<BES.SessionHistory> lstItems = bcSessions.List(CardCode, LogDate, BES.relSessionHistory.User);
                var items = lstItems.Select(x => new { UserName = x.User.Name, x.LogDate });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            //return PartialView("LogIns", lstItems);
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private List<BES.LastLogs> GetItems(string SellerCode, string ClientName)
        {
            BCS.LastLogs bcLog = new();
            BCS.BlackList bcBlackList = new();
            BCA.Client bcClient = new();
            BCS.User bcUser = new();

            List<Field> lstFilter = new() { new Field { Name = "IFNULL(Disabled , '')", Value = "Y", Operator = Operators.Different } };
            if (!string.IsNullOrWhiteSpace(SellerCode))
            {
                lstFilter.AddRange(new[] { new Field("SellerCode", SellerCode.Trim()), new Field(LogicalOperators.And) });
            }
            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                lstFilter.AddRange(new[] {
                    new Field("LOWER(CardName)", ClientName.Trim().ToLower(), Operators.Likes), new Field("LOWER(CardFName)", ClientName.Trim().ToLower(), Operators.Likes), new Field("LOWER(CardCode)", ClientName.Trim().ToLower(), Operators.Likes),
                    new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.And)
                });
            }
            IEnumerable<BEA.Client> lstClients = bcClient.ListShort2(lstFilter, "1");
            List<BES.LastLogs> lstFinal = new(), lstItems = bcLog.List(DateTime.Now);
            IEnumerable<BES.BlackList> lstBlackList = bcBlackList.List("1");
            IEnumerable<BES.User> lstUsers = bcUser.List("1");

            foreach (var client in lstClients)
            {
                if (!(from b in lstBlackList select b.CardCode.ToUpper()).Contains(client.CardCode.ToUpper()))
                {
                    BES.LastLogs beItem;
                    if ((from u in lstUsers where u.CardCode.ToUpper() == client.CardCode.ToUpper() select u).Count() > 0)
                    {
                        beItem = (from i in lstItems where i.CardCode.ToUpper() == client.CardCode.ToUpper() select i).FirstOrDefault();
                        if (beItem == null)
                        {
                            beItem = new BES.LastLogs { CardCode = client.CardCode, Days = 1000, Period = "Ultimo Ingreso: Nunca" };
                        }
                        else
                        {
                            double intDays = (DateTime.Now - beItem.LastLog.Value).TotalDays;
                            if (intDays >= 0 & intDays <= 15)
                            {
                                beItem.Days = 0;
                                beItem.Period = "Ultimo Ingreso: Menor a 15 días";
                            }
                            if (intDays > 15 & intDays <= 30)
                            {
                                beItem.Days = 15;
                                beItem.Period = "Ultimo Ingreso: Mayor a 15 días";
                            }
                            if (intDays > 30 & intDays <= 90)
                            {
                                beItem.Days = 30;
                                beItem.Period = "Ultimo Ingreso: Mayor a 30 días";
                            }
                            if (intDays > 90 & intDays <= 180)
                            {
                                beItem.Days = 90;
                                beItem.Period = "Ultimo Ingreso: Mayor a 90 días";
                            }
                            if (intDays > 180 & intDays <= 365)
                            {
                                beItem.Days = 180;
                                beItem.Period = "Ultimo Ingreso: Mayor a 180 días";
                            }
                            if (intDays > 365)
                            {
                                beItem.Days = 365;
                                beItem.Period = "Ultimo Ingreso: Mayor a 365 días";
                            }
                        }
                        beItem.ClientName = $"{client.CardCode} - {client.CardName}";
                        beItem.Seller = client.SellerName;
                    }
                    else
                    {
                        beItem = new BES.LastLogs { CardCode = client.CardCode, ClientName = $"{client.CardCode} - {client.CardName}", Seller = client.SellerName, Days = 1100, Period = "Sin Acceso" };
                    }
                    beItem.EMail = client.EMail;
                    lstFinal.Add(beItem);
                }
            }
            lstFinal = (from f in lstFinal
                        orderby f.Days descending
                        group f by new { f.CardCode, f.ClientName, f.EMail, f.Seller } into g
                        select new BES.LastLogs
                        {
                            CardCode = g.Key.CardCode,
                            ClientName = g.Key.ClientName,
                            EMail = g.Key.EMail,
                            Seller = g.Key.Seller,
                            Days = g.FirstOrDefault().Days,
                            Period = g.FirstOrDefault().Period,
                            LastLog = g.FirstOrDefault().LastLog
                        }).ToList();
            return lstFinal;
        }

        #endregion
    }
}