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
	/// Class     : SentEmail
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table CIESD
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:23 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class SentEmail : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPurchase { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string EMail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Type { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Purchase Purchase { get; set; } 


		#endregion

		#region Contructors 

		public SentEmail() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator SentEmail
    /// </summary>
    /// <remarks></remarks>
    public enum relSentEmail 
     { 
        Purchase
	}

}