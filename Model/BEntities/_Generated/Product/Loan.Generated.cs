using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : BEntities
    /// NameSpace : Product
    /// Class     : Loan
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table Product
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  29/1/2024 14:11:10 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Loan : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime InitialDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime FinalDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(1, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string State { get; set; }

        public string Comments { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Product Product { get; set; }


        #endregion

        #region Contructors 

        public Loan() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator Loan
    /// </summary>
    /// <remarks></remarks>
    public enum relLoan
    {
        Product
    }

}