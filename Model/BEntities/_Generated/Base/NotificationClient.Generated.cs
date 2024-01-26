using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Base 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Base
	/// Class     : NotificationClient
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Base
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:18 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class NotificationClient : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdNotification { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Notification Notification { get; set; } 


		#endregion

		#region Contructors 

		public NotificationClient() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator NotificationClient
    /// </summary>
    /// <remarks></remarks>
    public enum relNotificationClient 
     { 
        Notification
	}

}