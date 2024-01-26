using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP {
    public class ProviderBillItem : BEntity {
        public string Subsidiary { get; set; }
        public int DocNumber { get; set; }
        public int Quantity { get; set; }
        public string ItemCode { get; set; }
        public string BrandCode { get; set; }
        public string Description { get; set; }
        public string TaxCode { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
