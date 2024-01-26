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
	/// Class     : UserPivotConfig
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:55 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class UserPivotConfig : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        public string Rows { get; set; }

        public string Columns { get; set; }

        public string Values { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public User User { get; set; } 


		#endregion

		#region Contructors 

		public UserPivotConfig() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator UserPivotConfig
    /// </summary>
    /// <remarks></remarks>
    public enum relUserPivotConfig 
     { 
        User
	}

}