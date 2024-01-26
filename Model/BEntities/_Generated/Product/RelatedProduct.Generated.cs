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
    /// Class     : RelatedProduct
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table Product
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  7/9/2022 16:11:27 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class RelatedProduct : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdRelated { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Product Product { get; set; }

        public Product Related { get; set; }


        #endregion

        #region Contructors 

        public RelatedProduct() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator RelatedProduct
    /// </summary>
    /// <remarks></remarks>
    public enum relRelatedProduct
    {
        Product, Related
    }

}