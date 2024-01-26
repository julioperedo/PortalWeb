using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class OrdersResume : DALEntity<BEA.OrdersResume>
    {
        #region Global Variables

        #endregion

        #region Methods

        protected override void LoadRelations(ref IEnumerable<BEA.OrdersResume> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.OrdersResume item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.OrdersResume> List(DateTime InitialDate, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT      ""CardCode"", COUNT(*) AS ""Total"" ");
            sb.AppendLine($@"FROM        ( SELECT    ""CardCode"", ""DocEntry"" ");
            sb.AppendLine($@"              FROM      {DBSA}.ORDR ");
            sb.AppendLine($@"              WHERE     CAST(""DocDate"" AS DATE) >= '{InitialDate:yyyy-MM-dd}' AND CANCELED = 'N' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    ""CardCode"", ""DocEntry"" ");
            sb.AppendLine($@"              FROM      {DBIQ}.ORDR ");
            sb.AppendLine($@"              WHERE     CAST(""DocDate"" AS DATE) >= '{InitialDate:yyyy-MM-dd}' AND CANCELED = 'N' ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT    ""CardCode"", ""DocEntry"" ");
            sb.AppendLine($@"              FROM      {DBLA}.ORDR ");
            sb.AppendLine($@"              WHERE     CAST(""DocDate"" AS DATE) >= '{InitialDate:yyyy-MM-dd}' AND CANCELED = 'N' ) a ");
            sb.AppendLine($@"GROUP BY    a.""CardCode"" ");
            sb.AppendLine($@"HAVING      COUNT(*) > 0 ");
            sb.AppendLine($@"ORDER BY    1 ");

            IEnumerable<BEA.OrdersResume> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Constructors

        public OrdersResume() : base() { }

        #endregion
    }
}
