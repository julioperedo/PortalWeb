using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class OrderDestination : BEntity
    {
        public int DocNumber { get; set; }

        public string ClientCode { get; set; }

        public string EMail { get; set; }
    }
}
