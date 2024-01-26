using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using BEntities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using Portal.Membership;
using BCS = BComponents.Security;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class ClientConfigController : BaseController
    {
        #region Constructores

        public ClientConfigController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        // GET: Customer/ConfigData
        public IActionResult Index()
        {
            ViewBag.CardCode = CardCode;
            return View();
        }

        public IActionResult GetItems()
        {
            string message = "";
            try
            {
                var items = GetContactsList();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetTypes()
        {
            var items = new[] { new { Id = 1, Name = "Recordatorio de Vencimientos" }, new { Id = 2, Name = "Copia de Notas de Venta" }, new { Id = 3, Name = "Ofertas" } };
            return Json(items);
        }

        public IActionResult Edit(long Id)
        {
            string message = "";
            try
            {
                BCS.ClientContacts bcContact = new BCS.ClientContacts();
                var item = bcContact.Search(Id);
                return Json(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult EditUser(long Id)
        {
            string message = "";
            try
            {
                BCS.WebServiceUser bcUser = new BCS.WebServiceUser();
                var item = bcUser.Search(Id);
                return Json(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetUsers()
        {
            string message = "";
            try
            {
                BCS.WebServiceClient bcWSClient = new BCS.WebServiceClient();
                var clients = bcWSClient.List("1");
                bool allowed = clients.Select(x => x.CardCode).Contains(CardCode);

                var users = GetUsersList();
                return Json(new { message, allowed, users });
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
        public IActionResult Edit(ClientContacts Item)
        {
            string message = "";
            try
            {
                if (IsEMailBlacklisted(Item.EMail))
                {
                    message = "El correo está en lista negra, para mayor información comuníquese con su ejecutivo de ventas.";
                }
                else
                {
                    BCS.ClientContacts bcContact = new();
                    Item.StatusType = Item.Id > 0 ? BEntities.StatusType.Update : BEntities.StatusType.Insert;
                    Item.CardCode = CardCode;
                    Item.LogUser = UserCode;
                    Item.LogDate = DateTime.Now;
                    bcContact.Save(ref Item);

                    IEnumerable<BES.ClientContacts> items = GetContactsList();
                    return Json(new { message, items });
                }
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
                BCS.ClientContacts bcContact = new();
                BES.ClientContacts contact = new() { StatusType = BEntities.StatusType.Delete, Id = Id, LogDate = DateTime.Now };
                bcContact.Save(ref contact);

                IEnumerable<BES.ClientContacts> items = GetContactsList();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult EditUser(WebServiceUser Item)
        {
            string message = "", errors = "";
            try
            {
                BCS.WebServiceUser bcUser = new();

                List<Field> filters = new() { new Field("Id", Item.Id, Operators.Different), new Field("LOWER(CardCode)", Item.CardCode.ToLower()), new Field("LOWER(Login)", Item.Login.ToLower()), new Field(LogicalOperators.And), new Field(LogicalOperators.And) };
                var tempUsers = bcUser.List(filters, "1");

                if (tempUsers?.Count() > 0)
                {
                    message = $"Ya existe un usuario llamado: {Item.Login}";
                }
                else
                {
                    Item.StatusType = Item.Id > 0 ? BEntities.StatusType.Update : BEntities.StatusType.Insert;
                    Item.Password = Crypt.Encrypt(Item.Password);
                    Item.CardCode = CardCode;
                    Item.LogUser = UserCode;
                    Item.LogDate = DateTime.Now;
                    bcUser.Save(ref Item);

                    IEnumerable<BES.WebServiceUser> items = GetUsersList();
                    return Json(new { message, errors, items });
                }
            }
            catch (Exception ex)
            {
                errors = GetError(ex);
            }
            return Json(new { message, errors });
        }

        [HttpPost]
        public IActionResult DeleteUser(long Id)
        {
            string message = "";
            try
            {
                BCS.WebServiceUser bcUser = new();
                BES.WebServiceUser user = new() { StatusType = BEntities.StatusType.Delete, Id = Id, LogDate = DateTime.Now };
                bcUser.Save(ref user);

                IEnumerable<BES.WebServiceUser> items = GetUsersList();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Metodos Privados

        private IEnumerable<BES.ClientContacts> GetContactsList()
        {
            BCS.ClientContacts bcContact = new();
            List<Field> filters = new() { new Field { Name = "CardCode", Value = CardCode } };
            IEnumerable<BES.ClientContacts> lstItems = bcContact.List(filters, "Name");

            if (lstItems?.Count() > 0)
            {
                BCS.MailBlacklist bcBlackList = new BCS.MailBlacklist();
                string mails = string.Join(",", lstItems.Select(x => $"'{x.EMail}'").Distinct());
                filters = new List<Field> { new Field("EMail", mails, Operators.In) };
                var blackListed = bcBlackList.List(filters, "1");

                foreach (var x in lstItems)
                {
                    x.TypeDesc = x.Type == 1 ? "Recordatorio de Vencimientos" : (x.Type == 2 ? "Copias de Notas de Venta" : "Ofertas");
                    x.BlackList = blackListed.Select(z => z.EMail).Contains(x.EMail);
                }
            }

            return lstItems;
        }

        private IEnumerable<BES.WebServiceUser> GetUsersList()
        {
            BCS.WebServiceUser bcUser = new();
            List<Field> filters = new() { new Field("CardCode", CardCode) };
            IEnumerable<BES.WebServiceUser> items = bcUser.List(filters, "Id");
            foreach (var x in items) x.Password = Crypt.Decrypt(x.Password);
            return items;
        }

        #endregion
    }
}
