using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class LastNotification
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.LastNotification> List(long IdUser)
        {
            string query = $@"SELECT *
                              FROM	 Security.LastNotification ln
                              WHERE	 ln.LogUser = {IdUser}
		                             AND EXISTS ( SELECT *
						                          FROM	 Base.Notification n
						                          WHERE  ln.IdNotification = n.Id
									                     AND n.Enabled = 1
									                     AND CAST(GETDATE() AS DATE) BETWEEN CAST(ISNULL(n.InitialDate, GETDATE()) AS DATE) AND CAST(ISNULL(n.FinalDate, GETDATE()) AS DATE) ) ";
            List<BES.LastNotification> Items = Connection.Query<BES.LastNotification>(query).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}