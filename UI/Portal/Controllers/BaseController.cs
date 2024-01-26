using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCF = BComponents.Staff;
using BCH = BComponents.HumanResources;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BCT = BComponents.Staff;
using BCV = BComponents.Visits;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BEH = BEntities.HumanResources;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BET = BEntities.Staff;
using BEV = BEntities.Visits;

namespace Portal.Controllers
{
    public class BaseController : Controller
    {

        #region Global Variables

        protected string HomeCardCode => "CDMC-002";

        protected long UserCode => User.Identity.IsAuthenticated ? long.Parse(((ClaimsIdentity)User.Identity).FindFirst("UserCode")?.Value ?? "0") : 0;

        protected string CardCode => User.Identity.IsAuthenticated ? (((ClaimsIdentity)User.Identity).FindFirst("CardCode")?.Value ?? "") : "";

        protected string CardName => User.Identity.IsAuthenticated ? (((ClaimsIdentity)User.Identity).FindFirst("CardName")?.Value ?? "") : "";

        protected long ProfileCode => User.Identity.IsAuthenticated ? long.Parse(((ClaimsIdentity)User.Identity).FindFirst("ProfileCode")?.Value ?? "0") : 0;

        protected string UserName => User.Identity.IsAuthenticated ? (((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.GivenName)?.Value ?? "") : "";

        protected string EMail => User.Identity.IsAuthenticated ? (((ClaimsIdentity)User.Identity).FindFirst("Login")?.Value ?? "") : "";

        private readonly IConfiguration config;

        private readonly IWebHostEnvironment env;

        protected string rootDirectory = @"C:\inetpub\wwwroot";

        protected bool IsDevelopmentMode => env.EnvironmentName == "Development";

        protected bool IsClient => CardCode != "CDMC-002";

        #endregion

        #region Contructors

        public BaseController(IConfiguration Configuration, IWebHostEnvironment HEnviroment)
        {
            config = Configuration;
            env = HEnviroment;
            rootDirectory = HEnviroment.ContentRootPath;
        }

        #endregion

        #region Protected Methods

        protected short GetPermission(string ActivityName)
        {
            List<int> inserts = new() { 1, 3, 5, 7 }, updates = new() { 2, 3, 6, 7 }, deletes = new() { 4, 5, 6, 7 };

            BCS.ProfileActivity bcActivity = new();
            int profilePermission = bcActivity.GetPermissionByProfile(ProfileCode, ActivityName);

            BCS.UserActivity bcUserAct = new();
            int userPermission = bcUserAct.GetPermission(UserCode, ActivityName);

            short intPermission = 0;
            if (inserts.Contains(profilePermission) | inserts.Contains(userPermission)) intPermission += 1;
            if (updates.Contains(profilePermission) | updates.Contains(userPermission)) intPermission += 2;
            if (deletes.Contains(profilePermission) | deletes.Contains(userPermission)) intPermission += 4;

            return intPermission;
        }

        protected bool IsAllowed(string Area, string Controller, string Action)
        {
            bool boAllowed = false;
            if (IsEnabled())
            {
                List<BEB.Menu> lstMenu = GetMenu();
                BEB.Menu beItem = lstMenu.Find(i => i.Page != null && (i.Page.Area.ToLower() == Area.ToLower() & i.Page.Controller.ToLower() == Controller.ToLower() & i.Page.Action.ToLower() == Action.ToLower()));
                string strMessage = $"Acceso a página: /{Area}/{Controller}/{Action}, denegado";
                if (beItem != null)
                {
                    boAllowed = true;
                    strMessage = $"Acceso a página: {beItem.Title}, concedido";
                }
                SaveLog(strMessage);
            }
            else
            {
                //RedirectToAction("logout", "account", new { action = "" });
                RedirectToAction("logout", "account", new { area = "" });
            }
            return boAllowed;
        }

        protected bool IsAllowed<T>(T controller) where T : BaseController
        {
            bool allowed = false;
            if (IsEnabled())
            {
                var values = controller.ControllerContext.RouteData.Values;
                string areaName = (string)values["area"], controllerName = (string)values["controller"], actionName = (string)values["action"];
                List<BEB.Menu> menuItems = GetMenu();
                BEB.Menu item = menuItems.Find(i => i.Page != null && (i.Page.Area.ToLower() == areaName.ToLower() & i.Page.Controller.ToLower() == controllerName.ToLower() & i.Page.Action.ToLower() == actionName.ToLower()));
                string message = $"Acceso a página: /{areaName}/{controllerName}/{actionName}, denegado";
                if (item != null)
                {
                    allowed = true;
                    message = $"Acceso a página: {item.Title}, concedido";
                }
                SaveLog(message);
            }
            else
            {
                //RedirectToAction("logout", "account", new { action = "" });
                RedirectToAction("logout", "account", new { area = "" });
            }
            return allowed;
        }

        protected static string ToTitle(string message)
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(message))
            {
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                result = myTI.ToTitleCase(message.ToLower());
            }
            return result;
        }

