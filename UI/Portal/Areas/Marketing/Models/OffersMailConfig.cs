using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEP = BEntities.Product;
using BEM = BEntities.Marketing;

namespace Portal.Areas.Marketing.Models
{
    public class OffersMailConfig
    {
        public IEnumerable<BEP.Line> Lines { get; set; }
        public IEnumerable<BEM.OffersMailConfig> Asigned { get; set; }

        public OffersMailConfig()
        {
            Lines = Enumerable.Empty<BEP.Line>();
            Asigned = Enumerable.Empty<BEM.OffersMailConfig>();
        }
    }
}
