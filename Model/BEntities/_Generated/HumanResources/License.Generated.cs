using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.HumanResources 
{
    /// -----------------------------------------------------------------------------
    /// Project   : BEntities
    /// NameSpace : HumanResources
    /// Class     : License
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table HumanResources
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  4/12/2023 14:06:20 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class License : BEntity 
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdRequest { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime Date { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public TimeSpan InitialTime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(5, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public TimeSpan FinalTime { get; set; }

        public long? IdReason { get; set; }

        public string ReasonDescription { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Base.Classifier Reason { get; set; }

        public Request Request { get; set; } 

        public IList<Vacation> ListVacations { get; set; }


        #endregion

        #region Contructors 

        public License() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator License
    /// </summary>
    /// <remarks></remarks>
    public enum relLicense 
     { 
        Reason, Request, Vacations
    }

}