using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class StateAccountDetail : BEntity {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string Type { get; set; }
        public string TypeId { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int DocNum { get; set; }
        public string DocBase { get; set; }
        public string ClientOrder { get; set; }
        public DateTime DocDate { get; set; }
        public string Terms { get; set; }
        public DateTime DueDate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal? Balance { get; set; }
        public int? Days { get; set; }
        public string Seller { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? StockPrice { get; set; }
        public decimal Factor { get; set; }
        public string State { get; set; }

        public StateAccountDetail() { }
    }
}
