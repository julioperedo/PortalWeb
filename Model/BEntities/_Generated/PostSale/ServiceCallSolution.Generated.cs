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
	/// Class     : ServiceCallSolution
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PostSale
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:36 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ServiceCallSolution : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdServiceCall { get; set; }

        public int? SAPCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(20, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ItemCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int StatusCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int OwnerCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Owner { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime CreateDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int UpdatedByCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string UpdatedBy { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime UpdateDate { get; set; }

        [StringLength(127, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Subject { get; set; }

        [StringLength(127, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Symtom { get; set; }

        [StringLength(127, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Cause { get; set; }

        public string Description { get; set; }

        public string Attachment { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public ServiceCall ServiceCall { get; set; } 


		#endregion

		#region Contructors 

		public ServiceCallSolution() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ServiceCallSolution
    /// </summary>
    /// <remarks></remarks>
    public enum relServiceCallSolution 
     { 
        ServiceCall
	}

}