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
	/// Class     : StaffMembers
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table AppData
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:11 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class StaffMembers : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Active { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<BE.Visits.Visit> ListVisits { get; set; }

        public IList<BE.Visits.VisitReception> ListVisitReceptions { get; set; }


		#endregion

		#region Contructors 

		public StaffMembers() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator StaffMembers
    /// </summary>
    /// <remarks></remarks>
    public enum relStaffMembers 
     { 
        Visits, VisitReceptions
	}

}