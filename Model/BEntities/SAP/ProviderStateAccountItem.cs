using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP {
    public class ProviderStateAccountItem : BEntity {
        public string Subsidiary { get; set; }
        public string Type { get; set; }
        public int DocNum { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderName { get; set; }
        public int? BillProvider { get; set; }
        public string DocBase { get; set; }
        public DateTime DocDate { get; set; }
        public string Terms { get; set; }
        public DateTime DocDueDate { get; set; }
        public decimal Balance { get; set; }
        public int Days { get; set; }
        public string State { get; set; }
        public string BillNumber { get; set; }
        public decimal DocTotal { get; set; }
        public string ProductManager { get; set; }
        public string ProductManagerShort { get; set; }

        public ProviderStateAccountItem() { }
    }
}
