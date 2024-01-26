using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Product 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Product
	/// Class     : Line
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  27/04/2022 11:04:00 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Line : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageURL { get; set; }

        public bool HasExternalPrice { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ExternalSiteName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FilterType { get; set; }

        public bool WhenFilteredShowInfo { get; set; }

        public long? IdManager { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Staff.Member Manager { get; set; }

        public IList<BE.Base.NotificationDetail> ListNotificationDetails { get; set; }

        public IList<BE.Marketing.OffersMailConfig> ListOffersMailConfigs { get; set; }

        public IList<LineDetail> ListLineDetails { get; set; }

        public IList<BE.Security.LineNotAllowed> ListLineNotAlloweds { get; set; }


		#endregion

		#region Contructors 

		public Line() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Line
    /// </summary>
    /// <remarks></remarks>
    public enum relLine 
     { 
        Manager, NotificationDetails, OffersMailConfigs, LineDetails, LineNotAlloweds
	}

}