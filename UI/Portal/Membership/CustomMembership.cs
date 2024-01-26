using BEntities.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Portal.Membership;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCA = BComponents.SAP;
using BCP = BComponents.AppData;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEP = BEntities.AppData;
using BES = BEntities.Security;

namespace Portal.Membership
{
    public class CustomMembership : ICustomMembership
    {
        private IHttpContextAccessor _context;
        //private readonly Dictionary<string, string> connexions;
        public CustomMembershipOptions Options { get; private set; }

        public CustomMembership(IHttpContextAccessor context, CustomMembershipOptions options, IConfiguration configuration)
        {
            _context = context;
            Options = options;
            //config = configuration;
            //connexions = new Dictionary<string, string> {
            //    ["Negocio"] = configuration.GetConnectionString("Negocio"),
            //    ["SAPSA"] = configuration.GetConnectionString("SAPSA")
            //};
        }

        public async Task<SessionDetails> GetSessionDetailsAsync(ClaimsPrincipal principal)
        {
            var sessionId = principal.FindFirstValue("sessionId");
            if (sessionId == null)
            {
                return null;
            }

            BCS.LoginSession bcSession = new();
            var session = await bcSession.GetSessionAsync(long.Parse(sessionId));

            BCS.User bcUser = new();
            var user = await bcUser.GetUserAsync(session.UserId);

            return new SessionDetails { Id = session.Id.ToString(), CreationTime = session.LoginTime, LogoutTime = session.LogoutTime, User = new UserDetails { Id = user.Id.ToString(), Username = user.Name, Email = user.EMail } };
        }

        public async Task<LoginResult> LoginAsync(string userName, string password)
        {
            BCS.User bcUser = new();
            var user = await bcUser.GetUserByUsernameAsync(userName, Crypt.Encrypt(password), true);
            if (user == null)
            {
                return LoginResult.GetFailed("Usuario y/o contraseña incorrectos.");
            }

            if (!user.Enabled)
            {
                return LoginResult.GetFailed($"Usuario deshabilitado. Si desea habilitarlo se acaba de habilitar un botón en la parte inferior para solicitarlo.");
            }

            if (user.ListUserClients?.Count > 0)
            {
                if (!user.ListUserClients.Select(x => x.CardCode).Contains(user.CardCode))
                {
                    BCA.Client bcClient = new();
                    var client = bcClient.Search(user.CardCode);
                    user.ListUserClients.Insert(0, new BES.UserClient { CardCode = client.CardCode, CardName = client.CardName });
                }
            }
            var clients = user.ListUserClients?.Select(i => new BEA.Client { CardCode = i.CardCode, CardName = i.CardName }).ToList() ?? new List<BEA.Client>();
            var profiles = user.ListUserProfiles?.Select(i => i.Profile).ToList() ?? new List<BES.Profile>();

            bool logedIn = false;
            if (clients.Count == 0 & profiles.Count == 0)
            {
                if (!string.IsNullOrWhiteSpace(user.CardCode))
                {
                    BCA.Client bcClient = new();
                    var client = bcClient.Search(user.CardCode);
                    if (!string.IsNullOrWhiteSpace(client?.CardName))
                    {
                        user.ClientName = client.CardName;
                    }
                }
                await SignInAsync(user);
                logedIn = true;
            }
            return LoginResult.GetSuccess(clients, profiles, logedIn);
        }

        public async Task<LoginResult> LoginAsync2(string userName, string password, string cardCode, long? idProfile)
        {
            BCS.User bcUser = new();
            var user = await bcUser.GetUserByUsernameAsync(userName, Crypt.Encrypt(password), false);
            if (user == null)
            {
                return LoginResult.GetFailed("Usuario y/o contraseña incorrectos.");
            }

            if (!user.Enabled)
            {
                return LoginResult.GetFailed($"Usuario deshabilitado. Si desea habilitarlo se acaba de habilitar un botón en la parte inferior para solicitarlo.");
            }

            if (!string.IsNullOrWhiteSpace(cardCode))
            {
                user.CardCode = cardCode;
                user.ClientName = cardCode;
            }
            if (idProfile.HasValue) user.IdProfile = idProfile.Value;

            if (!string.IsNullOrWhiteSpace(user.CardCode))
            {
                user.ClientName = user.CardCode;
                try
                {
                    BCP.Client bcClient = new();
                    var client = bcClient.Search(user.CardCode) ?? new BEP.Client();
                    user.ClientName = client?.Name ?? "";
                }
                catch (Exception) { }
            }
            await SignInAsync(user);
            return LoginResult.GetSuccess(null, null, true);
        }

        public async Task LogoutAsync()
        {
            await _context.HttpContext.SignOutAsync();

            var sessionId = _context.HttpContext.User.FindFirstValue("sessionId");
            if (sessionId != null)
            {
                BCS.LoginSession bcSession = new BCS.LoginSession();
                var session = await bcSession.GetSessionAsync(long.Parse(sessionId));
                if (session != null)
                {
                    session.StatusType = BEntities.StatusType.Update;
                    session.LogoutTime = DateTime.Now;
                    await bcSession.SaveAsync(session);
                }
            }
        }

        public async Task<RegisterResult> RegisterAsync(string userName, string email, string password)
        {
            var user = new BES.User { Login = userName, EMail = email, Password = password };

            //try {
            //    user = await _persistence.Users.CreateUserAsync(user);
            //} catch(Exception ex) {
            //    //TODO reduce breadth of exception statement
            //    return RegisterResult.GetFailed("Username is already in use");
            //}

            await SignInAsync(user);

            return RegisterResult.GetSuccess();
        }

        public async Task<bool> ValidateLoginAsync(ClaimsPrincipal principal)
        {
            var sessionId = principal.FindFirstValue("sessionId");
            if (sessionId == null)
            {
                return false;
            }

            BCS.LoginSession bcSession = new BCS.LoginSession();
            BES.LoginSession session = await bcSession.GetSessionAsync(long.Parse(sessionId));
            if (session != null && session.LogoutTime.HasValue)
            {
                return false;
            }

            // add in options like updating it with a last seen time, expiration, etc
            // add in options like IP Address roaming check

            return true;
        }

        private async Task SignInAsync(BES.User user)
        {
            //key the login to a server-side session id to make it easy to invalidate later
            var session = new BES.LoginSession { UserId = user.Id, LoginTime = DateTime.Now, StatusType = BEntities.StatusType.Insert };
            BCS.LoginSession bcSession = new();
            session.Id = await bcSession.SaveAsync(session);

            BCS.User bcUser = new();
            await bcUser.UnforceToLogOffAsync(user.Id);

            var identity = new ClaimsIdentity(Options.AuthenticationType);
            identity.AddClaim(new Claim("sessionId", session.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            identity.AddClaim(new Claim("ProfileCode", user.IdProfile.ToString()));
            identity.AddClaim(new Claim("UserCode", user.Id.ToString()));
            identity.AddClaim(new Claim("CardCode", user.CardCode));
            identity.AddClaim(new Claim("CardName", user.ClientName ?? ""));
            identity.AddClaim(new Claim("Login", user.Login));
            //identity.AddClaim(new Claim("Picture", user.Picture));
            await _context.HttpContext.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(20) });
        }

    }
}
