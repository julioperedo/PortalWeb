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


namespace DALayer.Base
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Base
    /// Class     : Activity
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Activity 
    ///     for the service Base.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Base
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Activity : DALEntity<BEB.Activity>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Activity
        /// </summary>
        /// <param name="Item">Business object of type Activity </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.Activity Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[Activity]([Id], [Name], [Description], [IdSModule], [LogUser], [LogDate]) VALUES(@Id, @Name, @Description, @IdSModule, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[Activity] SET [Name] = @Name, [Description] = @Description, [IdSModule] = @IdSModule, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[Activity] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Activity", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListProfileActivitys?.Count() > 0)
			{
				var list = Item.ListProfileActivitys;
				foreach (var item in list) item.IdActivity = itemId;
				using (var dal = new DALayer.Security.ProfileActivity(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListProfileActivitys = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Activity		
        /// </summary>
        /// <param name="Items">Business object of type Activity para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.Activity> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Activity", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Activity type object
        /// </summary>
        /// <param name="Id">Object Identifier Activity</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Activity</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.Activity ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Activity de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Activity</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Activity</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.Activity> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.Activity> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[Activity] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEB.Activity> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Security.ProfileActivity> lstProfileActivitys = null; 
			IEnumerable<BE.Base.SModule> lstSModules = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Base.relActivity.ProfileActivitys))
				{
					using (var dal = new DALayer.Security.ProfileActivity(Connection))
					{
						lstProfileActivitys = dal.List(Keys, "IdActivity", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relActivity.SModule))
				{
					using(var dal = new SModule(Connection))
					{
						Keys = (from i in Items select i.IdSModule).Distinct();
						lstSModules = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstProfileActivitys != null)
					{
						Item.ListProfileActivitys = lstProfileActivitys.Where(x => x.IdActivity == Item.Id)?.ToList();
					}
					if (lstSModules != null)
					{
						Item.SModule = (from i in lstSModules where i.Id == Item.IdSModule select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEB.Activity Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Base.relActivity.ProfileActivitys))
				{
					using (var dal = new DALayer.Security.ProfileActivity(Connection))
					{
						Item.ListProfileActivitys = dal.List(Keys, "IdActivity", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relActivity.SModule))
				{
					using (var dal = new SModule(Connection))
					{
						Item.SModule = dal.ReturnMaster(Item.IdSModule, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Activity
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Activity</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Activity> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[Activity] ORDER By " + Order;
            IEnumerable<BEB.Activity> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Activity
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Activity> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[Activity] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.Activity> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Activity
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Activity</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Activity> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Activity] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.Activity> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Activity    	
        /// </summary>
        /// <param name="Id">Object identifier Activity</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Activity</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.Activity Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Activity] WHERE [Id] = @Id";
            BEB.Activity Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Activity() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Activity(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Activity(SqlConnection connection) : base(connection) { }

        #endregion

    }
}