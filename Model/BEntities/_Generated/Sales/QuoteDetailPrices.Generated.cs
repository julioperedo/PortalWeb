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
	/// Class     : QuoteDetailPrices
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Sales
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:46 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class QuoteDetailPrices : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdDetail { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSubsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        public string Observations { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Selected { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public QuoteDetail Detail { get; set; } 

        public Base.Classifier Subsidiary { get; set; }


		#endregion

		#region Contructors 

		public QuoteDetailPrices() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator QuoteDetailPrices
    /// </summary>
    /// <remarks></remarks>
    public enum relQuoteDetailPrices 
     { 
        Detail, Subsidiary
	}

}