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
    /// Class     : TravelReplacement
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
    public partial class TravelReplacement : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdTravel { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdReplacement { get; set; }

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

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string IdState { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Employee Replacement { get; set; }

        public Travel Travel { get; set; }


        #endregion

        #region Contructors 

        public TravelReplacement() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator TravelReplacement
    /// </summary>
    /// <remarks></remarks>
    public enum relTravelReplacement
    {
        Replacement, Travel
    }

}