using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models
{
    public class ShoppingCart
    {
        public long Id { get; set; }
        public long IdUser { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Commentaries { get; set; }
        public string ClientSaleNote { get; set; }
        public decimal Total { get; set; }
        public string SellerCode { get; set; }
        public string SellerName { get; set; }
        public long StateIdc { get; set; }
        public bool WithDropShip { get; set; }
        public List<ShoppingCartDetail> Details { get; set; }

        public ShoppingCart()
        {
            Details = new List<ShoppingCartDetail>();
        }

        public ShoppingCart(BEntities.Online.Sale Item)
        {
            Id = Item.Id;
            IdUser = Item.IdUser;
            Code = Item.Code;
            Name = Item.Name;
            Address = Item.Address;
            Commentaries = Item.Commentaries;
            Total = Item.Total;
            SellerCode = Item.SellerCode;
            SellerName = Item.SellerName;
            StateIdc = Item.StateIdc;
            Details = Item.ListSaleDetails?.Select(x => new ShoppingCartDetail(x))?.ToList() ?? new List<ShoppingCartDetail>();
        }
    }

    public class ShoppingCartDetail
    {
        public long Id { get; set; }
        public long IdSale { get; set; }
        public long IdProduct { get; set; }
        public int Quantity { get; set; }
        public int? ApprovedQuantity { get; set; }
        public decimal Price { get; set; }
        public long? IdSubsidiary { get; set; }
        public string Warehouse { get; set; }
        public decimal Stock { get; set; }
        public ProductShort Product { get; set; }
        public Subsidiary Subsidiary { get; set; }
        public List<ShoppingCartDetailExtra> DataExtra { get; set; }

        public ShoppingCartDetail()
        {
            DataExtra = new List<ShoppingCartDetailExtra>();
        }

        public ShoppingCartDetail(BEntities.Online.SaleDetail Item)
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
            DataExtra = new List<ShoppingCartDetailExtra>();
        }
    }

    public class ShoppingCartDetailExtra
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public List<ShoppingCartVolumePrice> VolumePrices { get; set; }
        public long Stock { get; set; }
        public bool IsDigital { get; set; }
    }

    public class ShoppingCartVolumePrice
    {
        public long Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
