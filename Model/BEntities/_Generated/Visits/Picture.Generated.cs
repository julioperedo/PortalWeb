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
	/// Class     : Picture
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
	public partial class Picture : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Type { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPerson { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Person Person { get; set; } 


		#endregion

		#region Contructors 

		public Picture() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Picture
    /// </summary>
    /// <remarks></remarks>
    public enum relPicture 
     { 
        Person
	}

}