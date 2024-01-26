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
	/// Class     : PromoBanner
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  25/10/2022 17:52:22 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PromoBanner : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        public System.DateTime? FinalDate { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<PromoBannerItem> ListPromoBannerItems { get; set; }

        public IList<PromoBannerTrigger> ListPromoBannerTriggers { get; set; }


		#endregion

		#region Contructors 

		public PromoBanner() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PromoBanner
    /// </summary>
    /// <remarks></remarks>
    public enum relPromoBanner 
     { 
        PromoBannerItems, PromoBannerTriggers
	}

}