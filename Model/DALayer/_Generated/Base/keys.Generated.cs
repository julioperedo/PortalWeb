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
    /// Class     : keys
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type keys 
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
    public partial class keys : DALEntity<BEB.keys>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type keys
        /// </summary>
        /// <param name="Item">Business object of type keys </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.keys Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[keys]([Tabla], [Contador]) VALUES(@Tabla, @Contador)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[keys] SET [Contador] = @Contador WHERE [Tabla] = @Tabla";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[keys] WHERE [Tabla] = @Tabla";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  keys		
        /// </summary>
        /// <param name="Items">Business object of type keys para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.keys> Items)
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
        /// 	For use on data access layer at assembly level, return an  keys type object
        /// </summary>
        /// <param name="Id">Object Identifier keys</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type keys</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.keys ReturnMaster(string Tabla, params Enum[] Relations)
        {
            return Search(Tabla, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto keys de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a keys</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo keys</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.keys> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.keys> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[keys] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEB.keys> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEB.keys Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of keys
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type keys</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.keys> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[keys] ORDER By " + Order;
            IEnumerable<BEB.keys> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of keys
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.keys> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[keys] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.keys> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of keys
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type keys</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.keys> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[keys] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.keys> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type keys    	
        /// </summary>
        /// <param name="Id">Object identifier keys</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type keys</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.keys Search(string Tabla, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[keys] WHERE [Tabla] = @Tabla";
            BEB.keys Item = SQLSearch(strQuery, new { @Tabla = Tabla }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public keys() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public keys(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal keys(SqlConnection connection) : base(connection) { }

        #endregion

    }
}