using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Security 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Security
	/// Class     : WebServiceClient
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Security
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:57 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class WebServiceClient : BEntity 
	{

		#region Properties 

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string CardCode { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 


		#endregion

		#region Contructors 

		public WebServiceClient() { }

		#endregion

	}


}