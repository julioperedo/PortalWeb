using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class CreditNote : BEntity
    {
        public int Id { get; set; }
        public long DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
        public string Reference { get; set; }
        public string SellerCode { get; set; }
        public decimal Total { get; set; }
        public string Comments { get; set; }
        public string Memo { get; set; }
        public long TransId { get; set; }
        public string NoteNumber { get; set; }
        public string Terms { get; set; }
    }

    public class CreditNoteItem : BEntity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string ManufacterCode { get; set; }
        public long AccountCode { get; set; }
        public string AccountName { get; set; }
    }


}
