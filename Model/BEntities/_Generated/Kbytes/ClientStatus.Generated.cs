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
	/// Class     : ClientStatus
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  2/2/2024 14:27:47 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ClientStatus : BEntity 
	{

		#region Properties 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quarter { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdStatus { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Points { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Status Status { get; set; } 


		#endregion

		#region Contructors 

		public ClientStatus() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ClientStatus
    /// </summary>
    /// <remarks></remarks>
    public enum relClientStatus 
     { 
        Status
	}

}