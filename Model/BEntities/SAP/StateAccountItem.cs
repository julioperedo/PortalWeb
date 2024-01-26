using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class StateAccountItem : BEntity
    {
        public string Subsidiary { get; set; }
        public string Type { get; set; }
        public int DocNum { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string Warehouse { get; set; }
        public int SaleNote { get; set; }
        public string SaleOrder { get; set; }
        public DateTime DocDate { get; set; }
        public string Terms { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Balance { get; set; }
        public decimal Margin { get; set; }
        public decimal PercetageMargin { get; set; }
        public decimal TaxlessToral { get; set; }
        public int Days { get; set; }
        public string Hold { get; set; }
        public string State { get; set; }
        public string ClientOrder { get; set; }
        public decimal Total { get; set; }
        public string SellerName { get; set; }
        public string SellerCode { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public DateTime? PickupDate { get; set; }
        public string IsDeliveryNote { get; set; }
        public int BillSerie { get; set; }
        public StateAccountItem() { }
    }
}
