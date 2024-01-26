using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class RequestController : BaseController
    {
        #region Constructores

        public RequestController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewBag.Permission = GetPermission("Request-SeeAll");
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetProducts()
        {
            IEnumerable<BEP.Product> products;
            try
            {
                BCP.Product bcProduct = new();
                products = bcProduct.ListWithPrices(null, "1");
            }
            catch (System.Exception)
            {
                products = new List<BEP.Product>();
            }
            var items = products.Select(x => new { x.Id, x.ItemCode, x.Line, Name = x.ItemCode + " - " + x.Name });
            return Json(items);
        }

        public IActionResult Filter(long? ProductId, long? UserId, long? SubsidiaryId, string CardCode, string Reported, DateTime? Since, DateTime? Until)
        {
            string message = "";
            try
            {
                short permission = GetPermission("Request-SeeAll");
                List<Field> filters = new();
                if (ProductId.HasValue && ProductId.Value > 0) filters.Add(new Field("IdProduct", ProductId.Value));
                if (UserId.HasValue && UserId.Value > 0) filters.Add(new Field("IdUser", UserId.Value));
                if (SubsidiaryId.HasValue && SubsidiaryId.Value > 0) filters.Add(new Field("IdSubsidiary", SubsidiaryId.Value));
                if (!string.IsNullOrEmpty(CardCode)) filters.Add(new Field("LOWER(CardCode)", CardCode.ToLower()));
                if (!string.IsNullOrEmpty(Reported) && Reported != "B") filters.Add(new Field("Reported", Reported == "Y"));
                if (Since.HasValue) filters.Add(new Field("RequestDate", Since.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                if (Until.HasValue) filters.Add(new Field("RequestDate", Until.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                if (permission <= 0) filters.Add(new Field("IdUser", UserCode));
                CompleteFilters(ref filters);
                BCP.Request bcRequest = new();
                var requests = bcRequest.List(filters, "1", BEP.relRequest.User, BEP.relRequest.Subsidiary, BEP.relRequest.Product);
                IEnumerable<BEA.Client> clients = Enumerable.Empty<BEA.Client>();
                if (requests.Any())
                {
                    string clientCodes = string.Join(",", requests.Select(x => $"'{x.CardCode.ToLower()}'"));
                    BCA.Client bcClient = new();
                    filters = new List<Field> { new Field("LOWER(CardCode)", clientCodes, Operators.In) };
                    clients = bcClient.ListShort(filters, "1");
                }
                var items = from r in requests
                            join c in clients on r.CardCode.ToLower() equals c.CardCode.ToLower() into lj
                            from l in lj.DefaultIfEmpty()
                            select new { r.Id, r.IdProduct, ProductName = r.Product.Name, r.IdUser, UserName = r.User.Name, r.IdSubsidiary, SubsidiaryName = r.Subsidiary.Name, r.CardCode, CardName = l?.CardName ?? "", r.RequestDate, r.Quantity, r.Reported, r.Description };
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BEP.Request Item)
        {
            string message = "";
            try
            {
                DateTime now = DateTime.Now;
                if (Item.Id == 0)
                {
                    Item.StatusType = BEntities.StatusType.Insert;
                    Item.RequestDate = now;
                    Item.IdUser = UserCode;
                    Item.Reported = false;
                }
                else
                {
                    Item.StatusType = BEntities.StatusType.Update;
                }

                Item.LogDate = now;
                Item.LogUser = UserCode;

                BCP.Request bcReuqest = new();
                bcReuqest.Save(ref Item);
                Item.UserName = UserName;
                return Json(new { message, Item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete(long Id)
        {
            string message = "";
            try
            {
                BCP.Request bcRequest = new();
                BEP.Request item = new() { StatusType = BEntities.StatusType.Delete, Id = Id, LogDate = DateTime.Now, RequestDate = DateTime.Now };
                bcRequest.Save(ref item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string Ids)
        {
            string message = "";
            try
            {
                BCP.Request bcRequest = new();
                List<Field> filters = new() { new Field("Id", Ids, Operators.In) };
                IEnumerable<BEP.Request> requests = bcRequest.List(filters, "1", BEP.relRequest.Product, BEP.relRequest.User, BEP.relRequest.Subsidiary);

                string codes = string.Join(",", requests.Select(x => $"'{x.Product.ItemCode.ToLower()}'"));

                BCA.Product bcProduct = new();
                filters = new List<Field> { new Field("LOWER(ItemCode)", codes, Operators.In) };
                IEnumerable<BEA.Product> productManagers = bcProduct.ListPMs(filters, "1");

                codes = string.Join(",", productManagers.Select(x => $"'{x.ProductManagerCode.ToLower()}'"));
                filters = new List<Field> { new Field("LOWER(SellerCode)", codes, Operators.In) };
                BCS.UserData bcData = new();
                IEnumerable<BES.UserData> userDatas = bcData.List(filters, "1", BES.relUserData.User);

                codes = string.Join(",", requests.Select(x => $"'{x.CardCode.ToLower()}'"));
                filters = new List<Field> { new Field("LOWER(CardCode)", codes, Operators.In) };
                BCA.Client bcClient = new();
                var clients = bcClient.ListShort(filters, "1");

                var items = from i in (from r in requests
                                       join c in clients on r.CardCode.ToLower() equals c.CardCode.ToLower() into ljClients
                                       from lc in ljClients.DefaultIfEmpty()
                                       join m in productManagers on r.Product.ItemCode.ToLower() equals m.Code.ToLower() into ljProductManagers
                                       from lm in ljProductManagers.DefaultIfEmpty()
                                       join u in userDatas on lm?.ProductManagerCode.ToLower() equals u.SellerCode.ToLower() into ljUserDatas
                                       from lu in ljUserDatas.DefaultIfEmpty()
                                       select new
                                       {
                                           r.Product.ItemCode,
                                           ProductName = r.Product.Name,
                                           Aplicant = r.User.Name,
                                           Subsidiary = r.Subsidiary.Name,
                                           r.CardCode,
                                           CardName = lc?.CardName ?? "",
                                           r.RequestDate,
                                           r.Quantity,
                                           r.Description,
                                           ProductManager = lm?.ProductManager ?? "",
                                           ProductManagerEmail = lu?.User.EMail ?? ""
                                       })
                            group i by new { i.ProductManager, i.ProductManagerEmail } into g
                            select new { Name = !string.IsNullOrEmpty(g.Key.ProductManager) ? g.Key.ProductManager : "Paul Chang", Email = !string.IsNullOrEmpty(g.Key.ProductManagerEmail) ? g.Key.ProductManagerEmail : "paul.chang@dmc.bo", Items = g };

                foreach (var item in items)
                {
                    StringBuilder sb = new();
                    sb.AppendLine(@"<html>");
                    sb.AppendLine(@" <head>");
                    sb.AppendLine(@"     <style>");
                    sb.AppendLine(@"     body { font-size: 12px; } ");
                    sb.AppendLine(@"     table { border-collapse: collapse; }");
                    sb.AppendLine(@"      td { font-size: 11px; padding: 4px 8px; } ");
                    //sb.AppendLine(@"      tr:nth-child(even), tr:nth-child(even) td { background-color: #CDCDCD; } ");
                    sb.AppendLine(@"     .header { background-color: #5B9BD5; color: #FFF; } ");
                    sb.AppendLine(@"     .header td { font-size: 13px; }");
                    sb.AppendLine(@"     .odd { background-color: #EEE; } ");
                    sb.AppendLine(@"     .odd td { border-bottom: 1px solid #DEDEDE; } ");
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
                    sb.AppendLine($@"     <p>{item.Name}</p>");
                    sb.AppendLine(@"     <p>Le han hecho las siguientes solicitudes</p>");
                    sb.AppendLine($@"    <p><table style=""width: 100%""><tr class=""header""><td>Producto</td><td style=""text-align: right"">Cantidad</td><td style=""width: 120px"">Sucursal</td><td style=""width: 120px"">Solicitante</td><td style=""width: 100px"">Cliente</td></tr>");
                    int cont = 1;
                    foreach (var i in item.Items)
                    {
                        string c = cont % 2 == 1 ? "" : @" class=""odd""";
                        sb.AppendLine($@"<tr{c}><td>{i.ItemCode}<br />{i.ProductName}</td><td style=""text-align: right"">{i.Quantity}</td><td>{i.Subsidiary}</td><td>{i.Aplicant}</td><td>{i.CardCode}</td></tr>");
                        if (!string.IsNullOrEmpty(i.Description)) sb.AppendLine($@"<tr{c}><td colspan=""5"" style=""font-size: 11.5px"">{i.Description}</td></tr>");
                        cont++;
                    }
                    sb.AppendLine($@"    </table></p><br />");
                    sb.AppendLine($@"    <p>Atentamente<br />El equipo del Portal</p>");
                    sb.AppendLine(@"   </div>");
                    sb.AppendLine(@" </body>");
                    sb.AppendLine(@"</html>");

                    List<MailAddress> recipients = new(), copies = new(), blindCopies = new();
                    if (IsDevelopmentMode)
                    {
                        recipients.Add(new MailAddress("julio.peredo@dmc.bo", "Julio Peredo"));
                        blindCopies.Add(new MailAddress("julio.peredo@gmail.com", "Julio Peredo"));
                    }
                    else
                    {
                        recipients.Add(new MailAddress(item.Email, item.Name));
                        FillCustomCopies("Product", "Request", "SendEmail", ref copies, ref blindCopies);
                    }
                    await SendMailAsync("Lista de Productos Solicitados", sb.ToString(), recipients, copies, blindCopies);
                    foreach (var i in requests)
                    {
                        i.StatusType = BEntities.StatusType.Update;
                        i.Reported = true;
                        i.LogUser = UserCode;
                        i.LogDate = DateTime.Now;
                    }
                    IList<BEP.Request> list = requests.ToList();
                    bcRequest.Save(ref list);
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Metodos Privados

        #endregion
    }
}
