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
    /// Class     : QuoteSent
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type QuoteSent 
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
    public partial class QuoteSent : DALEntity<BEL.QuoteSent>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type QuoteSent
        /// </summary>
        /// <param name="Item">Business object of type QuoteSent </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEL.QuoteSent Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Sales].[QuoteSent]([Id], [IdQuote], [CardCode], [ClientName], [ClientMail], [CardName], [LogUser], [LogDate]) VALUES(@Id, @IdQuote, @CardCode, @ClientName, @ClientMail, @CardName, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Sales].[QuoteSent] SET [IdQuote] = @IdQuote, [CardCode] = @CardCode, [ClientName] = @ClientName, [ClientMail] = @ClientMail, [CardName] = @CardName, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Sales].[QuoteSent] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("QuoteSent", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  QuoteSent		
        /// </summary>
        /// <param name="Items">Business object of type QuoteSent para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEL.QuoteSent> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("QuoteSent", quantity);
				currentId = lastId - quantity + 1;
				if (lastId <= 0) throw new Exception("No se puede generar el identificador " + this.GetType().FullName);
			}

            for (int i = 0; i < Items.Count; i++)
            {
                var Item = Items[i];
                if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = currentId++;
                Save(ref Item);
                Items[i] = Item;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	For use on data access layer at assembly level, return an  QuoteSent type object
        /// </summary>
        /// <param name="Id">Object Identifier QuoteSent</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type QuoteSent</returns>
        /// <remarks>
        /// </remarks>		
        internal BEL.QuoteSent ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto QuoteSent de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a QuoteSent</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo QuoteSent</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEL.QuoteSent> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEL.QuoteSent> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Sales].[QuoteSent] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEL.QuoteSent> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Sales.Quote> lstQuotes = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Sales.relQuoteSent.Quote))
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
					if (lstQuotes != null)
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
        protected override void LoadRelations(ref BEL.QuoteSent Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Sales.relQuoteSent.Quote))
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
        /// 	Return an object Collection of QuoteSent
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type QuoteSent</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteSent> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Sales].[QuoteSent] ORDER By " + Order;
            IEnumerable<BEL.QuoteSent> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of QuoteSent
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteSent> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Sales].[QuoteSent] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEL.QuoteSent> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of QuoteSent
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type QuoteSent</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.QuoteSent> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[QuoteSent] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEL.QuoteSent> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type QuoteSent    	
        /// </summary>
        /// <param name="Id">Object identifier QuoteSent</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type QuoteSent</returns>
        /// <remarks>
        /// </remarks>    
        public BEL.QuoteSent Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[QuoteSent] WHERE [Id] = @Id";
            BEL.QuoteSent Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public QuoteSent() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public QuoteSent(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal QuoteSent(SqlConnection connection) : base(connection) { }

        #endregion

    }
}