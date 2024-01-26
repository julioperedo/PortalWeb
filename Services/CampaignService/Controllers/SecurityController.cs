using BEntities.Filters;
using CampaignService.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCN = BComponents.Campaign;
using BEN = BEntities.Campaign;

namespace CampaignService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        #region Global Variables

        private readonly ILogger<SecurityController> _logger;

        #endregion

        #region Constructors
        public SecurityController(ILogger<SecurityController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region GETs

        [HttpGet]
        public IActionResult RegisterUser(string Name, string StoreName, string EMail, string Password, string City, string Address, string Phone, long IdCampaign)
        {
            long id = 0;
            string message = "";
            bool alreadyExist = false;
            try
            {
                BCN.User bcUser = new();
                List<Field> filters = new() { new Field("LOWER(EMail)", EMail.ToLower()) };
                var existingUsers = bcUser.List(filters, "1");
                alreadyExist = existingUsers.Any();
                if (!alreadyExist)
                {
                    BEN.User user = new(Name, StoreName, EMail, Encryption.Encrypt(Password), City, Address, Phone, IdCampaign, true);
                    bcUser.Save(ref user);
                    id = user.Id;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el usuario");
                message = ex.Message;
                var e1 = ex.InnerException;
                while (e1 != null)
                {
                    message += Environment.NewLine + e1.Message;
                    e1 = e1.InnerException;
                }
            }
            return Ok(new { message, id, alreadyExist });
        }

        [HttpGet]
        public IActionResult ValidateEmail(string Email, long IdCampaign)
        {
            string message = "";
            BEN.User user = new();
            try
            {
                BCN.User bcUser = new();
                List<Field> filters = new() {
                    new Field("LOWER(EMail)", Email.ToLower()), new Field("Enabled", 1), new Field("IdCampaign", IdCampaign),
                    new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                };
                var results = bcUser.List(filters, "1");
                if (results.Any()) user = results.First();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar el usuario (E-Mail)");
                message = ex.Message;
                var e1 = ex.InnerException;
                while (e1 != null)
                {
                    message += Environment.NewLine + e1.Message;
                    e1 = e1.InnerException;
                }
            }
            return Ok(new { message, user });
        }

        [HttpGet]
        public async Task<IActionResult> UserLoginAsync(string Email, string Password, long IdCampaign)
        {
            string message = "";
            try
            {
                BCN.User bcUser = new();
                BEN.User? u = await bcUser.SearchAsync(Email, Encryption.Encrypt(Password), IdCampaign);
                if (u != null)
                {
                    var user = new { u.Id, u.Name, u.StoreName, u.EMail, u.City, u.Address, u.Phone, u.Enabled };
                    return Ok(new { message, user });
                }
                else
                {
                    return Ok(new { message, user = u });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar si el usuario es válido");
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

        [HttpGet]
        public IActionResult IsUserValid(long Id)
        {
            string message = "";
            try
            {
                BCN.User bcUser = new();
                BEN.User beUser = bcUser.Search(Id);
                bool valid = beUser?.Enabled ?? false;
                return Ok(new { message, valid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar si el usuario es válido");
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
    }
}
