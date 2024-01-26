using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class OrderNote: BEntity
    {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public int? NoteId { get; set; }
        public int? NoteNumber { get; set; }
        public DateTime? NoteDate { get; set; }
        public string NoteClientOrder { get; set; }
        public decimal? NoteTotal { get; set; }
        public decimal? NoteMargin { get; set; }
        public decimal? NoteTaxlessTotal { get; set; }
        public string TermConditions { get; set; }
        public int? OrderId { get; set; }
        public int? OrderNumber { get; set; }
        public string BillNumber { get; set; }
        public string AuthorizationNumber { get; set; }
        public string IsDeliveryNote { get; set; }
        public string ClientOrder { get; set; }
        public decimal? OrderTotal { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string OrderAuthorized { get; set; }
        public int NonCompleteItem { get; set; }
        public int ItemsCount { get; set; }
        public decimal? OpenAmount { get; set; }
        public bool HasTransport { get; set; }
        public bool HasFiles { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public decimal? Margin0100 { get; set; }

    }
}
