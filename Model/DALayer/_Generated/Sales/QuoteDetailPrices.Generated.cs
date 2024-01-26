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


namespace DALayer.Sales
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Sales
    /// Class     : QuoteDetailPrices
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type QuoteDetailPrices 
    ///     for the service Sales.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Sales
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:46 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class QuoteDetailPrices : DALEntity<BEL.QuoteDetailPrices>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type QuoteDetailPrices
        /// </summary>
        /// <param name="Item">Business object of type QuoteDetailPrices </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEL.QuoteDetailPrices Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Sales].[QuoteDetailPrices]([IdDetail], [IdSubsidiary], [Price], [Quantity], [Observations], [Selected], [LogUser], [LogDate]) VALUES(@IdDetail, @IdSubsidiary, @Price, @Quantity, @Observations, @Selected, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Sales].[QuoteDetailPrices] SET [IdDetail] = @IdDetail, [IdSubsidiary] = @IdSubsidiary, [Price] = @Price, [Quantity] = @Quantity, [Observations] = @Observations, [Selected] = @Selected, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Sales].[QuoteDetailPrices] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  QuoteDetailPrices		
        /// </summary>
        /// <param name="Items">Business object of type QuoteDetailPrices para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEL.QuoteDetailPrices> Items)
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
        /// 	For use on data access layer at assembly level, return an  QuoteDetailPrices type object
        /// </summary>
        /// <param name="Id">Object Identifier QuoteDetailPrices</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type QuoteDetailPrices</returns>
        /// <remarks>
        /// </remarks>		
        internal BEL.QuoteDetailPrices ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto QuoteDetailPrices de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a QuoteDetailPrices</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo QuoteDetailPrices</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEL.QuoteDetailPrices> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEL.QuoteDetailPrices> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Sales].[QuoteDetailPrices] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEL.QuoteDetailPrices> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Sales.QuoteDetail> lstDetails = null;
			IEnumerable<BE.Base.Classifier> lstSubsidiarys = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Sales.relQuoteDetailPrices.Detail))
				{
					using(var dal = new QuoteDetail(Connection))
					{
						Keys = (from i in Items select i.IdDetail).Distinct();
						lstDetails = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Sales.relQuoteDetailPrices.Subsidiary))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.IdSubsidiary).Distinct();
						lstSubsidiarys = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstDetails != null)
					{
						Item.Detail = (from i in lstDetails where i.Id == Item.IdDetail select i).FirstOrDefault();
					}					if (lstSubsidiarys != null)
					{
						Item.Subsidiary = (from i in lstSubsidiarys where i.Id == Item.IdSubsidiary select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEL.QuoteDetailPrices Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Sales.relQuoteDetailPrices.Detail))
				{
					using (var dal = new QuoteDetail(Connection))
					{
						Item.Detail = dal.ReturnMaster(Item.IdDetail, Relations);
					}
				}				if (RelationEnum.Equals(BE.Sales.relQuoteDetailPrices.Subsidiary))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.Subsidiary = dal.ReturnMaster(Item.IdSubsidiary, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of QuoteDetailPrices
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type QuoteDetailPrices</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteDetailPrices> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Sales].[QuoteDetailPrices] ORDER By " + Order;
            IEnumerable<BEL.QuoteDetailPrices> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of QuoteDetailPrices
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteDetailPrices> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Sales].[QuoteDetailPrices] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEL.QuoteDetailPrices> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of QuoteDetailPrices
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type QuoteDetailPrices</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteDetailPrices> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[QuoteDetailPrices] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEL.QuoteDetailPrices> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type QuoteDetailPrices    	
        /// </summary>
        /// <param name="Id">Object identifier QuoteDetailPrices</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type QuoteDetailPrices</returns>
        /// <remarks>
        /// </remarks>    
        public BEL.QuoteDetailPrices Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[QuoteDetailPrices] WHERE [Id] = @Id";
            BEL.QuoteDetailPrices Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public QuoteDetailPrices() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public QuoteDetailPrices(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal QuoteDetailPrices(SqlConnection connection) : base(connection) { }

        #endregion

    }
}