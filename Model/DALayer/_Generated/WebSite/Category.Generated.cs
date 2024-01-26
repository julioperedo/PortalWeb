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


namespace DALayer.WebSite
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : WebSite
    /// Class     : Category
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Category 
    ///     for the service WebSite.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service WebSite
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:53 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Category : DALEntity<BEW.Category>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Category
        /// </summary>
        /// <param name="Item">Business object of type Category </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEW.Category Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [WebSite].[Category]([Id], [IdParent], [Name], [Description], [Link], [Position]) VALUES(@Id, @IdParent, @Name, @Description, @Link, @Position)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [WebSite].[Category] SET [IdParent] = @IdParent, [Name] = @Name, [Description] = @Description, [Link] = @Link, [Position] = @Position WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [WebSite].[Category] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Category", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Category		
        /// </summary>
        /// <param name="Items">Business object of type Category para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEW.Category> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Category", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Category type object
        /// </summary>
        /// <param name="Id">Object Identifier Category</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Category</returns>
        /// <remarks>
        /// </remarks>		
        internal BEW.Category ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Category de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Category</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Category</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEW.Category> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEW.Category> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [WebSite].[Category] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEW.Category> Items, params Enum[] Relations)
        {

            foreach (Enum RelationEnum in Relations)
            {

            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {

                }
            }
        }

        /// <summary>
        /// Load Relationship of an Object
        /// </summary>
        /// <param name="Item">Given Object</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <remarks></remarks>
        protected override void LoadRelations(ref BEW.Category Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Category
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Category</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEW.Category> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [WebSite].[Category] ORDER By " + Order;
            IEnumerable<BEW.Category> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Category
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEW.Category> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [WebSite].[Category] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEW.Category> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Category
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Category</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEW.Category> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [WebSite].[Category] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEW.Category> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Category    	
        /// </summary>
        /// <param name="Id">Object identifier Category</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Category</returns>
        /// <remarks>
        /// </remarks>    
        public BEW.Category Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [WebSite].[Category] WHERE [Id] = @Id";
            BEW.Category Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Category() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Category(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Category(SqlConnection connection) : base(connection) { }

        #endregion

    }
}