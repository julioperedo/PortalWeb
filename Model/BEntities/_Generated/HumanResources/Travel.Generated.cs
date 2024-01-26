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
    /// Class     : Travel
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
    public partial class Travel : BEntity
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

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Destiny { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Request Request { get; set; }

        public IList<TravelReplacement> ListTravelReplacements { get; set; }


        #endregion

        #region Contructors 

        public Travel() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator Travel
    /// </summary>
    /// <remarks></remarks>
    public enum relTravel
    {
        Request, TravelReplacements
    }

}