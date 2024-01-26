using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEF = BEntities.Staff;

namespace DALayer.Staff
{
    public partial class Member
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEF.Member SearchByMail(string EMail, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM [Staff].[Member] WHERE LOWER(Mail) = '{EMail.ToLower()}'";
            BEF.Member Item = SQLSearch(strQuery, Relations);
            return Item;
        }

        #endregion

    }
}