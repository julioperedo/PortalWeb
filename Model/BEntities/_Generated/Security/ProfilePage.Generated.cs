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
	/// Class     : ProfilePage
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:52 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ProfilePage : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProfile { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPage { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Base.Page Page { get; set; }

        public Profile Profile { get; set; } 


		#endregion

		#region Contructors 

		public ProfilePage() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ProfilePage
    /// </summary>
    /// <remarks></remarks>
    public enum relProfilePage 
     { 
        Page, Profile
	}

}