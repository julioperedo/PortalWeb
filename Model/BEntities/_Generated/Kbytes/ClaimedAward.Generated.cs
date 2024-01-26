using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Kbytes 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Kbytes
	/// Class     : ClaimedAward
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:25 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ClaimedAward : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdAward { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime ClaimDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Points { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Awards Award { get; set; } 

        public IList<StatusDetail> ListStatusDetails { get; set; }


		#endregion

		#region Contructors 

		public ClaimedAward() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ClaimedAward
    /// </summary>
    /// <remarks></remarks>
    public enum relClaimedAward 
     { 
        Award, StatusDetails
	}

}