using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class PaymentItem: BEntity
    {
        public int LineNum { get; set; }
        public string Description { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Total { get; set; }
        public PaymentItem() { }
    }
}
