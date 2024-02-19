using BEntities.Filters;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using BE = BEntities;
using BED = BEntities.AppData;
using BEB = BEntities.Base;
using BEK = BEntities.Kbytes;
using BEG = BEntities.Logs;
using BEM = BEntities.Marketing;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEX = BEntities.CIESD;
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;


namespace DALayer.Security
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Security
    /// Class     : User
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type User 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   15/2/2024 13:33:52 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class User : DALEntity<BES.User>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type User
        /// </summary>
        /// <param name="Item">Business object of type User </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.User Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[User]([FirstName], [LastName], [EMail], [Address], [Phone], [Login], [Password], [CardCode], [Position], [IdProfile], [Enabled], [Commentaries], [AccountHolder], [Picture], [AllowLinesBlocked], [RequiredLogOff], [LogUser], [LogDate]) VALUES(@FirstName, @LastName, @EMail, @Address, @Phone, @Login, @Password, @CardCode, @Position, @IdProfile, @Enabled, @Commentaries, @AccountHolder, @Picture, @AllowLinesBlocked, @RequiredLogOff, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[User] SET [FirstName] = @FirstName, [LastName] = @LastName, [EMail] = @EMail, [Address] = @Address, [Phone] = @Phone, [Login] = @Login, [Password] = @Password, [CardCode] = @CardCode, [Position] = @Position, [IdProfile] = @IdProfile, [Enabled] = @Enabled, [Commentaries] = @Commentaries, [AccountHolder] = @AccountHolder, [Picture] = @Picture, [AllowLinesBlocked] = @AllowLinesBlocked, [RequiredLogOff] = @RequiredLogOff, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[User] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert)
					Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, Item));
				else
					Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListUserTokens?.Count() > 0)
			{
				var list = Item.ListUserTokens;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new DALayer.AppData.UserToken(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserTokens = list;
			}
			if (Item.ListQuoteSents?.Count() > 0)
			{
				var list = Item.ListQuoteSents;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new DALayer.Online.QuoteSent(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListQuoteSents = list;
			}
			if (Item.ListSales?.Count() > 0)
			{
				var list = Item.ListSales;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new DALayer.Online.Sale(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSales = list;
			}
			if (Item.ListTempSales?.Count() > 0)
			{
				var list = Item.ListTempSales;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new DALayer.Online.TempSale(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTempSales = list;
			}
			if (Item.ListLoans?.Count() > 0)
			{
				var list = Item.ListLoans;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new DALayer.Product.Loan(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListLoans = list;
			}
			if (Item.ListRequests?.Count() > 0)
			{
				var list = Item.ListRequests;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new DALayer.Product.Request(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListRequests = list;
			}
			if (Item.ListQuotes?.Count() > 0)
			{
				var list = Item.ListQuotes;
				foreach (var item in list) item.IdSeller = itemId;
				using (var dal = new DALayer.Sales.Quote(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListQuotes = list;
			}
			if (Item.ListSellerss?.Count() > 0)
			{
				var list = Item.ListSellerss;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new Sellers(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSellerss = list;
			}
			if (Item.ListSessionHistorys?.Count() > 0)
			{
				var list = Item.ListSessionHistorys;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new SessionHistory(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSessionHistorys = list;
			}
			if (Item.ListUserActivitys?.Count() > 0)
			{
				var list = Item.ListUserActivitys;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new UserActivity(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserActivitys = list;
			}
			if (Item.ListUserClients?.Count() > 0)
			{
				var list = Item.ListUserClients;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new UserClient(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserClients = list;
			}
			if (Item.ListUserDatas?.Count() > 0)
			{
				var list = Item.ListUserDatas;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new UserData(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserDatas = list;
			}
			if (Item.ListUserPersons?.Count() > 0)
			{
				var list = Item.ListUserPersons;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new UserPerson(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserPersons = list;
			}
			if (Item.ListUserPivotConfigs?.Count() > 0)
			{
				var list = Item.ListUserPivotConfigs;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new UserPivotConfig(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserPivotConfigs = list;
			}
			if (Item.ListUserProfiles?.Count() > 0)
			{
				var list = Item.ListUserProfiles;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new UserProfile(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserProfiles = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  User		
        /// </summary>
        /// <param name="Items">Business object of type User para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.User> Items)
        {

            for (int i = 0; i < Items.Count; i++)
            {
                var Item = Items[i];
                
                Save(ref Item);
                Items[i] = Item;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	For use on data access layer at assembly level, return an  User type object
        /// </summary>
        /// <param name="Id">Object Identifier User</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type User</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.User ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto User de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a User</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo User</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.User> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.User> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[User] WHERE Id IN ( {string.Join(",", Keys)} ) ";
                Items = SQLList(strQuery, Relations);
            }
            return Items;
        }

        /// <summary>
        /// Carga las Relations de la Collection dada
        /// </summary>
        /// <param name="Items">Lista de objetos para cargar las Relations</param>
        /// <param name="Relations">Enumerador de Relations a cargar</param>
        /// <remarks></remarks>
        protected override void LoadRelations(ref IEnumerable<BES.User> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.AppData.UserToken> lstUserTokens = null; 
			IEnumerable<BE.Online.QuoteSent> lstQuoteSents = null; 
			IEnumerable<BE.Online.Sale> lstSales = null; 
			IEnumerable<BE.Online.TempSale> lstTempSales = null; 
			IEnumerable<BE.Product.Loan> lstLoans = null; 
			IEnumerable<BE.Product.Request> lstRequests = null; 
			IEnumerable<BE.Sales.Quote> lstQuotes = null; 
			IEnumerable<BE.Security.Sellers> lstSellerss = null; 
			IEnumerable<BE.Security.SessionHistory> lstSessionHistorys = null; 
			IEnumerable<BE.Security.UserActivity> lstUserActivitys = null; 
			IEnumerable<BE.Security.UserClient> lstUserClients = null; 
			IEnumerable<BE.Security.UserData> lstUserDatas = null; 
			IEnumerable<BE.Security.UserPerson> lstUserPersons = null; 
			IEnumerable<BE.Security.UserPivotConfig> lstUserPivotConfigs = null; 
			IEnumerable<BE.Security.UserProfile> lstUserProfiles = null; 
			IEnumerable<BE.Security.Profile> lstProfiles = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Security.relUser.UserTokens))
				{
					using (var dal = new DALayer.AppData.UserToken(Connection))
					{
						lstUserTokens = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.QuoteSents))
				{
					using (var dal = new DALayer.Online.QuoteSent(Connection))
					{
						lstQuoteSents = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Sales))
				{
					using (var dal = new DALayer.Online.Sale(Connection))
					{
						lstSales = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.TempSales))
				{
					using (var dal = new DALayer.Online.TempSale(Connection))
					{
						lstTempSales = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Loans))
				{
					using (var dal = new DALayer.Product.Loan(Connection))
					{
						lstLoans = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Requests))
				{
					using (var dal = new DALayer.Product.Request(Connection))
					{
						lstRequests = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Quotes))
				{
					using (var dal = new DALayer.Sales.Quote(Connection))
					{
						lstQuotes = dal.List(Keys, "IdSeller", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Sellerss))
				{
					using (var dal = new Sellers(Connection))
					{
						lstSellerss = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.SessionHistorys))
				{
					using (var dal = new SessionHistory(Connection))
					{
						lstSessionHistorys = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserActivitys))
				{
					using (var dal = new UserActivity(Connection))
					{
						lstUserActivitys = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserClients))
				{
					using (var dal = new UserClient(Connection))
					{
						lstUserClients = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserDatas))
				{
					using (var dal = new UserData(Connection))
					{
						lstUserDatas = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserPersons))
				{
					using (var dal = new UserPerson(Connection))
					{
						lstUserPersons = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserPivotConfigs))
				{
					using (var dal = new UserPivotConfig(Connection))
					{
						lstUserPivotConfigs = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserProfiles))
				{
					using (var dal = new UserProfile(Connection))
					{
						lstUserProfiles = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Profile))
				{
					using(var dal = new Profile(Connection))
					{
						Keys = (from i in Items select i.IdProfile).Distinct();
						lstProfiles = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstUserTokens != null)
					{
						Item.ListUserTokens = lstUserTokens.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstQuoteSents != null)
					{
						Item.ListQuoteSents = lstQuoteSents.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstSales != null)
					{
						Item.ListSales = lstSales.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstTempSales != null)
					{
						Item.ListTempSales = lstTempSales.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstLoans != null)
					{
						Item.ListLoans = lstLoans.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstRequests != null)
					{
						Item.ListRequests = lstRequests.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstQuotes != null)
					{
						Item.ListQuotes = lstQuotes.Where(x => x.IdSeller == Item.Id)?.ToList();
					}
					if (lstSellerss != null)
					{
						Item.ListSellerss = lstSellerss.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstSessionHistorys != null)
					{
						Item.ListSessionHistorys = lstSessionHistorys.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstUserActivitys != null)
					{
						Item.ListUserActivitys = lstUserActivitys.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstUserClients != null)
					{
						Item.ListUserClients = lstUserClients.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstUserDatas != null)
					{
						Item.ListUserDatas = lstUserDatas.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstUserPersons != null)
					{
						Item.ListUserPersons = lstUserPersons.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstUserPivotConfigs != null)
					{
						Item.ListUserPivotConfigs = lstUserPivotConfigs.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstUserProfiles != null)
					{
						Item.ListUserProfiles = lstUserProfiles.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstProfiles != null)
					{
						Item.Profile = (from i in lstProfiles where i.Id == Item.IdProfile select i).FirstOrDefault();
					}
                }
            }
        }

        /// <summary>
        /// Load Relationship of an Object
        /// </summary>
        /// <param name="Item">Given Object</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <remarks></remarks>
        protected override void LoadRelations(ref BES.User Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Security.relUser.UserTokens))
				{
					using (var dal = new DALayer.AppData.UserToken(Connection))
					{
						Item.ListUserTokens = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.QuoteSents))
				{
					using (var dal = new DALayer.Online.QuoteSent(Connection))
					{
						Item.ListQuoteSents = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Sales))
				{
					using (var dal = new DALayer.Online.Sale(Connection))
					{
						Item.ListSales = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.TempSales))
				{
					using (var dal = new DALayer.Online.TempSale(Connection))
					{
						Item.ListTempSales = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Loans))
				{
					using (var dal = new DALayer.Product.Loan(Connection))
					{
						Item.ListLoans = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Requests))
				{
					using (var dal = new DALayer.Product.Request(Connection))
					{
						Item.ListRequests = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Quotes))
				{
					using (var dal = new DALayer.Sales.Quote(Connection))
					{
						Item.ListQuotes = dal.List(Keys, "IdSeller", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Sellerss))
				{
					using (var dal = new Sellers(Connection))
					{
						Item.ListSellerss = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.SessionHistorys))
				{
					using (var dal = new SessionHistory(Connection))
					{
						Item.ListSessionHistorys = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserActivitys))
				{
					using (var dal = new UserActivity(Connection))
					{
						Item.ListUserActivitys = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserClients))
				{
					using (var dal = new UserClient(Connection))
					{
						Item.ListUserClients = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserDatas))
				{
					using (var dal = new UserData(Connection))
					{
						Item.ListUserDatas = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserPersons))
				{
					using (var dal = new UserPerson(Connection))
					{
						Item.ListUserPersons = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserPivotConfigs))
				{
					using (var dal = new UserPivotConfig(Connection))
					{
						Item.ListUserPivotConfigs = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.UserProfiles))
				{
					using (var dal = new UserProfile(Connection))
					{
						Item.ListUserProfiles = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUser.Profile))
				{
					using (var dal = new Profile(Connection))
					{
						Item.Profile = dal.ReturnMaster(Item.IdProfile, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of User
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type User</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.User> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[User] ORDER By " + Order;
            IEnumerable<BES.User> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of User
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.User> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[User] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.User> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of User
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type User</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.User> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[User] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.User> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type User    	
        /// </summary>
        /// <param name="Id">Object identifier User</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type User</returns>
        /// <remarks>
        /// </remarks>    
        public BES.User Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[User] WHERE [Id] = @Id";
            BES.User Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public User() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public User(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal User(SqlConnection connection) : base(connection) { }

        #endregion

    }
}