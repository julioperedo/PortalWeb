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
	/// Class     : Page
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Base
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:18 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Page : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Area { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Controller { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Action { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Params { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSModule { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public SModule SModule { get; set; } 

        public IList<Menu> ListMenus { get; set; }

        public IList<BE.Security.ProfilePage> ListProfilePages { get; set; }


		#endregion

		#region Contructors 

		public Page() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Page
    /// </summary>
    /// <remarks></remarks>
    public enum relPage 
     { 
        SModule, Menus, ProfilePages
	}

}