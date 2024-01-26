using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BEA = BEntities.SAP;

namespace ReportsDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditNoteController : ControllerBase
    {

        [HttpGet("getnote")]
        public IActionResult GetNote(string Subsidiary, long DocNumber)
        {
            string message;
            try
            {
                BCA.CreditNote bcCredit = new BCA.CreditNote();
                BEA.CreditNote n = bcCredit.Search(Subsidiary, DocNumber);
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
                var item = new { n.DocNumber, n.DocDate, n.DocDueDate, n.ClientCode, n.ClientName, ClientAddress = n.Address, n.Reference, n.SellerCode, n.Comments, n.Memo, n.TransId, SaleNote = n.NoteNumber, subsidiary, address, n.Terms, n.Total };
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
        public IActionResult GetItems(string Subsidiary, long DocNumber)
        {
            string message;
            try
            {
                BCA.CreditNote bcCredit = new BCA.CreditNote();
                IEnumerable<BEA.CreditNoteItem> temp = bcCredit.ListItems(Subsidiary, DocNumber);
                var items = temp.Select(x => new { x.ItemCode, x.ManufacterCode, x.ItemName, x.Quantity, x.Price, x.Total, x.AccountCode, x.AccountName });
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
