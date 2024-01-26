using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using BEntities.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCF = BComponents.Staff;
using BCK = BComponents.Kbytes;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEK = BEntities.Kbytes;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class ProductController : BaseController
    {
        #region Constructores

        public ProductController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                return View("Index2");
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetLines2()
        {
            BCP.Line bcLine = new();
            IEnumerable<BEP.Line> lstLines = new List<BEP.Line>();
            //lstLines.Insert(0, new BEP.Line { Id = 0, Name = "Ninguna" });
            if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
            {
                long idManager = 0;
                BCS.User bcUser = new();
                var beUser = bcUser.Search(UserCode, BES.relUser.UserDatas);
                BCF.Member bcMember = new();
                var member = bcMember.SearchByMail(beUser.EMail);
                idManager = member?.Id ?? 0;
                List<Field> filters = new() { new Field("IdManager", idManager) };
                lstLines = bcLine.List(filters, "Name");
            }
            else
            {
                lstLines = bcLine.List("Name");
            }
            var items = lstLines.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetCategories2(long? LineId)
        {
            IEnumerable<string> lstCategories = Enumerable.Empty<string>();
            if (LineId.HasValue)
            {
                BCP.Product bcProduct = new();
                lstCategories = bcProduct.ListCategories(LineId.Value);
            }
            var lstItems = lstCategories.Select(c => new { Name = c });
            return Json(lstItems);
        }

        public IActionResult GetSAPLines()
        {
            BCA.Line bcLine = new();
            IEnumerable<BEA.Item> lstLines = bcLine.List();
            var lstResult = from l in lstLines select new { l.Id, l.Name };
            return Json(lstResult);
        }

        public IActionResult GetSAPCategories()
        {
            BCA.Category bcCategory = new();
            IEnumerable<BEA.Item> lstCategories = bcCategory.List(new List<Field>(), "2");
            var lstResult = from l in lstCategories select new { l.Id, Name = ToTitle(l.Name) };
            return Json(lstResult);
        }

        public IActionResult GetSAPSubcategories(string Category)
        {
            BCA.Subcategory bcSubcategory = new();
            IEnumerable<BEA.Item> lstSubcategories = bcSubcategory.List(Category);
            var lstResult = from l in lstSubcategories select new { l.Id, Name = ToTitle(l.Name) };
            return Json(lstResult);
        }

        public IActionResult Filter(string Name, long? LineId, string CategoryId, string Enabled, string Offer)
        {
            string strMessage = "";
            IEnumerable<BEP.Product> lstItems = new List<BEP.Product>();
            try
            {
                lstItems = GetProducts(LineId, Name, CategoryId, Enabled, Offer);
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            var beResult = new { Message = strMessage, Items = lstItems };
            return Json(beResult);
        }

        public IActionResult Edit2(int Id, string ItemCode)
        {
            string message = "";
            try
            {
                BCP.Line bcLine = new();
                BEP.Line beLine = bcLine.SearchByProduct(Id);
                string externalSite = beLine?.ExternalSiteName?.Trim() ?? "";

                Models.ExternalPrice externalPrice = new();
                if (!string.IsNullOrWhiteSpace(externalSite))
                {
                    BCP.PriceExternalSite bcExPrice = new();
                    externalPrice = new Models.ExternalPrice(bcExPrice.SearchByProduct(Id)) ?? new Models.ExternalPrice();
                }

                BCP.Price bcPrice = new();
                List<Field> filters = new() { new Field("IdProduct", Id) };
                List<BEP.Price> lstPrices = bcPrice.List(filters, "1").ToList() ?? new List<Price>();

                BCP.VolumePricing bcVolume = new();
                var lstVolumePrices = bcVolume.List(filters, "1").ToList() ?? new List<VolumePricing>();
                BCB.Classifier bcClassifier = new();
                List<BEB.Classifier> lstSubsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");

                List<Models.SubsidiaryPrice> prices = new();
                IEnumerable<BEA.ProductStock> lstStock = new List<BEA.ProductStock>(), lstFifo = new List<BEA.ProductStock>();
                try
                {
                    if (!string.IsNullOrWhiteSpace(ItemCode))
                    {
                        BCA.ProductStock bcInventory = new();
                        filters = new List<Field> {
                            new("LOWER(ItemCode)", ItemCode.ToLower()), new("Stock", 0, Operators.HigherThan), new("Reserved", 0, Operators.HigherThan), new("Requested", 0, Operators.HigherThan),
                            new(LogicalOperators.Or), new(LogicalOperators.Or), new(LogicalOperators.And)
                        };
                        lstStock = bcInventory.List(filters, "1");
                        filters = new List<Field> { new("LOWER(ItemCode)", ItemCode.ToLower()), new("Stock", 0, Operators.HigherThan), new(LogicalOperators.And) };
                        lstFifo = bcInventory.ListBalance(filters, "1");
                    }
                }
                catch (Exception)
                {
                    lstStock = new List<BEA.ProductStock>();
                    lstFifo = new List<BEA.ProductStock>();
                }

                filters = new List<Field> { new Field("IdProduct", Id) };
                BCK.AcceleratorLot bcAcc = new();
                var accelerators = bcAcc.List(filters, "1");

                BCP.PriceOffer bcOffer = new();
                var offers = bcOffer.List(filters, "1") ?? new List<BEP.PriceOffer>();

                var volumePrices = from s in lstSubsidiaries
                                   select new
                                   {
                                       s.Id,
                                       Name = ToTitle(s.Name),
                                       Items = from v in lstVolumePrices
                                               where v.IdSubsidiary == s.Id
                                               select new { v.Id, v.IdSubsidiary, v.Price, v.Quantity, v.Observations },
                                       StockDetail = from i in lstStock where i.Subsidiary.ToLower() == s.Name.ToLower() orderby i.Warehouse select new Models.SubsidiaryStock(i),
                                       FifoDetail = from i in lstFifo where i.Subsidiary.ToLower() == s.Name.ToLower() orderby i.Warehouse, i.Date select new Models.SubsidiaryStock(i)
                                   };

                foreach (var beSub in lstSubsidiaries)
                {
                    Price beTempPrice = lstPrices.FirstOrDefault(p => p.IdSudsidiary == beSub.Id) ?? new Price();
                    Models.SubsidiaryPrice bePrice = new(beTempPrice)
                    {
                        IdSubsidiary = beSub.Id,
                        Subsidiary = ToTitle(beSub.Name),
                        StockDetail = (from i in lstStock where i.Subsidiary.ToLower() == beSub.Name.ToLower() orderby i.Warehouse select new Models.SubsidiaryStock(i)).ToList(),
                        FifoDetail = (from i in lstFifo where i.Subsidiary.ToLower() == beSub.Name.ToLower() orderby i.Warehouse, i.Date select new Models.SubsidiaryStock(i)).ToList(),
                        Offers = offers.Where(x => x.IdSubsidiary == beSub.Id).ToList(),
                    };
                    prices.Add(bePrice);
                }

                return Json(new { message, prices, volumePrices, externalSite, externalPrice, accelerators });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Edit(int id = -1)
        {
            if (id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                BEP.Product beTemp;
                string strExternalSite;
                if (id == 0)
                {
                    beTemp = new BEP.Product { Editable = true, ListAcceleratorLots = new List<BEK.AcceleratorLot>() };
                    strExternalSite = "";
                }
                else
                {
                    BCP.Product bcProduct = new();
                    beTemp = bcProduct.Search(id, relProduct.AcceleratorLots, relProduct.PriceOffers);

                    //BCK.AcceleratorLot bcAcc = new();
                    //var accs = bcAcc.List("1");

                    BCP.Line bcLine = new();
                    BEP.Line beLine = bcLine.SearchByProduct(id);
                    strExternalSite = beLine?.ExternalSiteName?.Trim();
                }
                Models.Product beProduct = new(beTemp) { ExternalSite = strExternalSite };
                if (!string.IsNullOrWhiteSpace(beTemp.ExtraComments)) beProduct.ExtraComments = SetHTMLSafe(beTemp.ExtraComments);
                if (!string.IsNullOrWhiteSpace(strExternalSite))
                {
                    BCP.PriceExternalSite bcExPrice = new();
                    beProduct.ExternalPrice = new Models.ExternalPrice(bcExPrice.SearchByProduct(id));
                    beProduct.ExternalPrice ??= new Models.ExternalPrice();
                }

                BCP.Price bcPrice = new();
                List<Field> lstFilter = new() { new Field("IdProduct", id) };
                List<BEP.Price> lstPrices = bcPrice.List(lstFilter, "1").ToList() ?? new List<Price>();
                BCP.VolumePricing bcVolume = new();
                beProduct.Volumen = bcVolume.List(lstFilter, "1").ToList();
                BCB.Classifier bcClassifier = new();
                List<BEB.Classifier> lstSubsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");

                IEnumerable<BEA.ProductStock> lstStock, lstFifo;
                try
                {
                    if (!string.IsNullOrWhiteSpace(beProduct.ItemCode))
                    {
                        BCA.ProductStock bcInventory = new();
                        lstFilter = new List<Field> {
                            new Field("LOWER(ItemCode)", beProduct.ItemCode.ToLower()), new Field("Stock", 0, Operators.HigherThan), new Field("Reserved", 0, Operators.HigherThan), new Field("Requested", 0, Operators.HigherThan),
                            new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.And)
                        };
                        lstStock = bcInventory.List(lstFilter, "1");
                        lstFilter = new List<Field> { new Field("ItemCode", beProduct.ItemCode), new Field("Stock", 0, Operators.HigherThan), new Field(LogicalOperators.And) };
                        lstFifo = bcInventory.ListBalance(lstFilter, "1");
                    }
                    else
                    {
                        lstStock = new List<BEA.ProductStock>();
                        lstFifo = new List<BEA.ProductStock>();
                    }
                }
                catch (Exception)
                {
                    lstStock = new List<BEA.ProductStock>();
                    lstFifo = new List<BEA.ProductStock>();
                }

                foreach (var beSub in lstSubsidiaries)
                {
                    Price beTempPrice = lstPrices.FirstOrDefault(p => p.IdSudsidiary == beSub.Id) ?? new Price();
                    Models.SubsidiaryPrice bePrice = new(beTempPrice)
                    {
                        IdSubsidiary = beSub.Id,
                        Subsidiary = ToTitle(beSub.Name),
                        StockDetail = (from i in lstStock where i.Subsidiary.ToLower() == beSub.Name.ToLower() orderby i.Warehouse select new Models.SubsidiaryStock(i)).ToList(),
                        FifoDetail = (from i in lstFifo where i.Subsidiary.ToLower() == beSub.Name.ToLower() orderby i.Warehouse, i.Date select new Models.SubsidiaryStock(i)).ToList()
                    };
                    beProduct.Prices.Add(bePrice);
                }

                return PartialView(beProduct.Editable ? "EditFull" : "Edit", beProduct);
            }
        }

        public IActionResult Detail(long Id = -1)
        {
            if (Id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                BEP.Product product;
                Models.Product item;
                BCP.ClientAllowed bcClientAllowed = new();

                ViewBag.SeeStock = bcClientAllowed.IsAllowed(CardCode) ? "Y" : "N";
                ViewBag.LocalUser = CardCode == HomeCardCode ? "Y" : "N";
                try
                {
                    BCB.Classifier bcClassifier = new();
                    List<BEB.Classifier> classifiers;
                    bool boPM = (long)BEE.Types.Profile.ProductManagement == ProfileCode, boSC = true, boIQ = true, boLA = true;
                    long IdManager = 0;
                    if (boPM)
                    {
                        BCF.Member bcMember = new();
                        var manager = bcMember.SearchByMail(EMail);
                        IdManager = manager?.Id ?? 0;
                    }
                    if (CardCode != HomeCardCode)
                    {
                        classifiers = bcClassifier.List((long)BEE.Classifiers.PriceListMinimunAmounts, "1");
                        decimal minSC, minIQ, minLA, totalSC, totalIQ, totalLA;
                        int months = int.Parse(classifiers.First(x => x.Name == "PL-MIN-MONTHS").Value);
                        minSC = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-SC").Value);
                        minIQ = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-IQ").Value);
                        minLA = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-LA").Value);

                        BCA.Client bcClientSAP = new();
                        var amounts = bcClientSAP.ListResumeAmounts(CardCode, DateTime.Today.AddMonths(-months));
                        totalSC = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "santa cruz")?.Balance ?? 0;
                        totalIQ = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "iquique")?.Balance ?? 0;
                        totalLA = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "miami")?.Balance ?? 0;

                        boSC = totalSC >= minSC;
                        boIQ = totalIQ >= minIQ;
                        boLA = totalLA >= minLA;
                    }

                    BCP.Product bcProduct = new();
                    product = bcProduct.Search(Id, relProduct.Prices, relProduct.VolumePricings, relProduct.PriceOffers) ?? new BEP.Product();
                    BCA.Product bcProductS = new();
                    BEA.Product p = bcProductS.SearchSC(product.ItemCode);

                    BCP.Line bcLine = new();
                    BEP.Line line = bcLine.SearchByProduct(Id);

                    item = new Models.Product(product, true) { Prices = new List<Models.SubsidiaryPrice>(), Volumen = new List<VolumePricing>(), FactoryCode = p.FactoryCode, Brand = p.Brand };
                    item.Offers = item.Offers.Where(x => (!x.Since.HasValue || x.Since.Value <= DateTime.Today) & (!x.Until.HasValue || x.Until.Value >= DateTime.Today)).ToList();
                    classifiers = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");
                    foreach (var subsidiary in classifiers)
                    {
                        var oPrice = product.ListPrices?.FirstOrDefault(x => x.IdSudsidiary == subsidiary.Id) ?? new Price { IdSudsidiary = subsidiary.Id, Sudsidiary = subsidiary };
                        Models.SubsidiaryPrice price = new(oPrice)
                        {
                            Subsidiary = ToTitle(subsidiary.Name),
                            LocalUser = CardCode == HomeCardCode,
                            Volumen = !boPM | (boPM & line.IdManager == IdManager) ? product.ListVolumePricings?.Where(x => x.IdSubsidiary == subsidiary.Id).ToList() : new List<VolumePricing>(),
                            Offer = !boPM | (boPM & line.IdManager == IdManager) ? item.Offers?.Where(o => o.IdSubsidiary == subsidiary.Id)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Price : null,
                            OfferDescription = !boPM | (boPM & line.IdManager == IdManager) ? item.Offers?.Where(o => o.IdSubsidiary == subsidiary.Id)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Description : null,
                            Regular = (subsidiary.Id == 1 & boSC) | (subsidiary.Id == 2 & boLA) | (subsidiary.Id == 3 & boIQ) ? oPrice.Regular : 0
                        };
                        item.Prices.Add(price);
                    }
                }
                catch (Exception)
                {
                    item = new Models.Product { Prices = new List<Models.SubsidiaryPrice>() };
                }
                return View(item);
            }
        }

        public IActionResult DetailPopUp(long Id)
        {
            BEP.Product product;
            Models.Product item;
            BCP.ClientAllowed bcClientAllowed = new();

            ViewBag.SeeStock = bcClientAllowed.IsAllowed(CardCode) ? "Y" : "N";
            ViewBag.LocalUser = CardCode == HomeCardCode ? "Y" : "N";
            try
            {
                BCP.Product bcProduct = new();
                product = bcProduct.Search(Id, relProduct.Prices, relProduct.VolumePricings, relProduct.PriceOffers) ?? new BEP.Product();
                item = new Models.Product(product, true) { Prices = new List<Models.SubsidiaryPrice>() };
                item.Offers = item.Offers.Where(x => (!x.Since.HasValue || x.Since.Value <= DateTime.Today) & (!x.Until.HasValue || x.Until.Value >= DateTime.Today)).ToList();
                BCB.Classifier bcClassifier = new();
                List<BEB.Classifier> classifiers;

                BCP.Line bcLine = new();
                BEP.Line line = bcLine.SearchByProduct(Id);

                bool boPM = (long)BEE.Types.Profile.ProductManagement == ProfileCode, boSC = true, boIQ = true, boLA = true;
                long IdManager = 0;
                if (boPM)
                {
                    BCF.Member bcMember = new();
                    var manager = bcMember.SearchByMail(EMail);
                    IdManager = manager?.Id ?? 0;
                }
                if (CardCode != HomeCardCode)
                {
                    classifiers = bcClassifier.List((long)BEE.Classifiers.PriceListMinimunAmounts, "1");
                    decimal minSC, minIQ, minLA, totalSC, totalIQ, totalLA;
                    int months = int.Parse(classifiers.First(x => x.Name == "PL-MIN-MONTHS").Value);
                    minSC = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-SC").Value);
                    minIQ = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-IQ").Value);
                    minLA = decimal.Parse(classifiers.First(x => x.Name == "PL-MIN-LA").Value);

                    BCA.Client bcClientSAP = new();
                    var amounts = bcClientSAP.ListResumeAmounts(CardCode, DateTime.Today.AddMonths(-months));
                    totalSC = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "santa cruz")?.Balance ?? 0;
                    totalIQ = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "iquique")?.Balance ?? 0;
                    totalLA = amounts.FirstOrDefault(x => x.Subsidiary.ToLower() == "miami")?.Balance ?? 0;

                    boSC = totalSC >= minSC;
                    boIQ = totalIQ >= minIQ;
                    boLA = totalLA >= minLA;
                }

                classifiers = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");

                foreach (var subsidiary in classifiers)
                {
                    var oPrice = product.ListPrices?.FirstOrDefault(x => x.IdSudsidiary == subsidiary.Id) ?? new Price { IdSudsidiary = subsidiary.Id, Sudsidiary = subsidiary };
                    Models.SubsidiaryPrice price = new(oPrice)
                    {
                        Subsidiary = ToTitle(subsidiary.Name),
                        LocalUser = CardCode == HomeCardCode,
                        Volumen = !boPM | (boPM & line.IdManager == IdManager) ? product.ListVolumePricings?.Where(x => x.IdSubsidiary == subsidiary.Id).ToList() : new List<VolumePricing>(),
                        Offer = !boPM | (boPM & line.IdManager == IdManager) ? item.Offers?.Where(o => o.IdSubsidiary == subsidiary.Id)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Price : null,
                        OfferDescription = !boPM | (boPM & line.IdManager == IdManager) ? item.Offers?.Where(o => o.IdSubsidiary == subsidiary.Id)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Description : null,
                        Regular = (subsidiary.Id == 1 & boSC) | (subsidiary.Id == 2 & boLA) | (subsidiary.Id == 3 & boIQ) ? oPrice.Regular : 0
                    };
                    item.Prices.Add(price);
                }
            }
            catch (Exception)
            {
                item = new Models.Product { Prices = new List<Models.SubsidiaryPrice>() };
            }
            return PartialView(item);
        }

        public IActionResult PriceHistory(long Id)
        {
            Models.PriceHistory beData = new();

            List<Field> lstFilter = new() { new Field("IdProduct", Id) };

            BCP.PriceHistory bcPrice = new();
            beData.Prices = bcPrice.List2(lstFilter, "LogDate DESC", BEP.relPriceHistory.Sudsidiary);

            BCP.VolumePricingHistory bcVolume = new();
            beData.Volume = bcVolume.List2(lstFilter, "LogDate DESC", BEP.relVolumePricingHistory.Subsidiary);

            return Json(beData);
        }

        public IActionResult ExportExcel(string Name, long? LineId, string CategoryId, string Enabled, string Offer)
        {
            var lstItems = (from i in GetProducts(LineId, Name, CategoryId, Enabled, Offer) orderby i.Line, i.Category select i).ToList();
            using ExcelPackage objExcel = new();
            ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
            FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

            wsMain.Name = "Productos";
            wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
            wsMain.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            wsMain.Cells.Style.Font.Size = 9;

            wsMain.Cells[1, 1].Value = "LISTADO DE PRODUCTOS";
            wsMain.Cells[1, 1].Style.Font.Size = 12;
            wsMain.Cells[1, 1].Style.Font.Bold = true;
            wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[1, 1, 1, 9].Merge = true;
            wsMain.Cells[4, 1, 4, 9].Merge = true;
            wsMain.Cells[4, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var imgLogo = wsMain.Drawings.AddPicture("logo", logo);
            imgLogo.SetPosition(5, 5);

            wsMain.Cells[6, 1, 7, 1].Merge = true;
            wsMain.Cells[6, 2, 7, 2].Merge = true;
            wsMain.Cells[6, 3, 7, 3].Merge = true;
            wsMain.Cells[6, 4, 6, 6].Merge = true;
            wsMain.Cells[6, 7, 6, 9].Merge = true;

            wsMain.Cells[6, 1].Value = "Item";
            wsMain.Cells[6, 2].Value = "Nombre";
            wsMain.Cells[6, 3].Value = "Habilitado";
            wsMain.Cells[6, 4].Value = "Precios";
            wsMain.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[6, 7].Value = "Stock";
            wsMain.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            wsMain.Cells[7, 4].Value = "Santa Cruz";
            wsMain.Cells[7, 5].Value = "Miami";
            wsMain.Cells[7, 6].Value = "Iquique";
            wsMain.Cells[7, 7].Value = "Santa Cruz";
            wsMain.Cells[7, 8].Value = "Miami";
            wsMain.Cells[7, 9].Value = "Iquique";

            wsMain.Cells[6, 1, 7, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGray);
            wsMain.Cells[6, 1, 7, 9].Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
            wsMain.Cells[6, 1, 7, 9].Style.Font.Bold = true;

            int row = 7;
            foreach (var item in lstItems)
            {
                row++;

                wsMain.Cells[row, 1].Value = item.ItemCode;
                wsMain.Cells[row, 2].Value = item.Name;
                wsMain.Cells[row, 3].Value = item.Enabled ? "Si" : "No";
                wsMain.Cells[row, 4].Value = item.SantaCruz;
                wsMain.Cells[row, 5].Value = item.Miami;
                wsMain.Cells[row, 6].Value = item.Iquique;
                wsMain.Cells[row, 7].Value = ReFormat(item.StockSantaCruz);
                wsMain.Cells[row, 8].Value = ReFormat(item.StockMiami);
                wsMain.Cells[row, 9].Value = ReFormat(item.StockIquique);
            }
            wsMain.Cells.AutoFitColumns();
            wsMain.Cells[8, 7, row, 9].Style.WrapText = true;

            // Assign borders
            var range = wsMain.Cells[6, 1, row, 9];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            byte[] objData = objExcel.GetAsByteArray();
            objExcel.Dispose();
            return File(objData, "application/xlsx", $"Listado-Productos-{DateTime.Now:yyyy-MM-dd-HH-mm}.xlsx");
        }

        public async Task<ActionResult> UploadImage(IEnumerable<IFormFile> files)
        {
            // The Name of the Upload component is "files"
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                        var physicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "products", fileName);

                        using var fileStream = new FileStream(physicalPath, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception) { }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DeleteImage(string[] fileNames)
        {
            foreach (var strFileName in fileNames)
            {
                string strPhysicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "products", strFileName);
                if (System.IO.File.Exists(strPhysicalPath))
                {
                    System.IO.File.Delete(strPhysicalPath);
                }
            }
            return Content("");
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public IActionResult Edit(Models.Product Item, Models.ProductFilters Filters)
        {
            string strMessage = "";
            IEnumerable<BEP.Product> lstItems = new List<BEP.Product>();
            try
            {
                BCP.Product bcProduct = new();
                List<Field> lstFilter = new() { new Field("ItemCode", Item.ItemCode), new Field("Id", Item.Id, Operators.Different), new Field(LogicalOperators.And) };
                IEnumerable<BEP.Product> lstTemp = bcProduct.List(lstFilter, "1");

                if (lstTemp?.Count() > 0)
                {
                    strMessage = "Ya existe un producto registrado con ese Código";
                }
                else
                {
                    DateTime logDate = DateTime.Now;

                    if (Item.Lots?.Count > 0)
                    {
                        Item.Lots.ForEach(x =>
                        {
                            x.LogUser = UserCode;
                            x.LogDate = DateTime.Now;
                        });
                    }

                    BEP.Product beProduct = Item.ToEntity(UserCode, logDate);
                    beProduct.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                    beProduct.ImageURL = string.IsNullOrWhiteSpace(Item.NewImageURL) ? Item.ImageURL : (Item.NewImageURL == "None" ? "" : Item.NewImageURL);

                    beProduct.ListPrices = new List<BEP.Price>();
                    foreach (Models.SubsidiaryPrice price in Item.Prices)
                    {
                        BEP.Price bePrice = price.ToEntity(UserCode, logDate);
                        if (bePrice.Id == 0)
                        {
                            if (bePrice.HasData()) bePrice.StatusType = BEntities.StatusType.Insert;
                        }
                        else
                        {
                            bePrice.StatusType = bePrice.HasData() ? BEntities.StatusType.Update : BEntities.StatusType.Delete;
                        }
                        if (bePrice.StatusType != BEntities.StatusType.NoAction) beProduct.ListPrices.Add(bePrice);
                    }

                    if (Item.Volumen?.Count > 0)
                    {
                        foreach (var vol in Item.Volumen)
                        {
                            vol.LogUser = UserCode;
                            vol.LogDate = logDate;
                        }
                        beProduct.ListVolumePricings = Item.Volumen;
                    }

                    if (Item.ExternalPrice != null)
                    {
                        beProduct.ListPriceExternalSites = new List<BEP.PriceExternalSite>();
                        BEP.PriceExternalSite beExternalPrice = Item.ExternalPrice.ToEntity(UserCode, logDate);
                        if (Item.ExternalPrice.HasData())
                        {
                            beExternalPrice.StatusType = beExternalPrice.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                        }
                        else
                        {
                            if (beExternalPrice.Id > 0) beExternalPrice.StatusType = BEntities.StatusType.Delete;
                        }
                        if (beExternalPrice.StatusType != BEntities.StatusType.NoAction) beProduct.ListPriceExternalSites.Add(beExternalPrice);
                    }

                    bcProduct.Save(ref beProduct);
                    lstItems = GetProducts(Filters.LineId, Filters.Name, Filters.CategoryId, Filters.Enabled, Filters.Offer);
                }
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            var beResult = new { Message = strMessage, Items = lstItems };
            return Json(beResult);
        }

        [HttpPost()]
        public IActionResult Edit2(Models.Product Item, Models.ProductFilters Filters)
        {
            string strMessage = "";
            IEnumerable<BEP.Product> lstItems = new List<BEP.Product>();
            try
            {
                BCP.Product bcProduct = new();
                List<Field> lstFilter = new() { new Field("ItemCode", Item.ItemCode), new Field("Id", Item.Id, Operators.Different), new Field(LogicalOperators.And) };
                IEnumerable<BEP.Product> lstTemp = bcProduct.List(lstFilter, "1");

                if (lstTemp?.Count() > 0)
                {
                    strMessage = "Ya existe un producto registrado con ese Código";
                }
                else
                {
                    DateTime logDate = DateTime.Now;

                    if (Item.Lots?.Count > 0)
                    {
                        Item.Lots.ForEach(x =>
                        {
                            x.LogUser = UserCode;
                            x.LogDate = logDate;
                        });
                    }

                    BEP.Product beProduct = Item.ToEntity(UserCode, logDate);
                    beProduct.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                    beProduct.ImageURL = string.IsNullOrWhiteSpace(Item.NewImageURL) ? Item.ImageURL : (Item.NewImageURL == "None" ? "" : Item.NewImageURL);

                    beProduct.ListPrices = new List<BEP.Price>();
                    foreach (Models.SubsidiaryPrice price in Item.Prices)
                    {
                        BEP.Price bePrice = price.ToEntity(UserCode, logDate);
                        if (bePrice.Id == 0)
                        {
                            if (bePrice.HasData()) bePrice.StatusType = BEntities.StatusType.Insert;
                        }
                        else
                        {
                            bePrice.StatusType = bePrice.HasData() ? BEntities.StatusType.Update : BEntities.StatusType.Delete;
                        }
                        if (bePrice.StatusType != BEntities.StatusType.NoAction) beProduct.ListPrices.Add(bePrice);
                    }

                    if (Item.Volumen?.Count > 0)
                    {
                        foreach (var vol in Item.Volumen)
                        {
                            vol.LogUser = UserCode;
                            vol.LogDate = logDate;
                        }
                        beProduct.ListVolumePricings = Item.Volumen;
                    }

                    if (Item.ExternalPrice != null)
                    {
                        beProduct.ListPriceExternalSites = new List<BEP.PriceExternalSite>();
                        BEP.PriceExternalSite beExternalPrice = Item.ExternalPrice.ToEntity(UserCode, logDate);
                        if (Item.ExternalPrice.HasData())
                        {
                            beExternalPrice.StatusType = beExternalPrice.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                        }
                        else
                        {
                            if (beExternalPrice.Id > 0) beExternalPrice.StatusType = BEntities.StatusType.Delete;
                        }
                        if (beExternalPrice.StatusType != BEntities.StatusType.NoAction) beProduct.ListPriceExternalSites.Add(beExternalPrice);
                    }

                    beProduct.ListPriceOffers = Item.Offers?.Count > 0 ? Item.Offers.Select(x => x.ToEntity(UserCode, logDate)).ToList() : new();

                    bcProduct.Save(ref beProduct);
                    lstItems = GetProducts(Filters.LineId, Filters.Name, Filters.CategoryId, Filters.Enabled, Filters.Offer);
                }
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            var beResult = new { Message = strMessage, Items = lstItems };
            return Json(beResult);
        }

        [HttpPost()]
        public IActionResult Delete(long Id, Models.ProductFilters Filters)
        {
            string strMessage = "";
            IEnumerable<BEP.Product> lstItems = new List<BEP.Product>();
            try
            {
                BEP.Product beProduct = new() { Id = Id, StatusType = BEntities.StatusType.Delete, LogDate = DateTime.Now };
                BCP.Product bcProduct = new();
                bcProduct.Save(ref beProduct);
                lstItems = GetProducts(Filters.LineId, Filters.Name, Filters.CategoryId, Filters.Enabled, Filters.Offer);
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            var beResult = new { Message = strMessage, Items = lstItems };
            return Json(beResult);
        }

        [HttpPost()]
        public IActionResult Sync(long Id, Models.ProductFilters Filters)
        {
            string strMessage = "";
            IEnumerable<BEP.Product> lstItems = new List<BEP.Product>();
            bool boError = false;
            try
            {
                BCA.Product bcProductSAP = new();
                BCP.Product bcProduct = new();
                BEA.Product sapProduct = null;
                BEP.Product Item = bcProduct.Search(Id);
                if (Item?.Id > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Item.ItemCode))
                    {
                        sapProduct = bcProductSAP.SearchSC(Item.ItemCode) ?? bcProductSAP.SearchIQQ(Item.ItemCode) ?? bcProductSAP.SearchLA(Item.ItemCode);
                        Item.StatusType = BEntities.StatusType.Update;
                        Item.LogUser = UserCode;
                        Item.LogDate = DateTime.Now;
                        if (sapProduct != null)
                        {
                            Item.Name = sapProduct.Name;
                            Item.Category = ToTitle(sapProduct.Category);
                            Item.SubCategory = ToTitle(sapProduct.Subcategory);
                            Item.Line = sapProduct.Line;
                            Item.Description = sapProduct.Description;
                            Item.Commentaries = sapProduct.Commentaries;
                            Item.Warranty = sapProduct.Warranty;
                            Item.Enabled = true;
                            Item.Editable = false;
                            if (!string.IsNullOrWhiteSpace(sapProduct.Picture) & !string.IsNullOrWhiteSpace(sapProduct.FilePath))
                            {
                                Item.ImageURL ??= "";
                                if (Item.ImageURL.Trim() != sapProduct.Picture.Trim())
                                {
                                    string strPath = Path.Combine(rootDirectory, "wwwroot", "images", "products"), strImageName = ImageName(sapProduct.Picture);
                                    if (System.IO.File.Exists(Path.Combine(sapProduct.FilePath, sapProduct.Picture)))
                                    {
                                        Item.ImageURL = strImageName;
                                        if (!System.IO.File.Exists(Path.Combine(strPath, strImageName)))
                                        {
                                            if (SaveImage(sapProduct.FilePath, sapProduct.Picture, strPath) == false)
                                            {
                                                Item.ImageURL = null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Item.Enabled = false;
                            Item.Editable = true;
                            strMessage = $"No se ha podido encontrar un producto en ninguna de las sucursales con el siguiente código: {Item.ItemCode}, y será deshabilitado.";
                        }
                        bcProduct.Save(ref Item);
                    }
                    else
                    {
                        strMessage = "No se puede sincronizar este producto porque no tiene ningún Código de Producto.";
                    }
                }
                else
                {
                    strMessage = "No se ha podido recuperar la información del Producto.";
                }
                lstItems = GetProducts(Filters.LineId, Filters.Name, Filters.CategoryId, Filters.Enabled, Filters.Offer);
            }
            catch (Exception ex)
            {
                boError = true;
                strMessage += $"Se ha producido el siguiente error al sincronizar el producto: <br /> {GetError(ex)}";
            }
            return Json(new { Message = strMessage, Items = lstItems, Error = boError });
        }

        #endregion

        #region Metodos Privados

        private IEnumerable<BEP.Product> GetProducts(long? IdLine, string Name, string CategoryId, string Enabled, string Offer)
        {
            IEnumerable<BEP.Product> lstItems = new List<BEP.Product>();

            BCP.Product bcProduct = new();
            List<Field> lstFilter = new();
            if (Enabled != "A")
            {
                lstFilter.Add(new Field("Enabled", Enabled == "V" ? 1 : 0));
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                var values = Name.Split(',');
                foreach (var item in values)
                {
                    lstFilter.Add(new Field("LOWER(Name)", item.Trim().ToLower(), Operators.Likes));
                }
                for (int i = 1; i < values.Length; i++)
                {
                    lstFilter.Add(new Field(LogicalOperators.And));
                }
                foreach (var item in values)
                {
                    lstFilter.Add(new Field("LOWER(ItemCode)", item.Trim().ToLower(), Operators.Likes));
                }
                for (int i = 1; i < values.Length; i++)
                {
                    lstFilter.Add(new Field { LogicalOperator = LogicalOperators.And });
                }
                lstFilter.Add(new Field { LogicalOperator = LogicalOperators.Or });
            }
            if (IdLine.HasValue)
            {
                lstFilter.Add(new Field("Line", $"SELECT SAPLine FROM Product.LineDetail WHERE IdLine = {IdLine}", Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(CategoryId))
            {
                lstFilter.Add(new Field("Category", CategoryId));
            }
            switch (Offer)
            {
                case "V":
                    lstFilter.Add(new Field("Id", "SELECT IdProduct FROM Product.PriceOffer WHERE IdProduct = Product.Id AND ISNULL(Price, 0) > 0", Operators.In));
                    break;
                case "F":
                    lstFilter.Add(new Field("Id", "SELECT IdProduct FROM Product.PriceOffer WHERE IdProduct = Product.Id AND ISNULL(Price, 0) > 0", Operators.NotIn));
                    break;
                default:
                    break;
            }

            CompleteFilters(ref lstFilter);
            lstItems = bcProduct.List(lstFilter, "ItemCode", BEP.relProduct.Prices);
            if (lstItems?.Count() > 0)
            {
                BCA.ProductStock bcStock = new();
                lstFilter = new List<Field> {
                    new Field("LOWER(ItemCode)", string.Join(",", lstItems.Select(x => $"'{x.ItemCode.ToLower()}'").Distinct()), Operators.In), new Field("Stock", 0, Operators.HigherThan), new Field("Requested", 0, Operators.HigherThan),
                    new Field("Reserved", 0, Operators.HigherThan), new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.And)
                };
                IEnumerable<BEA.ProductStock> lstInventory = bcStock.List(lstFilter, "1");

                if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
                {
                    BCP.Line bcLine = new();
                    lstFilter = new();
                    long idManager = 0;
                    BCS.User bcUser = new();
                    var beUser = bcUser.Search(UserCode, BES.relUser.UserDatas);
                    BCF.Member bcMember = new();
                    var member = bcMember.SearchByMail(beUser.EMail);
                    idManager = member?.Id ?? 0;
                    lstFilter.Add(new Field("IdManager", idManager));

                    var lines = bcLine.List(lstFilter, "1", BEP.relLine.LineDetails);
                    lstItems = lstItems.Where(x => lines.Any(l => l.ListLineDetails.Any(d => (d.SAPLine?.ToLower() ?? "") == (x.Line?.ToLower() ?? ""))));
                }
                foreach (var beProduct in lstItems)
                {
                    if (beProduct.Line == null || beProduct.Line.Trim() == "")
                    {
                        beProduct.Line = "Sin Asignar";
                    }
                    if (beProduct.Category == null || beProduct.Category.Trim().Length == 0)
                    {
                        beProduct.Category = "Sin Asignar";
                    }
                    if ((from p in beProduct.ListPrices where p.IdSudsidiary == 1 select p).Any())
                    {
                        beProduct.SantaCruz = (from p in beProduct.ListPrices where p.IdSudsidiary == 1 select p.Regular).FirstOrDefault();
                    }
                    if ((from p in beProduct.ListPrices where p.IdSudsidiary == 2 select p).Any())
                    {
                        beProduct.Miami = (from p in beProduct.ListPrices where p.IdSudsidiary == 2 select p.Regular).FirstOrDefault();
                    }
                    if ((from p in beProduct.ListPrices where p.IdSudsidiary == 3 select p).Any())
                    {
                        beProduct.Iquique = (from p in beProduct.ListPrices where p.IdSudsidiary == 3 select p.Regular).FirstOrDefault();
                    }

                    List<BEA.ProductStock> lstTemp = (from i in lstInventory where i.ItemCode.Trim().ToLower() == beProduct.ItemCode.Trim().ToLower() select i).ToList();
                    beProduct.StockSantaCruz = "";
                    foreach (var item in (from t in lstTemp where t.Subsidiary.Trim().ToLower() == "santa cruz" select t).ToList())
                    {
                        if (item.Stock > 0 | item.Reserved > 0)
                        {
                            beProduct.StockSantaCruz += $"{ToTitle(item.Warehouse)}: {item.Stock} - {item.Reserved}<br />";
                        }
                        if (item.Requested > 0)
                        {
                            beProduct.StockSantaCruz += $"{ToTitle(item.Warehouse)} (Pedido): {item.Requested}<br />";
                        }
                    }
                    beProduct.StockMiami = "";
                    foreach (var item in (from t in lstTemp where t.Subsidiary.Trim().ToLower() == "miami" select t).ToList())
                    {
                        if (item.Stock > 0 | item.Reserved > 0)
                        {
                            beProduct.StockMiami += $"{ToTitle(item.Warehouse)}: {item.Stock} - {item.Reserved}<br />";
                        }
                        if (item.Requested > 0)
                        {
                            beProduct.StockMiami += $"{ToTitle(item.Warehouse)} (Pedido): {item.Requested}<br />";
                        }
                    }
                    beProduct.StockIquique = "";
                    foreach (var item in (from t in lstTemp where t.Subsidiary.Trim().ToLower() == "iquique" select t).ToList())
                    {
                        if (item.Stock > 0 | item.Reserved > 0)
                        {
                            beProduct.StockIquique += $"{ToTitle(item.Warehouse)}: {item.Stock} - {item.Reserved}<br />";
                        }
                        if (item.Requested > 0)
                        {
                            beProduct.StockIquique += $"{ToTitle(item.Warehouse)} (Pedido): {item.Requested}<br />";
                        }
                    }
                }
            }
            return lstItems;
        }

        private static bool SaveImage(string FilePath, string FileName, string DestinyPath)
        {
            bool boResult = true;
            try
            {
                var valids = new[] { "gif", "jpg", "png" };
                using Image image = Image.Load(Path.Combine(FilePath, FileName));
                int newWidth, newHeight, originalWidth = image.Width, originalHeight = image.Height;
                float percentWidth = 1440 / (float)originalWidth;
                float percentHeight = 1080 / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);

                image.Mutate(x => x.Resize(newWidth, newHeight));

                string strExt = FileName.Split('.').Last().ToLower(), strName = !valids.Contains(strExt) ? FileName.Replace($".{strExt}", ".jpg") : FileName;
                image.Save(Path.Combine(DestinyPath, ImageName(strName)));
            }
            catch (Exception)
            {
                boResult = false;
            }
            return boResult;
        }

        private static string ImageName(string Name)
        {
            string strResult = "", strExt;
            if (Name != null)
            {
                strResult = Name.Replace("#", "").Replace(" ", "-").Replace("+", "-");
                var valids = new[] { "gif", "jpg", "png" };
                strExt = strResult.Split('.').Last().ToLower();
                if (!valids.Contains(strExt))
                {
                    strResult = strResult.Replace("." + strExt, ".jpg");
                }
            }
            return strResult;
        }

        private static string ReFormat(string Stock)
        {
            string result = Stock;
            if (result.EndsWith("<br />")) result = result[..(Stock.Length - 6)];
            result = result.Replace("<br />", Environment.NewLine);
            return result;
        }

        #endregion
    }
}