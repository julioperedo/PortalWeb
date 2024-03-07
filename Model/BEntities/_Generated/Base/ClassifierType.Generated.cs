using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.Base
{
    /// -----------------------------------------------------------------------------
    /// Project   : BEntities
    /// NameSpace : Base
    /// Class     : ClassifierType
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table Base
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  4/3/2022 21:28:15 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ClassifierType : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(50, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool isBase { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public IList<Classifier> ListClassifiers { get; set; }


        #endregion

        #region Contructors 

        public ClassifierType() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator ClassifierType
    /// </summary>
    /// <remarks></remarks>
    public enum relClassifierType
    {
        Classifiers
    }

}