using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.CIESD 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : CIESD
	/// Class     : AllowedClient
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table CIESD
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  20/4/2022 16:15:34 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class AllowedClient : BEntity 
	{

		#region Properties 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public AllowedClient() { }

		#endregion

	}


}