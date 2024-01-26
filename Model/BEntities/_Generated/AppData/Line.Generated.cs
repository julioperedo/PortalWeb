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
	/// Class     : Line
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
	public partial class Line : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ImageURL { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public Line() { }

		#endregion

	}


}