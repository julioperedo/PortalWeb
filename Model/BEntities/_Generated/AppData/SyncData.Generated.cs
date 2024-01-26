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
	/// Class     : SyncData
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
	public partial class SyncData : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Version { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string App { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public SyncData() { }

		#endregion

	}


}