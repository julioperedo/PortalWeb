using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Kbytes 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Kbytes
	/// Class     : ClientNotAllowed
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:26 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ClientNotAllowed : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        public System.DateTime? FinalDate { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public ClientNotAllowed() { }

		#endregion

	}


}