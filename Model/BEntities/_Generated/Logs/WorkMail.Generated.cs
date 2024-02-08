using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Logs
{
    /// -----------------------------------------------------------------------------
    /// Project   : BEntities
    /// NameSpace : Logs
    /// Class     : WorkMail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table Logs
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  10/8/2022 11:13:47 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class WorkMail : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(60, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LastName { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Birthday { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string MaritalStatus { get; set; }

        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string IdentitynDoc { get; set; }

        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string City { get; set; }

        [StringLength(200, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Address { get; set; }

        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Cellphone { get; set; }

        [StringLength(15, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Phone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string EMail { get; set; }

        [StringLength(60, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string MeetRequirements { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string AcademicTraining { get; set; }

        [StringLength(2, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Experience { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Languages { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Hobbies { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string AboutYourself { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string References { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string WhyUs { get; set; }

        [StringLength(300, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Achievements { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LeavingReason { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LaboralExperience { get; set; }

        [StringLength(30, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Position { get; set; }

        [StringLength(10, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string SalaryPretension { get; set; }

        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string TravelAvailability { get; set; }

        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string LinkCV { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 


        #endregion

        #region Contructors 

        public WorkMail() { }

        #endregion

    }


}