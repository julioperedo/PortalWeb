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


namespace DALayer.Security
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Security
    /// Class     : UserPerson
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type UserPerson 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:50 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class UserPerson : DALEntity<BES.UserPerson>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type UserPerson
        /// </summary>
        /// <param name="Item">Business object of type UserPerson </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.UserPerson Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[UserPerson]([IdPerson], [IdUser], [LogUser], [LogDate]) VALUES(@IdPerson, @IdUser, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[UserPerson] SET [IdPerson] = @IdPerson, [IdUser] = @IdUser, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[UserPerson] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert)
					Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, Item));
				else
					Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  UserPerson		
        /// </summary>
        /// <param name="Items">Business object of type UserPerson para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.UserPerson> Items)
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
        /// 	For use on data access layer at assembly level, return an  UserPerson type object
        /// </summary>
        /// <param name="Id">Object Identifier UserPerson</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserPerson</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.UserPerson ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto UserPerson de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a UserPerson</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo UserPerson</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.UserPerson> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.UserPerson> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[UserPerson] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.UserPerson> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Visits.Person> lstPersons = null;
			IEnumerable<BE.Security.User> lstUsers = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserPerson.Person))
				{
					using(var dal = new Visits.Person(Connection))
					{
						Keys = (from i in Items select i.IdPerson).Distinct();
						lstPersons = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Security.relUserPerson.User))
				{
					using(var dal = new User(Connection))
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
					if (lstPersons != null)
					{
						Item.Person = (from i in lstPersons where i.Id == Item.IdPerson select i).FirstOrDefault();
					}					if (lstUsers != null)
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
        protected override void LoadRelations(ref BES.UserPerson Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserPerson.Person))
				{
					using (var dal = new Visits.Person(Connection))
					{
						Item.Person = dal.ReturnMaster(Item.IdPerson, Relations);
					}
				}				if (RelationEnum.Equals(BE.Security.relUserPerson.User))
				{
					using (var dal = new User(Connection))
					{
						Item.User = dal.ReturnMaster(Item.IdUser, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of UserPerson
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserPerson</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserPerson> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[UserPerson] ORDER By " + Order;
            IEnumerable<BES.UserPerson> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of UserPerson
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserPerson> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[UserPerson] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.UserPerson> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of UserPerson
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserPerson</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserPerson> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserPerson] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.UserPerson> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type UserPerson    	
        /// </summary>
        /// <param name="Id">Object identifier UserPerson</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserPerson</returns>
        /// <remarks>
        /// </remarks>    
        public BES.UserPerson Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserPerson] WHERE [Id] = @Id";
            BES.UserPerson Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public UserPerson() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public UserPerson(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal UserPerson(SqlConnection connection) : base(connection) { }

        #endregion

    }
}