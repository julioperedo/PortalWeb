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
    public class User : DALEntity<BEA.User>
    {

        #region Methods

        //internal BEA.Seller ReturnMaster(string SellerCode, params Enum[] Relations)
        //{
        //    return Search(SellerCode, Relations);
        //}

        protected override void LoadRelations(ref IEnumerable<BEA.User> Items, params Enum[] Relations) { }

        protected override void LoadRelations(ref BEA.User item, params Enum[] Relations) { }

        #endregion

        #region List Methods

        public async Task<IEnumerable<BEA.User>> ListAsync(List<Field> Filters, string SortingBy, params Enum[] Relations)
        {
            string filter = Filters?.Count > 0 ? $"AND {GetFilter(Filters?.ToArray())}" : "", query;
            query = $@"SELECT 	USERID AS ""Id"", USER_CODE AS ""Code"", U_NAME AS ""Name"", ""E_Mail"" AS ""Email""  
FROM 	{DBSA}.OUSR o  
WHERE   ""Locked"" = 'N' {filter}
ORDER BY {GetOrder(SortingBy)} ";

            IEnumerable<BEA.User> items =await SQLListAsync(query, Relations);
            return items;
        }

        #endregion

        #region Search Methods

        //public BEA.Seller Search(string SellerCode, params Enum[] Relations)
        //{
        //    StringBuilder sb = new();
        //    sb.AppendLine($@"SELECT   * ");
        //    sb.AppendLine($@"FROM     ({GetQuery("")}) a ");
        //    sb.AppendLine($@"WHERE    ""ShortName"" = '{SellerCode}' ");

        //    BEA.Seller item = SQLSearch(sb.ToString(), Relations);
        //    return item;
        //}

        #endregion

        #region Private Methods

        #endregion

        #region Constructors

        public User() : base() { }

        #endregion
    }
}
