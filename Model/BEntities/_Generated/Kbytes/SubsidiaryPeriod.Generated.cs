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
	/// Class     : SubsidiaryPeriod
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:30 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class SubsidiaryPeriod : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(20, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Subsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        public System.DateTime? FinalDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool JustForElders { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool OnlyCash { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public SubsidiaryPeriod() { }

		#endregion

	}


}