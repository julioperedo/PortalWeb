using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportsDataService.Models {
    public class Order {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string BillingAddress { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationAddress { get; set; }
        public string Incoterms { get; set; }
        public string ClientOrder { get; set; }
        public string Correlative { get; set; }
        public string Seller { get; set; }
        public string Transport { get; set; }
        public string Terms { get; set; }
        public string Commentaries { get; set; }
    }
}
