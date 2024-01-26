using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Commercial.Models
{
    public class SaleNoteReport
    {
        public string Subsidiary { get; set; }
        public int SaleNote { get; set; }
        public string User { get; set; }
        public string CardCode { get; set; }
    }
}
