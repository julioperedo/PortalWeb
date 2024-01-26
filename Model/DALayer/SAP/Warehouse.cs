using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class Warehouse : DALEntity<BEA.Item>
    {
        #region Global Variables

        #endregion

        #region Methods

        protected override void LoadRelations(ref IEnumerable<BEA.Item> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.Item item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Item> List(params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            sbQuery.AppendLine($@"SELECT  DISTINCT ""WhsCode"" AS ""Code"", ""WhsName"" AS ""Name"", ""Subsidiary"" AS ""Parent"" ");
            sbQuery.AppendLine($@"FROM    ({GetQuery()}) a ");
            sbQuery.AppendLine($@"ORDER By 1 ");
            IEnumerable<BEA.Item> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Item> ListIn(string Subsidiary, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            sbQuery.AppendLine($@"SELECT  DISTINCT ""WhsCode"" AS ""Code"", ""WhsName"" AS ""Name"", ""Subsidiary"" AS ""Parent"" ");
            sbQuery.AppendLine($@"FROM    ({GetQuery()}) a ");
            sbQuery.AppendLine($@"WHERE   ""Subsidiary"" IN ( {Subsidiary} ) ");
            sbQuery.AppendLine($@"ORDER By 1 ");
            IEnumerable<BEA.Item> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Private Methods

        private string GetQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  'Santa Cruz' AS ""Subsidiary"", T2.""WhsName"", T1.""WhsCode"" ");
            sb.AppendLine($@"FROM    {DBSA}.OITM T0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OITW T1 ON T0.""ItemCode"" = T1.""ItemCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OWHS T2 ON T1.""WhsCode"" = T2.""WhsCode"" ");
            sb.AppendLine($@"WHERE   T0.""frozenFor"" <> 'Y' AND   T0.U_SUBCATEG <> 'PORTATILES' ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'Miami', T2.""WhsName"", T1.""WhsCode"" ");
            sb.AppendLine($@"FROM    {DBLA}.OITM T0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OITW T1 ON T0.""ItemCode"" = T1.""ItemCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.OWHS T2 ON T1.""WhsCode"" = T2.""WhsCode"" ");
            sb.AppendLine($@"WHERE   T0.""frozenFor"" <> 'Y' AND   T0.U_SUBCATEG <> 'PORTATILES' ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'Iquique', T2.""WhsName"", T1.""WhsCode"" ");
            sb.AppendLine($@"FROM    {DBIQ}.OITM T0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OITW T1 ON T0.""ItemCode"" = T1.""ItemCode"" ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.OWHS T2 ON T1.""WhsCode"" = T2.""WhsCode"" ");
            sb.AppendLine($@"WHERE   T0.""frozenFor"" <> 'Y' AND   T0.U_SUBCATEG <> 'PORTATILES' ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public Warehouse() : base() { }

        #endregion
    }
}
