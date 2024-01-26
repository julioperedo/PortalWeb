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
using BET = BEntities.PostSale;
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEC = BEntities.CIESD;


namespace DALayer.AppData
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : AppData
    /// Class     : UserToken
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type UserToken 
    ///     for the service AppData.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service AppData
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class UserToken : DALEntity<BED.UserToken>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type UserToken
        /// </summary>
        /// <param name="Item">Business object of type UserToken </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BED.UserToken Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [AppData].[UserToken]([Id], [IdUser], [Token], [LogUser], [LogDate]) VALUES(@Id, @IdUser, @Token, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [AppData].[UserToken] SET [IdUser] = @IdUser, [Token] = @Token, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [AppData].[UserToken] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("UserToken", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  UserToken		
        /// </summary>
        /// <param name="Items">Business object of type UserToken para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BED.UserToken> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("UserToken", quantity);
				currentId = lastId - quantity + 1;
				if (lastId <= 0) throw new Exception("No se puede generar el identificador " + this.GetType().FullName);
			}

            for (int i = 0; i < Items.Count; i++)
            {
                var Item = Items[i];
                if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = currentId++;
                Save(ref Item);
                Items[i] = Item;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	For use on data access layer at assembly level, return an  UserToken type object
        /// </summary>
        /// <param name="Id">Object Identifier UserToken</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserToken</returns>
        /// <remarks>
        /// </remarks>		
        internal BED.UserToken ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto UserToken de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a UserToken</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo UserToken</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BED.UserToken> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BED.UserToken> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [AppData].[UserToken] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BED.UserToken> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Security.User> lstUsers = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.AppData.relUserToken.User))
				{
					using(var dal = new Security.User(Connection))
					{
						Keys = (from i in Items select i.IdUser).Distinct();
						lstUsers = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstUsers != null)
					{
						Item.User = (from i in lstUsers where i.Id == Item.IdUser select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BED.UserToken Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.AppData.relUserToken.User))
				{
					using (var dal = new Security.User(Connection))
					{
						Item.User = dal.ReturnMaster(Item.IdUser, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of UserToken
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserToken</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.UserToken> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [AppData].[UserToken] ORDER By " + Order;
            IEnumerable<BED.UserToken> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of UserToken
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.UserToken> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [AppData].[UserToken] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BED.UserToken> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of UserToken
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserToken</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.UserToken> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[UserToken] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BED.UserToken> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type UserToken    	
        /// </summary>
        /// <param name="Id">Object identifier UserToken</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserToken</returns>
        /// <remarks>
        /// </remarks>    
        public BED.UserToken Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[UserToken] WHERE [Id] = @Id";
            BED.UserToken Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public UserToken() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public UserToken(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal UserToken(SqlConnection connection) : base(connection) { }

        #endregion

    }
}