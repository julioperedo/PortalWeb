using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.WebSite 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : WebSite
	/// Class     : Category
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table WebSite
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:29:01 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Category : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdParent { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Link { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Position { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public Category() { }

		#endregion

	}


}