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
	/// Class     : PromoBannerItem
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
	public partial class PromoBannerItem : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPromo { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSubsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Price { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product Product { get; set; } 

        public PromoBanner Promo { get; set; } 

        public Base.Classifier Subsidiary { get; set; }


		#endregion

		#region Contructors 

		public PromoBannerItem() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PromoBannerItem
    /// </summary>
    /// <remarks></remarks>
    public enum relPromoBannerItem 
     { 
        Product, Promo, Subsidiary
	}

}