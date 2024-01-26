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
    /// Class     : RelatedCategory
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table Product
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  7/9/2022 16:09:18 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class RelatedCategory : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Category { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Related { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 


        #endregion

        #region Contructors 

        public RelatedCategory() { }

        #endregion

    }


}