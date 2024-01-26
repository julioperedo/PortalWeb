using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BEI = BEntities.PiggyBank;

namespace DALayer.PiggyBank
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

        public BEI.Serial PointsResume(long UserId, params Enum[] Relations)
        {
            //string query = $@"SELECT SUM(Points) AS Points FROM [PiggyBank].[Serial] WHERE State = 'V' AND [IdUser] = ${UserId} ";
            string query = $@"SELECT  ISNULL(SUM(s.Points), 0) AS PointsSum, COUNT(*) AS SerialsCount
FROM    PiggyBank.Serial s
        INNER JOIN PiggyBank.[User] u ON s.IdUser = u.Id
WHERE   s.State = 'V' AND s.IdUser = {UserId} ";
            BEI.Serial Item = SQLSearch(query, Relations);
            return Item;
        }

        #endregion

    }
}