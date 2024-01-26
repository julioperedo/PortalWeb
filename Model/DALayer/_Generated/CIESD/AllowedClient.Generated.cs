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


namespace DALayer.CIESD
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : CIESD
    /// Class     : AllowedClient
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type AllowedClient 
    ///     for the service CIESD.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service CIESD
    /// </remarks>
    /// <history>
    ///     [DMC]   20/4/2022 16:15:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class AllowedClient : DALEntity<BEC.AllowedClient>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type AllowedClient
        /// </summary>
        /// <param name="Item">Business object of type AllowedClient </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEC.AllowedClient Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [CIESD].[AllowedClient]([CardCode], [LogUser], [LogDate]) VALUES(@CardCode, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [CIESD].[AllowedClient] SET [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [CardCode] = @CardCode";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [CIESD].[AllowedClient] WHERE [CardCode] = @CardCode";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  AllowedClient		
        /// </summary>
        /// <param name="Items">Business object of type AllowedClient para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEC.AllowedClient> Items)
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
        /// 	For use on data access layer at assembly level, return an  AllowedClient type object
        /// </summary>
        /// <param name="Id">Object Identifier AllowedClient</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type AllowedClient</returns>
        /// <remarks>
        /// </remarks>		
        internal BEC.AllowedClient ReturnMaster(string CardCode, params Enum[] Relations)
        {
            return Search(CardCode, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto AllowedClient de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a AllowedClient</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo AllowedClient</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEC.AllowedClient> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEC.AllowedClient> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [CIESD].[AllowedClient] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEC.AllowedClient> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEC.AllowedClient Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of AllowedClient
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type AllowedClient</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.AllowedClient> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [CIESD].[AllowedClient] ORDER By " + Order;
            IEnumerable<BEC.AllowedClient> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of AllowedClient
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.AllowedClient> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [CIESD].[AllowedClient] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEC.AllowedClient> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of AllowedClient
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type AllowedClient</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.AllowedClient> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[AllowedClient] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEC.AllowedClient> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type AllowedClient    	
        /// </summary>
        /// <param name="Id">Object identifier AllowedClient</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type AllowedClient</returns>
        /// <remarks>
        /// </remarks>    
        public BEC.AllowedClient Search(string CardCode, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[AllowedClient] WHERE [CardCode] = @CardCode";
            BEC.AllowedClient Item = SQLSearch(strQuery, new { @CardCode = CardCode }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public AllowedClient() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public AllowedClient(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal AllowedClient(SqlConnection connection) : base(connection) { }

        #endregion

    }
}