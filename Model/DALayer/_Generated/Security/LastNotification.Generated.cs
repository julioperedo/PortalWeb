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
    /// Class     : LastNotification
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type LastNotification 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:48 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class LastNotification : DALEntity<BES.LastNotification>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type LastNotification
        /// </summary>
        /// <param name="Item">Business object of type LastNotification </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.LastNotification Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[LastNotification]([IdNotification], [LogUser], [LogDate]) VALUES(@IdNotification, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[LastNotification] SET [LogDate] = @LogDate WHERE [IdNotification] = @IdNotification AND [LogUser] = @LogUser";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[LastNotification] WHERE [IdNotification] = @IdNotification AND [LogUser] = @LogUser";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  LastNotification		
        /// </summary>
        /// <param name="Items">Business object of type LastNotification para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.LastNotification> Items)
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
        /// 	For use on data access layer at assembly level, return an  LastNotification type object
        /// </summary>
        /// <param name="Id">Object Identifier LastNotification</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type LastNotification</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.LastNotification ReturnMaster(long IdNotification, long LogUser, params Enum[] Relations)
        {
            return Search(IdNotification, LogUser, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto LastNotification de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a LastNotification</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo LastNotification</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.LastNotification> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.LastNotification> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[LastNotification] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.LastNotification> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BES.LastNotification Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of LastNotification
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type LastNotification</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.LastNotification> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[LastNotification] ORDER By " + Order;
            IEnumerable<BES.LastNotification> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of LastNotification
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.LastNotification> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[LastNotification] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.LastNotification> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of LastNotification
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type LastNotification</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.LastNotification> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[LastNotification] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.LastNotification> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type LastNotification    	
        /// </summary>
        /// <param name="Id">Object identifier LastNotification</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type LastNotification</returns>
        /// <remarks>
        /// </remarks>    
        public BES.LastNotification Search(long IdNotification, long LogUser, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[LastNotification] WHERE [IdNotification] = @IdNotification AND [LogUser] = @LogUser";
            BES.LastNotification Item = SQLSearch(strQuery, new { @IdNotification = IdNotification, @LogUser = LogUser }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public LastNotification() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public LastNotification(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal LastNotification(SqlConnection connection) : base(connection) { }

        #endregion

    }
}