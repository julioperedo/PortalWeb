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
	/// Class     : ProfileActivity
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
	public partial class ProfileActivity : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProfile { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdActivity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public short Permission { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Base.Activity Activity { get; set; }

        public Profile Profile { get; set; } 


		#endregion

		#region Contructors 

		public ProfileActivity() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ProfileActivity
    /// </summary>
    /// <remarks></remarks>
    public enum relProfileActivity 
     { 
        Activity, Profile
	}

}