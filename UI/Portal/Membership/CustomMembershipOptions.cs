using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Membership {
    public class CustomMembershipOptions {
        public string DefaultPathAfterLogin { get; set; }
        public string DefaultPathAfterLogout { get; set; }
        public string AuthenticationType { get; set; }
    }
}
