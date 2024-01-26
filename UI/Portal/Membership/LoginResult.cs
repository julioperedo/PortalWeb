using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BES = BEntities.Security;
using BEA = BEntities.SAP;

namespace Portal.Membership
{
    public class LoginResult
    {
        public bool Failed { get; set; }
        public bool LoggedIn { get; set; }
        public string FailMessage { get; set; }
        public List<BES.Profile> AssignedProfiles { get; set; }
        public List<BEA.Client> AssignedClients { get; set; }
        public bool IsMultiClient { get { return AssignedClients?.Count > 0; } }
        public bool IsMultiProfile { get { return AssignedProfiles?.Count > 0; } }

        public static LoginResult GetFailed(string Message)
        {
            return new LoginResult() { Failed = true, FailMessage = Message };
        }

        public static LoginResult GetSuccess(List<BEA.Client> Clients, List<BES.Profile> Profiles, bool LoggedIn)
        {
            return new LoginResult() { Failed = false, AssignedProfiles = Profiles, AssignedClients = Clients, LoggedIn = LoggedIn };
        }
    }
}
