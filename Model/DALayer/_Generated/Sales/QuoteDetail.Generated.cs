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
    /// Class     : QuoteDetail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type QuoteDetail 
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
    public partial class QuoteDetail : DALEntity<BEL.QuoteDetail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type QuoteDetail
        /// </summary>
        /// <param name="Item">Business object of type QuoteDetail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEL.QuoteDetail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Sales].[QuoteDetail]([IdQuote], [IdProduct], [ProductCode], [ProductName], [ProductDescription], [ProductImageURL], [ProductLink], [LogUser], [LogDate]) VALUES(@IdQuote, @IdProduct, @ProductCode, @ProductName, @ProductDescription, @ProductImageURL, @ProductLink, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Sales].[QuoteDetail] SET [IdQuote] = @IdQuote, [IdProduct] = @IdProduct, [ProductCode] = @ProductCode, [ProductName] = @ProductName, [ProductDescription] = @ProductDescription, [ProductImageURL] = @ProductImageURL, [ProductLink] = @ProductLink, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Sales].[QuoteDetail] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert)
					Item.Id = Convert.ToInt64(Connection.ExecuteScalar(strQuery, Item));
				else
					Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListQuoteDetailPricess?.Count() > 0)
			{
				var list = Item.ListQuoteDetailPricess;
				foreach (var item in list) item.IdDetail = itemId;
				using (var dal = new QuoteDetailPrices(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListQuoteDetailPricess = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  QuoteDetail		
        /// </summary>
        /// <param name="Items">Business object of type QuoteDetail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEL.QuoteDetail> Items)
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
        /// 	For use on data access layer at assembly level, return an  QuoteDetail type object
        /// </summary>
        /// <param name="Id">Object Identifier QuoteDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type QuoteDetail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEL.QuoteDetail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto QuoteDetail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a QuoteDetail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo QuoteDetail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEL.QuoteDetail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEL.QuoteDetail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Sales].[QuoteDetail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEL.QuoteDetail> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Sales.QuoteDetailPrices> lstQuoteDetailPricess = null; 
			IEnumerable<BE.Product.Product> lstProducts = null;
			IEnumerable<BE.Sales.Quote> lstQuotes = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Sales.relQuoteDetail.QuoteDetailPricess))
				{
					using (var dal = new QuoteDetailPrices(Connection))
					{
						lstQuoteDetailPricess = dal.List(Keys, "IdDetail", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Sales.relQuoteDetail.Product))
				{
					using(var dal = new Product.Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Sales.relQuoteDetail.Quote))
				{
					using(var dal = new Quote(Connection))
					{
						Keys = (from i in Items select i.IdQuote).Distinct();
						lstQuotes = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstQuoteDetailPricess != null)
					{
						Item.ListQuoteDetailPricess = lstQuoteDetailPricess.Where(x => x.IdDetail == Item.Id)?.ToList();
					}
					if (lstProducts != null)
					{
						Item.Product = (from i in lstProducts where i.Id == Item.IdProduct select i).FirstOrDefault();
					}					if (lstQuotes != null)
					{
						Item.Quote = (from i in lstQuotes where i.Id == Item.IdQuote select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEL.QuoteDetail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Sales.relQuoteDetail.QuoteDetailPricess))
				{
					using (var dal = new QuoteDetailPrices(Connection))
					{
						Item.ListQuoteDetailPricess = dal.List(Keys, "IdDetail", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Sales.relQuoteDetail.Product))
				{
					using (var dal = new Product.Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}				if (RelationEnum.Equals(BE.Sales.relQuoteDetail.Quote))
				{
					using (var dal = new Quote(Connection))
					{
						Item.Quote = dal.ReturnMaster(Item.IdQuote, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of QuoteDetail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type QuoteDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteDetail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Sales].[QuoteDetail] ORDER By " + Order;
            IEnumerable<BEL.QuoteDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of QuoteDetail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Sales].[QuoteDetail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEL.QuoteDetail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of QuoteDetail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type QuoteDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteDetail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[QuoteDetail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEL.QuoteDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type QuoteDetail    	
        /// </summary>
        /// <param name="Id">Object identifier QuoteDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type QuoteDetail</returns>
        /// <remarks>
        /// </remarks>    
        public BEL.QuoteDetail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[QuoteDetail] WHERE [Id] = @Id";
            BEL.QuoteDetail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public QuoteDetail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public QuoteDetail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal QuoteDetail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}