using Microsoft.AspNetCore.Mvc;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    public class PiggyBankPrivacyPolicyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
