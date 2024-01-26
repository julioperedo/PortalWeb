using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Portal.Controllers;
using System.Collections.Generic;
using System;
using BCG = BComponents.PiggyBank;
using BEG = BEntities.PiggyBank;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Portal.Models;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Portal.Areas.Marketing.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using BEntities;
using Microsoft.AspNetCore.Http;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class PiggyBankNotificationsController : BaseController
    {
        #region Variables Globales
        //private readonly IConfiguration _config;
        //private readonly IWebHostEnvironment _env;
        //private static readonly HttpClient http = new();
        #endregion

        #region Constructors

        public PiggyBankNotificationsController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment)
        {
            //_config = Configuration;
            //_env = HEnviroment;
        }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            int intPermission = GetPermission("Notificaciones");
            if (IsAllowed("Marketing", "PiggyBankNotifications", "Index"))
            {
                ViewData["Permission"] = intPermission;
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public async Task<IActionResult> FilterAsync(string Name, DateTime? InitialDate, DateTime? FinalDate)
        {
            string message = "";
            try
            {
                IEnumerable<BEG.Message> lstMessages = await GetItemsAsync(Name, InitialDate, FinalDate);
                var items = lstMessages.Select(m => new { m.Id, m.Title, m.Body, m.Date });
                return Json(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public async Task<IActionResult> GetUsersAsync(string Filter)
        {
            BCG.User bcUser = new();
            IEnumerable<BEG.User> lstUsers = await bcUser.ListWithTokensAsync(Filter, "Name");
            var items = from u in lstUsers orderby u.Name select new { u.Id, u.Name, u.StoreName, FullName = $"{u.City} - {u.Name}", FullName2 = $"{u.City} - {u.StoreName} - {u.Name}" };
            return Json(items);
        }

        public IActionResult RemoveFile(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            foreach (var strFileName in fileNames)
            {
                string strPhysicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "piggybank", "messages", strFileName);
                if (System.IO.File.Exists(strPhysicalPath))
                    System.IO.File.Delete(strPhysicalPath);
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
                        var physicalPath = Path.Combine(rootDirectory, "wwwroot", "images", "piggybank", "messages", fileName);
                        using var fileStream = new FileStream(physicalPath, FileMode.Create);
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

        [HttpPost]
        public async Task<IActionResult> SendMessageAsync(string Title, string Message, string ToType, List<long> Tos, string ImageUrl)
        {
            string message = "";
            bool sent = false;
            long id = 0;
            try
            {
                List<string> lstTos = new();
                if (ToType == "U")
                {
                    BCG.UserToken bcToken = new();
                    List<Field> lstFilter = new() { new Field("IdUser", string.Join(",", Tos), Operators.In) };
                    IEnumerable<BEG.UserToken> lstTokens = bcToken.List(lstFilter, "1");
                    lstTos = (from t in lstTokens select t.Token).ToList();
                }

                BCG.Message bcMessage = new();
                BEG.Message msg = new() { StatusType = StatusType.Insert, Title = Title, Body = Message, ImageUrl = ImageUrl, RecipientsType = ToType, Date = DateTime.Now, LogUser = UserCode, LogDate = DateTime.Now, ListMessageRecipientss = new List<BEG.MessageRecipients>() };
                if (ToType == "U")
                {
                    foreach (var item in Tos)
                    {
                        msg.ListMessageRecipientss.Add(new BEG.MessageRecipients { StatusType = StatusType.Insert, Recipient = item, LogUser = UserCode, LogDate = DateTime.Now });
                    }
                }
                id = await bcMessage.SaveAsync(msg);


                var app = FirebaseApp.GetInstance("PiggyBank");
                var messagingInstance = FirebaseMessaging.GetMessaging(app);

                var data = new Dictionary<string, string> {
                    { "msgId", id.ToString() }, { "msgTitle", Title }, { "msgBody", Message }, { "msgDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, { "msgImage", ImageUrl },
                    { "click_action", "FLUTTER_NOTIFICATION_CLICK" }
                };
                if (ToType == "U")
                {
                    var pushMessage = new MulticastMessage() { Notification = new Notification { Title = Title, Body = Message }, Data = data, Tokens = lstTos };
                    var result = await messagingInstance.SendMulticastAsync(pushMessage);
                    sent = result.FailureCount == 0;
                }
                else
                {
                    var pushMessage = new Message() { Notification = new Notification { Title = Title, Body = Message }, Data = data };
                    if (IsDevelopmentMode)
                    {
                        pushMessage.Token = "dlcdYhazR46F806BYWUts7:APA91bGRPlEndhGO31yb-0KryydEwOEvhZ_9NuDviE9bfBw4hMRNVeMN_7N2pGqitmJesBmGvjmENN6cOC4CM4AgabodOjeZYr9ymXDVnt-5-sXbL6EsRHE538N9HJi2bVq_UIYPobvs";
                    }
                    else
                    {
                        pushMessage.Topic = "news";
                    }
                    var result = await messagingInstance.SendAsync(pushMessage);
                    if (string.IsNullOrEmpty(result)) message = "Error al enviar el mensaje";
                    sent = !string.IsNullOrEmpty(result);
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, sent, id });
        }

        #endregion

        #region Private Methods

        private async Task<IEnumerable<BEG.Message>> GetItemsAsync(string Filter, DateTime? Initial, DateTime? Final)
        {
            BCG.Message bcMessage = new();
            List<Field> lstFilter = new();
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                lstFilter.AddRange(new[] { new Field("Title", Filter.Trim(), Operators.Likes), new Field("Body", Filter.Trim(), Operators.Likes), new Field(LogicalOperators.Or) });
            }
            if (Initial.HasValue)
            {
                lstFilter.Add(new Field("CAST([Date] AS DATE)", Initial.Value.ToString("yyyy/MM/dd"), Operators.HigherOrEqualThan));
                if (lstFilter.Count % 2 == 0) lstFilter.Add(new Field(LogicalOperators.And));
            }
            if (Final.HasValue)
            {
                lstFilter.Add(new Field("CAST([Date] AS DATE)", Final.Value.ToString("yyyy/MM/dd"), Operators.LowerOrEqualThan));
                if (lstFilter.Count % 2 == 0) lstFilter.Add(new Field(LogicalOperators.And));
            }
            IEnumerable<BEG.Message> lstMessages = await bcMessage.ListAsync(lstFilter, "1");
            return lstMessages;
        }

        #endregion
    }
}
