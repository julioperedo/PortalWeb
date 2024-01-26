using BEntities.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCA = BComponents.SAP;
using BCN = BComponents.Campaign;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEN = BEntities.Campaign;
using BEP = BEntities.Product;

namespace CampaignService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region Global Variables

        private readonly ILogger<ProductController> _logger;

        #endregion

        #region Constructors

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region GETs

        [HttpGet()]
        public IActionResult GetProducts(long IdUser)
        {
            string message = "";
            try
            {
                BCN.User bcUser = new();
                var user = bcUser.Search(IdUser);

                BCP.Product bc = new();
                IEnumerable<BEP.Product> products = bc.ListForCampaign(user.IdCampaign, "1", BEP.relProduct.Prices, BEP.relPrice.Sudsidiary, BEP.relProduct.PriceOffers);

                BCN.Category bcCategory = new();
                var category = bcCategory.SearchByUser(IdUser);

                BCP.WarehouseAllowed bcWarehouse = new();
                IEnumerable<BEP.WarehouseAllowed> lstWarehouses = bcWarehouse.List("1");

                IEnumerable<BEA.ProductStock> lstStock = Enumerable.Empty<BEA.ProductStock>();
                BCA.ProductStock bcStock = new();
                string itemCodes = string.Join(",", (from p in products where !string.IsNullOrWhiteSpace(p.Line) & p.ListPrices.Any(x => x.Regular > 0) select $"'{p.ItemCode}'"));
                List<Field> filters = new() { new Field("LOWER(ItemCode)", itemCodes.ToLower(), Operators.In), new Field("Available2", 0, Operators.HigherThan), new Field(LogicalOperators.And) };
                try
                {
                    lstStock = bcStock.List(filters, "1");
                }
                catch (Exception) { }

                var items = from x in products
                            select new
                            {
                                x.Id,
                                x.ItemCode,
                                Name = x.Name ?? "",
                                Description = x.Description ?? "",
                                Category = x.Category ?? "",
                                Subcategory = x.SubCategory ?? "",
                                Line = x.Line ?? "",
                                ImageURL = x.ImageURL ?? "",
                                x.IsDigital,
                                Prices = from r in (x.ListPrices ?? new List<BEP.Price>())
                                         where r.Regular > 0
                                         select new
                                         {
                                             subsidiary = r.Sudsidiary.Name,
                                             price = r.Regular,
                                             offer = (from s in lstStock
                                                      where s.Subsidiary.ToLower() == r.Sudsidiary.Name.ToLower() & s.ItemCode.ToLower() == x.ItemCode.ToLower() &
                                                           (from w in lstWarehouses where w.ClientVisible select w.Name.ToLower()).Contains(s.Warehouse.ToLower())
                                                      select s.Available2).Sum() > 0 ? x.ListPriceOffers?.Where(o => o.IdSubsidiary == r.IdSudsidiary)?.OrderBy(o => o.Price)?.FirstOrDefault()?.Price ?? 0.0M : 0.0M,
                                             stock = from s in lstStock
                                                     where s.Subsidiary.ToLower() == r.Sudsidiary.Name.ToLower() & s.ItemCode.ToLower() == x.ItemCode.ToLower() & s.Available2 > 0
                                                     select new { s.Warehouse, Percentage = GetPercentage(s.Stock, s.Reserved, s.Rotation) }
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

        [HttpGet()]
        public IActionResult GetSerialsRegistered(int UserId, bool OnlyValid = true)
        {
            string message = "";
            try
            {
                BCN.Serial bc = new();
                List<Field> filters = new() { new Field("IdUser", UserId) };
                if (OnlyValid) filters.AddRange(new[] { new Field("State", "V"), new Field(LogicalOperators.And) });
                var serials = bc.List(filters, "1");
                var items = serials.Select(x => new
                {
                    x.Id,
                    x.IdUser,
                    x.SerialNumber,
                    x.RegisterDate,
                    x.State,
                    RejectReason = x.RejectReason ?? "",
                    CardCode = x.CardCode ?? "",
                    CardName = x.CardName ?? "",
                    ItemCode = x.ItemCode ?? "",
                    ItemName = x.ItemName ?? "",
                    x.Points,
                    x.Latitude,
                    x.Longitude
                });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet()]
        public IActionResult GetPoints(int UserId)
        {
            string message = "";
            int points = 0, count = 0, minPoints = 0, usedPoints = 0;
            try
            {
                BCN.Serial bc = new();
                var item = bc.PointsResume(UserId);
                points = item?.PointsSum ?? 0;
                count = item?.SerialsCount ?? 0;
                usedPoints = 0; //por ahora luego hay que descontar lo redimido

                BCN.Prizes bcPrizes = new();
                var prizes = bcPrizes.List("1");
                minPoints = prizes.Min(x => x.Points);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var o = new { points, count, minPoints, usedPoints };
            return Ok(new { message, item = o });
        }

        [HttpGet()]
        public IActionResult GetPrizes(int UserId = 0)
        {
            string message = "";
            try
            {
                BCN.Serial bcSerial = new();
                var resume = bcSerial.PointsResume(UserId);

                BCN.Prizes bc = new();
                List<Field> filters = new() { new Field("Enabled", 1) };
                int points = resume?.PointsSum ?? 0;
                IEnumerable<BEN.Prizes> prizes = points > 0 ? bc.List(filters, "1") : new List<BEN.Prizes>();
                var items = prizes.Select(x => new { x.Id, x.Name, x.Description, x.ImageUrl, x.Points });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet()]
        public IActionResult ValidateSerial(string SerialNumber)
        {
            string message = "", valid = "0", invalidReason = "";
            try
            {
                BCN.Product bcProduct = new();
                IEnumerable<BEN.Product> products = bcProduct.List("1");
                BCA.Serial bcSerial = new();
                List<Field> filters = new() { new Field("LOWER(SerialNumber)", SerialNumber.Trim().ToLower()), new Field("DocType", "13,15", Operators.In), new Field(LogicalOperators.And) };
                var productCards = bcSerial.List(filters, "DocDate DESC");
                valid = productCards.Any(x => products.Any(y => y.ItemCode.ToLower() == x.ItemCode.ToLower())) ? "1" : "0";
                if (valid == "0") invalidReason = productCards.Any() ? "P" : "S";
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message, valid, invalidReason });
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public IActionResult RegisterSerial(string SerialNumber, bool IsScanned, long IdUser, double? Latitude, double? Longitude)
        {
            string message = "", state, rejectReason = "", cardCode = "", itemCode = "", itemName = "";
            long id = 0;
            try
            {
                BCN.Product bcProduct = new();
                IEnumerable<BEN.Product> products = bcProduct.List("1");

                BCN.Serial bcRecord = new();
                List<Field> filters = new() { new Field("LOWER(SerialNumber)", SerialNumber.Trim().ToLower()), new Field("State", "V"), new Field(LogicalOperators.And) };
                var records = bcRecord.List(filters, "1");

                BCA.Serial bcSerial = new();
                filters = new() { new Field("LOWER(SerialNumber)", SerialNumber.Trim().ToLower()), new Field("DocType", "13,15", Operators.In), new Field(LogicalOperators.And) };
                var productCards = bcSerial.List(filters, "DocDate DESC");
                int count = records.Count(), points = 0;
                state = records.Any() || !productCards.Any(x => products.Any(y => y.ItemCode.ToLower() == x.ItemCode.ToLower())) ? "I" : "V";
                if (state == "I")
                {
                    rejectReason = records.Any() ? "A" : (productCards.Any() ? "IP" : "I");
                }
                else
                {
                    filters = new() { new Field("IdUser", IdUser), new Field("State", "V"), new Field(LogicalOperators.And) };
                    records = bcRecord.List(filters, "1");
                    count = records.Count();
                    points = count switch
                    {
                        > 100 and <= 200 => 3,
                        > 200 and <= 325 => 4,
                        _ => 2,
                    };
                }
                DateTime now = DateTime.Now;
                var card = productCards?.FirstOrDefault();
                BEN.Serial newRecord = new()
                {
                    StatusType = BEntities.StatusType.Insert,
                    IdUser = IdUser,
                    SerialNumber = SerialNumber.ToUpper(),
                    IsScanned = IsScanned,
                    RegisterDate = now,
                    State = state,
                    RejectReason = rejectReason,
                    CardCode = card?.ClientCode,
                    CardName = card?.ClientName,
                    ItemCode = card?.ItemCode,
                    ItemName = card?.ItemName,
                    Points = points,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    LogDate = now
                };
                bcRecord.Save(ref newRecord);
                id = newRecord.Id;
            }
            catch (Exception ex)
            {
                message = GetError(ex);
                state = "";
                rejectReason = "";
            }
            return Ok(new { message, state, rejectReason, id, cardCode, itemCode, itemName });
        }

        #endregion

        #region Private Methods

        private static int? GetPercentage(int Stock, int Reserved, string Rotation)
        {
            int? decResult = null;
            if (!string.IsNullOrWhiteSpace(Rotation))
            {
                decResult = (Stock - Reserved) * 100;
                switch (Rotation.ToLower())
                {
                    case "baja":
                        decResult /= 10;
                        break;
                    case "media":
                        decResult /= 50;
                        break;
                    case "intermedia":
                        decResult /= 100;
                        break;
                    case "alta":
                        decResult /= 500;
                        break;
                    default:
                        decResult = null;
                        break;
                }
                if (decResult.HasValue && decResult < 0) decResult = 0;
                if (decResult.HasValue && decResult > 100) decResult = 100;
            }
            return decResult;
        }

        private string GetError(Exception ex)
        {
            _logger.LogError(ex, "Error al regitrar el serial");
            string message = ex.Message;
            var e1 = ex.InnerException;
            while (e1 != null)
            {
                message += Environment.NewLine + e1.Message;
                e1 = e1.InnerException;
            }
            return message;
        }

        #endregion
    }
}
