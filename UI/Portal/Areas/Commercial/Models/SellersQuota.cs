using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Commercial.Models
{
    public class SellersQuota
    {
        public long Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Commentaries { get; set; }
    }
}
