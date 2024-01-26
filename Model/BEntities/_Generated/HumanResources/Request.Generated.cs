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
    /// Class     : Request
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
    public partial class Request : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdEmployee { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime RequestDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string IdType { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdState { get; set; }

        public string Comments { get; set; }

        public string RejectComments { get; set; }

        public int? ExternalCode { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Base.Classifier State { get; set; }

        public Employee Employee { get; set; }

        public IList<HomeOffice> ListHomeOffices { get; set; }

        public IList<License> ListLicenses { get; set; }

        public IList<Travel> ListTravels { get; set; }

        public IList<Vacation> ListVacations { get; set; }


        #endregion

        #region Contructors 

        public Request() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator Request
    /// </summary>
    /// <remarks></remarks>
    public enum relRequest
    {
        State, Employee, HomeOffices, Licenses, Travels, Vacations
    }

}