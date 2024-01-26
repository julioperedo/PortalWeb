using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCL = BComponents.Sales;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEL = BEntities.Sales;
using BES = BEntities.Security;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class TransportController : BaseController
    {
        #region Constructores

        public TransportController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

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

        public IActionResult GetTransporter()
        {
            BCB.Classifier bcClassifier = new BCB.Classifier();
            List<BEB.Classifier> lstItems = bcClassifier.List((long)BEE.Classifiers.Transporters, "Name");
            return Json(lstItems.Select(x => new { x.Id, x.Name }));
        }

        public IActionResult GetSource()
        {
            BCB.Classifier bcClassifier = new BCB.Classifier();
            List<BEB.Classifier> lstItems = bcClassifier.List((long)BEE.Classifiers.TransportSource, "Name");
            return Json(lstItems.Select(x => new { x.Id, x.Name }));
        }

        public IActionResult GetDestiny()
        {
            BCB.Classifier bcClassifier = new BCB.Classifier();
            List<BEB.Classifier> lstItems = bcClassifier.List((long)BEE.Classifiers.TransportDestiny, "Name");
            return Json(lstItems.Select(x => new { x.Id, x.Name }));
        }

        public IActionResult GetTransportType()
        {
            BCB.Classifier bcClassifier = new BCB.Classifier();
            List<BEB.Classifier> lstItems = bcClassifier.List((long)BEE.Classifiers.TransportType, "Name");
            return Json(lstItems.Select(x => new { x.Id, x.Name }));
        }

        public IActionResult Filter(string Filters)
        {
            string message = "";
            try
            {
                Models.TransportFilter filters = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.TransportFilter>(Filters);
                var items = GetItems(filters);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Edit(long Id = -1)
        {
            if (Id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                BEL.Transport transport;
                if (Id == 0)
                {
                    transport = new BEL.Transport { Date = DateTime.Today.AddDays(-1), TypeId = (long)BEE.Types.Transport.Products, Sent = false };
                }
                else
                {
                    BCL.Transport bcTransport = new();
                    transport = bcTransport.Search(Id, BEL.relTransport.TransportDetails);
                    if (transport.TypeId == 149)
                    {
                        transport.StringValues = string.Join(",", transport.ListTransportDetails.Select(x => x.Code));
                    }
                    else
                    {
                        transport.StringValues = Newtonsoft.Json.JsonConvert.SerializeObject(transport.ListTransportDetails?.Select(x => new { x.Name, x.EMail }));
                    }
                }
                return PartialView(transport);
            }
        }

        public IActionResult OpenPreview(string Ids)
        {
            BCL.Transport bcTransport = new();
            List<Field> lstFilters = new() { new Field("Id", Ids, Operators.In) };
            IEnumerable<BEL.Transport> transports = bcTransport.List(lstFilters, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter, BEL.relTransport.TransportDetails);
            List<Models.Transport> items = new();
            foreach (var transport in transports)
            {
                Models.Transport item = new(transport);
                if (transport.ListTransportDetails?.Count > 0)
                {
                    if (transport.TypeId == 149)
                    {
                        BCA.Order bcOrder = new();
                        string orderCodes = string.Join(",", transport.ListTransportDetails.Select(x => x.Code));
                        lstFilters = new List<Field> {
                            new Field { Name = "DocNumber", Value = orderCodes, Operator = Operators.In },
                            new Field { Name = "LOWER(Subsidiary)", Value = "santa cruz" },
                            new Field { LogicalOperator = LogicalOperators.And }
                        };
                        var orders = bcOrder.List(lstFilters, "1");
                        if (orders?.Count() > 0)
                        {
                            var cardCodes = string.Join(",", orders.Select(x => $"'{x.ClientCode}'").Distinct());
                            var sellerCodes = string.Join(",", orders.Select(x => $"'{x.SellerCode}'").Distinct());

                            BCS.ClientContacts bcContact = new BCS.ClientContacts();
                            lstFilters = new List<Field> {
                                new Field { Name = "LOWER(CardCode)", Value = cardCodes.ToLower(), Operator = Operators.In },
                                new Field { Name = "Type", Value = 2 },
                                new Field { LogicalOperator = LogicalOperators.And }
                            };
                            IEnumerable<BES.ClientContacts> allContacts = bcContact.List(lstFilters, "Name") ?? new List<BES.ClientContacts>();

                            BCA.Client bcClient = new();
                            lstFilters = new List<Field> { new Field { Name = "LOWER(CardCode)", Value = cardCodes.ToLower(), Operator = Operators.In } };
                            var clients = bcClient.ListShort(lstFilters, "1");

                            BCS.UserData bcData = new();
                            lstFilters = new List<Field> { new Field { Name = "LOWER(SellerCode)", Value = sellerCodes.ToLower(), Operator = Operators.In } };
                            IEnumerable<BES.UserData> userData = bcData.List(lstFilters, "1", BES.relUserData.User);

                            var tempClients = (from c in clients
                                               group c by c.CardCode into g
                                               select new BEA.Client { CardCode = g.Key, CardName = g.FirstOrDefault().CardName, EMail = g.FirstOrDefault().EMail }).ToList();
                            foreach (var client in tempClients)
                            {
                                Models.Client beClient = new() { Code = client.CardCode, Name = client.CardName };
                                var contacts = allContacts?.Where(x => x.CardCode.ToLower() == client.CardCode.ToLower()) ?? new List<BES.ClientContacts>();
                                var clientNotes = orders.Where(x => x.ClientCode.ToLower() == client.CardCode.ToLower());
                                var noteSellers = userData.Where(x => clientNotes.Select(i => i.SellerCode.ToLower()).Contains(x.SellerCode.ToLower()));
                                beClient.Codes = string.Join(", ", clientNotes.Select(x => x.DocNumber.ToString()));

                                if (contacts.Count() > 0)
                                {
                                    beClient.Contacts.AddRange(contacts.Select(x => new Models.Mailer { EMail = x.EMail, Name = x.Name }));
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(client.EMail) && IsEmailValid(client.EMail))
                                    {
                                        beClient.Contacts.Add(new Models.Mailer { EMail = client.EMail, Name = client.CardName });
                                    }
                                }
                                if (noteSellers?.Count() > 0)
                                {
                                    beClient.Sellers.AddRange((from i in noteSellers where i.User != null select new Models.Mailer { EMail = i.User.EMail, Name = i.User.Name }));
                                }
                                item.Clients.Add(beClient);
                            }
                        }
                    }
                    else
                    {
                        Models.Client beClient = new() { Contacts = transport.ListTransportDetails.Select(x => new Models.Mailer { Name = x.Name, EMail = x.EMail }).ToList() };
                        item.Clients.Add(beClient);
                    }
                }
                items.Add(item);
            }
            return PartialView(items);
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BEL.Transport Item, Models.TransportFilter Filters)
        {
            string message = "";
            try
            {
                BCL.Transport bcTransport = new();
                BCL.TransportDetail bcDetail = new();
                List<Field> filters = new() { new Field("TransportId", Item.Id) };
                var detail = bcDetail.List(filters, "1");

                Item.StatusType = Item.Id == 0 ? BEntities.StatusType.Insert : BEntities.StatusType.Update;
                Item.LogDate = DateTime.Now;
                Item.LogUser = UserCode;
                Item.ListTransportDetails = new List<BEL.TransportDetail>();

                if (detail?.Count() > 0)
                {
                    foreach (var item in detail)
                    {
                        string value = Item.TypeId != 149 ? Newtonsoft.Json.JsonConvert.SerializeObject(new { item.Name, item.EMail }).ToLower() : item.Code?.ToLower();
                        if (!Item.Values.Select(x => x.ToLower()).Contains(value))
                        {
                            item.StatusType = BEntities.StatusType.Delete;
                            Item.ListTransportDetails.Add(item);
                        }
                    }
                }
                if (Item.Values?.Count > 0)
                {
                    foreach (var item in Item.Values)
                    {
                        if (!detail.Select(x => Item.TypeId != 149 ? Newtonsoft.Json.JsonConvert.SerializeObject(new { x.Name, x.EMail }).ToLower() : x.Code?.ToLower()).Contains(item.ToLower()))
                        {
                            Models.Mailer mailTo = null;
                            if (Item.TypeId != 149) mailTo = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Mailer>(item);
                            Item.ListTransportDetails.Add(new BEL.TransportDetail
                            {
                                StatusType = BEntities.StatusType.Insert,
                                Code = Item.TypeId == 149 ? item : null,
                                Name = Item.TypeId != 149 ? mailTo.Name : null,
                                EMail = Item.TypeId != 149 ? mailTo.EMail : null,
                                LogDate = DateTime.Now,
                                LogUser = UserCode
                            });
                        }
                    }
                }

                bcTransport.Save(ref Item);

                var items = GetItems(Filters);

                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete(long Id = -1)
        {
            if (Id <= 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                string message = "";
                try
                {
                    BCL.Transport bcTransport = new();
                    BEL.Transport transport = new() { StatusType = BEntities.StatusType.Delete, Id = Id, Date = DateTime.Today, LogDate = DateTime.Today };
                    bcTransport.Save(ref transport);
                }
                catch (Exception ex)
                {
                    if (ex.GetType().FullName == "System.Data.SqlClient.SqlException" && (((System.Data.SqlClient.SqlException)ex).Errors.Count > 0 && ((System.Data.SqlClient.SqlException)ex).Errors[0].Number == 547))
                    {
                        message = "El registro no se puede eliminar porque ya fué enviado.";
                    }
                    else
                    {
                        message = GetError(ex);
                    }
                }
                return Json(new { message });
            }
        }

        [HttpPost]
        public IActionResult SendMail(string Ids, Models.TransportFilter Filters)
        {
            string message = "";
            try
            {
                BCL.Transport bcTransport = new();
                List<Field> lstFilters = new() { new Field("Id", Ids, Operators.In) };
                IEnumerable<BEL.Transport> transports = bcTransport.List(lstFilters, "1", BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter, BEL.relTransport.TransportDetails);

                foreach (var item in transports)
                {
                    if (item.ListTransportDetails?.Count > 0)
                    {
                        if (item.TypeId == 149)
                        {
                            //Despacho de mercadería
                            BCA.Order bcOrder = new();
                            string orderCodes = string.Join(",", item.ListTransportDetails.Select(x => x.Code));
                            lstFilters = new List<Field> {
                                new Field { Name = "DocNumber", Value = orderCodes, Operator = Operators.In },
                                new Field { Name = "LOWER(Subsidiary)", Value = "santa cruz" },
                                new Field { LogicalOperator = LogicalOperators.And }
                            };
                            var orders = bcOrder.List(lstFilters, "1");
                            if (orders?.Count() > 0)
                            {
                                var cardCodes = string.Join(",", orders.Select(x => $"'{x.ClientCode}'").Distinct());
                                var sellerCodes = string.Join(",", orders.Select(x => $"'{x.SellerCode}'").Distinct());

                                BCS.ClientContacts bcContact = new BCS.ClientContacts();
                                lstFilters = new List<Field> {
                                    new Field { Name = "LOWER(CardCode)", Value = cardCodes.ToLower(), Operator = Operators.In },
                                    new Field { Name = "Type", Value = 2 },
                                    new Field { LogicalOperator = LogicalOperators.And }
                                };
                                IEnumerable<BES.ClientContacts> allContacts = bcContact.List(lstFilters, "Name") ?? new List<BES.ClientContacts>();

                                BCA.Client bcClient = new BCA.Client();
                                lstFilters = new List<Field> { new Field { Name = "LOWER(CardCode)", Value = cardCodes.ToLower(), Operator = Operators.In } };
                                var clients = bcClient.List(lstFilters, "1");

                                BCS.UserData bcData = new();
                                lstFilters = new List<Field> { new Field { Name = "LOWER(SellerCode)", Value = sellerCodes.ToLower(), Operator = Operators.In } };
                                IEnumerable<BES.UserData> userData = bcData.List(lstFilters, "1", BES.relUserData.User);

                                var tempClients = (from c in clients
                                                   group c by c.CardCode into g
                                                   select new BEA.Client { CardCode = g.Key, CardName = g.FirstOrDefault().CardName, EMail = g.FirstOrDefault().EMail }).ToList();

                                foreach (var client in tempClients)
                                {
                                    var contacts = allContacts?.Where(x => x.CardCode.ToLower() == client.CardCode.ToLower()) ?? new List<BES.ClientContacts>();
                                    var clientNotes = orders.Where(x => x.ClientCode.ToLower() == client.CardCode.ToLower());
                                    var noteSellers = userData.Where(x => clientNotes.Select(i => i.SellerCode.ToLower()).Contains(x.SellerCode.ToLower()));
                                    List<string> noteNumbers = clientNotes.Select(x => x.DocNumber.ToString()).ToList();

                                    List<Models.Mailer> destinataries = new(), copies = new List<Models.Mailer>();
                                    if (contacts.Count() > 0)
                                    {
                                        destinataries.AddRange(from c in contacts where IsEmailValid(c.EMail) & !IsEMailBlacklisted(c.EMail) select new Models.Mailer(c.Name, c.EMail));
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrWhiteSpace(client.EMail) && IsEmailValid(client.EMail) && !IsEMailBlacklisted(client.EMail))
                                        {
                                            destinataries.Add(new Models.Mailer { EMail = client.EMail, Name = client.CardName });
                                        }
                                    }
                                    //destinataries.Add(new Models.Mailer { Name = "Julio Peredo F", EMail = "julio.peredo@gmail.com" });
                                    if (noteSellers?.Count() > 0)
                                    {
                                        copies.AddRange((from i in noteSellers where i.User != null select new Models.Mailer { EMail = i.User.EMail, Name = i.User.Name }));
                                        //copies.AddRange(new[] { new Models.Mailer { EMail = "julio.peredo@dmc.bo", Name = "Julio C. Peredo" } });
                                    }
                                    SendTransportEMail(item.Id, client.CardName, item, noteNumbers, destinataries, copies);
                                }
                            }
                        }
                        else
                        {
                            List<Models.Mailer> destinataries = new(), copies = new();
                            destinataries = (from x in item.ListTransportDetails where IsValidEmail(x.EMail) & !IsEMailBlacklisted(x.EMail) select new Models.Mailer(x.Name, x.EMail)).ToList();
                            SendTransportEMail(item.Id, "", item, null, destinataries, copies);
                        }
                    }
                }
                var items = GetItems(Filters);
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Métodos Privados

        private IEnumerable<BEL.Transport> GetItems(Models.TransportFilter Filters)
        {
            BCL.Transport bcTransport = new();
            List<Field> lstFilters = new();
            if (Filters.InitialDate.HasValue)
            {
                lstFilters.Add(new Field { Name = "Date", Value = Filters.InitialDate.Value.ToString("yyyy-MM-dd"), Operator = Operators.HigherOrEqualThan });
            }
            if (Filters.FinalDate.HasValue)
            {
                lstFilters.Add(new Field { Name = "Date", Value = Filters.FinalDate.Value.ToString("yyyy-MM-dd"), Operator = Operators.LowerOrEqualThan });
            }
            if (Filters.TransporterId.HasValue && Filters.TransporterId.Value > 0)
            {
                lstFilters.Add(new Field { Name = "TransporterId", Value = Filters.TransporterId.Value });
            }
            if (Filters.SourceId.HasValue && Filters.SourceId.Value > 0)
            {
                lstFilters.Add(new Field { Name = "SourceId", Value = Filters.SourceId.Value });
            }
            if (Filters.DestinationId.HasValue && Filters.DestinationId.Value > 0)
            {
                lstFilters.Add(new Field { Name = "DestinationId", Value = Filters.DestinationId.Value });
            }
            if (!string.IsNullOrWhiteSpace(Filters.Sent) && Filters.Sent.Trim() != "B")
            {
                lstFilters.Add(new Field { Name = "Sent", Value = Filters.Sent == "Y" ? 1 : 0 });
            }
            if (!string.IsNullOrWhiteSpace(Filters.Filter))
            {
                lstFilters.AddRange(new[] {
                    new Field { Name = "DocNumber", Value = Filters.Filter.Trim(), Operator = Operators.Likes },
                    new Field { Name = "DeliveryTo", Value = Filters.Filter.Trim(), Operator = Operators.Likes },
                    new Field { Name = "Observations", Value = Filters.Filter.Trim(), Operator = Operators.Likes },
                    new Field { LogicalOperator= LogicalOperators.Or },
                    new Field { LogicalOperator= LogicalOperators.Or }
                });
            }
            CompleteFilters(ref lstFilters);
            IEnumerable<BEL.Transport> lstItems = bcTransport.List(lstFilters, "Date DESC", BEL.relTransport.TransportDetails, BEL.relTransport.Destination, BEL.relTransport.Source, BEL.relTransport.Transporter, BEL.relTransport.Type) ?? new List<BEL.Transport>();
            if (lstItems?.Count() > 0)
            {
                string orderNumbers = string.Join(",", (from x in lstItems where x.ListTransportDetails.Any(d => !string.IsNullOrEmpty(d.Code)) select string.Join(",", (from y in x.ListTransportDetails where !string.IsNullOrEmpty(y.Code) select y.Code))));
                if (!string.IsNullOrEmpty(orderNumbers))
                {
                    BCA.Order bcOrder = new();
                    var destinations = bcOrder.ListDestinations(orderNumbers);
                    if (destinations?.Count() > 0)
                    {
                        string cardCodes = string.Join(",", destinations.Select(x => $"'{x.ClientCode.ToLower()}'"));
                        BCS.ClientContacts bcContacts = new();
                        lstFilters = new List<Field> { new Field("LOWER(CardCode)", cardCodes, Operators.In) };
                        var contacts = bcContacts.List(lstFilters, "1");
                        foreach (var item in lstItems)
                        {

                            bool validEmail = false;
                            int count = item.ListTransportDetails.Count, i = 0;
                            while (!validEmail & i < count)
                            {
                                var d = item.ListTransportDetails[i];
                                if (string.IsNullOrEmpty(d.Code) & !string.IsNullOrEmpty(d.EMail))
                                {
                                    validEmail = IsValidEmail(d.EMail) & IsEmailValid(d.EMail) & !IsEMailBlacklisted(d.EMail);
                                }
                                else
                                {
                                    var dest = destinations.FirstOrDefault(x => x.DocNumber.ToString() == d.Code);
                                    validEmail = dest != null && (IsValidEmail(dest?.EMail) & IsEmailValid(dest?.EMail) & !IsEMailBlacklisted(dest?.EMail)) || contacts.Count(x => x.CardCode.ToLower() == dest?.ClientCode?.ToLower() & IsValidEmail(x.EMail) & !IsEMailBlacklisted(x.EMail)) > 0;
                                }
                                i++;
                            }
                            item.ValidEmail = validEmail;
                        }
                    }
                }
            }
            return lstItems;
        }

        private void SendTransportEMail(long Id, string Name, BEL.Transport Item, List<string> Notes, List<Models.Mailer> Destinataries, List<Models.Mailer> Copies)
        {
            string subject = "Datos de Guía de Transporte";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($@"	<style> ");
            sb.AppendLine(@"		body { background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; } ");
            sb.AppendLine(@"		img { margin: 20px 15px; }");
            sb.AppendLine(@"		.central td { padding: 0 8px; line-height: 18px; }");
            sb.AppendLine($@"	</style>");
            if (Destinataries.Count == 0)
            {
                Destinataries.Add(new Models.Mailer("Paul Chang", "paul.chang@dmc.bo"));
                sb.AppendLine($@"	<div style=""background-color: #f65e5e; color: #FFF; font-weight: 600; margin: 10px; padding: 15px; border-radius: 8px; max-width: 768px; margin-bottom: 10px;"">Este cliente no tiene un correo v&aacute;lido y no recibir&aacute; este correo.</div>");
            }
            sb.AppendLine($@"	<div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px; max-width: 768px;"" >");
            sb.AppendLine($@"		<table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse; max-width: 750px;"">");
            sb.AppendLine($@"			<tr>");
            sb.AppendLine($@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine($@"				<td style=""height:130px"">");
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine($@"				</td>");
            sb.AppendLine($@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine($@"			</tr>");
            sb.AppendLine($@"			<tr>");
            sb.AppendLine($@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine($@"				<td>");
            if (Item.TypeId == 149)
            {
                sb.AppendLine($@"				    <p>Estimado Cliente: <strong>{Name}</strong><br />Le hemos hecho un despacho con los siguientes datos<p/>");
            }
            else
            {
                sb.AppendLine($@"                    <p>Estimado Socio de negocios le hemos hecho un despacho con los siguientes datos:</p>");
            }
            sb.AppendLine($@"                    <p>");
            sb.AppendLine($@"                        <table class=""central"" style=""width: 100%; max-width: 740px;"">");
            sb.AppendLine($@"                        	<tr>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"" style=""font-weight: 600; width: 180px;"">Fecha:</td><td>{Item.Date.ToString("dd/MM/yyyy")}</td>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">No. de Guía:</td><td>{Item.DocNumber}</td>");
            sb.AppendLine($@"                        	</tr>");
            sb.AppendLine($@"                        	<tr>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Transporte:</td><td colspan=""3"">{Item.Transporter?.Name ?? ""}</td>");
            sb.AppendLine($@"                        	</tr>");
            sb.AppendLine($@"                        	<tr>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Origen:</td><td>{Item.Source?.Name ?? ""}</td>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Destino:</td><td>{Item.Destination?.Name ?? ""}</td>");
            sb.AppendLine($@"                        	</tr>");
            sb.AppendLine($@"                        	<tr>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Entregar a:</td><td colspan=""3"">{Item.DeliveryTo}</td>");
            sb.AppendLine($@"                        	</tr>");
            sb.AppendLine($@"                        	<tr>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Observaciones:</td><td colspan=""3"">{Item.Observations}</td>");
            sb.AppendLine($@"                        	</tr>");
            sb.AppendLine($@"                        	<tr>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Peso (Kg):</td><td>{Item.Weight.ToString("N2")}</td>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Piezas:</td><td>{Item.QuantityPieces}</td>");
            sb.AppendLine($@"                        	</tr>");
            sb.AppendLine($@"                        	<tr>");
            sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Monto (por pagar) Bs:</td><td colspan=""3"">{(Item.RemainingAmount > 0 ? Item.RemainingAmount.ToString("N2") : "")}</td>");
            sb.AppendLine($@"                        	</tr>");
            if (Item.TypeId == 149)
            {
                sb.AppendLine($@"                           <tr>");
                sb.AppendLine($@"                        		<td style=""font-weight: 600;"">Ordenes:</td><td colspan=""3"">{string.Join(", ", Notes)}</td>");
                sb.AppendLine($@"                        	</tr>");
            }
            sb.AppendLine($@"                        </table>");
            sb.AppendLine($@"                    </p>");
            sb.AppendLine($@"					<p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine($@"				</td>");
            sb.AppendLine($@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine($@"			</tr>");
            sb.AppendLine($@"		</table>");
            sb.AppendLine($@"	</div>");

            List<MailAddress> lstTo = Destinataries.Select(x => new MailAddress(x.EMail, x.Name)).ToList(), lstCopies = Copies.Select(x => new MailAddress(x.EMail, x.Name)).ToList(), lstBlindCopies = new List<MailAddress>();
            if (Item.WithCopies)
            {
                FillCustomCopies("Sales", "Transport", "TransportGuide", ref lstCopies, ref lstBlindCopies);
            }
            else
            {
                List<MailAddress> noCopies = default;
                FillCustomCopies("Sales", "Transport", "TransportGuide", ref noCopies, ref lstBlindCopies);
            }

            _ = SendMailAsync(subject, sb.ToString(), lstTo, lstCopies, lstBlindCopies);

            IEnumerable<Models.Mailer> tos = lstTo.Select(x => new Models.Mailer(x.DisplayName, x.Address)), copies = lstCopies.Select(x => new Models.Mailer(x.DisplayName, x.Address)),
                blinCopies = lstBlindCopies.Select(x => new Models.Mailer(x.DisplayName, x.Address));

            BEL.TransportSent item = new BEL.TransportSent
            {
                StatusType = BEntities.StatusType.Insert,
                IdTransport = Id,
                Tos = Newtonsoft.Json.JsonConvert.SerializeObject(tos),
                CCs = Newtonsoft.Json.JsonConvert.SerializeObject(copies),
                BCCs = Newtonsoft.Json.JsonConvert.SerializeObject(blinCopies),
                Body = sb.ToString(),
                LogUser = UserCode,
                LogDate = DateTime.Now
            };

            BCL.Transport bcTransport = new BCL.Transport();
            BEL.Transport transport = bcTransport.Search(Id);
            transport.StatusType = BEntities.StatusType.Update;
            transport.Sent = true;
            transport.ListTransportSents = new List<BEL.TransportSent> { item };

            bcTransport.Save(ref transport);
        }

        #endregion
    }
}