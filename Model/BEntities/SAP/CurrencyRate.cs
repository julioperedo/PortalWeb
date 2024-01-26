using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class CurrencyRate: BEntity
    {
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}
