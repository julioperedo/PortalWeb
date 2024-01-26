using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEntities;
using BE = BEntities;

namespace BEntities.PiggyBank
{
    /// -----------------------------------------------------------------------------
    /// Project   : BEntities
    /// NameSpace : PiggyBank
    /// Class     : UserToken
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///    This Business Entity has all properties to work with a database table PiggyBank
    /// </summary>
    /// <remarks>	
    /// </remarks>
    /// <history>
    ///   [DMC]  8/9/2023 16:25:57 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class UserToken : BEntity
    {

        #region Properties 

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public long IdUser { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo requerido")]
        [StringLength(500, ErrorMessage = "No debe exceder los {1} caracteres.")]
        public string Token { get; set; }

        public long LogUser { get; set; }

        public System.DateTime LogDate { get; set; }


        #endregion

        #region Additional Properties 

        public User User { get; set; }


        #endregion

        #region Contructors 

        public UserToken() { }

        #endregion

    }

    /// <summary>
    /// Relationship enumerator UserToken
    /// </summary>
    /// <remarks></remarks>
    public enum relUserToken
    {
        User
    }

}