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
	/// Class     : PurchaseReturn
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
	public partial class PurchaseReturn : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPurchase { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.Guid ServiceTransactionId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.Guid ClientTransactionId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime ReturnDate { get; set; }

        public long? OrderNumber { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Purchase Purchase { get; set; } 


		#endregion

		#region Contructors 

		public PurchaseReturn() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PurchaseReturn
    /// </summary>
    /// <remarks></remarks>
    public enum relPurchaseReturn 
     { 
        Purchase
	}

}