using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class CustomResume : BEntity
    {
        public string Name { get; set; }
        public string SubName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Total { get; set; }

        public CustomResume() { }
    }
}
