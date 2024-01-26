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
	/// Class     : VolumePricingHistory
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:42 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class VolumePricingHistory : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSubsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Price { get; set; }

        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Observations { get; set; }

        public long? LogUser { get; set; }

        public System.DateTime? LogDate { get; set; }

        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LogAction { get; set; }


		#endregion

		#region Additional Properties 

        public Product Product { get; set; } 

        public Base.Classifier Subsidiary { get; set; }


		#endregion

		#region Contructors 

		public VolumePricingHistory() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator VolumePricingHistory
    /// </summary>
    /// <remarks></remarks>
    public enum relVolumePricingHistory 
     { 
        Product, Subsidiary
	}

}