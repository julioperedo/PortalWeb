using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Campaign 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Campaign
	/// Class     : ClientCategory
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Campaign
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  27/7/2023 15:05:57 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ClientCategory : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdCategory { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Category Category { get; set; } 


		#endregion

		#region Contructors 

		public ClientCategory() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ClientCategory
    /// </summary>
    /// <remarks></remarks>
    public enum relClientCategory 
     { 
        Category
	}

}