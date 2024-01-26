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


namespace DALayer.Online
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Online
    /// Class     : SaleState
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type SaleState 
    ///     for the service Online.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Online
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:40 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class SaleState : DALEntity<BEO.SaleState>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type SaleState
        /// </summary>
        /// <param name="Item">Business object of type SaleState </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEO.SaleState Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Online].[SaleState]([IdSale], [StateIdc], [LogUser], [LogDate]) VALUES(@IdSale, @StateIdc, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Online].[SaleState] SET [IdSale] = @IdSale, [StateIdc] = @StateIdc, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Online].[SaleState] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  SaleState		
        /// </summary>
        /// <param name="Items">Business object of type SaleState para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEO.SaleState> Items)
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
        /// 	For use on data access layer at assembly level, return an  SaleState type object
        /// </summary>
        /// <param name="Id">Object Identifier SaleState</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SaleState</returns>
        /// <remarks>
        /// </remarks>		
        internal BEO.SaleState ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto SaleState de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a SaleState</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo SaleState</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEO.SaleState> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEO.SaleState> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Online].[SaleState] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEO.SaleState> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Online.Sale> lstSales = null;
			IEnumerable<BE.Base.Classifier> lstStates = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relSaleState.Sale))
				{
					using(var dal = new Sale(Connection))
					{
						Keys = (from i in Items select i.IdSale).Distinct();
						lstSales = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Online.relSaleState.State))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.StateIdc).Distinct();
						lstStates = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstSales != null)
					{
						Item.Sale = (from i in lstSales where i.Id == Item.IdSale select i).FirstOrDefault();
					}					if (lstStates != null)
					{
						Item.State = (from i in lstStates where i.Id == Item.StateIdc select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEO.SaleState Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relSaleState.Sale))
				{
					using (var dal = new Sale(Connection))
					{
						Item.Sale = dal.ReturnMaster(Item.IdSale, Relations);
					}
				}				if (RelationEnum.Equals(BE.Online.relSaleState.State))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.State = dal.ReturnMaster(Item.StateIdc, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of SaleState
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SaleState</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleState> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Online].[SaleState] ORDER By " + Order;
            IEnumerable<BEO.SaleState> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of SaleState
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleState> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Online].[SaleState] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEO.SaleState> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of SaleState
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SaleState</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleState> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[SaleState] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEO.SaleState> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type SaleState    	
        /// </summary>
        /// <param name="Id">Object identifier SaleState</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SaleState</returns>
        /// <remarks>
        /// </remarks>    
        public BEO.SaleState Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[SaleState] WHERE [Id] = @Id";
            BEO.SaleState Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public SaleState() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public SaleState(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal SaleState(SqlConnection connection) : base(connection) { }

        #endregion

    }
}