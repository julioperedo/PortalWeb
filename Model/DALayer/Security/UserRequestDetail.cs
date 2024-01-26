using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class UserRequestDetail
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.UserRequestDetail> List(long IdRequest, string Order)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(@"SELECT	d.*, c.Name AS StateName, u.FirstName + ' ' + u.LastName AS UserName ");
            sb.AppendLine(@"FROM	Security.UserRequestDetail d ");
            sb.AppendLine(@"		INNER JOIN Base.Classifier c ON d.StateIdc = c.Id ");
            sb.AppendLine(@"		INNER JOIN Security.[User] u ON d.LogUser = u.Id ");
            sb.AppendLine(@"WHERE	d.IdRequest = @IdRequest ");
            sb.AppendLine(@"ORDER BY " + Order);

            List<BES.UserRequestDetail> Items = SQLList(sb.ToString(), new { @IdRequest = IdRequest }).AsList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}