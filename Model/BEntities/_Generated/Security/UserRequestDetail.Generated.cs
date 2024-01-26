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
	/// Class     : UserRequestDetail
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
	public partial class UserRequestDetail : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdRequest { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long StateIdc { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public UserRequest Request { get; set; } 

        public Base.Classifier State { get; set; }


		#endregion

		#region Contructors 

		public UserRequestDetail() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator UserRequestDetail
    /// </summary>
    /// <remarks></remarks>
    public enum relUserRequestDetail 
     { 
        Request, State
	}

}