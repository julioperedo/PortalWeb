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
	/// Class     : Projection
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Sales
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:44 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Projection : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Subsidiary { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Division { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Amount { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public Projection() { }

		#endregion

	}


}