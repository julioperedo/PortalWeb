using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCL = BComponents.Sales;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class QuoteOrderController : BaseController
    {
        #region Constructores

        public QuoteOrderController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs
        public IActionResult Index()
        {
            if (IsAllowed(this) & CardCode == HomeCardCode)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetSignature()
        {
            BCS.User bcUser = new BCS.User();
            BES.User beUser = bcUser.Search(UserCode, BES.relUser.UserDatas);
            string strSignature = SetHTMLSafe(beUser.ListUserDatas?.FirstOrDefault()?.Signature ?? $"<p>Atentamente</p><br /><p>{beUser.Name}</p>");
            return Content(strSignature);
        }

        public IActionResult GetLines2()
        {
            BCP.Line bcLine = new();
            IEnumerable<BEP.Line> lstLines = bcLine.List("Name");
            var lstResult = (from l in lstLines select new { l.Id, l.Name }).ToList();
            lstResult.Add(new { Id = (long)-11, Name = "OFERTAS SANTA CRUZ" });
            lstResult.Add(new { Id = (long)-12, Name = "OFERTAS MIAMI" });
            lstResult.Add(new { Id = (long)-13, Name = "OFERTAS IQUIQUE" });
            return Json(lstResult);
        }

        public IActionResult Products(long? IdLine, string Category, string Name, string Available)
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                IEnumerable<BEA.ProductStock> lstInventory = new List<BEA.ProductStock>();
                IEnumerable<BEP.PriceOffer> offers = Enumerable.Empty<BEP.PriceOffer>();
                IEnumerable<BEP.Product> lstProducts = Enumerable.Empty<BEP.Product>();
                string strCategory = "";
                List<string> categories = new();
                if (!string.IsNullOrWhiteSpace(Category))
                {
                    strCategory = string.Join(",", (from c in Category.Split(',') select $"'{c}'").ToArray());
                    categories = Category.Split(',').ToList();
                }
                if (IdLine.HasValue && IdLine.Value < 0)
                {
                    List<Field> lstFilter = new() { new Field { Name = "Enabled", Value = true } };
                    if (!string.IsNullOrWhiteSpace(Category))
                    {
                        lstFilter.Add(new Field { Name = "ISNULL(SubCategory, '')", Value = strCategory, Operator = Operators.In });
                        lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                    }
                    if (!string.IsNullOrEmpty(Name))
                    {
                        lstFilter.Add(new Field { Name = "Name", Value = Name.Trim(), Operator = Operators.Likes });
                        lstFilter.Add(new Field { Name = "Description", Value = Name.Trim(), Operator = Operators.Likes });
                        lstFilter.Add(new Field { Name = "ItemCode", Value = Name.Trim(), Operator = Operators.Likes });
                        lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                        lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                        lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                    }
                    lstProducts = IdLine switch
                    {
                        -11 => bcProduct.ListWithOfferSA(lstFilter, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers),
                        -12 => bcProduct.ListWithOfferLA(lstFilter, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers),
                        -13 => bcProduct.ListWithOfferIQ(lstFilter, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers),
                        _ => bcProduct.ListWithOffer(lstFilter, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers),
                    };
                }
                else
                {
                    lstProducts = bcProduct.ListWithPrices2(IdLine, Name, categories, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers);
                }

                if (lstProducts.Any())
                {
                    List<Field> lstFilter = new() { new Field { Name = "ItemCode", Value = string.Join(",", (from p in lstProducts select "'" + p.ItemCode + "'").Distinct()), Operator = Operators.In } };
                    if (!string.IsNullOrEmpty(Available))
                    {
                        string strAvailable = string.Join(",", (from s in Available.Split(',') select "'" + s.ToLower() + "'").ToArray());
                        lstFilter.Add(new Field { Name = "LOWER(Subsidiary)", Value = strAvailable, Operator = Operators.In });
                        lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                    }
                    lstFilter.Add(new Field { Name = "Stock", Value = 0, Operator = Operators.HigherThan });
                    lstFilter.Add(new Field { Name = "Available", Value = 0, Operator = Operators.HigherThan });
                    lstFilter.Add(new Field { Name = "Available2", Value = 0, Operator = Operators.HigherThan });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                    BCA.ProductStock bcInventory = new();
                    lstInventory = bcInventory.List(lstFilter, "1");

                    BCP.WarehouseAllowed bcWarehouse = new();
                    IEnumerable<BEP.WarehouseAllowed> lstWarehouses = bcWarehouse.List("1");
                    lstInventory = from i in lstInventory where (from w in lstWarehouses select w.Name.ToLower()).Contains(i.Warehouse.Trim().ToLower()) select i;

                    BCP.PriceOffer bcOffer = new();
                    string productIds = string.Join(",", lstProducts.Select(x => x.Id)), today = DateTime.Today.ToString("yyyy-MM-dd");
                    lstFilter = new List<Field>
                    {
                        new Field("IdProduct", productIds, Operators.In), new Field("Enabled", 1), new Field($"ISNULL(Since,'{today}')", today, Operators.LowerOrEqualThan),
                        new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan)
                    };
                    CompleteFilters(ref lstFilter);
                    offers = bcOffer.List(lstFilter, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();
                }

                var items = from p in lstProducts
                            orderby p.Name
                            select new
                            {
                                p.Id,
                                p.Name,
                                p.ItemCode,
                                Prices = from d in p.ListPrices
                                         where d.Regular > 0 | offers.Any(x => x.IdProduct == p.Id & x.IdSubsidiary == d.IdSudsidiary & x.Price > 0)
                                         select new { Subsidiary = ToTitle(d.Sudsidiary.Name), Regular = offers.Any(x => x.IdProduct == p.Id & x.IdSubsidiary == d.IdSudsidiary & x.Price > 0) ? offers.First(x => x.IdProduct == p.Id & x.IdSubsidiary == d.IdSudsidiary & x.Price > 0).Price : d.Regular },
                                Stock = from i in lstInventory
                                        where i.ItemCode.ToLower() == p.ItemCode.ToLower()
                                        select new { i.Subsidiary, Warehouse = ToTitle(i.Warehouse), Available = i.Subsidiary.ToLower() == "iquique" ? i.Stock : i.Available2, InTransit = i.Requested }
                            };
                if (!string.IsNullOrEmpty(Available))
                {
                    items = from i in items where ((from iv in lstInventory select iv.ItemCode).Contains(i.ItemCode) | i.ItemCode == "TODOS") & (from d in i.Stock where d.Available > 0 select d).Any() select i;
                }
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Products2(string ItemCodes)
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                IEnumerable<BEA.ProductStock> lstInventory = Enumerable.Empty<BEA.ProductStock>();
                IEnumerable<BEP.PriceOffer> offers = Enumerable.Empty<BEP.PriceOffer>();
                IEnumerable<BEP.Product> lstProducts = Enumerable.Empty<BEP.Product>();
                if (!string.IsNullOrEmpty(ItemCodes))
                {
                    lstProducts = bcProduct.ListWithPrices3(ItemCodes.Trim(), "Name", BEP.relProduct.Prices, BEP.relProduct.PriceOffers, BEP.relPrice.Sudsidiary);
                }

                if (lstProducts.Any())
                {
                    List<Field> lstFilter = new()
                    {
                        new Field { Name = "ItemCode", Value = ItemCodes, Operator = Operators.In },
                        new Field { Name = "Stock", Value = 0, Operator = Operators.HigherThan },
                        new Field { Name = "Available", Value = 0, Operator = Operators.HigherThan },
                        new Field { Name = "Available2", Value = 0, Operator = Operators.HigherThan },
                        new Field { LogicalOperator = LogicalOperators.Or },
                        new Field { LogicalOperator = LogicalOperators.Or },
                        new Field { LogicalOperator = LogicalOperators.And }
                    };
                    BCA.ProductStock bcInventory = new();
                    lstInventory = bcInventory.List(lstFilter, "1");

                    BCP.WarehouseAllowed bcWarehouse = new();
                    IEnumerable<BEP.WarehouseAllowed> lstWarehouses = bcWarehouse.List("1");
                    lstInventory = from i in lstInventory where (from w in lstWarehouses select w.Name.ToLower()).Contains(i.Warehouse.Trim().ToLower()) select i;

                    BCP.PriceOffer bcOffer = new();
                    string productIds = string.Join(",", lstProducts.Select(x => x.Id)), today = DateTime.Today.ToString("yyyy-MM-dd");
                    lstFilter = new List<Field>
                    {
                        new Field("IdProduct", productIds, Operators.In), new Field("Enabled", 1), new Field($"ISNULL(Since,'{today}')", today, Operators.LowerOrEqualThan),
                        new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan)
                    };
                    CompleteFilters(ref lstFilter);
                    offers = bcOffer.List(lstFilter, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();
                }

                var items = from p in lstProducts
                            orderby p.Name
                            select new
                            {
                                p.Id,
                                p.Name,
                                p.ItemCode,
                                Prices = from d in p.ListPrices
                                         where d.Regular > 0 | offers.Any(x => x.IdProduct == p.Id & x.IdSubsidiary == d.IdSudsidiary & x.Price > 0)
                                         select new { Subsidiary = ToTitle(d.Sudsidiary.Name), Regular = offers.Any(x => x.IdProduct == p.Id & x.IdSubsidiary == d.IdSudsidiary & x.Price > 0) ? offers.First(x => x.IdProduct == p.Id & x.IdSubsidiary == d.IdSudsidiary & x.Price > 0).Price : d.Regular },
                                Stock = from i in lstInventory
                                        where i.ItemCode.ToLower() == p.ItemCode.ToLower()
                                        select new { i.Subsidiary, Warehouse = ToTitle(i.Warehouse), Available = i.Subsidiary.ToLower() == "iquique" ? i.Stock : i.Available2, InTransit = i.Requested }
                            };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }

            return Json(new { message });
        }

        public IActionResult Product(string ItemCode)
        {
            if (!string.IsNullOrWhiteSpace(ItemCode))
            {
                Models.QuoteDetail product = GetProduct(ItemCode);

                //Por ahora aca, pero debería ser una solución global
                var cul = CultureInfo.CreateSpecificCulture("es-BO");
                cul.NumberFormat.NumberDecimalSeparator = ".";
                cul.NumberFormat.NumberGroupSeparator = ",";
                System.Threading.Thread.CurrentThread.CurrentCulture = cul;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cul;

                return PartialView("Product", product);
            }
            else
            {
                return Content("");
            }
        }

        public IActionResult Client(string CardCode)
        {
            BCA.Client bcClient = new();
            BEA.Client beClient = bcClient.Search(CardCode);
            return Json(new { Name = beClient.CardName, beClient.EMail });
        }

        public IActionResult Filter(DateTime? Since, DateTime? Until, string Client)
        {
            string message = "";
            IEnumerable<BEL.Quote> items = new List<BEL.Quote>();
            try
            {
                long intUserId = UserCode;
                BCL.Quote bcQuote = new();
                List<Field> lstFilter = new() { new Field { Name = "IdSeller", Value = intUserId } };
                if (!string.IsNullOrEmpty(Client))
                {
                    lstFilter.Add(new Field { Name = "CardCode", Value = Client.Trim(), Operator = Operators.Likes });
                    lstFilter.Add(new Field { Name = "CardName", Value = Client.Trim(), Operator = Operators.Likes });
                    lstFilter.Add(new Field { Name = "ClientName", Value = Client.Trim(), Operator = Operators.Likes });
                    lstFilter.Add(new Field { Name = "ClientMail", Value = Client.Trim(), Operator = Operators.Likes });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                }
                if (Since.HasValue)
                {
                    lstFilter.Add(new Field { Name = "QuoteDate", Value = Since.Value.ToString("yyyy-MM-dd"), Operator = Operators.HigherOrEqualThan });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                }
                if (Until.HasValue)
                {
                    lstFilter.Add(new Field { Name = "QuoteDate", Value = Until.Value.ToString("yyyy-MM-dd"), Operator = Operators.LowerOrEqualThan });
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                }
                items = bcQuote.List(lstFilter, "QuoteDate DESC, CardName");
            }
            catch (Exception ex)
            {
                message = "Se ha producido el siguiente error:<br />" + GetError(ex);
            }
            return Json(new { message, items });
        }

        /*public IActionResult New()
        {
            Models.Quote beItem = new();
            return PartialView("Quote", beItem);
        }*/

        public IActionResult Edit(long Id)
        {
            string message = "";
            Models.Quote beItem = new();
            try
            {
                BCL.Quote bcQuote = new();
                BEL.Quote beQuote = bcQuote.Search(Id, BEL.relQuote.QuoteDetails, BEL.relQuoteDetail.QuoteDetailPricess, BEL.relQuoteDetailPrices.Subsidiary, BEL.relQuote.QuoteSents);

                List<Field> lstFilter = new()
                {
                    new Field { Name = "ItemCode", Value = string.Join(",", (from d in beQuote.ListQuoteDetails select $"'{d.ProductCode}'").Distinct().ToArray()), Operator = Operators.In },
                    new Field { Name = "Available", Value = 0, Operator = Operators.HigherThan },
                    new Field { Name = "Available2", Value = 0, Operator = Operators.HigherThan },
                    new Field { LogicalOperator = LogicalOperators.Or },
                    new Field { LogicalOperator = LogicalOperators.And }
                };
                BCA.ProductStock bcInventory = new();
                IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(lstFilter, "1");

                IEnumerable<BEP.VolumePricing> lstVolume = new List<BEP.VolumePricing>();
                if (beQuote.ListQuoteDetails?.Count > 0)
                {
                    BCP.VolumePricing bcVolume = new();
                    lstFilter = new List<Field> { new Field { Name = "IdProduct", Value = string.Join(",", (from d in beQuote.ListQuoteDetails select $"'{d.IdProduct}'").ToArray()), Operator = Operators.In } };
                    lstVolume = bcVolume.List(lstFilter, "1", BEP.relVolumePricing.Subsidiary);
                }

                beItem.Id = beQuote.Id;
                beItem.QuoteCode = beQuote.QuoteCode;
                beItem.IdSeller = beQuote.IdSeller;
                beItem.QuoteDate = beQuote.QuoteDate;
                beItem.CardCode = beQuote.CardCode;
                beItem.CardName = beQuote.CardName;
                beItem.ClientName = beQuote.ClientName;
                beItem.ClientMail = beQuote.ClientMail;
                beItem.Header = beQuote.Header;
                beItem.Footer = beQuote.Footer;
                beItem.Details = (from d in beQuote.ListQuoteDetails
                                  select new Models.QuoteDetail
                                  {
                                      Id = d.Id,
                                      IdQuote = d.IdQuote,
                                      IdProduct = d.IdProduct,
                                      ProductCode = d.ProductCode,
                                      ProductName = d.ProductName,
                                      ProductDescription = d.ProductDescription,
                                      ProductImageURL = d.ProductImageURL,
                                      ProductLink = d.ProductLink,
                                      Prices = (from p in d.ListQuoteDetailPricess
                                                select new Models.QuoteDetailPrices
                                                {
                                                    Id = p.Id,
                                                    IdDetail = p.IdDetail,
                                                    IdSubsidiary = p.IdSubsidiary,
                                                    Subsidiary = p.Subsidiary.Name.ToLower(),
                                                    Price = p.Price,
                                                    Observations = p.Observations,
                                                    Selected = p.Selected,
                                                    Quantity = p.Quantity
                                                }).ToList(),
                                      Stock = (from i in lstInventory
                                               where i.ItemCode.ToLower() == d.ProductCode.ToLower()
                                               select new Models.QuoteDetailStock { Warehouse = ToTitle(i.Warehouse), Available = i.Available2, InTransit = i.Requested }).ToList(),
                                      Volume = (from v in lstVolume
                                                where v.IdProduct == d.IdProduct
                                                select new Models.QuoteVolumePrices { Price = v.Price, Quantity = v.Quantity, Observations = v.Observations, Subsidiary = v.Subsidiary.Name.ToUpper() }).ToList()
                                  }).ToList();
                beItem.SentMails = (from s in beQuote.ListQuoteSents
                                    select new Models.QuoteSent { CardDeatil = $"{s.CardCode} - {s.CardName}", ClientDetail = s.ClientName + " ( " + s.ClientMail + " ) ", Date = s.LogDate.ToString("dd/MM/yyyy HH:mm") }).ToList();

            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            //Por ahora aca, pero debería ser una solución global
            //var cul = CultureInfo.CreateSpecificCulture("es-BO");
            //cul.NumberFormat.NumberDecimalSeparator = ".";
            //cul.NumberFormat.NumberGroupSeparator = ",";
            //System.Threading.Thread.CurrentThread.CurrentCulture = cul;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = cul;

            //return PartialView("Quote", beItem);
            return Json(new { message, item = beItem });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(Models.Quote Quote, bool SendMail)
        {
            string message = "";
            try
            {
                BCL.Quote bcQuote = new();
                BEL.Quote beOld = bcQuote.Search(Quote.Id, BEL.relQuote.QuoteDetails, BEL.relQuoteDetail.QuoteDetailPricess) ?? new BEL.Quote { QuoteDate = DateTime.Now, ListQuoteDetails = new List<BEL.QuoteDetail>() };
                BEL.Quote beQuote = Quote.ToEntity();

                long intUserId = UserCode;
                DateTime objDate = DateTime.Now;

                beQuote.IdSeller = intUserId;
                beQuote.LogUser = intUserId;
                beQuote.LogDate = objDate;

                foreach (var item in beQuote.ListQuoteDetails)
                {
                    BEL.QuoteDetail beTemp = (from o in beOld.ListQuoteDetails where o.IdProduct == item.IdProduct select o).FirstOrDefault();
                    if (beTemp == null || beTemp.Id == 0)
                    {
                        item.StatusType = StatusType.Insert;
                    }
                    else
                    {
                        item.StatusType = StatusType.Update;
                        item.Id = beTemp.Id;
                    }
                    item.ProductImageURL = !string.IsNullOrEmpty(item.ProductImageURL) ? item.ProductImageURL.Split('/').Last() : "";
                    item.LogUser = intUserId;
                    item.LogDate = objDate;
                    if (item.ListQuoteDetailPricess == null)
                    {
                        item.ListQuoteDetailPricess = new List<BEL.QuoteDetailPrices>();
                    }
                    foreach (var price in item.ListQuoteDetailPricess)
                    {
                        if (beTemp == null || beTemp.Id == 0)
                        {
                            price.StatusType = StatusType.Insert;
                        }
                        else
                        {
                            BEL.QuoteDetailPrices beSubTemp = (from o in beTemp.ListQuoteDetailPricess where o.IdSubsidiary == price.IdSubsidiary select o).FirstOrDefault();
                            if (beSubTemp == null || beSubTemp.Id == 0)
                            {
                                price.StatusType = StatusType.Insert;
                            }
                            else
                            {
                                price.StatusType = StatusType.Update;
                                price.Id = beSubTemp.Id;
                            }
                        }
                        price.LogUser = intUserId;
                        price.LogDate = objDate;
                    }
                    if (beTemp?.ListQuoteDetailPricess?.Count > 0)
                    {
                        foreach (var price in beTemp.ListQuoteDetailPricess)
                        {
                            if ((from i in item.ListQuoteDetailPricess where i.Id == price.Id select i).Count() == 0)
                            {
                                price.StatusType = StatusType.Delete;
                                item.ListQuoteDetailPricess.Add(price);
                            }
                        }
                    }
                }
                if (beOld?.ListQuoteDetails?.Count > 0)
                {
                    foreach (var item in beOld.ListQuoteDetails)
                    {
                        if ((from i in beQuote.ListQuoteDetails where i.Id == item.Id select i).Count() == 0)
                        {
                            item.StatusType = StatusType.Delete;
                            beQuote.ListQuoteDetails.Add(item);
                        }
                    }
                }
                if (beQuote.Id > 0)
                {
                    beQuote.StatusType = StatusType.Update;
                    beQuote.QuoteDate = DateTime.ParseExact(beQuote.QuoteCode.Substring(0, 8), "yyyyMMdd", null);
                }
                else
                {
                    beQuote.StatusType = StatusType.Insert;
                    beQuote.QuoteDate = objDate;
                    var lastQuote = bcQuote.Search(objDate);
                    var intCont = 1;
                    if (!string.IsNullOrWhiteSpace(lastQuote?.QuoteCode))
                    {
                        intCont = int.Parse(lastQuote.QuoteCode[8..]) + 1;
                    }
                    beQuote.QuoteCode = objDate.ToString("yyyyMMdd") + intCont.ToString().PadLeft(4, '0');
                }

                bcQuote.Save(ref beQuote);
                Quote.Id = beQuote.Id;
                Quote.QuoteCode = beQuote.QuoteCode;

                if (SendMail)
                {
                    sendByMail(Quote);
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, item = Quote });
        }

        [HttpPost]
        public IActionResult Delete(long Id)
        {
            string message = "";
            try
            {
                BCL.Quote bcQuote = new();
                BEL.Quote beQuote = new() { Id = Id, StatusType = StatusType.Delete, QuoteDate = DateTime.Now, LogDate = DateTime.Now };
                bcQuote.Save(ref beQuote);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Métodos Privados

        private Models.QuoteDetail GetProduct(string ItemCode)
        {
            Models.QuoteDetail detail = new();

            BCP.Product bcProduct = new();
            BEP.Product beProduct = bcProduct.Search(ItemCode);

            List<Field> filters = new()
            {
                new Field { Name = "ItemCode", Value = ItemCode },
                new Field { Name = "Stock", Value = 0, Operator = Operators.HigherThan },
                new Field { Name = "Available", Value = 0, Operator = Operators.HigherThan },
                new Field { Name = "Available2", Value = 0, Operator = Operators.HigherThan },
                new Field { LogicalOperator = LogicalOperators.Or },
                new Field { LogicalOperator = LogicalOperators.Or },
                new Field { LogicalOperator = LogicalOperators.And }
            };
            BCA.ProductStock bcInventory = new();
            IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(filters, "1");

            BCP.Price bcPrice = new();
            filters = new List<Field> {
                new Field { Name = "IdProduct", Value = beProduct.Id },
                new Field { Name = "Regular", Value = 0, Operator = Operators.HigherThan },
                new Field { Name = "ISNULL(Offer, 0)", Value = 0, Operator = Operators.HigherThan }
            };
            if (beProduct.ShowAlways)
            {
                filters.Add(new Field { Name = "ISNULL(Observations, '')", Value = "", Operator = Operators.Different });
                filters.Add(new Field { LogicalOperator = LogicalOperators.Or });
            }
            filters.Add(new Field { LogicalOperator = LogicalOperators.Or });
            filters.Add(new Field { LogicalOperator = LogicalOperators.And });
            IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1", BEP.relPrice.Sudsidiary);

            BCP.VolumePricing bcVolume = new();
            filters = new List<Field> { new Field("IdProduct", beProduct.Id) };
            IEnumerable<BEP.VolumePricing> lstVolume = bcVolume.List(filters, "1", BEP.relVolumePricing.Subsidiary);

            BCP.PriceOffer bcOffer = new();
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            filters = new List<Field> { new Field("IdProduct", beProduct.Id), new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) };
            CompleteFilters(ref filters);
            IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

            detail = new Models.QuoteDetail
            {
                IdProduct = beProduct.Id,
                ProductCode = beProduct.ItemCode,
                ProductName = beProduct.Name,
                ProductImageURL = beProduct.ImageURL,
                ProductLink = beProduct.Link,
                ProductDescription = ReplaceCharacters(beProduct.Description + (beProduct.Commentaries != null ? "<br />" + beProduct.Commentaries : "") + (beProduct.Consumables != null ? "<br />Consumibles : " + beProduct.Consumables : "") + (beProduct.Warranty != null ? "<br />Garantía : " + beProduct.Warranty : "")),
                Prices = (from r in lstPrices
                          select new Models.QuoteDetailPrices
                          {
                              Id = r.Id,
                              Price = offers.Any(x => x.IdSubsidiary == r.IdSudsidiary & x.Price > 0) ? offers.First(x => x.IdSubsidiary == r.IdSudsidiary & x.Price > 0).Price : r.Regular,
                              Subsidiary = r.Sudsidiary.Name.ToLower(),
                              IdSubsidiary = r.IdSudsidiary,
                              Selected = true,
                              Observations = r.Observations + (offers.Any(x => x.IdSubsidiary == r.IdSudsidiary & x.Price > 0 & !string.IsNullOrEmpty(x.Description)) ? " " + offers.First(x => x.IdSubsidiary == r.IdSudsidiary & x.Price > 0 & !string.IsNullOrEmpty(x.Description)).Description.Trim() : ""),
                              Quantity = 1
                          }).ToList(),
                Stock = (from i in lstInventory
                         where i.Subsidiary.ToLower() == "iquique" ? i.Stock > 0 : i.Available2 > 0
                         select new Models.QuoteDetailStock { Warehouse = i.Warehouse.ToUpper(), Available = i.Subsidiary.ToLower() == "iquique" ? i.Stock : i.Available2, InTransit = i.Requested }).ToList(),
                Volume = (from v in lstVolume
                          select new Models.QuoteVolumePrices { Price = v.Price, Quantity = v.Quantity, Observations = v.Observations, Subsidiary = v.Subsidiary.Name.ToUpper() }).ToList()
            };

            return detail;
        }

        private string ReplaceCharacters(string Data)
        {
            var strReturn = "";
            if (Data != null)
            {
                strReturn = Data.Replace("\r", "<br />");
            }
            return strReturn;
        }

        private void sendByMail(Models.Quote Quote)
        {
            //Por ahora aca, pero debería ser una solución global
            var cul = CultureInfo.CreateSpecificCulture("es-BO");
            cul.NumberFormat.NumberDecimalSeparator = ".";
            cul.NumberFormat.NumberGroupSeparator = ",";
            System.Threading.Thread.CurrentThread.CurrentCulture = cul;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cul;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div style='font-family: Verdana, Geneva, sans-serif; font-size: 12px; color: #666;'>");
            sb.AppendLine("<p>Estimado Cliente<br />");
            //sb.AppendLine("<strong>" + Quote.CardCode + " - " + Quote.CardName + "</strong><br />");
            sb.AppendLine($"{Quote.ClientName} ( {Quote.ClientMail} )</p>");
            if (!string.IsNullOrEmpty(Quote.Header))
            {
                sb.AppendLine($"<p>{Quote.Header}</p>");
            }
            sb.AppendLine("<style>");
            sb.AppendLine(" .image { width: 30%; padding: 2px 0 2px 2px; text-align: center; } ");
            sb.AppendLine(" .image img { width: 200px; } ");
            sb.AppendLine(" td { font-size: 11.5px; } ");
            sb.AppendLine(" tr { border-bottom: 1px solid #666; } ");
            sb.AppendLine("</style>");

            sb.AppendLine("<table style='width: 95%; font-family: Verdana, Geneva, sans-serif; font-size: 11px; color: #666;'>");
            foreach (var item in Quote.Details)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("   <td class='image' width='200' style='width: 200px;'>");
                sb.AppendLine("     <img src='http://portal.dmc.bo/images/products/" + item.ProductImageURL.Split('/').LastOrDefault() + "' width='200' style='width: 200px;' />");
                sb.AppendLine("   </td>");
                sb.AppendLine("   <td style='padding-left: 5px;'>");
                if (!string.IsNullOrEmpty(item.ProductLink))
                {
                    sb.AppendLine($"     <a href='{item.ProductLink.Trim()}' target='_blank' style='color: #337AB7; text-decoration: none;'><strong><span style='font-size: 13.5px;'>{item.ProductCode}</span></strong></a><br />");
                }
                else
                {
                    sb.AppendLine($"     <strong><span style='font-size: 13.5px;'>{item.ProductCode}</span></strong><br />");
                }
                sb.AppendLine($"     <span style='font-size: 12.5px;'>{item.ProductName}</span><br />");
                sb.AppendLine(item.ProductDescription);
                sb.AppendLine("   </td>");
                sb.AppendLine("   <td style='width: 20%; padding-top: 8px; padding-left: 8px;'>");
                foreach (var price in item.Prices)
                {
                    if (price.Selected)
                    {
                        sb.AppendLine("         <div style='margin-top: 8px; background-color: #F9F9F9; border-radius: 5px; padding: 2px 10px 10px 4px; width: 100%;'>");
                        sb.AppendLine($"             <span style='font-size: 1.4em; color: #CCCCCC;'>{price.Subsidiary}</span>");
                        if (price.Price > 0)
                        {
                            sb.AppendLine($"             <p style='font-size: 1.6em; font-weight: 700; text-align: right; width: 100%;'>{price.Price:#,##0.00} $Us</p>");
                        }
                        if (price.Quantity > 0)
                        {
                            sb.AppendLine($"             <p style='font-size: 1.6em; font-weight: 700; text-align: right; width: 100%;'>{price.Quantity} unidades</p>");
                        }
                        if (!string.IsNullOrEmpty(price.Observations))
                        {
                            sb.AppendLine($"             <p style='text-align: right; width: 100%;'>{price.Observations}</p>");
                        }
                        sb.AppendLine("         </div>");
                    }
                }
                sb.AppendLine("   </td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr>");
                sb.AppendLine("<td colspan='3' style='border-top: 1px solid #666;'></td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            if (!string.IsNullOrEmpty(Quote.Footer))
            {
                sb.AppendLine($"<p>{Quote.Footer}</p>");
            }

            BCS.User bcUser = new();
            BES.User beUser = bcUser.Search(UserCode);
            BCS.UserData bcData = new();
            BES.UserData beData = bcData.SearchByUser(UserCode);
            if (!string.IsNullOrEmpty(beData?.Signature))
            {
                string signature = beData.Signature;
                // TODO: Luego cambiar a la ruta nueva
                if (signature.Contains("/Portal/Content/UserData/"))
                {
                    sb.AppendLine(SetHTMLSafe(signature.Replace("/Portal/Content/UserData/", "http://portal.dmc.bo/images/userdata/")));
                }
                else if (signature.Contains("/Content/UserData/"))
                {
                    sb.AppendLine(SetHTMLSafe(signature.Replace("/Content/UserData/", "http://portal.dmc.bo/images/userdata/")));
                }
                else if (signature.Contains("/Portal/images/userdata/"))
                {
                    sb.AppendLine(SetHTMLSafe(signature.Replace("/Portal/images/userdata/", "http://portal.dmc.bo/images/userdata/")));
                }
                else if (signature.Contains("/images/userdata/"))
                {
                    sb.AppendLine(SetHTMLSafe(signature.Replace("/images/userdata/", "http://portal.dmc.bo/images/userdata/")));
                }
                else
                {
                    sb.AppendLine(SetHTMLSafe(signature));
                }
            }
            else
            {
                sb.AppendLine("<p>Atentamente</p>");
                if (!string.IsNullOrEmpty(beUser?.Name))
                {
                    sb.AppendLine("<p>" + beUser.Name + "</p>");
                }
                else
                {
                    sb.AppendLine("<p>El equipo de DMC</p>");
                }
            }
            sb.AppendLine("</div>");

            List<MailAddress> lstTo = new() { new MailAddress(Quote.ClientMail, Quote.ClientName) }, lstCopy = new();

            string strTitle = "Cotización DMC";

            MailAddress from = null;
            if (!string.IsNullOrEmpty(beUser?.EMail) && IsValidEmail(beUser.EMail.Trim()))
            {
                from = new MailAddress(beUser.EMail, beUser.Name);
                lstCopy.Add(from);
            }

            _ = SendMailAsync(strTitle, sb.ToString(), lstTo, null, lstCopy, from);

            //Guarda constancia del envío
            BCL.QuoteSent bcSent = new();
            BEL.QuoteSent beSent = new()
            {
                StatusType = StatusType.Insert,
                IdQuote = Quote.Id,
                CardCode = Quote.CardCode,
                CardName = Quote.CardName,
                ClientName = Quote.ClientName,
                ClientMail = Quote.ClientMail,
                LogUser = UserCode,
                LogDate = DateTime.Now
            };
            bcSent.Save(ref beSent);
        }

        #endregion
    }
}