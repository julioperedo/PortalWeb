using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using BCP = BComponents.AppData;
using BEP = BEntities.AppData;
using System.Linq;

namespace MobileService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        #region Global Variables
        #endregion

        #region Constructors
        #endregion

        #region GETs

        [HttpGet()]
        public async Task<IActionResult> GetNotificationsAsync(long UserId)
        {
            string message = "";
            try
            {
                BCP.Message bcMessage = new();
                var messages = await bcMessage.ListByUserAsync(UserId, "1");
                var items = messages.Select(x => new { x.Id, x.Title, x.Body, Date = x.Date.ToString("yyyy-MM-dd HH:mm:ss"), x.ImageUrl });
                return Ok(new { message, items });
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error al traer las notificaciones.");
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
