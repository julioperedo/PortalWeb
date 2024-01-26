using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Product 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Product
	/// Class     : PriceGroupLineClient
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  2/8/2023 11:58:00 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PriceGroupLineClient : BEntity 
	{

		#region Properties 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdGroupLine { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public PriceGroupLine GroupLine { get; set; } 


		#endregion

		#region Contructors 

		public PriceGroupLineClient() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator PriceGroupLineClient
    /// </summary>
    /// <remarks></remarks>
    public enum relPriceGroupLineClient 
     { 
        GroupLine
	}

}