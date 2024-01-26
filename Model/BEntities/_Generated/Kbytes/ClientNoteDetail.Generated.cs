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
	/// Class     : ClientNoteDetail
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Kbytes
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:27 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ClientNoteDetail : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdNote { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int AcceleratedQuantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal AcceleratedTotal { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Accelerator { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal ExtraPoints { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public ClientNote Note { get; set; } 

        public Product.Product Product { get; set; }


		#endregion

		#region Contructors 

		public ClientNoteDetail() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ClientNoteDetail
    /// </summary>
    /// <remarks></remarks>
    public enum relClientNoteDetail 
     { 
        Note, Product
	}

}