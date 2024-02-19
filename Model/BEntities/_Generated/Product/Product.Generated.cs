using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Product 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Product
	/// Class     : Product
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  15/2/2024 13:33:52 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Product : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Commentaries { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ItemCode { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Link { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Warranty { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Consumables { get; set; }

        public string ExtraComments { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool ShowAlways { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool ShowInWeb { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool ShowTeamOnly { get; set; }

        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageURL { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Line { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Category { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SubCategory { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Brand { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string BrandCode { get; set; }

        public bool Editable { get; set; }

        public System.DateTime? EnabledDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool IsDigital { get; set; }

        public decimal? Cost { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<BE.Kbytes.AcceleratorLot> ListAcceleratorLots { get; set; }

        public IList<BE.Kbytes.ClientNoteDetail> ListClientNoteDetails { get; set; }

        public IList<BE.Online.SaleDetail> ListSaleDetails { get; set; }

        public IList<BE.Online.TempSaleDetail> ListTempSaleDetails { get; set; }

        public IList<Document> ListDocuments { get; set; }

        public IList<Loan> ListLoans { get; set; }

        public IList<OpenBox> ListOpenBoxs { get; set; }

        public IList<OpenBoxHistory> ListOpenBoxHistorys { get; set; }

        public IList<Price> ListPrices { get; set; }

        public IList<PriceExternalSite> ListPriceExternalSites { get; set; }

        public IList<PriceHistory> ListPriceHistorys { get; set; }

        public IList<PriceOffer> ListPriceOffers { get; set; }

        public IList<PromoBannerItem> ListPromoBannerItems { get; set; }

        public IList<PromoBannerTrigger> ListPromoBannerTriggers { get; set; }

        public IList<RelatedProduct> ListRelatedProduct_Products { get; set; }

        public IList<RelatedProduct> ListRelatedProduct_Relateds { get; set; }

        public IList<Request> ListRequests { get; set; }

        public IList<VolumePricing> ListVolumePricings { get; set; }

        public IList<VolumePricingHistory> ListVolumePricingHistorys { get; set; }

        public IList<BE.Sales.QuoteDetail> ListQuoteDetails { get; set; }


		#endregion

		#region Contructors 

		public Product() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Product
    /// </summary>
    /// <remarks></remarks>
    public enum relProduct 
     { 
        AcceleratorLots, ClientNoteDetails, SaleDetails, TempSaleDetails, Documents, Loans, OpenBoxs, OpenBoxHistorys, Prices, PriceExternalSites, PriceHistorys, PriceOffers, PromoBannerItems, PromoBannerTriggers, RelatedProduct_Products, RelatedProduct_Relateds, Requests, VolumePricings, VolumePricingHistorys, QuoteDetails
	}

}