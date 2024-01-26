using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Order : BEntity
    {
        public int Id { get; set; }
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public int DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string ClientOrder { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public string State { get; set; }
        public string Authorized { get; set; }
        public decimal Total { get; set; }
        public decimal OpenAmount { get; set; }
        public string TermConditions { get; set; }
        public int NonCompleteItem { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string IsDeliveryNote { get; set; }
        public int TotalFiles { get; set; }
        public decimal Margin { get; set; }
        public decimal TaxlessTotal { get; set; }

        public List<OrderItem> Items { get; set; }
        public List<Note> Notes { get; set; }
        public List<OrderFile> Files { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
            Notes = new List<Note>();
            Files = new List<OrderFile>();
        }
    }

    public enum relOrder
    {
        OrderItems, Notes, Files
    }

}
