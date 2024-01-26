using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Kbytes 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Kbytes
	/// Class     : ClientNote
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:26 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ClientNote : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Subsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long Number { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime Date { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Amount { get; set; }

        [StringLength(20, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Terms { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<ClientNoteDetail> ListClientNoteDetails { get; set; }

        public IList<StatusDetail> ListStatusDetails { get; set; }


		#endregion

		#region Contructors 

		public ClientNote() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ClientNote
    /// </summary>
    /// <remarks></remarks>
    public enum relClientNote 
     { 
        ClientNoteDetails, StatusDetails
	}

}