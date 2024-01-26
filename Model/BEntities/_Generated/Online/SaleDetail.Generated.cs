using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Online 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Online
	/// Class     : SaleDetail
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Online
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:32 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class SaleDetail : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSale { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        public int? ApprovedQuantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSubsidiary { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Warehouse { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool OpenBox { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product.Product Product { get; set; }

        public Sale Sale { get; set; } 

        public Base.Classifier Subsidiary { get; set; }


		#endregion

		#region Contructors 

		public SaleDetail() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator SaleDetail
    /// </summary>
    /// <remarks></remarks>
    public enum relSaleDetail 
     { 
        Product, Sale, Subsidiary
	}

}