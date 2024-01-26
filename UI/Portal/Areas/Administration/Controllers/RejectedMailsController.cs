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
using BCA = BComponents.Sales;
using BCB = BComponents.Base;
using BCL = BComponents.Logs;
using BCP = BComponents.SAP;
using BCS = BComponents.Security;
using BEA = BEntities.Sales;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEL = BEntities.Logs;
using BEP = BEntities.SAP;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class RejectedMailsController : BaseController
    {
        #region Constructores

        public RejectedMailsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            //config = configuration;
            //_env = env;
        }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Filter(string ClientCode, string Email, DateTime? InitialDate, DateTime? FinalDate, string Reported)
        {
            string message = "";
            try
            {
                List<Field> filters = new();
                if (!string.IsNullOrEmpty(ClientCode))
                {
                    filters.Add(new Field("LOWER(ClientCode)", CardCode.ToLower()));
                }
                if (!string.IsNullOrEmpty(Email))
                {
                    filters.Add(new Field("LOWER(EMail)", Email.ToLower(), Operators.Likes));
                }
                if (InitialDate.HasValue)
                {
                    filters.Add(new Field("CAST(RejectionDate AS DATE)", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (FinalDate.HasValue)
                {
                    filters.Add(new Field("CAST(RejectionDate AS DATE)", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (Reported != "B")
                {
                    filters.Add(new Field("Reported", Reported == "Y" ? 1 : 0));
                }
                BCS.RejectedMails bcRejected = new();
                var items = bcRejected.ListWithDesc(filters, "1");
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetReasons()
        {
            BCB.Classifier bcClassifier = new();
            List<BEB.Classifier> reasons = bcClassifier.List((long)BEE.Classifiers.MailRejectionReasons, "Name");
            var items = reasons.Select(x => new { Code = x.Value, x.Name });
            return Json(items);
        }

        public IActionResult VerifyEmail(string Email)
        {
            string message = "", clientCode = "", clientName = "", seller = "";
            try
            {
                BCS.ClientContacts bcContact = new();
                List<Field> filters = new() { new Field("LOWER(EMail)", Email.ToLower()) };
                IEnumerable<BES.ClientContacts> contacts = bcContact.List(filters, "1");

                if (contacts?.Count() > 0)
                {
                    var contact = contacts.First();
                    clientCode = contact.CardCode;
                }

                if (clientCode == "")
                {
                    BCS.User bcUser = new();
                    IEnumerable<BES.User> users = bcUser.List(filters, "1");
                    if (users.Count() > 0)
                    {
                        var u = users.First();
                        clientCode = u.CardCode;
                    }
                }
                if (clientCode == "")
                {
                    BCL.BillSent bcSent = new();
                    filters = new() { new Field("LOWER(Recepients)", Email.ToLower(), Operators.Likes) };
                    IEnumerable<BEL.BillSent> sent = bcSent.List(filters, "1");
                    if (sent?.Count() > 0)
                    {
                        var last = sent.Last();
                        clientCode = last.CardCode;
                    }
                }
                if (clientCode == "")
                {
                    BCA.QuoteSent bcSent = new();
                    filters = new() { new Field("LOWER(ClientMail)", Email.ToLower()) };
                    IEnumerable<BEA.QuoteSent> sent = bcSent.List(filters, "1");
                    if (sent?.Count() > 0)
                    {
                        var last = sent.Last();
                        clientCode = last.CardCode;
                    }
                }
                if (clientCode != "")
                {
                    BCP.Client bcClient = new();
                    BEP.Client client = bcClient.Search(clientCode);

                    if (client != null)
                    {
                        clientName = client.CardName;
                        seller = client.SellerName;
                    }
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, clientCode, clientName, seller });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BES.RejectedMails Item)
        {
            string message = "";
            try
            {
                Item.StatusType = BEntities.StatusType.Insert;
                Item.LogUser = UserCode;
                Item.RejectionDate = Item.RejectionDate.AddHours(-4); //Se ajusta porque viene en UTC
                Item.LogDate = DateTime.Now;
                BCS.RejectedMails bcMail = new();
                bcMail.Save(ref Item);

                return Json(new { message, id = Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public async Task<IActionResult> ReportEmailsAsync(string Ids, string Comments)
        {
            string message = "";
            try
            {
                BCS.RejectedMails bcMail = new();
                List<Field> filters = new() { new Field("rm.Id", Ids, Operators.In) };
                List<BES.RejectedMails> items = bcMail.ListWithDesc(filters, "Seller, ClientName");

                string emails = string.Join(",", items.Select(x => $"'{x.Email}'"));
                BCS.MailBlacklist bcBlackList = new();
                filters = new() { new Field("EMail", emails, Operators.In) };
                IEnumerable<BES.MailBlacklist> blackListed = bcBlackList.List(filters, "1");
                foreach (var i in items)
                {
                    if (!blackListed.Any(x => x.EMail == i.Email))
                    {
                        BES.MailBlacklist reported = new() { EMail = i.Email, LogUser = UserCode, LogDate = DateTime.Now, StatusType = BEntities.StatusType.Insert };
                        bcBlackList.Save(ref reported);
                    }
                    BES.RejectedMails temp = i;
                    temp.Reported = true;
                    temp.LogUser = UserCode;
                    temp.LogDate = DateTime.Now;
                    temp.StatusType = BEntities.StatusType.Update;
                    bcMail.Save(ref temp);
                }

                BCS.UserData bcData = new();
                BES.UserData beData = bcData.SearchByUser(UserCode);

                string table = @"<table><tr class=""header""><td>Código</td><td>Cliente</td><td>Correo</td><td>Ejecutivo</td><td>Error reportado por el Exchange</td></tr>", signature = SetHTMLSafe(beData.Signature);
                List<string> used = new();
                foreach (var item in items)
                {
                    if (!used.Contains(item.Email))
                    {
                        table += $@"<tr><td>{item.ClientCode}</td><td>{item.ClientName}</td><td>{item.Email}</td><td>{item.Seller}</td><td>{item.ReasonDesc}</td></tr>";
                        used.Add(item.Email);
                    }
                }
                table += @"</table>";

                StringBuilder sb = new();
                sb.AppendLine(@"<html>");
                sb.AppendLine(@" <head>");
                sb.AppendLine(@"     <style>");
                sb.AppendLine(@"     body { font-size: 12px; } ");
                sb.AppendLine(@"     table { border-collapse: collapse; }");
                sb.AppendLine(@"      td { font-size: 11px; padding: 4px 8px; } ");
                sb.AppendLine(@"      tr:nth-child(even) {background: #CCC} ");
                sb.AppendLine(@"     .header { background-color: #5B9BD5; color: #FFF; } ");
                sb.AppendLine(@"     .header td { font-size: 13px; }");
                sb.AppendLine(@"     </style>");
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
                sb.AppendLine(@"     <p>Paul</p>");
                sb.AppendLine(@"     <p>Estos correos entran en lista negra hoy</p>");
                sb.AppendLine($@"    <p>{table}</p>");
                sb.AppendLine($@"    <p>{Comments}</p><br />");
                sb.AppendLine($@"    <p>{signature}</p>");
                sb.AppendLine(@"   </div>");
                sb.AppendLine(@" </body>");
                sb.AppendLine(@"</html>");

                List<MailAddress> recipients = new() { new MailAddress("Paul.Chang@dmc.bo", "Paul Chang") }, blindCopies = new() { new("julio.peredo@dmc.bo", "Julio C. Peredo F.") };
                //List<MailAddress> recipients = new() { new MailAddress("julio.peredo@gmail.com", "Paul Chang") }, blindCopies = new() { new("julio.peredo@dmc.bo", "Julio C. Peredo F.") };
                MailAddress sender = new("julio.peredo@dmc.bo", "Julio C. Peredo F.");
                await SendMailAsync("Lista Negra de Correos", sb.ToString(), recipients, null, blindCopies, sender);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion
    }
}
