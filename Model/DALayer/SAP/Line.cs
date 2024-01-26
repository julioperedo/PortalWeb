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
    public class Line : DALEntity<BEA.Item>
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
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  DISTINCT a.U_LINEA AS ""Name"" ");
            sb.AppendLine($@"FROM    ( SELECT    T0.U_LINEA ");
            sb.AppendLine($@"          FROM      {DBSA}.OITM T0 ");
            sb.AppendLine($@"          WHERE     T0.""frozenFor"" <> 'Y' AND  T0.U_SUBCATEG <> 'PORTATILES' ");
            sb.AppendLine($@"          UNION ");
            sb.AppendLine($@"          SELECT    T0.U_LINEA ");
            sb.AppendLine($@"          FROM      {DBLA}.OITM T0 ");
            sb.AppendLine($@"          WHERE     T0.""frozenFor"" <> 'Y' AND  T0.U_SUBCATEG <> 'PORTATILES' ");
            sb.AppendLine($@"          UNION ");
            sb.AppendLine($@"          SELECT    T0.U_LINEA ");
            sb.AppendLine($@"          FROM      {DBIQ}.OITM T0 ");
            sb.AppendLine($@"          WHERE     T0.""frozenFor"" <> 'Y' AND  T0.U_SUBCATEG <> 'PORTATILES' ) AS a ");
            sb.AppendLine($@"WHERE   a.U_LINEA IS NOT NULL ");

            IEnumerable<BEA.Item> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Constructors

        public Line() : base() { }

        #endregion
    }
}
