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
    /// Class     : SaleDetail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type SaleDetail 
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
    public partial class SaleDetail : DALEntity<BEO.SaleDetail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type SaleDetail
        /// </summary>
        /// <param name="Item">Business object of type SaleDetail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEO.SaleDetail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Online].[SaleDetail]([Id], [IdSale], [IdProduct], [Quantity], [ApprovedQuantity], [Price], [IdSubsidiary], [Warehouse], [OpenBox], [LogUser], [LogDate]) VALUES(@Id, @IdSale, @IdProduct, @Quantity, @ApprovedQuantity, @Price, @IdSubsidiary, @Warehouse, @OpenBox, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Online].[SaleDetail] SET [IdSale] = @IdSale, [IdProduct] = @IdProduct, [Quantity] = @Quantity, [ApprovedQuantity] = @ApprovedQuantity, [Price] = @Price, [IdSubsidiary] = @IdSubsidiary, [Warehouse] = @Warehouse, [OpenBox] = @OpenBox, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Online].[SaleDetail] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("SaleDetail", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  SaleDetail		
        /// </summary>
        /// <param name="Items">Business object of type SaleDetail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEO.SaleDetail> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("SaleDetail", quantity);
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
        /// 	For use on data access layer at assembly level, return an  SaleDetail type object
        /// </summary>
        /// <param name="Id">Object Identifier SaleDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SaleDetail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEO.SaleDetail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto SaleDetail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a SaleDetail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo SaleDetail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEO.SaleDetail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEO.SaleDetail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Online].[SaleDetail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEO.SaleDetail> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.Product> lstProducts = null;
			IEnumerable<BE.Online.Sale> lstSales = null;
			IEnumerable<BE.Base.Classifier> lstSubsidiarys = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relSaleDetail.Product))
				{
					using(var dal = new Product.Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Online.relSaleDetail.Sale))
				{
					using(var dal = new Sale(Connection))
					{
						Keys = (from i in Items select i.IdSale).Distinct();
						lstSales = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Online.relSaleDetail.Subsidiary))
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
					if (lstProducts != null)
					{
						Item.Product = (from i in lstProducts where i.Id == Item.IdProduct select i).FirstOrDefault();
					}					if (lstSales != null)
					{
						Item.Sale = (from i in lstSales where i.Id == Item.IdSale select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEO.SaleDetail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relSaleDetail.Product))
				{
					using (var dal = new Product.Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}				if (RelationEnum.Equals(BE.Online.relSaleDetail.Sale))
				{
					using (var dal = new Sale(Connection))
					{
						Item.Sale = dal.ReturnMaster(Item.IdSale, Relations);
					}
				}				if (RelationEnum.Equals(BE.Online.relSaleDetail.Subsidiary))
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
        /// 	Return an object Collection of SaleDetail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SaleDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleDetail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Online].[SaleDetail] ORDER By " + Order;
            IEnumerable<BEO.SaleDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of SaleDetail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Online].[SaleDetail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEO.SaleDetail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of SaleDetail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SaleDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleDetail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[SaleDetail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEO.SaleDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type SaleDetail    	
        /// </summary>
        /// <param name="Id">Object identifier SaleDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SaleDetail</returns>
        /// <remarks>
        /// </remarks>    
        public BEO.SaleDetail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[SaleDetail] WHERE [Id] = @Id";
            BEO.SaleDetail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public SaleDetail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public SaleDetail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal SaleDetail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}