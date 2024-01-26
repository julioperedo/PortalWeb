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
	/// Class     : Price
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Product
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  10/8/2023 11:20:15 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Price : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSudsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Regular { get; set; }

        public decimal? ClientSuggested { get; set; }

        public string Observations { get; set; }

        public string Commentaries { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Product Product { get; set; } 

        public Base.Classifier Sudsidiary { get; set; }


		#endregion

		#region Contructors 

		public Price() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Price
    /// </summary>
    /// <remarks></remarks>
    public enum relPrice 
     { 
        Product, Sudsidiary
	}

}