using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.AppData 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : AppData
	/// Class     : MessageRecipients
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table AppData
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  11/9/2023 16:07:51 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class MessageRecipients : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdMessage { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Recipient { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Message Message { get; set; } 


		#endregion

		#region Contructors 

		public MessageRecipients() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator MessageRecipients
    /// </summary>
    /// <remarks></remarks>
    public enum relMessageRecipients 
     { 
        Message
	}

}