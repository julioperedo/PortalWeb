using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Sales 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Sales
	/// Class     : QuoteDetail
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Sales
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:45 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class QuoteDetail : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdQuote { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ProductCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string ProductName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string ProductDescription { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ProductImageURL { get; set; }

        [StringLength(350, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ProductLink { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product.Product Product { get; set; }

        public Quote Quote { get; set; } 

        public IList<QuoteDetailPrices> ListQuoteDetailPricess { get; set; }


		#endregion

		#region Contructors 

		public QuoteDetail() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator QuoteDetail
    /// </summary>
    /// <remarks></remarks>
    public enum relQuoteDetail 
     { 
        Product, Quote, QuoteDetailPricess
	}

}