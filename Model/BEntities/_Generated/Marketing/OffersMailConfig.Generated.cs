using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Marketing 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Marketing
	/// Class     : OffersMailConfig
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Marketing
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:30 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class OffersMailConfig : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdLine { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int WeekDay { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product.Line Line { get; set; }


		#endregion

		#region Contructors 

		public OffersMailConfig() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator OffersMailConfig
    /// </summary>
    /// <remarks></remarks>
    public enum relOffersMailConfig 
     { 
        Line
	}

}