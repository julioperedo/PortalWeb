using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEntities.SAP
{
    public class Product : BEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Line { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Commentaries { get; set; }
        public string Warranty { get; set; }
        public string Picture { get; set; }
        public string FilePath { get; set; }
        public string Subsidiary { get; set; }
        public string ProductManager { get; set; }
        public string ProductManagerCode { get; set; }
        public string FactoryCode { get; set; }
        public string Blocked { get; set; }
        public string Rotation { get; set; }
        public string CodeBars { get; set; }

        public List<ProductStock> Stock { get; set; }
    }

    public enum relProduct
    {
        ProductStocks
    }
}
