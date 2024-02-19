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
	/// Class     : AcceleratorLotExcluded
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  8/2/2024 16:31:05 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class AcceleratorLotExcluded : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdAccelerator { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public AcceleratorLot Accelerator { get; set; } 


		#endregion

		#region Contructors 

		public AcceleratorLotExcluded() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator AcceleratorLotExcluded
    /// </summary>
    /// <remarks></remarks>
    public enum relAcceleratorLotExcluded 
     { 
        Accelerator
	}

}