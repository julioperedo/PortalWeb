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
    public class OrderFile : DALEntity<BEA.OrderFile>
    {
        #region Global Variables

        #endregion

        #region Methods

        internal IEnumerable<BEA.OrderFile> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            IEnumerable<BEA.OrderFile> items = List(Keys, Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.OrderFile> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.OrderFile item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.OrderFile> List(IEnumerable<string> DocEntries, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  * ");
            sb.AppendLine($@"FROM    ({GetQuery()}) a ");
            if (DocEntries?.Count() > 0)
            {
                sb.AppendLine($@"WHERE   LOWER(a.""Subsidiary"") || '-' || CAST(a.""DocEntry"" AS VARCHAR(5000)) IN ( {string.Join(",", DocEntries.Select(x => $"'{x.Replace("'", "").ToLower()}'"))} ) ");
            }

            IEnumerable<BEA.OrderFile> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.OrderFile> ListByDeliveries(IEnumerable<string> DocEntries, params Enum[] Relations)
        {
            string query = $@"SELECT  *
                              FROM    ( SELECT    'Santa Cruz' AS ""Subsidiary"", t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS VARCHAR(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt""
                                        FROM      {DBSA}.ODLN t0
                                                  INNER JOIN {DBSA}.ATC1 t1 ON t0.""AtcEntry"" = t1.""AbsEntry""
                                        UNION
                                        SELECT    'Iquique' AS ""Subsidiary"", t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS VARCHAR(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt""
                                        FROM      {DBIQ}.ODLN t0
                                                  INNER JOIN {DBIQ}.ATC1 t1 ON t0.""AtcEntry"" = t1.""AbsEntry""
                                        UNION
                                        SELECT    'Miami' AS ""Subsidiary"", t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS VARCHAR(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt""
                                        FROM      {DBLA}.ODLN t0
                                                  INNER JOIN {DBLA}.ATC1 t1 ON t0.""AtcEntry"" = t1.""AbsEntry"" ) a
                              WHERE   LOWER(a.""Subsidiary"") || '-' || CAST(a.""DocEntry"" AS VARCHAR(5000)) IN ( {string.Join(",", DocEntries.Select(x => $"'{x.ToLower()}'"))} ) ";
            IEnumerable<BEA.OrderFile> items = SQLList(query, Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.OrderFile Search(string Subsidiary, int DocEntry, string FileName, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery()}) a ");
            sb.AppendLine($@"WHERE    LOWER(a.""Subsidiary"") = '{Subsidiary.ToLower()}' AND a.""DocEntry"" = {DocEntry} AND (IFNULL(a.""FileName"", '') || '.' || IFNULL(a.""FileExt"", '')) = '{FileName}' ");

            BEA.OrderFile item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Private Methods

        private string GetQuery()
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  'Santa Cruz' AS ""Subsidiary"", t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" ");
            sb.AppendLine($@"FROM    {DBSA}.ORDR t0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'Iquique', t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" ");
            sb.AppendLine($@"FROM    {DBIQ}.ORDR t0 ");
            sb.AppendLine($@"        INNER JOIN {DBIQ}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  'Miami', t0.""DocEntry"", CAST(IFNULL(T1.""trgtPath"", '') AS varchar(5000)) AS ""Path"", IFNULL(T1.""FileName"", '') AS ""FileName"", IFNULL(T1.""FileExt"", '') AS ""FileExt"" ");
            sb.AppendLine($@"FROM    {DBLA}.ORDR t0 ");
            sb.AppendLine($@"        INNER JOIN {DBLA}.ATC1 T1 ON T1.""AbsEntry"" = t0.""AtcEntry"" ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public OrderFile() : base() { }

        #endregion
    }
}
