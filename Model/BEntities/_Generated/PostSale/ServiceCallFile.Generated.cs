using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.PostSale 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : PostSale
	/// Class     : ServiceCallFile
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table PostSale
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:36 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class ServiceCallFile : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdServiceCall { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string FileName { get; set; }


		#endregion

		#region Additional Properties 

        public ServiceCall ServiceCall { get; set; } 


		#endregion

		#region Contructors 

		public ServiceCallFile() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator ServiceCallFile
    /// </summary>
    /// <remarks></remarks>
    public enum relServiceCallFile 
     { 
        ServiceCall
	}

}