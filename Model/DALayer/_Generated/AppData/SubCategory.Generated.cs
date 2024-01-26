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


namespace DALayer.AppData
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : AppData
    /// Class     : SubCategory
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type SubCategory 
    ///     for the service AppData.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service AppData
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class SubCategory : DALEntity<BED.SubCategory>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type SubCategory
        /// </summary>
        /// <param name="Item">Business object of type SubCategory </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BED.SubCategory Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [AppData].[SubCategory]([Id], [Name], [IdCategory]) VALUES(@Id, @Name, @IdCategory)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [AppData].[SubCategory] SET [Name] = @Name, [IdCategory] = @IdCategory WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [AppData].[SubCategory] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("SubCategory", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  SubCategory		
        /// </summary>
        /// <param name="Items">Business object of type SubCategory para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BED.SubCategory> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("SubCategory", quantity);
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
        /// 	For use on data access layer at assembly level, return an  SubCategory type object
        /// </summary>
        /// <param name="Id">Object Identifier SubCategory</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SubCategory</returns>
        /// <remarks>
        /// </remarks>		
        internal BED.SubCategory ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto SubCategory de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a SubCategory</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo SubCategory</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BED.SubCategory> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BED.SubCategory> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [AppData].[SubCategory] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BED.SubCategory> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BED.SubCategory Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of SubCategory
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SubCategory</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.SubCategory> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [AppData].[SubCategory] ORDER By " + Order;
            IEnumerable<BED.SubCategory> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of SubCategory
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.SubCategory> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [AppData].[SubCategory] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BED.SubCategory> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of SubCategory
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SubCategory</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.SubCategory> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[SubCategory] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BED.SubCategory> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type SubCategory    	
        /// </summary>
        /// <param name="Id">Object identifier SubCategory</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SubCategory</returns>
        /// <remarks>
        /// </remarks>    
        public BED.SubCategory Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[SubCategory] WHERE [Id] = @Id";
            BED.SubCategory Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public SubCategory() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public SubCategory(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal SubCategory(SqlConnection connection) : base(connection) { }

        #endregion

    }
}