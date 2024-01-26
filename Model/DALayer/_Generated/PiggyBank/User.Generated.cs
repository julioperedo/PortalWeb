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


namespace DALayer.PiggyBank
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : PiggyBank
    /// Class     : User
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type User 
    ///     for the service PiggyBank.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PiggyBank
    /// </remarks>
    /// <history>
    ///     [DMC]   23/10/2023 09:55:27 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class User : DALEntity<BEI.User>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type User
        /// </summary>
        /// <param name="Item">Business object of type User </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEI.User Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PiggyBank].[User]([Name], [StoreName], [EMail], [Password], [Enabled], [City], [Address], [Phone], [LogUser], [LogDate]) VALUES(@Name, @StoreName, @EMail, @Password, @Enabled, @City, @Address, @Phone, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PiggyBank].[User] SET [Name] = @Name, [StoreName] = @StoreName, [EMail] = @EMail, [Password] = @Password, [Enabled] = @Enabled, [City] = @City, [Address] = @Address, [Phone] = @Phone, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PiggyBank].[User] WHERE [Id] = @Id";
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
			if (Item.ListClaimedPrizes?.Count() > 0)
			{
				var list = Item.ListClaimedPrizes;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new ClaimedPrize(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListClaimedPrizes = list;
			}
			if (Item.ListUserTokens?.Count() > 0)
			{
				var list = Item.ListUserTokens;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new UserToken(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserTokens = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  User		
        /// </summary>
        /// <param name="Items">Business object of type User para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEI.User> Items)
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
        internal BEI.User ReturnMaster(long Id, params Enum[] Relations)
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
        internal IEnumerable<BEI.User> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEI.User> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PiggyBank].[User] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEI.User> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.PiggyBank.ClaimedPrize> lstClaimedPrizes = null; 
			IEnumerable<BE.PiggyBank.UserToken> lstUserTokens = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.PiggyBank.relUser.ClaimedPrizes))
				{
					using (var dal = new ClaimedPrize(Connection))
					{
						lstClaimedPrizes = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.PiggyBank.relUser.UserTokens))
				{
					using (var dal = new UserToken(Connection))
					{
						lstUserTokens = dal.List(Keys, "IdUser", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstClaimedPrizes != null)
					{
						Item.ListClaimedPrizes = lstClaimedPrizes.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstUserTokens != null)
					{
						Item.ListUserTokens = lstUserTokens.Where(x => x.IdUser == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEI.User Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.PiggyBank.relUser.ClaimedPrizes))
				{
					using (var dal = new ClaimedPrize(Connection))
					{
						Item.ListClaimedPrizes = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.PiggyBank.relUser.UserTokens))
				{
					using (var dal = new UserToken(Connection))
					{
						Item.ListUserTokens = dal.List(Keys, "IdUser", Relations)?.ToList();
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
        public IEnumerable<BEI.User> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PiggyBank].[User] ORDER By " + Order;
            IEnumerable<BEI.User> Items = SQLList(strQuery, Relations);
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
        public IEnumerable<BEI.User> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PiggyBank].[User] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEI.User> Items = SQLList(sbQuery.ToString(), Relations);
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
        public IEnumerable<BEI.User> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[User] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEI.User> Items = SQLList(strQuery, Relations);
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
        public BEI.User Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[User] WHERE [Id] = @Id";
            BEI.User Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
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