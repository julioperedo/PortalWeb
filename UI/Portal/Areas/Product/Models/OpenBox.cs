using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Product.Models {
    public class OpenBoxFilters {
        public long? IdSubsidiary { get; set; }
        public string Product { get; set; }
        public bool Enabled { get; set; }
        public bool WithStock { get; set; }
    }

    public class OpenBox {
        public long Id { get; set; }
        public string Subsidiary { get; set; }
        public string ItemCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public string ProductComments { get; set; }
        public string Comments { get; set; }
        public string Link { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
