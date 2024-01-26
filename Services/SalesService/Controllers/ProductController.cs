using BEntities.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace SalesService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "GetProductList")]
        public IActionResult GetProductList()
        {
            string error = "";
            try
            {
                BCA.Product bcProduct = new();
                IEnumerable<BEA.Product> products = bcProduct.ListCatalog2(new List<Field>(), "1") ?? Enumerable.Empty<BEA.Product>();
                string codes = string.Join(",", products.Select(x => $"'{x.Code}'"));

                BCP.Price bcPrice = new();
                List<Field> filters = new();
                IEnumerable<BEP.Price> prices = bcPrice.ListBySubsidiary((long)BEE.Subsidiaries.SantaCruz, BEP.relPrice.Product);

                BCP.Product bcProd = new();
                IEnumerable<BEP.Product> portalProducts = bcProd.ListWithPrices(new List<Field>(), "1");

                IEnumerable<long> productIds = portalProducts.Select(x => x.Id).Distinct();
                BCP.PriceOffer bcOffer = new();
                var offers = bcOffer.ListEnabled(productIds, "Price") ?? new List<BEP.PriceOffer>();

                BCP.WarehouseAllowed bcAllowed = new();
                filters = new List<Field> { new Field("LOWER(Subsidiary)", "santa cruz"), new Field("ClientVisible", "1"), new Field(LogicalOperators.And) };
                var allowed = bcAllowed.List(filters, "1");

                BCA.ProductStock bcStock = new();
                IEnumerable<BEA.ProductStock> stockItems = bcStock.ListServiceMany(codes ?? "") ?? new List<BEA.ProductStock>();

                BCA.CurrencyRate bcRate = new();
                BEA.CurrencyRate beRate = bcRate.Search(DateTime.Now);
                decimal rate = beRate?.Rate ?? 0;

                BCP.Line bcLine = new();
                var lines = bcLine.List("1", BEP.relLine.LineDetails);

                var stock = from i in stockItems
                            join a in allowed on i.Warehouse.ToLower() equals a.Name.ToLower()
                            group i by i.ItemCode into g
                            select new { code = g.Key, warehouses = g.Select(x => new { name = x.Warehouse, available = x.Available2 }) };

                var items = from p in products
                            join r in prices on p.Code.ToLower() equals r.Product.ItemCode.ToLower()
                            where r.Regular > 0
                            join p2 in portalProducts on p.Code.ToLower() equals p2.ItemCode.ToLower()
                            join s in stock on p.Code.ToLower() equals s.code.ToLower() into ljStock
                            from ls in ljStock.DefaultIfEmpty()
                            select new
                            {
                                code = p.Code,
                                name = p.Name,
                                codeBars = !string.IsNullOrEmpty(p.CodeBars) && Regex.IsMatch(p.CodeBars, @"^\d+$") ? p.CodeBars : "",
                                description = p.Description,
                                price = offers.Any(x => x.IdProduct == r.Product.Id & x.Price > 0) ? offers.Where(x => x.IdProduct == r.Product.Id).First().Price : r.Regular,
                                priceLocal = rate * (offers.Any(x => x.IdProduct == r.Product.Id & x.Price > 0) ? offers.Where(x => x.IdProduct == r.Product.Id).First().Price : r.Regular),
                                category = p.Category,
                                subcategory = p.Subcategory,
                                brand = p.Brand,
                                brandCode = p.FactoryCode,
                                stock = ls?.warehouses,
                                line = lines.FirstOrDefault(x => x.ListLineDetails.Any(y => y.SAPLine?.ToLower() == p.Line?.ToLower()))?.Name ?? "",
                                picture = string.IsNullOrEmpty(p2.ImageURL) ? "" : $"https://portal.dmc.bo/images/products/{p2.ImageURL}"
                            };
                return Ok(new { error, items });

            }
            catch (Exception ex)
            {
                error = ex.StackTrace ?? ex.Message; //"Se ha producido un error al traer los datos.";
            }
            return Ok(new { error });
        }
    }
}
