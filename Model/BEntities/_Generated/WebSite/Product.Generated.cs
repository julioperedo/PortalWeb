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
	/// Class     : Product
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
	public partial class Product : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CodeReference { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string DescriptionShort { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdManufacturer { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdCategory { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageName { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public Product() { }

		#endregion

	}


}