using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Security 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Security
	/// Class     : UserData
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  17/10/2023 14:27:43 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class UserData : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Signature { get; set; }

        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SellerCode { get; set; }

        public int? SAPUserId { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SAPUser { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SAPPassword { get; set; }

        public long? IdEmployee { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public HumanResources.Employee Employee { get; set; }

        public User User { get; set; } 


		#endregion

		#region Contructors 

		public UserData() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator UserData
    /// </summary>
    /// <remarks></remarks>
    public enum relUserData 
     { 
        Employee, User
	}

}