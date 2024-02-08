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
	/// Class     : StatusDetail
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
	public partial class StatusDetail : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdStatus { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdStatusUsed { get; set; }

        public long? IdAward { get; set; }

        public long? IdNote { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Points { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal ExtraPoints { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal ExtraPointsPeriod { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal AcceleratorPeriod { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal TotalPoints { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal TotalAmount { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public ClaimedAward Award { get; set; } 

        public ClientNote Note { get; set; } 

        public Status Status { get; set; } 

        public Status StatusUsed { get; set; } 


		#endregion

		#region Contructors 

		public StatusDetail() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator StatusDetail
    /// </summary>
    /// <remarks></remarks>
    public enum relStatusDetail 
     { 
        Award, Note, Status, StatusUsed
	}

}