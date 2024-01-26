using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class DeliveryNoteItem : BEntity
    {
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public string NoteNumber { get; set; }
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ItemCode { get; set; }
        public string BrandCode { get; set; }
        public string ItemName { get; set; }
        public string Line { get; set; }
        public string Warranty { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public int Stock { get; set; }
        public decimal Total { get; set; }
        public decimal Margin { get; set; }
        public decimal TaxlessTotal { get; set; }
    }
}
