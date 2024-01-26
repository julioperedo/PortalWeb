using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Product.Models
{
    public class PriceHistory
    {
        public IEnumerable<BEntities.Product.PriceHistory> Prices { get; set; }
        public IEnumerable<BEntities.Product.VolumePricingHistory> Volume { get; set; }
    }
}
