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
	/// Class     : PromoBannerTrigger
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  25/10/2022 17:52:23 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PromoBannerTrigger : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPromo { get; set; }

        public long? IdProduct { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Category { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public PromoBanner Promo { get; set; } 

        public Product Product { get; set; } 


		#endregion

		#region Contructors 

		public PromoBannerTrigger() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PromoBannerTrigger
    /// </summary>
    /// <remarks></remarks>
    public enum relPromoBannerTrigger 
     { 
        Promo, Product
	}

}