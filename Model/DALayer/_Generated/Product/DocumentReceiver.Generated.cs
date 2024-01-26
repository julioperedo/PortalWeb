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


namespace DALayer.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Product
    /// Class     : DocumentReceiver
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type DocumentReceiver 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   26/07/2022 10:27:57 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class DocumentReceiver : DALEntity<BEP.DocumentReceiver>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type DocumentReceiver
        /// </summary>
        /// <param name="Item">Business object of type DocumentReceiver </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.DocumentReceiver Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[DocumentReceiver]([CardCode], [Name], [EMail], [Enabled], [LogUser], [LogDate]) VALUES(@CardCode, @Name, @EMail, @Enabled, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[DocumentReceiver] SET [CardCode] = @CardCode, [Name] = @Name, [EMail] = @EMail, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[DocumentReceiver] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert)
					Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, Item));
				else
					Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  DocumentReceiver		
        /// </summary>
        /// <param name="Items">Business object of type DocumentReceiver para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.DocumentReceiver> Items)
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
        /// 	For use on data access layer at assembly level, return an  DocumentReceiver type object
        /// </summary>
        /// <param name="Id">Object Identifier DocumentReceiver</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type DocumentReceiver</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.DocumentReceiver ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto DocumentReceiver de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a DocumentReceiver</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo DocumentReceiver</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.DocumentReceiver> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.DocumentReceiver> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[DocumentReceiver] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.DocumentReceiver> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEP.DocumentReceiver Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of DocumentReceiver
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type DocumentReceiver</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.DocumentReceiver> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[DocumentReceiver] ORDER By " + Order;
            IEnumerable<BEP.DocumentReceiver> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of DocumentReceiver
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.DocumentReceiver> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[DocumentReceiver] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.DocumentReceiver> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of DocumentReceiver
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type DocumentReceiver</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.DocumentReceiver> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[DocumentReceiver] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.DocumentReceiver> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type DocumentReceiver    	
        /// </summary>
        /// <param name="Id">Object identifier DocumentReceiver</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type DocumentReceiver</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.DocumentReceiver Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[DocumentReceiver] WHERE [Id] = @Id";
            BEP.DocumentReceiver Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public DocumentReceiver() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public DocumentReceiver(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal DocumentReceiver(SqlConnection connection) : base(connection) { }

        #endregion

    }
}