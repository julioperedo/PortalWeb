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


namespace DALayer.PostSale
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : PostSale
    /// Class     : ServiceCallActivity
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ServiceCallActivity 
    ///     for the service PostSale.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PostSale
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:42 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ServiceCallActivity : DALEntity<BET.ServiceCallActivity>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ServiceCallActivity
        /// </summary>
        /// <param name="Item">Business object of type ServiceCallActivity </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BET.ServiceCallActivity Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PostSale].[ServiceCallActivity]([IdServiceCall], [SAPCode], [ActivityDate], [TreatedByCode], [TreatedBy], [AssignedByCode], [AssignedBy], [Closed], [Notes], [Details], [ActivityTypeCode], [ActivityType], [SubjectCode], [Subject], [ContactCode], [Contact], [Telephone], [Attachment], [LogUser], [LogDate]) VALUES(@IdServiceCall, @SAPCode, @ActivityDate, @TreatedByCode, @TreatedBy, @AssignedByCode, @AssignedBy, @Closed, @Notes, @Details, @ActivityTypeCode, @ActivityType, @SubjectCode, @Subject, @ContactCode, @Contact, @Telephone, @Attachment, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PostSale].[ServiceCallActivity] SET [IdServiceCall] = @IdServiceCall, [SAPCode] = @SAPCode, [ActivityDate] = @ActivityDate, [TreatedByCode] = @TreatedByCode, [TreatedBy] = @TreatedBy, [AssignedByCode] = @AssignedByCode, [AssignedBy] = @AssignedBy, [Closed] = @Closed, [Notes] = @Notes, [Details] = @Details, [ActivityTypeCode] = @ActivityTypeCode, [ActivityType] = @ActivityType, [SubjectCode] = @SubjectCode, [Subject] = @Subject, [ContactCode] = @ContactCode, [Contact] = @Contact, [Telephone] = @Telephone, [Attachment] = @Attachment, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PostSale].[ServiceCallActivity] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ServiceCallActivity		
        /// </summary>
        /// <param name="Items">Business object of type ServiceCallActivity para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BET.ServiceCallActivity> Items)
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
        /// 	For use on data access layer at assembly level, return an  ServiceCallActivity type object
        /// </summary>
        /// <param name="Id">Object Identifier ServiceCallActivity</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCallActivity</returns>
        /// <remarks>
        /// </remarks>		
        internal BET.ServiceCallActivity ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ServiceCallActivity de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ServiceCallActivity</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ServiceCallActivity</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BET.ServiceCallActivity> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BET.ServiceCallActivity> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PostSale].[ServiceCallActivity] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BET.ServiceCallActivity> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.PostSale.ServiceCall> lstServiceCalls = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PostSale.relServiceCallActivity.ServiceCall))
				{
					using(var dal = new ServiceCall(Connection))
					{
						Keys = (from i in Items select i.IdServiceCall).Distinct();
						lstServiceCalls = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstServiceCalls != null)
					{
						Item.ServiceCall = (from i in lstServiceCalls where i.Id == Item.IdServiceCall select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BET.ServiceCallActivity Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PostSale.relServiceCallActivity.ServiceCall))
				{
					using (var dal = new ServiceCall(Connection))
					{
						Item.ServiceCall = dal.ReturnMaster(Item.IdServiceCall, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ServiceCallActivity
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCallActivity</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallActivity> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PostSale].[ServiceCallActivity] ORDER By " + Order;
            IEnumerable<BET.ServiceCallActivity> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ServiceCallActivity
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallActivity> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PostSale].[ServiceCallActivity] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BET.ServiceCallActivity> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ServiceCallActivity
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCallActivity</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallActivity> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCallActivity] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BET.ServiceCallActivity> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ServiceCallActivity    	
        /// </summary>
        /// <param name="Id">Object identifier ServiceCallActivity</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCallActivity</returns>
        /// <remarks>
        /// </remarks>    
        public BET.ServiceCallActivity Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCallActivity] WHERE [Id] = @Id";
            BET.ServiceCallActivity Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ServiceCallActivity() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ServiceCallActivity(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ServiceCallActivity(SqlConnection connection) : base(connection) { }

        #endregion

    }
}