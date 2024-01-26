using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class BankResume : BEntity
    {
        public string Subsidiary { get; set; }
        public string Account { get; set; }
        public decimal SAPUsd { get; set; }
        public decimal SAPBs { get; set; }
        public decimal SAPCLP { get; set; }
        public decimal ExtractUsd { get; set; }
        public decimal ExtractBs { get; set; }
        public decimal ExtractCLP { get; set; }
        public decimal Checks { get; set; }

        public BankResume() { }
    }
}
