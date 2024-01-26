using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEA = BEntities.SAP;

namespace DALayer.SAP.Hana
{
    [Serializable()]
    public class Seller : DALEntity<BEA.Seller>
    {

        #region Methods

        internal BEA.Seller ReturnMaster(string SellerCode, params Enum[] Relations)
        {
            return Search(SellerCode, Relations);
        }

        protected override void LoadRelations(ref IEnumerable<BEA.Seller> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.Seller item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public IEnumerable<BEA.Seller> List(List<Field> Filters, string Order, params Enum[] Relations)
        {
            string strFilter = "";
            if (Filters?.Count > 0) strFilter = GetFilter(Filters?.ToArray());

            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery("")}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE    {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Seller> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Seller> ListWithClient(List<Field> Filters, string Order, params Enum[] Relations)
        {
            string strFilter = "";
            if (Filters?.Count > 0) strFilter = GetFilter(Filters?.ToArray());

            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery("C")}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE    {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Seller> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        public IEnumerable<BEA.Seller> ProductManagers(List<Field> Filters, string Order, params Enum[] Relations)
        {
            string strFilter = "";
            if (Filters?.Count > 0) strFilter = GetFilter(Filters?.ToArray());

            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery("PM")}) a ");
            if (strFilter != "") sb.AppendLine($@"WHERE    {strFilter} ");
            sb.AppendLine($@"ORDER By {GetOrder(Order)} ");

            IEnumerable<BEA.Seller> items = SQLList(sb.ToString(), Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public BEA.Seller Search(string SellerCode, params Enum[] Relations)
        {
            StringBuilder sb = new();
            sb.AppendLine($@"SELECT   * ");
            sb.AppendLine($@"FROM     ({GetQuery("")}) a ");
            sb.AppendLine($@"WHERE    ""ShortName"" = '{SellerCode}' ");

            BEA.Seller item = SQLSearch(sb.ToString(), Relations);
            return item;
        }

        #endregion

        #region Private Methods

        private string GetQuery(string SellerType)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"SELECT  ""SlpName"" AS ""ShortName"", ""Memo"" AS ""Name"", ""Active"" ");
            sb.AppendLine($@"FROM    {DBSA}.OSLP o ");
            sb.AppendLine($@"WHERE   ""Memo"" IS NOT NULL ");
            if (SellerType == "C") sb.AppendLine($@"        AND EXISTS ( SELECT * FROM {DBSA}.OCRD WHERE ""SlpCode"" = o.""SlpCode"" ) ");
            if (SellerType == "PM") sb.AppendLine($@"        AND ""SlpName"" IN ( SELECT DISTINCT U_CODGRT FROM {DBSA}.OITM WHERE U_CODGRT IS NOT NULL ) ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  ""SlpName"", ""Memo"", ""Active"" ");
            sb.AppendLine($@"FROM    {DBIQ}.OSLP o ");
            sb.AppendLine($@"WHERE   ""Memo"" IS NOT NULL ");
            if (SellerType == "C") sb.AppendLine($@"        AND EXISTS ( SELECT * FROM {DBIQ}.OCRD WHERE ""SlpCode"" = o.""SlpCode"" ) ");
            if (SellerType == "PM") sb.AppendLine($@"        AND ""SlpName"" IN ( SELECT DISTINCT U_CODGRT FROM {DBIQ}.OITM WHERE U_CODGRT IS NOT NULL ) ");
            sb.AppendLine($@"UNION ");
            sb.AppendLine($@"SELECT  ""SlpName"", ""Memo"", ""Active"" ");
            sb.AppendLine($@"FROM    {DBLA}.OSLP o ");
            sb.AppendLine($@"WHERE   ""Memo"" IS NOT NULL ");
            if (SellerType == "C") sb.AppendLine($@"        AND EXISTS ( SELECT * FROM {DBLA}.OCRD WHERE ""SlpCode"" = o.""SlpCode"" ) ");
            if (SellerType == "PM") sb.AppendLine($@"        AND ""SlpName"" IN ( SELECT DISTINCT U_CODGRT FROM {DBLA}.OITM WHERE U_CODGRT IS NOT NULL ) ");
            return sb.ToString();
        }

        #endregion

        #region Constructors

        public Seller() : base() { }

        #endregion
    }
}
