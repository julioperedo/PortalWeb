using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class BalanceDetail : BEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string Term { get; set; }
        public DateTime Expires { get; set; }
        public int Days { get; set; }
        public decimal Balance { get; set; }
    }
}
