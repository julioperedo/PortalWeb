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
	/// Class     : ServiceCall
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PostSale
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:34 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ServiceCall : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        public int? SAPCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int AssigneeCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Assignee { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int CallType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CityCode { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Country { get; set; }

        [StringLength(2, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime CreateDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ClientName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ItemCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string ItemName { get; set; }

        public int? LocationCode { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Location { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SerialNumber { get; set; }

        public string Resolution { get; set; }

        public System.DateTime? ResolutionDate { get; set; }

        public string Room { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime StartDate { get; set; }

        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string StateCode { get; set; }

        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int StatusCode { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Status { get; set; }

        public string Street { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int TechnicianCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Technician { get; set; }

        public System.DateTime? AdmissionDate { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string City2 { get; set; }

        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FinalUser { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FinalUserEMail { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FinalUserPhone { get; set; }

        public string DeliveredBy { get; set; }

        public string ExternalService { get; set; }

        public string ExternalServiceTechnician { get; set; }

        public string ExternalServiceNumber { get; set; }

        public string ExternalServiceAddress { get; set; }

        public string GuideNumber { get; set; }

        public string Transport { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int CountedPieces { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int PriorCountedPieces { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int DiffCountedPieces { get; set; }

        public System.DateTime? PurchaseDate { get; set; }

        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ReportedBy { get; set; }

        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ReceivedBy { get; set; }

        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string RefNV { get; set; }

        public string Comments { get; set; }

        public System.DateTime? CloseDate { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FileName { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FilePath { get; set; }

        public System.DateTime? DeliveredDate { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Brand { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Warranty { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Priority { get; set; }

        public int? OriginCode { get; set; }

        public int? ProblemTypeCode { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<ServiceCallActivity> ListServiceCallActivitys { get; set; }

        public IList<ServiceCallFile> ListServiceCallFiles { get; set; }

        public IList<ServiceCallSolution> ListServiceCallSolutions { get; set; }


		#endregion

		#region Contructors 

		public ServiceCall() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ServiceCall
    /// </summary>
    /// <remarks></remarks>
    public enum relServiceCall 
     { 
        ServiceCallActivitys, ServiceCallFiles, ServiceCallSolutions
	}

}