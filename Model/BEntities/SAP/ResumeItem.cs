using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP {
    public class ResumeItem : BEntity {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public string MonthDesc { get; set; }
        public string ClientName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int BillNumber { get; set; }        
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Line { get; set; }
        public string Seller { get; set; }
        public string ProductManager { get; set; }
        public decimal Total { get; set; }
        public decimal Quantity { get; set; }
        public decimal Margin { get; set; }
        public decimal PorcentageMargin { get; set; }
        public decimal TaxlessTotal { get; set; }
    }
}
