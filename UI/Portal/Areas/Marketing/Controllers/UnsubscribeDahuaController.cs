using BEntities.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using SixLabors.ImageSharp.Processing.Processors.Overlays;
using System.Collections.Generic;
using System.Linq;
using BCG = BComponents.PiggyBank;
using BEG = BEntities.PiggyBank;

namespace Portal.Areas.Marketing.Controllers
{
	[Area("Marketing")]
	public class UnsubscribeDahuaController : BaseController
	{
		public UnsubscribeDahuaController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost()]
		public IActionResult Delete(string eMail)
		{
			string message = "";
			try
			{
				BCG.User bc = new();
				List<Field> filters = new() { new Field("EMail", eMail.Trim()), new Field("Enabled", 1), new Field(LogicalOperators.And) };
				IEnumerable<BEG.User> users = bc.List(filters, "1");
				var u = users.FirstOrDefault();
				if (u != null)
				{
					u.StatusType = BEntities.StatusType.Update;
					u.Enabled = false;
					bc.Save(ref u);
				}
				else
				{
					message = $"No existe el correo {EMail} en nuestra base de datos.";
				}
			}
			catch (System.Exception)
			{
				message = $"Se ha producido un error al eliminar la subscripción del correo {EMail}"; //GetError(ex);
			}
			return Json(new { message });
		}
	}
}
