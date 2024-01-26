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
	/// Class     : Quote
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
	public partial class Quote : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string QuoteCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSeller { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime QuoteDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientMail { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Security.User Seller { get; set; }

        public IList<QuoteDetail> ListQuoteDetails { get; set; }

        public IList<QuoteSent> ListQuoteSents { get; set; }


		#endregion

		#region Contructors 

		public Quote() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Quote
    /// </summary>
    /// <remarks></remarks>
    public enum relQuote 
     { 
        Seller, QuoteDetails, QuoteSents
	}

}