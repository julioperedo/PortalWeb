using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Online 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Online
	/// Class     : SaleFiles
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Online
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:32 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class SaleFiles : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSale { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Type { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Sale Sale { get; set; } 


		#endregion

		#region Contructors 

		public SaleFiles() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator SaleFiles
    /// </summary>
    /// <remarks></remarks>
    public enum relSaleFiles 
     { 
        Sale
	}

}