using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BEP = BEntities.Product;
using BEA = BEntities.SAP;
using BEK = BEntities.Kbytes;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using BEntities;
using Portal.Misc;

namespace Portal.Areas.Product.Models
{
    public class Product
    {
        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Commentaries { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ItemCode { get; set; }
        public string FactoryCode { get; set; }

        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Link { get; set; }

        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageURL { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Warranty { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Consumables { get; set; }

        public string ExtraComments { get; set; }

        public bool ShowTeamOnly { get; set; }

        public bool IsNew { get; set; }

        public bool ShowAlways { get; set; }

        public string NewImageURL { get; set; }

        public bool Enabled { get; set; }
        public bool IsDigital { get; set; }

        public bool ShowInWeb { get; set; }

        public bool WithPrice { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Line { get; set; }
        public string Brand { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Category { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SubCategory { get; set; }

        public bool Editable { get; set; }

        public long IdDetail { get; set; }

        public bool AllowShoppingCart { get; set; }

        public string ExternalSite { get; set; }

        public ExternalPrice ExternalPrice { get; set; }

        public List<SubsidiaryPrice> Prices { get; set; }

        public List<BEP.VolumePricing> Volumen { get; set; }

        public List<BEK.AcceleratorLot> Lots { get; set; }

        public List<PriceOffer> Offers { get; set; }

        public Product()
        {
            Prices = new List<SubsidiaryPrice>();
            Volumen = new List<BEP.VolumePricing>();
            Lots = new List<BEK.AcceleratorLot>();
            Offers = new List<PriceOffer>();
        }

        public Product(BEP.Product Item)
        {
            Id = Item.Id;
            Name = Item.Name;
            Description = Item.Description;
            Commentaries = Item.Commentaries;
            ItemCode = Item.ItemCode;
            Link = Item.Link;
            ImageURL = Item.ImageURL;
            Warranty = Item.Warranty;
            Consumables = Item.Consumables;
            Enabled = Item.Enabled;
            Editable = Item.Editable;
            ShowAlways = Item.ShowAlways;
            ShowInWeb = Item.ShowInWeb;
            Line = Item.Line;
            Category = Item.Category;
            SubCategory = Item.SubCategory;
            ShowTeamOnly = Item.ShowTeamOnly;
            ExtraComments = Item.ExtraComments;
            IsDigital = Item.IsDigital;

            Prices = new List<SubsidiaryPrice>();
            if (Item.ListPrices != null)
            {
                foreach (var price in Item.ListPrices)
                {
                    Prices.Add(new SubsidiaryPrice(price));
                }
            }
            Offers = Item.ListPriceOffers?.Select(x => new PriceOffer(x))?.ToList() ?? new List<PriceOffer>();
            Lots = Item.ListAcceleratorLots?.ToList() ?? new List<BEK.AcceleratorLot>();
            Volumen = Item.ListVolumePricings?.ToList() ?? new List<BEP.VolumePricing>();
        }

        public Product(BEP.Product Item, bool HTMLSafe)
        {
            static string toHTML(string x, bool s) => (s ? x?.Replace("\r", "<br />") : x) ?? "";
            Id = Item.Id;
            Name = Item.Name;
            Description = toHTML(Item.Description, HTMLSafe);
            Commentaries = toHTML(Item.Commentaries, HTMLSafe);
            ItemCode = Item.ItemCode;
            Link = Item.Link;
            ImageURL = Item.ImageURL;
            Warranty = toHTML(Item.Warranty, HTMLSafe);
            Consumables = toHTML(Item.Consumables, HTMLSafe);
            Enabled = Item.Enabled;
            Editable = Item.Editable;
            ShowAlways = Item.ShowAlways;
            ShowInWeb = Item.ShowInWeb;
            Line = Item.Line;
            Category = Item.Category;
            SubCategory = Item.SubCategory;
            ShowTeamOnly = Item.ShowTeamOnly;
            ExtraComments = toHTML(Item.ExtraComments, HTMLSafe);
            IsDigital = Item.IsDigital;

            Prices = new List<SubsidiaryPrice>();
            if (Item.ListPrices != null)
            {
                foreach (var price in Item.ListPrices)
                {
                    Prices.Add(new SubsidiaryPrice(price));
                }
            }
            Offers = Item.ListPriceOffers?.Select(x => new PriceOffer(x))?.ToList() ?? new List<PriceOffer>();
            Lots = Item.ListAcceleratorLots?.ToList() ?? new List<BEK.AcceleratorLot>();
            Volumen = Item.ListVolumePricings?.ToList() ?? new List<BEP.VolumePricing>();
        }

        public BEP.Product ToEntity()
        {
            return new BEP.Product
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Commentaries = Commentaries,
                ItemCode = ItemCode,
                Link = Link,
                Warranty = Warranty,
                ExtraComments = ExtraComments,
                Consumables = Consumables,
                Enabled = Enabled,
                Editable = Editable,
                ShowAlways = ShowAlways,
                ShowInWeb = ShowInWeb,
                ShowTeamOnly = ShowTeamOnly,
                Line = Line,
                Category = Category,
                SubCategory = SubCategory,
                IsDigital = IsDigital,
                ListAcceleratorLots = Lots
            };
        }

        public BEP.Product ToEntity(long LogUser, DateTime LogDate)
        {
            BEP.Product product = ToEntity();
            product.LogUser = LogUser;
            product.LogDate = LogDate;
            return product;
        }
    }

    public class ProductShort
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Commentaries { get; set; }

        public string ItemCode { get; set; }

        public string Link { get; set; }

        public string ImageURL { get; set; }

        public string Warranty { get; set; }

        public string Consumables { get; set; }

        public string ExtraComments { get; set; }

        public bool ShowTeamOnly { get; set; }

        public string ShowAlways { get; set; }

        public bool IsNew { get; set; }
        public bool IsDigital { get; set; }

        public List<Price> Prices { get; set; }

        public ProductShort()
        {
            Prices = new List<Price>();
        }

        public ProductShort(BEP.Product Item)
        {
            string toHTML(string x) => x?.Replace("\r", "<br />") ?? "";
            Id = Item.Id;
            Name = Item.Name;
            Description = toHTML(Item.Description);
            Commentaries = toHTML(Item.Commentaries);
            ItemCode = Item.ItemCode;
            Link = Item.Link;
            ImageURL = Item.ImageURL;
            Warranty = toHTML(Item.Warranty);
            Consumables = toHTML(Item.Consumables);
            ShowTeamOnly = Item.ShowTeamOnly;
            ExtraComments = toHTML(Item.ExtraComments);
            IsDigital = Item.IsDigital;

            Prices = new List<Price>();
            if (Item.ListPrices != null)
            {
                foreach (var price in Item.ListPrices)
                {
                    Prices.Add(new Price(price));
                }
            }

        }
    }

    public class SubsidiaryPrice
    {
        public long Id { get; set; }

        public long IdProduct { get; set; }

        public long IdSubsidiary { get; set; }

        public string Subsidiary { get; set; }

        public decimal Regular { get; set; }

        public decimal? Offer { get; set; }

        [StringLength(255, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string OfferDescription { get; set; }

        public decimal? ClientSuggested { get; set; }

        [StringLength(255, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Observations { get; set; }

        [StringLength(255, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Commentaries { get; set; }

        public decimal? Stock => StockDetail?.Sum(x => (LocalUser | SeeStock) ? (IdSubsidiary == 3 ? x.Stock : x.Available) : x.Percentage);

        public List<SubsidiaryStock> StockDetail { get; set; }
        public List<SubsidiaryStock> FifoDetail { get; set; }

        public List<BEP.VolumePricing> Volumen { get; set; }

        public List<BEP.PriceOffer> Offers { get; set; }

        public bool LocalUser { get; set; }

        public bool SeeStock { get; set; }

        public SubsidiaryPrice()
        {
            StockDetail = new List<SubsidiaryStock>();
            FifoDetail = new List<SubsidiaryStock>();
            Volumen = new List<BEP.VolumePricing>();
            Offers = new List<BEP.PriceOffer>();
        }

        public SubsidiaryPrice(BEP.Price Item)
        {
            Id = Item.Id;
            IdProduct = Item.IdProduct;
            IdSubsidiary = Item.IdSudsidiary;
            Subsidiary = Item.Sudsidiary?.Name ?? "";
            Regular = Item.Regular;
            ClientSuggested = Item.ClientSuggested;
            Observations = Item.Observations;
            Commentaries = Item.Commentaries;
            StockDetail = new List<SubsidiaryStock>();
            FifoDetail = new List<SubsidiaryStock>();
            Volumen = new List<BEP.VolumePricing>();
            Offers = new List<BEP.PriceOffer>();
        }

        public BEP.Price ToEntity()
        {
            return new BEP.Price { Id = Id, IdSudsidiary = IdSubsidiary, Regular = Regular, ClientSuggested = ClientSuggested, Observations = Observations, Commentaries = Commentaries };
        }

        public BEP.Price ToEntity(long LogUser, DateTime LogDate)
        {
            BEP.Price price = ToEntity();
            price.LogUser = LogUser;
            price.LogDate = LogDate;
            return price;
        }
    }

    public class Price
    {
        //public long IdSubsidiary { get; set; }

        public string Subsidiary { get; set; }

        public decimal Regular { get; set; }

        public decimal? Offer { get; set; }
        public DateTime? OfferSince { get; set; }
        public DateTime? OfferUntil { get; set; }
        public decimal? Suggested { get; set; }

        public string OfferDescription { get; set; }

        public string Observations { get; set; }

        public string Commentaries { get; set; }

        public List<VolumePrice> Volume { get; set; }

        public List<ListName> OtherPrices { get; set; }
        public Price()
        {
            Volume = new List<VolumePrice>();
        }

        public Price(BEP.Price Item)
        {
            //IdSubsidiary = Item.IdSudsidiary;
            Subsidiary = Item.Sudsidiary?.Name ?? "";
            Regular = Item.Regular;
            //Offer = Item.Offer;
            //OfferDescription = Item.OfferDescription;
            Observations = Item.Observations;
            Commentaries = Item.Commentaries;
            Volume = new List<VolumePrice>();
        }
    }

    public class ListName
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ListName(string Name, decimal Price)
        {
            this.Name = Name;
            this.Price = Price;
        }
    }

    public class VolumePrice
    {
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Observations { get; set; }
    }

    public class PriceOffer
    {
        public long Id { get; set; }
        public long IdSubsidiary { get; set; }
        public string Subsidiary { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public DateTime? Since { get; set; }
        public DateTime? Until { get; set; }
        public bool OnlyWithStock { get; set; }
        public StatusType StatusType { get; set; }

        public PriceOffer() { }

        public PriceOffer(BEP.PriceOffer Item)
        {
            Id = Item.Id;
            IdSubsidiary = Item.IdSubsidiary;
            Subsidiary = Item.Subsidiary?.Name ?? "";
            Price = Item.Price;
            Description = Item.Description;
            Enabled = Item.Enabled;
            Since = Item.Since;
            Until = Item.Until;
            OnlyWithStock = Item.OnlyWithStock;
            StatusType = Item.StatusType;
        }

        public BEP.PriceOffer ToEntity()
        {
            return new BEP.PriceOffer { Id = Id, IdSubsidiary = IdSubsidiary, Price = Price, Description = Description, Enabled = Enabled, Since = Since, Until = Until, OnlyWithStock = OnlyWithStock, StatusType = StatusType };
        }

        public BEP.PriceOffer ToEntity(long LogUser, DateTime LogDate)
        {
            BEP.PriceOffer offer = ToEntity();
            offer.LogUser = LogUser;
            offer.LogDate = LogDate;
            return offer;
        }

    }

    public class SubsidiaryStock
    {
        public string Subsidiary { get; set; }
        public string Warehouse { get; set; }
        public decimal Stock { get; set; }
        public decimal Reserve { get; set; }
        public decimal Available { get; set; }
        public decimal Transit { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? RealCost { get; set; }
        public decimal? Total { get; set; }
        public DateTime? Date { get; set; }

        public SubsidiaryStock() { }
        public SubsidiaryStock(BEA.ProductStock Item)
        {
            Subsidiary = Common.ToTitle(Item?.Subsidiary) ?? "";
            Warehouse = Common.ToTitle(Item?.Warehouse) ?? "";
            Stock = Item?.Stock ?? 0;
            Reserve = Item?.Reserved ?? 0;
            Available = Item?.Available2 ?? 0;
            Transit = Item?.Requested ?? 0;
            RealCost = Item?.PriceReal;
            Total = Item?.TotalReal;
            Date = Item?.Date;
        }
    }

    public class ExternalPrice
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public string Commentaries { get; set; }
        public bool ShowAlways { get; set; }

        public ExternalPrice()
        {
            Id = 0;
            Price = 0;
            ShowAlways = false;
            Commentaries = "";
        }
        public ExternalPrice(BEP.PriceExternalSite price)
        {
            Id = price?.Id ?? 0;
            Price = price?.Price ?? 0;
            Commentaries = price?.Commentaries ?? "";
            ShowAlways = price?.ShowAlways ?? false;
        }

        public bool HasData()
        {
            return Price > 0 | !string.IsNullOrWhiteSpace(Commentaries) | ShowAlways;
        }

        public BEP.PriceExternalSite ToEntity()
        {
            return new BEP.PriceExternalSite { Id = Id, Price = Price, Commentaries = Commentaries, ShowAlways = ShowAlways };
        }

        public BEP.PriceExternalSite ToEntity(long LogUser, DateTime LogDate)
        {
            BEP.PriceExternalSite price = ToEntity();
            price.LogDate = LogDate;
            price.LogUser = LogUser;
            return price;
        }
    }

    public class ProductFilters
    {
        public string Name { get; set; }
        public long? LineId { get; set; }
        public string CategoryId { get; set; }
        public string Enabled { get; set; }
        public string Offer { get; set; }
    }
}
