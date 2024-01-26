using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCL = BComponents.Sales;
using BEL = BEntities.Sales;

namespace Portal.Areas.Commercial.Controllers
{
    [Area("Commercial")]
    public class BankAccountsController : BaseController
    {
        #region Constructores

        public BankAccountsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            BCL.BankAccount bcAccount = new();
            IEnumerable<BEL.BankAccount> lstAccounts = bcAccount.List("1");

            var lstItems = from a in lstAccounts
                           group a by a.Subsidiary into g
                           select new Administration.Models.BankAccountGroup
                           {
                               Name = g.Key,
                               Items = g.Select(x => new Administration.Models.BankAccount(x)).ToList()
                           };
            return View(lstItems);
        }

        #endregion
    }
}
