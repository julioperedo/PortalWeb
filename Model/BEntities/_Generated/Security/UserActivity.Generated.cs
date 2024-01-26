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
	/// Class     : UserActivity
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  11/05/2022 11:54:18 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class UserActivity : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdActivity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public short Permission { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Base.Activity Activity { get; set; }

        public User User { get; set; } 


		#endregion

		#region Contructors 

		public UserActivity() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator UserActivity
    /// </summary>
    /// <remarks></remarks>
    public enum relUserActivity 
     { 
        Activity, User
	}

}