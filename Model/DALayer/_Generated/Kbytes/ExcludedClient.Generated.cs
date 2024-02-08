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
using BEP = BEntities.Product;
using BEL = BEntities.Sales;
using BEA = BEntities.SAP;
using BES = BEntities.Security;
using BEF = BEntities.Staff;
using BEV = BEntities.Visits;
using BEW = BEntities.WebSite;
using BEX = BEntities.CIESD;
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;


namespace DALayer.Kbytes
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Kbytes
    /// Class     : ExcludedClient
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ExcludedClient 
    ///     for the service Kbytes.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Kbytes
    /// </remarks>
    /// <history>
    ///     [DMC]   2/2/2024 14:27:47 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ExcludedClient : DALEntity<BEK.ExcludedClient>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ExcludedClient
        /// </summary>
        /// <param name="Item">Business object of type ExcludedClient </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.ExcludedClient Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[ExcludedClient]([CardCode], [LogUser], [LogDate]) VALUES(@CardCode, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[ExcludedClient] SET [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [CardCode] = @CardCode";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[ExcludedClient] WHERE [CardCode] = @CardCode";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  ExcludedClient		
        /// </summary>
        /// <param name="Items">Business object of type ExcludedClient para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.ExcludedClient> Items)
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
        /// 	For use on data access layer at assembly level, return an  ExcludedClient type object
        /// </summary>
        /// <param name="Id">Object Identifier ExcludedClient</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ExcludedClient</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.ExcludedClient ReturnMaster(string CardCode, params Enum[] Relations)
        {
            return Search(CardCode, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ExcludedClient de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ExcludedClient</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ExcludedClient</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.ExcludedClient> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.ExcludedClient> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[ExcludedClient] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.ExcludedClient> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEK.ExcludedClient Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ExcludedClient
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ExcludedClient</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ExcludedClient> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[ExcludedClient] ORDER By " + Order;
            IEnumerable<BEK.ExcludedClient> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ExcludedClient
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ExcludedClient> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[ExcludedClient] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.ExcludedClient> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ExcludedClient
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ExcludedClient</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ExcludedClient> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ExcludedClient] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.ExcludedClient> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ExcludedClient    	
        /// </summary>
        /// <param name="Id">Object identifier ExcludedClient</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ExcludedClient</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.ExcludedClient Search(string CardCode, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ExcludedClient] WHERE [CardCode] = @CardCode";
            BEK.ExcludedClient Item = SQLSearch(strQuery, new { @CardCode = CardCode }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ExcludedClient() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ExcludedClient(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ExcludedClient(SqlConnection connection) : base(connection) { }

        #endregion

    }
}