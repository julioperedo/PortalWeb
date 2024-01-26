using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class CustomData: BEntity
    {
        public int InitialYear { get; set; }
        public int FinalYear { get; set; }
        public double Days { get; set; }

        public CustomData() { }
    }
}
