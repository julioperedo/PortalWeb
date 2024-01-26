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


namespace DALayer.Base
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Base
    /// Class     : Notification
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Notification 
    ///     for the service Base.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Base
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:35 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Notification : DALEntity<BEB.Notification>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Notification
        /// </summary>
        /// <param name="Item">Business object of type Notification </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.Notification Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[Notification]([Name], [Description], [InitialDate], [FinalDate], [Enabled], [Frequency], [Value], [MobileValue], [Popup], [LogUser], [LogDate]) VALUES(@Name, @Description, @InitialDate, @FinalDate, @Enabled, @Frequency, @Value, @MobileValue, @Popup, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[Notification] SET [Name] = @Name, [Description] = @Description, [InitialDate] = @InitialDate, [FinalDate] = @FinalDate, [Enabled] = @Enabled, [Frequency] = @Frequency, [Value] = @Value, [MobileValue] = @MobileValue, [Popup] = @Popup, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[Notification] WHERE [Id] = @Id";
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
			if (Item.ListNotificationClients?.Count() > 0)
			{
				var list = Item.ListNotificationClients;
				foreach (var item in list) item.IdNotification = itemId;
				using (var dal = new NotificationClient(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListNotificationClients = list;
			}
			if (Item.ListNotificationDetails?.Count() > 0)
			{
				var list = Item.ListNotificationDetails;
				foreach (var item in list) item.IdNotification = itemId;
				using (var dal = new NotificationDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListNotificationDetails = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Notification		
        /// </summary>
        /// <param name="Items">Business object of type Notification para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.Notification> Items)
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
        /// 	For use on data access layer at assembly level, return an  Notification type object
        /// </summary>
        /// <param name="Id">Object Identifier Notification</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Notification</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.Notification ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Notification de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Notification</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Notification</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.Notification> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.Notification> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[Notification] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEB.Notification> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.NotificationClient> lstNotificationClients = null; 
			IEnumerable<BE.Base.NotificationDetail> lstNotificationDetails = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Base.relNotification.NotificationClients))
				{
					using (var dal = new NotificationClient(Connection))
					{
						lstNotificationClients = dal.List(Keys, "IdNotification", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relNotification.NotificationDetails))
				{
					using (var dal = new NotificationDetail(Connection))
					{
						lstNotificationDetails = dal.List(Keys, "IdNotification", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstNotificationClients != null)
					{
						Item.ListNotificationClients = lstNotificationClients.Where(x => x.IdNotification == Item.Id)?.ToList();
					}
					if (lstNotificationDetails != null)
					{
						Item.ListNotificationDetails = lstNotificationDetails.Where(x => x.IdNotification == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEB.Notification Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Base.relNotification.NotificationClients))
				{
					using (var dal = new NotificationClient(Connection))
					{
						Item.ListNotificationClients = dal.List(Keys, "IdNotification", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relNotification.NotificationDetails))
				{
					using (var dal = new NotificationDetail(Connection))
					{
						Item.ListNotificationDetails = dal.List(Keys, "IdNotification", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Notification
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Notification</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Notification> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[Notification] ORDER By " + Order;
            IEnumerable<BEB.Notification> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Notification
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Notification> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[Notification] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.Notification> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Notification
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Notification</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Notification> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Notification] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.Notification> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Notification    	
        /// </summary>
        /// <param name="Id">Object identifier Notification</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Notification</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.Notification Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Notification] WHERE [Id] = @Id";
            BEB.Notification Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Notification() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Notification(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Notification(SqlConnection connection) : base(connection) { }

        #endregion

    }
}