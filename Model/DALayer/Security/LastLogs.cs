using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEB = BEntities.Base;
using BEP = BEntities.Product;
using BEA = BEntities.SAP;
using BES = BEntities.Security;

using DALayer.Base;
using DALayer.Product;


using System.Data.Common;
using BEntities.Filters;
using Dapper;
using System.Data.SqlClient;

namespace DALayer.Security
{
    [Serializable]
    public class LastLogs : DALEntity<BES.LastLogs>
    {

        #region Methods

        protected override void LoadRelations(ref BES.LastLogs Item, params Enum[] Relations)
        {
            //throw new NotImplementedException();
        }

        protected override void LoadRelations(ref IEnumerable<BES.LastLogs> Items, params Enum[] Relations)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region List Methods

        public List<BES.LastLogs> List(DateTime? Until)
        {
            DynamicParameters parameters = new();
            StringBuilder sb = new();
            sb.AppendLine("SELECT   * ");
            sb.AppendLine("FROM     Security.vwLastLogs l ");
            sb.AppendLine("WHERE    LastLog IS NOT NULL ");
            if (Until.HasValue)
            {
                sb.AppendLine("         AND CAST(LastLog AS DATE) <= @Until ");
                parameters.Add("Until", Until.Value.ToString("yyyy/MM/dd"));
            }
            sb.AppendLine("ORDER BY LastLog");

            List<BES.LastLogs> Collection = SQLList(sb.ToString(), parameters).AsList();
            return Collection;
        }



        #endregion

        #region  Constructors 

        public LastLogs() : base("Negocio") { }

        public LastLogs(string StringConnection) : base(StringConnection) { }

        internal LastLogs(SqlConnection connection) : base(connection) { }

        #endregion
    }
}
