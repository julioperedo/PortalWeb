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
    /// Class     : SyncData
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type SyncData 
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
    public partial class SyncData : DALEntity<BED.SyncData>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type SyncData
        /// </summary>
        /// <param name="Item">Business object of type SyncData </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BED.SyncData Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [AppData].[SyncData]([Name], [Version], [App]) VALUES(@Name, @Version, @App) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [AppData].[SyncData] SET [Name] = @Name, [Version] = @Version, [App] = @App WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [AppData].[SyncData] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  SyncData		
        /// </summary>
        /// <param name="Items">Business object of type SyncData para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BED.SyncData> Items)
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
        /// 	For use on data access layer at assembly level, return an  SyncData type object
        /// </summary>
        /// <param name="Id">Object Identifier SyncData</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SyncData</returns>
        /// <remarks>
        /// </remarks>		
        internal BED.SyncData ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto SyncData de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a SyncData</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo SyncData</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BED.SyncData> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BED.SyncData> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [AppData].[SyncData] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BED.SyncData> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BED.SyncData Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of SyncData
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SyncData</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.SyncData> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [AppData].[SyncData] ORDER By " + Order;
            IEnumerable<BED.SyncData> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of SyncData
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.SyncData> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [AppData].[SyncData] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BED.SyncData> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of SyncData
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SyncData</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BED.SyncData> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[SyncData] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BED.SyncData> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type SyncData    	
        /// </summary>
        /// <param name="Id">Object identifier SyncData</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SyncData</returns>
        /// <remarks>
        /// </remarks>    
        public BED.SyncData Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [AppData].[SyncData] WHERE [Id] = @Id";
            BED.SyncData Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public SyncData() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public SyncData(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal SyncData(SqlConnection connection) : base(connection) { }

        #endregion

    }
}