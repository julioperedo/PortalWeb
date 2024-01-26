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
    /// Class     : NotificationDetail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type NotificationDetail 
    ///     for the service Base.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Base
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:36 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class NotificationDetail : DALEntity<BEB.NotificationDetail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type NotificationDetail
        /// </summary>
        /// <param name="Item">Business object of type NotificationDetail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.NotificationDetail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[NotificationDetail]([IdNotification], [IdLine], [LogUser], [LogDate]) VALUES(@IdNotification, @IdLine, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[NotificationDetail] SET [IdNotification] = @IdNotification, [IdLine] = @IdLine, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[NotificationDetail] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  NotificationDetail		
        /// </summary>
        /// <param name="Items">Business object of type NotificationDetail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.NotificationDetail> Items)
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
        /// 	For use on data access layer at assembly level, return an  NotificationDetail type object
        /// </summary>
        /// <param name="Id">Object Identifier NotificationDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type NotificationDetail</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.NotificationDetail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto NotificationDetail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a NotificationDetail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo NotificationDetail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.NotificationDetail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.NotificationDetail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[NotificationDetail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEB.NotificationDetail> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.Line> lstLines = null;
			IEnumerable<BE.Base.Notification> lstNotifications = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Base.relNotificationDetail.Line))
				{
					using(var dal = new Product.Line(Connection))
					{
						Keys = (from i in Items select i.IdLine).Distinct();
						lstLines = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Base.relNotificationDetail.Notification))
				{
					using(var dal = new Notification(Connection))
					{
						Keys = (from i in Items select i.IdNotification).Distinct();
						lstNotifications = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstLines != null)
					{
						Item.Line = (from i in lstLines where i.Id == Item.IdLine select i).FirstOrDefault();
					}					if (lstNotifications != null)
					{
						Item.Notification = (from i in lstNotifications where i.Id == Item.IdNotification select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEB.NotificationDetail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Base.relNotificationDetail.Line))
				{
					using (var dal = new Product.Line(Connection))
					{
						Item.Line = dal.ReturnMaster(Item.IdLine, Relations);
					}
				}				if (RelationEnum.Equals(BE.Base.relNotificationDetail.Notification))
				{
					using (var dal = new Notification(Connection))
					{
						Item.Notification = dal.ReturnMaster(Item.IdNotification, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of NotificationDetail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type NotificationDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.NotificationDetail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[NotificationDetail] ORDER By " + Order;
            IEnumerable<BEB.NotificationDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of NotificationDetail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.NotificationDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[NotificationDetail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.NotificationDetail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of NotificationDetail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type NotificationDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.NotificationDetail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[NotificationDetail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.NotificationDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type NotificationDetail    	
        /// </summary>
        /// <param name="Id">Object identifier NotificationDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type NotificationDetail</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.NotificationDetail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[NotificationDetail] WHERE [Id] = @Id";
            BEB.NotificationDetail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public NotificationDetail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public NotificationDetail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal NotificationDetail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}