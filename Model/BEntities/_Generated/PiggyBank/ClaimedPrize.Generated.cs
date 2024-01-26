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
	/// Class     : ClaimedPrize
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PiggyBank
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  24/10/2023 16:45:31 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ClaimedPrize : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdPrize { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime ClaimDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Points { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Prizes Prize { get; set; } 

        public User User { get; set; } 


		#endregion

		#region Contructors 

		public ClaimedPrize() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ClaimedPrize
    /// </summary>
    /// <remarks></remarks>
    public enum relClaimedPrize 
     { 
        Prize, User
	}

}