using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class ResumeSale
    {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public decimal Total { get; set; }
        public decimal Margin { get; set; }
        public decimal TaxlessTotal { get; set; }
        public decimal MarginPercentage { get; set; }
    }
}
