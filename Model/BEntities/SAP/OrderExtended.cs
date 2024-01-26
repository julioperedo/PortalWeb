using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class OrderExtended : BEntity
    {
        public int? Id { get; set; }
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public int? DocNumber { get; set; }
        public DateTime? DocDate { get; set; }
        public string ClientOrder { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public string State { get; set; }
        public string Authorized { get; set; }
        public decimal? Total { get; set; }
        public string TermConditions { get; set; }
        public int NonCompleteItem { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string IsDeliveryNote { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string BusinessName { get; set; }
        public string NIT { get; set; }
        public string FCName { get; set; }
        public string FCMail { get; set; }
        public string FCPhone { get; set; }
        public string FCCity { get; set; }
        public string FCAddress { get; set; }

        public decimal? OpenAmount { get; set; }
        //public string NoteNumbers { get; set; }
        public int? NoteNumber { get; set; }
        public DateTime? NoteDate { get; set; }
        public List<NoteNumber> NoteNumbers { get; set; }
        public string BillsNumbers { get; set; }
        public string BillDates { get; set; }
        public decimal? TotalBilled { get; set; }
        public bool HasFiles { get; set; }
        public decimal Margin { get; set; }
        public decimal? Margin0100 { get; set; }

        public string Comments { get; set; }
        public string BillingAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string DestinationCode { get; set; }
        public string Incoterms { get; set; }
        public string Correlative { get; set; }
        public string Transport { get; set; }
        public decimal Discount { get; set; }

        public bool HasTransport { get; set; }

        public List<string> AttachedFiles { get; set; }
    }

    public class NoteNumber
    {
        public int Number { get; set; }
        public string Delivery { get; set; }
    }
}
