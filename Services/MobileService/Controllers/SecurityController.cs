using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Mvc;
using MobileService.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCD = BComponents.AppData;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BCV = BComponents.Visits;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BED = BEntities.AppData;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;
using BEV = BEntities.Visits;

namespace MobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : BaseController
    {
        #region GETs

        [HttpGet("userlogin")]
        public IActionResult UserLogin(string UserName, string Password, string CardCode, long? IdProfile)
        {
            string message = "", strCardCode = "", strCardName = "";
            bool boProductData = false, boOrdersData = false, boFinancialData = false, boDashboardData = false;
            IEnumerable<long> lstLines = Enumerable.Empty<long>();
            try
            {
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search2(UserName, Encryption.Encrypt(Password), BES.relUser.Profile, BES.relProfile.ProfilePages, BES.relUser.UserClients, BES.relUser.UserProfiles, BES.relUserProfile.Profile, BES.relProfile.ProfileCharts, BES.relProfile.ProfileActivitys);
                if (beUser == null || beUser.Id <= 0)
                {
                    message = "Usuario y/o contraseña incorrectos.";
                }
                else
                {
                    if (!beUser.Enabled)
                    {
                        message = "Usuario deshabilitado.";
                    }
                    else
                    {
                        strCardCode = string.IsNullOrWhiteSpace(CardCode) ? beUser.CardCode : CardCode.Trim();
                        BCA.Client bcClient = new();
                        BEA.Client beClient = bcClient.Search(strCardCode);
                        strCardName = beClient.CardName;
                        if (IdProfile.HasValue && IdProfile.Value > 0)
                        {
                            BCS.Profile bcProfile = new();
                            beUser.Profile = bcProfile.Search(IdProfile.Value, BES.relProfile.ProfilePages, BES.relProfile.ProfileCharts);
                        }
                        if (beUser.Profile?.ListProfilePages?.Count > 0)
                        {
                            boProductData = (from p in beUser.Profile.ListProfilePages where p.IdPage == 7 | p.IdPage == 16 select p).Any();
                            boFinancialData = (from p in beUser.Profile.ListProfilePages where p.IdPage == 9 | p.IdPage == 17 select p).Any();
                            boOrdersData = (from p in beUser.Profile.ListProfilePages where p.IdPage == 10 select p).Any();
                            if (beUser.Profile?.ListProfileCharts?.Count > 0)
                            {
                                boDashboardData = (from p in beUser.Profile.ListProfileCharts where p.IdChart == 7 select p).Any();
                            }
                        }

                        BCP.Line bcLine = new();
                        IEnumerable<BEP.Line> lstFullLines = bcLine.ListForPriceList("Name");

                        BCS.LineNotAllowed bcNotAllowed = new();
                        var lstFilter = new List<Field> { new Field { Name = "CardCode", Value = strCardCode } };
                        var lstNotAllowed = bcNotAllowed.List(lstFilter, "1") ?? new List<BES.LineNotAllowed>();
                        lstLines = (from l in lstFullLines
                                    where !(from n in lstNotAllowed select n.IdLine).Contains(l.Id)
                                    select l.Id).Distinct();

                        if (beUser.ListUserClients?.Count > 0)
                        {
                            if (!(from c in beUser.ListUserClients select c.CardCode).Contains(beUser.CardCode))
                            {
                                beClient = bcClient.Search(beUser.CardCode);
                                beUser.ListUserClients.Add(new BES.UserClient { CardCode = beClient.CardCode, CardName = beClient.CardName });
                            }
                        }
                        else
                        {
                            beUser.ListUserClients = new List<BES.UserClient>();
                        }
                        var clients = (from i in beUser.ListUserClients orderby i.CardName select new { id = i.CardCode, name = i.CardName }).Distinct().ToList();
                        if (beUser.ListUserProfiles?.Count > 0)
                        {
                            if (!(from p in beUser.ListUserProfiles select p.IdProfile).Contains(beUser.IdProfile))
                            {
                                var bcProfile = new BCS.Profile();
                                var beProfile = bcProfile.Search(beUser.IdProfile);
                                beUser.ListUserProfiles.Add(new BES.UserProfile { Profile = beProfile });
                            }
                        }
                        else
                        {
                            beUser.ListUserProfiles = new List<BES.UserProfile>();
                        }
                        var profiles = (from p in beUser.ListUserProfiles orderby p.Profile.Name select new { id = p.Profile.Id, name = p.Profile.Name }).Distinct().ToList();

                        BCS.SessionHistory bcLog = new();
                        BES.SessionHistory beLog = new() { StatusType = BEntities.StatusType.Insert, IdUser = beUser.Id, Description = "Inicio de Sessión Aplicación Móvil", LogDate = DateTime.Now };
                        bcLog.Save(ref beLog);
                        List<string> lstActions = new();
                        if (boProductData) lstActions.Add("productData");
                        if (boOrdersData) lstActions.Add("ordersData");
                        if (boFinancialData) lstActions.Add("finantialData");
                        if (boDashboardData) lstActions.Add("dashboardData");
                        if (beUser?.Profile?.ListProfileActivitys?.Count(x => x.IdActivity == 15 & x.Permission > 0) > 0) lstActions.Add("seeMargin");

                        var user = new
                        {
                            id = beUser.Id,
                            firstName = beUser.FirstName ?? "",
                            lastName = beUser.LastName ?? "",
                            cardCode = strCardCode,
                            cardName = strCardName,
                            address = beUser.Address ?? "",
                            phone = beUser.Phone ?? "",
                            position = beUser.Position ?? "",
                            profile = beUser.Profile?.Name ?? "",
                            picture = beUser.Picture ?? "",
                            actions = lstActions,
                            lines = lstLines
                        };
                        return Ok(new { message, user, clients, profiles });
                    }
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpGet("assignedprofiles")]
        public IActionResult AssignedProfiles(string UserName, string Password, string CardCode)
        {
            List<BES.Profile> lstProfiles = new List<BES.Profile>();
            string message = "";
            try
            {
                BCS.User bcUser = new BCS.User();
                BES.User beUser = bcUser.Search(UserName, Encryption.Encrypt(Password), BES.relUser.UserProfiles, BES.relUserProfile.Profile);
                string strCardCode = !string.IsNullOrWhiteSpace(CardCode) ? CardCode : beUser.CardCode;
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
                    beProfile = bcProfile.Search(boLocalSelected ? 5 : 3);
                    lstProfiles.Add(beProfile);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    message += Environment.NewLine + ex.Message;
                }
            }
            var items = (from i in lstProfiles orderby i.isExternalCapable, i.Name select new { id = i.Id, name = i.Name }).ToList();
            return Ok(new { message, items });
        }

        [HttpGet("isuservalid")]
        public IActionResult IsUserValid(long Id)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new BCS.User();
                BES.User beUser = bcUser.Search(Id);
                bool valid = false;
                string picture = "";
                if (beUser != null)
                {
                    valid = beUser.Enabled;
                    picture = beUser.Picture ?? "";
                }
                return Ok(new { message, valid, picture });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        //[HttpGet]
        //public IActionResult SyncData(string App = "P") {
        //    string strMessage = "";
        //    try {
        //        BCD.SyncData bcSync = new BCD.SyncData();
        //        List<BED.SyncData> lstData = bcSync.List("1");
        //        var lstItems = (from d in lstData where d.App.Contains(App) select new { Id = d.Id, Name = d.Name, Version = d.Version }).ToList();
        //        return Ok(new { Message = strMessage, Items = lstItems });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Ok(new { Message = strMessage });
        //}

        //[HttpGet]
        //public IActionResult GetAllowedLines(long UserId) {
        //    string strMessage = "";
        //    try {
        //        var lstResult = new List<long>();
        //        BCS.User bcUser = new BCS.User();
        //        var beUser = bcUser.Search(UserId);
        //        if(beUser != null) {
        //            BCP.Line bcLine = new BCP.Line();
        //            List<BEP.Line> lstFullLines = bcLine.ListForPriceList("Name");

        //            BCS.LineNotAllowed bcNotAllowed = new BCS.LineNotAllowed();
        //            var lstFilter = new List<Field> { new Field { Name = "CardCode", Value = beUser.CardCode } };
        //            var lstNotAllowed = bcNotAllowed.List(lstFilter, "1");
        //            if(lstNotAllowed == null) {
        //                lstNotAllowed = new List<BES.LineNotAllowed>();
        //            }

        //            lstResult = (from l in lstFullLines
        //                         where !(from n in lstNotAllowed select n.IdLine).Contains(l.Id)
        //                         select l.Id).Distinct().ToList();
        //        }
        //        return Ok(new { Message = strMessage, Items = lstResult });
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Ok(new { Message = strMessage });
        //}

        #endregion

        #region POSTs

        [HttpPost("savetoken")]
        public IActionResult SaveToken(long IdUser, string Token)
        {
            string message = "";
            try
            {
                BCD.UserToken bcToken = new();
                BED.UserToken beToken = bcToken.SearchByUser(IdUser);
                beToken ??= new BED.UserToken { IdUser = IdUser, LogDate = DateTime.Now, LogUser = IdUser };
                beToken.Token = Token;
                beToken.StatusType = beToken.Id == 0 ? StatusType.Insert : StatusType.Update;
                bcToken.Save(ref beToken);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        //[HttpGet]
        //public IActionResult SaveCheckPoint(long IdGuard, double Latitude, double Longitude, String Date, double Altitude, double Accuracy, string Provider, string Type, string PointName) {
        //    string strMessage = "OK";
        //    try {
        //        BCD.CheckPoint bcCheckpoint = new BCD.CheckPoint();
        //        BED.CheckPoint bePoint = new BED.CheckPoint {
        //            IdGuard = IdGuard, Latitude = Latitude, Longitude = Longitude, Altitude = Altitude, CheckDate = DateTime.ParseExact(Date, "yyyy-MM-dd HH:mm:ss", null), Accuracy = Accuracy, Provider = Provider, Type = Type,
        //            PointName = PointName, LogUser = 0, LogDate = DateTime.Now, StatusType = BEntities.StatusType.Insert
        //        };
        //        bcCheckpoint.Save(ref bePoint);
        //    } catch(Exception ex) {
        //        strMessage = GetError(ex);
        //    }
        //    return Ok(new { message = strMessage });
        //}

        [HttpPost("saveuserdata")]
        public IActionResult SaveUserData(long IdUser, string FirstName, string LastName, string Address, string Phone, string Position)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new BCS.User();
                BES.User beUser = bcUser.Search(IdUser);
                beUser.FirstName = FirstName;
                beUser.LastName = LastName;
                beUser.Address = Address;
                beUser.Phone = Phone;
                beUser.Position = Position;
                beUser.LogUser = IdUser;
                beUser.LogDate = DateTime.Now;
                beUser.StatusType = BEntities.StatusType.Update;

                bcUser.Save(ref beUser);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Ok(new { message });
        }

        [HttpPost("changepassword")]
        public IActionResult ChangePassword(long IdUser, string OldPassword, string NewPassword)
        {
            string message = "";
            try
            {
                BCS.User bcUser = new BCS.User();
                BES.User beUser = bcUser.Search(IdUser);
                if (Encryption.Encrypt(OldPassword) == beUser.Password)
                {
                    beUser.Password = Encryption.Encrypt(NewPassword);
                    beUser.LogUser = IdUser;
                    beUser.LogDate = DateTime.Now;
                    beUser.StatusType = BEntities.StatusType.Update;

                    bcUser.Save(ref beUser);
                }
                else
                {
                    message = "wrongpassword";
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

        #endregion
    }
}
