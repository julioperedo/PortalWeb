using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;
using BEI = BEntities.PiggyBank;

namespace DALayer.PiggyBank
{
    public partial class User
    {

        #region Save Methods

        //public async Task DeleteUserAsync(string eMail)
        //{
        //    string query = $"DELETE FROM PiggyBank.[User] WHERE EMail = '{eMail}'";
        //    await Connection.ExecuteAsync(query);
        //}

        public async Task Disable(long Id)
        {
            string query = $"UPDATE [PiggyBank].[User] SET [Enabled] = 0, [LogDate] = GETDATE() WHERE [Id] = {Id}";
            await Connection.ExecuteAsync(query);
        }

        #endregion

        #region Methods

        #endregion

        #region List Methods

        public IEnumerable<BEI.User> ListResume(string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  u.Id, u.Name, u.StoreName, ISNULL(SUM(s.Points), 0) AS Points, COUNT(s.Id) AS SerialsCount
FROM    PiggyBank.[User] u
        LEFT OUTER JOIN PiggyBank.Serial s ON u.Id = s.IdUser AND s.State = 'V'
WHERE   u.Enabled = 1
GROUP BY u.Id, u.Name, u.StoreName 
ORDER BY {SortingBy} ";
            IEnumerable<BEI.User> Items = SQLList(query, Relations);
            return Items;
        }

        //        public IEnumerable<BEI.User> ListExtended(string SortingBy, params Enum[] Relations)
        //        {
        //            string query = $@"SELECT  s.*, u.Name AS UserName, u.StoreName, u.City
        //FROM    PiggyBank.Serial s
        //        INNER JOIN PiggyBank.[User] u ON s.IdUser = u.Id
        //WHERE   s.State = 'V'
        //ORDER BY {SortingBy} ";
        //            IEnumerable<BEI.User> Items = SQLList(query, Relations);
        //            return Items;
        //        }

        public async Task<IEnumerable<BEI.User>> ListWithTokensAsync(string Filter, string SortingBy, params Enum[] Relations)
        {
            string query = $@"SELECT  *
FROM    PiggyBank.[User] u
WHERE   EXISTS ( SELECT * FROM PiggyBank.UserToken ut WHERE ut.IdUser = u.Id )
        AND ( u.Name LIKE '%{Filter}%' OR u.StoreName LIKE '%{Filter}%' )
ORDER BY {SortingBy} ";

            IEnumerable<BEI.User> items = await SQLListAsync(query, Relations);
            return items;
        }

        #endregion

        #region Search Methods

        public async Task<BEI.User> SearchAsync(string EMail, string Password, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM [PiggyBank].[User] WHERE EMail = '{EMail}' AND Password = '{Password}' ";
            BEI.User Item = await SQLSearchAsync(strQuery, Relations);
            return Item;
        }

        public async Task<BEI.User> SearchAsync(string EMail, params Enum[] Relations)
        {
            string strQuery = $@"SELECT * FROM [PiggyBank].[User] WHERE EMail = '{EMail}' ";
            BEI.User Item = await SQLSearchAsync(strQuery, Relations);
            return Item;
        }

        #endregion

    }
}