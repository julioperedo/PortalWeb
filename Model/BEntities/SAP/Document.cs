using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class Document
    {
        public int Id { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string SellerName { get; set; }
        public string ClientOrder { get; set; }
        public string DocType { get; set; }
        public decimal Total { get; set; }
        public decimal OpenAmount { get; set; }
        public string State { get; set; }
        public decimal Margin { get; set; }
        public decimal TaxlessTotal { get; set; }
        public decimal Margin0100 { get; set; }
        public bool Authorized { get; set; }
        public bool Complete { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public int Series { get; set; }
        public IEnumerable<Attachment> Files { get; set; }
        public IEnumerable<DocumentRelated> RelatedDocs { get; set; }
        //public IEnumerable<DocumentItem> Items { get; set; }
        public IEnumerable<TransportOrder> Transport { get; set; }

        public Document() { }
    }

    public class DocumentRelated : BEntity
    {
        public int Id { get; set; }
        public string DocType { get; set; }
        public int DocNumber { get; set; }
        public int BaseId { get; set; }
        public string Subsidiary { get; set; }
        public int Series { get; set; }
    }

    public class DocumentItem
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string Line { get; set; }
        public int Quantity { get; set; }
        public int OpenQuantity { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public decimal ItemTotal { get; set; }
        public decimal Margin0100 { get; set; }
        public bool Complete { get; set; }
        public DocumentItem() { }
    }

    public class TransportOrder
    {
        public string DocNumber { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Transporter { get; set; }
        public string DeliveryTo { get; set; }
        public string Observations { get; set; }
        public decimal Weight { get; set; }
        public int QuantityPieces { get; set; }
        public decimal RemainingAmount { get; set; }
        public string StringValues { get; set; }
    }
}
