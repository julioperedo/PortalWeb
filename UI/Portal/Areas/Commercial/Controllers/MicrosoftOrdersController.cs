using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Portal.Areas.Commercial.Models;
using Portal.Controllers;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Reporting.Processing;
using Telerik.Reporting;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BCX = BComponents.CIESD;
using BEA = BEntities.SAP;
using BEP = BEntities.Product;
using BEX = BEntities.CIESD;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class MicrosoftOrdersController : BaseController
    {

        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        #region Constructores

        public MicrosoftOrdersController(IConfiguration configuration, IWebHostEnvironment enviroment) : base(configuration, enviroment)
        {
            _config = configuration;
            _env = enviroment;
        }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                if (HomeCardCode == CardCode)
                {
                    short permission = GetPermission("ESD-ProductExit");
                    ViewBag.ProductExitPermission = permission > 0 ? "Y" : "N";
                    return View();
                }
                else
                {
                    BCX.AllowedClient bcAllowed = new();
                    List<Field> filters = new() { new Field("LOWER(CardCode)", CardCode.ToLower()) };
                    var alloweds = bcAllowed.List(filters, "1");
                    ViewBag.CardCode = CardCode;
                    ViewBag.CardName = CardName;
                    ViewBag.Title = $"Microsoft ESD de {CardName}";
                    if (alloweds?.Count() > 0)
                    {
                        BCA.Client bcClient = new();
                        var holdInfo = bcClient.SearchHoldInfo(CardCode);
                        ViewBag.OnHoldCredit = holdInfo.OnHoldCredit == 1;
                        ViewBag.OnHoldDue = holdInfo.OnHoldDue == 1;
                        return View("IndexClient");
                    }
                    else
                    {
                        return View("NotAllowed");
                    }
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetProducts()
        {
            IEnumerable<BEX.Product> products = default;
            IEnumerable<BEP.Price> prices = default;
            try
            {
                BCX.Product bcProd = new();
                products = bcProd.ListAvailable("Name");

                string codes = string.Join(",", products.Select(x => $"'{x.ItemCode.ToUpper()}'"));

                BCP.Price bcPrice = new();
                prices = bcPrice.ListByCode(codes, "1", BEP.relPrice.Product);
            }
            catch (Exception) { }
            //var items = products?.Select(x => new { Id = x.ItemCode, Name = $"( {x.ItemCode} ) {x.Name}" });
            var items = from p in products
                        join pr in prices on p.ItemCode.ToLower() equals pr.Product.ItemCode.ToLower()
                        select new { Id = p.ItemCode, Name = $"( {p.ItemCode} ) {p.Name}", Price = pr.Regular };
            return Json(items);
        }

        public IActionResult Filter(string Code, string ClientCode, string Type, long? ProductId, DateTime InitialDate, DateTime FinalDate)
        {
            string message = "";
            try
            {
                IEnumerable<BEX.Purchase> purchases = GetItems(Code, ClientCode, Type, ProductId, InitialDate, FinalDate);
                if (purchases?.Count() > 0)
                {
                    //string codes = string.Join(",", purchases.Select(x => $"'{x.CardCode}'").Distinct());
                    //BCA.Client bcClient = new();
                    //List<Field> filters = new() { new Field("LOWER(CardCode)", codes.ToLower(), Operators.In) };
                    //IEnumerable<BEA.Client> clients = bcClient.ListShort(filters, "1");

                    var items = from x in purchases
                                    //join c in clients on x.CardCode.ToLower() equals c.CardCode.ToLower() into ljClients
                                    //from l in ljClients
                                where x.ListTokens.Count(t => t.ListTokenReturns?.Any() ?? false) < x.Quantity || CardCode == HomeCardCode
                                select new
                                {
                                    x.Id,
                                    x.IdProduct,
                                    x.Code,
                                    x.CardCode,
                                    x.Currency,
                                    x.DocNumber,
                                    x.DocType,
                                    x.Price,
                                    x.Quantity,
                                    ProductName = $"( {x.Product.ItemCode} ) {x.Product.Name}",
                                    x.PurchaseDate,
                                    CardName = x.Client?.CardName ?? "",
                                    EMail = x.Client?.EMail ?? "",
                                    Anulled = x.ListTokens.Count(t => t.ListTokenReturns?.Any() ?? false),
                                    Sent = x.ListSentEmails?.Any(),
                                    Returnable = x.Product.ReturnType != "NotReturnable"
                                };
                    return Json(new { message, items });
                }
                else
                {
                    return Json(new { message, items = purchases });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetPurchase(long Id)
        {
            string message = "";
            try
            {
                BCX.Link bcLink = new();
                BCX.Token bcToken = new();
                //BCX.PurchaseReturn bcReturn = new();
                BCX.SentEmail bcSent = new();
                List<Field> filters = new() { new Field("IdPurchase", Id) };
                IEnumerable<BEX.Link> links = bcLink.List(filters, "1");
                IEnumerable<BEX.Token> tokens = bcToken.List(filters, "1");
                IEnumerable<BEX.SentEmail> sentEmails = bcSent.List(filters, "1");
                //IEnumerable<BEX.PurchaseReturn> anulled = bcReturn.List(filters, "1");
                //return Json(new { message, links, tokens, sentEmails, anulled });
                return Json(new { message, links, tokens, sentEmails });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetTokens(long Id)
        {
            string message = "";
            try
            {
                BCX.Token bcToken = new();
                IEnumerable<BEX.Token> items = bcToken.List(new() { new Field("IdPurchase", Id) }, "1");
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public async Task<IActionResult> GetProductToken(string ItemCode, string ClientCode, string PurchaseCode, int OrderNumber, string DocType)
        {
            string message = "";
            try
            {
                var settings = _config.GetSection("MicrosoftESDSettings").Get<MicrosoftESDSettings>();

                string tokenData, content;
                HttpClient client = new();
                HttpContent userData = new StringContent(JsonConvert.SerializeObject(new { settings.UserName, settings.Password }), Encoding.UTF8);
                userData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync($"{settings.BaseURL}/Security/GetToken", userData);
                TokenResponse token;
                if (response.IsSuccessStatusCode)
                {
                    tokenData = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<TokenResponse>(tokenData);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    content = await client.GetStringAsync($"{settings.BaseURL}/Product/GetProductToken?ItemCode={ItemCode}&CardCode={ClientCode}&PurchaseId={PurchaseCode}&DocNumber={OrderNumber}&UserId={UserCode}&DocType={DocType}");

                    if (!string.IsNullOrEmpty(content))
                    {
                        ServiceResponse<ProductLicence> data = JsonConvert.DeserializeObject<ServiceResponse<ProductLicence>>(content);
                        if (data != null)
                        {
                            if (string.IsNullOrEmpty(data.Message))
                            {
                                BCX.Purchase bcPurchase = new();
                                BEX.Purchase p = bcPurchase.Search(data.Item.Id, BEX.relPurchase.Links, BEX.relPurchase.Tokens);
                                BCA.Client bcClient = new();
                                BEA.Client beClient = bcClient.Search(p.CardCode);

                                var item = new
                                {
                                    p.Id,
                                    p.Code,
                                    p.CardCode,
                                    beClient.CardName,
                                    EMail = beClient.EMail ?? "",
                                    p.Currency,
                                    p.IdProduct,
                                    p.DocNumber,
                                    p.DocType,
                                    p.Price,
                                    p.PurchaseDate,
                                    p.Quantity,
                                    Tokens = data.Item.Tokens.Select(x => new { x.Id, x.Code, x.SequenceNumber, x.Type, x.Description, TransactionNumber = data.Item.TransactionId }),
                                    data.Item.Links
                                };
                                if (OrderNumber > 0)
                                {
                                    try
                                    {
                                        List<MailAddress> copies = new(), blindCopies = new();
                                        FillCustomCopies("Commercial", "MicrosoftOrders", "GetProductToken", ref copies, ref blindCopies);
                                        _ = NotifyOrder(OrderNumber, beClient.CardName, beClient.NIT, beClient.EMail, copies, blindCopies, IsDevelopmentMode);
                                    }
                                    catch (Exception) { }
                                }
                                return Json(new { message, item });
                            }
                            else
                            {
                                message = data.Message;
                            }
                        }
                        else
                        {
                            message = "No se obtuvo resultados del servidor";
                        }
                    }
                    else
                    {
                        message = "No se obtuvo resultados del servidor";
                    }
                }
                else
                {
                    message = "El servidor no responió correctamente";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public async Task<IActionResult> SendEMailToClient(long Id, string EMail, string Name)
        {
            string message = "";
            try
            {
                string mailBody;
                BCX.Purchase bcPurchase = new();
                BEX.Purchase item = bcPurchase.Search(Id, BEX.relPurchase.Product, BEX.relPurchase.Tokens, BEX.relPurchase.Links);

                BCA.Client bcClient = new();
                BEA.Client client = bcClient.Search(item.CardCode) ?? new BEA.Client { CardCode = item.CardCode, CardName = "", EMail = "" };

                StringBuilder sb = new();
                sb.AppendLine($@"<p><b>C&oacute;digo:</b> {item.Code} <br />");
                sb.AppendLine($@"<b>Producto:</b> ( {item.Product.ItemCode} ) {item.Product.Name} <br />");
                //sb.AppendLine($@"<b>No. de Transacci&oacute;n:</b> {item.TransactionNumber} <br />");
                sb.AppendLine($@"<b>Fecha compra:</b> {item.PurchaseDate:dd-MM-yyyy HH:mm} </p>");
                if (item.ListTokens?.Count > 0 || item.ListLinks?.Count > 0)
                {
                    sb.AppendLine($@"<table style=""width: 100%;"">");
                    int countTokens = item.ListTokens?.Count ?? 0, countLinks = item.ListLinks?.Count ?? 0;
                    for (int i = 0; i < countTokens; i++)
                    {
                        BEX.Token t = item.ListTokens[i];
                        BEX.Link l = countLinks > 0 ? item.ListLinks[i] : default;
                        string styleColor = i % 2 == 1 ? @" background-color: #eaeaea;" : "";
                        sb.AppendLine($@"<tr><td style=""padding: 8px;{styleColor}""><b>C&oacute;digo:</b> {t.Code}<br /><b>Descripci&oacute;n:</b> {t.Description}<br />");
                        if (l != null)
                        {
                            sb.AppendLine($@"<a href=""{l.Uri}"" class=""action action-link"">V&iacute;nculo</a>&nbsp;&nbsp;&nbsp;{l.Description}&nbsp;&nbsp;&nbsp;<b>Expira:</b> {(l.ExpirationDate.HasValue ? l.ExpirationDate : ""):dd-MM-yyyy}<br />");
                        }
                        sb.AppendLine($@"<b>No. Transacci&oacute;n:</b> {t.TransactionNumber}</td></tr>");
                    }
                    sb.Append(@"</table>");
                }

                mailBody = $@"<!DOCTYPE html>
                <html>
                <head>
                	<meta charset=""utf-8"" />
                	<!--[if !mso]><!-->
                	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
                	<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" /> <!--<![endif]-->
                	<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                	<title></title>
                    <link href=""http://fonts.googleapis.com/css?family=Montserrat"" rel=""stylesheet"" type=""text/css"">
                	<style type=""text/css"">
                        body {{ font-size: 12px; }}
                		.align-center {{ margin: 0 auto; }}
                		.text-align-center {{ text-align: center; }}
                		.text-align-left {{ text-align: left; }}
                		.text-align-right {{ text-align: right; }}
                		a[x-apple-data-detectors=true] {{ text-decoration: none !important; color: inherit !important; }}
                		div[style*=""margin: 16px 0""] {{ margin: 0 !important; }}
                		@media screen and (min-width: 370px) and (max-width: 499px) {{
                			.outer {{ width: 85% !important; margin: 0 auto !important; }}
                		}}
                		p {{ margin-bottom: 0px; }}
                		.subtitle {{ font-size: 1.3em; font-weight: 600; }}
                		.footer a {{ text-decoration: none; color: #48545E; font-weight: 600; }}
                        .image {{ max-height: 220px; max-width: 220px; }}
                        td {{ color: rgb(90, 90, 90); font-family: 'Montserrat', Verdana, Geneva, sans-serif; line-height: 1.4em; }}
                	</style>
                	<!--[if(mso)|(IE)]>
                	<style type=""text/css"">
                		table {{ border-collapse: collapse !important; mso-table-lspace:0pt !important; mso-table-rspace:0pt !important; }}
                		table, div, td {{ font-family: 'Montserrat', Verdana, Geneva, sans-serif !important; }}
                	</style>
                	<![endif]-->
                	<!--[if mso]>
                		<style type=""text/css"">
                			ol, ul {{ margin-left: 25px !important; margin-top: 0 !important; }}
                		</style>
                		<xml>
                		  <o:OfficeDocumentSettings>
                			<o:AllowPNG/>
                			<o:PixelsPerInch>96</o:PixelsPerInch>
                		 </o:OfficeDocumentSettings>
                		</xml>
                		<![endif]-->
                </head>
                <body style=""margin: 0px; padding: 0px; font-family: 'Montserrat', Verdana, Geneva, sans-serif; background-color: rgb(217, 217, 214); -webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale;"">
                	<table align=""center"" class=""wrapper"" style=""margin: 0px auto; width: 100%; font-family: 'Montserrat', Verdana, Geneva, sans-serif; table-layout: fixed; -ms-text-size-adjust: 100%;color: rgb(148, 148, 148); background-color: rgb(217, 217, 214); -webkit-text-size-adjust: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
                		<tbody>
                			<tr>
                				<td align=""center"">
                					<table class=""outer"" style=""margin: 0px auto; width: 100%; max-width: 740px;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
                						<tbody>
                							<tr>
                								<td id=""main-container"" bgcolor=""#ffffff"">
                									<table style=""text-align: left; color: rgb(148, 148, 148); line-height: 20px; width:100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
                										<tbody>
                											<tr>
                												<td style=""padding:25px 20px 20px 20px;"">
                													<a href=""http://www.dmc.bo/"" style=""text-decoration:none;""><img src=""https://portal.dmc.bo/images/logo3.png"" height=""50"" alt=""DMC S.A.""></a>
                												</td>
                                                                <td style=""padding:25px 20px 20px 20px;text-align: right;""><img src=""https://portal.dmc.bo/images/lines/microsoft.png"" height=""50"" alt=""Microsoft""></td>
                											</tr>
                											<tr>
                												<td colspan=""2"">
                													<table style=""width: 100%;"">
                														<tr>
                															<td style=""width: 20px;"">&nbsp;</td>
                															<td>
                																<p>Estimado Cliente:<br />
                                                                                <b>({client.CardCode}) {client.CardName}</b> 
                                                                                </p>
                																<p>Se deja expresa constancia que en la presente venta de Software, El Cliente declara tener pleno conocimiento y acepta que la descarga y uso se realiza on line, por lo que se entiende por entregado, utilizado y consumido (el software adquirido), en el preciso instante en que se ingresa el código o clave de descarga y acceso proporcionado a El Cliente, ya que desde ese instante el software entra en uso, no existiendo la posibilidad de devolución del mismo, naciendo la obligación de pago de acuerdo a contratos vigente y factura respectiva.<br />Los datos de la licencia adquirida son:</p>{sb}
                															</td>
                															<td style=""width: 20px;"">&nbsp;</td>
                														</tr>
                														<tr>
                															<td colspan=""3"">&nbsp;</td>
                														</tr>
                													</table>
                												</td>
                											</tr>
                										</tbody>
                									</table>
                								</td>
                							</tr>
                							<tr>
                								<td class=""footer"">
                                                    <table style=""width: 100%;"">
                                                        <tr>
                                                            <td style=""width: 20px;"">&nbsp;</td>
                                                            <td style=""text-align: center; font-size: 0.9em;"">
                                                                <br /><b>DMC S.A.</b> - Av. Grigota # 3800, Santa Cruz - Bolivia<br />
                									            <b>DMC LATIN AMERICA INC.</b> - 9935 NW 88 Ave., Miami, FL 33178<br />
                									            <b>IMPORTADORA DMC IQUIQUE LTDA</b> - Recinto Amurallado, Manzana 12, Galpón 8 (Zona Franca Iquique), Iquique - Chile<br />
                									            Visite nuestra lista online en <a href=""http://portal.dmc.bo/Product/PriceList"">portal.dmc.bo</a>                									
                                                            </td>
                                                            <td style=""width: 20px;"">&nbsp;</td>
                                                        </tr>
                                                    </table>                									
                								</td>
                							</tr>
                							<tr><td>&nbsp;</td></tr>
                						</tbody>
                					</table>
                				</td>
                			</tr>
                		</tbody>
                	</table>
                </body>
                </html>";

                string email = EMail, name = Name;
                List<MailAddress> lstTos = new(), lstCopies = new(), lstBlindCopies = new();
                if (string.IsNullOrWhiteSpace(name)) name = IsDevelopmentMode ? "Julio C Peredo F" : client.CardName;
                if (string.IsNullOrWhiteSpace(email)) email = IsDevelopmentMode ? "julio.peredo@gmail.com" : client.EMail;
                FillCustomCopies("Commercial", "MicrosoftOrders", "SendEMailToClient", ref lstCopies, ref lstBlindCopies);
                lstTos.Add(new MailAddress(email, name));
                if (!string.IsNullOrWhiteSpace(email))
                {
                    await SendMailAsync("Detalle Llaves adquiridas a Microsoft", mailBody, lstTos, lstCopies, lstBlindCopies);

                    BCX.SentEmail bcEmail = new();
                    BEX.SentEmail sent = new() { StatusType = BEntities.StatusType.Insert, IdPurchase = Id, Name = name, EMail = email, Type = "E", LogUser = UserCode, LogDate = DateTime.Now };
                    bcEmail.Save(ref sent);
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public async Task<IActionResult> ReturnLicence(string LicenceCode, long PurchaseId)
        {
            string message = "";
            try
            {
                var settings = _config.GetSection("MicrosoftESDSettings").Get<MicrosoftESDSettings>();

                string tokenData, content;
                HttpClient client = new();
                HttpContent userData = new StringContent(JsonConvert.SerializeObject(new { settings.UserName, settings.Password }), Encoding.UTF8);
                userData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync($"{settings.BaseURL}/Security/GetToken", userData);
                TokenResponse token;
                if (response.IsSuccessStatusCode)
                {
                    tokenData = await response.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<TokenResponse>(tokenData);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    content = await client.GetStringAsync($"{settings.BaseURL}/Product/ReturnToken?LicenceCode={LicenceCode}&PurchaseId={PurchaseId}");
                    if (!string.IsNullOrEmpty(content))
                    {
                        ServiceResponse<ReturnLicence> data = JsonConvert.DeserializeObject<ServiceResponse<ReturnLicence>>(content);
                        if (data != null)
                        {
                            if (data.Message == "")
                            {
                                //_ = SendCacellationEMail(PurchaseId);
                                var item = new { data.Item.ClientTransactionId, data.Item.ServiceTransactionId };
                                return Json(new { message, item });
                            }
                            else
                            {
                                message = data.Message;
                            }
                        }
                        else
                        {
                            message = "No se obtuvo resultados del servidor";
                        }
                    }
                    else
                    {
                        message = "No se obtuvo resultados del servidor";
                    }
                }
                else
                {
                    message = "El servidor no responió correctamente";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        //public async Task<IActionResult> SendCacellationEMail(long IdPurchase)
        //{
        //    string message = "";
        //    try
        //    {
        //        BCX.Purchase bcPurchase = new();
        //        BEX.Purchase item = bcPurchase.Search(IdPurchase, BEX.relPurchase.Product, BEX.relPurchase.Tokens, BEX.relPurchase.PurchaseReturns, BEX.relPurchase.SentEmails);

        //        BEX.PurchaseReturn anulled = item.ListPurchaseReturns?.FirstOrDefault() ?? new BEX.PurchaseReturn();

        //        BCA.Client bcClient = new();
        //        BEA.Client client = bcClient.Search(item.CardCode) ?? new BEA.Client { CardCode = item.CardCode, CardName = "", EMail = "" };

        //        StringBuilder sb = new();
        //        if (item.ListTokens?.Count() > 0)
        //        {
        //            foreach (var token in item.ListTokens)
        //            {
        //                sb.AppendLine($@"<li><b>{token.Id}</b></li>");
        //            }
        //        }

        //        string mailBody = $@"<!DOCTYPE html>
        //        <html>
        //        <head>
        //        	<meta charset=""utf-8"" />
        //        	<!--[if !mso]><!-->
        //        	<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
        //        	<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" /> <!--<![endif]-->
        //        	<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        //        	<title></title>
        //            <link href=""http://fonts.googleapis.com/css?family=Montserrat"" rel=""stylesheet"" type=""text/css"">
        //        	<style type=""text/css"">
        //                body {{ font-size: 12px; }}
        //        		.align-center {{ margin: 0 auto; }}
        //        		.text-align-center {{ text-align: center; }}
        //        		.text-align-left {{ text-align: left; }}
        //        		.text-align-right {{ text-align: right; }}
        //        		a[x-apple-data-detectors=true] {{ text-decoration: none !important; color: inherit !important; }}
        //        		div[style*=""margin: 16px 0""] {{ margin: 0 !important; }}
        //        		@media screen and (min-width: 370px) and (max-width: 499px) {{
        //        			.outer {{ width: 85% !important; margin: 0 auto !important; }}
        //        		}}
        //        		p {{ margin-bottom: 0px; }}
        //        		.subtitle {{ font-size: 1.3em; font-weight: 600; }}
        //        		.footer a {{ text-decoration: none; color: #48545E; font-weight: 600; }}
        //                .image {{ max-height: 220px; max-width: 220px; }}
        //                td {{ color: rgb(90, 90, 90); font-family: 'Montserrat', Verdana, Geneva, sans-serif; line-height: 1.4em; }}
        //                .table-resume {{ width: 100%; border-spacing: initial; margin-bottom: 12px; }}
        //                .table-resume thead {{ font-weight: 600; background-color: #eee; }}
        //                .table-resume thead td {{ padding-top: 3px; }}
        //                .table-resume td {{ font-size: 11px; }}
        //        	</style>
        //        	<!--[if(mso)|(IE)]>
        //        	<style type=""text/css"">
        //        		table {{ border-collapse: collapse !important; mso-table-lspace:0pt !important; mso-table-rspace:0pt !important; }}
        //        		table, div, td {{ font-family: 'Montserrat', Verdana, Geneva, sans-serif !important; }}
        //        	</style>
        //        	<![endif]-->
        //        	<!--[if mso]>
        //        		<style type=""text/css"">
        //        			ol, ul {{ margin-left: 25px !important; margin-top: 0 !important; }}
        //        		</style>
        //        		<xml>
        //        		  <o:OfficeDocumentSettings>
        //        			<o:AllowPNG/>
        //        			<o:PixelsPerInch>96</o:PixelsPerInch>
        //        		 </o:OfficeDocumentSettings>
        //        		</xml>
        //        		<![endif]-->
        //        </head>
        //        <body style=""margin: 0px; padding: 0px; font-family: 'Montserrat', Verdana, Geneva, sans-serif; background-color: rgb(217, 217, 214); -webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale;"">
        //        	<table align=""center"" class=""wrapper"" style=""margin: 0px auto; width: 100%; font-family: 'Montserrat', Verdana, Geneva, sans-serif; table-layout: fixed; -ms-text-size-adjust: 100%;color: rgb(148, 148, 148); background-color: rgb(217, 217, 214); -webkit-text-size-adjust: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
        //        		<tbody>
        //        			<tr>
        //        				<td align=""center"">
        //        					<table class=""outer"" style=""margin: 0px auto; width: 100%; max-width: 740px;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
        //        						<tbody>
        //        							<tr>
        //        								<td id=""main-container"" bgcolor=""#ffffff"">
        //        									<table style=""text-align: left; color: rgb(148, 148, 148); line-height: 20px; width:100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
        //        										<tbody>
        //        											<tr>
        //        												<td style=""padding:25px 20px 20px 20px;"">
        //        													<a href=""http://www.dmc.bo/"" style=""text-decoration:none;""><img src=""https://portal.dmc.bo/images/logo3.png"" height=""50"" alt=""DMC S.A.""></a>
        //        												</td>
        //                                                        <td style=""padding:25px 20px 20px 20px;text-align: right;""><img src=""https://portal.dmc.bo/images/lines/microsoft.png"" height=""50"" alt=""Microsoft""></td>
        //        											</tr>
        //        											<tr>
        //        												<td colspan=""2"">
        //        													<table style=""width: 100%;"">
        //        														<tr>
        //        															<td style=""width: 20px;"">&nbsp;</td>
        //        															<td>
        //        																<p>Estimado Cliente:<br />
        //                                                                        <b>({client.CardCode}) {client.CardName}</b> 
        //                                                                        </p>
        //        																<p>La licencia adquirida <b>{item.Id}</b> en fecha <b>{item.PurchaseDate:dd-MM-yyyy}</b> para <b>{item.Product.Name}</b> ha sido anulada.</p>
        //                                                                        <p>Por consiguiente las siguientes llaves ya no son operativas: <ul>{sb}</ul></p>
        //                                                                        <p>Datos de la anulaci&oacute;n:<br /> Id Transacci&oacute;n: <b>{anulled.ServiceTransactionId}</b><br /> Id Transacci&oacute;n Cliente: <b>{anulled.ClientTransactionId}</b></p>
        //        															</td>
        //        															<td style=""width: 20px;"">&nbsp;</td>
        //        														</tr>
        //        														<tr>
        //        															<td colspan=""3"">&nbsp;</td>
        //        														</tr>
        //        													</table>
        //        												</td>
        //        											</tr>
        //        										</tbody>
        //        									</table>
        //        								</td>
        //        							</tr>
        //        							<tr>
        //        								<td class=""footer"">
        //                                            <table style=""width: 100%;"">
        //                                                <tr>
        //                                                    <td style=""width: 20px;"">&nbsp;</td>
        //                                                    <td style=""text-align: center; font-size: 0.9em;"">
        //                                                        <br /><b>DMC S.A.</b> - Av. Grigota # 3800, Santa Cruz - Bolivia<br />
        //        									            <b>DMC LATIN AMERICA INC.</b> - 9935 NW 88 Ave., Miami, FL 33178<br />
        //        									            <b>IMPORTADORA DMC IQUIQUE LTDA</b> - Recinto Amurallado, Manzana 12, Galpón 8 (Zona Franca Iquique), Iquique - Chile<br />
        //        									            Visite nuestra lista online en <a href=""http://portal.dmc.bo/Product/PriceList"">portal.dmc.bo</a>                									
        //                                                    </td>
        //                                                    <td style=""width: 20px;"">&nbsp;</td>
        //                                                </tr>
        //                                            </table>                									
        //        								</td>
        //        							</tr>
        //        							<tr><td>&nbsp;</td></tr>
        //        						</tbody>
        //        					</table>
        //        				</td>
        //        			</tr>
        //        		</tbody>
        //        	</table>
        //        </body>
        //        </html>";

        //        long userCode = UserCode;
        //        List<MailAddress> lstTos = new(), lstCopies = new(), lstBlindCopies = new();
        //        if (IsDevelopmentMode)
        //        {
        //            lstTos.Add(new MailAddress("julio.peredo@gmail.com", "Julio Peredo"));
        //            lstBlindCopies.Add(new MailAddress("julio.peredo@dmc.bo", "Julio C Peredo F"));
        //        }
        //        else
        //        {
        //            lstTos = item.ListSentEmails.Select(x => new MailAddress(x.EMail, x.Name)).Distinct().ToList();
        //            FillCustomCopies("Commercial", "MicrosoftOrders", "SendEMailToClient", ref lstCopies, ref lstBlindCopies);
        //        }
        //        await SendMailAsync("Anulación de Llaves adquiridas a Microsoft", mailBody, lstTos, lstCopies, lstBlindCopies);

        //        BCX.SentEmail bcEmail = new();
        //        IList<BEX.SentEmail> sent = item.ListSentEmails.Select(x => new BEX.SentEmail { StatusType = BEntities.StatusType.Insert, IdPurchase = IdPurchase, Name = x.Name, EMail = x.EMail, Type = "D", LogUser = userCode, LogDate = DateTime.Now }).ToList();
        //        bcEmail.Save(ref sent);
        //    }
        //    catch (Exception ex)
        //    {
        //        message = GetError(ex);
        //    }
        //    return Json(new { message });
        //}

        public IActionResult ValidateClient(string CardCode)
        {
            string message = "";
            bool enabled = false, onHoldCredit = true, onHoldDue = true;
            try
            {
                BCX.AllowedClient bcAllowed = new();
                List<Field> filters = new() { new Field("LOWER(CardCode)", CardCode.ToLower()) };
                var alloweds = bcAllowed.List(filters, "1");

                BCA.Client bcClient = new();
                var holdInfo = bcClient.SearchHoldInfo(CardCode);
                onHoldCredit = holdInfo.OnHoldCredit == 1;
                onHoldDue = holdInfo.OnHoldDue == 1;

                enabled = alloweds?.Count() > 0;
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, enabled, onHoldCredit, onHoldDue });
        }

        public IActionResult ValidateOrder(string CardCode, string ItemCode, int Quantity, int OrderNumber)
        {
            string message = "";
            bool valid = true;
            try
            {
                BCA.Order bcOrder = new();
                var order = bcOrder.Search(OrderNumber, "Santa Cruz", BEA.relOrder.OrderItems);
                if (order == null)
                {
                    valid = false;
                    message += $"<br />No existe la Orden de Venta <b>{OrderNumber}</b>.";
                }
                else
                {
                    if (order.ClientCode.ToLower() != CardCode.ToLower())
                    {
                        valid = false;
                        message += $"<br />La OV <b>{OrderNumber}</b> no pertenece al cliente <b>{CardCode}</b>.";
                    }
                    else
                    {
                        BCX.Purchase bcPurchase = new();
                        List<Field> filters = new() { new Field("DocNumber", OrderNumber), new Field("DocType", "SO"), new Field(LogicalOperators.And) };
                        IEnumerable<BEX.Purchase> purchases = bcPurchase.List(filters, "1", BEX.relPurchase.Product);
                        int quantityOnPurchases = (from p in purchases where p.Product.ItemCode.ToLower() == ItemCode.ToLower() select p.Quantity).Sum();
                        string numbers = string.Join(", ", from p in purchases where p.Product.ItemCode.ToLower() == ItemCode.ToLower() select $"{p.Code} ({p.Quantity})");

                        int quantity = (from i in order.Items where i.ItemCode.ToLower() == ItemCode.ToLower() select i.Quantity).Sum();
                        if (quantity < (Quantity + quantityOnPurchases))
                        {
                            valid = false;
                            message += $"<br />La OV <b>{OrderNumber}</b> contiene menos licencias ({quantity}) que las que quiere adquirir{(quantityOnPurchases > 0 ? $", se ha usado en la adquisición de licencias: {numbers}" : "")}.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, valid });
        }

        public IActionResult ExportToExcel(string Code, string ClientCode, string Type, long? ProductId, DateTime InitialDate, DateTime FinalDate)
        {
            var purchases = GetItems(Code, ClientCode, Type, ProductId, InitialDate, FinalDate);

            BCA.Client bcClient = new();

            //var resume = from p in purchases
            //             group p by p.IdProduct into g
            //             select new { IdProduct = g.Key, g.First().Product.Sku, g.First().Product.Name, Count = g.Sum(x => x.Quantity), ReturnedCount = g.Count(x => x.ListTokens.Any(y => y.ListTokenReturns?.Any() ?? false)) };

            List<Attachment> files = new();
            using ExcelPackage objExcel = new();
            ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");

            wsMain.Cells.Style.Font.Size = 10;

            wsMain.Row(1).Style.Font.Size = 11;
            wsMain.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            wsMain.Row(1).Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
            wsMain.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            wsMain.Row(1).Style.Font.Bold = true;
            wsMain.Row(1).Height = 24;

            int col = 1, row = 1;
            wsMain.Cells[row, col].Value = "Invoice Number";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 255, 80, 80);
            wsMain.Cells[row, col].Value = "Invoice Date";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 255, 80, 80);
            wsMain.Cells[row, col].Value = "SB Name";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 146, 208, 80);
            wsMain.Cells[row, col].Value = "SB ID";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 217, 217, 217);
            wsMain.Cells[row, col].Value = "SB Address Line 1";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 255, 80, 80);
            wsMain.Cells[row, col].Value = "SB City";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 146, 208, 80);
            wsMain.Cells[row, col].Value = "SB State";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 217, 217, 217);
            wsMain.Cells[row, col].Value = "SB Country Code";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 146, 208, 80);
            wsMain.Cells[row, col++].Value = "# Doc.";
            wsMain.Cells[row, col++].Value = "Tipo de Doc.";
            wsMain.Cells[row, col].Value = "Sale Date";
            wsMain.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            wsMain.Cells[row, col++].Value = "Reseller";
            wsMain.Cells[row, col++].Value = "Reseller Address";
            wsMain.Cells[row, col++].Value = "Reseller City";
            wsMain.Cells[row, col++].Value = "Reseller Country";
            wsMain.Cells[row, col].Value = "MS Part Number";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 255, 80, 80);
            wsMain.Cells[row, col++].Value = "UPC";
            wsMain.Cells[row, col].Value = "MS Product Name";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 146, 208, 80);
            wsMain.Cells[row, col].Value = "Quantity Sold";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 146, 208, 80);
            wsMain.Cells[row, col].Value = "Quantity Returned";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 217, 217, 217);
            wsMain.Cells[row, col].Value = "Microsoft Agreement Number";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 217, 217, 217);
            wsMain.Cells[row, col].Value = "Discount Price";
            wsMain.Cells[row, col++].Style.Fill.BackgroundColor.SetColor(100, 217, 217, 217);
            wsMain.Cells[row, col].Value = "Promotion Number";
            wsMain.Cells[row++, col++].Style.Fill.BackgroundColor.SetColor(100, 217, 217, 217);

            foreach (var item in purchases)
            {
                col = 3;
                wsMain.Cells[row, col++].Value = "DMC";
                col++;
                wsMain.Cells[row, col++].Value = "Av. Grigota # 3800";
                wsMain.Cells[row, col++].Value = "Santa Cruz de la Sierra";
                wsMain.Cells[row, col++].Value = "Santa Cruz";
                wsMain.Cells[row, col++].Value = "BO";
                wsMain.Cells[row, col++].Value = item.DocNumber;
                wsMain.Cells[row, col++].Value = item.DocType == "SO" ? "Orden de Venta" : "Baja de Mercadería";
                wsMain.Cells[row, col].Value = item.PurchaseDate;
                wsMain.Cells[row, col++].Style.Numberformat.Format = "dd/MM/yyyy";
                wsMain.Cells[row, col++].Value = $"{item.CardCode} - {item.Client?.CardName ?? ""}";
                wsMain.Cells[row, col++].Value = item.Client?.Address ?? "";
                wsMain.Cells[row, col++].Value = item.Client?.City ?? "";
                wsMain.Cells[row, col++].Value = "BO";
                wsMain.Cells[row, col++].Value = item.Product.Sku;
                wsMain.Cells[row, col++].Value = item.Product.UPC.Contains("UPC:") ? item.Product.UPC.Replace("UPC:", "") : "";
                wsMain.Cells[row, col++].Value = item.Product.Name;
                wsMain.Cells[row, col++].Value = item.Quantity;
                wsMain.Cells[row++, col++].Value = item.ListTokens.Count(x => x.ListTokenReturns?.Any() ?? false);
            }

            wsMain.Cells.AutoFitColumns();

            string strFileName = $"Resumen-Pedidos-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
            byte[] objData = objExcel.GetAsByteArray();
            objExcel.Dispose();
            return File(objData, "application/xlsx", strFileName);
        }

        public IActionResult GetPurchaseTokens(long Id)
        {
            string message = "";
            try
            {
                BCX.Token bcToken = new();
                List<Field> filters = new() { new Field("IdPurchase", Id) };
                IEnumerable<BEX.Token> tokens = bcToken.List(filters, "1", BEX.relToken.TokenReturns);
                var items = tokens.Select(x => new { x.Id, Number = x.TransactionNumber, Sequence = x.SequenceNumber, x.Type, x.Code, x.ListTokenReturns?.FirstOrDefault()?.ReturnDate });
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

        private IEnumerable<BEX.Purchase> GetItems(string Code, string ClientCode, string Type, long? ProductId, DateTime InitialDate, DateTime FinalDate)
        {
            BCX.Purchase bcPurchase = new();
            const string dateFormat = "yyyy-MM-dd";
            List<Field> filters = new()
            {
                new Field("CAST(PurchaseDate AS DATE)", InitialDate.ToString(dateFormat), Operators.HigherOrEqualThan),
                new Field("CAST(PurchaseDate AS DATE)", FinalDate.ToString(dateFormat), Operators.LowerOrEqualThan)
            };
            if (!string.IsNullOrEmpty(Code)) filters.Add(new Field("Code", Code));
            if (!string.IsNullOrEmpty(ClientCode)) filters.Add(new Field("LOWER(CardCode)", ClientCode.ToLower()));
            if (Type != "B") filters.Add(new Field("DocType", Type));
            if (ProductId.HasValue) filters.Add(new Field("IdProduct", ProductId.Value));
            CompleteFilters(ref filters);
            IEnumerable<BEX.Purchase> items = bcPurchase.List2(filters, "1", BEX.relPurchase.Product, BEX.relPurchase.Tokens, BEX.relPurchase.SentEmails, BEX.relToken.TokenReturns) ?? new List<BEX.Purchase>();
            if (items.Any())
            {
                //if (CardCode != HomeCardCode) items = items.Where(x => x.ListTokens.Count(y => y.ListTokenReturns == null || y.ListTokenReturns.Count == 0) > 0);
                string codes = string.Join(",", items.Select(x => $"'{x.CardCode}'").Distinct());
                BCA.Client bcClient = new();
                filters = new() { new Field("LOWER(CardCode)", codes.ToLower(), Operators.In) };
                IEnumerable<BEA.Client> clients = bcClient.List(filters, "1");

                codes = string.Join(",", items.Select(x => $"'{x.Product.ItemCode.ToLower()}'").Distinct());
                filters = new() { new Field("LOWER(ItemCode)", codes, Operators.In) };
                BCP.Product bcProduct = new();
                var products = bcProduct.List(filters, "1");

                foreach (var item in items)
                {
                    item.Client = clients.First(x => x.CardCode.ToLower() == item.CardCode.ToLower());
                    item.Product.UPC = products.FirstOrDefault(x => x.ItemCode.ToLower() == item.Product.ItemCode.ToLower())?.Description ?? "";
                }
            }
            return items;
        }

        private async Task NotifyOrder(long OrderNumber, string Name, string NIT, string EMail, List<MailAddress> CopiesTo, List<MailAddress> BlindCopiesTo, bool Test)
        {
            string mailBody = $@"<!DOCTYPE html>
                            <html lang=""en"" xmlns=""https://www.w3.org/1999/xhtml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
                            <head>
                                <meta charset=""utf-8"">
                                <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
                                <meta name=""x-apple-disable-message-reformatting"">
                                <title></title>
                                <!--[if mso]>
                                <style>
                                   body {{ background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; }} 
                                   img {{ margin: 20px 15px; }} 
                                   td {{ padding: 0 8px; line-height: 18px; }} 
                                    table {{ border-collapse:collapse;border-spacing:0;border:none;margin:0; }}
                                    div, td {{ padding:0; }}
                                    div {{ margin:0 !important; }}
                                </style>
                                <noscript>
                                    <xml>
                                        <o:OfficeDocumentSettings>
                                        <o:PixelsPerInch>96</o:PixelsPerInch>
                                        </o:OfficeDocumentSettings>
                                    </xml>
                                </noscript>
                                <![endif]-->
                            </head>
                            <body style=""margin:0;padding:0;word-spacing:normal;background-color:#fff;"">                                
                                <div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"">
                                  <table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse;"">
                                    <tr>
                                        <td style=""width: 20px;"">&nbsp;</td>
                                        <td style=""height:130px"">
                                            <br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />
                                        </td>
                                        <td style=""width: 20px;"">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style=""width: 20px;"">&nbsp;</td>
                                        <td>
                                            {(Test ? @"<p style=""font-size: 18px;""><strong>ORDEN DE PRUEBA (GENERADA EN BASE DE PRUEBA)</strong></p>" : "")}
                                            <p>Orden generada: <strong>{OrderNumber}</strong><p/>
                                            <p>La orden se ha creado con los siguientes datos:<br />
                                                <table>
                                                    <tr><td>Nombre Cliente Final:</td><td><strong>{Name}</strong> </td></tr>
                                                    <tr><td>NIT Cliente Final:</td><td><strong>{NIT}</strong></td></tr>
                                                    <tr><td>E-Mail Cliente Final:</td><td><strong>{EMail}</strong> </td></tr>
                                                </table>
                                            </p>
                                            <p>El resto de los datos se encuentran en el archivo adjunto.</p>
                                            <p>Atentamente<br />El equipo de DMC</p><br />
                                        </td>
                                        <td style=""width: 20px;"">&nbsp;</td>
                                    </tr>
                                  </table>                
                                </div>
                            </body>
                            </html>";

            string fileName = $"OrdenVenta-{OrderNumber}.pdf", title = $"Orden Generada de Licencias Microsoft {OrderNumber}";
            List<Attachment> files = new() { new Attachment(GetReport((int)OrderNumber, "Santa Cruz"), fileName) };

            var bcBlackList = new BCS.MailBlacklist();
            var lstBlackList = bcBlackList.List("1");
            var blackList = lstBlackList.Select(x => x.EMail);

            List<MailAddress> destinataries = new(), blindCopies = new();
            foreach (var c in CopiesTo)
            {
                if (IsValidEmail(c.Address) & !blackList.Contains(c.Address))
                {
                    destinataries.Add(new MailAddress(c.Address, c.DisplayName));
                }
            }
            foreach (var c in BlindCopiesTo)
            {
                if (IsValidEmail(c.Address) & !blackList.Contains(c.Address))
                {
                    blindCopies.Add(new MailAddress(c.Address, c.DisplayName));
                }
            }
            await SendMailAsync(title, mailBody, destinataries, null, blindCopies, null, files);
        }

        private Stream GetReport(int DocNum, string Subsidiary)
        {
            string /*userName = "Generado Automáticamente",*/ filePath;
            Telerik.Reporting.Report report;
            var reportPackager = new ReportPackager();

            filePath = Path.Combine(_env.WebRootPath, "reports", "SaleOrder.trdp");
            using (var sourceStream = System.IO.File.OpenRead(filePath))
            {
                report = (Telerik.Reporting.Report)reportPackager.UnpackageDocument(sourceStream);
                BCA.Order bcOrder = new();
                BEA.OrderExtended o = bcOrder.SearchExtended(DocNum, Subsidiary);
                string address = "", subsidiary = "";
                if (Subsidiary.ToLower() == "santa cruz")
                {
                    address = "Av. Grigota # 3800 <br /> Santa Cruz - Bolivia <br /> Fono: (591) 3-3543000 <br /> Fax: (591) 3-3543637";
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
                var order = new
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
                report.DataSource = order;

                foreach (var item in report.Items)
                {
                    if (item.Name == "detailSection1")
                    {
                        foreach (var i in item.Items)
                        {
                            if (i.Name == "ttDetail")
                            {
                                var billItems = bcOrder.ListItems2(Subsidiary, DocNum, "1");
                                Telerik.Reporting.Table table = (Telerik.Reporting.Table)i;
                                table.DataSource = billItems;
                            }
                        }
                    }
                }
            }

            ReportProcessor reportProcessor = new();
            InstanceReportSource instanceReportSource = new() { ReportDocument = report };
            RenderingResult result = reportProcessor.RenderReport("PDF", instanceReportSource, null);

            return new MemoryStream(result.DocumentBytes);
        }

        #endregion
    }
}
