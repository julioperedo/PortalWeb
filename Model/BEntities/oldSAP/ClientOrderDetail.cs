using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class ClientOrderDetail : BEntity {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string Line { get; set; }
        public decimal Quantity { get; set; }
        public decimal OpenQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal TotalCalculo { get; set; }
        public decimal Margen { get; set; }
        public decimal MargenPorcentaje { get; set; }
        public decimal InStock { get; set; }
        public string CompleteOrder { get; set; }

        public ClientOrderDetail() { }
    }
}
