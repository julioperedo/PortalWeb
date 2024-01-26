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
    /// Class     : ProfileChart
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ProfileChart 
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
    public partial class ProfileChart : DALEntity<BES.ProfileChart>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ProfileChart
        /// </summary>
        /// <param name="Item">Business object of type ProfileChart </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.ProfileChart Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[ProfileChart]([Id], [IdChart], [IdProfile], [LogUser], [LogDate]) VALUES(@Id, @IdChart, @IdProfile, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[ProfileChart] SET [IdChart] = @IdChart, [IdProfile] = @IdProfile, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[ProfileChart] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("ProfileChart", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  ProfileChart		
        /// </summary>
        /// <param name="Items">Business object of type ProfileChart para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.ProfileChart> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("ProfileChart", quantity);
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
        /// 	For use on data access layer at assembly level, return an  ProfileChart type object
        /// </summary>
        /// <param name="Id">Object Identifier ProfileChart</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ProfileChart</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.ProfileChart ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ProfileChart de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ProfileChart</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ProfileChart</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.ProfileChart> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.ProfileChart> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[ProfileChart] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.ProfileChart> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.Chart> lstCharts = null;
			IEnumerable<BE.Security.Profile> lstProfiles = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relProfileChart.Chart))
				{
					using(var dal = new Base.Chart(Connection))
					{
						Keys = (from i in Items select i.IdChart).Distinct();
						lstCharts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Security.relProfileChart.Profile))
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
					if (lstCharts != null)
					{
						Item.Chart = (from i in lstCharts where i.Id == Item.IdChart select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BES.ProfileChart Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relProfileChart.Chart))
				{
					using (var dal = new Base.Chart(Connection))
					{
						Item.Chart = dal.ReturnMaster(Item.IdChart, Relations);
					}
				}				if (RelationEnum.Equals(BE.Security.relProfileChart.Profile))
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
        /// 	Return an object Collection of ProfileChart
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ProfileChart</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfileChart> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[ProfileChart] ORDER By " + Order;
            IEnumerable<BES.ProfileChart> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ProfileChart
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfileChart> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[ProfileChart] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.ProfileChart> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ProfileChart
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ProfileChart</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.ProfileChart> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[ProfileChart] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.ProfileChart> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ProfileChart    	
        /// </summary>
        /// <param name="Id">Object identifier ProfileChart</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ProfileChart</returns>
        /// <remarks>
        /// </remarks>    
        public BES.ProfileChart Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[ProfileChart] WHERE [Id] = @Id";
            BES.ProfileChart Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ProfileChart() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ProfileChart(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ProfileChart(SqlConnection connection) : base(connection) { }

        #endregion

    }
}