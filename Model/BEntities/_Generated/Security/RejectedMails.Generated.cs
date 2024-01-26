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
	/// Class     : RejectedMails
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:52 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class RejectedMails : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Email { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientCode { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime RejectionDate { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Seller { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Reason { get; set; }

        public string Comments { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Reported { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public RejectedMails() { }

		#endregion

	}


}