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
    public class DeliveryController : ControllerBase
    {
        [HttpGet("getnote")]
        public IActionResult GetNote(string Subsidiary, int Id, int SearchType)
        {
            string message;
            try
            {
                BCA.DeliveryNote bcNote = new BCA.DeliveryNote();
                BEA.DeliveryNote n = SearchType == 1 ? bcNote.SearchBySaleNote(Id, Subsidiary) : (SearchType == 2 ? bcNote.Search(Id, Subsidiary) : bcNote.SearchById(Id, Subsidiary));

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
                if (n != null)
                {
                    var item = new
                    {
                        Subsidiary = subsidiary,
                        Warehouse = n.Warehouse ?? "",
                        n.DocNumber,
                        Date = n.DocDate,
                        ClientCode = n.ClientCode ?? "",
                        ClientName = n.ClientName ?? "",
                        Address = address,
                        ClientAddress = n.ClientAddress ?? "",
                        DeliveryAddress = n.DeliveryAddress ?? "",
                        NoteNumber = n.NoteNumber ?? "",
                        Incoterms = n.Incoterms ?? "",
                        ClientNote = n.ClientNote ?? "",
                        Correlative = n.Correlative ?? "",
                        Seller = n.SellerCode,
                        Transport = n.Transport ?? "",
                        Terms = n.Terms ?? "",
                        Comments = n.Comments ?? "",
                        n.Total,
                        n.Discount,
                        BillName = n.BillName ?? "",
                        Phone = n.Phone ?? "",
                        Mobile = n.Mobile ?? "",
                        DeliveryName = n.DeliveryName ?? "",
                        DeliveryPhone = n.DeliveryPhone ?? "",
                        Accredited = n.Accredited ?? "",
                        DeliveryMobile = n.DeliveryMobile ?? "",
                        NIT = n.NIT ?? "",
                        SellerName = n.SellerName ?? ""
                    };
                    return Ok(item);
                }
                else {
                    message = "Documento no encontrado";
                }
          
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
        public IActionResult GetItems(string Subsidiary, int Id, int SearchType)
        {
            string message = "";
            try
            {
                BCA.DeliveryNote bcNote = new BCA.DeliveryNote();
                BCA.Serial bcSerial = new BCA.Serial();
                List<Field> filters = new List<Field> { new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field(SearchType == 1 ? "NoteNumber" : "DocNumber", Id.ToString()), new Field(LogicalOperators.And) };
                IEnumerable<BEA.DeliveryNoteItem> lstItems = bcNote.ListItems(filters, "LineNum");
                int newId = lstItems.FirstOrDefault().DocNumber;
                filters = new List<Field> { new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field("DocNum", newId), new Field(LogicalOperators.And) };
                IEnumerable<BEA.Serial> lstSerials = bcSerial.List(filters, "1");
                var items = lstItems.Select(x => new
                {
                    x.ItemCode,
                    x.ItemName,
                    x.BrandCode,
                    x.Quantity,
                    x.Price,
                    x.Unit,
                    x.Warranty,
                    Serials = string.Join(", ", (from s in lstSerials where s.ItemCode.ToLower() == x.ItemCode.ToLower() & !string.IsNullOrWhiteSpace(s.SerialNumber) select s.SerialNumber))
                });
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