using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class ReservedItem : BEntity
    {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Authorized { get; set; }
        public string Correlative { get; set; }
    }
}
