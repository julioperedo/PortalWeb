using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Administration.Models
{
    public class UserRequest
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string ClientName { get; set; }
        public string Position { get; set; }
        public string City { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }
        public string CardCode { get; set; }
        public DateTime RequestDate { get; set; }
        public long StateIdc { get; set; }
        public string Comments { get; set; }
        public bool HasOrders { get; set; }
        public bool ClientHasUsers { get; set; }
        public bool ClientHasEnabledUsers { get; set; }
        public bool SameClient { get; set; }
        public bool ValidEMail { get; set; }
        public bool InBlackList { get; set; }
        public bool ValidNames { get; set; }
        public bool Enabled { get; set; }
        public bool AccountHolder { get; set; }
        public long IdUser { get; set; }
        public string ValidEMails { get; set; }
        public string ValidCardNames { get; set; }
        public string SellerName { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsNew { get; set; }
    }
}
