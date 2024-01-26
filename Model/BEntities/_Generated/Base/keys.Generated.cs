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
	/// Class     : keys
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
	public partial class keys : BEntity 
	{

		#region Properties 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(70, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Tabla { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Contador { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public keys() { }

		#endregion

	}


}