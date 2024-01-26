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
    /// Class     : UserProfile
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type UserProfile 
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
    public partial class UserProfile : DALEntity<BES.UserProfile>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type UserProfile
        /// </summary>
        /// <param name="Item">Business object of type UserProfile </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.UserProfile Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[UserProfile]([IdUser], [IdProfile], [LogUser], [LogDate]) VALUES(@IdUser, @IdProfile, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[UserProfile] SET [IdUser] = @IdUser, [IdProfile] = @IdProfile, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[UserProfile] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  UserProfile		
        /// </summary>
        /// <param name="Items">Business object of type UserProfile para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.UserProfile> Items)
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
        /// 	For use on data access layer at assembly level, return an  UserProfile type object
        /// </summary>
        /// <param name="Id">Object Identifier UserProfile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserProfile</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.UserProfile ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto UserProfile de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a UserProfile</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo UserProfile</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.UserProfile> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.UserProfile> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[UserProfile] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.UserProfile> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Security.Profile> lstProfiles = null;
			IEnumerable<BE.Security.User> lstUsers = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserProfile.Profile))
				{
					using(var dal = new Profile(Connection))
					{
						Keys = (from i in Items select i.IdProfile).Distinct();
						lstProfiles = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Security.relUserProfile.User))
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
					if (lstProfiles != null)
					{
						Item.Profile = (from i in lstProfiles where i.Id == Item.IdProfile select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BES.UserProfile Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserProfile.Profile))
				{
					using (var dal = new Profile(Connection))
					{
						Item.Profile = dal.ReturnMaster(Item.IdProfile, Relations);
					}
				}				if (RelationEnum.Equals(BE.Security.relUserProfile.User))
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
        /// 	Return an object Collection of UserProfile
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserProfile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserProfile> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[UserProfile] ORDER By " + Order;
            IEnumerable<BES.UserProfile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of UserProfile
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserProfile> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[UserProfile] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.UserProfile> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of UserProfile
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserProfile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserProfile> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserProfile] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.UserProfile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type UserProfile    	
        /// </summary>
        /// <param name="Id">Object identifier UserProfile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserProfile</returns>
        /// <remarks>
        /// </remarks>    
        public BES.UserProfile Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserProfile] WHERE [Id] = @Id";
            BES.UserProfile Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public UserProfile() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public UserProfile(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal UserProfile(SqlConnection connection) : base(connection) { }

        #endregion

    }
}