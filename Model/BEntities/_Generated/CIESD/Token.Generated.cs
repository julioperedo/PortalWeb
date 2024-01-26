using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.CIESD 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : CIESD
	/// Class     : Token
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table CIESD
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  07/06/2022 10:15:28 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Token : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPurchase { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Line { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.Guid TransactionNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SequenceNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Type { get; set; }

        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Code { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Purchase Purchase { get; set; } 

        public IList<TokenReturn> ListTokenReturns { get; set; }


		#endregion

		#region Contructors 

		public Token() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Token
    /// </summary>
    /// <remarks></remarks>
    public enum relToken 
     { 
        Purchase, TokenReturns
	}

}