using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Visits 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Visits
	/// Class     : Visit
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Visits
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:59 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Visit : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long VisitorId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        public System.DateTime? FinalDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long StaffId { get; set; }

        public string ReasonVisit { get; set; }

        [StringLength(20, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LicencePlate { get; set; }

        public string Commentaries { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientDescription { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public AppData.StaffMembers Staff { get; set; }

        public Person Visitor { get; set; } 


		#endregion

		#region Contructors 

		public Visit() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Visit
    /// </summary>
    /// <remarks></remarks>
    public enum relVisit 
     { 
        Staff, Visitor
	}

}