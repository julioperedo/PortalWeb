using BEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCP = BComponents.Product;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class UpDownPricesController : BaseController
    {

        #region Constructores

        public UpDownPricesController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

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

        public async Task<IActionResult> GetProductsAsync(string LineIds)
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                var products = await bcProduct.ListWithPricesAsync(LineIds, "1", BEP.relProduct.Prices, BEP.relProduct.PriceOffers);
                decimal? decNull = null;
                var items = from p in products
                            orderby p.LineName, p.Category, p.SubCategory, p.ItemCode
                            where p.Enabled
                            select new
                            {
                                p.Id,
                                Line = p.LineName,
                                p.Category,
                                Subcategory = p.SubCategory,
                                p.ItemCode,
                                p.Name,
                                PriceSA = p.ListPrices.FirstOrDefault(x => x.IdSudsidiary == (long)BEE.Subsidiaries.SantaCruz)?.Regular ?? decNull,
                                PriceLA = p.ListPrices.FirstOrDefault(x => x.IdSudsidiary == (long)BEE.Subsidiaries.Miami)?.Regular ?? decNull,
                                PriceIQ = p.ListPrices.FirstOrDefault(x => x.IdSudsidiary == (long)BEE.Subsidiaries.Iquique)?.Regular ?? decNull,
                                OfferSA = p.CurrentOffers.FirstOrDefault(x => x.IdSubsidiary == (long)BEE.Subsidiaries.SantaCruz)?.Price ?? decNull,
                                OfferLA = p.CurrentOffers.FirstOrDefault(x => x.IdSubsidiary == (long)BEE.Subsidiaries.Miami)?.Price ?? decNull,
                                OfferIQ = p.CurrentOffers.FirstOrDefault(x => x.IdSubsidiary == (long)BEE.Subsidiaries.Iquique)?.Price ?? decNull
                            };
                return Json(new { message, items });
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
        public async Task<IActionResult> SavePricesAsync(string LineIds, decimal RegularSA, decimal OfferSA, decimal VolumeSA, decimal RegularIQ, decimal OfferIQ, decimal VolumeIQ, decimal RegularLA, decimal OfferLA, decimal VolumeLA, string RoundType)
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                var products = await bcProduct.ListWithPricesAsync(LineIds, "1", BEP.relProduct.Prices, BEP.relProduct.PriceOffers, BEP.relProduct.VolumePricings);
                BCP.Price bcPrice = new();
                BCP.PriceOffer bcOffer = new();
                BCP.VolumePricing bcVolumePricing = new();
                decimal percent = 0;
                const long saId = (long)BEE.Subsidiaries.SantaCruz, iqId = (long)BEE.Subsidiaries.Iquique;

                foreach (var p in products)
                {
                    if (p.ListPrices?.Count > 0)
                    {
                        foreach (var r in p.ListPrices)
                        {
                            percent = r.IdSudsidiary switch { saId => RegularSA, iqId => RegularIQ, _ => RegularLA };
                            if (percent != 0)
                            {
                                r.Regular *= (100 + percent) / 100;
                                if (RoundType != "N") r.Regular = RoundType == "U" ? Math.Ceiling(r.Regular) : Math.Floor(r.Regular);
                                r.StatusType = StatusType.Update;
                                r.LogUser = UserCode;
                                r.LogDate = DateTime.Now;
                                await bcPrice.SaveAsync(r);
                            }
                        }
                    }
                    if (p.CurrentOffers?.Count() > 0)
                    {
                        foreach (var o in p.CurrentOffers)
                        {
                            percent = o.IdSubsidiary switch { saId => OfferSA, iqId => OfferIQ, _ => OfferLA };
                            if (percent != 0)
                            {
                                o.Price *= (100 + percent) / 100;
                                if (RoundType != "N") o.Price = RoundType == "U" ? Math.Ceiling(o.Price) : Math.Floor(o.Price);
                                o.StatusType = StatusType.Update;
                                o.LogUser = UserCode;
                                o.LogDate = DateTime.Now;
                                await bcOffer.SaveAsync(o);
                            }
                        }
                    }
                    if(p.ListVolumePricings?.Count > 0)
                    {
                        foreach (var v in p.ListVolumePricings)
                        {
                            percent = v.IdSubsidiary switch { saId => VolumeSA, iqId => VolumeIQ, _ => VolumeLA };
                            if (percent != 0)
                            {
                                v.Price *= (100 + percent) / 100;
                                if (RoundType != "N") v.Price = RoundType == "U" ? Math.Ceiling(v.Price) : Math.Floor(v.Price);
                                v.StatusType = StatusType.Update;
                                v.LogUser = UserCode;
                                v.LogDate = DateTime.Now;
                                await bcVolumePricing.SaveAsync(v);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