        protected string GetError(Exception ex)
        {


            string strMessage = "";
            string strType = ex.GetType().FullName;
            switch (strType)
            {
                //    Case GetType(BComponents.BCException)
                //        If CType(ex, BComponents.BCException).ErrorCollection.Count > 0 Then
                //            message = String.Join(Environment.NewLine, CType(ex, BComponents.BCException).ErrorCollection)
                //        Else
                //            message = ex.Message
                //        End If
                case "System.Data.SqlClient.SqlException":
                    System.Data.SqlClient.SqlException ex1 = (System.Data.SqlClient.SqlException)ex;
                    if (ex1.Errors.Count > 0)
                    {
                        switch (ex1.Errors[0].Number)
                        {
                            case 547:
                                strMessage = "El registro no puede ser eliminado porque tiene dependencias.";
                                break;
                            case 2601:
                            case 2627:
                                strMessage = "Violación de la llave primaria o la llave no ha sido especificada apropiadamente.";
                                break;
                            default:
                                //message = string.Join(Environment.NewLine, (from e in ex1.Errors select e.Message).ToArray);
                                for (int i = 0; i < ex1.Errors.Count; i++)
                                {
                                    strMessage += Environment.NewLine + ex1.Errors[i].Message;
                                }
                                break;
                        }
                    }
                    break;
                default:
                    strMessage = ex.Message;
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        strMessage += Environment.NewLine + ex.Message;
                    }
                    break;
            }

            string data = "", mailBody;
            List<string> list = new();
            if (Request.Method == "POST")
            {
                try
                {
                    Request.Form.ToList().ForEach(f => list.Add($"{f.Key}: {f.Value}"));
                }
                catch (Exception) { }
            }
            else
            {
                if (Request.Query.Count > 0) Request.Query.ToList().ForEach(f => list.Add($"{f.Key}: {f.Value}"));
            }
            data = string.Join("<br />&nbsp;&nbsp;&nbsp;&nbsp;", list);

            mailBody = $@"<style> 
                            body {{ background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; }} 
                            img {{ margin: 20px 15px; }}
                            td {{ padding: 0 8px; line-height: 18px; }}
                          </style>    
                          <p style=""font-size: 16px;""><b>ERROR GENERADO EN EL PORTAL</b></p>
                          <p><b>Fecha</b> : {DateTime.Now:dd/MM/yyyy HH:mm:ss}<br />
                          <b>Cliente</b>: ( {CardCode} ) {CardName}<br />
                          <b>Usuario:</b> ( {UserCode} ) {UserName}<br />
                          <b>Ambiente</b>: {env.EnvironmentName}<br />
                          <b>Método</b>: {this.Request.Method}<br />
                          <b>Dirección</b>: {this.Request.Scheme}://{this.Request.Host}{this.Request.Path} <br/>
                          {(data != "" ? $@"<b>Par&aacute;metros</b>: <br />&nbsp;&nbsp;&nbsp;&nbsp;{data}" : "")}</p>
                          <b>Detalle error: </b><br />
                          <div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"">
                            <table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse;"">
                              <tr>
                                  <td style=""width: 20px;"">&nbsp;</td>
                                  <td>                                            
                                      <pre>{strMessage.Replace("\r\n", "<br />")}<br />{ex.StackTrace.Replace("\r\n", "<br />")}</pre>
                                  </td>
                                  <td style=""width: 20px;"">&nbsp;</td>
                              </tr>
                            </table>                
                          </div><br />";

