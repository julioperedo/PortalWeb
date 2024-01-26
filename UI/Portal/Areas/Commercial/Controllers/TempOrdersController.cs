using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUglify.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Areas.Product.Models;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCF = BComponents.Staff;
using BCO = BComponents.Online;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BCX = BComponents.CIESD;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEF = BEntities.Staff;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEX = BEntities.CIESD;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class TempOrdersController : BaseController
    {
        #region Constructores

        public TempOrdersController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            BCX.Product bcXProduct = new();
            IEnumerable<BEX.Product> xProducts = bcXProduct.ListAvailable("1");
            if (CardCode == HomeCardCode)
            {
                ViewData["Title"] = $"Carrito de Compras";
                long userId = UserCode;
                List<Field> filters = new() { new Field { Name = "IdUser", Value = userId } };
                BCO.TempSale bcSale = new();
                IEnumerable<BEO.TempSale> lstSales = bcSale.List(filters, "1", BEO.relTempSale.TempSaleDetails, BEO.relTempSaleDetail.Product, BEO.relTempSaleDetail.Subsidiary);
                BEO.TempSale beTemp = lstSales?.FirstOrDefault() ?? new BEO.TempSale { Id = 0, ListTempSaleDetails = new List<BEO.TempSaleDetail>() };
                Portal.Models.OnlineSale beSale = new(beTemp);
                if (beSale.Details?.Count > 0)
                {
                    string strItemCodes = string.Join(",", (from i in beSale.Details where i.Product != null select $"'{i.Product.ItemCode}'").ToArray());
                    BCA.ProductStock bcInventory = new();
                    filters = new List<Field> {
                        new Field("Stock", 0, Operators.HigherThan), new Field("ItemCode", strItemCodes, Operators.In), new Field("Blocked", "N"),
                        new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                    };
                    IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(filters, "1");

                    BCB.Classifier bcClassifier = new();
                    filters = new List<Field> { new Field { Name = "IdType", Value = (long)BEE.Classifiers.Subsidiary } };
                    IEnumerable<BEB.Classifier> lstSubsidiaries = bcClassifier.List(filters, "1");

                    string strCodes = string.Join(",", (from i in beSale.Details where i.IdProduct != 0 select i.IdProduct).ToArray());
                    BCP.Price bcPrice = new();
                    filters = new List<Field> { new Field { Name = "IdProduct", Value = strCodes, Operator = Operators.In } };
                    IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1");

                    BCP.PriceOffer bcOffer = new();
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    filters.AddRange(new[] { new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) });
                    CompleteFilters(ref filters);
                    IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

                    foreach (var item in beSale.Details)
                    {
                        if (item.IdSubsidiary.HasValue)
                        {
                            var beSubsidiary = (from i in lstSubsidiaries where i.Id == item.IdSubsidiary.Value select i).FirstOrDefault();
                            item.Stock = (from i in lstInventory
                                          where i.Subsidiary.ToLower() == beSubsidiary.Name.ToLower() & i.ItemCode.ToLower() == item.Product.ItemCode.ToLower() & i.Blocked == "N"
                                          select beSubsidiary.Name.ToLower() == "iquique" ? i.Stock : i.Available2).Sum();
                        }
                        var lstStock = from i in lstInventory
                                       where i.ItemCode.ToLower() == item.Product.ItemCode.ToLower()
                                       group i by i.Subsidiary into g
                                       select new BEA.ProductStock { Subsidiary = g.Key, Stock = g.Sum(d => d.Stock), Available2 = g.Sum(d => d.Available2) };
                        item.DataExtra = (from s in lstSubsidiaries
                                          join i in lstStock on s.Name.ToLower() equals i.Subsidiary.ToLower() into jStock
                                          from ls in jStock.DefaultIfEmpty()
                                          join p in (from ip in lstPrices where ip.IdProduct == item.IdProduct select ip).ToList() on s.Id equals p.IdSudsidiary
                                          select new Portal.Models.OnlineSaleDetailExtra
                                          {
                                              Id = s.Id,
                                              Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0).Price : p.Regular,
                                              Stock = (s.Name.ToLower() == "iquique" ? ls?.Stock : ls?.Available2) ?? 0,
                                              IsDigital = xProducts.Any(x => x.ItemCode.ToLower() == item.Product.ItemCode.ToLower())
                                          }).ToList();
                    }
                }
                return View(beSale);
            }
            else
            {
                long IdUser = UserCode;
                BCO.Sale bcSale = new();
                BEO.Sale beSale = bcSale.SearchCurrent(IdUser, BEO.relSale.SaleDetails, BEO.relSaleDetail.Product, BEO.relSaleDetail.Subsidiary);
                BCA.Client bcClient = new();
                BEA.Client beClient = bcClient.Search(CardCode);
                beSale ??= new BEO.Sale { Id = 0, IdUser = IdUser, Name = beClient.CardName, Address = beClient.Address, SellerCode = beClient.SellerCode, SellerName = beClient.SellerName.Trim() };
                beSale.SellerCode = beClient.SellerCode;
                beSale.SellerName = beClient.SellerName.Trim();
                Portal.Models.ShoppingCart cart = new(beSale);

                if (beSale.ListSaleDetails?.Count > 0)
                {
                    string strItemCodes = string.Join(",", (from i in beSale.ListSaleDetails where i.Product != null select $"'{i.Product.ItemCode}'").ToArray());
                    BCA.ProductStock bcInventory = new();
                    List<Field> filters = new()
                    {
                        new Field { Name = "Stock", Value = 0, Operator = Operators.HigherThan },
                        new Field { Name = "LOWER(ItemCode)", Value = strItemCodes.ToLower(), Operator = Operators.In },
                        new Field { Name = "Blocked", Value = "N" },
                        new Field { LogicalOperator = LogicalOperators.And },
                        new Field { LogicalOperator = LogicalOperators.And }
                    };
                    IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(filters, "1");

                    BCB.Classifier bcClassifier = new();
                    filters = new List<Field> { new Field { Name = "IdType", Value = (long)BEE.Classifiers.Subsidiary } };
                    IEnumerable<BEB.Classifier> lstSubsidiaries = bcClassifier.List(filters, "1");

                    string strCodes = string.Join(",", (from i in beSale.ListSaleDetails where i.IdProduct != 0 select i.IdProduct).ToArray());
                    BCP.Price bcPrice = new();
                    filters = new List<Field> { new Field("IdProduct", strCodes, Operators.In) };
                    IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1");

                    BCP.PriceOffer bcOffer = new();
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    filters.AddRange(new[] { new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) });
                    CompleteFilters(ref filters);
                    IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

                    BCP.WarehouseAllowed bcWarehouses = new();
                    filters = new List<Field> { new Field("ClientVisible", 1) };
                    var allowedWarehouses = bcWarehouses.List(filters, "1");

                    foreach (var item in cart.Details)
                    {
                        if (item.IdSubsidiary.HasValue)
                        {
                            var beSubsidiary = lstSubsidiaries.FirstOrDefault(x => x.Id == item.IdSubsidiary.Value);
                            item.Stock = (from i in lstInventory
                                          where i.Subsidiary.ToLower() == beSubsidiary.Name.ToLower() & i.ItemCode.ToLower() == item.Product.ItemCode.ToLower() & i.Blocked == "N"
                                          select beSubsidiary.Name.ToLower() == "iquique" ? i.Stock : i.Available2).Sum();
                            var beTempPrice = lstPrices.FirstOrDefault(x => x.IdProduct == item.IdProduct & x.IdSudsidiary == item.IdSubsidiary);
                            var offer = offers.FirstOrDefault(x => x.IdProduct == item.IdProduct & x.IdSubsidiary == item.IdSubsidiary & x.Price > 0);
                            if (offer != null)
                            {
                                item.Price = offer.Price;
                            }
                            else
                            {
                                if (beTempPrice != null)
                                {
                                    item.Price = beTempPrice.Regular;
                                }
                            }
                        }
                        var lstStock = from i in lstInventory
                                       where i.ItemCode.ToLower() == item.Product.ItemCode.ToLower() & allowedWarehouses.Any(x => x.Name.ToLower() == i.Warehouse.ToLower())
                                       group i by i.Subsidiary into g
                                       select new BEA.ProductStock { Subsidiary = g.Key, Stock = g.Sum(d => d.Stock), Available2 = g.Sum(d => d.Available2) };

                        item.DataExtra = (from s in lstSubsidiaries
                                          join i in lstStock on s.Name.ToLower() equals i.Subsidiary.ToLower() into jStock
                                          from ls in jStock.DefaultIfEmpty()
                                          join p in (from ip in lstPrices where ip.IdProduct == item.IdProduct select ip) on s.Id equals p.IdSudsidiary
                                          select new Portal.Models.ShoppingCartDetailExtra
                                          {
                                              Id = s.Id,
                                              Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0).Price : p.Regular,
                                              Stock = (s.Name.ToLower() == "iquique" ? ls?.Stock : ls?.Available2) ?? 0,
                                              IsDigital = xProducts.Any(x => x.ItemCode.ToLower() == item.Product.ItemCode.ToLower())
                                          }).ToList();
                    }
                }

                ViewData["Title"] = $"Carrito de Compras : {CardName}";
                ViewBag.CardCode = CardCode;
                return View("IndexClient", cart);
            }
        }

        public IActionResult GetSubsidiariesByProduct(long ProductId)
        {
            BCP.Price bcPrice = new();
            List<Field> filters = new() { new Field("IdProduct", ProductId) };
            IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1", BEP.relPrice.Sudsidiary);
            BCP.PriceOffer bcOffer = new();
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            filters.AddRange(new[] { new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) });
            CompleteFilters(ref filters);
            IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

            var items = from p in lstPrices
                        where p.Regular > 0
                        select new { Id = p.IdSudsidiary, Name = ToTitle(p.Sudsidiary.Name), Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.Price > 0).Price : p.Regular };
            return Json(items);
        }

        public IActionResult GetRelatedProducts(int Quantity)
        {
            string message = "";
            try
            {
                BCX.Product bcXProduct = new();
                IEnumerable<BEX.Product> xProducts = bcXProduct.ListAvailable("1");

                BCP.RelatedCategory bcRelCategory = new();
                IEnumerable<BEP.RelatedCategory> relCategories = bcRelCategory.List(UserCode, "1");

                BCP.RelatedProduct bcRelProduct = new();
                IEnumerable<BEP.RelatedProduct> relProducts = bcRelProduct.List(UserCode, "1", BEP.relRelatedProduct.Related) ?? Enumerable.Empty<BEP.RelatedProduct>();

                string categories = string.Join(",", relCategories.Select(x => $"'{x.Related.ToLower()}'")), productCodes = string.Join(",", relProducts.Select(x => $"'{x.Related.ItemCode.ToLower()}'")),
                    productIds = string.Join(",", relProducts.Select(x => $"'{x.IdRelated}'"));

                BCP.WarehouseAllowed bcWarehouses = new();
                List<Field> filters = new() { new Field("ClientVisible", 1) };
                var allowedWarehouses = bcWarehouses.List(filters, "1");

                string warehouses = string.Join(",", allowedWarehouses.Select(x => $"'{x.Name.ToLower()}'"));

                BCA.ProductStock bcStock = new();
                IEnumerable<BEA.ProductStock> stockItems = bcStock.ListRelated(categories, productCodes, warehouses, Quantity * 3);
                productCodes = string.Join(",", stockItems.Select(x => $"'{x.ItemCode.ToLower()}'"));

                BCP.Product bcProduct = new();
                filters = new() { new Field("LOWER(ItemCode)", productCodes, Operators.In) };
                var products = stockItems.Any() ? bcProduct.List(filters, "1", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary) : new List<BEP.Product>();

                IEnumerable<BEP.PriceOffer> offers = Enumerable.Empty<BEP.PriceOffer>();
                if (relProducts.Any())
                {
                    BCP.PriceOffer bcOffer = new();
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    filters = new List<Field> { new Field("IdProduct", productIds, Operators.In), new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) };
                    CompleteFilters(ref filters);
                    offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();
                }

                var items = from p in products
                            select new
                            {
                                p.Id,
                                p.Name,
                                p.ItemCode,
                                p.IsDigital,
                                p.ImageURL,
                                Prices = from r in p.ListPrices
                                         join s in stockItems on new { C = p.ItemCode.ToLower(), S = r.Sudsidiary.Name.ToLower() } equals new { C = s.ItemCode.ToLower(), S = s.Subsidiary.ToLower() } into jStock
                                         from ls in jStock.DefaultIfEmpty()
                                         where ls?.Stock > 0 || ls?.Available2 > 0
                                         select new
                                         {
                                             Id = r.IdSudsidiary,
                                             Name = ToTitle(r.Sudsidiary.Name),
                                             Price = offers.Any(x => x.IdSubsidiary == r.IdSudsidiary & x.IdProduct == r.IdProduct & x.Price > 0) ? offers.First(x => x.IdSubsidiary == r.IdSudsidiary & x.IdProduct == r.IdProduct & x.Price > 0).Price : r.Regular,
                                             Stock = (r.Sudsidiary.Name.ToLower() == "iquique" ? ls?.Stock : ls?.Available2) ?? 0,
                                             IsDigital = xProducts.Any(x => x.ItemCode.ToLower() == p.ItemCode.ToLower())
                                         }
                            };
                items = items.Where(x => x.Prices.Any()).Take(Quantity);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetBanner()
        {
            string message = "";
            try
            {
                BCP.PromoBanner bcBanner = new();
                var banner = bcBanner.SearchSuggested(UserCode, BEP.relPromoBanner.PromoBannerItems, BEP.relPromoBannerItem.Product, BEP.relPromoBannerItem.Subsidiary);
                if (banner != null)
                {
                    BCP.WarehouseAllowed bcWarehouses = new();
                    List<Field> filters = new() { new Field("ClientVisible", 1) };
                    var allowedWarehouses = bcWarehouses.List(filters, "1");

                    string warehouses = string.Join(",", allowedWarehouses.Select(x => $"'{x.Name.ToLower()}'"));

                    string codes = string.Join(",", banner.ListPromoBannerItems.Select(x => $"'{x.Product.ItemCode.ToLower()}'"));
                    BCA.ProductStock bcStock = new();
                    var stock = bcStock.ListRelated("", codes, warehouses, 3);

                    var item = new
                    {
                        banner.Name,
                        banner.ImageUrl,
                        Items = banner.ListPromoBannerItems.Select(i => new
                        {
                            Id = i.IdProduct,
                            i.Product.IsDigital,
                            i.Product.ItemCode,
                            ProductName = i.Product.Name,
                            i.Price,
                            i.IdSubsidiary,
                            SubsidiaryName = i.Subsidiary.Name
                        })
                    };
                    return Json(new { message, item });
                }
                else
                {
                    return Json(new { message, item = banner });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BEO.Sale Item)
        {
            string message = "";
            try
            {
                SaveSale(ref Item);
                return Json(new { message, Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public async Task<IActionResult> Send(BEO.Sale Item)
        {
            string message = "";
            try
            {
                Item.StateIdc = (long)BEE.States.SaleOnline.Sent;
                SaveSale(ref Item);
                var (sent, strMessage) = await SendMailAsync(Item);
                if (!sent) message = $"No se ha podido enviar el correo a las partes. <br />{strMessage}";
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete(long Id)
        {
            string message = "";
            try
            {
                BCO.Sale bcSale = new();
                BEO.Sale beSale = bcSale.Search(Id);
                beSale.StatusType = StatusType.Update;
                beSale.StateIdc = (long)BEE.States.SaleOnline.Canceled;
                beSale.Code ??= GetCode();
                beSale.LogUser = UserCode;
                beSale.LogDate = DateTime.Now;

                bcSale.Save(ref beSale);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult AddItem(long IdProduct, int Quantity, decimal Price, bool OpenBox, long IdSubsidiary, string Warehouse)
        {
            string message = "";
            long id = 0;
            try
            {
                long IdUser = UserCode;
                BCO.Sale bcSale = new();
                BEO.Sale beSale = bcSale.SearchCurrent(IdUser, BEO.relSale.SaleDetails);

                if (beSale == null)
                {
                    BCA.Client bcClient = new();
                    BEA.Client beClient = bcClient.Search(CardCode);
                    beSale = new BEO.Sale
                    {
                        StatusType = StatusType.Insert,
                        IdUser = IdUser,
                        Name = beClient.CardName ?? "",
                        Address = beClient.Address ?? "",
                        SellerCode = beClient.SellerCode,
                        SellerName = beClient.SellerName.Trim(),
                        LogUser = IdUser,
                        LogDate = DateTime.Now,
                        StateIdc = (long)BEE.States.SaleOnline.Created,
                        ListSaleDetails = new List<BEO.SaleDetail>()
                    };
                }

                if (!(from d in beSale.ListSaleDetails where d.IdProduct == IdProduct & d.IdSubsidiary == IdSubsidiary select d).Any())
                {
                    var detail = new BEO.SaleDetail
                    {
                        StatusType = StatusType.Insert,
                        IdProduct = IdProduct,
                        IdSubsidiary = IdSubsidiary,
                        Quantity = Quantity,
                        Price = Price,
                        OpenBox = OpenBox,
                        Warehouse = Warehouse ?? "",
                        LogUser = IdUser,
                        LogDate = DateTime.Now
                    };
                    beSale.ListSaleDetails.Add(detail);
                    beSale.Total = (from d in beSale.ListSaleDetails select d.Quantity * d.Price).Sum();
                    bcSale.Save(ref beSale);
                    id = detail.Id;
                }
                else
                {
                    message = "Ya se ha agregado ese item al Carrito de Compras";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, id });
        }

        [HttpPost]
        public IActionResult DeleteItem(long Id)
        {
            string message = "";
            try
            {
                BCO.SaleDetail bcDetail = new();
                BEO.SaleDetail item = new() { Id = Id, StatusType = StatusType.Delete };
                bcDetail.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private void SaveSale(ref BEO.Sale Item)
        {
            long IdUser = UserCode;
            DateTime objDate = DateTime.Now;
            BCO.Sale bcSale = new();
            BEO.Sale beSale;
            BCA.Client bcClient = new();
            BEA.Client beClient = bcClient.Search(CardCode);
            if (Item.Id == 0)
            {
                beSale = new BEO.Sale { StatusType = StatusType.Insert, StateIdc = (long)BEE.States.SaleOnline.Created, IdUser = IdUser };
            }
            else
            {
                beSale = bcSale.Search(Item.Id, BEO.relSale.SaleDetails);
                beSale.StatusType = StatusType.Update;
            }
            if (string.IsNullOrEmpty(beSale.Code)) beSale.Code = GetCode();
            beSale.Name = Item.Name?.Trim();
            beSale.Address = Item.Address?.Trim();
            beSale.Commentaries = Item.Commentaries?.Trim();
            beSale.ClientSaleNote = Item.ClientSaleNote;
            beSale.WithDropShip = Item.WithDropShip;
            beSale.StateIdc = Item.StateIdc != 0 ? Item.StateIdc : (long)BEE.States.SaleOnline.Created;
            beSale.SellerCode = beClient.SellerCode ?? "";
            beSale.SellerName = beClient.SellerName?.Trim() ?? "";
            beSale.LogUser = IdUser;
            beSale.LogDate = objDate;

            beSale.ListSaleDetails ??= new List<BEO.SaleDetail>();
            Item.ListSaleDetails ??= new List<BEO.SaleDetail>();

            foreach (var item in beSale.ListSaleDetails)
            {
                BEO.SaleDetail beItem = (from d in Item.ListSaleDetails where d.IdProduct == item.IdProduct select d).FirstOrDefault();
                if (beItem == null)
                {
                    item.StatusType = StatusType.Delete;
                }
                else
                {
                    item.StatusType = StatusType.Update;
                    item.Quantity = beItem.Quantity;
                    item.Warehouse = beItem.Warehouse ?? "";
                    item.IdSubsidiary = beItem.IdSubsidiary;
                    item.Price = beItem.Price;
                    item.LogUser = IdUser;
                    item.LogDate = objDate;
                }
            }
            foreach (var item in Item.ListSaleDetails)
            {
                if (!(from d in beSale.ListSaleDetails select d.IdProduct).Contains(item.IdProduct))
                {
                    item.StatusType = StatusType.Insert;
                    item.Warehouse ??= "";
                    item.LogUser = IdUser;
                    item.LogDate = objDate;
                    beSale.ListSaleDetails.Add(item);
                }
            }
            beSale.Total = (from d in beSale.ListSaleDetails where d.StatusType != StatusType.Delete select d.Quantity * d.Price).Sum();
            bcSale.Save(ref beSale);
            Item = beSale;
        }

        private async Task<(bool, string)> SendMailAsync(BEO.Sale Item)
        {
            bool boReturn = false;
            string strSubject = "Carrito de Compras", message = "";
            StringBuilder sb = new();

            List<Field> lstFilter = new() { new Field("Id", string.Join(",", Item.ListSaleDetails.Select(x => x.IdProduct)), Operators.In) };
            BCP.Product bcProduct = new();
            IEnumerable<BEP.Product> lstProducts = bcProduct.List(lstFilter, "1");

            BCB.Classifier bcClassifier = new();
            List<BEB.Classifier> lstSubsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");

            BCA.Client bcClient = new();
            BEA.Client beClient = bcClient.Search(CardCode);

            BCS.User bcUser = new();
            BES.User beUser = bcUser.Search(UserCode), beSeller = bcUser.SearchSeller(Item.SellerCode) ?? bcUser.SearchSeller(beClient.SellerCode) ?? new BES.User();

            sb.AppendLine(@"<html>");
            sb.AppendLine(@"<head>");
            sb.AppendLine(@"<meta charset=""utf-8"">");
            sb.AppendLine(@"<meta name=""viewport"" content=""width=device-width"">");
            sb.AppendLine(@"<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">");
            sb.AppendLine(@"<meta name=""x-apple-disable-message-reformatting"">");
            sb.AppendLine(@"<title>Solicitud</title>");
            sb.AppendLine(@"<!-- Web Font / @font-face -->");
            sb.AppendLine(@"<!-- Desktop Outlook chokes on web font references and defaults to Times New Roman, so we force a safe fallback font. -->");
            sb.AppendLine(@"<!--[if mso]>");
            sb.AppendLine(@"        <style>");
            sb.AppendLine(@"            * { font-family: sans-serif !important; }");
            sb.AppendLine(@"        </style>");
            sb.AppendLine(@"        <![endif]-->");
            sb.AppendLine(@"<!-- End Web Font / @font-face -->");
            sb.AppendLine(@"<!-- CSS Reset -->");
            sb.AppendLine(@"");
            sb.AppendLine(@"<style type=""text/css"">");
            sb.AppendLine(@"@import 'https://fonts.googleapis.com/css?family=Lato:400,700,900&subset=latin-ext';");
            sb.AppendLine(@"body { color: #666; font-family: 'Lato', sans-serif;  font-size: 11px; }");
            sb.AppendLine(@".table { width: 650px; margin: 10px 5px 10px 5px; border-spacing: 0; border-collapse: collapse; }");
            sb.AppendLine(@".table thead td { padding: 4px; background-color: #3E3C4E; color: #FFF; }");
            sb.AppendLine(@".table .right { text-align: right; }");
            sb.AppendLine(@".table tfoot td { background-color: #3E3C4E; color: #FFF; }");
            sb.AppendLine(@".footer { background-color: #3e3c4e; color: #EEE; text-align: center; font-size: 0.8em; border-radius: 0 0 5px 5px; padding: 6px; }");
            sb.AppendLine(@"p { font-size: 0.85em; margin: 6px; }");
            sb.AppendLine(@"</style>");
            sb.AppendLine(@"</head>");
            sb.AppendLine(@"<body valign=""middle"" style=""margin: 0 auto; padding: 0; height: 100%; width: 100%; text-align: center; background-repeat: no-repeat;"">");
            sb.AppendLine(@"<div style=""max-width: 680px; margin: auto;"">");
            sb.AppendLine(@"  <div style=""background-color: #FFF; margin: 10px; border-radius: 5px; text-align:left;""><img src=""http://www.dmc.bo/img/logo3.png"" alt=""DMC S.A."" style=""margin: 8px;"" height=""70"" />");
            sb.AppendLine($@"    <div style=""background-color: #CADFF7; color: #0C4278;padding: 10px;""> <strong>Cliente:</strong> ({beClient.CardCode}) {beClient.CardName} <br />");
            sb.AppendLine($@"      <strong>Nombre Entrega:</strong> {Item.Name} <br />");
            sb.AppendLine($@"      <strong>Direcci&oacute;n Entrega:</strong> {Item.Address} <br />");
            sb.AppendLine($@"      <strong>Comentarios:</strong> {Item.Commentaries} <br />");
            sb.AppendLine($@"      <strong>Orden de Compra Cliente:</strong> {Item.ClientSaleNote} <br />");
            sb.AppendLine($@"      <strong>Vendedor:</strong> {beSeller.Name} <br />");
            //if(Item.WithDropShip) {
            //    sb.AppendLine($@"      <strong>Requiere DropShip:</strong> SI </div>");
            //} else {
            //    sb.AppendLine($@"      <strong>Requiere DropShip:</strong> NO </div>");
            //}
            sb.AppendLine(@"    </div>");
            sb.AppendLine(@"    <div style=""background-color: #FFF;"">");
            sb.AppendLine(@"      <table class=""table"">");
            sb.AppendLine(@"        <thead>");
            sb.AppendLine(@"          <tr>");
            sb.AppendLine(@"            <td>C&oacute;digo</td>");
            sb.AppendLine(@"            <td>Nombre</td>");
            sb.AppendLine(@"            <td>Sucursal</td>");
            //sb.AppendLine(@"            <td>Almac&eacute;n</td>");
            sb.AppendLine(@"            <td class=""right"">Cantidad</td>");
            sb.AppendLine(@"            <td class=""right"">Precio</td>");
            sb.AppendLine(@"            <td class=""right"">Subtotal</td>");
            sb.AppendLine(@"          </tr>");
            sb.AppendLine(@"        </thead>");
            sb.AppendLine(@"        <tbody>");

            var files = new List<Attachment>();
            Stream stream;
            using (ExcelPackage objExcel = new())
            {
                ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
                FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

                wsMain.Name = "Pedido";

                wsMain.Column(1).Width = 20;
                wsMain.Column(2).Width = 35;
                wsMain.Column(3).Width = 11;

                wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
                wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
                wsMain.Cells.Style.Font.Size = 9;

                wsMain.Cells[4, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                wsMain.Cells[6, 1].Value = "Código";
                wsMain.Cells[6, 2].Value = "Nombre";
                wsMain.Cells[6, 3].Value = "Sucursal";
                wsMain.Cells[6, 4].Value = "Cantidad";
                wsMain.Cells[6, 5].Value = "Precio";
                wsMain.Cells[6, 5].Value = "Subtotal";

                int row = 7;
                foreach (var detail in Item.ListSaleDetails)
                {
                    BEP.Product beProduct = lstProducts.FirstOrDefault(x => x.Id == detail.IdProduct);
                    BEB.Classifier beSubsidiary = lstSubsidiaries.FirstOrDefault(x => x.Id == detail.IdSubsidiary);
                    if (beProduct != null)
                    {
                        sb.AppendLine(@"          <tr>");
                        sb.AppendLine($@"            <td>{beProduct.ItemCode}</td>");
                        sb.AppendLine($@"            <td>{beProduct.Name}</td>");
                        sb.AppendLine($@"            <td>{beSubsidiary.Name}</td>");
                        //sb.AppendLine($@"            <td>{detail.Warehouse}</td>");
                        sb.AppendLine($@"            <td class=""right"">{detail.Quantity}</td>");
                        sb.AppendLine($@"            <td class=""right"">{detail.Price:#,##0.00}</td>");
                        sb.AppendLine($@"            <td class=""right"">{detail.Price * detail.Quantity:#,##0.00}</td>");
                        sb.AppendLine(@"          </tr>");

                        wsMain.Cells[row, 1].Value = beProduct.ItemCode;
                        wsMain.Cells[row, 2].Value = beProduct.Name;
                        wsMain.Cells[row, 3].Value = beSubsidiary.Name;
                        wsMain.Cells[row, 4].Value = detail.Quantity;
                        wsMain.Cells[row, 5].Value = detail.Price;
                        wsMain.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
                        wsMain.Cells[row, 6].Formula = $"D{row}*E{row}";
                        wsMain.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
                        row++;
                    }
                }
                wsMain.Cells[row, 6].Formula = $"SUM(F7:F{row - 1})";
                wsMain.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
                wsMain.Cells.AutoFitColumns();
                //wsMain.Cells.Style.WrapText = true;

                var file = objExcel.GetAsByteArray();
                stream = new MemoryStream(file);
                objExcel.Dispose();
            }

            files.Add(new Attachment(stream, "Detalle.xlsx"));
            sb.AppendLine(@"        </tbody>");
            sb.AppendLine(@"        <tfoot>");
            sb.AppendLine(@"          <tr>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            //sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine($@"            <td class=""right"">{(from d in Item.ListSaleDetails select d.Price * d.Quantity).Sum():#,##0.00}</td>");
            sb.AppendLine(@"          </tr>");
            sb.AppendLine(@"        </tfoot>");
            sb.AppendLine(@"      </table>");
            sb.AppendLine(@"      <p>Su solicitud ha sido enviada en espera de ser atendida por su Ejecutivo de Cuenta, el cual se pondr&aacute; en contacto con usted para su confirmaci&oacute;n y ser procesada.<br />");
            sb.AppendLine(@"      <b>Validez de la Cotizaci&oacute;n:</b> 3 d&iacute;as<br /><b>Nota:</b>En esta cotizaci&oacute;n se puede incrementar productos y cantidades.<br />Los descuentos por vol&uacute;men de compra ser&aacute;n confirmados por su Ejecutivo de Cuenta al procesar la orden.</p>");
            sb.AppendLine(@"    </div>");
            sb.AppendLine(@"    <div class=""footer"">Este correo es generado por un servicio autom&aacute;tico, por favor no responda.</div>");
            sb.AppendLine(@"  </div>");
            sb.AppendLine(@"</div>");
            sb.AppendLine(@"</body>");
            sb.AppendLine(@"</html>");

            List<MailAddress> lstTo = new(), lstCopies = new();
            if ((beUser != null && !string.IsNullOrWhiteSpace(beUser.EMail)) & (beSeller != null && !string.IsNullOrWhiteSpace(beSeller.EMail)))
            {
                if (IsDevelopmentMode)
                {
                    lstTo.Add(new MailAddress("julio.peredo@gmail.com", "Julio Peredo"));
                    lstCopies.Add(new MailAddress("julio.peredo@dmc.bo", "Julio C Peredo F"));
                }
                else
                {
                    if (!IsEMailBlacklisted(beUser.EMail))
                    {
                        lstTo.Add(new MailAddress(beUser.EMail, beUser.Name));
                        lstTo.Add(new MailAddress(beSeller.EMail, beSeller.Name));

                        List<Field> filters = new()
                        {
                            new Field("SellerCode", Item.SellerCode),
                            new Field("InitialDate", DateTime.Today.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan),
                            new Field("FinalDate", DateTime.Today.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan),
                            new Field(LogicalOperators.And),
                            new Field(LogicalOperators.And)
                        };
                        BCF.Replace bcReplace = new();
                        IEnumerable<BEF.Replace> replaces = bcReplace.List(filters, "1");
                        if (replaces?.Count() > 0)
                        {
                            filters = new List<Field> { new Field("SellerCode", string.Join(",", replaces.Select(x => $"'{x.ReplaceCode}'").Distinct()), Operators.In) };
                            BCS.UserData bcData = new();
                            IEnumerable<BES.UserData> userDatas = bcData.List(filters, "1", BES.relUserData.User);
                            if (userDatas?.Count() > 0)
                            {
                                foreach (var item in userDatas)
                                {
                                    if (IsValidEmail(item.User?.EMail)) lstCopies.Add(new MailAddress(item.User.EMail, item.User.Name));
                                }
                            }
                        }
                    }
                    else
                    {
                        message = $"Su correo {beUser.EMail} está en nuestra lista negra, por favor comuníquese con su Ejecutivo de Ventas";
                    }
                }
                if (lstTo.Count > 0 || lstCopies.Count > 0)
                {
                    await SendMailAsync(strSubject, sb.ToString(), lstTo, lstCopies, null, null, files);
                    boReturn = true;
                }
            }
            return (boReturn, message);
        }

        private string GetCode()
        {
            string strCode = $"{CardCode.Replace("-", "")}-{DateTime.Now.Year}";
            BCO.Sale bcSale = new();
            List<Field> lstFilter = new() { new Field("Code", strCode, Operators.Likes), new Field("IdUser", UserCode), new Field(LogicalOperators.And) };

            IEnumerable<BEO.Sale> lstSales = bcSale.List(lstFilter, "Code DESC");
            if (lstSales?.Count() > 0)
            {
                string lastCode = lstSales.First().Code;
                if (!string.IsNullOrWhiteSpace(lastCode))
                {
                    long intCode = long.Parse(lastCode.Split('-').Last()) + 1;
                    strCode = CardCode.Replace("-", "") + "-" + intCode.ToString().PadLeft(4, '0');
                }
                else
                {
                    strCode = CardCode.Replace("-", "") + "-" + DateTime.Now.Year.ToString() + "0001";
                }
            }
            else
            {
                strCode = CardCode.Replace("-", "") + "-" + DateTime.Now.Year.ToString() + "0001";
            }
            return strCode;
        }

        #endregion
    }
}
