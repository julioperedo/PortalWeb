using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Membership {
    public class SessionDetails {
        public string Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public UserDetails User { get; set; }
    }
}
