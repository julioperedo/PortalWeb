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
    /// Class     : Profile
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Profile 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:48 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Profile : DALEntity<BES.Profile>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Profile
        /// </summary>
        /// <param name="Item">Business object of type Profile </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.Profile Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[Profile]([Name], [Description], [isBase], [isExternalCapable], [LogUser], [LogDate]) VALUES(@Name, @Description, @isBase, @isExternalCapable, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[Profile] SET [Name] = @Name, [Description] = @Description, [isBase] = @isBase, [isExternalCapable] = @isExternalCapable, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[Profile] WHERE [Id] = @Id";
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
			if (Item.ListProfileActivitys?.Count() > 0)
			{
				var list = Item.ListProfileActivitys;
				foreach (var item in list) item.IdProfile = itemId;
				using (var dal = new ProfileActivity(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListProfileActivitys = list;
			}
			if (Item.ListProfileCharts?.Count() > 0)
			{
				var list = Item.ListProfileCharts;
				foreach (var item in list) item.IdProfile = itemId;
				using (var dal = new ProfileChart(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListProfileCharts = list;
			}
			if (Item.ListProfilePages?.Count() > 0)
			{
				var list = Item.ListProfilePages;
				foreach (var item in list) item.IdProfile = itemId;
				using (var dal = new ProfilePage(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListProfilePages = list;
			}
			if (Item.ListUsers?.Count() > 0)
			{
				var list = Item.ListUsers;
				foreach (var item in list) item.IdProfile = itemId;
				using (var dal = new User(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUsers = list;
			}
			if (Item.ListUserProfiles?.Count() > 0)
			{
				var list = Item.ListUserProfiles;
				foreach (var item in list) item.IdProfile = itemId;
				using (var dal = new UserProfile(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserProfiles = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Profile		
        /// </summary>
        /// <param name="Items">Business object of type Profile para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.Profile> Items)
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
        /// 	For use on data access layer at assembly level, return an  Profile type object
        /// </summary>
        /// <param name="Id">Object Identifier Profile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Profile</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.Profile ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Profile de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Profile</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Profile</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.Profile> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.Profile> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[Profile] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.Profile> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Security.ProfileActivity> lstProfileActivitys = null; 
			IEnumerable<BE.Security.ProfileChart> lstProfileCharts = null; 
			IEnumerable<BE.Security.ProfilePage> lstProfilePages = null; 
			IEnumerable<BE.Security.User> lstUsers = null; 
			IEnumerable<BE.Security.UserProfile> lstUserProfiles = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Security.relProfile.ProfileActivitys))
				{
					using (var dal = new ProfileActivity(Connection))
					{
						lstProfileActivitys = dal.List(Keys, "IdProfile", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.ProfileCharts))
				{
					using (var dal = new ProfileChart(Connection))
					{
						lstProfileCharts = dal.List(Keys, "IdProfile", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.ProfilePages))
				{
					using (var dal = new ProfilePage(Connection))
					{
						lstProfilePages = dal.List(Keys, "IdProfile", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.Users))
				{
					using (var dal = new User(Connection))
					{
						lstUsers = dal.List(Keys, "IdProfile", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.UserProfiles))
				{
					using (var dal = new UserProfile(Connection))
					{
						lstUserProfiles = dal.List(Keys, "IdProfile", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstProfileActivitys != null)
					{
						Item.ListProfileActivitys = lstProfileActivitys.Where(x => x.IdProfile == Item.Id)?.ToList();
					}
					if (lstProfileCharts != null)
					{
						Item.ListProfileCharts = lstProfileCharts.Where(x => x.IdProfile == Item.Id)?.ToList();
					}
					if (lstProfilePages != null)
					{
						Item.ListProfilePages = lstProfilePages.Where(x => x.IdProfile == Item.Id)?.ToList();
					}
					if (lstUsers != null)
					{
						Item.ListUsers = lstUsers.Where(x => x.IdProfile == Item.Id)?.ToList();
					}
					if (lstUserProfiles != null)
					{
						Item.ListUserProfiles = lstUserProfiles.Where(x => x.IdProfile == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BES.Profile Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Security.relProfile.ProfileActivitys))
				{
					using (var dal = new ProfileActivity(Connection))
					{
						Item.ListProfileActivitys = dal.List(Keys, "IdProfile", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.ProfileCharts))
				{
					using (var dal = new ProfileChart(Connection))
					{
						Item.ListProfileCharts = dal.List(Keys, "IdProfile", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.ProfilePages))
				{
					using (var dal = new ProfilePage(Connection))
					{
						Item.ListProfilePages = dal.List(Keys, "IdProfile", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.Users))
				{
					using (var dal = new User(Connection))
					{
						Item.ListUsers = dal.List(Keys, "IdProfile", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relProfile.UserProfiles))
				{
					using (var dal = new UserProfile(Connection))
					{
						Item.ListUserProfiles = dal.List(Keys, "IdProfile", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Profile
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Profile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.Profile> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[Profile] ORDER By " + Order;
            IEnumerable<BES.Profile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Profile
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.Profile> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[Profile] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.Profile> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Profile
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Profile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.Profile> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[Profile] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.Profile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Profile    	
        /// </summary>
        /// <param name="Id">Object identifier Profile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Profile</returns>
        /// <remarks>
        /// </remarks>    
        public BES.Profile Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[Profile] WHERE [Id] = @Id";
            BES.Profile Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Profile() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Profile(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Profile(SqlConnection connection) : base(connection) { }

        #endregion

    }
}