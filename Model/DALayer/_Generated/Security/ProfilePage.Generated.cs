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
    /// Class     : ProfilePage
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ProfilePage 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:49 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ProfilePage : DALEntity<BES.ProfilePage>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ProfilePage
        /// </summary>
        /// <param name="Item">Business object of type ProfilePage </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.ProfilePage Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[ProfilePage]([IdProfile], [IdPage], [LogUser], [LogDate]) VALUES(@IdProfile, @IdPage, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[ProfilePage] SET [IdProfile] = @IdProfile, [IdPage] = @IdPage, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[ProfilePage] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ProfilePage		
        /// </summary>
        /// <param name="Items">Business object of type ProfilePage para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.ProfilePage> Items)
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
        /// 	For use on data access layer at assembly level, return an  ProfilePage type object
        /// </summary>
        /// <param name="Id">Object Identifier ProfilePage</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ProfilePage</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.ProfilePage ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ProfilePage de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ProfilePage</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ProfilePage</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.ProfilePage> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.ProfilePage> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[ProfilePage] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.ProfilePage> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.Page> lstPages = null;
			IEnumerable<BE.Security.Profile> lstProfiles = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relProfilePage.Page))
				{
					using(var dal = new Base.Page(Connection))
					{
						Keys = (from i in Items select i.IdPage).Distinct();
						lstPages = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Security.relProfilePage.Profile))
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
					if (lstPages != null)
					{
						Item.Page = (from i in lstPages where i.Id == Item.IdPage select i).FirstOrDefault();
					}					if (lstProfiles != null)
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
        protected override void LoadRelations(ref BES.ProfilePage Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relProfilePage.Page))
				{
					using (var dal = new Base.Page(Connection))
					{
						Item.Page = dal.ReturnMaster(Item.IdPage, Relations);
					}
				}				if (RelationEnum.Equals(BE.Security.relProfilePage.Profile))
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
        /// 	Return an object Collection of ProfilePage
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ProfilePage</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfilePage> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[ProfilePage] ORDER By " + Order;
            IEnumerable<BES.ProfilePage> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ProfilePage
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfilePage> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[ProfilePage] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.ProfilePage> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ProfilePage
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ProfilePage</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfilePage> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[ProfilePage] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.ProfilePage> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ProfilePage    	
        /// </summary>
        /// <param name="Id">Object identifier ProfilePage</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ProfilePage</returns>
        /// <remarks>
        /// </remarks>    
        public BES.ProfilePage Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[ProfilePage] WHERE [Id] = @Id";
            BES.ProfilePage Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ProfilePage() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ProfilePage(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ProfilePage(SqlConnection connection) : base(connection) { }

        #endregion

    }
}