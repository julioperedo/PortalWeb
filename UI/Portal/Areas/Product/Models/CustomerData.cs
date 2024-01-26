using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Areas.Product.Models {
    public class CustomerData {
        public string host { get; set; }
        public string token { get; set; }
        public string clearSession { get; set; }
        public string mfr { get; set; }
        public string uEmail { get; set; }
        public string uName { get; set; }
        public string uTel { get; set; }
        public string CName { get; set; }
        public string cPCode { get; set; }
        public string cAccountNum { get; set; }
        public string reff { get; set; }
        public string BasketURL { get; set; }
        public string PPID { get; set; }
        public string UserID { get; set; }
        public string OrderEntry { get; set; }
        public string TeamQuotes { get; set; }
        public string TargetPage { get; set; }
        public string SessionID { get; set; }
        public string @base { get; set; }
        public bool Valid { get; set; }
    }
}
