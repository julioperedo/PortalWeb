using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class PriceListCategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
