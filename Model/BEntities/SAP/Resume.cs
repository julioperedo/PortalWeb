using System;
using System.Collections.Generic;
using System.Text;

namespace BEntities.SAP
{
    public class Resume : BEntity
    {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string Division { get; set; }
        public string Line { get; set; }
        public decimal Total { get; set; }
        public decimal TaxlessTotal { get; set; }
        public decimal Margin { get; set; }
    }

    public class ResumePeriod : BEntity
    {
        public string Subsidiary { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Division { get; set; }
        public decimal Total { get; set; }
    }

    public class ResumeByClient : Resume
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
    }

    public class ResumeBySeller : Resume
    {
        public string SellerName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}