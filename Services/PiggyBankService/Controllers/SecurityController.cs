using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PiggyBankService.Misc;
using PiggyBankService.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using BCB = BComponents.Base;
using BCI = BComponents.PiggyBank;
using BCS = BComponents.Security;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEI = BEntities.PiggyBank;
using BES = BEntities.Security;

namespace PiggyBankService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {

        #region Global Variables

        private readonly ILogger<SecurityController> _logger;
        private readonly IConfiguration _config;

        #endregion

        #region Constructors
        public SecurityController(ILogger<SecurityController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        #endregion

        #region GETs

        [HttpGet]
        public IActionResult ValidateEmail(string Email)
        {
            string message = "";
            BEI.User user = new();
            try
            {
                BCI.User bcUser = new();
                List<Field> filters = new() { new Field("LOWER(EMail)", Email.ToLower()), new Field("Enabled", 1), new Field(LogicalOperators.And) };
                var results = bcUser.List(filters, "1");
                if (results.Any())
                {
                    user = results.First();
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex, "Error al validar el usuario (E-Mail)");
            }
            return Ok(new { message, user });
        }

        [HttpGet]
        public async Task<IActionResult> UserLoginAsync(string Email, string Password)
        {
            string message = "";
            try
            {
                BCI.User bcUser = new();
                BEI.User? u = await bcUser.SearchAsync(Email, Encryption.Encrypt(Password));
                if (u != null)
                {
                    var user = new { u.Id, u.Name, u.StoreName, u.EMail, u.City, u.Address, u.Phone, u.Enabled };
                    return Ok(new { message, user });
                }
                else
                {
                    return Ok(new { message, user = u });
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex, "Error al validar si el usuario es válido.");
            }
            return Ok(new { message });
        }

        [HttpGet]
        public IActionResult IsUserValid(long Id)
        {
            string message = "";
            try
            {
                BCI.User bcUser = new();
                BEI.User beUser = bcUser.Search(Id);
                bool valid = beUser?.Enabled ?? false;
                return Ok(new { message, valid });
            }
            catch (Exception ex)
            {
                message = GetError(ex, "Error al validar si el usuario es válido.");
            }
            return Ok(new { message });
        }

        [HttpGet]
        public async Task<IActionResult> SendPassword(string Email)
        {
            string message = "";
            bool valid = false, enabled = false;
            try
            {
                BCI.User bcUser = new();
                BEI.User user = await bcUser.SearchAsync(Email);

                valid = user?.Id > 0;
                enabled = user?.Enabled ?? false;
                if (valid & enabled)
                {
                    string subject = "Datos de Acceso a la Alcancía Ganadora";
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
                    sb.AppendLine($@"				    <p>Estimado Cliente: <strong>{user?.Name} ( {user?.StoreName} )</strong><p/>");
                    sb.AppendLine(@"                    <p>Sus datos de acceso a la ALcancía Ganadora son:<br />");
                    sb.AppendLine($@"                   <table><tr><td>Nombre de usuario :</td><td><strong>{user?.EMail}</strong> </td></tr>");
                    sb.AppendLine($@"                   <tr><td>Contrase&ntilde;a :</td><td><strong>{Encryption.Decrypt(user?.Password ?? "")}</strong></td></tr></table> </p>");
                    sb.AppendLine(@"                    <p>La contrase&ntilde;a es sensitiva ( distingue may&uacute;sculas de min&uacute;sculas ).</p>");
                    sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
                    sb.AppendLine(@"				</td>");
                    sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
                    sb.AppendLine(@"			</tr>");
                    sb.AppendLine(@"		</table>");
                    sb.AppendLine(@"	</div>");

                    List<MailAddress> lstTo = new() { new MailAddress(user?.EMail ?? "", user?.Name) }, lstCopies = new(), lstBlindCopies = new();
                    await SendMailAsync(subject, sb.ToString(), lstTo, lstCopies, lstBlindCopies);
                }
            }
            catch (Exception ex)
            {
                message += GetError(ex, "Error al enviar contraseña al usuario.");
            }
            return Ok(new { message, valid, enabled });
        }

        [HttpGet]
        public IActionResult GetBaseSettings() {
			string message = "";
            try
            {
                BCB.Classifier bc = new();
                var results = bc.List((long)BEE.Classifiers.PiggyBank, "1");
                string phoneNumber = results.FirstOrDefault(x => x.Name == "PhoneNumber")?.Value ?? "59148700997";
				return Ok(new { message, phoneNumber });
			}
			catch (Exception ex)
			{
				message = GetError(ex, "Error al obtener las configuraciones");
			}
			return Ok(new { message });
		}

		#endregion

		#region POSTs

		[HttpPost]
        public IActionResult RegisterUser(string Name, string StoreName, string EMail, string Password, string City, string Address, string Phone)
        {
            long id = 0;
            string message = "";
            bool alreadyExist = false;
            try
            {
                BCI.User bcUser = new();
                List<Field> filters = new() { new Field("LOWER(EMail)", EMail.ToLower()) };
                var existingUsers = bcUser.List(filters, "1");
                alreadyExist = existingUsers.Any();
                if (!alreadyExist)
                {
                    BEI.User user = new(Name, StoreName, EMail, Encryption.Encrypt(Password), City, Address, Phone, true);
                    bcUser.Save(ref user);
                    id = user.Id;
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex, "Error al registrar el usuario");
            }
            return Ok(new { message, id, alreadyExist });
        }

        [HttpPost]
        public async Task<IActionResult> DisableUserAsync(long Id)
        {
            string message = "";
            bool result = true;
            try
            {
                BCI.User bcUser = new();
                await bcUser.Disable(Id);
            }
            catch (Exception ex)
            {
                message = GetError(ex, "Error al deshabilitar el usuario");
                result = false;
            }
            return Ok(new { message, result });
        }

        [HttpPost]
        public async Task<IActionResult> SaveTokenAsync(long UserId, string Token)
        {
            string message = "";
            try
            {
                BCI.UserToken bcToken = new();
                BEI.UserToken beToken = await bcToken.SearchAsync(UserId);
                beToken ??= new BEI.UserToken { IdUser = UserId, LogDate = DateTime.Now, LogUser = 0, Id = 0 };
                beToken.Token = Token;
                beToken.StatusType = beToken.Id == 0 ? StatusType.Insert : StatusType.Update;
                await bcToken.SaveAsync(beToken);
            }
            catch (Exception ex)
            {
                message = GetError(ex, "Error al actualizar el Token");
            }
            return Ok(new { message });
        }

        #endregion

        #region Private Methods

        private string GetError(Exception ex, string Title)
        {
            _logger.LogError(ex, Title);
            string message = ex.Message;
            var e1 = ex.InnerException;
            while (e1 != null)
            {
                message += Environment.NewLine + e1.Message;
                e1 = e1.InnerException;
            }
            return message;
        }

        private async Task SendMailAsync(string Subject, string MessageBody, List<MailAddress>? Destinataries = null, List<MailAddress>? Copies = null, List<MailAddress>? BlindCopies = null, List<Attachment>? Files = null)
        {
            MailSettings settings = _config.GetSection("MailSettings").Get<MailSettings>();
            MailAccount account = settings.Accounts.First();

            MailMessage message = new();
            MailAddress fromAddress = new(account.Email, account.Name);
            message.From = fromAddress;
            if (Destinataries?.Count > 0) Destinataries.ForEach(x => message.To.Add(x));
            if (Copies?.Count > 0) Copies.ForEach(x => message.CC.Add(x));
            if (BlindCopies?.Count > 0) BlindCopies.ForEach(x => message.Bcc.Add(x));
            if (Files?.Count > 0) Files.ForEach(x => message.Attachments.Add(x));
            message.Subject = Subject;
            message.IsBodyHtml = true;
            message.Body = MessageBody;

            SmtpClient smtpClient = new(settings.Server, settings.Port) { Credentials = new NetworkCredential(account.User, account.Password), DeliveryMethod = SmtpDeliveryMethod.Network };
            await smtpClient.SendMailAsync(message);
            message.Dispose();
            smtpClient.Dispose();
        }

        #endregion

    }
}
