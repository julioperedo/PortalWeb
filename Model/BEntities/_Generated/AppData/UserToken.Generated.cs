using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.AppData 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : AppData
	/// Class     : UserToken
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table AppData
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:12 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class UserToken : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Token { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Security.User User { get; set; }


		#endregion

		#region Contructors 

		public UserToken() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator UserToken
    /// </summary>
    /// <remarks></remarks>
    public enum relUserToken 
     { 
        User
	}

}