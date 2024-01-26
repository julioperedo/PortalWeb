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
	/// Class     : Employee
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
	public partial class Employee : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ShortName { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Position { get; set; }

        public int? HierarchyLevel { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Mail { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Photo { get; set; }

        public int? Phone { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<EmployeeDepartment> ListEmployeeDepartment_Managers { get; set; }

        public IList<EmployeeDepartment> ListEmployeeDepartment_Employees { get; set; }

        public IList<Request> ListRequests { get; set; }

        public IList<TravelReplacement> ListTravelReplacements { get; set; }

        public IList<VacationReplacement> ListVacationReplacements { get; set; }

        public IList<BE.Security.UserData> ListUserDatas { get; set; }


		#endregion

		#region Contructors 

		public Employee() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Employee
    /// </summary>
    /// <remarks></remarks>
    public enum relEmployee 
     { 
        EmployeeDepartment_Managers, EmployeeDepartment_Employees, Requests, TravelReplacements, VacationReplacements, UserDatas
	}

}