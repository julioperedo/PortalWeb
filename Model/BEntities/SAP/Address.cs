using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Address : BEntity
    {
        public string Name { get; set; }
        public string CardCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Contact { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string TaxCode { get; set; }
        public string Type { get; set; }

        public Address() { }
    }
}