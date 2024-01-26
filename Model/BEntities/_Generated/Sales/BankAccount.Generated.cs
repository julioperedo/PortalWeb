using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Sales 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Sales
	/// Class     : BankAccount
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Sales
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  2/3/2023 15:27:26 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class BankAccount : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Subsidiary { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Holder { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Bank { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Currency { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Number { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Country { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Type { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ABANumber { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Swift { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Comments { get; set; }

        public string QR { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public BankAccount() { }

		#endregion

	}


}