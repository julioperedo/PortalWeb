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
	/// Class     : Person
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
	public partial class Person : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(20, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string DocumentId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Phone { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<BE.Security.UserPerson> ListUserPersons { get; set; }

        public IList<Picture> ListPictures { get; set; }

        public IList<Visit> ListVisits { get; set; }

        public IList<VisitReception> ListVisitReceptions { get; set; }


		#endregion

		#region Contructors 

		public Person() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Person
    /// </summary>
    /// <remarks></remarks>
    public enum relPerson 
     { 
        UserPersons, Pictures, Visits, VisitReceptions
	}

}