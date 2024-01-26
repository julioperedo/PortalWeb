using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.PiggyBank 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : PiggyBank
	/// Class     : Serial
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PiggyBank
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  26/7/2023 11:16:21 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Serial : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool IsScanned { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime RegisterDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string State { get; set; }

        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string RejectReason { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardName { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ItemCode { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ItemName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Points { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public User User { get; set; } 


		#endregion

		#region Contructors 

		public Serial() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Serial
    /// </summary>
    /// <remarks></remarks>
    public enum relSerial 
     { 
        User
	}

}