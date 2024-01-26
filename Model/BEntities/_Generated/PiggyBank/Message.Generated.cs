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
	/// Class     : Message
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PiggyBank
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  16/9/2023 20:22:10 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Message : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime Date { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string RecipientsType { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageUrl { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<MessageRecipients> ListMessageRecipientss { get; set; }


		#endregion

		#region Contructors 

		public Message() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Message
    /// </summary>
    /// <remarks></remarks>
    public enum relMessage 
     { 
        MessageRecipientss
	}

}