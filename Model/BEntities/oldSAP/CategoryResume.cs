using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class CategoryResume : BEntity
    {
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public decimal Total { get; set; }

        public CategoryResume() { }
    }
}
