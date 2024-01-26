using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class ProviderOrder : BEntity
    {
        #region Properties

        public string Subsidiary { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string State { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public string ReferenceOrder { get; set; }
        public string Warehouse { get; set; }
        public string SellerCode { get; set; }
        public string Terms { get; set; }
        public decimal OtherCosts { get; set; }
        public decimal Total { get; set; }
        public int? BillNumber { get; set; }
        public DateTime? BillDate { get; set; }
        public string BillingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public int Quantity { get; set; }
        public int OpenQuantity { get; set; }
        public string Comments { get; set; }
        public string DailyComments { get; set; }
        public string Transport { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }

        #endregion

        #region Contructors

        #endregion
    }
}
