using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEA = BEntities.SAP;

namespace Portal.Areas.Finance.Models
{
    public class Flow
    {
        public IEnumerable<BEA.BankResume> Banks { get; set; }
        public BalanceSubsidiary BalanceSA { get; set; }
        public BalanceSubsidiary BalanceIQ { get; set; }
        public BalanceSubsidiary BalanceLA { get; set; }
    }

    public class BalanceSubsidiary
    {
        public string Name { get; set; }
        public IEnumerable<BEA.BalanceDetail> Clients { get; set; }
        public IEnumerable<BEA.BalanceDetail> Providers { get; set; }

        public BalanceSubsidiary()
        {
            Clients = new List<BEA.BalanceDetail>();
            Providers = new List<BEA.BalanceDetail>();
        }
    }
}
