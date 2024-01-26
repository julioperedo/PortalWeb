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
	/// Class     : SaleState
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Online
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:33 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class SaleState : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSale { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long StateIdc { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Sale Sale { get; set; } 

        public Base.Classifier State { get; set; }


		#endregion

		#region Contructors 

		public SaleState() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator SaleState
    /// </summary>
    /// <remarks></remarks>
    public enum relSaleState 
     { 
        Sale, State
	}

}