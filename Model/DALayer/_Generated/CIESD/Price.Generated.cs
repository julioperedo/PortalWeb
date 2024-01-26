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
    /// Class     : Price
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Price 
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
    public partial class Price : DALEntity<BEC.Price>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Price
        /// </summary>
        /// <param name="Item">Business object of type Price </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEC.Price Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [CIESD].[Price]([IdProduct], [Amount], [Currency], [PriceType], [ValidFrom], [ValidTo], [LogUser], [LogDate]) VALUES(@IdProduct, @Amount, @Currency, @PriceType, @ValidFrom, @ValidTo, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [CIESD].[Price] SET [IdProduct] = @IdProduct, [Amount] = @Amount, [Currency] = @Currency, [PriceType] = @PriceType, [ValidFrom] = @ValidFrom, [ValidTo] = @ValidTo, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [CIESD].[Price] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  Price		
        /// </summary>
        /// <param name="Items">Business object of type Price para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEC.Price> Items)
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
        /// 	For use on data access layer at assembly level, return an  Price type object
        /// </summary>
        /// <param name="Id">Object Identifier Price</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Price</returns>
        /// <remarks>
        /// </remarks>		
        internal BEC.Price ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Price de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Price</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Price</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEC.Price> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEC.Price> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [CIESD].[Price] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEC.Price> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.CIESD.Product> lstProducts = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.CIESD.relPrice.Product))
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
        protected override void LoadRelations(ref BEC.Price Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.CIESD.relPrice.Product))
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
        /// 	Return an object Collection of Price
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Price</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.Price> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [CIESD].[Price] ORDER By " + Order;
            IEnumerable<BEC.Price> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Price
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.Price> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [CIESD].[Price] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEC.Price> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Price
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Price</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEC.Price> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Price] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEC.Price> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Price    	
        /// </summary>
        /// <param name="Id">Object identifier Price</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Price</returns>
        /// <remarks>
        /// </remarks>    
        public BEC.Price Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [CIESD].[Price] WHERE [Id] = @Id";
            BEC.Price Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Price() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Price(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Price(SqlConnection connection) : base(connection) { }

        #endregion

    }
}