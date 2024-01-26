using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class ClientResumeDebt : BEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Difference { get; set; }
        public ClientResumeDebt() { }
    }
}
