using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Kbytes 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Kbytes
	/// Class     : AcceleratorLot
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  2/2/2024 14:27:47 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class AcceleratorLot : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int InitialQuantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Accelerator { get; set; }

        public System.DateTime? InitialDate { get; set; }

        public System.DateTime? FinalDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product.Product Product { get; set; }


		#endregion

		#region Contructors 

		public AcceleratorLot() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator AcceleratorLot
    /// </summary>
    /// <remarks></remarks>
    public enum relAcceleratorLot 
     { 
        Product
	}

}