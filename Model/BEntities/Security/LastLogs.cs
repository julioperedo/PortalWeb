using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.Security
{
    public class LastLogs : BEntity
    {
        public string CardCode { get; set; }
        public string ClientName { get; set; }
        public string EMail { get; set; }
        public DateTime? LastLog { get; set; }
        public string Period { get; set; }
        //public int PeriodOrder { get; set; }
        public int Days { get; set; }
        public string Seller { get; set; }
    }
}
