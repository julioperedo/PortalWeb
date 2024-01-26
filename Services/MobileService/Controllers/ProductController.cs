using BEntities.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCF = BComponents.Staff;
using BCL = BComponents.Sales;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace MobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseController
    {
        #region Global Variables

        private readonly string HomeCardCode = "CDMC-002";

        #endregion

        #region GETs

        [HttpGet("lines")]
        public IActionResult Lines(long UserCode)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new();
                BES.User user = bcUser.Search(UserCode) ?? new BES.User { CardCode = "", AllowLinesBlocked = false };

                BCP.Line bcLine = new();
                IEnumerable<BEP.Line> lstLines = bcLine.ListForPriceList(HomeCardCode == user.CardCode, "Name");

                BCS.LineNotAllowed bcClient = new();
                List<Field> lstFilter = new() { new Field("CardCode", user.CardCode) };
                var lstClientExceptions = bcClient.List(lstFilter, "1");

                var items = from l in lstLines
                            where user.CardCode == HomeCardCode || user.AllowLinesBlocked || l.WhenFilteredShowInfo || ((l.FilterType == "AllBut" & !(from a in lstClientExceptions select a.IdLine).Contains(l.Id)) || (l.FilterType == "NoneBut" & (from a in lstClientExceptions select a.IdLine).Contains(l.Id)))
                            select new { l.Id, l.Name, l.ImageURL };

                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("categories")]
        public IActionResult Categories()
        {
            string message = "";
            try
            {
                BCD.Category bcCategory = new();
                IEnumerable<BED.Category> lstCategories = bcCategory.List("1");
                var items = lstCategories.Select(x => new { id = x.Id, name = x.Name, idLine = x.IdLine });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("subcategories")]
        public IActionResult Subcategories()
        {
            string message = "";
            try
            {
                BCD.SubCategory bcSubcategory = new();
                IEnumerable<BED.SubCategory> lstSubCategories = bcSubcategory.List("1");
                var items = lstSubCategories.Select(x => new { id = x.Id, name = x.Name, idCategory = x.IdCategory });
                return Ok(new { message, items });

            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("productsearch")]
        public IActionResult ProductSearch(long? LineId, string Category, string Subcategory, string Filter, long UserId, string CardCode)
        {
            string message = "";
            try
            {
                IEnumerable<BEP.Product> lstProducts = GetItems(LineId, Category, Subcategory, Filter, UserId);
                BCP.PriceHistory bcHistory = new();
                List<long> lstProductIds = (from p in lstProducts where !string.IsNullOrWhiteSpace(p.Line) select p.Id).Distinct().ToList();
                IEnumerable<BEP.PriceHistory> lstHistory = bcHistory.ListFirst(lstProductIds);
                IEnumerable<BEA.ProductStock> lstStock = Enumerable.Empty<BEA.ProductStock>();
                BCP.WarehouseAllowed bcWarehouse = new();
                IEnumerable<BEP.WarehouseAllowed> lstWarehouses = bcWarehouse.List("1");
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(UserId, BES.relUser.Profile, BES.relProfile.ProfilePages);

                BCS.LineNotAllowed bcClient = new();
                List<Field> lstFilter2 = new() { new Field { Name = "CardCode", Value = CardCode } };
                var lstClientExceptions = bcClient.List(lstFilter2, "1");

                BCP.PriceGroupLine bcGroupLine = new();
                var groupLineList = bcGroupLine.ListByClient(CardCode, "1");

                BCP.PriceGroup bcGroup = new();
                var groupLists = bcGroup.ListByClient(CardCode, "1");

                BCP.Line bcLine = new();
                var lines = bcLine.ListForPriceList("1", BEP.relLine.LineDetails, BEP.relLine.Manager);
                var lstLines = (from l in lines
                                where CardCode == "CDMC-002" || beUser.AllowLinesBlocked || l.WhenFilteredShowInfo || ((l.FilterType == "AllBut" & !(from a in lstClientExceptions select a.IdLine).Contains(l.Id)) || (l.FilterType == "NoneBut" & (from a in lstClientExceptions select a.IdLine).Contains(l.Id)))
                                join g in groupLineList on l.Id equals g.IdLine into ljGroup
                                from lj in ljGroup.DefaultIfEmpty()
                                select new Models.LineShort
                                {
                                    Id = l.Id,
                                    Name = l.Name,
                                    Description = l.Description?.Replace("\r", "<br />") ?? "",
                                    Footer = l.Footer?.Replace("\r", "<br />") ?? "",
                                    Header = l.Header?.Replace("\r", "<br />") ?? "",
                                    ImageURL = l.ImageURL,
                                    SAPLines = l.ListLineDetails.Select(x => x.SAPLine).ToList(),
                                    Manager = l.Manager != null ? new Models.Manager(l.Manager) : null,
                                    Percentage = lj?.Percentage ?? groupLists.FirstOrDefault()?.Percentage ?? 0,
                                    FilterType = l.FilterType,
                                }).ToList();

                if (UserId > 0)
                {
                    BCA.ProductStock bcStock = new();
                    string itemCodes = string.Join(",", (from p in lstProducts where !string.IsNullOrWhiteSpace(p.Line) & p.ListPrices.Any(x => x.Regular > 0) select $"'{p.ItemCode}'"));
                    List<Field> filters = new() { new Field("LOWER(ItemCode)", itemCodes.ToLower(), Operators.In) };
                    try
                    {
                        lstStock = bcStock.List(filters, "1");
                    }
                    catch (Exception) { }
                }

                var items = from p in lstProducts
                            orderby p.Name
                            join h in lstHistory on p.Id equals h.IdProduct into lj
                            from lh in lj.DefaultIfEmpty()
                            join l in lstLines on p.Line equals l.Name
                            select new
                            {
                                id = p.Id,
                                itemCode = p.ItemCode,
                                name = p.Name,
                                description = p.Description ?? "",
                                category = p.Category ?? "",
                                subcategory = p.SubCategory ?? "",
                                line = p.Line ?? "",
                                imageUrl = p.ImageURL ?? "",
                                isDigital = p.IsDigital,
                                isNew = (lh != null && (lh.LogDate.AddDays(30) >= DateTime.Now)),
                                prices = from r in (p.ListPrices ?? new List<BEP.Price>())
                                         where r.Regular > 0 & (CardCode == HomeCardCode | beUser.AllowLinesBlocked | (l.FilterType == "NoneBut" & lstClientExceptions.Any(x => x.IdLine == l.Id)) | (l.FilterType == "AllBut" & !lstClientExceptions.Any(x => x.IdLine == l.Id)))
                                         select new
                                         {
                                             subsidiary = r.Sudsidiary.Name,
                                             price = (groupLineList.Any() || groupLists.Any()) & p?.Cost > 0 & l.Percentage >= 0 ? p.Cost.Value * (100 + l.Percentage) / 100 / 0.84m : r.Regular,
                                             offer = (from s in lstStock
                                                      where s.Subsidiary.ToLower() == r.Sudsidiary.Name.ToLower() & s.ItemCode.ToLower() == p.ItemCode.ToLower() &
                                                           (from w in lstWarehouses where w.ClientVisible | CardCode == HomeCardCode select w.Name.ToLower()).Contains(s.Warehouse.ToLower())
                                                      select s.Available2).Sum() > 0 ? p.ListPriceOffers?.Where(o => o.IdSubsidiary == r.IdSudsidiary)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Price ?? 0.0M : 0.0M,
                                         }
                            };
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("product")]
        public IActionResult Product(long Id, long UserId, string CardCode)
        {
            string message = "";
            try
            {
                BCP.PriceGroupLine bcGroupLine = new();
                var priceGroupLine = bcGroupLine.SearchByUser(UserId);

                BCP.PriceGroup bcGroup = new();
                var priceGroup = bcGroup.SearchByUser(UserId);

                BCP.Product bcProduct = new();
                BEP.Product beProduct;
                if (UserId > 0)
                {
                    beProduct = bcProduct.Search(Id, BEP.relProduct.Prices, BEP.relProduct.PriceOffers, BEP.relPrice.Sudsidiary, BEP.relProduct.VolumePricings, BEP.relVolumePricing.Subsidiary);
                }
                else
                {
                    beProduct = bcProduct.Search(Id);
                }
                beProduct.ListPrices ??= new List<BEP.Price>();
                beProduct.ListVolumePricings ??= new List<BEP.VolumePricing>();
                bool boLocal = false, boSeeStock = false, allowLinesBlocked = false;
                BCS.User bcUser = new();
                if (UserId > 0)
                {
                    BES.User beUser = bcUser.Search(UserId);
                    if (beUser?.Id > 0)
                    {
                        BCP.ClientAllowed bcClientAllowed = new();
                        boSeeStock = bcClientAllowed.IsAllowed(CardCode);
                        boLocal = HomeCardCode == CardCode;
                        allowLinesBlocked = beUser.AllowLinesBlocked;
                    }
                }

                BCA.ProductStock bcInvetory = new();
                List<Field> lstFilter = new()
                {
                    new Field("LOWER(ItemCode)", beProduct.ItemCode.ToLower()),
                    new Field("Stock", 0, Operators.HigherThan),
                    new Field("Requested", 0, Operators.HigherThan),
                    new Field("Reserved", 0, Operators.HigherThan),
                    new Field(LogicalOperators.Or),
                    new Field(LogicalOperators.Or),
                    new Field(LogicalOperators.And)
                };
                IEnumerable<BEA.ProductStock> lstInventory;
                try
                {
                    lstInventory = bcInvetory.List(lstFilter, "1");
                }
                catch (Exception)
                {
                    lstInventory = new List<BEA.ProductStock>();
                }

                BCP.WarehouseAllowed bcAllowed = new();
                IEnumerable<BEP.WarehouseAllowed> lstAllowed = bcAllowed.List("1");

                if (lstInventory == null)
                {
                    lstInventory = new List<BEA.ProductStock>();
                }
                else
                {
                    if (!boLocal & !boSeeStock)
                    {
                        lstInventory = (from i in lstInventory
                                        where (from a in lstAllowed where a.Subsidiary.ToLower() == i.Subsidiary.ToLower() & a.Name.ToLower() == i.Warehouse.ToLower() & (a.ClientVisible | boLocal) select a).Any()
                                                & (from w in lstAllowed select w.Name.ToLower()).Contains(i.Warehouse.ToLower())
                                        select i).ToList();
                    }
                    else
                    {
                        lstInventory = (from i in lstInventory
                                        join w in lstAllowed on new { s = i.Subsidiary.ToLower(), w = i.Warehouse.ToLower() } equals new { s = w.Subsidiary.ToLower(), w = w.Name.ToLower() }
                                        where w.ClientVisible | boLocal
                                        select i).ToList();
                    }
                }

                BCP.Line bcLine = new();
                var lines = bcLine.ListForPriceList("1", BEP.relLine.LineDetails);
                var line = lines.FirstOrDefault(x => x.ListLineDetails?.Any(y => y.SAPLine == beProduct.Line) ?? false);

                BCS.LineNotAllowed bcClient = new();
                List<Field> lstFilter2 = new() { new Field { Name = "CardCode", Value = CardCode } };
                var lstClientExceptions = bcClient.List(lstFilter2, "1");

                int? intValue = null;
                decimal percentage = priceGroupLine?.Percentage ?? priceGroup?.Percentage ?? 0;

                var item = new
                {
                    itemCode = beProduct.ItemCode,
                    name = beProduct.Name,
                    description = beProduct.Description?.Replace("\r", "\n") ?? "",
                    commentaries = beProduct.Commentaries?.Replace("\r", "\n") ?? "",
                    imageUrl = beProduct.ImageURL ?? "",
                    warranty = beProduct.Warranty ?? "",
                    link = beProduct.Link ?? "",
                    isNew = (beProduct.EnabledDate.HasValue && (beProduct.EnabledDate.Value.AddDays(30) >= DateTime.Now)),
                    isDigital = beProduct.IsDigital,
                    prices = from p in (beProduct.ListPrices ?? new List<BEP.Price>())
                             where (p.Regular > 0 /*| (p.Offer.HasValue && p.Offer.Value > 0)*/) & (boLocal | allowLinesBlocked | (line.FilterType == "NoneBut" & lstClientExceptions.Any(x => x.IdLine == line.Id)) | (line.FilterType == "AllBut" & !lstClientExceptions.Any(x => x.IdLine == line.Id)))
                             select new
                             {
                                 subsidiary = p.Sudsidiary.Name.ToLower(),
                                 price = (priceGroupLine != null || priceGroup != null) & beProduct.Cost > 0 & percentage >= 0 ? beProduct.Cost.Value * (100 + percentage) / 100 / 0.84m : p.Regular,
                                 observations = p.Observations ?? "",
                                 commentaries = boLocal ? (p.Commentaries ?? "") : "",
                                 offer = (from i in lstInventory where i.Subsidiary.ToLower() == p.Sudsidiary.Name.ToLower() select i.Available2).Sum() > 0 ? beProduct.ListPriceOffers?.Where(o => o.IdSubsidiary == p.IdSudsidiary)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Price ?? 0.0M : 0.0M,
                                 OfferCommentaries = lstInventory.Any(x => x.Subsidiary.ToLower() == p.Sudsidiary.Name.ToLower() & x.Available2 > 0) ? beProduct.ListPriceOffers?.Where(o => o.IdSubsidiary == p.IdSudsidiary)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Description ?? "" : "",
                                 volumen = boLocal ? from v in beProduct.ListVolumePricings
                                                     where p.IdSudsidiary == v.IdSubsidiary
                                                     select new { quantity = v.Quantity, price = v.Price, observations = v.Observations } : null,
                                 stock = from i in lstInventory
                                         where i.Subsidiary.ToLower() == p.Sudsidiary.Name.ToLower()
                                         select new
                                         {
                                             warehouse = i.Warehouse.ToLower(),
                                             percentage = GetPercentage(i.Stock, i.Reserved, i.Rotation),
                                             stock = (boLocal | boSeeStock) ? i.Stock : intValue,
                                             reserved = (boLocal | boSeeStock) ? i.Reserved : intValue,
                                             requested = (boLocal | boSeeStock) ? i.Requested : intValue,
                                             clientAllowed = lstAllowed.FirstOrDefault(x => x.Subsidiary.ToLower() == p.Sudsidiary.Name.ToLower() & x.Name.ToLower() == i.Warehouse.ToLower())?.ClientVisible ?? false
                                         }
                             }
                };
                return Ok(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        #endregion

        #region Private Methods

        private static IEnumerable<BEP.Product> GetItems(long? LineId, string Category, string Subcategory, string Filter, long UserId)
        {
            BCP.Product bcProduct = new();
            IEnumerable<BEP.Product> lstItems;
            if (UserId > 0)
            {
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(UserId, BES.relUser.Profile, BES.relProfile.ProfilePages);
                if (beUser?.Profile?.ListProfilePages?.Count > 0)
                {
                    if (beUser.Profile.ListProfilePages.Any(x => x.IdPage == 7 | x.IdPage == 16))
                    {
                        lstItems = bcProduct.ListWithPrices(LineId, Filter, Category, Subcategory, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers);
                    }
                    else
                    {
                        lstItems = bcProduct.ListWithPrices(LineId, Filter, Category, Subcategory, "Name");
                    }
                }
                else
                {
                    lstItems = bcProduct.ListWithPrices(LineId, Filter, Category, Subcategory, "Name");
                }
            }
            else
            {
                lstItems = bcProduct.ListWithPrices(LineId, Filter, Category, Subcategory, "Name");
            }
            return lstItems;
        }

        private static string GetPercentage(int Stock, int Reserved, string Rotation)
        {
            //decimal? decResult = null;
            //if (!string.IsNullOrWhiteSpace(Rotation))
            //{
            //    decResult = (Stock - Reserved) * 100;
            //    switch (Rotation.ToLower())
            //    {
            //        case "baja":
            //            decResult /= 10;
            //            break;
            //        case "media":
            //            decResult /= 50;
            //            break;
            //        case "intermedia":
            //            decResult /= 100;
            //            break;
            //        case "alta":
            //            decResult /= 500;
            //            break;
            //        default:
            //            decResult = null;
            //            break;
            //    }
            //    if (decResult.HasValue && decResult < 0) decResult = 0;
            //    if (decResult.HasValue && decResult > 100) decResult = 100;
            //}
            //return decResult;
            int available = Stock - Reserved;
            string result = available switch
            {
                < 0 => "0",
                > 50 => "+50",
                _ => available.ToString(),
            };
            return result;
        }

        #endregion
    }
}
