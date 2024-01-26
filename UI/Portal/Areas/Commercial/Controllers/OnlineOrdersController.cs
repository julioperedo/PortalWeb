using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System.Collections.Generic;
using System.Linq;
using System;

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
    public class OnlineOrdersController : BaseController
    {
        #region Constructores

        public OnlineOrdersController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            ViewData["Title"] = $"Carrito de Compras";
            if (IsClient)
            {
                ViewData["Title"] += $": {CardName}";
                ViewBag.CardCode = CardCode;
                return View("IndexClient");
            }
            else
            {
                return View();
            }
        }

        public IActionResult GetOrder()
        {
            BCX.Product bcXProduct = new();
            IEnumerable<BEX.Product> xProducts = bcXProduct.ListAvailable("1");
            string message = "";
            try
            {
                if (IsClient)
                {
                    BCO.Sale bcSale = new();
                    BEO.Sale beSale = bcSale.SearchCurrent(UserCode, BEO.relSale.SaleDetails, BEO.relSaleDetail.Product, BEO.relSaleDetail.Subsidiary);
                    BCA.Client bcClient = new();
                    BEA.Client beClient = bcClient.Search(CardCode);
                    beSale ??= new BEO.Sale { Id = 0, IdUser = UserCode, Name = beClient.CardName, Address = beClient.Address, SellerCode = beClient.SellerCode, SellerName = beClient.SellerName.Trim() };
                    beSale.SellerCode = beClient.SellerCode;
                    beSale.SellerName = beClient.SellerName.Trim();
                    Portal.Models.ShoppingCart item = new(beSale);

                    if (beSale.ListSaleDetails?.Count > 0)
                    {
                        string itemCodes = string.Join(",", (from i in beSale.ListSaleDetails where i.Product != null select $"'{i.Product.ItemCode}'").ToArray());
                        BCA.ProductStock bcInventory = new();
                        List<Field> filters = new() { new Field("Stock", 0, Operators.HigherThan), new Field("LOWER(ItemCode)", itemCodes.ToLower(), Operators.In), new Field("Blocked", "N") };
                        CompleteFilters(ref filters);
                        IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(filters, "1");

                        BCB.Classifier bcClassifier = new();
                        filters = new List<Field> { new Field("IdType", (long)BEE.Classifiers.Subsidiary) };
                        IEnumerable<BEB.Classifier> lstSubsidiaries = bcClassifier.List(filters, "1");

                        string productIds = string.Join(",", from i in beSale.ListSaleDetails where i.IdProduct != 0 select i.IdProduct);
                        BCP.Price bcPrice = new();
                        filters = new List<Field> { new Field("IdProduct", productIds, Operators.In) };
                        IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1");

                        BCP.PriceOffer bcOffer = new();
                        string today = DateTime.Today.ToString("yyyy-MM-dd");
                        filters.AddRange(new[] { new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) });
                        CompleteFilters(ref filters);
                        IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

                        BCP.WarehouseAllowed bcWarehouses = new();
                        filters = new List<Field> { new Field("ClientVisible", 1) };
                        var allowedWarehouses = bcWarehouses.List(filters, "1");

                        foreach (var d in item.Details)
                        {
                            if (d.IdSubsidiary.HasValue)
                            {
                                var beSubsidiary = lstSubsidiaries.FirstOrDefault(x => x.Id == d.IdSubsidiary.Value);
                                d.Stock = (from i in lstInventory
                                           where i.Subsidiary.ToLower() == beSubsidiary.Name.ToLower() & i.ItemCode.ToLower() == d.Product.ItemCode.ToLower() & i.Blocked == "N"
                                           select beSubsidiary.Name.ToLower() == "iquique" ? i.Stock : i.Available2).Sum();
                                var beTempPrice = lstPrices.FirstOrDefault(x => x.IdProduct == d.IdProduct & x.IdSudsidiary == d.IdSubsidiary);
                                var offer = offers.FirstOrDefault(x => x.IdProduct == d.IdProduct & x.IdSubsidiary == d.IdSubsidiary & x.Price > 0);
                                if (offer != null)
                                {
                                    d.Price = offer.Price;
                                }
                                else
                                {
                                    if (beTempPrice != null)
                                    {
                                        d.Price = beTempPrice.Regular;
                                    }
                                }
                            }
                            var lstStock = from i in lstInventory
                                           where i.ItemCode.ToLower() == d.Product.ItemCode.ToLower() & allowedWarehouses.Any(x => x.Name.ToLower() == i.Warehouse.ToLower())
                                           group i by i.Subsidiary into g
                                           select new BEA.ProductStock { Subsidiary = g.Key, Stock = g.Sum(d => d.Stock), Available2 = g.Sum(d => d.Available2) };

                            d.DataExtra = (from s in lstSubsidiaries
                                           join i in lstStock on s.Name.ToLower() equals i.Subsidiary.ToLower() into jStock
                                           from ls in jStock.DefaultIfEmpty()
                                           join p in (from ip in lstPrices where ip.IdProduct == d.IdProduct select ip) on s.Id equals p.IdSudsidiary
                                           select new Portal.Models.ShoppingCartDetailExtra
                                           {
                                               Id = s.Id,
                                               Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0).Price : p.Regular,
                                               Stock = (s.Name.ToLower() == "iquique" ? ls?.Stock : ls?.Available2) ?? 0,
                                               IsDigital = xProducts.Any(x => x.ItemCode.ToLower() == d.Product.ItemCode.ToLower())
                                           }).ToList();
                        }
                    }
                    return Json(new { message, item });
                }
                else
                {
                    List<Field> filters = new() { new Field("IdUser", UserCode) };
                    BCO.TempSale bcSale = new();
                    IEnumerable<BEO.TempSale> lstSales = bcSale.List(filters, "1", BEO.relTempSale.TempSaleDetails, BEO.relTempSaleDetail.Product, BEO.relTempSaleDetail.Subsidiary);
                    BEO.TempSale beTemp = lstSales?.FirstOrDefault() ?? new BEO.TempSale { Id = 0, ListTempSaleDetails = new List<BEO.TempSaleDetail>() };
                    Portal.Models.OnlineSale item = new(beTemp);
                    if (item.Details?.Count > 0)
                    {
                        string itemCodes = string.Join(",", item.Details.Where(x => x.Product != null).Select(x => $"'{x.Product.ItemCode}'"));
                        BCA.ProductStock bcInventory = new();
                        filters = new List<Field>
                        {
                            new Field("Stock", 0, Operators.HigherThan), new Field("ItemCode", itemCodes, Operators.In), new Field("Blocked", "N"),
                            new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                        };
                        IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(filters, "1");

                        BCB.Classifier bcClassifier = new();
                        filters = new List<Field> { new Field("IdType", (long)BEE.Classifiers.Subsidiary) };
                        IEnumerable<BEB.Classifier> lstSubsidiaries = bcClassifier.List(filters, "1");

                        string productIds = string.Join(",", from i in item.Details where i.IdProduct != 0 select i.IdProduct);
                        BCP.Price bcPrice = new();
                        filters = new List<Field> { new Field("IdProduct", productIds, Operators.In) };
                        IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1");

                        BCP.PriceOffer bcOffer = new();
                        string today = DateTime.Today.ToString("yyyy-MM-dd");
                        filters.AddRange(new[] { new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) });
                        CompleteFilters(ref filters);
                        IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

                        foreach (var d in item.Details)
                        {
                            if (d.IdSubsidiary.HasValue)
                            {
                                var beSubsidiary = lstSubsidiaries.FirstOrDefault(x => x.Id == d.IdSubsidiary.Value);
                                d.Stock = (from i in lstInventory
                                           where i.Subsidiary.ToLower() == beSubsidiary.Name.ToLower() & i.ItemCode.ToLower() == d.Product.ItemCode.ToLower() & i.Blocked == "N"
                                           select beSubsidiary.Name.ToLower() == "iquique" ? i.Stock : i.Available2).Sum();
                            }
                            var lstStock = from i in lstInventory
                                           where i.ItemCode.ToLower() == d.Product.ItemCode.ToLower()
                                           group i by i.Subsidiary into g
                                           select new BEA.ProductStock { Subsidiary = g.Key, Stock = g.Sum(d => d.Stock), Available2 = g.Sum(d => d.Available2) };
                            d.DataExtra = (from s in lstSubsidiaries
                                           join i in lstStock on s.Name.ToLower() equals i.Subsidiary.ToLower() into jStock
                                           from ls in jStock.DefaultIfEmpty()
                                           join p in (from ip in lstPrices where ip.IdProduct == d.IdProduct select ip).ToList() on s.Id equals p.IdSudsidiary
                                           select new Portal.Models.OnlineSaleDetailExtra
                                           {
                                               Id = s.Id,
                                               Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0).Price : p.Regular,
                                               Stock = (s.Name.ToLower() == "iquique" ? ls?.Stock : ls?.Available2) ?? 0,
                                               IsDigital = xProducts.Any(x => x.ItemCode.ToLower() == d.Product.ItemCode.ToLower())
                                           }).ToList();
                        }
                    }
                    return Json(new { message, item });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
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
                        select new
                        {
                            Id = p.IdSudsidiary,
                            Name = ToTitle(p.Sudsidiary.Name),
                            Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.Price > 0).Price : p.Regular
                        };
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

                    string warehouses = string.Join(",", allowedWarehouses.Select(x => $"'{x.Name.ToLower()}'")), codes = string.Join(",", banner.ListPromoBannerItems.Select(x => $"'{x.Product.ItemCode.ToLower()}'"));
                    BCA.ProductStock bcStock = new();
                    var stock = bcStock.ListRelated("", codes, warehouses, 3);

                    var item = new
                    {
                        banner.Name,
                        banner.ImageUrl,
                        Items = banner.ListPromoBannerItems.Select(i => new { Id = i.IdProduct, i.Product.IsDigital, i.Product.ItemCode, ProductName = i.Product.Name, i.Price, i.IdSubsidiary, SubsidiaryName = i.Subsidiary.Name })
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

        #region Metodos Privados

        #endregion

    }
}
