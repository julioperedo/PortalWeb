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


namespace DALayer.Staff
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Staff
    /// Class     : Replace
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Replace 
    ///     for the service Staff.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Staff
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:52 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Replace : DALEntity<BEF.Replace>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Replace
        /// </summary>
        /// <param name="Item">Business object of type Replace </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEF.Replace Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Staff].[Replace]([SellerCode], [ReplaceCode], [InitialDate], [FinalDate], [LogUser], [LogDate]) VALUES(@SellerCode, @ReplaceCode, @InitialDate, @FinalDate, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Staff].[Replace] SET [SellerCode] = @SellerCode, [ReplaceCode] = @ReplaceCode, [InitialDate] = @InitialDate, [FinalDate] = @FinalDate, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Staff].[Replace] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  Replace		
        /// </summary>
        /// <param name="Items">Business object of type Replace para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEF.Replace> Items)
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
        /// 	For use on data access layer at assembly level, return an  Replace type object
        /// </summary>
        /// <param name="Id">Object Identifier Replace</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Replace</returns>
        /// <remarks>
        /// </remarks>		
        internal BEF.Replace ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Replace de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Replace</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Replace</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEF.Replace> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEF.Replace> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Staff].[Replace] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEF.Replace> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEF.Replace Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Replace
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Replace</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Replace> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Staff].[Replace] ORDER By " + Order;
            IEnumerable<BEF.Replace> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Replace
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Replace> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Staff].[Replace] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEF.Replace> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Replace
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Replace</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Replace> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Staff].[Replace] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEF.Replace> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Replace    	
        /// </summary>
        /// <param name="Id">Object identifier Replace</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Replace</returns>
        /// <remarks>
        /// </remarks>    
        public BEF.Replace Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Staff].[Replace] WHERE [Id] = @Id";
            BEF.Replace Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Replace() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Replace(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Replace(SqlConnection connection) : base(connection) { }

        #endregion

    }
}