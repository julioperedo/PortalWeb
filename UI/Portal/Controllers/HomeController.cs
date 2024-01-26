using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Models;
using BEE = BEntities.Enums;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCK = BComponents.Kbytes;
using BCL = BComponents.Sales;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEL = BEntities.Sales;
using BES = BEntities.Security;
using BEntities.Filters;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Portal.Areas.Commercial.Models;
using System.IO;
using DocumentFormat.OpenXml.ExtendedProperties;
//using DocumentFormat.OpenXml.Spreadsheet;

namespace Portal.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        #region Constructores

        public HomeController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (CardCode == HomeCardCode)
            {
                BCS.ProfileChart bcChart = new();
                List<BES.ProfileChart> charts = bcChart.ListByProfile(ProfileCode, "1");
                ViewBag.Charts = Newtonsoft.Json.JsonConvert.SerializeObject(from c in charts where c.Id > 0 select new { c.ChartGroup, c.IdChart }); //string.Join(",", charts.Select(x => x.IdChart));
                ViewBag.SeeAllSalesDetail = GetPermission("SeeAllSalesDetail") > 0 ? "Y" : "N";
                string viewName = "Index";
                if (ProfileCode == (long)BEE.Types.Profile.Administrator)
                {
                    //viewName = "IndexAdm";
                }
                else if (ProfileCode == (long)BEE.Types.Profile.Management)
                {
                    //viewName = "IndexManagement";
                }
                else if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
                {
                    //viewName = "IndexProductManagement";
                }
                else if (ProfileCode == (long)BEE.Types.Profile.FinanceManagement)
                {
                    //viewName = "IndexFinancemanagement";
                }
                else if (ProfileCode == (long)BEE.Types.Profile.Sales)
                {
                    //viewName = "IndexSales";
                }
                return View(viewName);
            }
            else
            {
                return View("IndexClient");
            }

        }

        public IActionResult GetClientResume()
        {
            string message = "";
            try
            {
                int year = DateTime.Today.Year;
                BCK.ClientStatus bcStatus = new();
                List<Field> filters = new() { new Field("CardCode", CardCode), new Field("Year", year - 1, Operators.HigherOrEqualThan), new Field(LogicalOperators.And) };
                IEnumerable<BEK.ClientStatus> clientStatusList = bcStatus.List(filters, "Year");
                var status = clientStatusList.FirstOrDefault(x => x.Year == year) ?? new BEK.ClientStatus { Year = year, CardCode = CardCode };
                BCA.Client bcClient = new();
                var client = bcClient.Search(CardCode);
                var item = new { status.Amount, status.Points, Name = client.CardName, LegalName = client.CardFName, client.NIT };
                return Json(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LastEvents()
        {
            BCB.Event bcEvent = new();
            List<BEB.Event> lstEvents = bcEvent.ListLast();
            var items = (from e in lstEvents
                         orderby e.Date descending
                         select new { e.Id, e.Name, e.Description, e.Date, Detail = !string.IsNullOrWhiteSpace(e.Detail) }).ToList();
            return Json(items);
        }

        public IActionResult SalesResume(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                var lines = (from i in lstConfigItems where i.Type == "M" select i.Name).ToList();
                var sellers = (from i in lstConfigItems where i.Type == "E" select i.Name).ToList();
                BCA.Resume bcResume = new();
                var stockList = bcResume.ResumeStock(lines);
                var stock = stockList.Select(x => new { Subsidiary = ToTitle(x.Subsidiary), Warehouse = ToTitle(x.Warehouse), x.Division, x.Total });
                var salesList = bcResume.ResumeSaleByPeriod(InitialDate.Value, FinalDate.Value, lines, sellers);
                var sales = salesList.Select(x => new { Subsidiary = ToTitle(x.Subsidiary), Warehouse = ToTitle(x.Warehouse), x.Division, x.TaxlessTotal, x.Total, x.Margin });
                var salesTodayList = bcResume.ResumeSaleByPeriod(DateTime.Today, DateTime.Today, lines, sellers);
                var salesPeriod = bcResume.ResumeSaleByPeriod2(InitialDate.Value, FinalDate.Value, lines, sellers);
                var salesToday = salesTodayList.Select(x => new { Subsidiary = ToTitle(x.Subsidiary), Warehouse = ToTitle(x.Warehouse), x.Division, x.TaxlessTotal, x.Total, x.Margin });
                var authorizedList = bcResume.AuthorizedOrders(lines, sellers);
                var authorized = authorizedList.Select(x => new { Subsidiary = ToTitle(x.Subsidiary), Warehouse = ToTitle(x.Warehouse), x.Division, x.Total });
                var opensList = bcResume.OpenAmounts(lines, sellers);
                var opens = opensList.Select(x => new { Subsidiary = ToTitle(x.Subsidiary), Warehouse = ToTitle(x.Warehouse), x.Division, x.Total });
                BCA.Warehouse bcWarehouse = new();
                var warehousesList = bcWarehouse.List();
                var warehouses = from x in warehousesList orderby x.Parent, x.Name select new { Subsidiary = ToTitle(x.Parent), Warehouse = ToTitle(x.Name) };
                List<string> divisions = new() { "Consumer", "Enterprise", "Mobile" };
                BCB.Classifier bcClassifier = new();
                List<BEB.Classifier> subsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "Value");
                List<BEL.Projection> tempProjection = new();
                if (InitialDate.HasValue & FinalDate.HasValue)
                {
                    BCL.Projection bcProyection = new();
                    tempProjection = bcProyection.List(InitialDate.Value.Year, InitialDate.Value.Month, FinalDate.Value.Year, FinalDate.Value.Month, "1");
                }
                List<SalesProjection> tempProjection2 = new();
                foreach (var s in subsidiaries)
                {
                    foreach (var d in divisions)
                    {
                        decimal amount = (from x in tempProjection where x.Subsidiary.ToLower() == s.Name.ToLower() & x.Division.ToLower() == d.ToLower() select x.Amount).Sum();
                        tempProjection2.Add(new SalesProjection(0, ToTitle(s.Name), d, 0, 0, amount));
                    }
                }
                var projection = tempProjection2.Select(x => new { x.Subsidiary, Division = x.Division.Substring(0, 1), x.Amount });
                var periods = from x in tempProjection
                              orderby x.Year, x.Month
                              group x by new { x.Year, x.Month } into g
                              select new { g.Key.Year, g.Key.Month, g };

                var labels = periods.Select(x => $"{x.Year}-{x.Month:D2}");
                List<ProjectionSerie> series = new();

                foreach (var s in subsidiaries)
                {
                    foreach (var d in divisions)
                    {
                        var valuesProjection = (from x in tempProjection
                                                orderby x.Year, x.Month
                                                where x.Subsidiary.ToLower() == s.Name.ToLower() & x.Division.ToLower() == d.ToLower()
                                                select x.Amount).ToList();

                        List<decimal> valuesBilled = new();
                        string division = d.ToLower() == "consumer" ? "" : d.Substring(0, 1);
                        foreach (var p in periods)
                        {
                            var period = salesPeriod.FirstOrDefault(x => x.Division.ToLower() == division.ToLower() & x.Subsidiary.ToLower() == s.Name.ToLower() & x.Year == p.Year & x.Month == p.Month);
                            valuesBilled.Add(period?.Total ?? 0);
                        }

                        series.Add(new ProjectionSerie { Name = $"{ToTitle(s.Name)} Proyectado", Division = d.Substring(0, 1), Values = valuesProjection });
                        series.Add(new ProjectionSerie { Name = $"{ToTitle(s.Name)} Facturado", Division = d.Substring(0, 1), Values = valuesBilled });
                    }
                    var valuesP = (from x in tempProjection
                                   orderby x.Year, x.Month
                                   where x.Subsidiary.ToLower() == s.Name.ToLower()
                                   group x by new { x.Year, x.Month } into g
                                   select g.Sum(i => i.Amount)).ToList();
                    List<decimal> valuesB = new();
                    foreach (var p in periods)
                    {
                        var period = (from x in salesPeriod
                                      where x.Subsidiary.ToLower() == s.Name.ToLower() & x.Year == p.Year & x.Month == p.Month
                                      select x.Total).Sum();
                        valuesB.Add(period);
                    }
                    series.Add(new ProjectionSerie { Name = $"{ToTitle(s.Name)} Proyectado", Division = "G", Values = valuesP });
                    series.Add(new ProjectionSerie { Name = $"{ToTitle(s.Name)} Facturado", Division = "G", Values = valuesB });
                }

                var deliveredNotBilled = bcResume.ResumeOpenDeliveryNotes(lines, sellers);
                var notBilled = deliveredNotBilled?.Select(x => new { x.Subsidiary, Warehouse = ToTitle(x.Warehouse), x.Division, x.Total });

                return Json(new { message, stock, sales, salesToday, warehouses, authorized, opens, projection, notBilled, projectionChart = new { labels, series } });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult TopClients(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                var lines = (from i in lstConfigItems where i.Type == "M" select i.Name).ToList();
                var sellers = (from i in lstConfigItems where i.Type == "E" select i.Name).ToList();
                BCA.Resume bcResume = new();
                var lstClients = bcResume.ResumeClientByPeriod(InitialDate.Value, FinalDate.Value, lines, sellers);
                var clients = lstClients.Select(x => new { Code = x.CardCode, Name = x.CardName, MarginTotal = x.Margin, x.Total, x.TaxlessTotal, x.Subsidiary, x.Division });
                return Json(new { message, clients });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult OpenOrders(string Subsidiary, string Warehouse, string Division, bool Authorized)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                string lines = string.Join(",", (from i in lstConfigItems where i.Type == "M" select $"'{i.Name.ToLower()}'").ToArray());
                string sellers = string.Join(",", (from i in lstConfigItems where i.Type == "E" select $"'{i.Name.ToLower()}'").ToArray());

                BCA.Order bcOrder = new();
                List<Field> filters = new() { new Field("Subsidiary", Subsidiary), new Field("Warehouse", Warehouse), new Field("OpenAmount", 0, Operators.HigherThan) }, innerFilters = new();
                if (Authorized)
                {
                    filters.AddRange(new[] { new Field("State", "Abierto"), new Field("Authorized", "Y"), new Field("Correlative", "NO PROCESAR", Operators.NotLikes) });
                }
                if (Division == "Consumer")
                {
                    filters.Add(new Field("LOWER(IFNULL(SellerCode, ''))", sellers, Operators.NotIn));
                    innerFilters.Add(new Field("LOWER(IFNULL(U_LINEA, ''))", lines, Operators.NotIn));
                }
                else if (Division == "Enterprise")
                {
                    filters.Add(new Field("LOWER(IFNULL(SellerCode, ''))", sellers, Operators.In));
                }
                else if (Division == "Mobile")
                {
                    innerFilters.Add(new Field("LOWER(IFNULL(U_LINEA, ''))", lines, Operators.In));
                }
                if (GetPermission("SeeAllClients") == 0)
                {
                    BCS.UserData bcData = new();
                    var data = bcData.SearchByUser(UserCode);
                    filters.Add(new Field("LOWER(IFNULL(SellerCode, ''))", data?.SellerCode?.ToLower() ?? "XXX"));
                }
                CompleteFilters(ref filters);
                var orders = bcOrder.List4(filters, innerFilters, "1");

                BCA.OrderFile bcFile = new();
                var codes = orders.Select(x => $"{Subsidiary}-{x.Id}");
                var files = bcFile.List(codes);
                var resumeFiles = from f in files
                                  group f by f.DocEntry into g
                                  select new { DocEntry = g.Key, Count = g.Count(), Files = from d in g select $"{d.FileName}.{d.FileExt}" };

                var items = from o in orders
                            join f in resumeFiles on o.Id equals f.DocEntry into ljFiles
                            from lf in ljFiles.DefaultIfEmpty()
                            select new
                            {
                                o.Id,
                                o.Subsidiary,
                                o.Warehouse,
                                o.DocNumber,
                                o.DocDate,
                                o.ClientOrder,
                                o.ClientName,
                                o.SellerName,
                                o.Total,
                                o.OpenAmount,
                                Complete = o.NonCompleteItem == 0,
                                Header = o.Header?.Replace("\r", "<br />"),
                                Footer = o.Footer?.Replace("\r", "<br />"),
                                o.State,
                                HasFiles = lf != null && lf.Count > 0,
                                Files = lf?.Files ?? new List<string>()
                            };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult DetailOrder(string Subsidiary, int DocEntry, string State)
        {
            string message = "";
            try
            {
                BCA.Order bcItems = new();
                List<Field> filters = new() { new Field("Subsidiary", Subsidiary), new Field("Id", DocEntry), new Field("OpenQuantity", 0, Operators.HigherThan), new Field(LogicalOperators.And), new Field(LogicalOperators.And) };
                IEnumerable<BEA.OrderItem> items = bcItems.ListItems(filters, "1");
                List<string> lstItemCodes = new() { "FLETES", "ENVIO", "DMCSERVICIOS" };
                foreach (var item in items)
                {
                    item.MarginPercentage = (item.Margin / (item.TaxlessTotal != 0 ? item.TaxlessTotal : 1)) * 100;
                    item.Complete = State != "Abierto" || lstItemCodes.Contains(item.ItemCode) || item.OpenQuantity <= item.Stock;
                }
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult TodayBills(string Subsidiary, string Warehouse, string Division)
        {
            string message = "";
            try
            {
                BCA.Note bcNote = new();
                List<Field> filters = new() { new Field("DocDate", DateTime.Today.ToString("yyyy-MM-dd")), new Field("LOWER(Subsidiary)", Subsidiary.ToLower()) };
                if (!string.IsNullOrWhiteSpace(Warehouse))
                {
                    filters.Add(new Field("LOWER(Warehouse)", Warehouse.ToLower()));
                }
                if (ProfileCode == (long)BEE.Types.Profile.Sales && GetPermission("SeeAllClients") == 0)
                {
                    BCS.UserData bcData = new();
                    var data = bcData.SearchByUser(UserCode);
                    filters.Add(new Field("SellerCode", data?.SellerCode ?? "XXX"));
                }
                CompleteFilters(ref filters);
                var notes = bcNote.ListBySection(Division, filters, "1");
                var items = notes.Select(x => new
                {
                    x.Id,
                    x.Subsidiary,
                    x.Warehouse,
                    NoteNumber = x.DocNumber,
                    x.OrderNumber,
                    x.DocDate,
                    x.ClientName,
                    x.SellerName,
                    x.Total,
                    MarginTotal = x.Margin,
                    x.TaxlessTotal,
                    x.IsDeliveryNote,
                    Margin = x.TaxlessTotal > 0 ? (x.Margin / x.TaxlessTotal) * 100 : 0
                });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult PeriodBills(string Subsidiary, string Warehouse, string Division, DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                BCA.Note bcNote = new();
                List<Field> filters = new() { new Field("LOWER(Subsidiary)", Subsidiary.ToLower()) };
                if (!string.IsNullOrWhiteSpace(Warehouse))
                {
                    filters.AddRange(new[] { new Field("LOWER(Warehouse)", Warehouse.ToLower()), new Field(LogicalOperators.And) });
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("DocDate", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (GetPermission("SeeAllClients") == 0)
                {
                    BCS.UserData bcData = new();
                    var data = bcData.SearchByUser(UserCode);
                    filters.Add(new Field("SellerCode", data?.SellerCode ?? "XXX"));
                }
                CompleteFilters(ref filters);
                var notes = bcNote.ListBySection(Division, filters, "1");
                var items = notes.Select(x => new
                {
                    x.Id,
                    x.Subsidiary,
                    x.Warehouse,
                    NoteNumber = x.DocNumber,
                    x.OrderNumber,
                    x.DocDate,
                    x.ClientName,
                    x.SellerName,
                    x.Total,
                    MarginTotal = x.Margin,
                    x.TaxlessTotal,
                    x.IsDeliveryNote,
                    Margin = x.TaxlessTotal > 0 ? (x.Margin / x.TaxlessTotal) * 100 : 0
                });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult DetailNote(string Subsidiary, int DocNumber)
        {
            string message = "";
            try
            {
                BCA.Note bcNote = new();
                List<Field> filters = new() { new Field("Subsidiary", Subsidiary), new Field("DocNumber", DocNumber), new Field(LogicalOperators.And) };
                var noteItems = bcNote.ListItems(filters, "1");
                var items = noteItems.Select(x => new { x.Warehouse, x.ItemCode, x.ItemName, x.ItemTotal, x.Quantity, x.Price, Margin = (x.Margin / x.CalculedTotal) * 100 });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult DetailDeliveryNote(string Subsidiary, int DocNumber)
        {
            string message = "";
            try
            {
                BCA.DeliveryNote bcNote = new();
                List<Field> filters = new() { new Field("Subsidiary", Subsidiary), new Field("DocNumber", DocNumber), new Field(LogicalOperators.And) };
                var items = bcNote.ListItems(filters, "1");
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult StockDetail(string Subsidiary, string Division)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                var lines = (from i in lstConfigItems where i.Type == "M" select i.Name).ToList();

                BCA.Resume bcResume = new();
                var stock = bcResume.ResumeStock(lines);
                bool filter = Division == "General";
                var items = from i in stock
                            where i.Subsidiary.ToLower() == Subsidiary.ToLower() & (Division != "Mobile" || i.Division == "M")
                            group i by i.Warehouse into g
                            select new { Warehouse = g.Key, Total = g.Sum(x => x.Total) };

                return Json(new { message, stock, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult StockDetailByWarehouse(string Subsidiary, string Warehouse, string Division)
        {
            string message = "";
            try
            {
                BCA.Resume bcResume = new();
                var stock = bcResume.ResumeStock(Subsidiary, Warehouse, Division);
                var items = stock.Select(x => new { x.Line, x.Total });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }


        public IActionResult GetOpenDeliveryNotes(string Subsidiary, string Warehouse, string Division)
        {
            string message = "";
            try
            {
                BCB.DivisionConfig bcDivConfig = new();
                IEnumerable<BEB.DivisionConfig> lstConfigItems = bcDivConfig.List("Type, Name");
                string lines = string.Join(",", from i in lstConfigItems where i.Type == "M" select $"'{i.Name.ToLower()}'");
                string sellers = string.Join(",", from i in lstConfigItems where i.Type == "E" select $"'{i.Name.ToLower()}'");

                List<Field> filters = new() { new Field("Subsidiary", Subsidiary), new Field("Warehouse", Warehouse), new Field("OpenAmount", 0, Operators.HigherThan) }, innerFilters = new();
                if (!string.IsNullOrEmpty(Warehouse))
                {
                    filters.Add(new Field("Warehouse", Warehouse));
                }
                if (Division == "Consumer")
                {
                    filters.Add(new Field("LOWER(IFNULL(SellerCode, ''))", sellers, Operators.NotIn));
                    innerFilters.Add(new Field("LOWER(IFNULL(U_LINEA, ''))", lines, Operators.NotIn));
                }
                else if (Division == "Enterprise")
                {
                    filters.Add(new Field("LOWER(IFNULL(SellerCode, ''))", sellers, Operators.In));
                }
                else if (Division == "Mobile")
                {
                    innerFilters.Add(new Field("LOWER(IFNULL(U_LINEA, ''))", lines, Operators.In));
                }

                BCA.DeliveryNote bcDelivery = new();
                IEnumerable<BEA.DeliveryNote> openDeliveryNotes = bcDelivery.ListOpen(filters, innerFilters, "1");

                return Json(new { message, items = openDeliveryNotes });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetStockDetail(string Subsidiary, string Warehouse, string ItemCode)
        {
            string message = "";
            try
            {
                BCA.ProductStock bcInventory = new();
                List<Field> lstFilters = new()
                {
                    new Field("ItemCode", ItemCode),
                    new Field("LOWER(Subsidiary)", Subsidiary.ToLower()),
                    new Field("LOWER(Warehouse)", Warehouse.ToLower()),
                    new Field(LogicalOperators.And),
                    new Field(LogicalOperators.And)
                };
                IEnumerable<BEA.ProductStock> lstItems = bcInventory.List(lstFilters, "1");
                var items = lstItems.Select(x => new { x.Subsidiary, Warehouse = ToTitle(x.Warehouse), x.ItemCode, x.Stock, x.Reserved, x.Available, x.Available2, x.Requested });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult KbytesResume()
        {
            string message = "";
            try
            {
                BCK.ClientStatus bcStatus = new();
                List<BEK.ClientStatus> years = bcStatus.ListCalculated();

                BCK.ClaimedAward bcAward = new();
                var awards = bcAward.List("1");

                long points = 0, availablePoints = 0, claimedPoints = 0;
                points = years.Sum(x => (long)x.Points);
                claimedPoints = awards?.Count() > 0 ? awards.Sum(x => x.Points) : 0;
                availablePoints = points - claimedPoints;

                return Json(new { message, years, availablePoints, claimedPoints });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetAwardsClaimed()
        {
            string message = "";
            try
            {
                BCK.ClaimedAward bcAwards = new();
                List<Field> filters = new();
                IEnumerable<BEK.ClaimedAward> awards = bcAwards.List(filters, "ClaimDate DESC", BEK.relClaimedAward.Award);
                string cardCodes = string.Join(",", awards?.Select(x => $"'{x.CardCode}'").Distinct());
                BCA.Client bcClient = new();
                filters.Add(new Field("LOWER(CardCode)", cardCodes.ToLower(), Operators.In));
                var clients = bcClient.ListShort(filters, "1");
                var items = from a in awards
                            join c in clients on a.CardCode.ToLower() equals c.CardCode.ToLower()
                            select new { c.CardCode, c.CardName, award = a.Award.Name, a.Quantity, a.ClaimDate, a.Points };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetPointsResume(int Year)
        {
            string message = "";
            try
            {
                BCK.Status bcStatusType = new();
                BCK.ClientStatus bcStatus = new();
                var clientResumes = bcStatus.ListCalculated(Year);

                List<Field> filters = new() { new Field("Enabled", 1) };
                IEnumerable<BEK.Status> statusList = bcStatusType.List(filters, "MinAmount");

                if (clientResumes?.Count > 0)
                {
                    string cardCodes = string.Join(",", clientResumes?.Select(x => $"'{x.CardCode}'").Distinct());
                    BCA.Client bcClient = new();
                    filters = new List<Field> { new Field("LOWER(CardCode)", cardCodes.ToLower(), Operators.In) };
                    var clients = bcClient.ListShort(filters, "1");

                    if (Year < 2023)
                    {
                        var items = (from i in clientResumes
                                     group i by i.CardCode into g
                                     join c in clients on g.Key.ToLower() equals c.CardCode.ToLower()
                                     select new
                                     {
                                         c.CardCode,
                                         c.CardName,
                                         amountQ1 = g.Where(x => x.Year == Year & x.Quarter == 1).Sum(x => x.Amount),
                                         pointsQ1 = g.Where(x => x.Year == Year & x.Quarter == 1).Sum(x => x.Points),
                                         statusQ1 = (from x in statusList orderby x.MinAmount ascending where x.MinAmount <= g.Where(x => x.Year == (Year - 1) & x.Quarter == 4).Sum(x => x.Amount) select x.Name).Last(),
                                         amountQ2 = g.Where(x => x.Year == Year & x.Quarter == 2).Sum(x => x.Amount),
                                         pointsQ2 = g.Where(x => x.Year == Year & x.Quarter == 2).Sum(x => x.Points),
                                         statusQ2 = (from x in statusList orderby x.MinAmount ascending where x.MinAmount <= g.Where(x => x.Year == Year & x.Quarter == 1).Sum(x => x.Amount) select x.Name).Last(),
                                         amountQ3 = g.Where(x => x.Year == Year & x.Quarter == 3).Sum(x => x.Amount),
                                         pointsQ3 = g.Where(x => x.Year == Year & x.Quarter == 3).Sum(x => x.Points),
                                         statusQ3 = (from x in statusList orderby x.MinAmount ascending where x.MinAmount <= g.Where(x => x.Year == Year & x.Quarter == 2).Sum(x => x.Amount) select x.Name).Last(),
                                         amountQ4 = g.Where(x => x.Year == Year & x.Quarter == 4).Sum(x => x.Amount),
                                         pointsQ4 = g.Where(x => x.Year == Year & x.Quarter == 4).Sum(x => x.Points),
                                         statusQ4 = (from x in statusList orderby x.MinAmount ascending where x.MinAmount <= g.Where(x => x.Year == Year & x.Quarter == 3).Sum(x => x.Amount) select x.Name).Last(),
                                         totalAmount = g.Where(x => x.Year == Year).Sum(x => x.Amount),
                                         totalPoints = g.Where(x => x.Year == Year).Sum(x => x.Points)
                                     }).ToList();
                        return Json(new { message, items });
                    }
                    else
                    {
                        var items = from i in clientResumes
                                    group i by i.CardCode into g
                                    join c in clients on g.Key.ToLower() equals c.CardCode.ToLower()
                                    select new { c.CardCode, c.CardName, amount = g.Where(x => x.Year == Year).Sum(x => x.Amount), points = g.Where(x => x.Year == Year).Sum(x => x.Points) };
                        return Json(new { message, items });
                    }
                }
                return Json(new { message, items = clientResumes });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetAvailablePoints()
        {
            string message = "";
            try
            {
                BCK.ClientStatus bcStatus = new();
                var clientsResume = bcStatus.ListAllClientsCalculated();
                string cardCodes = string.Join(",", clientsResume.Select(x => $"'{x.CardCode.ToLower()}'").Distinct());
                BCA.Client bcClient = new();
                var clients = bcClient.ListShort(new List<Field> { new Field("LOWER(CardCode)", cardCodes, Operators.In) }, "CardName");
                BCK.ClaimedAward bcAward = new();
                var awards = bcAward.ListAllClientsCalculated();
                var years = clientsResume.Select(x => x.Year).Distinct().ToList();
                var items = (from c in clients
                             from y in years
                             join r in clientsResume on new { c = c.CardCode.ToLower(), y } equals new { c = r.CardCode.ToLower(), y = r.Year } into ljResume
                             from lr in ljResume.DefaultIfEmpty()
                             join a in awards on new { c = c.CardCode.ToLower(), y } equals new { c = a.CardCode.ToLower(), y = a.Year } into ljAward
                             from la in ljAward.DefaultIfEmpty()
                             select new { Year = y, c.CardCode, c.CardName, points = lr?.Points ?? 0, claimed = la?.Points ?? 0 }).Distinct().ToList();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ProductsResumeByLines(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                ChartData chart = new();
                BCA.Note bcNote = new();
                var items = bcNote.ResumeByLine(InitialDate, FinalDate);

                List<ChartSerie> lstSeries = new();
                ChartSerie serie = new() { Name = "Líneas", Type = "column", Data = new List<DataPair>() };
                foreach (var item in items.OrderByDescending(x => x.Total))
                {
                    serie.Data.Add(new DataPair { Label = item.Name, Value = item.Total });
                }
                lstSeries.Add(serie);

                chart.Title = "Ventas por Línea";
                chart.Labels = (from i in items orderby i.Total descending select i.Name).ToList();
                chart.Series = lstSeries;

                return Json(new { message, chart });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ProductsResumeByCategories(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                BCA.Note bcNote = new();
                var items = bcNote.ResumeByCategory(InitialDate, FinalDate);

                ChartData chart = new() { Series = new List<ChartSerie>(), DrillDown = new List<ChartSerie>() };

                var categories = (from c in items
                                  group c by c.Parent into g
                                  orderby g.Sum(p => p.Total) descending
                                  select new { Category = g.Key, Total = g.Sum(p => p.Total) }).ToList();
                var subcategories = (from c in items
                                     group c by c.Parent into g
                                     select new { Category = g.Key, Subcategories = (from d in g orderby d.Total descending select new { d.Name, d.Total }).ToList() }).ToList();

                ChartSerie beSerie = new()
                {
                    Name = "Categorias",
                    Data = (from c in categories select new DataPair { Label = c.Category, Value = c.Total, DrillDown = c.Category }).ToList()
                };
                chart.Series.Add(beSerie);

                chart.DrillDown.AddRange(subcategories.Select(x => new ChartSerie { Name = x.Category, Data = x.Subcategories.Select(z => new DataPair { Label = z.Name, Value = z.Total }).ToList() }));
                chart.Labels = categories.Select(x => x.Category).Distinct().ToList();
                chart.Title = "Ventas por Categoría";

                return Json(new { message, chart });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult TeamQuota()
        {
            string message = "";
            try
            {
                BCA.Resume bcResume = new();
                DateTime objUntil = new(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day), objSince = new(objUntil.Year, objUntil.Month, 1);
                var resumeSales = bcResume.ResumeSaleBySeller(objSince, objUntil);

                BCL.SellersQuota bcQuota = new();
                List<Field> lstFilter = new() { new Field { Name = "[Year]", Value = objSince.Year }, new Field { Name = "[Month]", Value = objSince.Month }, new Field { LogicalOperator = LogicalOperators.And } };
                IEnumerable<BEL.SellersQuota> lstQuota = bcQuota.List(lstFilter, "2");

                BCA.Seller bcSeller = new();
                var sellers = bcSeller.List(new List<Field>(), "1");

                List<ChartSerie> lstQSeries = new();
                List<string> lstLabels = new();

                ChartData chart = new();
                ChartSerie beQSerie = new() { Data = new List<DataPair>(), Name = "Ejecutivos de Cuenta" };
                foreach (var item in lstQuota)
                {
                    BEA.Seller beSeller = (from s in sellers where s.ShortName == item.SalesmanCode select s).FirstOrDefault();
                    if (beSeller != null)
                    {
                        var beSales = (from s in resumeSales where s.SellerName == beSeller.Name select s).FirstOrDefault();
                        if (beSales != null & item.Amount > 0)
                        {
                            beQSerie.Data.Add(new DataPair { Label = beSeller.Name, Value = beSales.Total, Total = item.Amount, Percentage = (beSales.Total * 100 / item.Amount), });
                        }
                        else
                        {
                            beQSerie.Data.Add(new DataPair { Label = beSeller.Name, Value = 0 });
                        }
                        lstLabels.Add(beSeller.Name);
                    }
                }
                lstQSeries.Add(beQSerie);

                chart.Title = $"Cumplimiento del Mes de {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(objSince.Month)} de {objSince.Year}";
                chart.Labels = lstLabels;
                chart.Series = lstQSeries;
                return Json(new { message, chart });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult TeamResume(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                ChartData chart = new();
                List<ChartSerie> lstSeries = new();
                if (InitialDate.HasValue & FinalDate.HasValue)
                {
                    BCA.Resume bcResume = new();
                    var salesResume = bcResume.ResumeSaleBySeller(InitialDate.Value, FinalDate.Value);

                    ChartSerie serie = new() { Name = "Vendedor", Type = "column", Data = new List<DataPair>() };
                    foreach (var item in (from r in salesResume orderby r.Total descending select r).ToList())
                    {
                        serie.Data.Add(new DataPair { Label = item.SellerName, Value = item.Total });
                    }
                    lstSeries.Add(serie);
                    chart.Labels = (from i in salesResume orderby i.Total descending select i.SellerName).ToList();
                }
                chart.Title = "Ventas por Vendedor";
                chart.Series = lstSeries;
                return Json(new { message, chart });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult TeamResumeInTime(DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                ChartData chart = new();
                List<ChartSerie> lstTSeries = new List<ChartSerie>();
                if (InitialDate.HasValue & FinalDate.HasValue)
                {
                    BCA.Resume bcResume = new BCA.Resume();
                    var salesResume = bcResume.ResumeSaleBySellerByMonth(InitialDate.Value, FinalDate.Value);

                    List<string> categories = (from r in salesResume orderby r.Year, r.Month select r.Month + "-" + r.Year).Distinct().ToList();
                    List<string> teamMembers = (from r in salesResume orderby r.SellerName select r.SellerName).Distinct().ToList();

                    foreach (var member in teamMembers)
                    {
                        ChartSerie beSerie = new ChartSerie { Name = member, Type = "line", Data = new List<DataPair>() };
                        foreach (var cat in categories)
                        {
                            int intYear = int.Parse(cat.Split('-')[1]);
                            int intMonth = int.Parse(cat.Split('-')[0]);
                            beSerie.Data.Add(new DataPair
                            {
                                Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(intMonth) + "-" + intYear,
                                Value = (from r in salesResume where r.Month == intMonth & r.Year == intYear & r.SellerName == member select r.Total).Sum()
                            });
                        }
                        lstTSeries.Add(beSerie);
                    }
                    chart.Labels = (from r in salesResume orderby r.Year, r.Month select CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(r.Month) + "-" + r.Year).Distinct().ToList();
                }
                chart.Title = "Comparativa de Ventas a lo largo del tiempo";
                chart.Series = lstTSeries;
                return Json(new { message, chart });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult ClientOpenOrders()
        {
            string message = "";
            try
            {
                BCA.Order bcOrder = new();
                List<Field> filters = new() { new Field("LOWER(ClientCode)", CardCode.ToLower()), new Field("State", "Abierto"), new Field(LogicalOperators.And) };
                var orders = bcOrder.List2(filters, "DocDate DESC", BEA.relOrder.Notes, BEA.relNote.NoteItems);
                if (orders?.Count() > 0)
                {
                    string strCodes = string.Join(",", orders.Select(x => $"'{x.DocNumber}'").ToArray());
                    decimal? decNull = null;

                    BCL.TransportDetail bcTransport = new();
                    filters = new List<Field> { new Field("Code", strCodes, Operators.In) };
                    var transportNotes = bcTransport.List(filters, "1");

                    var items = from o in orders
                                join t in transportNotes on new { DocNumber = o.DocNumber.ToString(), Subsidiary = o.Subsidiary.ToLower() } equals new { DocNumber = t.Code, Subsidiary = "santa cruz" } into ljTransport
                                from lt in ljTransport.DefaultIfEmpty()
                                select new
                                {
                                    o.Id,
                                    o.Subsidiary,
                                    o.Warehouse,
                                    o.DocNumber,
                                    o.DocDate,
                                    o.ClientOrder,
                                    SellerCode = o.Notes?.FirstOrDefault()?.SellerCode ?? o.SellerCode,
                                    SellerName = o.Notes?.FirstOrDefault()?.SellerName ?? o.SellerName,
                                    o.State,
                                    o.IsDeliveryNote,
                                    o.NonCompleteItem,
                                    o.Total,
                                    OpenAmount = o.Total - (o.Notes?.Sum(x => x.Total) ?? 0),
                                    BillsNumbers = string.Join(",", o.Notes?.Select(x => x.BillNumber) ?? Array.Empty<string>()),
                                    NoteNumbers = o.Notes?.Select(x => new BEA.NoteNumber { Number = x.DocNumber, Delivery = x.IsDeliveryNote }),
                                    BillDates = string.Join("<br />", o.Notes?.Select(x => x.DocDate.ToString("dd-MM-yyyy")) ?? Array.Empty<string>()),
                                    TotalBilled = o.Notes?.Sum(x => x.Total) > 0 ? o.Notes?.Sum(x => x.Total) : decNull,
                                    HasTransport = lt != null
                                };
                    return Json(new { message, items });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public new IActionResult NotFound()
        {
            return View();
        }

        public IActionResult DownloadFile(string Subsidiary, int DocEntry, string FileName)
        {
            try
            {
                BCA.OrderFile bcFile = new();
                BEA.OrderFile beFile = bcFile.Search(Subsidiary, DocEntry, FileName);
                string fullName = $@"{beFile.Path}\{beFile.FileName}.{beFile.FileExt}";
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                string ext = FileName.Split('.').Last();
                string contentType = ext switch
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
                return File(file, contentType, FileName);
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

        #region Private Methods

        #endregion
    }
}
