using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class ProductStock : BEntity
    {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Line { get; set; }
        public string Brand { get; set; }
        public string ProductManager { get; set; }
        public int Stock { get; set; }
        public int Reserved { get; set; }
        public int Requested { get; set; }
        public int Available { get; set; }
        public int Available2 { get; set; }
        public decimal? PriceReal { get; set; }
        public decimal? TotalReal { get; set; }
        public decimal? PriceModified { get; set; }
        public decimal? TotalModified { get; set; }
        public string Warning { get; set; }
        public string Percentage { get; set; }
        public DateTime Date { get; set; }
        public string Blocked { get; set; }
        public string Rotation { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public Product Product { get; set; }
    }

    public enum relProductStock
    {
        Product
    }
}
