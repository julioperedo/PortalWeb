using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Staff 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Staff
	/// Class     : Member
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Staff
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:58 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Member : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ShortName { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Position { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Mail { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Photo { get; set; }

        public int? Phone { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Manager { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdDepartment { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Order { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Department Department { get; set; } 


		#endregion

		#region Contructors 

		public Member() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Member
    /// </summary>
    /// <remarks></remarks>
    public enum relMember 
     { 
        Department
	}

}