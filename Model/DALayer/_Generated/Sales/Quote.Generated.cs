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
    /// Class     : Quote
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Quote 
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
    public partial class Quote : DALEntity<BEL.Quote>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Quote
        /// </summary>
        /// <param name="Item">Business object of type Quote </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEL.Quote Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Sales].[Quote]([QuoteCode], [IdSeller], [QuoteDate], [CardCode], [CardName], [ClientName], [ClientMail], [Header], [Footer], [LogUser], [LogDate]) VALUES(@QuoteCode, @IdSeller, @QuoteDate, @CardCode, @CardName, @ClientName, @ClientMail, @Header, @Footer, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Sales].[Quote] SET [QuoteCode] = @QuoteCode, [IdSeller] = @IdSeller, [QuoteDate] = @QuoteDate, [CardCode] = @CardCode, [CardName] = @CardName, [ClientName] = @ClientName, [ClientMail] = @ClientMail, [Header] = @Header, [Footer] = @Footer, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Sales].[Quote] WHERE [Id] = @Id";
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
			if (Item.ListQuoteDetails?.Count() > 0)
			{
				var list = Item.ListQuoteDetails;
				foreach (var item in list) item.IdQuote = itemId;
				using (var dal = new QuoteDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListQuoteDetails = list;
			}
			if (Item.ListQuoteSents?.Count() > 0)
			{
				var list = Item.ListQuoteSents;
				foreach (var item in list) item.IdQuote = itemId;
				using (var dal = new QuoteSent(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListQuoteSents = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Quote		
        /// </summary>
        /// <param name="Items">Business object of type Quote para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEL.Quote> Items)
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
        /// 	For use on data access layer at assembly level, return an  Quote type object
        /// </summary>
        /// <param name="Id">Object Identifier Quote</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Quote</returns>
        /// <remarks>
        /// </remarks>		
        internal BEL.Quote ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Quote de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Quote</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Quote</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEL.Quote> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEL.Quote> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Sales].[Quote] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEL.Quote> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Sales.QuoteDetail> lstQuoteDetails = null; 
			IEnumerable<BE.Sales.QuoteSent> lstQuoteSents = null; 
			IEnumerable<BE.Security.User> lstSellers = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Sales.relQuote.QuoteDetails))
				{
					using (var dal = new QuoteDetail(Connection))
					{
						lstQuoteDetails = dal.List(Keys, "IdQuote", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Sales.relQuote.QuoteSents))
				{
					using (var dal = new QuoteSent(Connection))
					{
						lstQuoteSents = dal.List(Keys, "IdQuote", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Sales.relQuote.Seller))
				{
					using(var dal = new Security.User(Connection))
					{
						Keys = (from i in Items select i.IdSeller).Distinct();
						lstSellers = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstQuoteDetails != null)
					{
						Item.ListQuoteDetails = lstQuoteDetails.Where(x => x.IdQuote == Item.Id)?.ToList();
					}
					if (lstQuoteSents != null)
					{
						Item.ListQuoteSents = lstQuoteSents.Where(x => x.IdQuote == Item.Id)?.ToList();
					}
					if (lstSellers != null)
					{
						Item.Seller = (from i in lstSellers where i.Id == Item.IdSeller select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEL.Quote Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Sales.relQuote.QuoteDetails))
				{
					using (var dal = new QuoteDetail(Connection))
					{
						Item.ListQuoteDetails = dal.List(Keys, "IdQuote", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Sales.relQuote.QuoteSents))
				{
					using (var dal = new QuoteSent(Connection))
					{
						Item.ListQuoteSents = dal.List(Keys, "IdQuote", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Sales.relQuote.Seller))
				{
					using (var dal = new Security.User(Connection))
					{
						Item.Seller = dal.ReturnMaster(Item.IdSeller, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Quote
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Quote</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.Quote> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Sales].[Quote] ORDER By " + Order;
            IEnumerable<BEL.Quote> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Quote
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.Quote> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Sales].[Quote] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEL.Quote> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Quote
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Quote</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.Quote> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[Quote] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEL.Quote> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Quote    	
        /// </summary>
        /// <param name="Id">Object identifier Quote</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Quote</returns>
        /// <remarks>
        /// </remarks>    
        public BEL.Quote Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[Quote] WHERE [Id] = @Id";
            BEL.Quote Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Quote() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Quote(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Quote(SqlConnection connection) : base(connection) { }

        #endregion

    }
}