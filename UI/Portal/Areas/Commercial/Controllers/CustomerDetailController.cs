using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    [Authorize]
    public class CustomerDetailController : BaseController
    {

        #region Constructores

        public CustomerDetailController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                ViewBag.SeeMargin = GetPermission("VerMargen") > 0 ? "Y" : "N";
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult GetClientsLocal(string Name)
        {
            IEnumerable<BEA.Client> lstClients;
            BCA.Client bcClients = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(Name))
                lstFilter.Add(new Field("CardName", Name, Operators.Likes));
            CompleteFilters(ref lstFilter);
            lstClients = bcClients.ListShort3(lstFilter, "CardName");

            var lstResult = from c in lstClients
                            group c by c.CardCode into g
                            select new { Code = g.Key, Name = $"{g.Key} - {g.FirstOrDefault()?.CardName ?? ""}      -      NIT: {g.FirstOrDefault()?.NIT}" };
            return Json(lstResult);
        }

        public IActionResult GetClient(string CardCode)
        {
            string message = "";
            try
            {
                BCA.Client bcClient = new();
                BCA.Payment bcPayment = new();
                BEA.Client client = bcClient.Search(CardCode) ?? new BEA.Client();
                BEA.ClientStatics staticts = bcClient.SearchStaticts(CardCode) ?? new BEA.ClientStatics();
                IEnumerable<BEA.ClientExtra> extras = bcClient.ListExtras(CardCode) ?? Enumerable.Empty<BEA.ClientExtra>();
                var years = bcClient.ListYears(CardCode) ?? Enumerable.Empty<BEA.ClientStatics>();

                BCS.User bcUser = new();
                List<BES.User> lstUsers = bcUser.List(CardCode, "FirstName, LastName", BES.relUser.Profile);
                var users = lstUsers.Select(u => new { u.Name, u.EMail, u.Enabled, Profile = u.Profile.Name });

                BCA.ContactPerson bcContacts = new();
                IEnumerable<BEA.ContactPerson> lstContacts = bcContacts.List(new List<Field> { new("CardCode", CardCode) }, "Name");
                var contacts = from c in lstContacts select new { c.Id, c.Name };

                BCA.Address bcAddress = new();
                IEnumerable<BEA.Address> lstAddresses = bcAddress.List(new List<Field> { new() { Name = "CardCode", Value = CardCode } }, "Name");
                var addresses = from c in lstAddresses
                                group c by c.Type into g
                                orderby g.Key
                                select new { Name = g.Key == "B" ? "Destinatario de Factura" : "Destino", Type = g.Key, Items = from d in g orderby d.Name select new { d.Name, Type = g.Key } };

                BCA.Attachment bcAttach = new();
                IEnumerable<BEA.Attachment> files = bcAttach.List(new List<Field> { new("CardCode", CardCode) }, "Line") ?? Enumerable.Empty<BEA.Attachment>();

                BCS.ClientContacts bcMailContacts = new();
                List<Field> filters = new() { new Field { Name = "CardCode", Value = CardCode } };
                IEnumerable<BES.ClientContacts> lstClientContacts = bcMailContacts.List(filters, "Type") ?? new List<BES.ClientContacts>();

                BCS.MailBlacklist bcBlackList = new();
                List<BES.MailBlacklist> lstBlackList = bcBlackList.ListInContacts(CardCode, "1");

                var mailContacts = (from c in lstClientContacts
                                    join b in lstBlackList on c.EMail.ToLower() equals b.EMail.ToLower() into ljBlackList
                                    from l in ljBlackList.DefaultIfEmpty()
                                    select new { c.Id, c.Name, c.EMail, c.Type, c.LogDate, BlackList = l != null }).Distinct();

                int avgDueDays = bcPayment.SearchAVGDuedays(CardCode);

                return Json(new { message, item = new { client, extras, users, contacts, addresses, files, mailContacts, staticts, years, avgDueDays } });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { Message = message });
        }

        public IActionResult GetContactData(int Code)
        {
            string message = "";
            try
            {
                BCA.ContactPerson bcContacts = new BCA.ContactPerson();
                BEA.ContactPerson beContact = bcContacts.Search(Code);
                return Json(new { message, data = beContact });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetAddressData(string CardCode, string Name, string Type)
        {
            string message = "";
            try
            {
                BCA.Address bcAddress = new BCA.Address();
                BEA.Address item = bcAddress.Search(CardCode, Name, Type);
                return Json(new { message, item });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetAttachmentData(string CardCode, int Line)
        {
            string message = "";
            try
            {
                BCA.Attachment bcAttachment = new();
                BEA.Attachment beAttach = bcAttachment.Search(CardCode, Line);
                string fullName = $@"{beAttach.Path}\{beAttach.FileName}.{beAttach.FileExt}";
                byte[] file = System.IO.File.ReadAllBytes(fullName);
                return File(file, "application/octet-stream", $"{beAttach.FileName}.{beAttach.FileExt}");
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { Message = message });
        }

        public IActionResult GetMonths(string CardCode, int Year)
        {
            string message = "";
            try
            {
                BCA.Client bcClient = new BCA.Client();
                var months = bcClient.ListMonths(CardCode, Year);
                return Json(new { message, months });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        #endregion
    }
}