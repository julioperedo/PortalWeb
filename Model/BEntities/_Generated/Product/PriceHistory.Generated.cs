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
	/// Class     : PriceHistory
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:40 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PriceHistory : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSudsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Regular { get; set; }

        public decimal? Offer { get; set; }

        [StringLength(255, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string OfferDescription { get; set; }

        public decimal? ClientSuggested { get; set; }

        [StringLength(255, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Observations { get; set; }

        [StringLength(255, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Commentaries { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LogAction { get; set; }


		#endregion

		#region Additional Properties 

        public Product Product { get; set; } 

        public Base.Classifier Sudsidiary { get; set; }


		#endregion

		#region Contructors 

		public PriceHistory() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PriceHistory
    /// </summary>
    /// <remarks></remarks>
    public enum relPriceHistory 
     { 
        Product, Sudsidiary
	}

}