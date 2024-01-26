using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BEP = BEntities.Product;
using BEA = BEntities.SAP;
using System.Linq;
using System;
using System.Text;
using System.Security.Cryptography.Xml;
using Portal.Areas.Product.Models;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class OffersBannerController : BaseController
    {
        #region Variables Globales

        private readonly IWebHostEnvironment _env;

        #endregion

        #region Constructores

        public OffersBannerController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment)
        {
            _env = HEnviroment;
        }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOffers()
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                List<Field> filters = new() { new Field("Enabled", 1), new Field("ShowTeamOnly", 0), new Field(LogicalOperators.And) };
                IEnumerable<BEP.Product> products = bcProduct.ListWithOffer(filters, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers);

                IEnumerable<BEA.ProductStock> stock;
                if (products?.Count() > 0)
                {
                    BCP.WarehouseAllowed bcAllowed = new();
                    filters = new List<Field> { new Field("Subsidiary", "Santa Cruz"), new Field("ClientVisible", 1), new Field(LogicalOperators.And) };
                    IEnumerable<BEP.WarehouseAllowed> warehousesAllowed = bcAllowed.List(filters, "1");
                    string whAllowed = string.Join(",", warehousesAllowed.Select(x => $"'{x.Name.ToLower()}'"));

                    BCA.ProductStock bcSTock = new();
                    string itemCodes = string.Join(",", products.Select(x => $"'{x.ItemCode.ToLower()}'").Distinct());
                    filters = new List<Field> {
                        new Field("LOWER(ItemCode)", itemCodes, Operators.In), new Field("Subsidiary", "Santa Cruz"), new Field("LOWER(Warehouse)", whAllowed, Operators.In),
                        new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                    };
                    stock = bcSTock.List(filters, "1");
                    var items = from i in products
                                where stock.Where(x => x.ItemCode.ToLower() == i.ItemCode.ToLower()).Sum(x => x.Available2) > 0
                                select new
                                {
                                    i.Id,
                                    i.Name,
                                    i.ItemCode,
                                    i.Line,
                                    i.ImageURL,
                                    i.ShowTeamOnly,
                                    ListPrices = i.ListPrices.Select(x => new { x.Regular, x.Sudsidiary, Offer = i.CurrentOffers.FirstOrDefault(o => o.IdSubsidiary == x.IdSudsidiary)?.Price ?? 0 })
                                };
                    return Json(new { message, items });
                }
            }
            catch (System.Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetNewProducts()
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                List<Field> filters = new() { new Field("Enabled", 1), new Field("ShowTeamOnly", 0), new Field(LogicalOperators.And) };
                IEnumerable<BEP.Product> products = bcProduct.ListWithPricesAndNew(30, filters, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers);

                IEnumerable<BEA.ProductStock> stock;
                if (products?.Count() > 0)
                {
                    BCP.WarehouseAllowed bcAllowed = new();
                    filters = new List<Field> { new Field("Subsidiary", "Santa Cruz"), new Field("ClientVisible", 1), new Field(LogicalOperators.And) };
                    IEnumerable<BEP.WarehouseAllowed> warehousesAllowed = bcAllowed.List(filters, "1");
                    string whAllowed = string.Join(",", warehousesAllowed.Select(x => $"'{x.Name.ToLower()}'"));

                    BCA.ProductStock bcSTock = new();
                    string itemCodes = string.Join(",", products.Select(x => $"'{x.ItemCode.ToLower()}'").Distinct());
                    filters = new List<Field> {
                        new Field("LOWER(ItemCode)", itemCodes, Operators.In), new Field("Subsidiary", "Santa Cruz"), new Field("LOWER(Warehouse)", whAllowed, Operators.In),
                        new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                    };
                    stock = bcSTock.List(filters, "1");
                    var items = from i in products
                                where stock.Where(x => x.ItemCode.ToLower() == i.ItemCode.ToLower()).Sum(x => x.Available2) > 0
                                select new
                                {
                                    i.Id,
                                    i.Name,
                                    i.ItemCode,
                                    i.Line,
                                    i.ImageURL,
                                    i.ShowTeamOnly,
                                    ListPrices = i.ListPrices.Select(x => new { x.Regular, x.Sudsidiary, Offer = i.CurrentOffers.FirstOrDefault(o => o.IdSubsidiary == x.IdSudsidiary)?.Price ?? 0 })
                                };
                    return Json(new { message, items });
                }
            }
            catch (System.Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetAllProducts()
        {
            string message = "";
            try
            {
                BCP.Product bcProduct = new();
                List<Field> filters = new() { new Field("Enabled", 1) };
                IEnumerable<BEP.Product> products = bcProduct.ListWithPrices(filters, "Name", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers);

                if (products?.Count() > 0)
                {
                    var items = from i in products
                                select new
                                {
                                    i.Id,
                                    i.Name,
                                    i.ItemCode,
                                    i.Line,
                                    i.ImageURL,
                                    i.ShowTeamOnly,
                                    ListPrices = i.ListPrices.Select(x => new { x.Regular, x.Sudsidiary, Offer = i.CurrentOffers.FirstOrDefault(o => o.IdSubsidiary == x.IdSudsidiary)?.Price ?? 0 })
                                };
                    return Json(new { message, items });
                }
            }
            catch (System.Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpGet]
        public async Task<IActionResult> GenerateFile(string Codes, string Subsidiaries, string ExportType, string ValidateStock)
        {
            string subsidiaries = Subsidiaries.Replace("sa", "santa cruz").Replace("la", "miami").Replace("iq", "iquique");
            bool IsLocal = CardCode == HomeCardCode;

            BCP.Product bcProduct = new();
            List<Field> filters = new() { new Field("Id", Codes, Operators.In) };
            var allProducts = bcProduct.List(filters, "1", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers);
            var newProducts = bcProduct.ListWithPricesAndNew(30, new List<Field>(), "1");
            IEnumerable<BEA.ProductStock> stock = new List<BEA.ProductStock>();
            string codes = "";

            if (allProducts.Any())
            {
                BCA.ProductStock bcStock = new();
                codes = string.Join(",", allProducts.Select(x => $"'{x.ItemCode.ToLower()}'").Distinct());

                filters = new List<Field>
                {
                    new Field("LOWER(ItemCode)", codes, Operators.In), new Field("Available2", 0, Operators.HigherThan), new Field("LOWER(Subsidiary)", subsidiaries, Operators.In),
                    new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                };
                stock = bcStock.List(filters, "1");
            }

            List<Product.Models.Product> products = new();
            foreach (var p in allProducts)
            {
                Product.Models.Product newProd = new(p) { Prices = new(), IsNew = newProducts.Any(x => x.Id == p.Id) };
                foreach (var r in p.ListPrices)
                {
                    if (r.Regular > 0 & subsidiaries.Contains(r.Sudsidiary.Name.ToLower()))
                    {
                        BEP.PriceOffer offer = p.CurrentOffers.FirstOrDefault(x => x.IdSubsidiary == r.IdSudsidiary);
                        var price = new SubsidiaryPrice(r) { Offer = offer?.Price, OfferDescription = offer?.Description };
                        newProd.Prices.Add(price);
                    }
                }
                if (newProd.Prices.Count > 0) products.Add(newProd);
            }

            StringBuilder sb = new();
            if (products.Count > 0)
            {
                codes = string.Join(",", products.Select(x => $"'{x.ItemCode.ToLower()}'").Distinct());
                BCA.Product bcCatalog = new();
                filters = new() { new Field("LOWER(Code)", codes, Operators.In) };
                var catalog = bcCatalog.ListCatalog(filters, "1");

                BCP.Line bcLine = new();
                codes = string.Join(",", products.Select(x => x.Id));
                var lines = bcLine.ListForPriceList(codes, "1", BEP.relLine.Manager, BEP.relLine.LineDetails);
                string baseUri = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

                sb.AppendLine(@"<html>");
                sb.AppendLine(@" <head>");
                sb.AppendLine(@"     <style>");
                sb.AppendLine(@"     body { font-size: 12px; font-family: 'Montserrat', Verdana, Geneva, sans-serif } ");
                sb.AppendLine(@"     table { border-collapse: collapse; width: 100% }");
                sb.AppendLine(@"      td { font-size: 11px; padding: 4px 8px; } ");
                //sb.AppendLine(@"      tr:nth-child(even) { background: #CCC } ");
                //sb.AppendLine(@"     .header { background-color: #5B9BD5; color: #FFF; } ");
                sb.AppendLine(@"     .header td { padding-bottom: 8px; }");
                sb.AppendLine(@"     .product img { display: block;max-width: 100%;max-height: 200px;margin: auto; }");
                sb.AppendLine(@"     .product img.new { position: absolute;left: 0;top: 0;z-index: 101;opacity: .8; } ");
                sb.AppendLine(@"    .price { padding: 8px; } ");
                sb.AppendLine(@"    .price.santa-cruz { background-color: #cdf7c5; color: #149a05; } ");
                sb.AppendLine(@"    .price.santa-cruz td { color: #149a05; font-size: 10px } ");
                sb.AppendLine(@"    .price.iquique { background-color: #d1e4eb;color: #4b7ca8 } ");
                sb.AppendLine(@"    .price.iquique td { color: #4b7ca8; font-size: 10px } ");
                sb.AppendLine(@"    .price.miami { background-color: #f2f7cd;color: #b8b97b } ");
                sb.AppendLine(@"    .price.miami td { color: #b8b97b; font-size: 10px } ");
                sb.AppendLine(@"    .price .regular { text-align: right } ");
                sb.AppendLine(@"    .price .regular.with-offer { text-decoration: line-through;font-size: 1.1em;font-weight: 600; } ");
                sb.AppendLine(@"    .price .offer { font-size: 1.1em;font-weight: 800;text-align: right;color: #f65656; } ");
                sb.AppendLine(@"    .price .offer-desc { text-align: right;color: #f65656; }");
                sb.AppendLine(@" </style>");
                sb.AppendLine(@"    <!--[if(mso)|(IE)]>");
                sb.AppendLine(@"    <style type=""text/css"">");
                sb.AppendLine(@"        table {border-collapse: collapse !important; mso-table-lspace:0pt !important; mso-table-rspace:0pt !important;}");
                sb.AppendLine(@"        table, div, td {font-family: 'Montserrat', Verdana, Geneva, sans-serif !important;}");
                sb.AppendLine(@"    </style>");
                sb.AppendLine(@"    <![endif]-->");
                sb.AppendLine(@"    <!--[if mso]>");
                sb.AppendLine(@"        <style type=""text/css"">");
                sb.AppendLine(@"            ol, ul {margin-left: 25px !important; margin-top: 0 !important;}");
                sb.AppendLine(@"        </style>");
                sb.AppendLine(@"        <xml>");
                sb.AppendLine(@"          <o:OfficeDocumentSettings>");
                sb.AppendLine(@"            <o:AllowPNG/>");
                sb.AppendLine(@"            <o:PixelsPerInch>96</o:PixelsPerInch>");
                sb.AppendLine(@"         </o:OfficeDocumentSettings>");
                sb.AppendLine(@"        </xml>");
                sb.AppendLine(@"        <![endif]-->");
                sb.AppendLine(@" </head>");
                sb.AppendLine(@" <body>");
                sb.AppendLine(@"   <div>");
                foreach (var l in lines)
                {
                    sb.AppendLine($@"<table>");
                    sb.AppendLine($@"   <tr>");
                    sb.AppendLine($@"       <td style=""width: 20%""><img src=""{baseUri}/images/lines/{l.ImageURL}"" height=""65""></td>");
                    sb.AppendLine($@"       <td style=""width: auto"">{l.Header}</td>");
                    sb.AppendLine($@"       <td style=""width: 40%;text-align:right;""><strong>{l.Manager.Name}</strong><br />{l.Manager.Position}<br />{l.Manager.Mail}<br />Interno: {l.Manager.Phone}</td>");
                    sb.AppendLine($@"       <td style=""width: 65px""><img src=""{baseUri}/images/staff/{l.Manager.Photo}"" height=""65""></td>");
                    sb.AppendLine($@"   </tr>");
                    sb.AppendLine($@"   <tr>");
                    sb.AppendLine($@"       <td colspan=""4"">");

                    var tempProducts = products.Where(x => l.ListLineDetails.Any(y => y.SAPLine.ToLower() == x.Line.ToLower()));
                    foreach (var p in tempProducts)
                    {
                        var tempStock = stock.Where(x => x.ItemCode.ToLower() == p.ItemCode.ToLower());
                        if (tempStock.Any() || ValidateStock == "N")
                        {
                            sb.AppendLine($@"           <div>");
                            sb.AppendLine($@"               <table class=""product"">");
                            sb.AppendLine($@"                   <tr>");
                            sb.AppendLine($@"                       <td style=""width:30%;position:relative"">");
                            sb.AppendLine($@"                           <img src=""{baseUri}/images/products/{p.ImageURL}"">");
                            if (p.IsNew) sb.AppendLine($@"                           <img src=""{baseUri}/images/nuevo.png"" class=""new"">");
                            sb.AppendLine($@"                       </td>");
                            sb.AppendLine($@"                       <td style=""width:40%;vertical-align:top"">");
                            sb.AppendLine($@"                           <b>{p.ItemCode}</b><br />{p.Name}<br /><i>Garantía</i>: {p.Warranty}");
                            sb.AppendLine($@"                       </td>");
                            sb.AppendLine($@"                       <td style=""width:30%;vertical-align:top"">");
                            foreach (var r in p.Prices)
                            {
                                string stockData = "Stock Digital";
                                if (!p.IsDigital)
                                {
                                    var c = catalog.First(x => x.Code.ToLower() == p.ItemCode.ToLower());
                                    if (tempStock.Any())
                                    {
                                        stockData = @"<table style=""width: 100%;""><tbody><tr><td></td><td class=""text-right"">Stock</td></tr>";
                                        foreach (var s in tempStock)
                                        {
                                            //stockData += $@"<tr><td>{ToTitle(s.Warehouse)}</td><td style=""text-align:right"">{GetPercentage(c.Rotation, s.Stock, s.Reserved)} %</td></tr>";
                                            stockData += $@"<tr><td>{ToTitle(s.Warehouse)}</td><td style=""text-align:right"">{(IsLocal || s.Stock <= 50 ? s.Stock.ToString() : "+50")} %</td></tr>";
                                        }
                                        stockData += @"</tbody></table>";
                                    }
                                    else
                                    {
                                        stockData = "";
                                    }
                                }
                                sb.AppendLine($@"                       <div class=""price {r.Subsidiary.Replace(" ", "-")}""><b>{r.Subsidiary}</b><br />");
                                if (r.Offer.HasValue && r.Offer.Value > 0) sb.AppendLine($@"                           <div class=""offer"">Oferta: <span> {r.Offer.Value:N2} $Us</span></div>");
                                if (r.Offer.HasValue && r.Offer.Value > 0 && !string.IsNullOrEmpty(r.OfferDescription)) sb.AppendLine($@"                           <div class=""offer-desc"">{r.OfferDescription}</div>");
                                sb.AppendLine($@"                           <div class=""regular {(r.Offer.HasValue && r.Offer.Value > 0 ? "with-offer" : "")}""> {r.Regular:N2} $Us</div>");
                                sb.AppendLine($@"                           <div>{r.Observations}</div>");
                                sb.AppendLine($@"                           <div class=""stock"">{stockData}</div>");
                                sb.AppendLine($@"                       </div>");
                            }
                            sb.AppendLine($@"                       </td>");
                            sb.AppendLine($@"                   </tr>");
                            sb.AppendLine($@"               </table>");
                            sb.AppendLine($@"           </div>");
                        }
                    }
                    sb.AppendLine($@"       </td>");
                    sb.AppendLine($@"   </tr>");
                    sb.AppendLine(@"</table>");
                }
                sb.AppendLine(@"   </div>");
                sb.AppendLine(@" </body>");
                sb.AppendLine(@"</html>");
            }
            else
            {
                sb.AppendLine("No hay nada que mostrar");
            }

            string fileType, fileName;
            System.IO.Stream fileContent;
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.EmulateMediaTypeAsync(MediaType.Screen);
            await page.SetContentAsync(sb.ToString());
            if (ExportType == "I")
            {
                fileType = "image/jpeg";
                fileName = "converted.jpg";
                fileContent = await page.ScreenshotStreamAsync(new ScreenshotOptions { FullPage = true, Type = ScreenshotType.Jpeg, Quality = 100 });
            }
            else
            {
                fileType = "application/pdf";
                fileName = "converted.pdf";
                fileContent = await page.PdfStreamAsync(new PdfOptions { Format = PaperFormat.Letter, PrintBackground = true });
            }
            return File(fileContent, fileType, fileName);
        }

        #endregion

        #region Private Methods

        #endregion

    }
}
