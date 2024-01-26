using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.CIESD 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : CIESD
	/// Class     : TokenReturn
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table CIESD
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  07/06/2022 10:15:28 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class TokenReturn : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdToken { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.Guid ServiceTransactionId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.Guid ClientTransactionId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime ReturnDate { get; set; }

        public long? OrderNumber { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Token Token { get; set; } 


		#endregion

		#region Contructors 

		public TokenReturn() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator TokenReturn
    /// </summary>
    /// <remarks></remarks>
    public enum relTokenReturn 
     { 
        Token
	}

}