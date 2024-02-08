using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class LoanController : BaseController
    {
        #region Constructors

        public LoanController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region POSTs

        #endregion

        #region Private Methods

        #endregion
    }
}
