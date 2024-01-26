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
	/// Class     : Status
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:29 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Status : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Points { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<ClientStatus> ListClientStatuss { get; set; }

        public IList<StatusDetail> ListStatusDetail_Statuss { get; set; }

        public IList<StatusDetail> ListStatusDetail_StatusUseds { get; set; }


		#endregion

		#region Contructors 

		public Status() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Status
    /// </summary>
    /// <remarks></remarks>
    public enum relStatus 
     { 
        ClientStatuss, StatusDetail_Statuss, StatusDetail_StatusUseds
	}

}