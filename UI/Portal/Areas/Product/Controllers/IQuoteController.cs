using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using Portal.Models;
using BCA = BComponents.SAP;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class IQuoteController : BaseController
    {
        private readonly IConfiguration config;

        #region Constructors

        public IQuoteController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            config = configuration;
        }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                var settings = config.GetSection("iQuoteSettings").Get<iQuoteSettings>();
                ViewBag.GateKeeperUrl = settings.GateKeeperUrl;

                long intUserID = UserCode;
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(intUserID);
                Models.CustomerData data = new()
                {
                    host = "DDMBO3800",
                    token = "9fe238296f7aa08b43bf5e592a0b4e47",
                    mfr = "HPE",
                    clearSession = "Y",
                    reff = "http://www.dmc.bo/",
                    BasketURL = "http://www.dmc.bo/",
                    OrderEntry = "Y",
                    Valid = true
                };
                if (beUser != null && beUser.Id > 0 && beUser.EMail != null && beUser.EMail.Trim().Length > 0 && beUser.Phone != null && beUser.Phone.Trim().Length > 0)
                {
                    BCA.Client bcClient = new();
                    BEA.Client beClient = bcClient.Search(beUser.CardCode);
                    data.uEmail = beUser.EMail;
                    data.uName = beUser.Name;
                    data.uTel = beUser.Phone;
                    data.cAccountNum = beUser.CardCode;
                    data.CName = beClient.CardName;
                    data.cPCode = beClient.NIT;
                    data.SessionID = beUser.CardCode + DateTime.Now.Ticks.ToString().Substring(2, 4);
                }
                else
                {
                    data.Valid = false;
                }
                return View(data);
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        #endregion

    }
}