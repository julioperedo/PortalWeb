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
	/// Class     : SessionHistory
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:53 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class SessionHistory : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public User User { get; set; } 


		#endregion

		#region Contructors 

		public SessionHistory() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator SessionHistory
    /// </summary>
    /// <remarks></remarks>
    public enum relSessionHistory 
     { 
        User
	}

}