using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
    public class NoteController : ControllerBase
    {
        #region GETs

        [HttpGet("getnote")]
        public IActionResult GetNote(string Subsidiary, int Id)
        {
            string message;
            try
            {
                BCA.Note bcNote = new();
                BEA.NoteExtended n = bcNote.SearchExtended(Id, Subsidiary);

                List<string> clientOrders = new();
                IEnumerable<BEA.DocumentRelated> relatedDocs = new List<BEA.DocumentRelated>();
                if (!string.IsNullOrEmpty(n?.OrderNumber) & Subsidiary.ToLower() == "santa cruz")
                {
                    relatedDocs = bcNote.ListRelatedDocuments(Subsidiary, n.Id.ToString());
                    List<Field> filters;
                    if (relatedDocs.Any(x => x.DocType == "Orden de Venta"))
                    {
                        BCA.Order bcOrder = new();
                        string codes = string.Join(",", relatedDocs.Where(x => x.DocType == "Orden de Venta").Select(x => x.DocNumber));
                        filters = new List<Field> { new Field("DocNumber", codes, Operators.In), new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field(LogicalOperators.And) };
                        IEnumerable<BEA.Order> orders = bcOrder.List(filters, "1");
                        clientOrders.AddRange(orders.Select(x => x.ClientOrder));
                    }
                    if (relatedDocs.Any(x => x.DocType == "Nota de Entrega"))
                    {
                        BCA.DeliveryNote bcDelivery = new();
                        string codes = string.Join(",", relatedDocs.Where(x => x.DocType == "Nota de Entrega").Select(x => x.DocNumber));
                        filters = new List<Field> { new Field("DocNumber", codes, Operators.In), new Field("LOWER(Subsidiary)", Subsidiary.ToLower()), new Field(LogicalOperators.And) };
                        IEnumerable<BEA.DeliveryNote> notes = bcDelivery.List(filters, "1");
                        clientOrders.AddRange(notes.Select(x => x.ClientOrder));
                    }
                }

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
                    Subsidiary = subsidiary,
                    n.Warehouse,
                    Id = n.DocNumber,
                    n.OrderNumber,
                    Date = n.DocDate,
                    n.ClientCode,
                    n.ClientName,
                    Address = address,
                    n.BillingAddress,
                    n.DestinationCode,
                    n.DestinationAddress,
                    n.Incoterms,
                    ClientNote = string.Join(",", clientOrders.Where(x => !string.IsNullOrEmpty(x)).Distinct()),
                    n.Correlative,
                    Seller = n.SellerCode,
                    n.Transport,
                    Terms = n.TermConditions,
                    n.Comments,
                    n.Total,
                    n.Discount,
                    n.BilledTo,
                    n.Phone,
                    n.Cellphone,
                    NamePC = n.NamePC ?? "",
                    PhonePC = n.PhonePC ?? "",
                    n.Accredited,
                    CellphonePC = n.CellphonePC ?? "",
                    IsDeliveryNote = n.IsDeliveryNote ?? "N",
                    Latitude = n.Latitude ?? "",
                    Longitude = n.Longitude ?? "",
                    BusinessName = n.BusinessName ?? "",
                    NIT = n.NIT ?? "",
                    FCName = n.FCName ?? "",
                    FCMail = n.FCMail ?? "",
                    FCPhone = n.FCPhone ?? "",
                    FCCity = n.FCCity ?? "",
                    FCAddress = n.FCAddress ?? ""
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
                BCA.Note bcNote = new();
                BCA.Serial bcSerial = new();
                IEnumerable<BEA.NoteItem> lstItems = bcNote.ListItems(Subsidiary, Id.ToString(), "LineNum");
                List<Field> filters = new() { new Field("Subsidiary", Subsidiary), new Field("DocNum", Id), new Field(LogicalOperators.And) };
                IEnumerable<BEA.Serial> lstSerials = bcSerial.List(filters, "1");
                var items = lstItems.Select(x => new
                {
                    x.ItemCode,
                    x.ItemName,
                    x.BrandCode,
                    x.Quantity,
                    x.Price,
                    x.ItemTotal,
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

        [HttpGet("getbill")]
        public IActionResult GetBill(int Id)
        {
            string message;
            try
            {
                BCA.Note bcNote = new();
                BEA.Bill n = bcNote.SearchBill(Id);

                var item = new
                {
                    n.AuthorizationNumber,
                    n.BillNumber,
                    n.ClientCode,
                    n.ClientName,
                    n.ControlCode,
                    n.DocDate,
                    n.DocNumber,
                    n.DocTotal,
                    n.DocTotalRated,
                    n.LimitDate,
                    n.NIT,
                    n.SysRate,
                    literalAmount = GetLiteralAmount(n.DocTotalRated)
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
        public IActionResult GetBillItems(int Id)
        {
            string message;
            try
            {
                BCA.Note bcNote = new();
                BCA.Serial bcSerial = new();
                IEnumerable<BEA.BillItem> lstItems = bcNote.ListBillItems(Id);
                return Ok(lstItems);
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

        [HttpGet("getelectronicbill")]
        public IActionResult GetElectronicBill(int Id)
        {
            string message;
            try
            {
                BCA.Note bcNote = new();
                BEA.Bill n = bcNote.SearchElectronicBill(Id);

                var item = new
                {
                    n.AuthorizationNumber,
                    n.BillNumber,
                    n.ClientCode,
                    n.ClientName,
                    n.DocDate,
                    n.DocNumber,
                    n.DocTotal,
                    n.DocTotalRated,
                    n.NIT,
                    n.SysRate,
                    literalAmount = GetLiteralAmount(n.DocTotalRated)
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

        [HttpGet("getelectronicbillitems")]
        public IActionResult GetElectronicBillItems(int Id)
        {
            string message;
            try
            {
                BCA.Note bcNote = new();
                BCA.Serial bcSerial = new();
                IEnumerable<BEA.BillItem> lstItems = bcNote.ListElectronicBillItems(Id);
                return Ok(lstItems);
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

        #endregion

        #region Private Methods

        private string GetLiteralAmount(decimal Monto)
        {
            string strMontoLiteral = "";

            if (Monto > 0m)
            {
                if (Monto > 1)
                {
                    strMontoLiteral = NumeroLiteral(Monto);
                    decimal decMontoFraccion = Math.Round(Convert.ToDecimal(Monto - Math.Floor(Monto)), 2);

                    if (decMontoFraccion > 0)
                    {
                        decMontoFraccion = Convert.ToDecimal(Math.Floor(decMontoFraccion * 100));
                        strMontoLiteral += " con " + decMontoFraccion.ToString() + "/100 ";
                    }
                    else
                    {
                        if (strMontoLiteral[0].ToString().ToUpper() != "0")
                            strMontoLiteral += " con 00/100 ";
                        else
                            strMontoLiteral = "0";
                    }
                }
                else
                {
                    strMontoLiteral = (Monto * 100).ToString() + "/100 Centavos";
                }
            }
            return ToTitle(strMontoLiteral);
        }

        private string NumeroLiteral(decimal Numero)
        {
            string[] Unidades = { "un", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez", "once", "doce", "trece", "catorce", "quince" };
            string[] Decenas = { "dieci", "veint", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };
            string[] Centenas = { "ciento ", "doscientos ", "trecientos ", "cuatrocientos ", "quinientos ", "seiscientos ", "setecientos ", "ochocientos ", "novecientos " };
            string[] Miles = { " mil ", " millones " };
            long MontoEntero = Convert.ToInt64(Math.Floor(Numero));
            StringBuilder Literal = new StringBuilder();
            while (MontoEntero > 0)
            {
                long Parcial;
                if (MontoEntero >= 1000000)
                {
                    Parcial = MontoEntero / 1000000;
                    MontoEntero %= 1000000;
                    if (Parcial == 1)
                        Literal.Append("un millon");
                    else
                        Literal.Append(NumeroLiteral(Parcial) + Miles[1]);
                }
                else if (MontoEntero >= 1000)
                {
                    Parcial = MontoEntero / 1000;
                    MontoEntero %= 1000;
                    if (Parcial == 1)
                        Literal.Append("un " + Miles[0]);
                    else
                        Literal.Append(NumeroLiteral(Parcial) + Miles[0]);
                }
                else if (MontoEntero >= 100)
                {
                    Parcial = MontoEntero / 100;
                    MontoEntero %= 100;
                    if (Parcial == 1 & MontoEntero == 0)
                        Literal.Append("cien");
                    else
                        Literal.Append(Centenas[Convert.ToInt32(Parcial - 1)]);
                }
                else if (MontoEntero > 15)
                {
                    string Comodin;
                    if (MontoEntero >= 20 & MontoEntero < 30)
                        Comodin = Convert.ToString((MontoEntero % 10 == 0 ? "e" : "i"));
                    else if (MontoEntero % 10 != 0 & MontoEntero > 30)
                        Comodin = " y ";
                    else
                        Comodin = "";
                    Parcial = MontoEntero / 10;
                    MontoEntero %= 10;
                    Literal.Append(Decenas[Convert.ToInt32(Parcial - 1)] + Comodin);
                }
                else
                {
                    Literal.Append(Unidades[Convert.ToInt32(MontoEntero - 1)]);
                    MontoEntero = 0;
                }
            }
            return Literal.ToString().Trim();
        }

        private string ToTitle(string message)
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(message))
            {
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                result = myTI.ToTitleCase(message.ToLower());
            }
            return result;
        }

        #endregion

    }
}