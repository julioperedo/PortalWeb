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
	/// Class     : Notification
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Base
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:17 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Notification : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        public System.DateTime? FinalDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Frequency { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Value { get; set; }

        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string MobileValue { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Popup { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<NotificationClient> ListNotificationClients { get; set; }

        public IList<NotificationDetail> ListNotificationDetails { get; set; }


		#endregion

		#region Contructors 

		public Notification() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Notification
    /// </summary>
    /// <remarks></remarks>
    public enum relNotification 
     { 
        NotificationClients, NotificationDetails
	}

}