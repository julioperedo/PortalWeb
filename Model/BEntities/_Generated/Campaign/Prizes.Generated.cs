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
	/// Class     : Prizes
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Campaign
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  28/7/2023 15:42:23 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Prizes : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Points { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdCampaign { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Info Campaign { get; set; } 


		#endregion

		#region Contructors 

		public Prizes() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Prizes
    /// </summary>
    /// <remarks></remarks>
    public enum relPrizes 
     { 
        Campaign
	}

}