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
    /// Class     : Document
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table Product
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  28/07/2022 13:40:20 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Document : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdProduct { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long TypeIdc { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(150, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public System.DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public bool Enabled { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public Product Product { get; set; }

        public Base.Classifier Type { get; set; }

        public IList<DocumentFile> ListDocumentFiles { get; set; }


        #endregion

        #region Contructors 

        public Document() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator Document
    /// </summary>
    /// <remarks></remarks>
    public enum relDocument
    {
        Product, Type, DocumentFiles
    }

}