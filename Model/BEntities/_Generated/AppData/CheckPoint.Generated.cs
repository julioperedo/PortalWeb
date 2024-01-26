using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.AppData
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : AppData
	/// Class     : CheckPoint
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table AppData
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:10 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class CheckPoint : BEntity
	{

		#region Properties 

		public long Id { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public long IdGuard { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public double Latitude { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public double Longitude { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public double Altitude { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public double Accuracy { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
		[StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string Provider { get; set; }

		[Required(ErrorMessage = "Campo requerido")]
		public System.DateTime CheckDate { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
		[StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string Type { get; set; }

		[StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
		public string PointName { get; set; }

		public long LogUser { get; set; }

		public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public CheckPoint() { }

		#endregion

	}


}