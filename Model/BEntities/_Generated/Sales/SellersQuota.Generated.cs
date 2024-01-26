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
	/// Class     : SellersQuota
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Sales
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:47 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class SellersQuota : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SalesmanCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Amount { get; set; }

        public string Commentaries { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public SellersQuota() { }

		#endregion

	}


}