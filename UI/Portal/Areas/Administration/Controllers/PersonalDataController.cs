using BEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using Portal.Membership;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

namespace Portal.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize]
    public class PersonalDataController : BaseController
    {

        #region Global Variables

        private readonly IWebHostEnvironment _env;

        #endregion

        #region Constructors

        public PersonalDataController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { _env = env; }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            BCS.User bcUser = new();
            var user = bcUser.Search(UserCode, BES.relUser.Profile, BES.relUser.UserDatas);
            user.Password = Crypt.Decrypt(user.Password);
            user.ClientName = User?.FindFirst("CardName")?.Value ?? "";
            if (user.ListUserDatas?.Count > 0)
            {
                foreach (var x in user.ListUserDatas)
                {
                    x.Signature = SetHTMLSafe(x.Signature);
                    x.SAPUser ??= "";
                    x.SAPPassword = String.IsNullOrEmpty(x.SAPPassword) ? "" : Crypt.Decrypt(x.SAPPassword);
                }
            }
            else
            {
                user.ListUserDatas = new List<BES.UserData> { new BES.UserData() };
            }
            return View(user);
        }

        public IActionResult ToValidatePassword()
        {
            BCS.User bcUser = new BCS.User();
            var user = bcUser.Search(UserCode);
            return Json(new { result = Crypt.Decrypt(user.Password) });
        }

        public async Task<IActionResult> GetSAPUsersAsync()
        {
            BCA.User bcUser = new();
            IEnumerable<BEA.User> items = await bcUser.ListAsync(null, "Name");
            return Json(items);
        }

        #endregion

        #region POSTs

        [HttpPost("UpdateData")]
        public IActionResult UpdateData(BES.User UserData)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new();
                UserData.Password = Crypt.Encrypt(UserData.Password);
                UserData.Login = UserData.EMail;
                UserData.LogUser = UserCode;
                UserData.LogDate = DateTime.Now;
                UserData.StatusType = StatusType.Update;
                bcUser.Save(ref UserData);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost("UpdatePassword")]
        public IActionResult UpdatePassword(string NewPassword)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new();
                var user = bcUser.Search(UserCode);
                user.Password = Crypt.Encrypt(NewPassword);
                user.LogUser = UserCode;
                user.LogDate = DateTime.Now;
                user.StatusType = StatusType.Update;
                bcUser.Save(ref user);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost("UpdateOtherData")]
        public IActionResult UpdateOtherData(BES.UserData UserData)
        {
            string message = "";
            try
            {
                BCS.UserData bcUserData = new();

                UserData.IdUser = UserCode;
                UserData.Signature ??= "";
                UserData.SAPUser ??= "";
                UserData.SAPPassword = string.IsNullOrEmpty(UserData.SAPPassword) ? "" : Crypt.Encrypt(UserData.SAPPassword);
                UserData.LogUser = UserCode;
                UserData.LogDate = DateTime.Now;
                UserData.StatusType = UserData.Id == 0 ? StatusType.Insert : StatusType.Update;

                bcUserData.Save(ref UserData);
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
            string strMessage = "", fileName = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(ImageBase64))
                {
                    BCS.User bcUser = new();
                    BES.User beUser = bcUser.Search(UserCode);
                    string strImage = ImageBase64.Split(',')[1];
                    fileName = $"Photo_{beUser.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                    string virtualPath = Path.Combine("images", "userdata", fileName);
                    var fullPath = _env.WebRootFileProvider.GetFileInfo(virtualPath).PhysicalPath;
                    byte[] bytes = Convert.FromBase64String(strImage);
                    System.IO.File.WriteAllBytes(fullPath, bytes);
                    beUser.Picture = fileName;
                    beUser.StatusType = StatusType.Update;
                    bcUser.Save(ref beUser);
                    this.HttpContext.Session.Set("Picture", fileName);
                }
            }
            catch (Exception ex)
            {
                strMessage = GetError(ex);
            }
            return Json(new { Message = strMessage, FileName = fileName });
        }

        #endregion

    }
}