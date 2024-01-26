using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class NotificationController : BaseController
    {
        #region Variables Globales

        private readonly IWebHostEnvironment _env;

        #endregion

        #region Constructores

        public NotificationController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { _env = env; }

        #endregion

        #region GETs
        public IActionResult Index()
        {
            int intPermission = GetPermission("Notificaciones");
            if (IsAllowed("Marketing", "Notification", "Index"))
            {
                ViewData["Permission"] = intPermission;
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public IActionResult Filter(string Name, DateTime? InitialDate, DateTime? FinalDate, string State)
        {
            string message = "";
            List<BEB.Notification> items = new List<BEB.Notification>();
            try
            {
                items = GetItems(Name, InitialDate, FinalDate, State);
            }
            catch (Exception ex)
            {
                message = "Se ha producido el siguiente error al traer los datos: <br />" + GetError(ex);
            }
            return Json(new { message, items });
        }

        public IActionResult Edit(long Id = -1)
        {
            if (Id >= 0)
            {
                BEB.Notification beItem;
                if (Id == 0)
                {
                    beItem = new BEB.Notification
                    {
                        InitialDate = DateTime.Today,
                        Frequency = 8,
                        Enabled = true,
                        Popup = false,
                        LogDate = DateTime.Now,
                        ListNotificationDetails = new List<BEB.NotificationDetail>(),
                        ListNotificationClients = new List<BEB.NotificationClient>()
                    };
                }
                else
                {
                    BCB.Notification bcNotification = new();
                    beItem = bcNotification.Search(Id, BEB.relNotification.NotificationDetails, BEB.relNotification.NotificationClients);
                    if (!string.IsNullOrWhiteSpace(beItem.Value))
                    {
                        beItem.Value = SetHTMLSafe(beItem.Value);
                    }
                }
                Models.PopupNotification item = new(beItem);
                return PartialView(item);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult GetLinesLocal()
        {
            BCP.Line bcLine = new();
            IEnumerable<BEP.Line> lstLines = bcLine.List("Name");
            return Json(lstLines.Select(x => new { x.Id, x.Name }));
        }

        [AllowAnonymous()]
        public IActionResult GetNotifications()
        {
            List<BEB.Notification> items = new();
            string message = "";
            bool authenticaded = false;
            string cardCode = CardCode;
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    authenticaded = true;
                    List<BEB.Notification> lstTemp = (from i in GetItems(null, DateTime.Today, DateTime.Today, "E")
                                                      where i.Popup & (i.ListNotificationClients?.Count == 0 | (i.ListNotificationClients?.Any(x => x.CardCode == cardCode) ?? false))
                                                      select i).ToList();

                    long IdUser = UserCode;
                    List<BES.LastNotification> lstLast = GetLastNotifications() ?? new List<BES.LastNotification>();
                    foreach (var item in lstTemp)
                    {
                        BES.LastNotification beLast = (from l in lstLast where l.IdNotification == item.Id select l).FirstOrDefault();
                        if (beLast != null)
                        {
                            TimeSpan period = DateTime.Now - beLast.LogDate;
                            if (period.TotalHours >= item.Frequency)
                            {
                                item.Value = SetHTMLSafe(item.Value);
                                items.Add(item);
                                beLast.LogDate = DateTime.Now;
                                beLast.StatusType = StatusType.Update;
                            }
                        }
                        else
                        {
                            item.Value = SetHTMLSafe(item.Value);
                            items.Add(item);
                            lstLast.Add(new BES.LastNotification { StatusType = StatusType.Insert, IdNotification = item.Id, LogUser = IdUser, LogDate = DateTime.Now });
                        }
                    }

                    SaveLastNotifications(lstLast);
                    items = (from i in items orderby Guid.NewGuid() select i).ToList();
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items, authenticaded });
        }

        public IActionResult GetAllNotifications()
        {
            List<BEB.Notification> items = new();
            string message = "";
            try
            {
                string cardCode = CardCode;
                items = (from i in GetItems(null, DateTime.Today, DateTime.Today, "E")
                         where i.ListNotificationClients == null || (i.ListNotificationClients.Count == 0 | (from d in i.ListNotificationClients select d.CardCode).Contains(cardCode))
                         select i).ToList();
                items?.ForEach(x => x.Value = SetHTMLSafe(x.Value));
                //if (items != null && items.Count > 0) {
                //    foreach (var item in items) {
                //        item.Value = SetHTMLSafe(item.Value);
                //    }
                //}
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public IActionResult Edit(BEB.Notification Item, Models.NotificationFilter Filters)
        {
            string message = "";
            List<BEB.Notification> items = new();
            try
            {
                DateTime objDate = DateTime.Now;

                Item.StatusType = Item.Id > 0 ? StatusType.Update : StatusType.Insert;

                if (!Item.Popup)
                {
                    Item.Frequency = 0;
                }
                Item.LogUser = UserCode;
                Item.LogDate = objDate;

                List<Field> filters = new() { new Field("IdNotification", Item.Id) };

                //Cargando el detalle
                BCB.NotificationDetail bcDetail = new();
                IEnumerable<BEB.NotificationDetail> lstOlds, lstNews;
                lstOlds = bcDetail.List(filters, "1") ?? new List<BEB.NotificationDetail>();
                lstNews = Item.ListNotificationDetails ?? new List<BEB.NotificationDetail>();

                Item.ListNotificationDetails = new List<BEB.NotificationDetail>();

                foreach (var item in lstOlds)
                {
                    if (!(from n in lstNews select n.IdLine).Contains(item.IdLine))
                    {
                        item.StatusType = StatusType.Delete;
                        Item.ListNotificationDetails.Add(item);
                    }
                }
                foreach (var item in lstNews)
                {
                    if (!(from o in lstOlds select o.IdLine).Contains(item.IdLine))
                    {
                        item.StatusType = StatusType.Insert;
                        item.LogUser = UserCode;
                        item.LogDate = objDate;
                        Item.ListNotificationDetails.Add(item);
                    }
                }

                //Cargando los clientes
                BCB.NotificationClient bcClient = new();
                IEnumerable<BEB.NotificationClient> lstClientOlds, lstClientNews;
                lstClientOlds = bcClient.List(filters, "1") ?? new List<BEB.NotificationClient>();
                lstClientNews = Item.ListNotificationClients ?? new List<BEB.NotificationClient>();

                Item.ListNotificationClients = new List<BEB.NotificationClient>();

                foreach (var item in lstClientOlds)
                {
                    if (!(from n in lstClientNews select n.CardCode).Contains(item.CardCode))
                    {
                        item.StatusType = StatusType.Delete;
                        Item.ListNotificationClients.Add(item);
                    }
                }
                foreach (var item in lstClientNews)
                {
                    if (!(from o in lstClientOlds select o.CardCode).Contains(item.CardCode))
                    {
                        item.StatusType = StatusType.Insert;
                        item.LogUser = UserCode;
                        item.LogDate = objDate;
                        Item.ListNotificationClients.Add(item);
                    }
                }

                BCB.Notification bcNotification = new();
                bcNotification.Save(ref Item);
                items = GetItems(Filters.Name, Filters.InitialDate, Filters.FinalDate, Filters.State);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        [HttpPost()]
        public IActionResult Delete(long Id, Models.NotificationFilter Filters)
        {
            string message = "";
            List<BEB.Notification> items = new();
            DateTime objDate = DateTime.Now;
            try
            {
                BEB.Notification beNotification = new() { Id = Id, StatusType = StatusType.Delete, InitialDate = objDate, LogDate = objDate };
                BCB.Notification bcNotificaction = new();
                bcNotificaction.Save(ref beNotification);
                items = GetItems(Filters.Name, Filters.InitialDate, Filters.FinalDate, Filters.State);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, items });
        }

        [HttpPost()]
        public IActionResult SendByMail(long Id, string EMail)
        {
            string message = "";
            try
            {
                BCB.Notification bcNotification = new();
                BEB.Notification beItem = bcNotification.Search(Id);
                if (beItem != null && beItem.Id > 0)
                {
                    if (beItem.Enabled)
                    {
                        List<MailAddress> lstTo = new() { new MailAddress(EMail) };
                        string strValue = "<div>" + SetHTMLSafe(beItem.Value.Replace("/Portal/", "/").Replace("/Content/", "http://portal.dmc.bo/Content/")) + "</div>";
                        _ = SendMailAsync($"Notificación DMC: {beItem.Name}", strValue, lstTo);
                    }
                    else
                    {
                        message = "La Notificación ha sido deshabilitada.";
                    }
                }
                else
                {
                    message = "No se ha encontrado la Notificación deseada.";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult SavePhotoBase64(string ImageBase64)
        {
            string message = "", fileName = "";
            try
            {
                string strImage = ImageBase64.Split(',')[1];
                fileName = $"Photo_{UserCode}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string virtualPath = Path.Combine("images", "notification", fileName);
                var fullPath = _env.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;
                byte[] bytes = Convert.FromBase64String(strImage);
                System.IO.File.WriteAllBytes(fullPath, bytes);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message, fileName });
        }

        #endregion

        #region Private Methods

        private List<BEB.Notification> GetItems(string Name, DateTime? InitialDate, DateTime? FinalDate, string State)
        {
            BCB.Notification bcNotification = new();
            bool? boEnabled = null;
            if (!string.IsNullOrWhiteSpace(State) && State.Trim() != "A")
            {
                boEnabled = State.Trim() == "E";
            }
            List<BEB.Notification> lstItems = bcNotification.List(CardCode, Name, InitialDate, FinalDate, boEnabled, "InitialDate DESC", BEB.relNotification.NotificationClients);
            return lstItems;
        }

        #endregion

    }
}