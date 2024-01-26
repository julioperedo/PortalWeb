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
    public class Attachment : DALEntity<BEA.Attachment>
    {
        #region Global Variables

        #endregion

        #region Methods

        internal IEnumerable<BEA.Attachment> ReturnChild(IEnumerable<string> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  t0.""Line"", t0.""trgtPath"" AS ""Path"", t0.""FileName"", t0.""FileExt"", t0.""Date"" ");
            sb.AppendLine($@"FROM    {DBSA}.ATC1 t0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD t1 ON t0.""AbsEntry"" = t1.""AtcEntry"" ");
            sb.AppendLine($@"WHERE   t1.""CardCode"" IN ( {string.Join(",", Keys.Select(x => $"'{x}'"))} ) ");

            IEnumerable<BEA.Attachment> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.Attachment> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.Attachment item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Attachment> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var filter = GetFilter(FilterList?.ToArray());
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  t0.""Line"", t0.""trgtPath"" AS ""Path"", t0.""FileName"", t0.""FileExt"", t0.""Date"" ");
            sb.AppendLine($@"FROM    {DBSA}.ATC1 t0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD t1 ON t0.""AbsEntry"" = t1.""AtcEntry"" ");
            sb.AppendLine($@"WHERE	{filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)}");

            IEnumerable<BEA.Attachment> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Attachment Search(string CardCode, int Line, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  t0.""Line"", t0.""trgtPath"" AS ""Path"", t0.""FileName"", t0.""FileExt"", t0.""Date"" ");
            sb.AppendLine($@"FROM    {DBSA}.ATC1 t0 ");
            sb.AppendLine($@"        INNER JOIN {DBSA}.OCRD t1 ON t0.""AbsEntry"" = t1.""AtcEntry"" ");
            sb.AppendLine($@"WHERE	t1.""CardCode"" = '{CardCode}' AND t0.""Line"" = '{Line}' ");

            BEA.Attachment item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Constructors

        public Attachment() : base() { }

        #endregion
    }
}
