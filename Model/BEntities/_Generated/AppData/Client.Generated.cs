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
	/// Class     : Client
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table AppData
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:10 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Client : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Version { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public Client() { }

		#endregion

	}


}