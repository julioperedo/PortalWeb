using BEntities.Filters;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUglify.Helpers;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BCP = BComponents.Product;
using BEP = BEntities.Product;
using Field = BEntities.Filters.Field;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class RelatedController : BaseController
    {
        #region Private Variables

        IWebHostEnvironment _env;

        #endregion

        #region Constructores

        public RelatedController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { _env = HEnviroment; }

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

        public IActionResult GetRelatedCategories()
        {
            string message = "";
            try
            {
                BCP.RelatedCategory bcCategories = new();
                var categories = bcCategories.List("1");
                var items = categories.Select(x => new { x.Id, x.Category, x.Related });
                return Json(new { message, items });
            }
            catch (System.Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetRelatedProducts()
        {
            string message = "";
            try
            {
                BCP.RelatedProduct bcProduct = new();
                var products = bcProduct.List("1", BEP.relRelatedProduct.Product, BEP.relRelatedProduct.Related);
                var items = products.Select(x => new { x.Id, x.IdProduct, x.IdRelated, ProductName = x.Product.Name, RelatedName = x.Related.Name, ProductCode = x.Product.ItemCode, RelatedCode = x.Related.ItemCode });
                return Json(new { message, items });
            }
            catch (System.Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetBanners()
        {
            string message = "";
            try
            {
                BCP.PromoBanner bcBanner = new();
                var banners = bcBanner.List("1");
                var items = banners.Select(x => new { x.Id, x.Name, x.ImageUrl, x.InitialDate, x.FinalDate, x.Enabled });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetUsedCategories()
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                var products = bcProduct.ListCategories();
                var items = products.Select(x => new { Name = x.Category });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetEnabledProducts()
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                List<Field> filters = new() { /*new Field("Enabled", 1)*/ };
                var products = bcProduct.List(filters, "1");
                var items = products.Select(x => new { x.Id, x.ItemCode, x.Name, FullName = $"{x.ItemCode} - {x.Name}" });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetBannerDetail(long Id)
        {
            string message = "";
            try
            {
                List<Field> filters = new() { new Field("IdPromo", Id) };

                BCP.PromoBannerItem bcItem = new();
                var bannerItems = bcItem.List(filters, "1", BEP.relPromoBannerItem.Subsidiary, BEP.relPromoBannerItem.Product);
                var items = bannerItems?.Select(x => new { x.Id, x.IdPromo, x.IdSubsidiary, x.IdProduct, SubsidiaryName = x.Subsidiary.Name, ProductName = x.Product.Name, ProductCode = x.Product.ItemCode, x.Price });

                BCP.PromoBannerTrigger bcTrigger = new();
                var bannerTrigers = bcTrigger.List(filters, "1", BEP.relPromoBannerTrigger.Product);
                var triggers = bannerTrigers?.Select(x => new { x.Id, x.IdPromo, x.IdProduct, x.Category, ProductName = x.Product?.Name, ProductCode = x.Product?.ItemCode });

                return Json(new { message, items, triggers });
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
        public IActionResult SaveCategory(BEP.RelatedCategory Item)
        {
            string message = "";
            try
            {
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;

                BCP.RelatedCategory bcRelated = new();
                bcRelated.Save(ref Item);
                return Json(new { message, Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteCategory(long Id)
        {
            string message = "";
            try
            {
                BCP.RelatedCategory bcCategory = new();
                BEP.RelatedCategory item = new() { Id = Id, StatusType = BEntities.StatusType.Delete };
                bcCategory.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SaveProduct(BEP.RelatedProduct Item)
        {
            string message = "";
            try
            {
                BCP.RelatedProduct bcProduct = new();
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;

                bcProduct.Save(ref Item);

                return Json(new { message, Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteProduct(long Id)
        {
            string message = "";
            try
            {
                BCP.RelatedProduct bcProduct = new();
                BEP.RelatedProduct item = new() { Id = Id, StatusType = BEntities.StatusType.Delete };
                bcProduct.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SaveBanner(BEP.PromoBanner Item)
        {
            string message = "";
            try
            {
                BCP.PromoBanner bcBanner = new();
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;

                bcBanner.Save(ref Item);

                return Json(new { message, Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteBanner(long Id)
        {
            string message = "";
            try
            {
                BCP.PromoBanner bcBanner = new();
                BEP.PromoBanner item = new() { Id = Id, StatusType = BEntities.StatusType.Delete };
                bcBanner.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SavePhotoBase64(string ImageBase64)
        {
            string message = "", fileName = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(ImageBase64))
                {
                    string strImage = ImageBase64.Split(',')[1];
                    fileName = $"Banner_{UserCode}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                    string virtualPath = Path.Combine("images", "promobanner", fileName);
                    var fullPath = _env.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;
                    byte[] bytes = Convert.FromBase64String(strImage);
                    System.IO.File.WriteAllBytes(fullPath, bytes);
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, fileName });
        }

        [HttpPost]
        public IActionResult SaveBannerItem(BEP.PromoBannerItem Item)
        {
            string message = "";
            try
            {
                BCP.PromoBannerItem bcBanner = new();
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;
                bcBanner.Save(ref Item);

                return Json(new { message, Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteBannerItem(long Id)
        {
            string message = "";
            try
            {
                BCP.PromoBannerItem bcBanner = new();
                BEP.PromoBannerItem item = new() { Id = Id, StatusType = BEntities.StatusType.Delete };
                bcBanner.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SaveBannerTrigger(BEP.PromoBannerTrigger Item)
        {
            string message = "";
            try
            {
                BCP.PromoBannerTrigger bcBanner = new();
                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;
                bcBanner.Save(ref Item);

                return Json(new { message, Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteBannerTrigger(long Id)
        {
            string message = "";
            try
            {
                BCP.PromoBannerTrigger bcBanner = new();
                BEP.PromoBannerTrigger item = new() { Id = Id, StatusType = BEntities.StatusType.Delete };
                bcBanner.Save(ref item);
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
