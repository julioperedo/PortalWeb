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
	/// Class     : QuoteSent
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
	public partial class QuoteSent : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdQuote { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientMail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardName { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Quote Quote { get; set; } 


		#endregion

		#region Contructors 

		public QuoteSent() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator QuoteSent
    /// </summary>
    /// <remarks></remarks>
    public enum relQuoteSent 
     { 
        Quote
	}

}