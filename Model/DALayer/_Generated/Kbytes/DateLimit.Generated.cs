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
    /// Class     : DateLimit
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type DateLimit 
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
    public partial class DateLimit : DALEntity<BEK.DateLimit>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type DateLimit
        /// </summary>
        /// <param name="Item">Business object of type DateLimit </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.DateLimit Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[DateLimit]([Year], [Month], [Day], [LogUser], [LogDate]) VALUES(@Year, @Month, @Day, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[DateLimit] SET [Year] = @Year, [Month] = @Month, [Day] = @Day, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[DateLimit] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  DateLimit		
        /// </summary>
        /// <param name="Items">Business object of type DateLimit para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.DateLimit> Items)
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
        /// 	For use on data access layer at assembly level, return an  DateLimit type object
        /// </summary>
        /// <param name="Id">Object Identifier DateLimit</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type DateLimit</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.DateLimit ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto DateLimit de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a DateLimit</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo DateLimit</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.DateLimit> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.DateLimit> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[DateLimit] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.DateLimit> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEK.DateLimit Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of DateLimit
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type DateLimit</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.DateLimit> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[DateLimit] ORDER By " + Order;
            IEnumerable<BEK.DateLimit> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of DateLimit
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.DateLimit> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[DateLimit] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.DateLimit> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of DateLimit
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type DateLimit</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.DateLimit> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[DateLimit] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.DateLimit> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type DateLimit    	
        /// </summary>
        /// <param name="Id">Object identifier DateLimit</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type DateLimit</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.DateLimit Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[DateLimit] WHERE [Id] = @Id";
            BEK.DateLimit Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public DateLimit() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public DateLimit(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal DateLimit(SqlConnection connection) : base(connection) { }

        #endregion

    }
}