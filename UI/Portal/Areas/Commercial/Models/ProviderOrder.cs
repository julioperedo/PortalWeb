using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Commercial.Models
{
    public class ProviderOrder
    {
        public string Subsidiary { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ReferenceOrder { get; set; }
        public string Warehouse { get; set; }
        public string SellerCode { get; set; }
        public string Terms { get; set; }
        public decimal OtherCosts { get; set; }
        public decimal Total { get; set; }
        public string State { get; set; }
        public int Quantity { get; set; }
        public int OpenQuantity { get; set; }
        public IEnumerable<ProviderOrderItem> Items { get; set; }
    }

    public class ProviderOrderItem
    {
        public int? BillNumber { get; set; }
        public System.DateTime? BillDate { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
    }
}
