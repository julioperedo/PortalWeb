using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class Seller : BEntity
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Active { get; set; }
    }
}
