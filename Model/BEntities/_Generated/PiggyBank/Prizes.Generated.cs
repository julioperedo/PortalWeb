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
	/// Class     : Prizes
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PiggyBank
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  23/10/2023 09:55:26 Created
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

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<ClaimedPrize> ListClaimedPrizes { get; set; }


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
        ClaimedPrizes
	}

}