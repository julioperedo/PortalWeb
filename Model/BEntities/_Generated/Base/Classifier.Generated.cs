using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Base 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Base
	/// Class     : Classifier
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Base
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  17/10/2023 14:27:43 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Classifier : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Value { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool isBase { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdType { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public ClassifierType Type { get; set; } 

        public IList<Chart> ListCharts { get; set; }

        public IList<BE.HumanResources.License> ListLicenses { get; set; }

        public IList<BE.HumanResources.Request> ListRequest_States { get; set; }

        public IList<BE.Online.Sale> ListSales { get; set; }

        public IList<BE.Online.SaleDetail> ListSaleDetails { get; set; }

        public IList<BE.Online.SaleState> ListSaleStates { get; set; }

        public IList<BE.Online.TempSaleDetail> ListTempSaleDetails { get; set; }

        public IList<BE.Product.Document> ListDocuments { get; set; }

        public IList<BE.Product.OpenBox> ListOpenBoxs { get; set; }

        public IList<BE.Product.OpenBoxHistory> ListOpenBoxHistorys { get; set; }

        public IList<BE.Product.Price> ListPrices { get; set; }

        public IList<BE.Product.PriceHistory> ListPriceHistorys { get; set; }

        public IList<BE.Product.PriceOffer> ListPriceOffers { get; set; }

        public IList<BE.Product.PromoBannerItem> ListPromoBannerItems { get; set; }

        public IList<BE.Product.Request> ListRequest_Subsidiarys { get; set; }

        public IList<BE.Product.VolumePricing> ListVolumePricings { get; set; }

        public IList<BE.Product.VolumePricingHistory> ListVolumePricingHistorys { get; set; }

        public IList<BE.Sales.QuoteDetailPrices> ListQuoteDetailPricess { get; set; }

        public IList<BE.Sales.Transport> ListTransport_Destinations { get; set; }

        public IList<BE.Sales.Transport> ListTransport_Sources { get; set; }

        public IList<BE.Sales.Transport> ListTransport_Transporters { get; set; }

        public IList<BE.Sales.Transport> ListTransport_Types { get; set; }

        public IList<BE.Security.UserRequest> ListUserRequests { get; set; }

        public IList<BE.Security.UserRequestDetail> ListUserRequestDetails { get; set; }

        public IList<BE.Visits.VisitReception> ListVisitReceptions { get; set; }


		#endregion

		#region Contructors 

		public Classifier() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Classifier
    /// </summary>
    /// <remarks></remarks>
    public enum relClassifier 
     { 
        Type, Charts, Licenses, Request_States, Sales, SaleDetails, SaleStates, TempSaleDetails, Documents, OpenBoxs, OpenBoxHistorys, Prices, PriceHistorys, PriceOffers, PromoBannerItems, Request_Subsidiarys, VolumePricings, VolumePricingHistorys, QuoteDetailPricess, Transport_Destinations, Transport_Sources, Transport_Transporters, Transport_Types, UserRequests, UserRequestDetails, VisitReceptions
	}

}