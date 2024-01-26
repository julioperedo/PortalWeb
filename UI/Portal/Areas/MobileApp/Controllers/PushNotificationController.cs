using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using DocumentFormat.OpenXml.Office2010.Excel;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Portal.Controllers;
using Portal.Models;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCS = BComponents.Security;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BES = BEntities.Security;

namespace Portal.Areas.MobileApp.Controllers
{
    [Area("MobileApp")]
    [Authorize]
    public class PushNotificationController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration _config;
        //string _appKey = "AIzaSyCDDwNYe1nUvQqVuQt68Xnd6gcJqzjgiRM", _senderId = "732364184134";
        #endregion

        #region Constructores

        public PushNotificationController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { _config = configuration; }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            int intPermission = GetPermission("Notificaciones");
            if (IsAllowed("MobileApp", "PushNotification", "Index"))
            {
                ViewData["Permission"] = intPermission;
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        //public IActionResult Edit(long Id)
        //{
        //    BED.Message beMessage = new() { Id = Id, ListMessageRecipientss = new List<BED.MessageRecipients>() };
        //    if (Id != 0)
        //    {
        //        BCD.Message bcMessage = new();
        //        beMessage = bcMessage.Search(Id, BED.relMessage.MessageRecipientss);
        //        beMessage.ListMessageRecipientss ??= new List<BED.MessageRecipients>();
        //    }
        //    return PartialView(beMessage);
        //}

        //public IActionResult GetTopics()
        //{
        //    BCB.Classifier bcClassifier = new();
        //    List<Field> filters = new() { new Field("IdType", (long)BEE.Classifiers.MobileNotificationTopics) };
        //    IEnumerable<BEB.Classifier> lstClassifiers = bcClassifier.List(filters, "1");
        //    var lstItems = lstClassifiers.Select(x => new { x.Id, x.Name });
        //    return Json(lstItems);
        //}

        public IActionResult GetRecipients(long MessageId)
        {
            string message = "";
            try
            {
                BCD.MessageRecipients bcRecipients = new();
                List<Field> filters = new() { new Field("IdMessage", MessageId) };
                IEnumerable<BED.MessageRecipients> items = bcRecipients.List(filters, "1") ?? new List<BED.MessageRecipients>();
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetUsers(string Filter)
        {
            BCS.User bcUser = new();
            List<BES.User> lstUsers = bcUser.ListWithTokens(Filter, "FirstName, Lastname");
            var items = (from u in lstUsers orderby u.Name select new { u.Id, u.Name, u.CardCode }).ToList();
            return Json(items);
        }

        public IActionResult Filter(string Name, DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                IEnumerable<BED.Message> lstMessages = GetItems(Name, InitialDate, FinalDate);
                var items = lstMessages.Select(m => new { m.Id, m.Title, m.Body, m.Date, m.RecipientsType });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult RemoveFile(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            foreach (var strFileName in fileNames)
            {
                string path = Path.Combine(rootDirectory, "wwwroot", "images", "notification", "push", strFileName);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            return Content("");
        }

        public async Task<IActionResult> SaveFile(IEnumerable<IFormFile> files)
        {
            // The Name of the Upload component is "files"
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                        var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                        var path = Path.Combine(rootDirectory, "wwwroot", "images", "notification", "push", fileName);
                        using var fileStream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception) { }
            // Return an empty string to signify success
            return Content("");
        }

        #endregion

        #region POSTs

        [HttpPost("send")]
        public async Task<IActionResult> SendAsync(string Title, string Message, string ToType, List<long> Tos, string ImageUrl)
        {
            string message = "";
            bool sent = false;
            long id = 0;
            try
            {
                (id, sent) = await SendMessageAsync(Title, Message, ToType, Tos, ImageUrl);
                //IEnumerable<BED.Message> lstMessages = GetItems("", DateTime.Now.AddDays(-7), null);
                //var items = lstMessages.Select(m => new { m.Id, m.Title, m.Body, m.Date, m.RecipientsType });
                //return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, sent, id });
        }

        #endregion

        #region Private Methods

        private IEnumerable<BED.Message> GetItems(string Filter, DateTime? Initial, DateTime? Final)
        {
            BCD.Message bcMessage = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                lstFilter.AddRange(new[] { new Field("Title", Filter.Trim(), Operators.Likes), new Field("Body", Filter.Trim(), Operators.Likes), new Field(LogicalOperators.Or) });
            }
            if (Initial.HasValue)
            {
                lstFilter.Add(new Field("CAST([Date] AS DATE)", Initial.Value.ToString("yyyy/MM/dd"), Operators.HigherOrEqualThan));
                if (lstFilter.Count % 2 == 0)
                {
                    lstFilter.Add(new Field(LogicalOperators.And));
                }
            }
            if (Final.HasValue)
            {
                lstFilter.Add(new Field("CAST([Date] AS DATE)", Final.Value.ToString("yyyy/MM/dd"), Operators.LowerOrEqualThan));
                if (lstFilter.Count % 2 == 0)
                {
                    lstFilter.Add(new Field(LogicalOperators.And));
                }
            }
            IEnumerable<BED.Message> lstMessages = bcMessage.List(lstFilter, "1");
            return lstMessages;
        }

        private async Task<(long, bool)> SendMessageAsync(string _title, string _message, string _type, List<long> _tos, string _imageUrl)
        {
            long intId = 0, intUserId = UserCode;
            bool sent = false;
            DateTime now = DateTime.Now;
            List<string> lstTos = new();
            if (_type == "U")
            {
                BCD.UserToken bcToken = new();
                List<Field> lstFilter = new() { new Field("IdUser", string.Join(",", _tos), Operators.In) };
                IEnumerable<BED.UserToken> lstTokens = bcToken.List(lstFilter, "1");
                lstTos = (from t in lstTokens select t.Token).ToList();
            }
            if (_type == "T")
            {
                BCB.Classifier bcClassifier = new();
                List<Field> filters = new() { new Field("IdType", (long)BEE.Classifiers.MobileNotificationTopics) };
                IEnumerable<BEB.Classifier> lstClassifier = bcClassifier.List(filters, "1");
                lstTos = (from t in lstClassifier where _tos.Contains(t.Id) select t.Name).ToList();
            }

            BCD.Message bcMessage = new();
            BED.Message beMessage = new()
            {
                StatusType = StatusType.Insert,
                Title = _title,
                Body = _message,
                RecipientsType = _type,
                Date = now,
                ImageUrl = _imageUrl,
                LogUser = intUserId,
                LogDate = now,
                ListMessageRecipientss = new List<BED.MessageRecipients>()
            };
            foreach (var item in _tos)
            {
                beMessage.ListMessageRecipientss.Add(new BED.MessageRecipients { StatusType = StatusType.Insert, Recipient = item, LogUser = intUserId, LogDate = now });
            }
            bcMessage.Save(ref beMessage);
            intId = beMessage.Id;

            var app = FirebaseApp.GetInstance("Portal");
            var messagingInstance = FirebaseMessaging.GetMessaging(app);
            var data = new Dictionary<string, string> {
                { "msgId", intId.ToString() }, { "msgTitle", _title }, { "msgBody", _message }, { "msgDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, { "msgImage", _imageUrl },
                { "click_action", "FLUTTER_NOTIFICATION_CLICK" }
            };
            if (_type == "U")
            {
                var pushMessage = new MulticastMessage() { Notification = new Notification { Title = _title, Body = _message }, Data = data, Tokens = lstTos };
                var result = await messagingInstance.SendMulticastAsync(pushMessage);
                sent = result.FailureCount == 0;
            }
            else
            {
                var pushMessage = new Message() { Notification = new Notification { Title = _title, Body = _message }, Data = data };
                if (IsDevelopmentMode)
                {
                    pushMessage.Token = "dlcdYhazR46F806BYWUts7:APA91bGRPlEndhGO31yb-0KryydEwOEvhZ_9NuDviE9bfBw4hMRNVeMN_7N2pGqitmJesBmGvjmENN6cOC4CM4AgabodOjeZYr9ymXDVnt-5-sXbL6EsRHE538N9HJi2bVq_UIYPobvs";
                }
                else
                {
                    pushMessage.Topic = "news";
                }
                var result = await messagingInstance.SendAsync(pushMessage);
                sent = !string.IsNullOrEmpty(result);
            }
            return (intId, sent);
        }

        #endregion

    }
}