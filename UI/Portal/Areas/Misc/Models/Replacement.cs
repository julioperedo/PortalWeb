using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEF = BEntities.Staff;

namespace Portal.Areas.Misc.Models {
    public class Replacement {
        public bool ValidSeller { get; set; }
        public string SellerCode { get; set; }
        public List<BEF.Replace> Replacements { get; set; }

        public Replacement() {
            ValidSeller = false;
            Replacements = new List<BEF.Replace>();
        }
    }

    public class ReplacementFilters {
        public DateTime? Initialdate { get; set; }
        public DateTime? FinalDate { get; set; }
        public string SellerCode { get; set; }
        public string ReplacementCode { get; set; }
    }
}
