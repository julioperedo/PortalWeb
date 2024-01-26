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


namespace DALayer.CIESD
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : CIESD
    /// Class     : Product
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Product 
    ///     for the service CIESD.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service CIESD
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:36 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Product : DALEntity<BEC.Product>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Product
        /// </summary>
        /// <param name="Item">Business object of type Product </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEC.Product Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [CIESD].[Product]([Sku], [ItemCode], [Name], [Description], [ReturnType], [FulfillmentType], [Enabled], [LogUser], [LogDate]) VALUES(@Sku, @ItemCode, @Name, @Description, @ReturnType, @FulfillmentType, @Enabled, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [CIESD].[Product] SET [Sku] = @Sku, [ItemCode] = @ItemCode, [Name] = @Name, [Description] = @Description, [ReturnType] = @ReturnType, [FulfillmentType] = @FulfillmentType, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [CIESD].[Product] WHERE [Id] = @Id";
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
			if (Item.ListPrices?.Count() > 0)
			{
				var list = Item.ListPrices;
				foreach (var item in list) item.IdProduct = itemId;
				using (var dal = new Price(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPrices = list;
			}
			if (Item.ListPurchases?.Count() > 0)
			{
				var list = Item.ListPurchases;
				foreach (var item in list) item.IdProduct = itemId;
				using (var dal = new Purchase(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPurchases = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Product		
        /// </summary>
        /// <param name="Items">Business object of type Product para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEC.Product> Items)
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
        /// 	For use on data access layer at assembly level, return an  Product type object
        /// </summary>
        /// <param name="Id">Object Identifier Product</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Product</returns>
        /// <remarks>
        /// </remarks>		
        internal BEC.Product ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Product de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Product</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Product</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEC.Product> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEC.Product> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [CIESD].[Product] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEC.Product> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.CIESD.Price> lstPrices = null; 
			IEnumerable<BE.CIESD.Purchase> lstPurchases = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.CIESD.relProduct.Prices))
				{
					using (var dal = new Price(Connection))
					{
						lstPrices = dal.List(Keys, "IdProduct", Relations);
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relProduct.Purchases))
				{
					using (var dal = new Purchase(Connection))
					{
						lstPurchases = dal.List(Keys, "IdProduct", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstPrices != null)
					{
						Item.ListPrices = lstPrices.Where(x => x.IdProduct == Item.Id)?.ToList();
					}
					if (lstPurchases != null)
					{
						Item.ListPurchases = lstPurchases.Where(x => x.IdProduct == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEC.Product Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.CIESD.relProduct.Prices))
				{
					using (var dal = new Price(Connection))
					{
						Item.ListPrices = dal.List(Keys, "IdProduct", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.CIESD.relProduct.Purchases))
				{
					using (var dal = new Purchase(Connection))
					{
						Item.ListPurchases = dal.List(Keys, "IdProduct", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Product
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Product</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.Product> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [CIESD].[Product] ORDER By " + Order;
            IEnumerable<BEC.Product> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Product
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.Product> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [CIESD].[Product] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEC.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Product
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Product</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.Product> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Product] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEC.Product> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Product    	
        /// </summary>
        /// <param name="Id">Object identifier Product</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Product</returns>
        /// <remarks>
        /// </remarks>    
        public BEC.Product Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Product] WHERE [Id] = @Id";
            BEC.Product Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Product() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Product(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Product(SqlConnection connection) : base(connection) { }

        #endregion

    }
}