            _ = SendMailAsync("Error controlado en el Portal", mailBody, new List<MailAddress> { new MailAddress("julio.peredo@dmc.bo") });
            return strMessage;
        }

        protected static bool IsValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                Regex rx = new(@"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
                return rx.IsMatch(email);
            }
            else
            {
                return false;
            }
        }

        protected static bool IsEMailBlacklisted(string EMail)
        {
            if (!string.IsNullOrEmpty(EMail))
            {
                BCS.MailBlacklist bcBlacklist = new();
                var blackList = bcBlacklist.List("1").Select(i => i.EMail.Trim());
                return blackList.Contains(EMail.Trim());
            }
            else
            {
                return false;
            }
        }

        protected List<BEB.Menu> GetMenu()
        {
            List<BEB.Menu> items = HttpContext.Session.Get<List<BEB.Menu>>("Menu");
            if (items == null)
            {
                BCB.Menu bcMenu = new();
                items = bcMenu.ListByProfile(ProfileCode, "1", BEB.relMenu.Page);
                HttpContext.Session.Set("Menu", items);
            }
            return items;
        }

        protected static string SetHTMLSafe(string Data)
        {
            string strReturn = "";
            if (Data != null)
            {
                strReturn = Data.Replace("&amp;lt;", "<").Replace("&lt;", "<").Replace("&amp;gt;", ">").Replace("&gt;", ">").Replace("&amp;nbsp;", "&nbsp;")
                    .Replace("&amp;aacute;", "&aacute;").Replace("&amp;eacute;", "&eacute;").Replace("&amp;iacute;", "&iacute;").Replace("&amp;oacute;", "&oacute;").Replace("&amp;uacute;", "&uacute;")
                    .Replace("&amp;ntilde;", "&ntilde;").Replace("&amp;amp;", "&").Replace("á", "&aacute;").Replace("é", "&eacute;").Replace("í", "&iacute;").Replace("ó", "&oacute;").Replace("ú", "&uacute;").Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;").Replace("É", "&Eacute;").Replace("Í", "&Iacute;").Replace("Ó", "&Oacute;").Replace("Ú", "&Uacute;").Replace("Ñ", "&Ntilde;");
            }
            return strReturn;
        }

        protected async Task SendMailAsync(string Subject, string MessageBody, List<MailAddress> Destinataries, List<MailAddress> Copies = null, List<MailAddress> BlindCopies = null, MailAddress From = null, List<Attachment> Files = null)
        {
            var settingsSection = config.GetSection("MailSettings");
            var settings = settingsSection.Get<MailSettings>();
            MailAccount account = (From == null ? settings.Accounts.Find(x => x.Id == "soporte-santacruz") : settings.Accounts.Find(x => x.EMail == From.Address)) ?? settings.Accounts.Find(x => x.Id == "soporte-santacruz");

            MailMessage message = new();
            MailAddress fromAddress = new(account.EMail, account.Name);
            if (From != null)
            {
                fromAddress = new MailAddress(From.Address, From.DisplayName);
                message.ReplyToList.Add(From);
            }
            message.From = fromAddress;
            if (Destinataries != null && Destinataries.Count > 0)
            {
                foreach (var item in Destinataries)
                {
                    message.To.Add(item);
                }
            }
            if (Copies != null && Copies.Count > 0)
            {
                foreach (var item in Copies)
                {
                    message.CC.Add(item);
                }
            }
            if (BlindCopies != null && BlindCopies.Count > 0)
            {
                foreach (var item in BlindCopies)
                {
                    message.Bcc.Add(item);
                }
            }
            if (Files?.Count > 0)
            {
                foreach (var file in Files)
                {
                    message.Attachments.Add(file);
                }
            }
            message.Subject = Subject;
            message.IsBodyHtml = true;
            message.Body = MessageBody;

            SmtpClient smtpClient = new(settings.Server, settings.Port) { Credentials = new NetworkCredential(account.User, account.Password), DeliveryMethod = SmtpDeliveryMethod.Network };
            await smtpClient.SendMailAsync(message);
            message.Dispose();
            smtpClient.Dispose();
        }

        protected static List<string> GetSubsidiariesList()
        {
            List<string> lstItems = new();
            BCB.Classifier bcClassifier = new();
            List<BEB.Classifier> lstSubsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "IdType");
            lstItems = lstSubsidiaries.Select(i => ToTitle(i.Name)).ToList();
            return lstItems;
        }

        protected static void SetCultureUI()
        {
            //Por ahora aca, pero debería ser una solución global
            var cul = CultureInfo.CreateSpecificCulture("es-BO");
            cul.NumberFormat.NumberDecimalSeparator = ".";
            cul.NumberFormat.NumberGroupSeparator = ",";
            System.Threading.Thread.CurrentThread.CurrentCulture = cul;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cul;
        }

        protected List<BES.LastNotification> GetLastNotifications()
        {
            long IdUser = UserCode;
            BCS.LastNotification bcLast = new();
            var items = (from i in bcLast.List(IdUser) orderby Guid.NewGuid() select i).ToList();
            return items;
        }

        protected static void SaveLastNotifications(List<BES.LastNotification> items)
        {
            BCS.LastNotification bcLast = new();
            IList<BES.LastNotification> list = items.Where(x => x.StatusType != BEntities.StatusType.NoAction).ToList();
            bcLast.Save(ref list);
        }

        protected static void CompleteFilters(ref List<Field> list)
        {
            int operantors = list.Count(x => x.LogicalOperator != LogicalOperators.None);
            int parts = list.Count(x => x.LogicalOperator == LogicalOperators.None);
            for (int i = 1; i < (parts - operantors); i++)
            {
                list.Add(new Field(LogicalOperators.And));
            }
        }

        protected bool IsEnabled()
        {
            bool enabled = false;
            try
            {
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(UserCode);
                if (beUser != null)
                {
                    enabled = beUser.Enabled;// & !beUser.RequiredLogOff;
                    //if (beUser.RequiredLogOff)
                    //{
                    //    beUser.RequiredLogOff = false;
                    //    beUser.StatusType = BEntities.StatusType.Update;
                    //    bcUser.Save(ref beUser);
                    //}
                }
            }
            catch (Exception) { }
            return enabled;
        }

        protected void FillCustomCopies(string AreaName, string ControllerName, string ActionName, ref List<MailAddress> Copies, ref List<MailAddress> BlindCopies)
        {
            if (IsDevelopmentMode)
            {
                BlindCopies.Add(new MailAddress("julio.peredo@dmc.bo", "Julio Peredo"));
            }
            else
            {
                MailSettings settings = default;
                try
                {
                    var settingsSection = config.GetSection("MailSettings");
                    settings = settingsSection.Get<MailSettings>();
                }
                catch (Exception) { }
                if (settings != null)
                {
                    var config = settings.Tasks?.FirstOrDefault(i => i.Area == AreaName & i.Controller == ControllerName & i.Name == ActionName);
                    Copies ??= new List<MailAddress>();
                    BlindCopies ??= new List<MailAddress>();
                    if (config?.CopiesTo?.Count > 0)
                    {
                        foreach (var item in config.CopiesTo)
                        {
                            if (IsValidEmail(item.EMail)) Copies.Add(new MailAddress(item.EMail, item.Name));
                        }
                    }
                    if (config?.BlindCopiesTo?.Count > 0)
                    {
                        foreach (var item in config.BlindCopiesTo)
                        {
                            if (IsValidEmail(item.EMail)) BlindCopies.Add(new MailAddress(item.EMail, item.Name));
                        }
                    }
                }
            }
        }

        protected static string NewPassword()
        {
            var opts = new PasswordOptions() { RequiredLength = 10, RequiredUniqueChars = 6, RequireDigit = true, RequireLowercase = true, RequireNonAlphanumeric = true, RequireUppercase = true };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$_-{}[]*."                   // non-alphanumeric
            };
            Random rand = new(Environment.TickCount);
            List<char> chars = new();

            if (opts.RequireUppercase) chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase) chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit) chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric) chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }
            return new string(chars.ToArray());
        }

        protected string GetPercentage(string Rotation, decimal Stock, decimal Reserved)
        {
            //decimal? decResult = 0;
            //if (!string.IsNullOrWhiteSpace(Rotation))
            //{
            //    decResult = (Stock - Reserved) * 100;
            //    switch (Rotation.ToLower())
            //    {
            //        case "baja":
            //            decResult /= 10;
            //            break;
            //        case "media":
            //            decResult /= 50;
            //            break;
            //        case "intermedia":
            //            decResult /= 100;
            //            break;
            //        case "alta":
            //            decResult /= 500;
            //            break;
            //        default:
            //            decResult = null;
            //            break;
            //    }

            //    if (decResult.HasValue && decResult < 0) decResult = 0;
            //    if (decResult.HasValue && decResult > 100) decResult = 100;
            //}
            //return decResult;
            decimal available = Stock - Reserved;
            string result = available switch
            {
                < 0 => "0",
                > 50 => "+50",
                _ => available.ToString(),
            };
            return result;
        }

        protected string GetLiteralAmount(decimal Monto)
        {
            string strMontoLiteral = "";
            if (Monto > 0m)
            {
                if (Monto > 1)
                {
                    strMontoLiteral = NumeroLiteral(Monto);
                    decimal decMontoFraccion = Math.Round(Convert.ToDecimal(Monto - Math.Floor(Monto)), 2);
                    strMontoLiteral += decMontoFraccion > 0 ? (" con " + Convert.ToDecimal(Math.Floor(decMontoFraccion * 100)).ToString() + "/100 ") : (strMontoLiteral[0].ToString().ToUpper() != "0" ? " con 00/100 " : "0");
                }
                else
                {
                    strMontoLiteral = (Monto * 100).ToString() + "/100 Centavos";
                }
            }
            return ToTitle(strMontoLiteral);
        }

        protected BEA.Employee GetEmployee()
        {
            BEA.Employee item = default;
            BCS.UserData bcUData = new();
            BES.UserData uData = bcUData.SearchByUser(UserCode);
            if (uData?.IdEmployee != null)
            {
                BCA.Employee bcEmployee = new();
                item = bcEmployee.Search((int)uData.IdEmployee);
            }
            return item;
        }

        protected static (int, int, int, int) GetTimeParts(DateTime since, DateTime until)
        {
            // compute & return the difference of two dates, returning years, months & days until should be the larger
            // (newest) of the two dates we want until to be the larger (newest) date flip if we need to
            int years, months, days, totalDays;
            if (until < since)
            {
                (until, since) = (since, until);
            }

            // compute difference in total months
            months = 12 * (until.Year - since.Year) + (until.Month - since.Month);

            // based upon the 'days',
            // adjust months & compute actual days difference
            if (until.Day < since.Day)
            {
                months--;
                days = DateTime.DaysInMonth(since.Year, since.Month) - since.Day + until.Day;
            }
            else
            {
                days = until.Day - since.Day;
            }
            // compute years & actual months
            years = months / 12;
            months -= years * 12;
            totalDays = (int)(until - since).TotalDays;
            return (years, months, days, totalDays);
        }

        #endregion

        #region Public Methods

        public IActionResult Denied()
        {
            ViewData["Message"] = "Acceso Denegado";
            return View();
        }

        public void SaveLog(string Description)
        {
            long IdUser = UserCode;
            BES.SessionHistory beLog = new() { IdUser = IdUser, Description = Description, LogDate = DateTime.Now, StatusType = BEntities.StatusType.Insert };
            BCS.SessionHistory bcLog = new();
            bcLog.Save(ref beLog);
        }

        public IActionResult GetProfiles(string CardCode)
        {
            BCS.Profile bcProfile = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(CardCode))
            {
                lstFilter.Add(new Field("isExternalCapable", CardCode.Trim().ToLower() != HomeCardCode.ToLower()));
            }
            IEnumerable<BES.Profile> lstProfiles = bcProfile.List(lstFilter, "isExternalCapable, Name");
            return Json(lstProfiles);
        }

        [AllowAnonymous()]
        public IActionResult getExternalProfiles()
        {
            BCS.Profile bcProfile = new();
            List<Field> lstFilter = new() { new Field("isExternalCapable", true) };
            IEnumerable<BES.Profile> lstProfiles = bcProfile.List(lstFilter, "1");
            return Json(lstProfiles);
        }

        public IActionResult GetClients(string Name)
        {
            IEnumerable<BEA.Client> lstClients;
            try
            {
                lstClients = GetClientsList(Name);
            }
            catch (Exception)
            {
                lstClients = new List<BEA.Client>();
            }
            var lstResult = from c in lstClients
                            group c by c.CardCode into g
                            select new { Code = g.Key, Name = $"{g.Key} - {g.FirstOrDefault()?.CardName ?? ""}" };
            return Json(lstResult);
        }

        public IEnumerable<BEA.Client> GetClientsList(string Name)
        {
            IEnumerable<BEA.Client> lstClients;
            BCA.Client bcClients = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                lstFilter.Add(new Field { Name = "CardName", Value = Name, Operator = Operators.Likes });
            }
            if (GetPermission("SeeAllClients") == 0)
            {
                BCS.UserData bcData = new();
                var userData = bcData.SearchByUser(UserCode);
                lstFilter.AddRange(new[] { new Field("SellerCode", userData?.SellerCode ?? "XXX"), new Field("SlpCode", -1) });
                if (!string.IsNullOrEmpty(userData?.SellerCode))
                {
                    BCT.Replace bcReplace = new();
                    var replacements = bcReplace.List(userData.SellerCode, "1");
                    if (replacements?.Count() > 0)
                    {
                        lstFilter.AddRange(replacements.Select(x => new Field("SellerCode", x.ReplaceCode)));
                        lstFilter.AddRange(replacements.Select(x => new Field(LogicalOperators.Or)));
                    }
                }
                lstFilter.Add(new Field(LogicalOperators.Or));
            }
            CompleteFilters(ref lstFilter);
            lstClients = bcClients.ListShort3(lstFilter, "CardName");
            return lstClients;
        }

        public IActionResult GetSubsidiaries()
        {
            string message = "";
            try
            {
                BCB.Classifier bcClassifier = new();
                var lstClassifiers = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");
                var items = (from c in lstClassifiers where !string.IsNullOrWhiteSpace(c.Name) select new { c.Id, Name = ToTitle(c.Name) });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetSubsidiariesItems()
        {
            string message = "";
            try
            {
                BCB.Classifier bcClassifier = new();
                var lstClassifiers = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");
                var items = (from c in lstClassifiers where !string.IsNullOrWhiteSpace(c.Name) select new { c.Id, Name = ToTitle(c.Name) });
                return Json(items);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetSellers()
        {
            IEnumerable<BEA.Seller> lstSalesmen;
            try
            {
                BCA.Seller bcSalesman = new();
                lstSalesmen = bcSalesman.List(null, "1");
            }
            catch (Exception)
            {
                lstSalesmen = new List<BEA.Seller>();
            }
            var lstResult = (from s in lstSalesmen orderby s.Name select new { Id = 0, s.ShortName, s.Name }).Distinct();
            return Json(lstResult);
        }

        public IActionResult GetLines(bool AddItemNone = false)
        {
            BCD.Line bcLine = new();
            var lstLines = bcLine.List("Name").ToList();
            if (AddItemNone) lstLines.Insert(0, new BED.Line { Id = -1, Name = "--- Ninguna ---" });
            var items = lstLines.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetAvailableLines()
        {
            BCP.Line bcLine = new();
            IEnumerable<BEP.Line> lstLines = new List<BEP.Line>();
            if (ProfileCode == (long)BEE.Types.Profile.ProductManagement)
            {
                long idManager = 0;
                BCS.User bcUser = new();
                var beUser = bcUser.Search(UserCode, BES.relUser.UserDatas);
                BCF.Member bcMember = new();
                var member = bcMember.SearchByMail(beUser.EMail);
                idManager = member?.Id ?? 0;
                List<Field> filters = new() { new Field("IdManager", idManager) };
                lstLines = bcLine.List(filters, "Name");
            }
            else
            {
                lstLines = bcLine.List("Name");
            }
            var items = lstLines.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetCategories()
        {
            BCD.Category bcCategory = new();
            var lstTempCategories = bcCategory.List("Name");
            List<BED.Category> lstCategories = new();
            foreach (var item in lstTempCategories)
            {
                if (!lstCategories.Select(x => x.Name.ToLower()).Contains(item.Name.ToLower()))
                {
                    lstCategories.Add(item);
                }
            }
            var items = lstCategories.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult GetSubcategories(short CategoryId)
        {
            BCD.SubCategory bcSubs = new();
            var lstTempSubcategories = bcSubs.ListDistinct(CategoryId, "1");
            List<BED.SubCategory> lstSubcategories = new List<BED.SubCategory>();
            foreach (var item in lstTempSubcategories)
            {
                if (!lstSubcategories.Select(x => x.Name.ToLower()).Contains(item.Name.ToLower()))
                {
                    lstSubcategories.Add(item);
                }
            }
            var items = lstSubcategories.Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public static bool IsEmailValid(string email)
        {
            if (email != null && email.Trim().Length > 0)
            {
                return Regex.IsMatch(email, "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$");
            }
            else
            {
                return false;
            }
        }

        public IActionResult GetWarehouses(string Subsidiary)
        {
            string message = "";
            IEnumerable<BEA.Item> items = new List<BEA.Item>();
            try
            {
                if (CardCode == HomeCardCode)
                {
                    BCA.Warehouse bcStorehouse = new();
                    items = !string.IsNullOrWhiteSpace(Subsidiary) ? bcStorehouse.ListIn(Subsidiary) : bcStorehouse.List();
                }
                else
                {
                    BCP.WarehouseAllowed bcWharehouse = new();
                    List<Field> filters = new() { new Field("ClientVisible", 1) };
                    if (!string.IsNullOrWhiteSpace(Subsidiary)) filters.AddRange(new[] { new Field("Subsidiary", Subsidiary, Operators.In), new Field(LogicalOperators.And) });
                    IEnumerable<BEP.WarehouseAllowed> lstWharehouses = bcWharehouse.List(filters, "Name");
                    items = lstWharehouses.Select(i => new BEA.Item { Code = i.Code, Name = ToTitle(i.Name) });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var beResult = new { message, items };
            return Json(beResult);
        }

        public IActionResult GetClientName(string CardCode)
        {
            string message = "", name = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(CardCode))
                {
                    BCA.Client bcClient = new();
                    BEA.Client client = bcClient.Search(CardCode);
                    name = client?.CardName ?? "";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, name });
        }

        public IActionResult GetProductManagers()
        {
            BCA.Seller bcSalesMan = new();
            IEnumerable<BEA.Seller> lstItems = bcSalesMan.ProductManagers(new List<Field>(), "Name");
            var items = lstItems.Select(x => new { x.ShortName, x.Name }).Distinct();
            return Json(items);
        }

        public IActionResult GetEmployees()
        {
            //BCA.Employee bcEmployee = new();
            //IEnumerable<BEA.Employee> lstItems = bcEmployee.List(null, "FirstName, LastName");
            BCH.Employee bcEmp = new();
            IEnumerable<BEH.Employee> lstItems = bcEmp.List("Name");
            var items = lstItems.Select(x => new { x.Id, x.Name, x.Enabled });
            return Json(items);
        }

        public IActionResult GetAvailableEmployees()
        {
            BCA.Employee bcEmployee = new();
            IEnumerable<BEA.Employee> lstItems = bcEmployee.List(null, "FirstName, LastName");
            var items = lstItems.Where(x => x.IsAvailable).Select(x => new { x.Id, x.Name });
            return Json(items);
        }

        public IActionResult RequireLogOff()
        {

            BCS.User bcUser = new();
            var user = bcUser.Search(UserCode);
            bool result = user?.RequiredLogOff ?? true;
            //if (user.RequiredLogOff)
            //{
            //    user.RequiredLogOff = false;
            //    user.StatusType = BEntities.StatusType.Update;
            //    bcUser.Save(ref user);
            //}
            return Json(result);
        }

        #endregion

        #region Private

        //private async Task SaveLogEntry(ConfigData config, DateTime LogDate, string RequestBody, string DocNum, List<Error> Errors)
        //{
        //    try
        //    {
        //        string path = config.Audit.Directory;
        //        if (!string.IsNullOrEmpty(path)) path = new Uri(path).LocalPath;

        //        StringBuilder logsText = new StringBuilder(), details = new StringBuilder();

        //        if (config.Test) logsText.AppendLine(@"SERVICIO DE PRUEBA");
        //        logsText.AppendLine($@"Solicitud recibida el : {LogDate:dd/MM/yyyy HH:mm:ss}" + Environment.NewLine);
        //        logsText.AppendLine(RequestBody + Environment.NewLine);
        //        if (Errors?.Count > 0)
        //        {
        //            logsText.AppendLine(@"Errores");
        //            foreach (var e in Errors)
        //            {
        //                details.AppendLine($@"<p>{(!string.IsNullOrEmpty(e.Comments) ? e.Comments : e.Message)}</p>");
        //                logsText.AppendLine(!string.IsNullOrEmpty(e.Comments) ? e.Comments : e.Message);
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(DocNum))
        //        {
        //            details.AppendLine($@"<p>Se obtuvieron los siguientes números de OV: {DocNum}</p>");
        //            logsText.AppendLine($@"Se obtuvieron los siguientes números de OV: {DocNum}");
        //        }

        //        string mailBody = $@"<style> 
        //                               body {{ background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; }} 
        //                               img {{ margin: 20px 15px; }}
        //                               td {{ padding: 0 8px; line-height: 18px; }}
        //                             </style>    
        //                             {(config.Test ? @"<p style=""font-size: 18px;""><strong>ORDEN DE PRUEBA (GENERADA EN BASE DE PRUEBA)</strong></p>" : "")}
        //                             <p>Solicitud recibida el : {LogDate:dd/MM/yyyy HH:mm:ss}</p>
        //                             <span>Datos Recibidos: </span><br />
        //                             <div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"">
        //                               <table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse;"">
        //                                 <tr>
        //                                     <td style=""width: 20px;"">&nbsp;</td>
        //                                     <td>                                            
        //                                         <pre>{RequestBody.Replace("\r\n", "<br />")}</pre>
        //                                     </td>
        //                                     <td style=""width: 20px;"">&nbsp;</td>
        //                                 </tr>
        //                               </table>                
        //                             </div><br />
        //                             {details}";

        //        List<MailAddress> tos = config.Audit.CopiesTo.Select(x => new MailAddress(x.EMail, x.Name)).ToList() ?? new List<MailAddress>();
        //        if (tos.Count > 0) await SendMailAsync("Log de solicitudes al servicio de Flow", mailBody, tos);

        //        if (!string.IsNullOrEmpty(path))
        //        {
        //            string fileName = $"{path}\\{DateTime.Now:yyyyMMdd-HHmmss}{(config.Test ? "-Test" : "")}.txt";
        //            FileStream objFile = File.Create(fileName);
        //            byte[] objData = Encoding.Unicode.GetBytes(logsText.ToString());
        //            objFile.Write(objData, 0, objData.Length);
        //            objFile.Close();
        //        }
        //    }
        //    catch (Exception) { }
        //}

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

        #endregion
    }
}