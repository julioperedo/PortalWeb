using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Online 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Online
	/// Class     : QuoteSent
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Online
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:31 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class QuoteSent : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string EMail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Body { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Security.User User { get; set; }


		#endregion

		#region Contructors 

		public QuoteSent() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator QuoteSent
    /// </summary>
    /// <remarks></remarks>
    public enum relQuoteSent 
     { 
        User
	}

}