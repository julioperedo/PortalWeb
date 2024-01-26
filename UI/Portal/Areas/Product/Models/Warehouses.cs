using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Product.Models {
    public class Subsidiary {
        public string Name { get; set; }
        public List<AvailableWarehouse> Warehouses { get; set; }
        public Subsidiary() {
            Warehouses = new List<AvailableWarehouse>();
        }
    }

    public class AvailableWarehouse {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public bool ClientVisible { get; set; }
    }
}
