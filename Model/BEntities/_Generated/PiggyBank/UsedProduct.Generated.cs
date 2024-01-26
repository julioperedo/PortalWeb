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
	/// Class     : UsedProduct
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
	public partial class UsedProduct : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ItemCode { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public UsedProduct() { }

		#endregion

	}


}