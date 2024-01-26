using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.HumanResources 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : HumanResources
	/// Class     : Department
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table HumanResources
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/12/2023 14:06:19 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Department : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<EmployeeDepartment> ListEmployeeDepartments { get; set; }


		#endregion

		#region Contructors 

		public Department() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Department
    /// </summary>
    /// <remarks></remarks>
    public enum relDepartment 
     { 
        EmployeeDepartments
	}

}