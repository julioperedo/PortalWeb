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
    /// Class     : HomeOffice
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
    public partial class HomeOffice : BEntity 
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdRequest { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime FromDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(2, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FromDatePeriod { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime ToDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(2, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string ToDatePeriod { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Request Request { get; set; } 


        #endregion

        #region Contructors 

        public HomeOffice() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator HomeOffice
    /// </summary>
    /// <remarks></remarks>
    public enum relHomeOffice 
     { 
        Request
    }

}