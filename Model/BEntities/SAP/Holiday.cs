using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Holiday
    {
        public string Year { get; set; }
        public DateTime Since { get; set; }
        public DateTime Until { get; set; }
        public string Name { get; set; }
        public Holiday() { }
    }
}
