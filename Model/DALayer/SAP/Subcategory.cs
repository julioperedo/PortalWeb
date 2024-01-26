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
    public class Subcategory : DALEntity<BEA.Item>
    {

        #region Methods

        protected override void LoadRelations(ref IEnumerable<BEA.Item> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.Item item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Item> List(int IdCategory, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            sbQuery.AppendLine($@"SELECT  DISTINCT U_SUBCATEG AS ""Name"" ");
            sbQuery.AppendLine($@"FROM    {DBSA}.OITM ");
            sbQuery.AppendLine($@"WHERE   U_SUBCATEG IS NOT NULL AND ""ItmsGrpCod"" = {IdCategory} ");
            sbQuery.AppendLine($@"ORDER By 1 ");

            IEnumerable<BEA.Item> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Item> List(string Category, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            sbQuery.AppendLine($@"SELECT  DISTINCT U_SUBCATEG AS ""Name"" ");
            sbQuery.AppendLine($@"FROM    {DBSA}.OITM ");
            sbQuery.AppendLine($@"WHERE   U_SUBCATEG IS NOT NULL AND LOWER(U_CATEGORIA) = '{Category.ToLower()}' ");
            sbQuery.AppendLine($@"ORDER By 1 ");

            IEnumerable<BEA.Item> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Item> ListIn(string Categories, params Enum[] Relations)
        {
            StringBuilder sbQuery = new();
            sbQuery.AppendLine($@"SELECT  DISTINCT U_SUBCATEG AS ""Name"" ");
            sbQuery.AppendLine($@"FROM    {DBSA}.OITM ");
            if (!string.IsNullOrWhiteSpace(Categories)) sbQuery.AppendLine($@"WHERE   U_SUBCATEG IS NOT NULL AND LOWER(U_CATEGORIA) IN ( {Categories.ToLower()} ) ");
            sbQuery.AppendLine($@"ORDER By 1 ");

            IEnumerable<BEA.Item> items = SQLList(sbQuery.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        #endregion

        #region Constructors

        public Subcategory() : base() { }

        #endregion
    }
}
