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
    /// Class     : PromoBanner
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type PromoBanner 
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
    public partial class PromoBanner : DALEntity<BEP.PromoBanner>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type PromoBanner
        /// </summary>
        /// <param name="Item">Business object of type PromoBanner </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.PromoBanner Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[PromoBanner]([Name], [ImageUrl], [Enabled], [InitialDate], [FinalDate], [LogUser], [LogDate]) VALUES(@Name, @ImageUrl, @Enabled, @InitialDate, @FinalDate, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[PromoBanner] SET [Name] = @Name, [ImageUrl] = @ImageUrl, [Enabled] = @Enabled, [InitialDate] = @InitialDate, [FinalDate] = @FinalDate, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[PromoBanner] WHERE [Id] = @Id";
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
			if (Item.ListPromoBannerItems?.Count() > 0)
			{
				var list = Item.ListPromoBannerItems;
				foreach (var item in list) item.IdPromo = itemId;
				using (var dal = new PromoBannerItem(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPromoBannerItems = list;
			}
			if (Item.ListPromoBannerTriggers?.Count() > 0)
			{
				var list = Item.ListPromoBannerTriggers;
				foreach (var item in list) item.IdPromo = itemId;
				using (var dal = new PromoBannerTrigger(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPromoBannerTriggers = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  PromoBanner		
        /// </summary>
        /// <param name="Items">Business object of type PromoBanner para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.PromoBanner> Items)
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
        /// 	For use on data access layer at assembly level, return an  PromoBanner type object
        /// </summary>
        /// <param name="Id">Object Identifier PromoBanner</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PromoBanner</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.PromoBanner ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto PromoBanner de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a PromoBanner</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo PromoBanner</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.PromoBanner> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.PromoBanner> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[PromoBanner] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.PromoBanner> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.PromoBannerItem> lstPromoBannerItems = null; 
			IEnumerable<BE.Product.PromoBannerTrigger> lstPromoBannerTriggers = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Product.relPromoBanner.PromoBannerItems))
				{
					using (var dal = new PromoBannerItem(Connection))
					{
						lstPromoBannerItems = dal.List(Keys, "IdPromo", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relPromoBanner.PromoBannerTriggers))
				{
					using (var dal = new PromoBannerTrigger(Connection))
					{
						lstPromoBannerTriggers = dal.List(Keys, "IdPromo", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstPromoBannerItems != null)
					{
						Item.ListPromoBannerItems = lstPromoBannerItems.Where(x => x.IdPromo == Item.Id)?.ToList();
					}
					if (lstPromoBannerTriggers != null)
					{
						Item.ListPromoBannerTriggers = lstPromoBannerTriggers.Where(x => x.IdPromo == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEP.PromoBanner Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Product.relPromoBanner.PromoBannerItems))
				{
					using (var dal = new PromoBannerItem(Connection))
					{
						Item.ListPromoBannerItems = dal.List(Keys, "IdPromo", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relPromoBanner.PromoBannerTriggers))
				{
					using (var dal = new PromoBannerTrigger(Connection))
					{
						Item.ListPromoBannerTriggers = dal.List(Keys, "IdPromo", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of PromoBanner
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PromoBanner</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PromoBanner> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[PromoBanner] ORDER By " + Order;
            IEnumerable<BEP.PromoBanner> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of PromoBanner
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PromoBanner> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[PromoBanner] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.PromoBanner> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of PromoBanner
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PromoBanner</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PromoBanner> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PromoBanner] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.PromoBanner> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type PromoBanner    	
        /// </summary>
        /// <param name="Id">Object identifier PromoBanner</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PromoBanner</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.PromoBanner Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PromoBanner] WHERE [Id] = @Id";
            BEP.PromoBanner Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public PromoBanner() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public PromoBanner(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal PromoBanner(SqlConnection connection) : base(connection) { }

        #endregion

    }
}