using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models
{
    public class OnlineSale
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public List<OnlineSaleDetail> Details { get; set; }

        public OnlineSale()
        {
            Details = new List<OnlineSaleDetail>();
        }

        public OnlineSale(BEntities.Online.TempSale Item)
        {
            Id = Item.Id;
            IdUser = Item.IdUser;
            Name = Item.Name;
            EMail = Item.EMail;
            Details = new List<OnlineSaleDetail>();
            if (Item.ListTempSaleDetails?.Count > 0)
            {
                foreach (var item in Item.ListTempSaleDetails)
                {
                    Details.Add(new OnlineSaleDetail(item));
                }
            }
        }
    }

    public class OnlineSaleDetail
    {
        public long Id { get; set; }
        public long IdSale { get; set; }
        public long IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public long? IdSubsidiary { get; set; }
        public string Warehouse { get; set; }
        public decimal Stock { get; set; }
        public ProductShort Product { get; set; }
        public Subsidiary Subsidiary { get; set; }
        public List<OnlineSaleDetailExtra> DataExtra { get; set; }

        public OnlineSaleDetail()
        {
            DataExtra = new List<OnlineSaleDetailExtra>();
        }

        public OnlineSaleDetail(BEntities.Online.TempSaleDetail Item)
        {
            Id = Item.Id;
            IdSale = Item.IdSale;
            IdProduct = Item.IdProduct;
            Quantity = Item.Quantity;
            Price = Item.Price;
            IdSubsidiary = Item.IdSubsidiary;
            Warehouse = Item.Warehouse;
            Product = Item.Product != null ? new ProductShort(Item.Product) : null;
            Subsidiary = Item.Subsidiary != null ? new Subsidiary(Item.Subsidiary) : null;
            DataExtra = new List<OnlineSaleDetailExtra>();
        }
    }

    public class OnlineSaleDetailExtra
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public long Stock { get; set; }
        public bool IsDigital { get; set; }
    }

    public class Subsidiary
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public Subsidiary(BEntities.Base.Classifier Item)
        {
            Id = Item.Id;
            Name = Item.Name;
            Value = Item.Value;
        }
    }

    public class ProductShort
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ItemCode { get; set; }

        public ProductShort(BEntities.Product.Product Item)
        {
            Id = Item.Id;
            Name = Item.Name;
            ItemCode = Item.ItemCode;
        }
    }
}
