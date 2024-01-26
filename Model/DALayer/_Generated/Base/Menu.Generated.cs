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
    /// Class     : Menu
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Menu 
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
    public partial class Menu : DALEntity<BEB.Menu>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Menu
        /// </summary>
        /// <param name="Item">Business object of type Menu </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.Menu Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[Menu]([Id], [Title], [Order], [isSeparator], [IdPage], [IdParent], [Icon], [LogUser], [LogDate]) VALUES(@Id, @Title, @Order, @isSeparator, @IdPage, @IdParent, @Icon, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[Menu] SET [Title] = @Title, [Order] = @Order, [isSeparator] = @isSeparator, [IdPage] = @IdPage, [IdParent] = @IdParent, [Icon] = @Icon, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[Menu] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Menu", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListMenus?.Count() > 0)
			{
				var list = Item.ListMenus;
				foreach (var item in list) item.IdParent = itemId;
				using (var dal = new Menu(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListMenus = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Menu		
        /// </summary>
        /// <param name="Items">Business object of type Menu para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.Menu> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Menu", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Menu type object
        /// </summary>
        /// <param name="Id">Object Identifier Menu</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Menu</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.Menu ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Menu de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Menu</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Menu</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.Menu> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.Menu> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[Menu] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEB.Menu> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.Menu> lstMenus = null; 
			IEnumerable<BE.Base.Page> lstPages = null;
			IEnumerable<BE.Base.Menu> lstParents = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Base.relMenu.Menus))
				{
					using (var dal = new Menu(Connection))
					{
						lstMenus = dal.List(Keys, "IdParent", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relMenu.Page))
				{
					using(var dal = new Page(Connection))
					{
						Keys = (from i in Items where i.IdPage.HasValue select i.IdPage.Value).Distinct();
						lstPages = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Base.relMenu.Parent))
				{
					using(var dal = new Menu(Connection))
					{
						Keys = (from i in Items where i.IdParent.HasValue select i.IdParent.Value).Distinct();
						lstParents = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstMenus != null)
					{
						Item.ListMenus = lstMenus.Where(x => x.IdParent == Item.Id)?.ToList();
					}
					if (lstPages != null)
					{
						Item.Page = (from i in lstPages where i.Id == Item.IdPage select i).FirstOrDefault();
					}					if (lstParents != null)
					{
						Item.Parent = (from i in lstParents where i.Id == Item.IdParent select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEB.Menu Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Base.relMenu.Menus))
				{
					using (var dal = new Menu(Connection))
					{
						Item.ListMenus = dal.List(Keys, "IdParent", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relMenu.Page))
				{
					using (var dal = new Page(Connection))
					{
						if (Item.IdPage.HasValue)
						{
							Item.Page = dal.ReturnMaster(Item.IdPage.Value, Relations);
						}
					}
				}				if (RelationEnum.Equals(BE.Base.relMenu.Parent))
				{
					using (var dal = new Menu(Connection))
					{
						if (Item.IdParent.HasValue)
						{
							Item.Parent = dal.ReturnMaster(Item.IdParent.Value, Relations);
						}
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Menu
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Menu</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Menu> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[Menu] ORDER By " + Order;
            IEnumerable<BEB.Menu> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Menu
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Menu> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[Menu] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.Menu> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Menu
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Menu</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Menu> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Menu] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.Menu> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Menu    	
        /// </summary>
        /// <param name="Id">Object identifier Menu</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Menu</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.Menu Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Menu] WHERE [Id] = @Id";
            BEB.Menu Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Menu() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Menu(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Menu(SqlConnection connection) : base(connection) { }

        #endregion

    }
}