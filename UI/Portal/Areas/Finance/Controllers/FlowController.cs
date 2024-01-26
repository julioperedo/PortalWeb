using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;

namespace Portal.Areas.Finance.Controllers
{
	[Area("Finance")]
	[Authorize]
	public class FlowController : BaseController
	{
		#region Constructores

		public FlowController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

		#endregion

		#region GETs
		public IActionResult Index()
		{
			if (IsAllowed(this))
			{
				Models.Flow beItem = new();
				BCA.BalanceDetail bcBalance = new();
				BCA.BankResume bcBank = new();

				beItem.Banks = bcBank.List(null, "ORDEN, ActId");
				beItem.BalanceSA = new Models.BalanceSubsidiary { Name = "DMC SA", Clients = bcBalance.ListClientsSA(null, "Days DESC"), Providers = bcBalance.ListProvidersSA(null, "Days DESC") };
				beItem.BalanceIQ = new Models.BalanceSubsidiary { Name = "DMC IQQ", Clients = bcBalance.ListClientsIQ(null, "Days DESC"), Providers = bcBalance.ListProvidersIQ(null, "Days DESC") };
				beItem.BalanceLA = new Models.BalanceSubsidiary { Name = "DMC LA", Clients = bcBalance.ListClientsLA(null, "Days DESC"), Providers = bcBalance.ListProvidersLA(null, "Days DESC") };
				return View(beItem);
			}
			else
			{
				return RedirectToAction("Denied");
			}
		}

		#endregion

		#region Private Methods

		#endregion
	}
}