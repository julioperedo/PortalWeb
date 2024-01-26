using MobileService.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BEntities.Filters;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCF = BComponents.Staff;
using BCL = BComponents.Sales;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BEF = BEntities.Staff;
using BEL = BEntities.Sales;
using BES = BEntities.Security;
using Microsoft.AspNetCore.Mvc;
using BEntities;

namespace MobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : BaseController
    {

        #region GETs

        [HttpGet("staff")]
        public IActionResult Staff()
        {
            string message = "";
            try
            {
                BCF.Department bcDepartment = new BCF.Department();
                IEnumerable<BEF.Department> lstDepartments = bcDepartment.List("Name", BEF.relDepartment.Members);
                var items = (from d in lstDepartments
                             orderby d.Order
                             select new
                             {
                                 id = d.Id,
                                 name = d.Name,
                                 listOrder = d.Order,
                                 members = (from m in d.ListMembers
                                            orderby m.Order
                                            select new { id = m.Id, name = m.Name, position = m.Position, mail = m.Mail, photo = m.Photo, phone = m.Phone, listOrder = m.Order }).ToList()
                             }).ToList();
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("clients")]
        public IActionResult Clients(long UserId)
        {
            string message = "";
            try
            {
                IEnumerable<BEA.Client> lstClients;
                BCA.Client bcClients = new();
                List<Field> lstFilter = new();

                BCS.ProfileActivity bcActivity = new();
                int permission = bcActivity.GetPermission(UserId, "SeeAllClients");
                if (permission == 0)
                {
                    BCS.UserData bcData = new();
                    var data = bcData.SearchByUser(UserId);
                    lstFilter.AddRange(new[] { new Field("SellerCode", data?.SellerCode ?? "XXX"), new Field("SlpCode", -1), new Field(LogicalOperators.Or) });
                }
                CompleteFilters(ref lstFilter);
                lstClients = bcClients.ListShort3(lstFilter, "CardName");

                var items = lstClients.Select(c => new { id = c.CardCode.ToUpper(), name = ToTitle(c.CardName) });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("sellers")]
        public IActionResult Sellers()
        {
            string message = "";
            try
            {
                BCA.Seller bcSeller = new();
                IEnumerable<BEA.Seller> lstItems = bcSeller.List(null, "1");
                var items = (from i in lstItems orderby i.Name select new { Code = i.ShortName, Name = ToTitle(i.Name) }).Distinct();
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("subsidiaries")]
        public IActionResult Subsidiaries()
        {
            string strMessage = "";
            try
            {
                BCB.Classifier bcClassifier = new();
                List<BEB.Classifier> lstItems = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");
                var lstResult = from i in lstItems select new { i.Id, Name = ToTitle(i.Name) };
                return Ok(new { Message = strMessage, Items = lstResult });
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            return Ok(new { Message = strMessage });
        }

        [HttpGet("bankaccounts")]
        public IActionResult BankAccounts()
        {
            string message = "";
            try
            {
                BCL.BankAccount bcAccount = new();
                IEnumerable<BEL.BankAccount> lstAccounts = bcAccount.List("1");

                var items = from a in lstAccounts
                            group a by a.Subsidiary into g
                            select new
                            {
                                name = g.Key,
                                items = from d in g
                                        select new { name = d.Holder, bank = d.Bank, currency = d.Currency, number = d.Number, country = d.Country, type = d.Type, abaNumber = d.ABANumber }
                            };
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("getpublicity")]
        public IActionResult GetPublicity(long IdUser, string CardCode)
        {
            List<BEB.Notification> notifications = new List<BEB.Notification>();
            string message = "";
            try
            {
                BCB.Notification bcNotification = new();
                List<BEB.Notification> lstTemp = bcNotification.List(CardCode, null, DateTime.Today, DateTime.Today, true, "InitialDate DESC", BEB.relNotification.NotificationClients);
                lstTemp = lstTemp.FindAll(i => !string.IsNullOrEmpty(i.MobileValue) & (i.ListNotificationClients == null || (i.ListNotificationClients.Count == 0 | (from d in i.ListNotificationClients select d.CardCode).Contains(CardCode))));

                BCS.LastNotification bcLast = new();
                var lstLast = (from i in bcLast.List(IdUser) ?? new List<BES.LastNotification>() orderby Guid.NewGuid() select i).ToList();

                foreach (var item in lstTemp)
                {
                    BES.LastNotification last = lstLast.FirstOrDefault(x => x.IdNotification == item.Id); //(from l in lstLast where l.IdNotification == item.Id select l).FirstOrDefault();
                    if (last != null)
                    {
                        TimeSpan period = DateTime.Now - last.LogDate;
                        if (period.TotalHours > item.Frequency)
                        {
                            item.Value = SetHTMLSafe(item.Value);
                            notifications.Add(item);
                        }
                    }
                    else
                    {
                        item.Value = SetHTMLSafe(item.Value);
                        notifications.Add(item);
                    }
                }

                if (notifications.Count > 0)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(1, notifications.Count);
                    var temp = notifications[index - 1];
                    var item = new { temp.Id, temp.Name, value = temp.MobileValue };

                    BES.LastNotification last = lstLast.FirstOrDefault(x => x.IdNotification == item.Id) ?? new BES.LastNotification { IdNotification = item.Id, StatusType = StatusType.Insert, LogDate = DateTime.Now, LogUser = IdUser };
                    if (last.StatusType == StatusType.NoAction)
                    {
                        last.StatusType = StatusType.Update;
                        last.LogDate = DateTime.Now;
                    }
                    bcLast.Save(ref last);

                    return Ok(new { message, item });
                }
                else
                {
                    message = "No hay notificaciones";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        #endregion

        #region Private Methods

        private List<BEB.Notification> GetItems(string CardCode, string Name, DateTime? InitialDate, DateTime? FinalDate, string State)
        {
            BCB.Notification bcNotification = new BCB.Notification();
            bool? boEnabled = null;
            if (!string.IsNullOrWhiteSpace(State) && State.Trim() != "A")
            {
                boEnabled = State.Trim() == "E";
            }
            List<BEB.Notification> lstItems = bcNotification.List(CardCode, Name, InitialDate, FinalDate, boEnabled, "InitialDate DESC", BEB.relNotification.NotificationClients);
            return lstItems;
        }

        private List<BES.LastNotification> GetLastNotifications(long IdUser)
        {
            BCS.LastNotification bcLast = new BCS.LastNotification();
            var items = (from i in bcLast.List(IdUser) orderby Guid.NewGuid() select i).ToList();
            return items;
        }

        private static void SaveLastNotifications(List<BES.LastNotification> items)
        {
            BCS.LastNotification bcLast = new();
            IList<BES.LastNotification> list = items.Where(x => x.StatusType != StatusType.NoAction).ToList(); //(from i in items where i.StatusType != StatusType.NoAction select i).ToList();
            bcLast.Save(ref list);
        }

        #endregion

        //[HttpGet]
        //public IActionResult FullStaff() {
        //    string strMessage = "";
        //    try {
        //        var bcStaff = new BCD.StaffMembers();
        //        var localStaff = bcStaff.List("1");

        //        var lstItems = (from d in localStaff where d.Active select new { id = d.Id, firstName = d.FirstName, lastName = d.LastName }).ToList();
        //        return Ok( new { Message = strMessage, Members = lstItems });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Ok( new { Message = strMessage });
        //}

        //[HttpGet]
        //public IActionResult ClientsByVersion(long Version) {
        //    string strMessage = "";
        //    try {
        //        BCD.Client bcClient = new BCD.Client();
        //        var lstFilter = new List<Field> { new Field { Name = "Version", Value = Version, Operator = Operators.HigherThan } };
        //        List<BED.Client> lstClients = bcClient.List(lstFilter, "Name");
        //        var lstItems = (from c in lstClients select new { Id = c.Id, Name = ToTitle(c.Name), Version = c.Version }).ToList();
        //        return Ok( new { Message = strMessage, Items = lstItems });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Ok( new { Message = strMessage });
        //}

        //[HttpGet]
        //public IActionResult NotificationTopics() {
        //    string strMessage = "";
        //    try {
        //        BCB.Classifier bcClassifier = new BCB.Classifier();
        //        List<BEB.Classifier> lstClassifiers = bcClassifier.List(new long[] { (long)BEE.Classifiers.MobileNotificationTopics }, "IdType");
        //        var lstItems = (from c in lstClassifiers select new { id = c.Id, name = c.Name, type = c.Value }).ToList();
        //        return Ok( new { Message = strMessage, Items = lstItems });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Ok( new { Message = strMessage });
        //}

        //[HttpGet]
        //public IActionResult Parameters() {
        //    string strMessage = "";
        //    try {
        //        BCB.Classifier bcClassifier = new BCB.Classifier();
        //        List<BEB.Classifier> lstClassifiers = bcClassifier.List(new long[] { (long)BEE.Classifiers.GatekeeperAppParameters }, "IdType");
        //        var lstItems = (from c in lstClassifiers select new { id = c.Id, name = c.Name, value = c.Value }).ToList();
        //        return Ok( new { message = strMessage, items = lstItems });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Ok( new { Message = strMessage });
        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadFile() {
        //    try {
        //        if(!Request.Content.IsMimeMultipartContent()) {
        //            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //        }

        //        //Save To this server location
        //        var uploadPath = WebConfigurationManager.AppSettings["PersonDirectory"];
        //        //HttpContext.Current.Server.MapPath("~/Uploads");
        //        //The reason we not use the default MultipartFormDataStreamProvider is because the saved file name is look weird, not believe me? uncomment below and try out, 
        //        //the odd file name is designed for security reason -_-'.
        //        //var streamProvider = new MultipartFormDataStreamProvider(uploadPath);

        //        //Save file via CustomUploadMultipartFormProvider
        //        var multipartFormDataStreamProvider = new CustomUploadMultipartFormProvider(uploadPath);

        //        // Read the MIME multipart asynchronously 
        //        await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

        //        // Show all the key-value pairs.
        //        foreach(var key in multipartFormDataStreamProvider.FormData.AllKeys) {
        //            foreach(var val in multipartFormDataStreamProvider.FormData.GetValues(key)) {
        //                Console.WriteLine(string.Format("{0}: {1}", key, val));
        //            }
        //        }

        //        //In Case you want to get the files name
        //        string localFileName = multipartFormDataStreamProvider.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

        //        return new IActionResult(HttpStatusCode.OK) { Content = new StringContent($"Se subió el archivo {localFileName}") };
        //    } catch(Exception ex) {
        //        string message = ex.Message;
        //        Exception e = ex.InnerException;
        //        while(e != null) {
        //            message += Environment.NewLine + e.Message;
        //            e = e.InnerException;
        //        }
        //        message += ex.StackTrace;
        //        return new IActionResult(HttpStatusCode.InternalServerError) { Content = new StringContent(message) };
        //    }

        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadUserPhoto() {
        //    try {
        //        if(!Request.Content.IsMimeMultipartContent()) {
        //            throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //        }

        //        //Save To this server location
        //        var uploadPath = WebConfigurationManager.AppSettings["UserDataDirectory"];
        //        //HttpContext.Current.Server.MapPath("~/Uploads");
        //        //The reason we not use the default MultipartFormDataStreamProvider is because the saved file name is look weird, not believe me? uncomment below and try out, 
        //        //the odd file name is designed for security reason -_-'.
        //        //var streamProvider = new MultipartFormDataStreamProvider(uploadPath);

        //        //Save file via CustomUploadMultipartFormProvider
        //        var multipartFormDataStreamProvider = new CustomUploadMultipartFormProvider(uploadPath);

        //        // Read the MIME multipart asynchronously 
        //        await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

        //        // Show all the key-value pairs.
        //        foreach(var key in multipartFormDataStreamProvider.FormData.AllKeys) {
        //            foreach(var val in multipartFormDataStreamProvider.FormData.GetValues(key)) {
        //                Console.WriteLine(string.Format("{0}: {1}", key, val));
        //            }
        //        }

        //        //In Case you want to get the files name
        //        string localFileName = multipartFormDataStreamProvider.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
        //        string fileName = localFileName.Split('\\').Last();
        //        if(!string.IsNullOrWhiteSpace(fileName)) {
        //            string[] parts = fileName.Split('_');
        //            if(parts.Length > 0) {
        //                long userId = 0;
        //                long.TryParse(parts[1], out userId);
        //                if(userId != 0) {
        //                    BCS.User bcUser = new BCS.User();
        //                    BES.User beUser = bcUser.Search(userId);
        //                    beUser.Picture = fileName;
        //                    beUser.StatusType = BEntities.StatusType.Update;
        //                    bcUser.Save(ref beUser);
        //                }
        //            }
        //        }

        //        return new IActionResult(HttpStatusCode.OK) { Content = new StringContent($"Se subió el archivo {localFileName}") };
        //    } catch(Exception ex) {
        //        string message = ex.Message;
        //        Exception e = ex.InnerException;
        //        while(e != null) {
        //            message += Environment.NewLine + e.Message;
        //            e = e.InnerException;
        //        }
        //        message += ex.StackTrace;
        //        return new IActionResult(HttpStatusCode.InternalServerError) { Content = new StringContent(message) };
        //    }

        //}

    }
}