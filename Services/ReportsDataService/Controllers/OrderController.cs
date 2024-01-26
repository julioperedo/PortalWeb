using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BEA = BEntities.SAP;

namespace ReportsDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet("getorder")]
        public IActionResult GetOrder(string Subsidiary, int Id)
        {
            string message;
            try
            {
                BCA.Order bcOrder = new();
                BEA.OrderExtended o = bcOrder.SearchExtended(Id, Subsidiary);
                string address = "", subsidiary = "";
                if (Subsidiary.ToLower() == "santa cruz")
                {
                    address = "Av. Grigota # 3800 <br /> Santa Cruz- Bolivia <br /> Fono: (591) 3-3543000 <br /> Fax: (591) 3-3543637";
                    subsidiary = "DMC S.A.";
                }
                if (Subsidiary.ToLower() == "miami")
                {
                    address = "9935 NW 88 Ave. MIAMI, FL 33178 <br /> TEL.: (786) 245 4457 <br /> FAX: (305) 675 8549";
                    subsidiary = "Latin America, Inc.";
                }
                if (Subsidiary.ToLower() == "iquique")
                {
                    address = "Recinto Aduanero Zofri Manzana 9, Sitio 16 <br /> Iquique - Chile <br /> Fono: 57-542460  <br /> Fax: 57-542461";
                    subsidiary = "Importadora DMC Iquique, Ltda.";
                }
                var item = new
                {
                    subsidiary,
                    o.Warehouse,
                    Id = o.DocNumber,
                    Date = o.DocDate,
                    o.ClientCode,
                    o.ClientName,
                    address,
                    o.BillingAddress,
                    o.DestinationCode,
                    o.DestinationAddress,
                    o.Incoterms,
                    o.ClientOrder,
                    o.Correlative,
                    Seller = o.SellerCode,
                    o.Transport,
                    Terms = o.TermConditions,
                    Comments = o.Comments ?? "",
                    o.Total,
                    o.Discount,
                    Latitude = o.Latitude ?? "",
                    Longitude = o.Longitude ?? "",
                    BusinessName = o.BusinessName ?? "",
                    NIT = o.NIT ?? "",
                    FCName = o.FCName ?? "",
                    FCMail = o.FCMail ?? "",
                    FCPhone = o.FCPhone ?? "",
                    FCCity = o.FCCity ?? "",
                    FCAddress = o.FCAddress ?? ""
                };
                return Ok(item);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message += Environment.NewLine + ex.Message;
                }
            }
            return Ok(new { message });
        }

        [HttpGet("getitems")]
        public IActionResult GetItems(string Subsidiary, int Id)
        {
            string message = "";
            try
            {
                BCA.Order bcOrder = new();
                IEnumerable<BEA.OrderItem> lstItems = bcOrder.ListItems2(Subsidiary, Id, "LineNum");
                var items = lstItems.Select(x => new { x.ItemCode, x.ItemName, x.BrandCode, x.Quantity, x.OpenQuantity, x.DeliveredQuantity, x.Price, x.ItemTotal });
                return Ok(items);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message += Environment.NewLine + ex.Message;
                }
            }
            return Ok(new { message });
        }
    }
}