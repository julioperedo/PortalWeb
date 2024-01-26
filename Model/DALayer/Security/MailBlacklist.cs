using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BE = BEntities;
using BES = BEntities.Security;

namespace DALayer.Security
{
    public partial class MailBlacklist
    {

        #region Save Methods

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public List<BES.MailBlacklist> ListInContacts(string CardCode, string Order, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"SELECT	t0.* ");
            sb.AppendLine(@"FROM	Security.MailBlacklist t0 ");
            sb.AppendLine($@"WHERE	EXISTS ( SELECT * FROM Security.ClientContacts WHERE EMail = t0.EMail AND CardCode = '{CardCode}' ) ");
            sb.AppendLine($@"ORDER By {Order} ");

            List<BES.MailBlacklist> Items = SQLList(sb.ToString(), Relations).ToList();
            return Items;
        }

        #endregion

        #region Search Methods

        #endregion

    }
}