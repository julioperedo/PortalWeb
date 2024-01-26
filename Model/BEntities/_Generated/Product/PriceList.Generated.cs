using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Product {
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Product
	/// Class     : PriceList
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  2/12/2019 12:35:53 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PriceList : BEntity {

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "*")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string URL { get; set; }

        [Required(ErrorMessage = "*")]
        public int Year { get; set; }

        [Required(ErrorMessage = "*")]
        public int Month { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public PriceList() { }

		#endregion

	}


}