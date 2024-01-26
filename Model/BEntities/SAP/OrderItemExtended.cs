using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class OrderItemExtended : BEntity {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public int OrderId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Line { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public int Quantity { get; set; }
        public int OpenQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public bool Complete { get; set; }
        public decimal CalculedTotal { get; set; }
        public decimal Margin { get; set; }
        public decimal? Margin0100 { get; set; }
        public decimal ItemTotal { get; set; }
    }
}
