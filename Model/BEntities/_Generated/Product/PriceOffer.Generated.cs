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
    /// Class     : PriceOffer
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table Product
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  10/1/2023 13:36:54 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class PriceOffer : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdSubsidiary { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public decimal Price { get; set; }

        [StringLength(255, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public System.DateTime? Since { get; set; }

        public System.DateTime? Until { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool OnlyWithStock { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Product Product { get; set; }

        public Base.Classifier Subsidiary { get; set; }


        #endregion

        #region Contructors 

        public PriceOffer() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator PriceOffer
    /// </summary>
    /// <remarks></remarks>
    public enum relPriceOffer
    {
        Product, Subsidiary
    }

}