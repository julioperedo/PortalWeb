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
	/// Class     : NotificationDetail
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
	public partial class NotificationDetail : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdNotification { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdLine { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product.Line Line { get; set; }

        public Notification Notification { get; set; } 


		#endregion

		#region Contructors 

		public NotificationDetail() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator NotificationDetail
    /// </summary>
    /// <remarks></remarks>
    public enum relNotificationDetail 
     { 
        Line, Notification
	}

}