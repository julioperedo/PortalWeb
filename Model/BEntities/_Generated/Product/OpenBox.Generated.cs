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
	/// Class     : OpenBox
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:39 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class OpenBox : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSubsidiary { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Warehouse { get; set; }

        public string Comments { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product Product { get; set; } 

        public Base.Classifier Subsidiary { get; set; }


		#endregion

		#region Contructors 

		public OpenBox() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator OpenBox
    /// </summary>
    /// <remarks></remarks>
    public enum relOpenBox 
     { 
        Product, Subsidiary
	}

}