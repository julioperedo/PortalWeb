using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class BillItem : BEntity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string ExternalCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceLocal { get; set; }
        public decimal Subtotal { get; set; }
        public decimal SubtotalLocal { get; set; }
        public string Unit { get; set; }
    }
}
