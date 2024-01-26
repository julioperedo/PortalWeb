using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCA = BComponents.SAP;
using BEA = BEntities.SAP;

namespace ReportsDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderOrderController : ControllerBase
    {

        [HttpGet("getorder")]
        public IActionResult GetOrder(string Subsidiary, int Id)
        {
            string message;
            try
            {
                BCA.ProviderOrder bcOrder = new BCA.ProviderOrder();
                BEA.ProviderOrder n = bcOrder.Search(Subsidiary, Id);

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
                    address = "RECINTO ADUANERO ZOFRI MANZANA 12, GALPON 8 <br /> Iquique - Chile <br /> Fono: 57-542460  <br /> Fax: 57-542461";
                    subsidiary = "Importadora DMC Iquique, Ltda.";
                }
                var item = new
                {
                    Subsidiary = subsidiary,
                    n.Warehouse,
                    Id = n.DocNumber,
                    n.DocNumber,
                    Date = n.DocDate,
                    n.ProviderCode,
                    n.ProviderName,
                    Address = address,
                    n.BillingAddress,
                    n.DeliveryAddress,
                    n.ReferenceOrder,
                    n.Terms,
                    n.OtherCosts,
                    Seller = n.SellerCode,
                    n.Comments,
                    n.Total,
                    n.DailyComments,
                    n.Transport,
                    n.EstimatedDate
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

        [HttpGet("getorderitems")]
        public IActionResult GetOrderItems(string Subsidiary, int Id)
        {
            string message = "";
            try
            {
                BCA.ProviderOrder bcBill = new BCA.ProviderOrder();
                List<Field> filters = new List<Field> { new Field("Subsidiary", Subsidiary), new Field("DocNumber", Id), new Field(LogicalOperators.And) };
                IEnumerable<BEA.ProviderOrderItem> lstItems = bcBill.ListItems(filters, "1");
                var items = lstItems.Select(x => new { x.ItemCode, x.ItemName, x.BrandCode, x.Quantity, x.Price, x.Subtotal, x.OpenQuantity });
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

        [HttpGet("getbill")]
        public IActionResult GetBill(string Subsidiary, int Id)
        {
            string message;
            try
            {
                BCA.ProviderBill bcNote = new BCA.ProviderBill();
                BEA.ProviderBill n = bcNote.Search(Subsidiary, Id);

                string address = "", subsidiary = "";
                if (Subsidiary.ToLower() == "santa cruz")
                {
                    address = "Av. Grigota # 3800 <br /> Santa Cruz- Bolivia <br /> Fono: (591) 3-3543000 <br /> Fax: (591) 3-3543637";
                    subsidiary = "DMC S.A.";
                }
                if (Subsidiary.ToLower() == "miami")
                {
                    address = "9935 NW 88 Ave. MIAMI, FL 33178 <br /> TEL.: (786) 245 4457 <br /> FAX: (305) 887 1955";
                    subsidiary = "Latin America, Inc.";
                }
                if (Subsidiary.ToLower() == "iquique")
                {
                    address = "Recinto Aduanero Zofri Manzana 9, Sitio 16 <br /> Iquique - Chile <br /> Fono: 57-542460  <br /> Fax: 57-542461";
                    subsidiary = "Importadora DMC Iquique, Ltda.";
                }
                var item = new
                {
                    Subsidiary = subsidiary,
                    n.Warehouse,
                    Id = n.DocNumber,
                    n.OrderNumber,
                    Date = n.DocDate,
                    n.ProviderCode,
                    n.ProviderName,
                    Address = address,
                    n.BillingAddress,
                    n.ProviderAddress,
                    n.Reference,
                    n.TermConditions,
                    n.OtherCosts,
                    Seller = n.SellerCode,
                    Terms = n.TermConditions,
                    n.Comments,
                    n.Total,
                    n.PaidToDate,
                    n.DailyComments
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

        [HttpGet("getbillitems")]
        public IActionResult GetBillItems(string Subsidiary, int Id)
        {
            string message = "";
            try
            {
                BCA.ProviderBill bcBill = new BCA.ProviderBill();
                IEnumerable<BEA.ProviderBillItem> lstItems = bcBill.List(Subsidiary, Id, "LineNum");
                var items = lstItems.Select(x => new { x.ItemCode, ItemName = x.Description, x.BrandCode, x.Quantity, x.Price, x.Total, x.TaxCode });
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