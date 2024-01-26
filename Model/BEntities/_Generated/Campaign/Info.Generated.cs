using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Campaign 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BEntities
	/// NameSpace : Campaign
	/// Class     : Info
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This Business Entity has all properties to work with a database table Campaign
	/// </summary>
	/// <remarks>	
	/// </remarks>
	/// <history>
	///   [DMC]  27/7/2023 15:05:58 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class Info : BEntity 
	{

		#region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


		#endregion

		#region Additional Properties 

        public IList<Category> ListCategorys { get; set; }

        public IList<Prizes> ListPrizess { get; set; }

        public IList<User> ListUsers { get; set; }


		#endregion

		#region Contructors 

		public Info() { }

		#endregion

	}

    /// <summary>
    /// Relationship enumerator Info
    /// </summary>
    /// <remarks></remarks>
    public enum relInfo 
     { 
        Categorys, Prizess, Users
	}

}