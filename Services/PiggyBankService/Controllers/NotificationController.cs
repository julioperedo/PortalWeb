using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCA = BComponents.SAP;
using BCI = BComponents.PiggyBank;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEI = BEntities.PiggyBank;
using BEP = BEntities.Product;

namespace PiggyBankService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        #region Global Variables

        private readonly ILogger<NotificationController> _logger;

        #endregion

        #region Constructors

        public NotificationController(ILogger<NotificationController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region GETs

        [HttpGet()]
        public async Task<IActionResult> GetNotificationsAsync(long UserId = 0)
        {
            string message = "";
            try
            {
                BCI.Message bcMessage = new();
                var messages = await bcMessage.ListByUserAsync(UserId, "1");
                var items = messages.Select(x => new { x.Id, x.Title, x.Body, Date = x.Date.ToString("yyyy-MM-dd HH:mm:ss"), x.ImageUrl });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al traer las notificaciones.");
                message = ex.Message;
                var e1 = ex.InnerException;
                while (e1 != null)
                {
                    message += Environment.NewLine + e1.Message;
                    e1 = e1.InnerException;
                }
            }
            return Ok(new { message });
        }

        #endregion

        #region POSTs

        #endregion

        #region Private Methods

        #endregion
    }
}
