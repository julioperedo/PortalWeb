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


namespace DALayer.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Product
    /// Class     : Line
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Line 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   27/04/2022 11:04:00 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Line : DALEntity<BEP.Line>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Line
        /// </summary>
        /// <param name="Item">Business object of type Line </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.Line Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[Line]([Id], [Name], [Description], [Header], [Footer], [ImageURL], [HasExternalPrice], [ExternalSiteName], [FilterType], [WhenFilteredShowInfo], [IdManager], [LogUser], [LogDate]) VALUES(@Id, @Name, @Description, @Header, @Footer, @ImageURL, @HasExternalPrice, @ExternalSiteName, @FilterType, @WhenFilteredShowInfo, @IdManager, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[Line] SET [Name] = @Name, [Description] = @Description, [Header] = @Header, [Footer] = @Footer, [ImageURL] = @ImageURL, [HasExternalPrice] = @HasExternalPrice, [ExternalSiteName] = @ExternalSiteName, [FilterType] = @FilterType, [WhenFilteredShowInfo] = @WhenFilteredShowInfo, [IdManager] = @IdManager, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[Line] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Line", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListNotificationDetails?.Count() > 0)
			{
				var list = Item.ListNotificationDetails;
				foreach (var item in list) item.IdLine = itemId;
				using (var dal = new DALayer.Base.NotificationDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListNotificationDetails = list;
			}
			if (Item.ListOffersMailConfigs?.Count() > 0)
			{
				var list = Item.ListOffersMailConfigs;
				foreach (var item in list) item.IdLine = itemId;
				using (var dal = new DALayer.Marketing.OffersMailConfig(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListOffersMailConfigs = list;
			}
			if (Item.ListLineDetails?.Count() > 0)
			{
				var list = Item.ListLineDetails;
				foreach (var item in list) item.IdLine = itemId;
				using (var dal = new LineDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListLineDetails = list;
			}
			if (Item.ListLineNotAlloweds?.Count() > 0)
			{
				var list = Item.ListLineNotAlloweds;
				foreach (var item in list) item.IdLine = itemId;
				using (var dal = new DALayer.Security.LineNotAllowed(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListLineNotAlloweds = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Line		
        /// </summary>
        /// <param name="Items">Business object of type Line para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.Line> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Line", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Line type object
        /// </summary>
        /// <param name="Id">Object Identifier Line</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Line</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.Line ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Line de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Line</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Line</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.Line> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.Line> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[Line] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.Line> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.NotificationDetail> lstNotificationDetails = null; 
			IEnumerable<BE.Marketing.OffersMailConfig> lstOffersMailConfigs = null; 
			IEnumerable<BE.Product.LineDetail> lstLineDetails = null; 
			IEnumerable<BE.Security.LineNotAllowed> lstLineNotAlloweds = null; 
			IEnumerable<BE.Staff.Member> lstManagers = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Product.relLine.NotificationDetails))
				{
					using (var dal = new DALayer.Base.NotificationDetail(Connection))
					{
						lstNotificationDetails = dal.List(Keys, "IdLine", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.OffersMailConfigs))
				{
					using (var dal = new DALayer.Marketing.OffersMailConfig(Connection))
					{
						lstOffersMailConfigs = dal.List(Keys, "IdLine", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.LineDetails))
				{
					using (var dal = new LineDetail(Connection))
					{
						lstLineDetails = dal.List(Keys, "IdLine", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.LineNotAlloweds))
				{
					using (var dal = new DALayer.Security.LineNotAllowed(Connection))
					{
						lstLineNotAlloweds = dal.List(Keys, "IdLine", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.Manager))
				{
					using(var dal = new Staff.Member(Connection))
					{
						Keys = (from i in Items where i.IdManager.HasValue select i.IdManager.Value).Distinct();
						lstManagers = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstNotificationDetails != null)
					{
						Item.ListNotificationDetails = lstNotificationDetails.Where(x => x.IdLine == Item.Id)?.ToList();
					}
					if (lstOffersMailConfigs != null)
					{
						Item.ListOffersMailConfigs = lstOffersMailConfigs.Where(x => x.IdLine == Item.Id)?.ToList();
					}
					if (lstLineDetails != null)
					{
						Item.ListLineDetails = lstLineDetails.Where(x => x.IdLine == Item.Id)?.ToList();
					}
					if (lstLineNotAlloweds != null)
					{
						Item.ListLineNotAlloweds = lstLineNotAlloweds.Where(x => x.IdLine == Item.Id)?.ToList();
					}
					if (lstManagers != null)
					{
						Item.Manager = (from i in lstManagers where i.Id == Item.IdManager select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.Line Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Product.relLine.NotificationDetails))
				{
					using (var dal = new DALayer.Base.NotificationDetail(Connection))
					{
						Item.ListNotificationDetails = dal.List(Keys, "IdLine", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.OffersMailConfigs))
				{
					using (var dal = new DALayer.Marketing.OffersMailConfig(Connection))
					{
						Item.ListOffersMailConfigs = dal.List(Keys, "IdLine", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.LineDetails))
				{
					using (var dal = new LineDetail(Connection))
					{
						Item.ListLineDetails = dal.List(Keys, "IdLine", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.LineNotAlloweds))
				{
					using (var dal = new DALayer.Security.LineNotAllowed(Connection))
					{
						Item.ListLineNotAlloweds = dal.List(Keys, "IdLine", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relLine.Manager))
				{
					using (var dal = new Staff.Member(Connection))
					{
						if (Item.IdManager.HasValue)
						{
							Item.Manager = dal.ReturnMaster(Item.IdManager.Value, Relations);
						}
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Line
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Line</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Line> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[Line] ORDER By " + Order;
            IEnumerable<BEP.Line> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Line
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Line> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[Line] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.Line> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Line
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Line</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Line> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[Line] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.Line> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Line    	
        /// </summary>
        /// <param name="Id">Object identifier Line</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Line</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.Line Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[Line] WHERE [Id] = @Id";
            BEP.Line Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Line() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Line(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Line(SqlConnection connection) : base(connection) { }

        #endregion

    }
}