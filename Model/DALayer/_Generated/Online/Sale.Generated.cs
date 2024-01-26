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
    /// Class     : Sale
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Sale 
    ///     for the service Online.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Online
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:39 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Sale : DALEntity<BEO.Sale>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Sale
        /// </summary>
        /// <param name="Item">Business object of type Sale </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEO.Sale Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Online].[Sale]([Id], [IdUser], [Code], [Name], [Address], [Commentaries], [ClientSaleNote], [Total], [SellerCode], [SellerName], [StateIdc], [WithDropShip], [LogUser], [LogDate]) VALUES(@Id, @IdUser, @Code, @Name, @Address, @Commentaries, @ClientSaleNote, @Total, @SellerCode, @SellerName, @StateIdc, @WithDropShip, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Online].[Sale] SET [IdUser] = @IdUser, [Code] = @Code, [Name] = @Name, [Address] = @Address, [Commentaries] = @Commentaries, [ClientSaleNote] = @ClientSaleNote, [Total] = @Total, [SellerCode] = @SellerCode, [SellerName] = @SellerName, [StateIdc] = @StateIdc, [WithDropShip] = @WithDropShip, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Online].[Sale] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Sale", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListSaleDetails?.Count() > 0)
			{
				var list = Item.ListSaleDetails;
				foreach (var item in list) item.IdSale = itemId;
				using (var dal = new SaleDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSaleDetails = list;
			}
			if (Item.ListSaleFiless?.Count() > 0)
			{
				var list = Item.ListSaleFiless;
				foreach (var item in list) item.IdSale = itemId;
				using (var dal = new SaleFiles(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSaleFiless = list;
			}
			if (Item.ListSaleStates?.Count() > 0)
			{
				var list = Item.ListSaleStates;
				foreach (var item in list) item.IdSale = itemId;
				using (var dal = new SaleState(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSaleStates = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Sale		
        /// </summary>
        /// <param name="Items">Business object of type Sale para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEO.Sale> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Sale", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Sale type object
        /// </summary>
        /// <param name="Id">Object Identifier Sale</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Sale</returns>
        /// <remarks>
        /// </remarks>		
        internal BEO.Sale ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Sale de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Sale</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Sale</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEO.Sale> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEO.Sale> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Online].[Sale] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEO.Sale> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Online.SaleDetail> lstSaleDetails = null; 
			IEnumerable<BE.Online.SaleFiles> lstSaleFiless = null; 
			IEnumerable<BE.Online.SaleState> lstSaleStates = null; 
			IEnumerable<BE.Security.User> lstUsers = null;
			IEnumerable<BE.Base.Classifier> lstStates = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Online.relSale.SaleDetails))
				{
					using (var dal = new SaleDetail(Connection))
					{
						lstSaleDetails = dal.List(Keys, "IdSale", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Online.relSale.SaleFiless))
				{
					using (var dal = new SaleFiles(Connection))
					{
						lstSaleFiless = dal.List(Keys, "IdSale", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Online.relSale.SaleStates))
				{
					using (var dal = new SaleState(Connection))
					{
						lstSaleStates = dal.List(Keys, "IdSale", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Online.relSale.User))
				{
					using(var dal = new Security.User(Connection))
					{
						Keys = (from i in Items select i.IdUser).Distinct();
						lstUsers = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Online.relSale.State))
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
					if (lstSaleDetails != null)
					{
						Item.ListSaleDetails = lstSaleDetails.Where(x => x.IdSale == Item.Id)?.ToList();
					}
					if (lstSaleFiless != null)
					{
						Item.ListSaleFiless = lstSaleFiless.Where(x => x.IdSale == Item.Id)?.ToList();
					}
					if (lstSaleStates != null)
					{
						Item.ListSaleStates = lstSaleStates.Where(x => x.IdSale == Item.Id)?.ToList();
					}
					if (lstUsers != null)
					{
						Item.User = (from i in lstUsers where i.Id == Item.IdUser select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEO.Sale Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Online.relSale.SaleDetails))
				{
					using (var dal = new SaleDetail(Connection))
					{
						Item.ListSaleDetails = dal.List(Keys, "IdSale", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Online.relSale.SaleFiless))
				{
					using (var dal = new SaleFiles(Connection))
					{
						Item.ListSaleFiless = dal.List(Keys, "IdSale", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Online.relSale.SaleStates))
				{
					using (var dal = new SaleState(Connection))
					{
						Item.ListSaleStates = dal.List(Keys, "IdSale", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Online.relSale.User))
				{
					using (var dal = new Security.User(Connection))
					{
						Item.User = dal.ReturnMaster(Item.IdUser, Relations);
					}
				}				if (RelationEnum.Equals(BE.Online.relSale.State))
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
        /// 	Return an object Collection of Sale
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Sale</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.Sale> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Online].[Sale] ORDER By " + Order;
            IEnumerable<BEO.Sale> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Sale
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.Sale> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Online].[Sale] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEO.Sale> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Sale
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Sale</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.Sale> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[Sale] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEO.Sale> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Sale    	
        /// </summary>
        /// <param name="Id">Object identifier Sale</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Sale</returns>
        /// <remarks>
        /// </remarks>    
        public BEO.Sale Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[Sale] WHERE [Id] = @Id";
            BEO.Sale Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Sale() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Sale(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Sale(SqlConnection connection) : base(connection) { }

        #endregion

    }
}