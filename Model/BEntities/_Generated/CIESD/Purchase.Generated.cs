using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.CIESD 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : CIESD
	/// Class     : Purchase
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table CIESD
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  07/06/2022 10:15:27 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Purchase : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(11, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Price { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Currency { get; set; }

        public long? DocNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(2, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string DocType { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool PurchaseOrderNeeded { get; set; }

        public long? UserId { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product Product { get; set; } 

        public IList<Link> ListLinks { get; set; }

        public IList<SentEmail> ListSentEmails { get; set; }

        public IList<Token> ListTokens { get; set; }


		#endregion

		#region Contructors 

		public Purchase() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Purchase
    /// </summary>
    /// <remarks></remarks>
    public enum relPurchase 
     { 
        Product, Links, SentEmails, Tokens
	}

}