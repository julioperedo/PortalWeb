using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Bill : BEntity
    {
        public long DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string NIT { get; set; }
        public string BillNumber { get; set; }
        public string AuthorizationNumber { get; set; }
        public string ControlCode { get; set; }
        public DateTime LimitDate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal SysRate { get; set; }
        public decimal DocTotalRated { get; set; }
    }
}
