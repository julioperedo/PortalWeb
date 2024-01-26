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
	/// Class     : Replace
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
	public partial class Replace : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SellerCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ReplaceCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime FinalDate { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public Replace() { }

		#endregion

	}


}