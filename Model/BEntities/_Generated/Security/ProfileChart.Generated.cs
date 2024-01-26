using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Security 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Security
	/// Class     : ProfileChart
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:51 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ProfileChart : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdChart { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProfile { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Base.Chart Chart { get; set; }

        public Profile Profile { get; set; } 


		#endregion

		#region Contructors 

		public ProfileChart() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ProfileChart
    /// </summary>
    /// <remarks></remarks>
    public enum relProfileChart 
     { 
        Chart, Profile
	}

}