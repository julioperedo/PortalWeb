using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Campaign 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Campaign
	/// Class     : User
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Campaign
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  27/7/2023 15:06:00 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class User : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string StoreName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string EMail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1000, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string City { get; set; }

        public string Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdCampaign { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Info Campaign { get; set; } 

        public IList<Serial> ListSerials { get; set; }


		#endregion

		#region Contructors 

		public User() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator User
    /// </summary>
    /// <remarks></remarks>
    public enum relUser 
     { 
        Campaign, Serials
	}

}