using Portal.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Models
{
    public class Line
    {
        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "Must not exceed {1} characters.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasExternalPrice { get; set; }

        public string ExternalSiteName { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        [StringLength(300, ErrorMessage = "Must not exceed {1} characters.")]
        public string ImageURL { get; set; }

        public string NewImageURL { get; set; }

        public long? IdManager { get; set; }

        public List<Category> Categories { get; set; }

        public List<string> Subsidiaries { get; set; }

        public List<string> SAPLines { get; set; }

        public List<Line> SubLines { get; set; }

        public bool LocalUser { get; set; }
        public string FilterType { get; set; }
        public bool WhenFilteredShowInfo { get; set; }

        public Line()
        {
            Categories = new List<Category>();
        }

        public Line(long Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public Line(BEP.Line Item)
        {
            Id = Item.Id;
            Name = Item?.Name;
            Description = Item?.Description;
            HasExternalPrice = Item.HasExternalPrice;
            ExternalSiteName = Item?.ExternalSiteName;
            Header = Item?.Header;
            Footer = Item.Footer?.Replace("\r", "<br />") ?? "";
            ImageURL = Item?.ImageURL;
            NewImageURL = "";
            FilterType = Item?.FilterType;
            WhenFilteredShowInfo = Item.WhenFilteredShowInfo;
            IdManager = Item?.IdManager;
        }

        public BEP.Line ToEntity()
        {
            BEP.Line item = new() { Id = Id, Name = Name, Description = Description, HasExternalPrice = HasExternalPrice, ExternalSiteName = ExternalSiteName, Header = Header, Footer = Footer, FilterType = FilterType, WhenFilteredShowInfo = WhenFilteredShowInfo, IdManager = IdManager, ListLineDetails = new List<BEP.LineDetail>() };
            return item;
        }

        public BEP.Line ToEntity(long UserId, DateTime ExecTime)
        {
            BEP.Line item = ToEntity();
            item.LogUser = UserId;
            item.LogDate = ExecTime;
            return item;
        }

    }

    public class LineShort
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        public string ImageURL { get; set; }

        public Manager Manager { get; set; }

        public List<Category> Categories { get; set; }
        public List<string> SAPLines { get; set; }
        public List<string> UsedGroups { get; set; }

        public decimal Percentage { get; set; }

        public LineShort()
        {
            Categories = new List<Category>();
            SAPLines = new List<string>();
            UsedGroups = new List<string>();
        }

        public LineShort(BEP.Line Item)
        {
            Id = Item.Id;
            Name = Item?.Name.ToWebSafe();
            Description = Item?.Description.ToWebSafe();
            Header = Item?.Header.ToWebSafe();
            Footer = Item?.Footer.ToWebSafe();
            ImageURL = Item?.ImageURL;
        }

    }

    public class Category
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<Subcategory> Subcategories { get; set; }

        public Category()
        {
            Subcategories = new List<Subcategory>();
        }
    }

    public class Subcategory
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public List<ProductShort> Products { get; set; }

        public Subcategory()
        {
            Products = new List<ProductShort>();
        }
    }

    public class Manager
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public int? Phone { get; set; }

        public Manager(BEntities.Staff.Member Item)
        {
            Id = Item?.Id ?? 0;
            Name = Item?.Name;
            Position = Item?.Position;
            Photo = Item?.Photo;
            Email = Item?.Mail;
            Phone = Item?.Phone;
        }
    }

}
