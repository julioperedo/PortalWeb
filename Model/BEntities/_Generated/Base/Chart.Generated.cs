using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Base 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Base
	/// Class     : Chart
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Base
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:13 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Chart : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdcChartType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Classifier cChartType { get; set; } 

        public IList<BE.Security.ProfileChart> ListProfileCharts { get; set; }


		#endregion

		#region Contructors 

		public Chart() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Chart
    /// </summary>
    /// <remarks></remarks>
    public enum relChart 
     { 
        cChartType, ProfileCharts
	}

}