using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class ClientResume : BEntity {
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string BusinessName { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal BalanceTotal { get; set; }
        public decimal BalanceSA { get; set; }
        public decimal BalanceLA { get; set; }
        public decimal BalanceIQQ { get; set; }
        public decimal OrdersTotal { get; set; }
        public decimal OrdersSA { get; set; }
        public decimal OrdersLA { get; set; }
        public decimal OrdersIQQ { get; set; }
        public string TermsSA { get; set; }
        public string TermsLA { get; set; }
        public string TermsIQQ { get; set; }

        public ClientResume() { }
    }
}
