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


namespace DALayer.CIESD
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : CIESD
    /// Class     : Purchase
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Purchase 
    ///     for the service CIESD.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service CIESD
    /// </remarks>
    /// <history>
    ///     [DMC]   07/06/2022 10:15:28 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Purchase : DALEntity<BEX.Purchase>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Purchase
        /// </summary>
        /// <param name="Item">Business object of type Purchase </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEX.Purchase Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [CIESD].[Purchase]([Code], [CardCode], [IdProduct], [PurchaseDate], [Quantity], [Price], [Currency], [DocNumber], [DocType], [PurchaseOrderNeeded], [UserId], [LogUser], [LogDate]) VALUES(@Code, @CardCode, @IdProduct, @PurchaseDate, @Quantity, @Price, @Currency, @DocNumber, @DocType, @PurchaseOrderNeeded, @UserId, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [CIESD].[Purchase] SET [Code] = @Code, [CardCode] = @CardCode, [IdProduct] = @IdProduct, [PurchaseDate] = @PurchaseDate, [Quantity] = @Quantity, [Price] = @Price, [Currency] = @Currency, [DocNumber] = @DocNumber, [DocType] = @DocType, [PurchaseOrderNeeded] = @PurchaseOrderNeeded, [UserId] = @UserId, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [CIESD].[Purchase] WHERE [Id] = @Id";
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
			if (Item.ListLinks?.Count() > 0)
			{
				var list = Item.ListLinks;
				foreach (var item in list) item.IdPurchase = itemId;
				using (var dal = new Link(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListLinks = list;
			}
			if (Item.ListSentEmails?.Count() > 0)
			{
				var list = Item.ListSentEmails;
				foreach (var item in list) item.IdPurchase = itemId;
				using (var dal = new SentEmail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSentEmails = list;
			}
			if (Item.ListTokens?.Count() > 0)
			{
				var list = Item.ListTokens;
				foreach (var item in list) item.IdPurchase = itemId;
				using (var dal = new Token(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTokens = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Purchase		
        /// </summary>
        /// <param name="Items">Business object of type Purchase para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEX.Purchase> Items)
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
        /// 	For use on data access layer at assembly level, return an  Purchase type object
        /// </summary>
        /// <param name="Id">Object Identifier Purchase</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Purchase</returns>
        /// <remarks>
        /// </remarks>		
        internal BEX.Purchase ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Purchase de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Purchase</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Purchase</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEX.Purchase> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEX.Purchase> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [CIESD].[Purchase] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEX.Purchase> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.CIESD.Link> lstLinks = null; 
			IEnumerable<BE.CIESD.SentEmail> lstSentEmails = null; 
			IEnumerable<BE.CIESD.Token> lstTokens = null; 
			IEnumerable<BE.CIESD.Product> lstProducts = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.CIESD.relPurchase.Links))
				{
					using (var dal = new Link(Connection))
					{
						lstLinks = dal.List(Keys, "IdPurchase", Relations);
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relPurchase.SentEmails))
				{
					using (var dal = new SentEmail(Connection))
					{
						lstSentEmails = dal.List(Keys, "IdPurchase", Relations);
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relPurchase.Tokens))
				{
					using (var dal = new Token(Connection))
					{
						lstTokens = dal.List(Keys, "IdPurchase", Relations);
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relPurchase.Product))
				{
					using(var dal = new Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstLinks != null)
					{
						Item.ListLinks = lstLinks.Where(x => x.IdPurchase == Item.Id)?.ToList();
					}
					if (lstSentEmails != null)
					{
						Item.ListSentEmails = lstSentEmails.Where(x => x.IdPurchase == Item.Id)?.ToList();
					}
					if (lstTokens != null)
					{
						Item.ListTokens = lstTokens.Where(x => x.IdPurchase == Item.Id)?.ToList();
					}
					if (lstProducts != null)
					{
						Item.Product = (from i in lstProducts where i.Id == Item.IdProduct select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEX.Purchase Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.CIESD.relPurchase.Links))
				{
					using (var dal = new Link(Connection))
					{
						Item.ListLinks = dal.List(Keys, "IdPurchase", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relPurchase.SentEmails))
				{
					using (var dal = new SentEmail(Connection))
					{
						Item.ListSentEmails = dal.List(Keys, "IdPurchase", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relPurchase.Tokens))
				{
					using (var dal = new Token(Connection))
					{
						Item.ListTokens = dal.List(Keys, "IdPurchase", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relPurchase.Product))
				{
					using (var dal = new Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Purchase
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Purchase</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.Purchase> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [CIESD].[Purchase] ORDER By " + Order;
            IEnumerable<BEX.Purchase> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Purchase
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.Purchase> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [CIESD].[Purchase] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEX.Purchase> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Purchase
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Purchase</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEX.Purchase> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Purchase] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEX.Purchase> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Purchase    	
        /// </summary>
        /// <param name="Id">Object identifier Purchase</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Purchase</returns>
        /// <remarks>
        /// </remarks>    
        public BEX.Purchase Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Purchase] WHERE [Id] = @Id";
            BEX.Purchase Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Purchase() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Purchase(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Purchase(SqlConnection connection) : base(connection) { }

        #endregion

    }
}