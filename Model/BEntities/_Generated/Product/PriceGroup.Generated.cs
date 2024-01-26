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
	/// Class     : PriceGroup
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  10/8/2023 10:15:34 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PriceGroup : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Percentage { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<PriceGroupClient> ListPriceGroupClients { get; set; }

        public IList<PriceGroupLine> ListPriceGroupLines { get; set; }


		#endregion

		#region Contructors 

		public PriceGroup() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PriceGroup
    /// </summary>
    /// <remarks></remarks>
    public enum relPriceGroup 
     { 
        PriceGroupClients, PriceGroupLines
	}

}