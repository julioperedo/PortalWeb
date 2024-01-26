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
	/// Class     : DocumentFile
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  28/07/2022 13:40:20 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class DocumentFile : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdDocument { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FileURL { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Document Document { get; set; } 


		#endregion

		#region Contructors 

		public DocumentFile() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator DocumentFile
    /// </summary>
    /// <remarks></remarks>
    public enum relDocumentFile 
     { 
        Document
	}

}