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
	/// Class     : Transport
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Sales
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  4/3/2022 21:28:47 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Transport : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(20, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string DocNumber { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime Date { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long TransporterId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long SourceId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long DestinationId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string DeliveryTo { get; set; }

        [StringLength(250, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Observations { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int QuantityPieces { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal RemainingAmount { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long TypeId { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool WithCopies { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Sent { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public Base.Classifier Destination { get; set; }

        public Base.Classifier Source { get; set; }

        public Base.Classifier Transporter { get; set; }

        public Base.Classifier Type { get; set; }

        public IList<TransportDetail> ListTransportDetails { get; set; }

        public IList<TransportSent> ListTransportSents { get; set; }


		#endregion

		#region Contructors 

		public Transport() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Transport
    /// </summary>
    /// <remarks></remarks>
    public enum relTransport 
     { 
        Destination, Source, Transporter, Type, TransportDetails, TransportSents
	}

}