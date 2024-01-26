using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUglify.Helpers;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCB = BComponents.Base;
using BEB = BEntities.Base;

namespace Portal.Areas.Misc.Controllers
{
    [Area("Misc")]
    [Authorize]
    public class UserHelpController : BaseController
    {
        #region Constructores

        public UserHelpController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            BCB.UserHelp bcHelp = new();
            var items = bcHelp.List("1");
            items.ForEach(x => x.Value = SetHTMLSafe(x.Value));
            return View(items.ToList());
        }

        #endregion
    }
}
