using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEN = BEntities.Campaign;

namespace DALayer.Campaign
{
    public partial class Category
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEN.Category SearchByUser(long IdUser, params Enum[] Relations)
        {
            string query = $@"SELECT  c.*
FROM    Campaign.Category c
        INNER JOIN Campaign.ClientCategory cc ON c.Id = cc.IdCategory
        INNER JOIN Security.[User] u ON cc.CardCode = u.CardCode
        INNER JOIN Campaign.[User] u1 ON u.EMail = u1.EMail
WHERE   u1.Id = {IdUser} ";
            BEN.Category Item = SQLSearch(query, Relations);
            return Item;
        }

        #endregion

    }
}