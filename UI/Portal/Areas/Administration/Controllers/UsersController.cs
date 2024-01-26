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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using Portal.Membership;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCS = BComponents.Security;
using BCV = BComponents.Visits;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BES = BEntities.Security;
using BEV = BEntities.Visits;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class UsersController : BaseController
    {
        #region Constructores

        public UsersController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewData["Permission"] = GetPermission("Usuarios");
                if (CardCode == HomeCardCode)
                {
                    return View();
                }
                else
                {
                    ViewData["CardCode"] = CardCode;
                    return View("IndexClient");
                }
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        // GET: Administration/User/Edit/5
        public IActionResult Edit(long id = -1)
        {
            ViewData["BlackList"] = "N";
            if (id == -1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                BES.User beUser = new();
                if (id == 0)
                {
                    beUser = new BES.User { AccountHolder = false, CardCode = "" };
                }
                else
                {
                    BCS.User bcUser = new();
                    beUser = bcUser.Search(id);
                    beUser.Password = Crypt.Decrypt(beUser.Password);
                    //BCA.Client bcClient = new BCA.Client();
                    //BEA.Client beClient = bcClient.Search(beUser.CardCode);
                    try
                    {
                        bool isValidForEnabling = true;
                        if (beUser.CardCode != HomeCardCode)
                        {
                            BCB.Classifier bcClassifier = new();
                            var classifiers = bcClassifier.List((long)BEE.Classifiers.UsersConfig, "1");
                            int months = int.Parse(classifiers.FirstOrDefault(x => x.Name == "Tiempo")?.Value ?? "9");
                            decimal amount = decimal.Parse(classifiers.FirstOrDefault(x => x.Name == "Monto")?.Value ?? "1000");

                            BCA.Client bcClient = new();
                            var client = bcClient.SearchAmount(beUser.CardCode, DateTime.Today.AddMonths(-months));

                            isValidForEnabling = client?.Balance >= amount;
                        }
                        beUser.ValidForEnabling = isValidForEnabling ? "Y" : "N";
                    }
                    catch (Exception) { }
                }
                BCS.BlackList bcBlack = new();
                List<Field> lstFilter = new() { new Field("CardCode", beUser.CardCode) };
                IEnumerable<BES.BlackList> lstBlack = bcBlack.List(lstFilter, "1");
                if (lstBlack?.Count() > 0)
                {
                    ViewData["BlackList"] = "Y";
                }

                ViewData["SeePasswords"] = GetPermission("SeePasswords") > 0 ? "Y" : "N";

                if (CardCode != HomeCardCode)
                {
                    beUser.CardCode = CardCode;
                    return PartialView("EditClient", beUser);
                }
                else
                {
                    return PartialView(beUser);
                }
            }
        }

        public IActionResult Filter(Models.UserFilters Filters)
        {
            var message = "";
            IEnumerable<BES.User> lstItems = Enumerable.Empty<BES.User>();
            try
            {
                lstItems = GetItems(Filters.CardCode, Filters.ProfileCode, Filters.Name);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            var items = lstItems.Select(u => new { u.Id, u.Name, u.ClientName, u.ProfileName, u.Login, u.EMail, u.Commentaries, u.Enabled, u.CardCode, u.SessionCount });
            return Json(new { message, items });
        }

        public IActionResult IsBlackList(string CardCode)
        {
            string isBlackListed = "N", ValidForEnabling = "N";
            try
            {
                BCS.BlackList bcBlack = new();
                List<Field> lstFilter = new() { new Field("CardCode", CardCode) };
                IEnumerable<BES.BlackList> lstBlack = bcBlack.List(lstFilter, "1");
                isBlackListed = lstBlack?.Count() > 0 ? "Y" : "N";

                BCA.Note bcNote = new();
                var last = bcNote.SearchLast(CardCode);
                BCB.Classifier bcClassifier = new();
                var classifiers = bcClassifier.List((long)BEE.Classifiers.UsersConfig, "1");
                var config = classifiers.FirstOrDefault(x => x.Name == "Tiempo");
                int months = int.Parse(config?.Value ?? "9");
                ValidForEnabling = last?.DocDate.AddMonths(months) >= DateTime.Today ? "Y" : "N";
            }
            catch (Exception) { }
            return Json(new { isBlackListed, ValidForEnabling });
        }

        public IActionResult AssignedClients(long Id)
        {
            string message = "";
            try
            {
                BCS.UserClient bcClient = new();
                List<Field> filters = new() { new Field("IdUser", Id) };
                IEnumerable<BES.UserClient> lstClients = bcClient.List(filters, "CardCode");
                BCS.UserProfile bcProfile = new();
                IEnumerable<BES.UserProfile> lstProfiles = bcProfile.List(filters, "1");
                return Json(new { message, clients = lstClients.Select(x => x.CardCode), profiles = lstProfiles.Select(x => x.IdProfile) });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GerPermissions(long Id)
        {
            string message = "";
            try
            {
                BCS.UserActivity bcActivity = new();
                IEnumerable<BES.UserActivity> activities = bcActivity.List(Id, "1");
                var items = from a in activities
                            group a by a.ModuleName into g
                            select new { Module = g.Key, items = g.Select(x => new { x.Id, x.IdActivity, Name = x.ActivityName, x.Permission }) };
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

        [HttpPost()]
        public IActionResult Edit(BES.User User, Models.UserFilters Filters)
        {
            string message = "", cardCode = "";
            List<BES.User> items = new();
            try
            {
                BCS.User bcUser = new();
                List<Field> lstFilter = new() { new Field("Id", User.Id, Operators.Different) };
                if (!string.IsNullOrWhiteSpace(User.EMail))
                {
                    lstFilter.AddRange(new[] { new Field("EMail", User.EMail), new Field(LogicalOperators.And) });
                }
                IEnumerable<BES.User> users = bcUser.List(lstFilter, "1");
                if (users == null || !users.Any())
                {
                    if (IsEMailBlacklisted(User.EMail))
                    {
                        message += "El correo está en lista negra, para mayor información comuníquese con su ejecutivo de ventas.";
                    }
                    else
                    {
                        User.Login = User.EMail;
                        User.LogDate = DateTime.Now;
                        User.LogUser = UserCode;
                        User.StatusType = User.Id > 0 ? StatusType.Update : StatusType.Insert;
                        User.Password = Crypt.Encrypt(User.Password);
                        bcUser.Save(ref User);

                        items = GetItems(Filters.CardCode, Filters.ProfileCode, Filters.Name).ToList();
                        if (!items.Any(x => x.Id == User.Id)) items.Insert(0, User);

                        cardCode = User.CardCode;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(User.EMail))
                    {
                        if (users.Any(x => User.EMail.Trim().ToLower() == User.EMail.Trim().ToLower())) message += "Ya existe un usuario registrado con ese Correo Electrónico";
                    }
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items, cardCode });
        }

        // POST: Administration/User/Delete/5
        [HttpPost()]
        public IActionResult Delete(long Id, Models.UserFilters Filters)
        {
            string message = "";
            IEnumerable<BES.User> items = Enumerable.Empty<BES.User>();
            try
            {
                BES.User beUser = new() { Id = Id, StatusType = StatusType.Delete, LogDate = DateTime.Now };
                BCS.User bcUser = new();
                bcUser.Save(ref beUser);
                items = GetItems(Filters.CardCode, Filters.ProfileCode, Filters.Name);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        [HttpPost]
        public IActionResult AssignedClients(long IdUser, List<string> ClientIds, List<long> ProfileIds)
        {
            string message = "";
            try
            {
                long currentUser = UserCode;
                DateTime objDate = DateTime.Now;

                List<Field> filters = new() { new Field("IdUser", IdUser) };

                //clientes
                BCS.UserClient bcUserClient = new();
                IList<BES.UserClient> lstNew = new List<BES.UserClient>();
                IEnumerable<BES.UserClient> lstOld = bcUserClient.List(filters, "1");
                IEnumerable<BEA.Client> lstClients = Enumerable.Empty<BEA.Client>();
                if (ClientIds?.Count > 0)
                {
                    BCA.Client bcClient = new();
                    filters = new List<Field> { new Field("CardCode", string.Join(",", ClientIds.Select(x => $"'{x}'")), Operators.In) };
                    lstClients = bcClient.ListShort(filters, "1");
                }
                else
                {
                    ClientIds = new List<string>();
                }
                foreach (string code in ClientIds)
                {
                    BEA.Client beClient = lstClients.FirstOrDefault(x => x.CardCode == code);
                    if (!lstOld.Select(x => x.CardCode).Contains(code))
                    {
                        lstNew.Add(new BES.UserClient { StatusType = StatusType.Insert, CardCode = code, CardName = beClient.CardName, IdUser = IdUser, LogUser = currentUser, LogDate = objDate });
                    }
                    else
                    {
                        BES.UserClient beOld = lstOld.FirstOrDefault(x => x.CardCode == code);
                        if (beOld.CardName != beClient.CardName)
                        {
                            beOld.CardName = beClient.CardName;
                            beOld.LogUser = currentUser;
                            beOld.LogDate = objDate;
                            beOld.StatusType = StatusType.Update;
                            lstNew.Add(beOld);
                        }
                    }
                }
                foreach (var item in lstOld)
                {
                    if (!ClientIds.Contains(item.CardCode))
                    {
                        item.StatusType = StatusType.Delete;
                        lstNew.Add(item);
                    }
                }
                bcUserClient.Save(ref lstNew);

                //perfiles
                if (ProfileIds == null) ProfileIds = new List<long>();
                BCS.UserProfile bcUserProfile = new();
                IList<BES.UserProfile> lstNewP = new List<BES.UserProfile>();
                IEnumerable<BES.UserProfile> lstOldP;
                filters = new List<Field> { new Field("IdUser", IdUser) };
                lstOldP = bcUserProfile.List(filters, "1");
                foreach (long id in ProfileIds)
                {
                    if (!lstOldP.Select(x => x.IdProfile).Contains(id))
                    {
                        lstNewP.Add(new BES.UserProfile { StatusType = StatusType.Insert, IdProfile = id, IdUser = IdUser, LogUser = currentUser, LogDate = objDate });
                    }
                }
                foreach (var item in lstOldP)
                {
                    if (!ProfileIds.Contains(item.IdProfile))
                    {
                        item.StatusType = StatusType.Delete;
                        lstNewP.Add(item);
                    }
                }
                bcUserProfile.Save(ref lstNewP);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost()]
        public IActionResult SendUserData(long Id, bool WithCopy)
        { //, string Name, string Login, string Email) {
            string message = "";
            try
            {
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(Id);
                if (beUser != null)
                {
                    string strPass = NewPassword();
                    beUser.Password = Crypt.Encrypt(strPass);
                    beUser.StatusType = StatusType.Update;
                    beUser.LogUser = UserCode;
                    beUser.LogDate = DateTime.Now;
                    bcUser.Save(ref beUser);
                    SendEmailUser(beUser.Name, beUser.EMail, strPass, WithCopy);
                }
                else
                {
                    message = "El Correo Electrónico no pertenece a un usuario registrado";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SavePermissions(IEnumerable<BES.UserActivity> Items)
        {
            string message = "";
            try
            {
                foreach (var item in Items)
                {
                    item.StatusType = item.Id == 0 ? StatusType.Insert : (item.Permission > 0 ? StatusType.Update : StatusType.Delete);
                    item.LogUser = UserCode;
                    item.LogDate = DateTime.Now;
                }
                IList<BES.UserActivity> items = Items.ToList();
                BCS.UserActivity bcActivity = new();
                bcActivity.Save(ref items);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BES.User> GetItems(string CardCode, long? ProfileCode, string Name)
        {
            IEnumerable<BES.User> lstItems = Enumerable.Empty<BES.User>(), items;
            BCS.User bcUser = new();
            List<Field> lstFilter = new() { new Field("ISNULL(Login, '')", "", Operators.Different) };
            if (!string.IsNullOrWhiteSpace(CardCode))
            {
                lstFilter.AddRange(new[] { new Field("CardCode", CardCode), new Field(LogicalOperators.And) });
            }
            if (ProfileCode.HasValue && ProfileCode.Value > 0)
            {
                lstFilter.AddRange(new[] { new Field("IdProfile", ProfileCode), new Field(LogicalOperators.And) });
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                lstFilter.AddRange(new[] {
                    new Field("FirstName", Name, Operators.Likes),
                    new Field("LastName", Name, Operators.Likes),
                    new Field("Email", Name, Operators.Likes),
                    new Field(LogicalOperators.Or),
                    new Field(LogicalOperators.Or),
                    new Field(LogicalOperators.And)
                });
            }
            lstItems = bcUser.ListExtended(lstFilter, "FirstName", BES.relUser.Profile);
            if (lstItems?.Count() > 0)
            {
                string strCodes = string.Join(",", lstItems.Select(x => $"'{x.CardCode}'"));
                BCA.Client bcClient = new();
                lstFilter = new List<Field> { new Field("CardCode", strCodes, Operators.In) };
                IEnumerable<BEA.Client> lstClients = bcClient.List(lstFilter, "1");

                foreach (var x in lstItems)
                {
                    x.ProfileName = x.Profile?.Name ?? " Ninguno"; //x.Profile != null ? x.Profile.Name : " Ninguno";
                    var beClient = lstClients.FirstOrDefault(c => c.CardCode == x.CardCode);
                    x.ClientName = !string.IsNullOrWhiteSpace(beClient?.CardName) ? $"( {beClient.CardCode} ) {beClient.CardName}" : "";
                }

                items = from i in lstItems
                        join c in lstClients on i.CardCode.ToLower() equals c.CardCode.ToLower() into ljClients
                        from l in ljClients.DefaultIfEmpty()
                        select new BES.User { Id = i.Id };
            }
            return lstItems;
        }

        private void SendEmailUser(string Name, string EMail, string Password, bool withCopy = true)
        {
            string strSubject = "Datos de Acceso al Portal de DMC";
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
            sb.AppendLine($@"				    <p>Estimado Cliente: <strong>{Name}</strong><p/>");
            sb.AppendLine(@"                    <p>Adjunto encontrar&aacute; su nombre de usuario y contrase&ntilde;a para el Portal, la cual deber&aacute; inmediatamente de cambiar por una nueva. Le recomendamos cambiar su contrase&ntilde;a peri&oacute;dicamente para su seguridad.</p>");
            sb.AppendLine(@"                    <p>Sus nuevos datos de acceso al portal son:<br />");
            sb.AppendLine($@"                   <table><tr><td>Nombre de usuario :</td><td><strong>{EMail}</strong> </td></tr>");
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
            sb.AppendLine(@"	</div>");

            List<MailAddress> lstTo = new() { new MailAddress(EMail, Name) }, lstCopies = new(), lstBlindCopies = new();
            if (withCopy)
            {
                FillCustomCopies("Administration", "Users", "SendUserData", ref lstCopies, ref lstBlindCopies);
            }
            _ = SendMailAsync(strSubject, sb.ToString(), lstTo, lstCopies, lstBlindCopies);
        }

        #endregion
    }
}