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
	/// Class     : Link
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table CIESD
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  16/3/2022 15:41:56 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Link : BEntity 
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
        public string Type { get; set; }

        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Uri { get; set; }

        public System.DateTime? ExpirationDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool ExpirationDateSpecified { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Purchase Purchase { get; set; } 


		#endregion

		#region Contructors 

		public Link() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Link
    /// </summary>
    /// <remarks></remarks>
    public enum relLink 
     { 
        Purchase
	}

}