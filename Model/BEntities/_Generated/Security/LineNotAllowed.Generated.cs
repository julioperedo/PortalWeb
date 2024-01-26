using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Security 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Security
	/// Class     : LineNotAllowed
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:50 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class LineNotAllowed : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdLine { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product.Line Line { get; set; }


		#endregion

		#region Contructors 

		public LineNotAllowed() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator LineNotAllowed
    /// </summary>
    /// <remarks></remarks>
    public enum relLineNotAllowed 
     { 
        Line
	}

}