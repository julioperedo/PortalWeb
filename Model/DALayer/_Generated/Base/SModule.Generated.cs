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
    /// Class     : SModule
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type SModule 
    ///     for the service Base.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Base
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:36 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class SModule : DALEntity<BEB.SModule>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type SModule
        /// </summary>
        /// <param name="Item">Business object of type SModule </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.SModule Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[SModule]([Id], [Name], [Description], [LogUser], [LogDate]) VALUES(@Id, @Name, @Description, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[SModule] SET [Name] = @Name, [Description] = @Description, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[SModule] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("SModule", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListActivitys?.Count() > 0)
			{
				var list = Item.ListActivitys;
				foreach (var item in list) item.IdSModule = itemId;
				using (var dal = new Activity(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListActivitys = list;
			}
			if (Item.ListPages?.Count() > 0)
			{
				var list = Item.ListPages;
				foreach (var item in list) item.IdSModule = itemId;
				using (var dal = new Page(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPages = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  SModule		
        /// </summary>
        /// <param name="Items">Business object of type SModule para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.SModule> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("SModule", quantity);
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
        /// 	For use on data access layer at assembly level, return an  SModule type object
        /// </summary>
        /// <param name="Id">Object Identifier SModule</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SModule</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.SModule ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto SModule de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a SModule</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo SModule</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.SModule> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.SModule> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[SModule] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEB.SModule> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.Activity> lstActivitys = null; 
			IEnumerable<BE.Base.Page> lstPages = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Base.relSModule.Activitys))
				{
					using (var dal = new Activity(Connection))
					{
						lstActivitys = dal.List(Keys, "IdSModule", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relSModule.Pages))
				{
					using (var dal = new Page(Connection))
					{
						lstPages = dal.List(Keys, "IdSModule", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstActivitys != null)
					{
						Item.ListActivitys = lstActivitys.Where(x => x.IdSModule == Item.Id)?.ToList();
					}
					if (lstPages != null)
					{
						Item.ListPages = lstPages.Where(x => x.IdSModule == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEB.SModule Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Base.relSModule.Activitys))
				{
					using (var dal = new Activity(Connection))
					{
						Item.ListActivitys = dal.List(Keys, "IdSModule", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relSModule.Pages))
				{
					using (var dal = new Page(Connection))
					{
						Item.ListPages = dal.List(Keys, "IdSModule", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of SModule
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SModule</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.SModule> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[SModule] ORDER By " + Order;
            IEnumerable<BEB.SModule> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of SModule
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.SModule> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[SModule] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.SModule> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of SModule
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SModule</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.SModule> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[SModule] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.SModule> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type SModule    	
        /// </summary>
        /// <param name="Id">Object identifier SModule</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SModule</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.SModule Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[SModule] WHERE [Id] = @Id";
            BEB.SModule Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public SModule() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public SModule(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal SModule(SqlConnection connection) : base(connection) { }

        #endregion

    }
}