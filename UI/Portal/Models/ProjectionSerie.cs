using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models
{
    public class ProjectionSerie
    {
        public string Name { get; set; }
        public string Division { get; set; }
        public List<decimal> Values { get; set; }

        public ProjectionSerie()
        {
            Values = new List<decimal>();
        }
    }
}
