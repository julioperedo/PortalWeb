using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class DeliveryNote : BEntity
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string NoteNumber { get; set; }
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientNote { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string ClientOrder { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public decimal Total { get; set; }
        public string BillName { get; set; }
        public string NIT { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Accredited { get; set; }
        public string ClientAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryPhone { get; set; }
        public string DeliveryMobile { get; set; }
        public string Comments { get; set; }
        public string Incoterms { get; set; }
        public string Correlative { get; set; }
        public string Transport { get; set; }
        public string Terms { get; set; }
        public decimal Discount { get; set; }
        public decimal Margin { get; set; }
        public decimal TaxlessTotal { get; set; }

        public IList<DeliveryNoteItem> Items { get; set; }
        public IList<Note> Notes { get; set; }
        public IList<OrderFile> Files { get; set; }

        public DeliveryNote()
        {
            Items = new List<DeliveryNoteItem>();
            Notes = new List<Note>();
            Files = new List<OrderFile>();
        }
    }

    public enum relDeliveryNote
    {
        Items, Notes, Files
    }
}
