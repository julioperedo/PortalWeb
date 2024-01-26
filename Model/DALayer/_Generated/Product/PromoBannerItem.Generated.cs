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


namespace DALayer.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Product
    /// Class     : PromoBannerItem
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type PromoBannerItem 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   25/10/2022 17:52:23 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class PromoBannerItem : DALEntity<BEP.PromoBannerItem>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type PromoBannerItem
        /// </summary>
        /// <param name="Item">Business object of type PromoBannerItem </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.PromoBannerItem Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[PromoBannerItem]([IdPromo], [IdProduct], [IdSubsidiary], [Price], [LogUser], [LogDate]) VALUES(@IdPromo, @IdProduct, @IdSubsidiary, @Price, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[PromoBannerItem] SET [IdPromo] = @IdPromo, [IdProduct] = @IdProduct, [IdSubsidiary] = @IdSubsidiary, [Price] = @Price, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[PromoBannerItem] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  PromoBannerItem		
        /// </summary>
        /// <param name="Items">Business object of type PromoBannerItem para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.PromoBannerItem> Items)
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
        /// 	For use on data access layer at assembly level, return an  PromoBannerItem type object
        /// </summary>
        /// <param name="Id">Object Identifier PromoBannerItem</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PromoBannerItem</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.PromoBannerItem ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto PromoBannerItem de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a PromoBannerItem</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo PromoBannerItem</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.PromoBannerItem> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.PromoBannerItem> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[PromoBannerItem] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.PromoBannerItem> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.Product> lstProducts = null;
			IEnumerable<BE.Product.PromoBanner> lstPromos = null;
			IEnumerable<BE.Base.Classifier> lstSubsidiarys = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Product.relPromoBannerItem.Product))
				{
					using(var dal = new Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Product.relPromoBannerItem.Promo))
				{
					using(var dal = new PromoBanner(Connection))
					{
						Keys = (from i in Items select i.IdPromo).Distinct();
						lstPromos = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Product.relPromoBannerItem.Subsidiary))
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
					}					if (lstPromos != null)
					{
						Item.Promo = (from i in lstPromos where i.Id == Item.IdPromo select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.PromoBannerItem Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Product.relPromoBannerItem.Product))
				{
					using (var dal = new Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}				if (RelationEnum.Equals(BE.Product.relPromoBannerItem.Promo))
				{
					using (var dal = new PromoBanner(Connection))
					{
						Item.Promo = dal.ReturnMaster(Item.IdPromo, Relations);
					}
				}				if (RelationEnum.Equals(BE.Product.relPromoBannerItem.Subsidiary))
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
        /// 	Return an object Collection of PromoBannerItem
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PromoBannerItem</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PromoBannerItem> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[PromoBannerItem] ORDER By " + Order;
            IEnumerable<BEP.PromoBannerItem> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of PromoBannerItem
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PromoBannerItem> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[PromoBannerItem] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.PromoBannerItem> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of PromoBannerItem
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PromoBannerItem</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PromoBannerItem> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PromoBannerItem] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.PromoBannerItem> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type PromoBannerItem    	
        /// </summary>
        /// <param name="Id">Object identifier PromoBannerItem</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PromoBannerItem</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.PromoBannerItem Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PromoBannerItem] WHERE [Id] = @Id";
            BEP.PromoBannerItem Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public PromoBannerItem() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public PromoBannerItem(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal PromoBannerItem(SqlConnection connection) : base(connection) { }

        #endregion

    }
}