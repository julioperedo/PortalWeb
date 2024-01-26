using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using Portal.Membership;
using Portal.Models;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class UserRequestController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration config;
        #endregion

        #region Constructores

        public UserRequestController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            config = configuration;
        }

        #endregion

        #region GETs
        public IActionResult Index()
        {
            int intPermission = GetPermission("Solicitudes");
            if (IsAllowed(this))
            {
                ViewData["Permission"] = intPermission;
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(long State, string Filter)
        {
            List<Models.UserRequest> items = new();
            try
            {
                items = GetRequest(State, Filter);
            }
            catch (Exception) { }
            return Json(items);
        }

        public IActionResult DetailRequest(long IdRequest)
        {
            BCS.UserRequestDetail bcRequest = new();
            List<BES.UserRequestDetail> lstItems = bcRequest.List(IdRequest, "LogDate DESC");
            return Json(lstItems.Select(x => new { x.StateName, x.UserName, x.LogDate }));
        }

        #endregion

        #region POSTs

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRequest(string nombre, string ciudad, string regempresa, string cargo, string codempresa, string email, string telefono)
        {
            BCA.Client bcClient = new BCA.Client();
            string strMessage;
            try
            {
                BEA.Client beClient = bcClient.Search(codempresa);
                if (!string.IsNullOrWhiteSpace(beClient?.CardCode))
                {
                    BCS.UserRequest bcRequest = new BCS.UserRequest();
                    List<Field> lstFilter = new List<Field> { new Field("CardCode", codempresa), new Field("StateIdc", (long)BEE.States.UserRequest.Created), new Field(LogicalOperators.And) };
                    IEnumerable<BES.UserRequest> lstRequests = bcRequest.List(lstFilter, "RequestDate");

                    if (lstRequests?.Count() > 0)
                    {
                        strMessage = ExistingRequestMessage(codempresa, nombre);
                        string strErrorMail = SendEmailUser("Solicitud NO Aceptada", strMessage, email, false);
                        if (!string.IsNullOrEmpty(strErrorMail))
                        {
                            strMessage = MailErrorMessage(nombre, email, strErrorMail);
                        }
                    }
                    else
                    {
                        BES.UserRequest beRequest = new BES.UserRequest { ListUserRequestDetails = new List<BES.UserRequestDetail>() };
                        beRequest.CardCode = codempresa;
                        beRequest.City = ciudad;
                        beRequest.ClientName = regempresa == null ? "" : regempresa.Trim();
                        beRequest.EMail = email;
                        beRequest.FullName = nombre;
                        beRequest.Phone = telefono;
                        beRequest.Position = cargo;
                        beRequest.RequestDate = DateTime.Now;
                        beRequest.StateIdc = (long)BEE.States.UserRequest.Created;
                        beRequest.LogUser = 0;
                        beRequest.LogDate = DateTime.Now;
                        beRequest.StatusType = StatusType.Insert;
                        beRequest.ListUserRequestDetails.Add(new BES.UserRequestDetail { StatusType = StatusType.Insert, StateIdc = (long)BEE.States.UserRequest.Created, LogUser = 0, LogDate = DateTime.Now });

                        bcRequest.Save(ref beRequest);

                        strMessage = SuccessfulRequestMessage(codempresa, regempresa, ciudad, nombre, cargo, email, telefono);
                        string strErrorMail = SendEmailUser("Solicitud Recibida", strMessage, email, true);
                        if (!string.IsNullOrEmpty(strErrorMail))
                        {
                            strMessage = MailErrorMessage(nombre, email, strErrorMail);
                            beRequest.ListUserRequestDetails = new List<BES.UserRequestDetail>();
                            beRequest.StatusType = StatusType.Delete;
                            bcRequest.Save(ref beRequest);
                        }
                    }
                }
                else
                {
                    strMessage = RejectMessageByCode(codempresa, nombre);
                }
            }
            catch (Exception ex)
            {
                string strError = GetError(ex);
                return Content(strError);
            }
            return Content(strMessage);
        }

        [HttpPost]
        public ActionResult ApproveUser(long IdRequest, long IdUser)
        {
            string message = "";
            bool result = true;
            try
            {
                DateTime logDate = DateTime.Now;
                BCS.UserRequest bcRequest = new BCS.UserRequest();
                BES.UserRequest beRequest = bcRequest.Search(IdRequest);
                BCS.User bcUser = new BCS.User();
                BES.User beUser;

                string strPassword = NewPassword();

                if (IdUser > 0)
                {
                    beUser = bcUser.Search(IdUser);
                    beUser.StatusType = BEntities.StatusType.Update;
                }
                else
                {
                    BCA.Client bcClient = new BCA.Client();
                    BEA.Client beClient = bcClient.Search(beRequest.CardCode);

                    beUser = new BES.User
                    {
                        StatusType = BEntities.StatusType.Insert,
                        CardCode = beRequest.CardCode.ToUpper(),
                        Position = beRequest.Position,
                        AccountHolder = true,
                        FirstName = beClient.CardName,
                        LastName = beClient.CardFName,
                        Address = beClient.Address,
                        Phone = beClient.Phone1
                    };
                }
                beUser.CardCode = beRequest.CardCode;
                beUser.EMail = beRequest.EMail;
                beUser.Login = beRequest.EMail;
                beUser.IdProfile = 2;
                beUser.Enabled = true;
                beUser.Password = Crypt.Encrypt(strPassword);
                beUser.LogUser = UserCode;
                beUser.LogDate = logDate;
                bcUser.Save(ref beUser);

                //Cambio el estado de la solicitud
                beRequest.StatusType = BEntities.StatusType.Update;

                beRequest.StateIdc = (long)BEE.States.UserRequest.Accepted;
                beRequest.LogDate = logDate;
                beRequest.LogUser = UserCode;
                beRequest.ListUserRequestDetails = new List<BES.UserRequestDetail> {
                    new BES.UserRequestDetail { StatusType = BEntities.StatusType.Insert, IdRequest = beRequest.Id, StateIdc = (long)BEE.States.UserRequest.Accepted, LogUser = UserCode, LogDate = logDate }
                };
                bcRequest.Save(ref beRequest);

                //Envío el correo
                if (!string.IsNullOrWhiteSpace(beUser.EMail) && IsValidEmail(beUser.EMail))
                {
                    string strMail = NewUserMailBody($"{beUser.CardCode} - {beUser.Name}", beUser.Login, strPassword);
                    string strErrorMail = SendEmailUser("Datos de Acceso al Portal de DMC", strMail, beUser.EMail, true);
                    if (!string.IsNullOrWhiteSpace(strErrorMail))
                    {
                        message = "Se produjo el siguiente error al enviar el correo: <br />" + strErrorMail;
                        result = false;

                        if (IdUser > 0)
                        {
                            //Deshabilito el usuario
                            beUser.StatusType = BEntities.StatusType.Update;
                            beUser.Enabled = false;
                        }
                        else
                        {
                            //Elimino el usuario creado
                            beUser.StatusType = BEntities.StatusType.Delete;
                        }
                        bcUser.Save(ref beUser);

                        //Vuelvo la solicitud a su estado anterior
                        beRequest.StatusType = BEntities.StatusType.Update;
                        beRequest.StateIdc = (long)BEE.States.UserRequest.Created;
                        beRequest.ListUserRequestDetails[0].StatusType = BEntities.StatusType.Delete;
                        bcRequest.Save(ref beRequest);
                    }
                    else
                    {
                        message += "Se ha habilitado al usuario correctamente y enviado el correo con los datos de acceso.";
                    }
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
                result = false;
            }
            return Json(new { message, result });
        }

        [HttpPost]
        public ActionResult RejectRequest(long IdRequest, string Code, string Name, string EMail, bool NameMismatch, bool WithoutOrders, bool InvalidEMail, bool IsNew)
        {
            string message = "Se ha rechazado la Solicitud Correctamente.";
            bool result = true;
            try
            {
                DateTime logDate = DateTime.Now;
                long stateIdc = (long)BEE.States.UserRequest.Rejected;
                BCS.UserRequest bcRequest = new BCS.UserRequest();
                BES.UserRequest beRequest = bcRequest.Search(IdRequest);

                //Cambio el estado de la solicitud
                beRequest.StatusType = StatusType.Update;
                beRequest.StateIdc = stateIdc;
                beRequest.LogUser = UserCode;
                beRequest.LogDate = logDate;
                beRequest.ListUserRequestDetails = new List<BES.UserRequestDetail> { new BES.UserRequestDetail { StatusType = StatusType.Insert, IdRequest = beRequest.Id, StateIdc = stateIdc, LogUser = UserCode, LogDate = logDate } };
                bcRequest.Save(ref beRequest);

                string strMail = RejectMessage(Code, Name, NameMismatch, WithoutOrders, InvalidEMail, IsNew);
                string strErrorMail = SendEmailUser("Solicitud Rechazada al Portal de DMC", strMail, EMail, true);
                if (!string.IsNullOrEmpty(strErrorMail))
                {
                    message = "Se ha presentado el siguiente error al enviar el correo al Cliente: <br />" + strErrorMail;
                    result = false;

                    beRequest.StatusType = StatusType.Update;
                    beRequest.StateIdc = (long)BEE.States.UserRequest.Created;
                    beRequest.ListUserRequestDetails[0].StatusType = BEntities.StatusType.Delete;
                    beRequest.LogUser = UserCode;
                    beRequest.LogDate = logDate;
                    bcRequest.Save(ref beRequest);
                }
            }
            catch (Exception ex)
            {
                message = "Se ha producido el siguiente error al intentar rechazar la Solicitud: <br />" + GetError(ex);
                result = false;
            }
            return Json(new { message, result });
        }

        [HttpPost]
        public ActionResult DeleteRequest(long IdRequest, string ReasonReject)
        {
            string message = "";
            try
            {
                DateTime logDate = DateTime.Now;
                long stateId = (long)BEE.States.UserRequest.Deleted;
                BCS.UserRequest bcRequest = new BCS.UserRequest();
                BES.UserRequest beRequest = bcRequest.Search(IdRequest);
                if (beRequest != null)
                {
                    beRequest.Comments = ReasonReject;
                    beRequest.StateIdc = stateId;
                    beRequest.ListUserRequestDetails = new List<BES.UserRequestDetail>();
                    beRequest.StatusType = StatusType.Update;
                    beRequest.LogUser = UserCode;
                    beRequest.LogDate = logDate;

                    BES.UserRequestDetail beDetail = new BES.UserRequestDetail { StatusType = StatusType.Insert, StateIdc = stateId, LogUser = UserCode, LogDate = logDate };
                    beRequest.ListUserRequestDetails.Add(beDetail);

                    bcRequest.Save(ref beRequest);
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public ActionResult UndoRequest(long IdRequest)
        {
            string message = "Se ha revertido a Pendiente el estado de la solicitud";
            bool result = true;
            try
            {
                DateTime logDate = DateTime.Now;
                long intState = (long)BEE.States.UserRequest.Created;
                BCS.UserRequest bcRequest = new();
                BES.UserRequest beRequest = bcRequest.Search(IdRequest);
                if (beRequest != null)
                {
                    List<Field> lstFilter = new() { new Field("CardCode", beRequest.CardCode), new Field("StateIdc", intState), new Field(LogicalOperators.And) };
                    IEnumerable<BES.UserRequest> lstTemp = bcRequest.List(lstFilter, "RequestDate");

                    if (lstTemp?.Count() > 0)
                    {
                        result = false;
                        message = "No se puede revertir esa solicitud, ya tiene una pendiente en la bandeja.";
                    }
                    else
                    {
                        beRequest.StatusType = StatusType.Update;
                        beRequest.StateIdc = intState;
                        beRequest.LogUser = UserCode;
                        beRequest.LogDate = logDate;
                        beRequest.ListUserRequestDetails = new List<BES.UserRequestDetail> {
                            new BES.UserRequestDetail { StatusType = StatusType.Insert, IdRequest = beRequest.Id, StateIdc = intState, LogUser = UserCode, LogDate = logDate }
                        };
                        bcRequest.Save(ref beRequest);
                    }
                }
                else
                {
                    message = "La Solicitud ya no existe.";
                    result = false;
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
                result = false;
            }
            var beResult = new { message, result };
            return Json(beResult);
        }

        #endregion

        #region Private Methods

        private static List<Models.UserRequest> GetRequest(long StateIdc, string Filter)
        {
            IEnumerable<BES.UserRequest> lstItems = new List<BES.UserRequest>();
            List<Models.UserRequest> lstResume = new();

            BCS.UserRequest bcRequest = new();
            BCA.Note bcNote = new();
            BCA.Client bcClient = new();
            BCS.BlackList bcBList = new();
            BCS.User bcUser = new();
            BCB.Classifier bcClassifier = new();

            List<BEB.Classifier> configs = bcClassifier.List((long)BEE.Classifiers.UsersConfig, "1") ?? new List<BEB.Classifier>();
            _ = int.TryParse(configs.FirstOrDefault(x => x.Name == "Tiempo")?.Value ?? "9", out int months);
            _ = decimal.TryParse(configs.FirstOrDefault(x => x.Name == "Monto")?.Value ?? "1000", out decimal amount);
            DateTime limitDate = DateTime.Today.AddMonths(-months);

            List<Field> filters = new() { new Field("StateIdc", StateIdc) };
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                Filter = Filter.Trim();
                filters.AddRange(new[] {
                    new Field("FullName", Filter, Operators.Likes),
                    new Field("ClientName", Filter, Operators.Likes),
                    new Field("Position", Filter, Operators.Likes),
                    new Field("EMail", Filter, Operators.Likes),
                    new Field("CardCode", Filter, Operators.Likes),
                    new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.Or), new Field(LogicalOperators.And)
                });
            }
            lstItems = bcRequest.List(filters, "1");
            List<string> clientCodes = lstItems.Select(x => x.CardCode).ToList();
            string strEmails = string.Join(",", lstItems.Select(x => $"'{x.EMail}'"));
            string strClientCodes = string.Join(",", clientCodes.Select(x => $"'{x}'"));
            filters = new List<Field> {
                new Field("DocDate", limitDate.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan),
                new Field("LOWER(ClientCode)", strClientCodes.ToLower(), Operators.In),
                new Field(LogicalOperators.And)
            };
            IEnumerable<BEA.Note> lstOrders = bcNote.List(filters, "1");
            IEnumerable<BEA.Client> lstMails = Enumerable.Empty<BEA.Client>(), lstNames = Enumerable.Empty<BEA.Client>(), clients = Enumerable.Empty<BEA.Client>();
            IEnumerable<BES.User> lstUsers = new List<BES.User>();
            if (clientCodes?.Count > 0)
            {
                lstMails = bcClient.ListValidEMails(clientCodes);
                lstNames = bcClient.ListValidNames(clientCodes);

                filters = new List<Field> { new Field("LOWER(CardCode)", strClientCodes.ToLower(), Operators.In) };
                clients = bcClient.List(filters, "1");

                filters = new List<Field> { new Field("CardCode", strClientCodes, Operators.In), new Field("EMail", strEmails, Operators.In), new Field(LogicalOperators.Or) };
                lstUsers = bcUser.List(filters, "1");
            }
            IEnumerable<BES.BlackList> lstBList = bcBList.List("1");

            lstResume = (from i in lstItems
                         join c in clients on i.CardCode.ToLower() equals c.CardCode.ToLower() into jClients
                         from lj in jClients.DefaultIfEmpty()
                         select new Models.UserRequest
                         {
                             Id = i.Id,
                             CardCode = i.CardCode,
                             FullName = i.FullName,
                             ClientName = i.ClientName,
                             SellerName = lj?.SellerName ?? "",
                             CreateDate = lj?.CreateDate ?? limitDate,
                             City = i.City,
                             EMail = i.EMail,
                             Phone = i.Phone,
                             StateIdc = i.StateIdc,
                             Comments = i.Comments,
                             Position = i.Position,
                             RequestDate = i.RequestDate,
                             IsNew = lj != null && lj.CreateDate >= limitDate,
                             SameClient = lstUsers.Any(x => x.EMail.Trim() == i.EMail.Trim() & x.CardCode.ToLower() == i.CardCode.ToLower()),
                             Enabled = lstUsers.FirstOrDefault(x => x.EMail.Trim() == i.EMail.Trim())?.Enabled ?? false,
                             IdUser = lstUsers.FirstOrDefault(x => x.EMail.Trim() == i.EMail.Trim())?.Id ?? 0,
                             ClientHasUsers = lstUsers.Any(x => x.CardCode.ToLower() == i.CardCode.ToLower()),
                             ClientHasEnabledUsers = lstUsers.Any(x => x.CardCode.ToLower() == i.CardCode.ToLower() & x.Enabled),
                             Amount = (from o in lstOrders where o.ClientCode.ToLower() == i.CardCode.ToLower() select o.Total).Sum(),
                             HasOrders = (from o in lstOrders where o.ClientCode.ToLower() == i.CardCode.ToLower() select o.Total).Sum() >= (lj?.CreateDate < limitDate ? amount : 0.0M),
                             ValidEMail = (from m in lstMails where m.CardCode.ToLower() == i.CardCode.ToLower() select m.EMail?.ToLower() ?? "").Contains(i.EMail?.ToLower() ?? ""),
                             ValidEMails = string.Join(", ", (from m in lstMails where m.CardCode.ToLower() == i.CardCode.ToLower() & !string.IsNullOrWhiteSpace(m.EMail) select m.EMail)),
                             ValidCardNames = string.Join(", ", (from n in lstNames where n.CardCode.ToLower() == i.CardCode.ToLower() & !string.IsNullOrWhiteSpace(n.CardFName) select n.CardFName)),
                             InBlackList = lstBList.Any(x => x.CardCode.ToLower() == i.CardCode.ToLower())
                         }).ToList();
            return lstResume;
        }

        private static string ExistingRequestMessage(string Code, string Name)
        {
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
            sb.AppendLine(@"				<td style=""height:130px"">");
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine(@"                    <p style='font-weight:bold; font-size: 1.5em; color: #F00;'>SOLICITUD RECHAZADA</p>");
            sb.AppendLine($@"				    <p>Estimado(a) Sr(a). <strong>{Name}</strong></p>");
            sb.AppendLine($@"                   <p>Su solicitud ha sido RECHAZADA debido a que ya existe una solicitud pendiente para su Empresa (<span style='font-weight:bold; font-size: 1.1em;'>{Code}</span>), espere a que la anterior solicitud sea atendida.</p>");
            sb.AppendLine(@"                    <br />");
            sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"		");
            sb.AppendLine(@"	</div>");

            return sb.ToString();
        }

        private static string SuccessfulRequestMessage(string ClientCode, string ClientName, string City, string Name, string Position, string EMail, string Telephone)
        {
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
            sb.AppendLine(@"				<td style=""height:130px"">");
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine(@"                    <p style='font-weight:bold; font-size: 1.5em; color: #0080C0;'>SOLICITUD RECIBIDA</p>");
            sb.AppendLine($@"				    <p>Estimado(a) Sr(a). <b>{Name} ( {ClientCode} )</b></p>");
            sb.AppendLine($@"                   <p>Su solicitud ha sido RECIBIDA, espere a que la misma sea atendida.</p>");
            sb.AppendLine(@"                    <br />");
            sb.AppendLine(@"                    <table style='border: 1px #0080C0 solid; width: 100%; border-spacing: 0; border-collapse: collapse; font-size: 12px;'>");
            sb.AppendLine(@"                        <thead><tr style='background-color: #0080C0; color: #FFF; font-size: 1.3em; font-weight: bold; text-align: center;'><td colspan='2' style='padding: 8px;'>DATOS ENVIADOS</td></tr></thead>");
            sb.AppendLine($@"                        <tr><td style='text-align: right; width: 200px;'>Nombre Completo:</td><td>{Name}</td></tr>");
            sb.AppendLine($@"                        <tr><td style='text-align: right;'>Ciudad:</td><td>{City}</td></tr>");
            sb.AppendLine($@"                        <tr><td style='text-align: right;'>Empresa:</td><td>{ClientName}</td></tr>");
            sb.AppendLine($@"                        <tr><td style='text-align: right;'>Cargo:</td><td>{Position}</td></tr>");
            sb.AppendLine($@"                        <tr><td style='text-align: right;'>Cod. Empresa en DMC:</td><td>{ClientCode}</td></tr>");
            sb.AppendLine($@"                        <tr><td style='text-align: right;'>E-Mail:</td><td>{EMail}</td></tr>");
            sb.AppendLine($@"                        <tr><td style='text-align: right;'>Tel&eacute;fono:</td><td>{Telephone}</td></tr>");
            sb.AppendLine($@"                        <tr><td style='text-align: right;'>Fecha de Solicitud:</td><td>{DateTime.Now:dd/MM/yyyy HH:mm}</td></tr>");
            sb.AppendLine(@"                    </table>");
            sb.AppendLine(@"                    <br />");
            sb.AppendLine(@"                    <p>La clave principal de acceso al portal ser&aacute; emitida &uacute;nicamente al Gerente General de cada empresa, quien tendr&aacute; los derechos de agregar m&aacute;s usuarios en el Portal.</p>");
            sb.AppendLine(@"                    <div style='background-color: #FF0; padding: 13px; text-align: center; border-radius: 8px; line-height: 12px;'>Este proceso podr&iacute;a demorar entre 24h a 72h laborables despu&eacute;s de la verificaci&oacute;n de los datos proporcionados, y la confirmaci&oacute;n de quien solicita el acceso es no solamente cliente activo de DMC, sino que es el Gerente General, Propietario o Representante Legal de la empresa que hace esta solicitud y que debe coincidir con nuestros records.</div>");
            sb.AppendLine(@"					<br /><p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"		");
            sb.AppendLine(@"	</div>");

            return sb.ToString();
        }

        private static string RejectMessageByCode(string Code, string Name)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(@"	<style> ");
            sb.AppendLine(@"		body { background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; } ");
            sb.AppendLine(@"		img { margin: 20px 15px; }");
            sb.AppendLine(@"		td { padding: 0 8px; line-height: 18px; }");
            sb.AppendLine(@"	</style>");
            sb.AppendLine(@"	<div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"" >");
            sb.AppendLine(@"		<table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse;"">");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td style=""height:130px"">");
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine(@"                    <p style='font-weight:bold; font-size: 1.5em; color: #F00;'>SOLICITUD RECHAZADA</p>");
            sb.AppendLine($@"				    <p>Estimado(a) Sr(a). <strong>{Name}</strong></p>");
            sb.AppendLine($@"                   <p>Su solicitud ha sido RECHAZADA debido a que el c&oacute;digo de Empresa suministrado (<span style='font-weight:bold; font-size: 1.1em;'>{Code}</span>) no pertenece a un cliente registrado en DMC.</p>");
            sb.AppendLine(@"                    <br />");
            sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"		");
            sb.AppendLine(@"	</div>");

            return sb.ToString();
        }

        private static string RejectMessage(string Code, string Name, bool NameMismatch, bool WithoutOrders, bool InvalidEMail, bool IsNew)
        {
            BCB.Classifier bcClassifier = new();
            List<BEB.Classifier> configs = bcClassifier.List((long)BEE.Classifiers.UsersConfig, "1") ?? new List<BEB.Classifier>();
            _ = int.TryParse(configs.FirstOrDefault(x => x.Name == "Tiempo")?.Value ?? "9", out int months);
            _ = decimal.TryParse(configs.FirstOrDefault(x => x.Name == "Monto")?.Value ?? "1000", out decimal amount);

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
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine(@"                    <br /><p style='font-weight:bold; font-size: 1.5em; color: #F00;'>SOLICITUD RECHAZADA</p>");
            sb.AppendLine($@"				    <p>Estimado(a) Sr(a). <strong>{Name}</strong></p>");
            if (NameMismatch | WithoutOrders | InvalidEMail)
            {
                sb.AppendLine(@"<p>Su solicitud ha sido RECHAZADA por la(s) siguiente(s) razones:</p>");
                sb.AppendLine(@"<ul>");
                if (NameMismatch)
                {
                    sb.AppendLine($@"<li>Su nombre no coincide con los nombres autorizados para el c&oacute;digo de cliente suministrado (<span style='font-weight:bold; font-size: 1.1em;'>{Code}</span>)</li>");
                }
                if (InvalidEMail)
                {
                    sb.AppendLine($@"<li>El correo sumistrado no coincide con ninguno de los correos que tenemos registrados para el c&oacute;digo de cliente (<span style='font-weight:bold; font-size: 1.1em;'>{Code}</span>)</li>");
                }
                if (WithoutOrders)
                {
                    if (IsNew)
                    {
                        sb.AppendLine(@"<li>No ha realizado ninguna compra todavía</li>");
                    }
                    else
                    {
                        sb.AppendLine($@"<li>No cumple con el monto mínimo de compras ( {amount:N2} ) en los últimos {months} meses.</li>");
                    }
                }
                sb.AppendLine(@"</ul>");
                sb.AppendLine(@"<p>Si desea realizar alguna cotización por favor comun&iacute;quese con su ejecutivo de cuentas.</p>");
            }
            else
            {
                sb.AppendLine(@"<p>Su solicitud ha sido RECHAZADA, comuníquese con su ejecutivo de cuentas si desea realizar alguna cotización.</p>");
            }
            sb.AppendLine(@"<br />");
            sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"		");
            sb.AppendLine(@"	</div>");

            return sb.ToString();
        }

        private static string NewUserMailBody(string Name, string Login, string Password)
        {
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
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine($@"				    <br/><p>Estimado Cliente: <strong>{Name}</strong></p>");
            sb.AppendLine(@"                    <p>Adjunto encontrar&aacute; su nombre de usuario y contrase&ntilde;a para el Portal, la cual deber&aacute; inmediatamente de cambiar por una nueva. Le recomendamos cambiar su contrase&ntilde;a peri&oacute;dicamente para su seguridad.</p>");
            sb.AppendLine(@"                    <p>Sus nuevos datos de acceso al portal son:<br />");
            sb.AppendLine($@"                   <table><tr><td>Nombre de usuario :</td><td><strong>{Login}</strong> </td></tr>");
            sb.AppendLine($@"                   <tr><td>Contrase&ntilde;a :</td><td><strong>{Password}</strong></td></tr></table> </p>");
            sb.AppendLine(@"                    <p>La contrase&ntilde;a es sensitiva ( distingue may&uacute;sculas de min&uacute;sculas ) y usa caracteres especiales.<br /> Para ingresar al portal vaya a la siguiente direcci&oacute;n http://portal.dmc.bo.</p>");
            sb.AppendLine(@"                    <p>Le recordamos que el uso de este Portal, así como el uso de aquellos usuarios que usted crea en su cuenta, es de su entera responsabilidad. DMC se reserva el derecho de cancelar o inhabilitar su cuenta en cualquier momento, sin ninguna responsabilidad hacia su empresa.</p>");
            sb.AppendLine(@"                    <p>Tampoco nos hacemos responsables si el servicio cumple o no sus expectativas, y como este es un servicio a través de Internet, es posible que en ciertas horas el servicio no est&eacute; disponible o sea muy lento, ya que DMC no controla la velocidad de acceso a Internet.</p>");
            sb.AppendLine(@"                    <p>Adicionalmente, la informaci&oacute;n que usted reciba a trav&eacute;s del portal es &uacute;nicamente informativa, y no nos hacemos responsables de errores u omisiones que aparezcan en el portal, y, aunque consideramos que la informaci&oacute;n desplegada en el Portal es correcta,  en caso de diferencias u errores, la informaci&oacute;n v&aacute;lida es la que se produzca en nuestros sistemas de gesti&oacute;n SAP.</p>");
            sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"		");
            sb.AppendLine(@"	</div>");

            return sb.ToString();
        }

        private static string MailErrorMessage(string Name, string EMail, string Error)
        {
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
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine($@"				    <br/><p>Estimado(a) Sr(a). <strong>{Name}</strong></p>");
            sb.AppendLine(@"                    <p>Gracias por solicitar su acceso personal al Portal de DMC.</p>");
            sb.AppendLine($@"                    <p>Se ha intentado enviar la información a su correo ({EMail}), lamentablemente ha ocurrido el siguiente error : <br />");
            sb.AppendLine($@"                   {Error}</p>");
            sb.AppendLine(@"                    <p>Por favor revice el estado de su correo e intente llenar nuevamente el formulario.</p>");
            sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"		");
            sb.AppendLine(@"	</div>");

            return sb.ToString();
        }

        private string SendEmailUser(string Subject, string Body, string Email, bool Copy)
        {
            string strMessage = "";
            try
            {
                var settingsSection = config.GetSection("MailSettings");
                var settings = settingsSection.Get<MailSettings>();
                var account = settings.Accounts.Find(x => x.Id == "soporte-santacruz");
                List<MailAddress> lstTo = new(), lstCopies = new() { new MailAddress(account.EMail, account.Name) };
                if (Copy)
                {
                    lstTo.Add(new MailAddress(Email));
                    _ = SendMailAsync(Subject, Body, lstTo, lstCopies);
                }
                else
                {
                    _ = SendMailAsync(Subject, Body, lstTo);
                }
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            return strMessage;
        }

        #endregion
    }
}