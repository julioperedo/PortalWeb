using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Interfaces;
using Portal.Membership;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEE = BEntities.Enums;
using BES = BEntities.Security;

namespace Portal.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {

        #region Global Variables

        private CustomMembership _membership;
        private readonly IEmailSender _emailSender;
        private Random random;

        #endregion

        #region Constructors

        public AccountController(ICustomMembership membership, IEmailSender emailSender)
        {
            _membership = (CustomMembership)membership;
            _emailSender = emailSender;
            random = new Random();
        }

        #endregion

        #region GETs

        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl ?? _membership.Options.DefaultPathAfterLogin;
            return View("Login");
        }

        [AllowAnonymous]
        [HttpGet("loginpostasync")]
        public async Task<IActionResult> LoginPostAsync(LoginModel user)
        {
            string message = "";
            LoginResult result;
            if (string.IsNullOrWhiteSpace(user.CardCode) & !user.IdProfile.HasValue)
            {
                result = await _membership.LoginAsync(user.Email, user.Password);
            }
            else
            {
                result = await _membership.LoginAsync2(user.Email, user.Password, user.CardCode, user.IdProfile);
            }
            if (result.Failed)
            {
                message = result.FailMessage;
            }
            else
            {
                if (result.LoggedIn)
                {
                    return Json(new { Message = message, Logued = true });
                }
                else
                {
                    bool hasProfiles = result.IsMultiProfile;
                    var clients = result.AssignedClients.Select(x => new { x.CardCode, CardName = $"{x.CardCode} - {x.CardName}" });
                    List<BES.Profile> profiles = clients.Any() ? new List<BES.Profile>() : result.AssignedProfiles;
                    return Json(new { Message = message, Logued = false, Clients = clients, Profiles = profiles, HasProfiles = hasProfiles });
                }
            }
            return Json(new { Message = message });
        }

        [AllowAnonymous]
        [HttpGet("requestenableuser")]
        public async Task<IActionResult> RequestEnableUser(string EMail)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new BCS.User();
                var user = await bcUser.SearchAsync(EMail);
                if (!user.Enabled)
                {
                    if (user.LogUser == 0 || user.LogUser == user.Id)
                    {
                        if (!string.IsNullOrEmpty(user.CardCode))
                        {
                            BCA.Note bcNote = new BCA.Note();
                            var lastNote = bcNote.SearchLast(user.CardCode);
                            if (lastNote != null)
                            {
                                if (lastNote.DocDate.AddMonths(9) > DateTime.Today)
                                {
                                    user.Enabled = true;
                                    user.LogDate = DateTime.Now;
                                    user.StatusType = BEntities.StatusType.Update;
                                    bcUser.Save(ref user);
                                }
                                else
                                {
                                    message = $"No se puede habilitar el usuario {EMail} porque su última compra fue hace más de 9 meses.";
                                }
                            }
                            else
                            {
                                message = $"No se puede habilitar el usuario {EMail} porque el cliente {user.CardCode} no ha realizado ninguna compra.";
                            }
                        }
                        else
                        {
                            message = $"El usuario {EMail} no está relacionado con ningún cliente v&aacute;lido de DMC.";
                        }
                    }
                    else
                    {
                        message = $"El usuario {EMail} no ha sido deshabilitado por un servicio automático de DMC, debe ponerse en contacto con el encargado de su organización.";
                    }
                }
                else
                {
                    message = $"El usuario {EMail} no está deshabilitado";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [AllowAnonymous]
        [HttpGet("GetAssignedProfiles")]
        public ActionResult GetAssignedProfiles(string email, string password, string cardCode)
        {
            List<BES.Profile> lstProfiles = new List<BES.Profile>();
            string strMessage = "";
            try
            {
                BCS.User bcUser = new BCS.User();
                BES.User beUser = bcUser.Search(email, Crypt.Encrypt(password), BES.relUser.UserProfiles, BES.relUserProfile.Profile);
                string strCardCode = !string.IsNullOrWhiteSpace(cardCode) ? cardCode : beUser.CardCode;
                bool boLocalSelected = strCardCode == "CDMC-002", boLocal = beUser.CardCode == "CDMC-002";
                lstProfiles = (from i in beUser.ListUserProfiles where i.Profile.isExternalCapable == !boLocalSelected select i.Profile).ToList();
                if (boLocal == boLocalSelected)
                {
                    if (!(from i in lstProfiles select i.Id).Contains(beUser.IdProfile))
                    {
                        BCS.Profile bcProfile = new BCS.Profile();
                        BES.Profile beProfile = bcProfile.Search(beUser.IdProfile);
                        lstProfiles.Add(beProfile);
                    }
                }
                if (lstProfiles.Count == 0)
                {
                    //Se le agrega un perfil de sólo ventas
                    BCS.Profile bcProfile = new BCS.Profile();
                    BES.Profile beProfile;
                    if (boLocalSelected)
                    {
                        beProfile = bcProfile.Search(5);
                    }
                    else
                    {
                        beProfile = bcProfile.Search(3);
                    }
                    lstProfiles.Add(beProfile);
                }
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    strMessage += Environment.NewLine + ex.Message;
                }
            }
            var items = (from i in lstProfiles orderby i.isExternalCapable, i.Name select new { i.Id, i.Name }).ToList();
            return Json(new { Message = strMessage, Items = items });
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await _membership.LogoutAsync();
            HttpContext.Session.Clear();
            if (_membership.Options.DefaultPathAfterLogout != null)
            {
                return Redirect(_membership.Options.DefaultPathAfterLogout);
            }
            else
            {
                return RedirectToAction("login");
            }
        }

        /*[HttpGet("protected")]
        [Authorize]
        public async Task<IActionResult> Protected()
        {
            var session = await _membership.GetSessionDetailsAsync(HttpContext.User);
            return View("Protected", new { UserId = session.User.Id, UserName = session.User.Username, SessionId = session.Id, SessionCreated = session.CreationTime });
        }*/

        [HttpGet("ForgotPasswordAsync")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPasswordAsync(string EMail)
        {
            bool failed = false;
            string message;
            try
            {
                BCS.User bcUser = new();
                var user = await bcUser.SearchAsync(EMail);
                var code = GetRandomHexNumber(8);

                if (user?.Id > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"	<style> ");
                    sb.AppendLine(@"		body { background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; } ");
                    sb.AppendLine(@"		img { margin: 20px 15px; }");
                    sb.AppendLine(@"		td { padding: 0 8px; line-height: 18px; }");
                    sb.AppendLine(@"        .code { font-size: 1.5em; } ");
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
                    sb.AppendLine($@"				    <p>Hola <strong>{user.Name}</strong><p/>");
                    sb.AppendLine($@"                    <p>Recibimos una solicitud para reestablecer la contrase&ntilde;a de tu cuenta del portal de DMC. Si fuiste t&uacute;, utiliza el siguiente c&oacute;digo:<br /><br /><span class=""code"">{code}</span></p>");
                    sb.AppendLine(@"                    <p>Si no quieres restablecer tu contrase&ntilde;a, ignora este mensaje y continua ingresando con tu contrase&ntilde;a actual.</p>");
                    sb.AppendLine(@"                    <p>No compartas este c&oacute;digo ya que con el podr&iacute;an tener acceso a tu cuenta del portal.</p>");
                    sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
                    sb.AppendLine(@"				</td>");
                    sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
                    sb.AppendLine(@"			</tr>");
                    sb.AppendLine(@"		</table>");
                    sb.AppendLine(@"		");
                    sb.AppendLine(@"	</div>");

                    await _emailSender.SendEmail("Restablecer contraseña", sb.ToString(), new Dictionary<string, string> { { user.EMail, user.Name } });
                    message = code;
                }
                else
                {
                    failed = true;
                    message = "Ese correo electrónico no corresponde a ningún usuario registrado.";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
                failed = true;
            }
            return Json(new { message, failed });
        }

        #endregion

        #region POSTs

        [AllowAnonymous]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string EMail, string NewPassword)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new BCS.User();
                var user = await bcUser.SearchAsync(EMail);
                user.Password = Crypt.Encrypt(NewPassword);
                user.LogDate = DateTime.Now;
                user.LogUser = user.Id;
                user.StatusType = BEntities.StatusType.Update;
                bcUser.Save(ref user);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [AllowAnonymous]
        [HttpPost("SendUserRequest")]
        public async Task<IActionResult> SendUserRequest(string ClientCode, string ClientName, string Name, string EMail, string Position)
        {
            string message = "", title = "";
            bool succeded = true, recover = false;
            try
            {
                BCA.Client bcClient = new();
                BEA.Client beClient = bcClient.Search(ClientCode);
                if (!string.IsNullOrWhiteSpace(beClient?.CardCode))
                {
                    BCS.User bcUser = new();
                    var lstUsers = bcUser.List(ClientCode, "1");
                    if (!lstUsers.Any(x => !string.IsNullOrWhiteSpace(x.EMail) & x.Enabled))
                    {
                        List<Field> lstFilter = new()
                        {
                            new Field { Name = "CardCode", Value = ClientCode },
                            new Field { Name = "StateIdc", Value = (long)BEE.States.UserRequest.Created },
                            new Field { LogicalOperator = LogicalOperators.And }
                        };
                        BCS.UserRequest bcRequest = new();
                        var lstRequests = bcRequest.List(lstFilter, "RequestDate");
                        if (lstRequests?.Count() > 0)
                        {
                            succeded = false;
                            title = "Solicitud Pendiente";
                            message = $"No se puede procesar su solicitud porque existe una solicitud en espera de aprobaci&oacute;n para este cliente (<b>{ClientCode}</b>).";
                        }
                        else
                        {
                            string mailContent = $@"<style> 
                                                       body {{ background - color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; }} 
                                                       img {{ margin: 20px 15px; }}
                                                       td {{ padding: 0 8px; line-height: 18px; }}
                                                    </style>
                                                    <div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"" >
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
                                                                <p style='font-weight:bold; font-size: 1.5em; color: #0080C0;'>SOLICITUD RECIBIDA</p>
                                                                <p>Estimado(a) Sr(a). <b>{Name} ( {ClientCode} )</b><p/>
                                                                <p>Su solicitud ha sido RECIBIDA, espere a que la misma sea atendida.</p><br />
                                                                <table style='border: 1px #0080C0 solid; width: 100%; border-spacing: 0; border-collapse: collapse; font-size: 12px;'>
                                                                    <thead><tr style='background-color: #0080C0; color: #FFF; font-size: 1.3em; font-weight: bold; text-align: center;'><td colspan='2' style='padding: 8px;'>DATOS ENVIADOS</td></tr></thead>
                                                                    <tr><td style='text-align: right; width: 200px;'>Nombre Completo:</td><td>{Name}</td></tr>
                                                                    <tr><td style='text-align: right;'>Empresa:</td><td>{ClientName}</td></tr>
                                                                    <tr><td style='text-align: right;'>Cargo:</td><td>{Position}</td></tr>
                                                                    <tr><td style='text-align: right;'>Cod. Empresa en DMC:</td><td>{ClientCode}</td></tr>
                                                                    <tr><td style='text-align: right;'>E-Mail:</td><td>{EMail}</td></tr>
                                                                    <tr><td style='text-align: right;'>Fecha de Solicitud:</td><td>{DateTime.Now:dd/MM/yyyy HH:mm}</td></tr>
                                                                </table><br />
                                                                <p>La clave principal de acceso al portal ser&aacute; emitida &uacute;nicamente al Gerente General de cada empresa, quien tendr&aacute; los derechos de agregar m&aacute;s usuarios en el Portal.</p>
                                                                <div style='background-color: #FF0; padding: 13px; text-align: center; border-radius: 8px; line-height: 12px;'>Este proceso podr&iacute;a demorar entre 24h a 72h laborables despu&eacute;s de la verificaci&oacute;n de los datos proporcionados, y la confirmaci&oacute;n de quien solicita el acceso es no solamente cliente activo de DMC, sino que es el Gerente General, Propietario o Representante Legal de la empresa que hace esta solicitud y que debe coincidir con nuestros records.</div>
                                                                <br /><p>Atentamente<br />El equipo de DMC</p><br />
                                                             </td>
                                                             <td style=""width: 20px;"">&nbsp;</td>
                                                          </tr>
                                                       </table>            
                                                    </div>";

                            Dictionary<string, string> tos = new Dictionary<string, string> { { EMail, Name } }, copies = new Dictionary<string, string> { { "julio.peredo@dmc.bo", "Julio C. Peredo" } };
                            await _emailSender.SendEmail("Solicitud Recibida", mailContent, tos, copies);

                            BES.UserRequest beRequest = new BES.UserRequest
                            {
                                CardCode = ClientCode,
                                ClientName = ClientName?.Trim() ?? "",
                                EMail = EMail.Trim(),
                                FullName = Name,
                                City = "",
                                Phone = "",
                                Position = Position.Trim(),
                                RequestDate = DateTime.Now,
                                StateIdc = (long)BEE.States.UserRequest.Created,
                                LogUser = 0,
                                LogDate = DateTime.Now,
                                StatusType = BEntities.StatusType.Insert,
                                ListUserRequestDetails = new List<BES.UserRequestDetail> { new BES.UserRequestDetail { StatusType = BEntities.StatusType.Insert, StateIdc = (long)BEE.States.UserRequest.Created, LogUser = 0, LogDate = DateTime.Now } }
                            };
                            bcRequest.Save(ref beRequest);
                        }
                    }
                    else
                    {
                        succeded = false;
                        recover = true;
                        title = "Usuarios encontrados";
                        message = "No se puede procesar su solicitud porque ya tiene usuarios habilitados, debe contactarse con el administrador de su empresa o solicitar un c&oacute;digo de recuperaci&oacute;n.";
                    }
                }
                else
                {
                    title = "Cliente NO encontrado";
                    message = $"El c&oacute;digo suministrado ({ClientCode}) no pertenece a ning&uacute;n cliente registrado";
                    succeded = false;
                }
            }
            catch (Exception ex)
            {
                title = "Algo salió mal";
                message = GetError(ex);
                succeded = false;
            }
            return Json(new { succeded, recover, message, title });
        }

        #endregion

        #region Private Methods

        private string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        private string GetError(Exception ex)
        {
            string strMessage = "";
            string strType = ex.GetType().FullName;
            switch (strType)
            {
                //    Case GetType(BComponents.BCException)
                //        If CType(ex, BComponents.BCException).ErrorCollection.Count > 0 Then
                //            strMessage = String.Join(Environment.NewLine, CType(ex, BComponents.BCException).ErrorCollection)
                //        Else
                //            strMessage = ex.Message
                //        End If
                case "System.Data.SqlClient.SqlException":
                    System.Data.SqlClient.SqlException ex1 = (System.Data.SqlClient.SqlException)ex;
                    if (ex1.Errors.Count > 0)
                    {
                        switch (ex1.Errors[0].Number)
                        {
                            //case 547:
                            //    strMessage = "El registro no puede ser eliminado porque tiene dependencias.";
                            //    break;
                            case 2601:
                            case 2627:
                                strMessage = "Violación de la llave primaria o la llave no ha sido especificada apropiadamente.";
                                break;
                            default:
                                //strMessage = string.Join(Environment.NewLine, (from e in ex1.Errors select e.Message).ToArray);
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
            return strMessage;
        }

        #endregion
    }
}