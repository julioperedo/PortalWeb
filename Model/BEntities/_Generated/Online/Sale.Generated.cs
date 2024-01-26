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
	/// Class     : Sale
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Online
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:31 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Sale : BEntity
	{

		#region Properties 

		public long Id { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public long IdUser { get; set; }

		[StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string Code { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
		[StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string Name { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
		public string Address { get; set; }

		public string Commentaries { get; set; }

		[StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string ClientSaleNote { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public decimal Total { get; set; }

		[StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string SellerCode { get; set; }

		[StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string SellerName { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public long StateIdc { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public bool WithDropShip { get; set; }

		public long LogUser { get; set; }

		public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

		public Security.User User { get; set; }

		public Base.Classifier State { get; set; }

		public IList<SaleDetail> ListSaleDetails { get; set; }

		public IList<SaleFiles> ListSaleFiless { get; set; }

		public IList<SaleState> ListSaleStates { get; set; }


		#endregion

		#region Contructors 

		public Sale() { }

		#endregion

	}

	/// <summary>
	/// Relationship enumerator Sale
	/// </summary>
	/// <remarks></remarks>
	public enum relSale
	{
		User, State, SaleDetails, SaleFiless, SaleStates
	}

}