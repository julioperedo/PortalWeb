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
	/// Class     : Menu
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Base
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:16 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Menu : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Order { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool isSeparator { get; set; }

        public long? IdPage { get; set; }

        public long? IdParent { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Icon { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Page Page { get; set; } 

        public Menu Parent { get; set; } 

        public IList<Menu> ListMenus { get; set; }


		#endregion

		#region Contructors 

		public Menu() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Menu
    /// </summary>
    /// <remarks></remarks>
    public enum relMenu 
     { 
        Page, Parent, Menus
	}

}