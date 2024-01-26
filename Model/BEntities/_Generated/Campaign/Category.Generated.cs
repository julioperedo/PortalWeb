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
	/// Class     : Category
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Campaign
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  27/7/2023 15:05:56 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Category : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal PricePercentage { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdCampaign { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Info Campaign { get; set; } 

        public IList<ClientCategory> ListClientCategorys { get; set; }


		#endregion

		#region Contructors 

		public Category() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Category
    /// </summary>
    /// <remarks></remarks>
    public enum relCategory 
     { 
        Campaign, ClientCategorys
	}

}