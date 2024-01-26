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
    /// Class     : TempSaleDetail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type TempSaleDetail 
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
    public partial class TempSaleDetail : DALEntity<BEO.TempSaleDetail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type TempSaleDetail
        /// </summary>
        /// <param name="Item">Business object of type TempSaleDetail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEO.TempSaleDetail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Online].[TempSaleDetail]([IdSale], [IdProduct], [Quantity], [Price], [IdSubsidiary], [Warehouse], [OpenBox], [LogUser], [LogDate]) VALUES(@IdSale, @IdProduct, @Quantity, @Price, @IdSubsidiary, @Warehouse, @OpenBox, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Online].[TempSaleDetail] SET [IdSale] = @IdSale, [IdProduct] = @IdProduct, [Quantity] = @Quantity, [Price] = @Price, [IdSubsidiary] = @IdSubsidiary, [Warehouse] = @Warehouse, [OpenBox] = @OpenBox, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Online].[TempSaleDetail] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  TempSaleDetail		
        /// </summary>
        /// <param name="Items">Business object of type TempSaleDetail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEO.TempSaleDetail> Items)
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
        /// 	For use on data access layer at assembly level, return an  TempSaleDetail type object
        /// </summary>
        /// <param name="Id">Object Identifier TempSaleDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TempSaleDetail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEO.TempSaleDetail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto TempSaleDetail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a TempSaleDetail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo TempSaleDetail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEO.TempSaleDetail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEO.TempSaleDetail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Online].[TempSaleDetail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEO.TempSaleDetail> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.Product> lstProducts = null;
			IEnumerable<BE.Online.TempSale> lstSales = null;
			IEnumerable<BE.Base.Classifier> lstSubsidiarys = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relTempSaleDetail.Product))
				{
					using(var dal = new Product.Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Online.relTempSaleDetail.Sale))
				{
					using(var dal = new TempSale(Connection))
					{
						Keys = (from i in Items select i.IdSale).Distinct();
						lstSales = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Online.relTempSaleDetail.Subsidiary))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items where i.IdSubsidiary.HasValue select i.IdSubsidiary.Value).Distinct();
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
        protected override void LoadRelations(ref BEO.TempSaleDetail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relTempSaleDetail.Product))
				{
					using (var dal = new Product.Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}				if (RelationEnum.Equals(BE.Online.relTempSaleDetail.Sale))
				{
					using (var dal = new TempSale(Connection))
					{
						Item.Sale = dal.ReturnMaster(Item.IdSale, Relations);
					}
				}				if (RelationEnum.Equals(BE.Online.relTempSaleDetail.Subsidiary))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						if (Item.IdSubsidiary.HasValue)
						{
							Item.Subsidiary = dal.ReturnMaster(Item.IdSubsidiary.Value, Relations);
						}
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of TempSaleDetail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TempSaleDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.TempSaleDetail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Online].[TempSaleDetail] ORDER By " + Order;
            IEnumerable<BEO.TempSaleDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of TempSaleDetail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.TempSaleDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Online].[TempSaleDetail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEO.TempSaleDetail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of TempSaleDetail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TempSaleDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.TempSaleDetail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[TempSaleDetail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEO.TempSaleDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type TempSaleDetail    	
        /// </summary>
        /// <param name="Id">Object identifier TempSaleDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TempSaleDetail</returns>
        /// <remarks>
        /// </remarks>    
        public BEO.TempSaleDetail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[TempSaleDetail] WHERE [Id] = @Id";
            BEO.TempSaleDetail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public TempSaleDetail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public TempSaleDetail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal TempSaleDetail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}