using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class Serial : BEntity
    {
        public string Subsidiary { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public int DocType { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string SerialNumber { get; set; }
        public decimal Price { get; set; }
    }
}
