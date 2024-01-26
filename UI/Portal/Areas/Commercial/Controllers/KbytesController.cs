using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCK = BComponents.Kbytes;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEK = BEntities.Kbytes;
using BEP = BEntities.Product;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class KbytesController : BaseController
    {
        #region Constructores

        public KbytesController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                if (CardCode == HomeCardCode)
                {
                    int perAllClients = GetPermission("KbytesAllClients"), perSeeDetail = GetPermission("KbytesSeeDetail"), perExtraPoints = GetPermission("KbytesSeeExtraPoints"), perExport = GetPermission("KbytesExport"),
                        perAddNote = GetPermission("KbytesAddNote"), perProcessClient = GetPermission("KbytesProcessClient");
                    if ((perAllClients + perExtraPoints + perSeeDetail) > 0)
                    {
                        ViewBag.KbytesData = new { AllClients = perAllClients, SeeDetail = perSeeDetail, ExtraPoints = perExtraPoints, Export = perExport, AddNote = perAddNote, ProcessClient = perProcessClient };
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Denied", "Home", new { area = "" });
                    }
                }
                else
                {
                    ViewBag.CardCode = CardCode;
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetDetail(string CardCode)
        {
            string message = "";
            try
            {
                var (beClient, years, claimedPoints, availablePoints) = GetItems(CardCode);
                return Json(new
                {
                    message,
                    years = years.Select(x => new { x.Year, x.Amount, x.Points, Status = x.Status?.Name ?? "Registered" }),
                    claimedPoints,
                    availablePoints
                });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetNotes(string CardCode, int Year)
        {
            string message = "";
            try
            {
                BCK.StatusDetail bcDetail = new();
                var details = bcDetail.ListNotes(CardCode, Year, "Id", BEK.relStatusDetail.Note, BEK.relStatusDetail.Status, BEK.relStatusDetail.StatusUsed, BEK.relStatusDetail.Award);

                var items = details.Select(x => new
                {
                    x.Note?.Id,
                    x.Note?.Subsidiary,
                    x.Note?.Number,
                    Date = x.Note?.Date ?? x.Award.ClaimDate,
                    x.Note?.Amount,
                    Status = x.Status.Name,
                    StatusUsed = x.StatusUsed.Name,
                    x.Points,
                    x.ExtraPoints,
                    x.ExtraPointsPeriod,
                    x.AcceleratorPeriod,
                    ItemPoints = x.Points + x.ExtraPoints + x.ExtraPointsPeriod,
                    x.TotalPoints,
                    x.TotalAmount
                });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetAwardsClaimed(string CardCode)
        {
            string message = "";
            try
            {
                BCK.ClaimedAward bcAwards = new();
                List<Field> filters = new() { new Field("CardCode", CardCode) };
                IEnumerable<BEK.ClaimedAward> awards = bcAwards.List(filters, "ClaimDate DESC", BEK.relClaimedAward.Award);
                var items = awards.Select(x => new { award = x.Award.Name, x.Quantity, x.ClaimDate, x.Points });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetNoteItems(long NoteId)
        {
            string message = "";
            try
            {
                BCK.ClientNoteDetail bcItems = new();
                List<Field> filters = new() { new Field("IdNote", NoteId), new Field("AcceleratedTotal", 0, Operators.HigherThan), new Field(LogicalOperators.And) };
                var details = bcItems.List(filters, "1", BEK.relClientNoteDetail.Product);
                var items = details.Select(x => new { x.Product.ItemCode, ItemName = x.Product.Name, x.Product.Line, x.Quantity, x.Total, x.AcceleratedQuantity, x.AcceleratedTotal, x.Accelerator, x.ExtraPoints });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetClientsBySeller()
        {
            IEnumerable<BEA.Client> clients;
            try
            {
                BCS.UserData bcData = new();
                var data = bcData.SearchByUser(UserCode);
                if (!string.IsNullOrEmpty(data?.SellerCode))
                {
                    List<Field> filters = new() { new Field("SellerCode", data.SellerCode) };
                    BCA.Client bcClient = new();
                    clients = bcClient.ListShort(filters, "CardName");
                }
                else
                {
                    clients = new List<BEA.Client>();
                }
            }
            catch (Exception)
            {
                clients = new List<BEA.Client>();
            }
            var items = clients.Select(x => new { Code = x.CardCode, Name = $"{x.CardCode} - {x.CardName}" });
            return Json(items);
        }

        public IActionResult ExportExcel(string CardCode)
        {
            var (beClient, years, claimedPoints, availablePoints) = GetItems(CardCode);
            bool localUser = base.CardCode == HomeCardCode;
            using (ExcelPackage objExcel = new ExcelPackage())
            {
                ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
                wsMain.Name = "Resumen";
                wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
                wsMain.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                wsMain.Cells.Style.Font.Size = 9;

                wsMain.Column(1).Width = 27;
                wsMain.Column(2).Width = 2;

                wsMain.Cells[1, 1, 1, 4].Merge = true;
                wsMain.Cells[1, 1].Value = $"{beClient.CardCode} - {beClient.CardName}";
                wsMain.Cells[1, 1].Style.Font.Bold = true;
                wsMain.Cells[1, 1].Style.Font.Size = 13;

                wsMain.Cells[3, 1, 4, 1].Merge = true;
                wsMain.Cells[3, 1].Value = availablePoints;
                wsMain.Cells[3, 1].Style.Numberformat.Format = "#,###,##0";
                wsMain.Cells[3, 1].Style.Font.Bold = true;
                wsMain.Cells[3, 1].Style.Font.Size = 28;
                wsMain.Cells[5, 1].Value = "puntos disponibles";
                var availableRange = wsMain.Cells[3, 1, 5, 1];
                availableRange.Style.Fill.BackgroundColor.SetColor(0, 189, 215, 238);
                availableRange.Style.Font.Color.SetColor(0, 31, 78, 120);
                availableRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                if (claimedPoints > 0)
                {
                    wsMain.Cells[7, 1].Value = claimedPoints;
                    wsMain.Cells[7, 1].Style.Numberformat.Format = "#,###,##0";
                    wsMain.Cells[7, 1].Style.Font.Bold = true;
                    wsMain.Cells[7, 1].Style.Font.Size = 14;
                    wsMain.Cells[8, 1].Value = "puntos usados";

                    var claimedRange = wsMain.Cells[7, 1, 8, 1];
                    claimedRange.Style.Fill.BackgroundColor.SetColor(0, 208, 206, 206);
                    claimedRange.Style.Font.Color.SetColor(0, 89, 89, 89);
                    claimedRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int col = 3;
                foreach (var item in years)
                {
                    var claimedRange = wsMain.Cells[3, col, 8, col + 1];
                    claimedRange.Style.Fill.BackgroundColor.SetColor(0, 208, 206, 206);
                    claimedRange.Style.Font.Color.SetColor(0, 89, 89, 89);

                    wsMain.Column(col).Width = 18;
                    wsMain.Cells[4, col].Value = "año";
                    wsMain.Cells[5, col, 6, col].Merge = true;
                    wsMain.Cells[5, col].Value = item.Year;
                    wsMain.Cells[5, col].Style.Font.Bold = true;
                    wsMain.Cells[5, col].Style.Font.Size = 26;
                    wsMain.Cells[4, col, 5, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    wsMain.Column(col).Width = 17;
                    wsMain.Cells[3, col].Value = item.Amount;
                    wsMain.Cells[3, col].Style.Numberformat.Format = "#,###,##0.00";
                    wsMain.Cells[3, col].Style.Font.Bold = true;
                    wsMain.Cells[3, col].Style.Font.Size = 14;
                    wsMain.Cells[4, col].Value = "monto acumulado";
                    wsMain.Cells[5, col].Value = item.Points;
                    wsMain.Cells[5, col].Style.Numberformat.Format = "#,###,##0";
                    wsMain.Cells[5, col].Style.Font.Bold = true;
                    wsMain.Cells[5, col].Style.Font.Size = 14;
                    wsMain.Cells[6, col].Value = "puntos acumulados";
                    wsMain.Cells[7, col].Value = item.Status?.Name ?? "Registered";
                    wsMain.Cells[7, col].Style.Font.Bold = true;
                    wsMain.Cells[7, col].Style.Font.Size = 14;
                    wsMain.Cells[8, col].Value = "status";
                    wsMain.Cells[3, col, 8, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsMain.Column(col).Width = 2;
                    col++;
                }

                BCK.ClaimedAward bcAwards = new();
                List<Field> filters = new() { new Field("CardCode", CardCode) };
                IEnumerable<BEK.ClaimedAward> awards = bcAwards.List(filters, "ClaimDate DESC", BEK.relClaimedAward.Award);

                BCK.StatusDetail bcDetail = new BCK.StatusDetail();
                var details = bcDetail.ListNotes(CardCode, "Id", BEK.relStatusDetail.Note, BEK.relStatusDetail.Status, BEK.relStatusDetail.StatusUsed, BEK.relClientNote.ClientNoteDetails, BEK.relClientNoteDetail.Product, BEK.relStatusDetail.Award);

                if (claimedPoints > 0)
                {
                    ExcelWorksheet wsClaimed = objExcel.Workbook.Worksheets.Add("Claimed");
                    wsClaimed.Name = "Puntos usados";
                    wsClaimed.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsClaimed.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
                    wsClaimed.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    wsClaimed.Cells.Style.Font.Size = 9;

                    wsClaimed.Cells[1, 1, 1, 4].Style.Fill.BackgroundColor.SetColor(0, 208, 206, 206);
                    wsClaimed.Cells[1, 1, 1, 4].Style.Font.Size = 11;
                    wsClaimed.Cells[1, 1, 1, 4].Style.Font.Bold = true;

                    int row = 1;
                    wsClaimed.Cells[row, 1].Value = "Fecha";

                    wsClaimed.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    wsClaimed.Cells[row, 2].Value = "Cantidad";
                    wsClaimed.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsClaimed.Cells[row, 3].Value = "Premio";
                    wsClaimed.Cells[row, 4].Value = "Puntos Usados";
                    wsClaimed.Cells[row++, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    foreach (var item in awards)
                    {
                        wsClaimed.Cells[row, 1].Value = item.ClaimDate;
                        wsClaimed.Cells[row, 1].Style.Numberformat.Format = "dd-MM-yyyy";
                        wsClaimed.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        wsClaimed.Cells[row, 2].Value = item.Quantity;
                        wsClaimed.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsClaimed.Cells[row, 3].Value = item.Award.Name;
                        wsClaimed.Cells[row, 4].Value = item.Award.Points * item.Quantity;
                        wsClaimed.Cells[row, 4].Style.Numberformat.Format = "#,###,##0";
                        wsClaimed.Cells[row++, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    wsClaimed.Cells.AutoFitColumns();
                }

                foreach (var item in years)
                {
                    ExcelWorksheet wsYear = objExcel.Workbook.Worksheets.Add($"Year-{item.Year}");
                    wsYear.Name = item.Year.ToString();
                    wsYear.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    wsYear.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
                    wsYear.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    wsYear.Cells.Style.Font.Size = 9;

                    var notes = details.FindAll(x => x.Note.Date.Year == item.Year);

                    int row = 1;
                    col = 1;

                    wsYear.Cells[row, col++].Value = "Sucursal";
                    wsYear.Cells[row, col++].Value = "No. Nota";
                    wsYear.Cells[row, col].Value = "Fecha";
                    wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    if (localUser)
                    {
                        wsYear.Cells[row, col++].Value = "Status";
                        wsYear.Cells[row, col++].Value = "Status Aplicado";
                    }
                    wsYear.Cells[row, col].Value = "Monto";
                    wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    wsYear.Cells[row, col].Value = "Puntos";
                    wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    if (localUser)
                    {
                        wsYear.Cells[row, col].Value = "Puntos Acelerador";
                        wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsYear.Cells[row, col].Value = "Puntos Acelerador (Período)";
                        wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsYear.Cells[row, col].Value = "Acelerador (Períodod)";
                        wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    wsYear.Cells[1, 1, 1, col - 1].Style.Fill.BackgroundColor.SetColor(0, 208, 206, 206);
                    wsYear.Cells[1, 1, 1, col - 1].Style.Font.Size = 11;
                    wsYear.Cells[1, 1, 1, col - 1].Style.Font.Bold = true;

                    foreach (var note in notes)
                    {
                        row++;
                        col = 1;

                        wsYear.Cells[row, col++].Value = note.Note.Subsidiary;
                        wsYear.Cells[row, col++].Value = note.Note.Number;
                        wsYear.Cells[row, col].Value = note.Note.Date;
                        wsYear.Cells[row, col].Style.Numberformat.Format = "dd-MM-yyyy";
                        wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        if (localUser)
                        {
                            wsYear.Cells[row, col++].Value = note.Status.Name;
                            wsYear.Cells[row, col++].Value = note.StatusUsed.Name;
                        }
                        wsYear.Cells[row, col].Value = note.Note.Amount;
                        wsYear.Cells[row, col].Style.Numberformat.Format = "#,###,##0.00";
                        wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        wsYear.Cells[row, col].Value = localUser ? note.Points : (note.Points + note.ExtraPoints + note.ExtraPointsPeriod);
                        wsYear.Cells[row, col].Style.Numberformat.Format = "#,###,##0";
                        wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        if (localUser)
                        {
                            wsYear.Cells[row, col].Value = note.ExtraPoints;
                            wsYear.Cells[row, col].Style.Numberformat.Format = "#,###,##0";
                            wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            wsYear.Cells[row, col].Value = note.ExtraPointsPeriod;
                            wsYear.Cells[row, col].Style.Numberformat.Format = "#,###,##0";
                            wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            wsYear.Cells[row, col].Value = note.AcceleratorPeriod;
                            wsYear.Cells[row, col].Style.Numberformat.Format = "#,###,##0.00";
                            wsYear.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                    }

                    wsYear.Cells.AutoFitColumns();
                }

                string strFileName = $"Resumen-Kbytes--{CardCode}-{DateTime.Now:yyyy-MM-dd-HHmm}.xlsx";
                byte[] objData = objExcel.GetAsByteArray();
                objExcel.Dispose();
                return File(objData, "application/xlsx", strFileName);
            }
        }

        public IActionResult GetNote(int Number, string Subsidiary)
        {
            string message = "";
            try
            {
                BCA.Note bcNote = new BCA.Note();
                BEA.Note n = bcNote.Search(Number, Subsidiary, BEA.relNote.NoteItems, BEA.relNote.Order);
                if (n != null)
                {
                    string productCodes = string.Join(",", n.Items.Select(x => $"'{x.ItemCode}'"));
                    BCP.Product bcProduct = new();
                    IEnumerable<BEP.Product> products = bcProduct.List(new List<Field> { new Field("ItemCode", productCodes, Operators.In) }, "1");
                    string productIds = string.Join(",", products.Select(x => x.Id));

                    BCK.AcceleratorLot bcLot = new();
                    List<BEK.AcceleratorLot> lots = bcLot.List(productIds, n.DocDate, BEK.relAcceleratorLot.Product) ?? new List<BEK.AcceleratorLot>();

                    var item = new
                    {
                        clientCode = n.ClientCode,
                        clientName = n.ClientName,
                        sellerName = n.SellerName,
                        clientOrder = n.Order?.ClientOrder ?? n.ClientOrder,
                        noteNumber = n.DocNumber,
                        noteDate = n.DocDate,
                        noteTotal = n.Total,
                        orderNumber = n.Order?.DocNumber,
                        orderTotal = n.Order?.Total,
                        orderDate = n.Order?.DocDate,
                        orderState = n.Order?.State,
                        terms = n.TermConditions,
                        items = (from x in n.Items
                                 join p in products on x.ItemCode.ToLower() equals p.ItemCode.ToLower() into ljProduct
                                 from lp in ljProduct.DefaultIfEmpty()
                                 join l in lots on x.ItemCode.ToLower() equals l.Product.ItemCode.ToLower() into ljLots
                                 from lj in ljLots.DefaultIfEmpty()
                                 select new { id = lp?.Id ?? 0, code = x.ItemCode, name = x.ItemName, price = x.Price, quantity = x.Quantity, total = x.ItemTotal, availableQuantity = lj?.Quantity ?? 0, accelerator = lj?.Accelerator ?? 0, idLot = lj?.Id ?? 0 }).ToList()
                    };
                    return Json(new { message, valid = true, item });
                }
                else
                {
                    return Json(new { message, valid = false });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetStatusList()
        {
            IEnumerable<BEK.Status> statusList = new List<BEK.Status>();
            try
            {
                BCK.Status bcStatus = new();
                statusList = bcStatus.List("1");
                return Json(statusList.Select(x => new { x.Id, Name = $"{x.Name} ( x{x.Points} )", x.Points }));
            }
            catch (Exception) { }
            return Json(statusList);
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult SaveNote(Models.ClientNote Item)
        {
            string message = "";
            try
            {
                DateTime logDate = DateTime.Now;
                var note = Item.ToEntity(UserCode, logDate, StatusType.Insert);

                BCK.Status bcStatus = new();
                var statusList = bcStatus.List("MinAmount");
                var statusUsed = statusList.First(x => x.Id == Item.IdStatus);

                BCK.ClientStatus bcClientStatus = new();
                var clientStatus = bcClientStatus.Search(Item.CardCode, Item.NoteDate.Year, getQuarter(Item.NoteDate));

                decimal amount = Item.Items.Sum(x => x.Enabled ? x.Total : 0);
                decimal points = amount * statusUsed.Points, extraPoints = Item.Items.Sum(x => x.ExtraPoints), extraPointsPeriod = amount * (Item.AcceleratorPeriod > 1 ? Item.AcceleratorPeriod - 1 : 0);

                BEK.Status newStatus = statusList.First();
                newStatus = statusList.LastOrDefault(x => x.MinAmount < (clientStatus.Amount + amount));

                BEK.StatusDetail status = new()
                {
                    StatusType = StatusType.Insert,
                    IdStatus = newStatus.Id,
                    IdStatusUsed = Item.IdStatus,
                    Points = points,
                    ExtraPoints = extraPoints,
                    AcceleratorPeriod = Item.AcceleratorPeriod > 1 ? Item.AcceleratorPeriod : 0,
                    ExtraPointsPeriod = extraPointsPeriod,
                    TotalPoints = clientStatus.Points + points + extraPoints + extraPointsPeriod,
                    Amount = amount,
                    TotalAmount = clientStatus.Amount + amount,
                    LogUser = UserCode,
                    LogDate = logDate
                };
                note.ListStatusDetails.Add(status);
                BCK.ClientNote bcNote = new BCK.ClientNote();

                clientStatus.Points = status.TotalPoints;
                clientStatus.Amount = status.TotalAmount;
                clientStatus.IdStatus = newStatus.Id;
                clientStatus.Status = newStatus;
                clientStatus.LogUser = UserCode;
                clientStatus.LogDate = logDate;
                clientStatus.StatusType = StatusType.Update;

                BCK.AcceleratorLot bcLot = new BCK.AcceleratorLot();
                foreach (var item in Item.Items)
                {
                    if (item.IdLot > 0)
                    {
                        bcLot.UpdateQuantity(item.IdLot, item.RemainQuantity);
                    }
                }
                bcNote.Save(ref note);
                bcClientStatus.Save(ref clientStatus);

                BCK.StatusDetail bcDetail = new BCK.StatusDetail();
                var details = bcDetail.List(clientStatus.CardCode, clientStatus.Year, "Id", BEK.relStatusDetail.Note, BEK.relStatusDetail.Status, BEK.relStatusDetail.StatusUsed, BEK.relClientNote.ClientNoteDetails, BEK.relClientNoteDetail.Product, BEK.relStatusDetail.Award);

                var items = details.Select(x => new
                {
                    x.Note?.Id,
                    x.Note?.Subsidiary,
                    x.Note?.Number,
                    Date = x.Note?.Date ?? x.Award.ClaimDate,
                    x.Note?.Amount,
                    Status = x.Status.Name,
                    StatusUsed = x.StatusUsed.Name,
                    x.Points,
                    x.ExtraPoints,
                    x.ExtraPointsPeriod,
                    x.AcceleratorPeriod,
                    ItemPoints = x.Points + x.ExtraPoints + x.ExtraPointsPeriod,
                    x.TotalPoints,
                    x.TotalAmount
                });

                return Json(new { message, status = clientStatus, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private (BEA.Client, List<BEK.ClientStatus>, long, long) GetItems(string CardCode)
        {
            BEA.Client client;
            IEnumerable<BEK.ClientStatus> years;
            long availablePoints = 0, pointsClaimed = 0, points;

            BCA.Client bcClient = new();
            client = bcClient.Search(CardCode) ?? new BEA.Client();

            BCK.ClientStatus bcStatus = new();
            List<Field> filters = new() { new Field("CardCode", CardCode) };
            years = bcStatus.List(filters, "Year DESC", BEK.relClientStatus.Status);

            BCK.ClaimedAward bcAwards = new();
            IEnumerable<BEK.ClaimedAward> awards = bcAwards.List(filters, "1", BEK.relClaimedAward.Award);
            pointsClaimed = awards?.Count() > 0 ? awards.Sum(x => x.Points) : 0;

            var calculated = bcStatus.ListCalculatedByYear(CardCode);
            //foreach (var item in calculated)
            //{
            //    var year = years.FirstOrDefault(x => x.Year == item.Year);
            //    if (year != null)
            //    {
            //        year.Amount = item.Amount;
            //        year.Points = Math.Round(item.Points);
            //    }
            //}
            foreach (var item in years)
            {
                var year = calculated.FirstOrDefault(x => x.Year == item.Year);
                if (year == null)
                {
                    calculated.Add(new BEK.ClientStatus { Year = item.Year, Amount = item.Amount, Points = item.Points, Status = item.Status });
                }
                else
                {
                    year.Status = item.Status;
                }
            }

            points = (long)calculated?.Sum(x => (long)x.Points);
            availablePoints = points - pointsClaimed;
            calculated = (from x in calculated orderby x.Year descending select x).ToList();
            return (client, calculated, pointsClaimed, availablePoints);
        }

        private int getQuarter(DateTime Date) => (int)Math.Floor((decimal)((Date.Month - 1) / 3)) + 1;

        #endregion
    }
}