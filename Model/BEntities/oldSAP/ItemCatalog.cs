using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class ItemCatalog : BEntity {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Line { get; set; }
        public short ItmsGrpCod { get; set; }
        public string Detail { get; set; }
        public string Detail2 { get; set; }
        public int Available { get; set; }
    }
}
