using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Base 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Base
	/// Class     : Activity
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Base
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:13 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Activity : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSModule { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public SModule SModule { get; set; } 

        public IList<BE.Security.ProfileActivity> ListProfileActivitys { get; set; }


		#endregion

		#region Contructors 

		public Activity() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Activity
    /// </summary>
    /// <remarks></remarks>
    public enum relActivity 
     { 
        SModule, ProfileActivitys
	}

}