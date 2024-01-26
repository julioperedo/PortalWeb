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


namespace DALayer.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Product
    /// Class     : PriceGroupLineClient
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type PriceGroupLineClient 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   2/8/2023 11:58:00 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class PriceGroupLineClient : DALEntity<BEP.PriceGroupLineClient>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type PriceGroupLineClient
        /// </summary>
        /// <param name="Item">Business object of type PriceGroupLineClient </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.PriceGroupLineClient Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[PriceGroupLineClient]([CardCode], [IdGroupLine], [LogUser], [LogDate]) VALUES(@CardCode, @IdGroupLine, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[PriceGroupLineClient] SET [IdGroupLine] = @IdGroupLine, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [CardCode] = @CardCode";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[PriceGroupLineClient] WHERE [CardCode] = @CardCode";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  PriceGroupLineClient		
        /// </summary>
        /// <param name="Items">Business object of type PriceGroupLineClient para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.PriceGroupLineClient> Items)
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
        /// 	For use on data access layer at assembly level, return an  PriceGroupLineClient type object
        /// </summary>
        /// <param name="Id">Object Identifier PriceGroupLineClient</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PriceGroupLineClient</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.PriceGroupLineClient ReturnMaster(string CardCode, params Enum[] Relations)
        {
            return Search(CardCode, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto PriceGroupLineClient de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a PriceGroupLineClient</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo PriceGroupLineClient</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.PriceGroupLineClient> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.PriceGroupLineClient> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[PriceGroupLineClient] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.PriceGroupLineClient> Items, params Enum[] Relations)
        {
            IEnumerable<long> Keys;
            IEnumerable<BE.Product.PriceGroupLine> lstGroupLines = null;

            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BE.Product.relPriceGroupLineClient.GroupLine))
                {
                    using (var dal = new PriceGroupLine(Connection))
                    {
                        Keys = (from i in Items select i.IdGroupLine).Distinct();
                        lstGroupLines = dal.ReturnChild(Keys, Relations)?.ToList();
                    }
                }
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
                    if (lstGroupLines != null)
                    {
                        Item.GroupLine = (from i in lstGroupLines where i.Id == Item.IdGroupLine select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.PriceGroupLineClient Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BE.Product.relPriceGroupLineClient.GroupLine))
                {
                    using (var dal = new PriceGroupLine(Connection))
                    {
                        Item.GroupLine = dal.ReturnMaster(Item.IdGroupLine, Relations);
                    }
                }
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of PriceGroupLineClient
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PriceGroupLineClient</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroupLineClient> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[PriceGroupLineClient] ORDER By " + Order;
            IEnumerable<BEP.PriceGroupLineClient> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of PriceGroupLineClient
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroupLineClient> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[PriceGroupLineClient] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.PriceGroupLineClient> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of PriceGroupLineClient
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PriceGroupLineClient</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroupLineClient> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PriceGroupLineClient] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.PriceGroupLineClient> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type PriceGroupLineClient    	
        /// </summary>
        /// <param name="Id">Object identifier PriceGroupLineClient</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PriceGroupLineClient</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.PriceGroupLineClient Search(string CardCode, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PriceGroupLineClient] WHERE [CardCode] = @CardCode";
            BEP.PriceGroupLineClient Item = SQLSearch(strQuery, new { @CardCode = CardCode }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public PriceGroupLineClient() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public PriceGroupLineClient(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal PriceGroupLineClient(SqlConnection connection) : base(connection) { }

        #endregion

    }
}