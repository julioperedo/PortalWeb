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
    /// Class     : ProfileActivity
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ProfileActivity 
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
    public partial class ProfileActivity : DALEntity<BES.ProfileActivity>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ProfileActivity
        /// </summary>
        /// <param name="Item">Business object of type ProfileActivity </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.ProfileActivity Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[ProfileActivity]([IdProfile], [IdActivity], [Permission], [LogUser], [LogDate]) VALUES(@IdProfile, @IdActivity, @Permission, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[ProfileActivity] SET [IdProfile] = @IdProfile, [IdActivity] = @IdActivity, [Permission] = @Permission, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[ProfileActivity] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ProfileActivity		
        /// </summary>
        /// <param name="Items">Business object of type ProfileActivity para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.ProfileActivity> Items)
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
        /// 	For use on data access layer at assembly level, return an  ProfileActivity type object
        /// </summary>
        /// <param name="Id">Object Identifier ProfileActivity</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ProfileActivity</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.ProfileActivity ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ProfileActivity de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ProfileActivity</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ProfileActivity</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.ProfileActivity> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.ProfileActivity> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[ProfileActivity] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.ProfileActivity> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.Activity> lstActivitys = null;
			IEnumerable<BE.Security.Profile> lstProfiles = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relProfileActivity.Activity))
				{
					using(var dal = new Base.Activity(Connection))
					{
						Keys = (from i in Items select i.IdActivity).Distinct();
						lstActivitys = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Security.relProfileActivity.Profile))
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
					if (lstActivitys != null)
					{
						Item.Activity = (from i in lstActivitys where i.Id == Item.IdActivity select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BES.ProfileActivity Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relProfileActivity.Activity))
				{
					using (var dal = new Base.Activity(Connection))
					{
						Item.Activity = dal.ReturnMaster(Item.IdActivity, Relations);
					}
				}				if (RelationEnum.Equals(BE.Security.relProfileActivity.Profile))
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
        /// 	Return an object Collection of ProfileActivity
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ProfileActivity</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfileActivity> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[ProfileActivity] ORDER By " + Order;
            IEnumerable<BES.ProfileActivity> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ProfileActivity
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfileActivity> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[ProfileActivity] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.ProfileActivity> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ProfileActivity
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ProfileActivity</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfileActivity> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[ProfileActivity] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.ProfileActivity> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ProfileActivity    	
        /// </summary>
        /// <param name="Id">Object identifier ProfileActivity</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ProfileActivity</returns>
        /// <remarks>
        /// </remarks>    
        public BES.ProfileActivity Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[ProfileActivity] WHERE [Id] = @Id";
            BES.ProfileActivity Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ProfileActivity() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ProfileActivity(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ProfileActivity(SqlConnection connection) : base(connection) { }

        #endregion

    }
}