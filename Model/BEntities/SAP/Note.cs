using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Note : BEntity
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
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
        public decimal Margin { get; set; }
        public decimal TaxlessTotal { get; set; }
        public string TermConditions { get; set; }
        public string IsDeliveryNote { get; set; }
        public string BillNumber { get; set; }
        public string AuthorizationNumber { get; set; }
        public int Series { get; set; }

        public List<NoteItem> Items { get; set; }
        public DeliveryNote DeliveryNote { get; set; }
        public Order Order { get; set; }
        public List<OrderFile> Files { get; set; }

        public Note()
        {
            Items = new List<NoteItem>();
            Files = new List<OrderFile>();
        }
    }

    public class NoteExtended : Note
    {
        public string Comments { get; set; }
        public string BillingAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationCode { get; set; }
        public string Incoterms { get; set; }
        public string Correlative { get; set; }
        public string Transport { get; set; }
        public decimal Discount { get; set; }
        public string BilledTo { get; set; }
        public string Phone { get; set; }
        public string Cellphone { get; set; }
        public string NamePC { get; set; }
        public string PhonePC { get; set; }
        public string CellphonePC { get; set; }
        public string Accredited { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string BusinessName { get; set; }
        public string NIT { get; set; }
        public string FCName { get; set; }
        public string FCMail { get; set; }
        public string FCPhone { get; set; }
        public string FCCity { get; set; }
        public string FCAddress { get; set; }
    }

    public enum relNote
    {
        NoteItems, DeliveryNote, Order, Files
    }
}
