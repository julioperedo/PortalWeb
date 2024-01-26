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
    public class Category : DALEntity<BEA.Item>
    {
        #region Global Variables

        #endregion

        #region Methods

        internal IEnumerable<BEA.Item> ReturnChild(IEnumerable<int> Keys, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT  ""ItmsGrpCod"" AS ""Id"", ""ItmsGrpNam"" AS ""Name"" ");
            sb.AppendLine($@"FROM    {DBSA}.OITB ");
            sb.AppendLine($@"WHERE  ""ItmsGrpCod"" IN ( {string.Join(",", Keys)} ) ");

            IEnumerable<BEA.Item> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        protected override void LoadRelations(ref IEnumerable<BEA.Item> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.Item item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Item> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            var filter = GetFilter(FilterList?.ToArray());
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""ItmsGrpCod"" AS ""Id"", ""ItmsGrpNam"" AS ""Name"" ");
            sb.AppendLine($@"FROM    {DBSA}.OITB ");
            sb.AppendLine($@"WHERE  {filter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)}");

            IEnumerable<BEA.Item> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Item Search(int Code, params Enum[] Relations)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""ItmsGrpCod"" AS ""Id"", ""ItmsGrpNam"" AS ""Name"" ");
            sb.AppendLine($@"FROM    {DBSA}.OITB ");
            sb.AppendLine($@"WHERE  ""ItmsGrpCod"" = {Code} ");

            BEA.Item item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Constructors

        public Category() : base() { }

        #endregion
    }
}
