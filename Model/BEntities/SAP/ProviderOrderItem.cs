using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class ProviderOrderItem : BEntity
    {
        public string Subsidiary { get; set; }
        public int DocNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BrandCode { get; set; }
        public string ItemLine { get; set; }
        public string ItemCategory { get; set; }
        public string ItemSubcategory { get; set; }
        public int Quantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int OpenQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public decimal OpenSubtotal { get; set; }
    }
}
