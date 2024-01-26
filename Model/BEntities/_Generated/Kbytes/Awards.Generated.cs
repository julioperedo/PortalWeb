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
	/// Class     : Awards
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
	public partial class Awards : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Points { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<ClaimedAward> ListClaimedAwards { get; set; }


		#endregion

		#region Contructors 

		public Awards() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Awards
    /// </summary>
    /// <remarks></remarks>
    public enum relAwards 
     { 
        ClaimedAwards
	}

}