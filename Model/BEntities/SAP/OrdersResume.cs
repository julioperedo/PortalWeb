using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class OrdersResume : BEntity {
        public string CardCode { get; set; }
        public int Total { get; set; }

        public OrdersResume() { }
    }
}
