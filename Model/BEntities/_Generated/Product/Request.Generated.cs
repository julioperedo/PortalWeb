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
	/// Class     : Request
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  11/3/2022 12:39:24 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Request : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSubsidiary { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime RequestDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Reported { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product Product { get; set; } 

        public Base.Classifier Subsidiary { get; set; }

        public Security.User User { get; set; }


		#endregion

		#region Contructors 

		public Request() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Request
    /// </summary>
    /// <remarks></remarks>
    public enum relRequest 
     { 
        Product, Subsidiary, User
	}

}