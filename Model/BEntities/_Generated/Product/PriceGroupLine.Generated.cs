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
	/// Class     : PriceGroupLine
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
	public partial class PriceGroupLine : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdLine { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdGroup { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Percentage { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public PriceGroup Group { get; set; } 

        public Line Line { get; set; } 

        public IList<PriceGroupLineClient> ListPriceGroupLineClients { get; set; }


		#endregion

		#region Contructors 

		public PriceGroupLine() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PriceGroupLine
    /// </summary>
    /// <remarks></remarks>
    public enum relPriceGroupLine 
     { 
        Group, Line, PriceGroupLineClients
	}

}