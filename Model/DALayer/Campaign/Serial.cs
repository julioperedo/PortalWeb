using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEN = BEntities.Campaign;

namespace DALayer.Campaign
{
    public partial class Serial
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        #endregion

        #region Search Methods

        public BEN.Serial PointsResume(long UserId, params Enum[] Relations)
        {
            string query = $@"SELECT  ISNULL(SUM(s.Points), 0) AS PointsSum, COUNT(*) AS SerialsCount
FROM    Campaign.Serial s
        INNER JOIN Campaign.[User] u ON s.IdUser = u.Id
WHERE   s.State = 'V' AND s.IdUser = {UserId} ";
            BEN.Serial Item = SQLSearch(query, Relations);
            return Item;
        }

        #endregion

    }
}