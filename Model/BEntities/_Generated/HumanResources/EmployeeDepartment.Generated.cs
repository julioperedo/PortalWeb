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
	/// Class     : EmployeeDepartment
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table HumanResources
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/12/2023 14:06:20 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class EmployeeDepartment : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdEmployee { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdDepartment { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdManager { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Employee Manager { get; set; } 

        public Department Department { get; set; } 

        public Employee Employee { get; set; } 


		#endregion

		#region Contructors 

		public EmployeeDepartment() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator EmployeeDepartment
    /// </summary>
    /// <remarks></remarks>
    public enum relEmployeeDepartment 
     { 
        Manager, Department, Employee
	}

}