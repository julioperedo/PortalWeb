using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP {
    public class ProviderBill : BEntity {
        public string Subsidiary { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string OrderNumber { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ProviderAddress { get; set; }
        public string BillingAddress { get; set; }
        public string Warehouse { get; set; }
        public string Reference { get; set; }
        public string SellerCode { get; set; }
        public string TermConditions { get; set; }
        public decimal OtherCosts { get; set; }
        public decimal Total { get; set; }
        public decimal PaidToDate { get; set; }
        public string Comments { get; set; }
        public string DailyComments { get; set; }
    }
}
