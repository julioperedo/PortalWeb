using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{

    [Serializable()]
    public class Serial : DALEntity<BEA.Serial>
    {
        #region Methods

        protected override void LoadRelations(ref BEA.Serial Item, params Enum[] Relations) { }

        protected override void LoadRelations(ref IEnumerable<BEA.Serial> Items, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Serial> List(List<Field> Filters, string OrderBy, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT      * ");
            sb.AppendLine($@"FROM        ( SELECT  'Santa Cruz' AS ""Subsidiary"", b.""CardCode"" AS ""ClientCode"", b.""CardName"" AS ""ClientName"", b.""DocNum"", CAST(b.""DocDate"" AS DATE) AS ""DocDate"" ");
            sb.AppendLine($@"                       , b.""DocType"", b.""ItemCode"", b.""ItemName"", c.""MnfSerial"" AS ""SerialNumber"", ( CASE b.""DocType"" WHEN 15 THEN g.""PriceAfVAT"" ELSE e.""PriceAfVAT"" END ) AS ""Price"" ");
            sb.AppendLine($@"              FROM    {DBSA}.ITL1 a ");
            sb.AppendLine($@"                      INNER JOIN {DBSA}.OITL b ON a.""LogEntry"" = b.""LogEntry"" ");
            sb.AppendLine($@"                      INNER JOIN {DBSA}.OSRN c ON a.""ItemCode"" = c.""ItemCode"" AND a.""SysNumber"" = c.""SysNumber"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBSA}.OINV d ON b.""DocNum"" = d.""DocNum"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBSA}.INV1 e ON d.""DocEntry"" = e.""DocEntry"" AND e.""ItemCode"" = b.""ItemCode"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBSA}.ODLN f ON b.""DocNum"" = f.""DocNum"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBSA}.DLN1 g ON f.""DocEntry"" = g.""DocEntry"" AND g.""ItemCode"" = b.""ItemCode"" ");
            sb.AppendLine($@"              WHERE   c.""MnfSerial"" IS NOT NULL ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT  'Iquique' AS ""Subsidiary"", b.""CardCode"", b.""CardName"", b.""DocNum"", CAST(b.""DocDate"" AS DATE) AS ""DocDate"" ");
            sb.AppendLine($@"                       , b.""DocType"", b.""ItemCode"", b.""ItemName"", c.""MnfSerial"" AS ""SerialNumber"", ( CASE b.""DocType"" WHEN 15 THEN g.""PriceAfVAT"" ELSE e.""PriceAfVAT"" END ) AS ""Price"" ");
            sb.AppendLine($@"              FROM    {DBIQ}.ITL1 a ");
            sb.AppendLine($@"                      INNER JOIN {DBIQ}.OITL b ON a.""LogEntry"" = b.""LogEntry"" ");
            sb.AppendLine($@"                      INNER JOIN {DBIQ}.OSRN c ON a.""ItemCode"" = c.""ItemCode"" AND a.""SysNumber"" = c.""SysNumber"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBIQ}.OINV d ON b.""DocNum"" = d.""DocNum"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBIQ}.INV1 e ON d.""DocEntry"" = e.""DocEntry"" AND e.""ItemCode"" = b.""ItemCode"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBIQ}.ODLN f ON b.""DocNum"" = f.""DocNum"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBIQ}.DLN1 g ON f.""DocEntry"" = g.""DocEntry"" AND g.""ItemCode"" = b.""ItemCode"" ");
            sb.AppendLine($@"              WHERE   c.""MnfSerial"" IS NOT NULL ");
            sb.AppendLine($@"              UNION ALL ");
            sb.AppendLine($@"              SELECT  'Miami' AS ""Subsidiary"", b.""CardCode"", b.""CardName"", b.""DocNum"", CAST(b.""DocDate"" AS DATE) AS ""DocDate"" ");
            sb.AppendLine($@"               , b.""DocType"", b.""ItemCode"", b.""ItemName"", c.""MnfSerial"" AS ""SerialNumber"", ( CASE b.""DocType"" WHEN 15 THEN g.""PriceAfVAT"" ELSE e.""PriceAfVAT"" END ) AS ""Price"" ");
            sb.AppendLine($@"              FROM    {DBLA}.ITL1 a ");
            sb.AppendLine($@"                      INNER JOIN {DBLA}.OITL b ON a.""LogEntry"" = b.""LogEntry"" ");
            sb.AppendLine($@"                      INNER JOIN {DBLA}.OSRN c ON a.""ItemCode"" = c.""ItemCode"" AND   a.""SysNumber"" = c.""SysNumber"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBLA}.OINV d ON b.""DocNum"" = d.""DocNum"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBLA}.INV1 e ON d.""DocEntry"" = e.""DocEntry"" AND e.""ItemCode"" = b.""ItemCode"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBLA}.ODLN f ON b.""DocNum"" = f.""DocNum"" ");
            sb.AppendLine($@"                      LEFT OUTER JOIN {DBLA}.DLN1 g ON f.""DocEntry"" = g.""DocEntry"" AND g.""ItemCode"" = b.""ItemCode"" ");
            sb.AppendLine($@"              WHERE   c.""MnfSerial"" IS NOT NULL ) t ");
            if (Filters?.Count > 0) sb.AppendLine($@"WHERE    {GetFilter(Filters.ToArray())} ");
            sb.AppendLine($@"ORDER BY {GetOrder(OrderBy)} ");

            IEnumerable<BEA.Serial> items = base.SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region  Private Methods

        #endregion

        #region Constructors

        public Serial() : base() { }

        #endregion
    }
}
