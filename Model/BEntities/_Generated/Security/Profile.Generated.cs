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
	/// Class     : Profile
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
	public partial class Profile : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool isBase { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool isExternalCapable { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<ProfileActivity> ListProfileActivitys { get; set; }

        public IList<ProfileChart> ListProfileCharts { get; set; }

        public IList<ProfilePage> ListProfilePages { get; set; }

        public IList<User> ListUsers { get; set; }

        public IList<UserProfile> ListUserProfiles { get; set; }


		#endregion

		#region Contructors 

		public Profile() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Profile
    /// </summary>
    /// <remarks></remarks>
    public enum relProfile 
     { 
        ProfileActivitys, ProfileCharts, ProfilePages, Users, UserProfiles
	}

}