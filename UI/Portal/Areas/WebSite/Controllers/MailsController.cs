using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using Portal.Models;
using BCL = BComponents.Logs;
using BEL = BEntities.Logs;

namespace Portal.Areas.WebSite.Controllers
{
    [Area("WebSite")]
    public class MailsController : BaseController
    {
        #region Variables Globales

        private readonly WebSiteSettings _config;

        #endregion

        #region Constructores

        public MailsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        { /*config = configuration;*/
            _config = configuration.GetSection("WebSiteSettings").Get<WebSiteSettings>();
        }

        #endregion

        #region GETs

        // GET: WebSite/Mails
        public ActionResult Index()
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

        public IActionResult Filter(string Type, DateTime? Since, DateTime? Until)
        {
            string message = "";
            try
            {
                List<Field> filters = new();
                if (Since.HasValue) filters.Add(new Field("CAST(LogDate AS DATE)", Since.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                if (Until.HasValue) filters.Add(new Field("CAST(LogDate AS DATE)", Until.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                BCL.WorkMail bcWorkLog = new();
                BCL.RMAMail bcRMALog = new();
                BCL.ContactMail bcContact = new();
                switch (Type)
                {
                    case "W":
                        CompleteFilters(ref filters);
                        var workMails = bcWorkLog.List(filters, "1");
                        return Json(new { message, items = workMails });
                    case "RU":
                        filters.Add(new Field("[Type]", "FU"));
                        CompleteFilters(ref filters);
                        var rmas = bcRMALog.List(filters, "1");
                        return Json(new { message, items = rmas });
                    case "RD":
                        filters.Add(new Field("[Type]", "DI"));
                        CompleteFilters(ref filters);
                        var rmas2 = bcRMALog.List(filters, "1");
                        return Json(new { message, items = rmas2 });
                    default:
                        CompleteFilters(ref filters);
                        var contacts = bcContact.List(filters, "1");
                        return Json(new { message, items = contacts });
                }
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
        public IActionResult Contact(string EMail, string Name = "", string Company = "", string Category = "", string Position = "", string WebSiteUrl = "", string Region = "", string City = "", string Address = "", string Phone = "",
            string Message = "", string NIT = "", string TipoDeCliente = "")
        {
            DateTime date = DateTime.Now;

            var sb = new StringBuilder();
            sb.AppendLine($@"<html>");
            sb.AppendLine($@"<head>");
            sb.AppendLine($@"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.AppendLine($@"<title>FORMULARIO CONTACTO DMC.bo</title>");
            sb.AppendLine($@"<style>");
            sb.AppendLine(@"body, p, table, th, td, div { font-family: Arial, Helvetica, sans-serif; font-size: 12px; }");
            sb.AppendLine(@"th { background-color:#cc3300; color:white; font-weight:bold; font-size:18px; border: 1px solid #cc3300; }");
            sb.AppendLine(@".margin-10 { margin-bottom: 10px; }");
            sb.AppendLine(@"#header { margin: 20px 0; background: #cc3300; text-align: center; color: white; font: bold 15px Helvetica, Sans-Serif; text-decoration: uppercase; letter-spacing: 10px; padding: 8px; }");
            sb.AppendLine(@"#terms { text-align: center; margin: 20px 0 0 0; }");
            sb.AppendLine(@"#terms h5 { text-transform: uppercase; font: 13px Helvetica, Sans-Serif; letter-spacing: 10px; border-bottom: 1px solid black; padding: 0 0 8px 0; margin: 0 0 8px 0; }");
            sb.AppendLine(@"td.colone { text-align: right; vertical-align: top; padding-top:6px; width:30%; }");
            sb.AppendLine(@"td.coltwo { color:red; text-align: center; vertical-align: top; padding-top:9px; }");
            sb.AppendLine(@"td.colthree { width:70% }");
            sb.AppendLine(@"table.border { border: 1px solid #cc3300; border-collapse: collapse; }");
            sb.AppendLine($@"</style>");
            sb.AppendLine($@"</head>");
            sb.AppendLine($@"<body>");
            sb.AppendLine($@"<img src=""http://www.dmc.bo/img/logox.png"" width=""100"" height=""40"" alt=""LOGO DMC"" class=""margin-10"" /><br />");
            sb.AppendLine($@"<table class=""border "" width=""100%"" cellpadding=""3"" cellspacing=""0"">");
            sb.AppendLine($@"  <tr><th colspan=""3"" id=""header"" align=""center"">NUEVO MENSAJE DEL FORMULARIO DE CONTACTO</th></tr>");
            sb.AppendLine($@"</table><br />");
            sb.AppendLine($@"<table class=""border"" width=""100%"" cellpadding=""2"" cellspacing=""0"">");
            sb.AppendLine($@"  <tr><th colspan=""3"" id=""header"" align=""center"">MENSAJE DE {TipoDeCliente.ToUpper() ?? "USUARIO"}</th></tr>");
            if (!string.IsNullOrEmpty(EMail)) sb.AppendLine($@"  <tr><td class=""colone"">Correo:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{EMail}</td></tr>");
            if (!string.IsNullOrEmpty(Name)) sb.AppendLine($@"  <tr><td class=""colone"">Nombre Completo:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Name}</td></tr>");
            if (!string.IsNullOrEmpty(Company)) sb.AppendLine($@"  <tr><td class=""colone"">Compa&ntilde;ia:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Company}</td></tr>");
            if (!string.IsNullOrEmpty(NIT)) sb.AppendLine($@"  <tr><td class=""colone"">NIT:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{NIT}</td></tr>");
            if (!string.IsNullOrEmpty(Category)) sb.AppendLine($@"  <tr><td class=""colone"">Rubro:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Category}</td></tr>");
            if (!string.IsNullOrEmpty(TipoDeCliente)) sb.AppendLine($@"  <tr><td class=""colone"">Tipo de cliente:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{TipoDeCliente}</td></tr>");
            if (!string.IsNullOrEmpty(Position)) sb.AppendLine($@"  <tr><td class=""colone"">Cargo:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Position}</td></tr>");
            if (!string.IsNullOrEmpty(WebSiteUrl)) sb.AppendLine($@"  <tr><td class=""colone"">Web:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{WebSiteUrl}</td></tr>");
            if (!string.IsNullOrEmpty(Region)) sb.AppendLine($@"  <tr><td class=""colone"">Regi&oacute;n:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Region}</td></tr>");
            if (!string.IsNullOrEmpty(City)) sb.AppendLine($@"  <tr><td class=""colone"">Ciudad:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{City}</td></tr>");
            if (!string.IsNullOrEmpty(Address)) sb.AppendLine($@"  <tr><td class=""colone"">Direcci&oacute;n:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Address}</td></tr>");
            if (!string.IsNullOrEmpty(Phone)) sb.AppendLine($@"  <tr><td class=""colone"">Tel.:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Phone}</td></tr>");
            if (!string.IsNullOrEmpty(Message)) sb.AppendLine($@"  <tr><td class=""colone"">Mensaje:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{Message}</td></tr>");
            sb.AppendLine($@"  <tr><td class=""colone"">Fecha - Hora de env&iacute;o:</td><td class=""coltwo"">&nbsp;</td><td class=""colthree"">{date:dd/MM/yyyy HH:mm}</td></tr>");
            sb.AppendLine($@"</table><br />");
            sb.AppendLine($@"<div id=""terms"">Su correo se ha enviado, pronto nos pondremos en contacto para ayudarle.</div>");
            sb.AppendLine($@"</body>");
            sb.AppendLine($@"</html>");

            string returnValue = sb.ToString();
            sb.Replace("Su correo se ha enviado, pronto nos pondremos en contacto para ayudarle.", "");

            string title = $"Formulario de contacto - Tipo Cliente: {TipoDeCliente ?? "No definido"}";
            MailAddress mailUser, mailSystem;
            try
            {
                mailUser = new MailAddress(EMail, Name);
                mailSystem = new MailAddress(_config.ContactMail.Sender.EMail, _config.ContactMail.Sender.Name);
                List<MailAddress> sendTo = new() { mailUser }, copiesTo = new(), blindCopiesTo = new();
                FillCustomCopies("WebSite", "Mails", "Contact", ref copiesTo, ref blindCopiesTo);
                _ = SendMailAsync(title, sb.ToString(), sendTo, copiesTo, blindCopiesTo, mailSystem);

                BCL.ContactMail bcLog = new();
                BEL.ContactMail item = new() { StatusType = BEntities.StatusType.Insert, LogDate = date, Address = Address, Category = Category, City = City, ClientType = TipoDeCliente, Company = Company, EMail = EMail, Message = Message, Name = Name, NIT = NIT, Phone = Phone, Position = Position, WebSiteUrl = WebSiteUrl, Region = Region };
                bcLog.Save(ref item);
            }
            catch (Exception ex)
            {
                returnValue = ErrorMessage(Name, ex);
            }
            return Content(returnValue);
        }

        [HttpPost]
        public IActionResult Work(string FirstName, string LastName, string EMail, string Birthday = "", string MaritalStatus = "", string IdentitynDoc = "", string City = "", string Address = "", string Cellphone = "",
            string Phone = "", string MeetRequirements = "", string AcademicTraining = "", string Experience = "", string Languages = "", string Hobbies = "", string AboutYourself = "", string References = "",
            string WhyUs = "", string Achievements = "", string LeavingReason = "", string LaboralExperience = "", string Position = "", string SalaryPretension = "", string TravelAvailability = "", string LinkCV = "")
        {
            string returnContent;
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($@"<html>");
                sb.AppendLine($@"<head>");
                sb.AppendLine($@"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
                sb.AppendLine($@"<title>FORMULARIO BOLSA DE TRABAJO</title>");
                sb.AppendLine($@"<style>");
                sb.AppendLine(@"body, p, table, th, td, div { font-family: Arial, Helvetica, sans-serif; font-size: 12px; }");
                sb.AppendLine(@"th { background-color:#0080C0; color:white; font-weight:bold; font-size:18px; border: 1px solid #0080C0; }");
                sb.AppendLine(@"#header { margin: 20px 0; background: #F9423A; text-align: center; color: white; font: bold 15px Helvetica, Sans-Serif; text-decoration: uppercase; letter-spacing: 10px; padding: 8px; }");
                sb.AppendLine(@"#terms { text-align: center; margin: 20px 0 0 0; }");
                sb.AppendLine(@"#terms h5 { text-transform: uppercase; font: 13px Helvetica, Sans-Serif; letter-spacing: 10px; border-bottom: 1px solid black; padding: 0 0 8px 0; margin: 0 0 8px 0; }");
                sb.AppendLine(@"td.colone { text-align: right; vertical-align: top; padding-top:6px; width:30%; }");
                sb.AppendLine(@"td.coltwo { color:red; text-align: center; vertical-align: top; padding-top:9px; }");
                sb.AppendLine(@"td.colthree { width:70% }");
                sb.AppendLine(@"table.border { border: 1px solid #0080C0; border-collapse: collapse; }");
                sb.AppendLine($@"</style>");
                sb.AppendLine($@"</head>");
                sb.AppendLine($@"<body>");
                sb.AppendLine($@"<img src='http://www.dmc.bo/img/logox.png' width='100' height='40' alt='LOGO DMC'>");
                sb.AppendLine($@"<table class='border' width='100%' cellpadding='3' cellspacing='0'>");
                sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>TRABAJO EN DMC</th></tr>");
                sb.AppendLine($@"</table><br /><br />");
                sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
                sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>INFORMACI&Oacute;N PERSONAL</th></tr>");
                if (!string.IsNullOrEmpty(FirstName)) sb.AppendLine($@"  <tr><td class='colone'>Nombres:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{FirstName}</td></tr>");
                if (!string.IsNullOrEmpty(LastName)) sb.AppendLine($@"  <tr><td class='colone'>Apellidos:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{LastName}</td></tr>");
                if (!string.IsNullOrEmpty(Birthday)) sb.AppendLine($@"   <tr><td class='colone'>Fecha de nacimiento:</td><td class='coltwo'></td><td class='colthree'>D&iacute;a: {Birthday}</td></tr>");
                if (!string.IsNullOrEmpty(MaritalStatus)) sb.AppendLine($@"  <tr><td class='colone'>Estado Civil:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{MaritalStatus}</td></tr>");
                if (!string.IsNullOrEmpty(IdentitynDoc)) sb.AppendLine($@"  <tr><td class='colone'>CI / Identificaci&oacute;n:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{IdentitynDoc}</td></tr>");
                if (!string.IsNullOrEmpty(City)) sb.AppendLine($@"  <tr><td class='colone'>Ciudad:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{City}</td></tr>");
                if (!string.IsNullOrEmpty(Address)) sb.AppendLine($@"  <tr><td class='colone'>Direcci&oacute;n:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Address}</td></tr>");
                if (!string.IsNullOrEmpty(Cellphone)) sb.AppendLine($@"  <tr><td class='colone'>Celular:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Cellphone}</td></tr>");
                if (!string.IsNullOrEmpty(Phone)) sb.AppendLine($@"  <tr><td class='colone'>Tel&eacute;fono:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Phone}</td></tr>");
                if (!string.IsNullOrEmpty(EMail)) sb.AppendLine($@"  <tr><td class='colone'>Correo:</td><td class='coltwo'></td><td class='colthree'>{EMail}</td></tr>");
                sb.AppendLine($@"</table><br /><br />");
                sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
                sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>PERFIL PROFESIONAL</th></tr>");
                if (!string.IsNullOrEmpty(MeetRequirements)) sb.AppendLine($@"  <tr><td class='colone'>¿Cumplo los requisitos solicitados para la vacante?</td><td class='coltwo'></td><td class='colthree'>{MeetRequirements}</td></tr>");
                if (!string.IsNullOrEmpty(AcademicTraining)) sb.AppendLine($@"  <tr><td class='colone'>Informaci&oacute;n acad&eacute;mica y conocimientos:(Detalle el nivel mayor de educaci&oacute;n obtenida, conocimientos adicionales y/o especializaciones)</td><td class='coltwo'></td><td class='colthree'>{AcademicTraining}</td></tr>");
                if (!string.IsNullOrEmpty(Experience)) sb.AppendLine($@"  <tr><td class='colone'>Experiencia laboral (Detalle):</td><td class='coltwo'></td><td class='colthree'>{Experience}</td></tr>");
                if (!string.IsNullOrEmpty(Languages)) sb.AppendLine($@"  <tr><td class='colone'>Idiomas (Detalle si lee, escribe y habla):</td><td class='coltwo'></td><td class='colthree'>{Languages}</td></tr>");
                if (!string.IsNullOrEmpty(Hobbies)) sb.AppendLine($@"  <tr><td class='colone'>Hobbies: (Intereses, actividades, deportes, afiliaciones, servicio social, etc.)</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Hobbies}</td></tr>");
                if (!string.IsNullOrEmpty(AboutYourself)) sb.AppendLine($@"  <tr><td class='colone'>Descr&iacute;bete: (M&aacute;ximo 100 palabras)</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{AboutYourself}</td></tr>");
                if (!string.IsNullOrEmpty(References)) sb.AppendLine($@"  <tr><td class='colone'>Referencias personales:(M&iacute;nimo 3)</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{References}</td></tr>");
                if (!string.IsNullOrEmpty(WhyUs)) sb.AppendLine($@"  <tr><td class='colone'>¿Porque te interesar&iacute;a trabajar con nosotros?: (Detalle)</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{WhyUs}</td></tr>");
                if (!string.IsNullOrEmpty(Achievements)) sb.AppendLine($@"  <tr><td class='colone'>Logros y objetivos(Detalle):</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Achievements}</td></tr>");
                sb.AppendLine($@"</table><br /><br />");
                sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
                sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>PREFERENCIAS LABORALES </th></tr>");
                if (!string.IsNullOrEmpty(LeavingReason)) sb.AppendLine($@"  <tr><td class='colone'>Raz&oacute;n para cambiar de empleo:</td><td class='coltwo'></td><td class='colthree'>{LeavingReason}</td></tr>");
                if (!string.IsNullOrEmpty(LaboralExperience)) sb.AppendLine($@"  <tr><td class='colone'>Experiencia laboral (a&ntilde;os)</td><td class='coltwo'></td><td class='colthree'>{LaboralExperience}</td></tr>");
                if (!string.IsNullOrEmpty(Position)) sb.AppendLine($@"  <tr><td class='colone'>Posici&oacute;n deseada</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Position}</td></tr>");
                if (!string.IsNullOrEmpty(SalaryPretension)) sb.AppendLine($@"  <tr><td class='colone'>Pretensi&oacute;n salarial mensual:</td><td class='coltwo'></td><td class='colthree'>{SalaryPretension}</td></tr>");
                if (!string.IsNullOrEmpty(TravelAvailability)) sb.AppendLine($@"  <tr><td class='colone'>Disponibilidad para viajar:</td><td class='coltwo'></td><td class='colthree'>{TravelAvailability}</td></tr>");
                sb.AppendLine($@"  <tr><td class='colone'>Archivo adjunto: </td><td class='coltwo'></td><td class='colthree'>");
                if (!string.IsNullOrWhiteSpace(LinkCV)) sb.Append($@"<a href=""{LinkCV}"" target=""_blank"" >Descargar CV</a>");
                sb.Append(@"</td></tr>");
                sb.AppendLine($@"</table><br /><br />");
                sb.AppendLine($@"<div id='terms'>");
                sb.Append($@"<p>Gracias por su inter&eacute;s en trabajar para nuestra Empresa.</p>");
                sb.Append($@"<p>Nos pondremos en contacto con usted si tenemos una posici&oacute;n disponible de acuerdo a su solicitud.</p></div>");
                sb.AppendLine($@"</body>");
                sb.AppendLine($@"</html>");

                returnContent = sb.ToString();
                string title = "Formulario de bolsa de trabajo";
                MailAddress mailUser = new(EMail, $"{FirstName} {LastName}"), mailSystem = new(_config.WorkMail.Sender.EMail, _config.WorkMail.Sender.Name);

                List<MailAddress> sendTo = new() { mailUser }, copiesTo = new(), blindCopiesTo = new();
                FillCustomCopies("WebSite", "Mails", "Work", ref copiesTo, ref blindCopiesTo);

                _ = SendMailAsync(title, sb.ToString(), sendTo, copiesTo, blindCopiesTo, mailSystem);

                BCL.WorkMail bcLog = new();
                BEL.WorkMail item = new() { StatusType = BEntities.StatusType.Insert, LogDate = DateTime.Now, AboutYourself = AboutYourself, AcademicTraining = AcademicTraining, Achievements = Achievements, Address = Address, Birthday = Birthday, Cellphone = Cellphone, City = City, EMail = EMail, Experience = Experience, FirstName = FirstName, Hobbies = Hobbies, IdentitynDoc = IdentitynDoc, LaboralExperience = LaboralExperience, Languages = Languages, LastName = LastName, LeavingReason = LeavingReason, LinkCV = LinkCV, MaritalStatus = MaritalStatus, MeetRequirements = MeetRequirements, Phone = Phone, Position = Position, References = References, SalaryPretension = SalaryPretension, TravelAvailability = TravelAvailability, WhyUs = WhyUs };
                bcLog.Save(ref item);
            }
            catch (Exception ex)
            {
                returnContent = ErrorMessage($"{FirstName} {LastName}", ex);
            }
            return Content(returnContent);
        }

        [HttpPost]
        public IActionResult RMAFinalUser(string Name, string Company, string TaxNumber, string City, string Address, string Cellphone, string Phone, string EMail, string ProductDesc,
            string Brand, string ItemCode, string SerialNumber, DateTime SaleDate, string SaleNote, string Failure)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"<html>");
            sb.AppendLine($@"<head>");
            sb.AppendLine($@"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.AppendLine($@"<title>FORMULARIO RMA CLIENTE</title>");
            sb.AppendLine($@"<style>");
            sb.AppendLine(@"body, p, table, th, td, div { font-family: Arial, Helvetica, sans-serif; font-size: 12px; }");
            sb.AppendLine(@"th { background-color:#0080C0; color:white; font-weight:bold; font-size:18px; border: 1px solid #0080C0; }");
            sb.AppendLine(@"#header { margin: 20px 0; background: #0080C0; text-align: center; color: white; font: bold 15px Helvetica, Sans-Serif; text-decoration: uppercase; letter-spacing: 10px;padding: 8px; }");
            sb.AppendLine(@"#terms { text-align: center; margin: 20px 0 0 0; }");
            sb.AppendLine(@"#terms h5 { text-transform: uppercase; font: 13px Helvetica, Sans-Serif; letter-spacing: 10px; border-bottom: 1px solid black; padding: 0 0 8px 0; margin: 0 0 8px 0; }");
            sb.AppendLine(@"td.colone { text-align: right; vertical-align: top; padding-top:6px; width:30%; }");
            sb.AppendLine(@"td.coltwo { color:red; text-align: center; vertical-align: top; padding-top:9px; }");
            sb.AppendLine(@"td.colthree { width:70% }");
            sb.AppendLine(@"table.border { border: 1px solid #0080C0; border-collapse: collapse; }");
            sb.AppendLine($@"</style>");
            sb.AppendLine($@"</head>");
            sb.AppendLine($@"<body>");
            sb.AppendLine($@"<img src='http://www.dmc.bo/img/logox.png' width='100' height='40' alt='LOGO DMC'>");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='3' cellspacing='0'>");
            sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>SERVICIO TÉCNICO - RMA</th></tr>");
            sb.AppendLine($@"</table><br /><br />");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
            sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>USUARIO FINAL</th></tr>");
            if (!string.IsNullOrEmpty(Name))
                sb.AppendLine($@"  <tr><td class='colone'>Nombre Completo:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Name}</td></tr>");
            if (!string.IsNullOrEmpty(Company))
                sb.AppendLine($@"  <tr><td class='colone'>Empresa:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Company}</td></tr>");
            if (!string.IsNullOrEmpty(TaxNumber))
                sb.AppendLine($@"  <tr><td class='colone'>NIT:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{TaxNumber}</td></tr>");
            if (!string.IsNullOrEmpty(City))
                sb.AppendLine($@"  <tr><td class='colone'>Ciudad:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{City}</td></tr>");
            if (!string.IsNullOrEmpty(Address))
                sb.AppendLine($@"  <tr><td class='colone'>Dirección:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Address}</td></tr>");
            if (!string.IsNullOrEmpty(Cellphone))
                sb.AppendLine($@"  <tr><td class='colone'>Celular:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Cellphone}</td></tr>");
            if (!string.IsNullOrEmpty(Phone))
                sb.AppendLine($@"  <tr><td class='colone'>Teléfono:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Phone}</td></tr>");
            if (!string.IsNullOrEmpty(EMail))
                sb.AppendLine($@"  <tr><td class='colone'>Correo:</td><td class='coltwo'></td><td class='colthree'>{EMail}</td></tr>");
            sb.AppendLine($@"</table><br /><br />");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
            sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>INFORMACIÓN DEL PRODUCTO</th></tr>");
            if (!string.IsNullOrEmpty(ProductDesc))
                sb.AppendLine($@"  <tr><td class='colone'>Descripción del Producto:</td><td class='coltwo'></td><td class='colthree'>{ProductDesc}</td></tr>");
            if (!string.IsNullOrEmpty(Brand))
                sb.AppendLine($@"  <tr><td class='colone'>Marca:</td><td class='coltwo'></td><td class='colthree'>{Brand}</td></tr>");
            if (!string.IsNullOrEmpty(ItemCode))
                sb.AppendLine($@"  <tr><td class='colone'>C&oacute;digo o No. De parte:</td><td class='coltwo'></td><td class='colthree'>{ItemCode}</td></tr>");
            if (!string.IsNullOrEmpty(SerialNumber))
                sb.AppendLine($@"  <tr><td class='colone'>No. de Serie:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{SerialNumber}</td></tr>");
            sb.AppendLine($@"  <tr><td class='colone'>Fecha de compra:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{SaleDate.ToString("dd/MM/yyyy")}</td></tr>");
            if (!string.IsNullOrEmpty(SaleNote))
                sb.AppendLine($@"  <tr><td class='colone'> No. De Nota de Venta:</td><td class='coltwo'></td><td class='colthree'>{SaleNote}</td></tr>");
            if (!string.IsNullOrEmpty(Failure))
                sb.AppendLine($@"  <tr><td bordercolor='1'class='colone'>Problema Reportado: </td><td bordercolor='1'class='coltwo'></td><td bordercolor='1'class='colthree'>{Failure}</td></tr>");
            sb.AppendLine($@"</table><br />");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
            sb.AppendLine($@"  <tr><td>Correo de servicio:</td><td><a href='mailto:rma@dmc.bo'>rma@dmc.bo</a></td></tr>");
            sb.AppendLine($@"  <tr><td >Teléfono:</td><td class='colthree'>3543000</td></tr>");
            sb.AppendLine($@"  <tr><td >Fecha - Hora de envio:</td><td class='colthree'>{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</td></tr>");
            sb.AppendLine($@"</table><br />");
            sb.AppendLine($@"<div>El presente documento no autoriza a enviar el producto a nuestras oficinas, es simplemente la solicitud de servicio para que personal asignado en DMC tome contacto con usted(es) , le(s) asigne un n&uacute;mero de RMA y le(s) instruya los pasos a seguir en el proceso.");
            sb.AppendLine($@"</div><br />");
            sb.AppendLine($@"<div id='terms'><p>Nos pondremos en contacto con usted a la brevedad posible.</p></div>");
            sb.AppendLine($@"</body>");
            sb.AppendLine($@"</html>");
            string returnValue;
            try
            {
                returnValue = sb.ToString();
                string title = "Formulario RMA cliente";
                MailAddress mailUser = new(EMail, Name), mailSystem = new(_config.ClientRMAMail.Sender.EMail, _config.ClientRMAMail.Sender.Name);

                List<MailAddress> sendTo = new() { mailUser }, copiesTo = new(), blindCopiesTo = new();
                FillCustomCopies("WebSite", "Mails", "RMAFinalUser", ref copiesTo, ref blindCopiesTo);

                _ = SendMailAsync(title, sb.ToString(), sendTo, copiesTo, blindCopiesTo, mailSystem);

                BCL.RMAMail bcLog = new();
                BEL.RMAMail item = new() { StatusType = BEntities.StatusType.Insert, LogDate = DateTime.Now, Address = Address, Brand = Brand, Cellphone = Cellphone, City = City, Company = Company, EMail = EMail, Failure = Failure, ItemCode = ItemCode, Name = Name, Phone = Phone, ProductDesc = ProductDesc, SaleNote = SaleNote, SaleDate = SaleDate, SerialNumber = SerialNumber, TaxNumber = TaxNumber, Type = "FU" };
                bcLog.Save(ref item);
            }
            catch (Exception ex)
            {
                returnValue = ErrorMessage(Name, ex);
            }

            return Content(returnValue);
        }

        [HttpPost]
        public IActionResult RMADistributor(string Name, string Company, string TaxNumber, string City, string Address, string Cellphone, string Phone, string EMail, string ProductDesc,
            string Brand, string ItemCode, string SerialNumber, DateTime SaleDate, string SaleNote, string Failure)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"<html>");
            sb.AppendLine($@"<head>");
            sb.AppendLine($@"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.AppendLine($@"<title>FORMULARIO RMA DISTRIBUIDOR</title>");
            sb.AppendLine($@"<style>");
            sb.AppendLine(@"body, p, table, th, td, div { font-family: Arial, Helvetica, sans-serif; font-size: 12px; }");
            sb.AppendLine(@"th { background-color:#0080C0; color:white; font-weight:bold; font-size:18px; border: 1px solid #0080C0; }");
            sb.AppendLine(@"#header { margin: 20px 0; background: #0080C0; text-align: center; color: white; font: bold 15px Helvetica, Sans-Serif; text-decoration: uppercase; letter-spacing: 10px; padding: 8px; }");
            sb.AppendLine(@"#terms { text-align: center; margin: 20px 0 0 0; }");
            sb.AppendLine(@"#terms h5 { text-transform: uppercase; font: 13px Helvetica, Sans-Serif; letter-spacing: 10px; border-bottom: 1px solid black; padding: 0 0 8px 0; margin: 0 0 8px 0; }");
            sb.AppendLine(@"td.colone { text-align: right; vertical-align: top; padding-top:6px; width:30%; }");
            sb.AppendLine(@"td.coltwo { color:red; text-align: center; vertical-align: top; padding-top:9px; }");
            sb.AppendLine(@"td.colthree { width:70% }");
            sb.AppendLine(@"table.border { border: 1px solid #0080C0; border-collapse: collapse; }");
            sb.AppendLine(@"caption { text-align:center; font-size:18px; font-weight:bold; }");
            sb.AppendLine($@"</style>");
            sb.AppendLine($@"</head>");
            sb.AppendLine($@"<body>");
            sb.AppendLine($@"<img src='http://www.dmc.bo/img/logox.png' width='100' height='40' alt='LOGO DMC'> ");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='3' cellspacing='0'>");
            sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>SERVICIO T&Eacute;CNICO - RMA</th></tr>");
            sb.AppendLine($@"</table><br /><br />");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
            sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>DISTRIBUIDORES</th></tr>");
            if (!string.IsNullOrEmpty(Name))
                sb.AppendLine($@"  <tr><td class='colone'>Nombre Completo:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Name}</td></tr>");
            if (!string.IsNullOrEmpty(Company))
                sb.AppendLine($@"  <tr><td class='colone'>Empresa:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Company}</td></tr>");
            if (!string.IsNullOrEmpty(TaxNumber))
                sb.AppendLine($@"  <tr><td class='colone'>NIT:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{TaxNumber}</td></tr>");
            if (!string.IsNullOrEmpty(City))
                sb.AppendLine($@"  <tr><td class='colone'>Ciudad:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{City}</td></tr>");
            if (!string.IsNullOrEmpty(Address))
                sb.AppendLine($@"  <tr><td class='colone'>Dirección:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Address}</td></tr>");
            if (!string.IsNullOrEmpty(Cellphone))
                sb.AppendLine($@"  <tr><td class='colone'>Celular:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Cellphone}</td></tr>");
            if (!string.IsNullOrEmpty(Phone))
                sb.AppendLine($@"  <tr><td class='colone'>Teléfono:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{Phone}</td></tr>");
            if (!string.IsNullOrEmpty(EMail))
                sb.AppendLine($@"  <tr><td class='colone'>Correo:</td><td class='coltwo'></td><td class='colthree'>{EMail}</td></tr>");
            sb.AppendLine($@"</table><br /><br />");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>");
            sb.AppendLine($@"  <tr><th colspan='3' id='header' align='center'>INFORMACIÓN DEL PRODUCTO </th></tr>");
            if (!string.IsNullOrEmpty(ProductDesc))
                sb.AppendLine($@"  <tr><td class='colone'>Descripción del Producto:</td><td class='coltwo'></td><td class='colthree'>{ProductDesc}</td></tr>");
            if (!string.IsNullOrEmpty(Brand))
                sb.AppendLine($@"  <tr><td class='colone'>Marca:</td><td class='coltwo'></td><td class='colthree'>{Brand}</td></tr>");
            if (!string.IsNullOrEmpty(ItemCode))
                sb.AppendLine($@"  <tr><td class='colone'>Código o No. De parte:</td><td class='coltwo'></td><td class='colthree'>{ItemCode}</td></tr>");
            if (!string.IsNullOrEmpty(SerialNumber))
                sb.AppendLine($@"  <tr><td class='colone'>No. de Serie:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{SerialNumber}</td></tr>");
            sb.AppendLine($@"  <tr><td class='colone'>Fecha de compra:</td><td class='coltwo'>&nbsp;</td><td class='colthree'>{SaleDate.ToString("dd/MM/yyyy")}</td></tr>");
            if (!string.IsNullOrEmpty(SaleNote))
                sb.AppendLine($@"  <tr><td class='colone'> No. De Nota de Venta:</td><td class='coltwo'></td><td class='colthree'>{SaleNote}</td></tr>");
            if (!string.IsNullOrEmpty(Failure))
                sb.AppendLine($@"  <tr><td class='colone'>Problema Reportado: </td><td class='coltwo'></td><td class='colthree'>{Failure}</td></tr>");
            sb.AppendLine($@"</table><br />");
            sb.AppendLine($@"<table class='border' width='100%' cellpadding='2' cellspacing='0'>  ");
            sb.AppendLine($@"  <tr><td>Correo de servicio:</td><td ><a href='mailto:rma@dmc.bo'>rma@dmc.bo</a></td></tr>");
            sb.AppendLine($@"  <tr><td >Tel&eacute;fono:</td><td class='colthree'>3543000</td></tr>");
            sb.AppendLine($@"  <tr><td >Fecha - Hora de envio:</td><td class='colthree'>{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</td></tr>");
            sb.AppendLine($@"</table><br />");
            sb.AppendLine($@"<div>El presente documento no autoriza a enviar el producto a nuestras oficinas, es simplemente la solicitud de servicio para que personal asignado en DMC tome contacto con usted(es) , le(s) asigne un n&uacute;mero de RMA y le(s) instruya los pasos a seguir en el proceso.</div><br />");
            sb.AppendLine($@"<div id='terms'><p>Nos pondremos en contacto con usted a la brevedad posible.</p></div> ");
            sb.AppendLine($@"</body>");
            sb.AppendLine($@"</html>");

            string returnValue;
            try
            {
                returnValue = sb.ToString();
                string title = "Formulario RMA Distribuidor";
                MailAddress mailUser = new(EMail, Name), mailSystem = new(_config.DistributorRMAMail.Sender.EMail, _config.DistributorRMAMail.Sender.Name);

                List<MailAddress> sendTo = new() { mailUser }, copiesTo = new(), blindCopiesTo = new();
                FillCustomCopies("WebSite", "Mails", "RMADistributor", ref copiesTo, ref blindCopiesTo);

                _ = SendMailAsync(title, sb.ToString(), sendTo, copiesTo, blindCopiesTo, mailSystem);

                BCL.RMAMail bcLog = new();
                BEL.RMAMail item = new() { StatusType = BEntities.StatusType.Insert, LogDate = DateTime.Now, Address = Address, Brand = Brand, Cellphone = Cellphone, City = City, Company = Company, EMail = EMail, Failure = Failure, ItemCode = ItemCode, Name = Name, Phone = Phone, ProductDesc = ProductDesc, SaleNote = SaleNote, SaleDate = SaleDate, SerialNumber = SerialNumber, TaxNumber = TaxNumber, Type = "DI" };
                bcLog.Save(ref item);
            }
            catch (Exception ex)
            {
                returnValue = ErrorMessage(Name, ex);
            }
            return Content(returnValue);
        }

        #endregion

        #region Private Methods

        private string ErrorMessage(string Name, Exception ex)
        {
            string error = GetError(ex);
            StringBuilder sb = new();
            sb.AppendLine(@"	<style> ");
            sb.AppendLine(@"		body { background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; } ");
            sb.AppendLine(@"		img { margin: 20px 15px; }");
            sb.AppendLine(@"		td { padding: 0 8px; line-height: 18px; }");
            sb.AppendLine(@"	</style>");
            sb.AppendLine(@"	<div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"" >");
            sb.AppendLine(@"		<table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse;"">");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine(@"                    <p style='font-weight:bold; font-size: 1.5em; color: #ff0000;'>ERROR AL PROCESAR SU SOLICITUD</p>");
            sb.AppendLine($@"				    <p>Estimado(a) Sr(a). <strong>{Name}</strong></p>");
            sb.AppendLine(@"                    <p>Se ha producido un error al procesar su solicitud, por favor intente nuevamente.</p>");
            sb.AppendLine($@"                   <p>{ex.Message}</p>");
            sb.AppendLine(@"					<br /><p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"	</div>");
            return sb.ToString();
        }

        #endregion
    }
}
