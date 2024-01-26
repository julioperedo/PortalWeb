using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.PostSale 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : PostSale
	/// Class     : ServiceCallActivity
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PostSale
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:35 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ServiceCallActivity : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdServiceCall { get; set; }

        public int? SAPCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime ActivityDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int TreatedByCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string TreatedBy { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int AssignedByCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string AssignedBy { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Closed { get; set; }

        public string Notes { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Details { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ActivityTypeCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ActivityType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(2, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SubjectCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Subject { get; set; }

        public int? ContactCode { get; set; }

        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Contact { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Telephone { get; set; }

        public string Attachment { get; set; }

        public long? LogUser { get; set; }

        public System.DateTime? LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public ServiceCall ServiceCall { get; set; } 


		#endregion

		#region Contructors 

		public ServiceCallActivity() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ServiceCallActivity
    /// </summary>
    /// <remarks></remarks>
    public enum relServiceCallActivity 
     { 
        ServiceCall
	}

}