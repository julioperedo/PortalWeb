using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Sales 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Sales
	/// Class     : TransportSent
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Sales
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:49 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class TransportSent : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdTransport { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Tos { get; set; }

        public string CCs { get; set; }

        public string BCCs { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        public string Body { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Transport Transport { get; set; } 


		#endregion

		#region Contructors 

		public TransportSent() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator TransportSent
    /// </summary>
    /// <remarks></remarks>
    public enum relTransportSent 
     { 
        Transport
	}

}