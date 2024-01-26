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
    /// Class     : Vacation
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
    public partial class Vacation : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdRequest { get; set; }

        public long? IdLicense { get; set; }

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

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Days { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public License License { get; set; }

        public Request Request { get; set; }

        public IList<VacationReplacement> ListVacationReplacements { get; set; }


        #endregion

        #region Contructors 

        public Vacation() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator Vacation
    /// </summary>
    /// <remarks></remarks>
    public enum relVacation
    {
        License, Request, VacationReplacements
    }

}