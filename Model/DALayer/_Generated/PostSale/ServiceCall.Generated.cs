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
    /// Class     : ServiceCall
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ServiceCall 
    ///     for the service PostSale.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PostSale
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:41 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ServiceCall : DALEntity<BET.ServiceCall>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ServiceCall
        /// </summary>
        /// <param name="Item">Business object of type ServiceCall </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BET.ServiceCall Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PostSale].[ServiceCall]([SAPCode], [AssigneeCode], [Assignee], [CallType], [City], [CityCode], [Country], [CountryCode], [CreateDate], [ClientCode], [ClientName], [ItemCode], [ItemName], [LocationCode], [Location], [SerialNumber], [Resolution], [ResolutionDate], [Room], [StartDate], [StateCode], [State], [StatusCode], [Status], [Street], [Subject], [TechnicianCode], [Technician], [AdmissionDate], [City2], [FinalUser], [FinalUserEMail], [FinalUserPhone], [DeliveredBy], [ExternalService], [ExternalServiceTechnician], [ExternalServiceNumber], [ExternalServiceAddress], [GuideNumber], [Transport], [CountedPieces], [PriorCountedPieces], [DiffCountedPieces], [PurchaseDate], [ReportedBy], [ReceivedBy], [RefNV], [Comments], [CloseDate], [FileName], [FilePath], [DeliveredDate], [Brand], [Warranty], [Priority], [OriginCode], [ProblemTypeCode], [LogUser], [LogDate]) VALUES(@SAPCode, @AssigneeCode, @Assignee, @CallType, @City, @CityCode, @Country, @CountryCode, @CreateDate, @ClientCode, @ClientName, @ItemCode, @ItemName, @LocationCode, @Location, @SerialNumber, @Resolution, @ResolutionDate, @Room, @StartDate, @StateCode, @State, @StatusCode, @Status, @Street, @Subject, @TechnicianCode, @Technician, @AdmissionDate, @City2, @FinalUser, @FinalUserEMail, @FinalUserPhone, @DeliveredBy, @ExternalService, @ExternalServiceTechnician, @ExternalServiceNumber, @ExternalServiceAddress, @GuideNumber, @Transport, @CountedPieces, @PriorCountedPieces, @DiffCountedPieces, @PurchaseDate, @ReportedBy, @ReceivedBy, @RefNV, @Comments, @CloseDate, @FileName, @FilePath, @DeliveredDate, @Brand, @Warranty, @Priority, @OriginCode, @ProblemTypeCode, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PostSale].[ServiceCall] SET [SAPCode] = @SAPCode, [AssigneeCode] = @AssigneeCode, [Assignee] = @Assignee, [CallType] = @CallType, [City] = @City, [CityCode] = @CityCode, [Country] = @Country, [CountryCode] = @CountryCode, [CreateDate] = @CreateDate, [ClientCode] = @ClientCode, [ClientName] = @ClientName, [ItemCode] = @ItemCode, [ItemName] = @ItemName, [LocationCode] = @LocationCode, [Location] = @Location, [SerialNumber] = @SerialNumber, [Resolution] = @Resolution, [ResolutionDate] = @ResolutionDate, [Room] = @Room, [StartDate] = @StartDate, [StateCode] = @StateCode, [State] = @State, [StatusCode] = @StatusCode, [Status] = @Status, [Street] = @Street, [Subject] = @Subject, [TechnicianCode] = @TechnicianCode, [Technician] = @Technician, [AdmissionDate] = @AdmissionDate, [City2] = @City2, [FinalUser] = @FinalUser, [FinalUserEMail] = @FinalUserEMail, [FinalUserPhone] = @FinalUserPhone, [DeliveredBy] = @DeliveredBy, [ExternalService] = @ExternalService, [ExternalServiceTechnician] = @ExternalServiceTechnician, [ExternalServiceNumber] = @ExternalServiceNumber, [ExternalServiceAddress] = @ExternalServiceAddress, [GuideNumber] = @GuideNumber, [Transport] = @Transport, [CountedPieces] = @CountedPieces, [PriorCountedPieces] = @PriorCountedPieces, [DiffCountedPieces] = @DiffCountedPieces, [PurchaseDate] = @PurchaseDate, [ReportedBy] = @ReportedBy, [ReceivedBy] = @ReceivedBy, [RefNV] = @RefNV, [Comments] = @Comments, [CloseDate] = @CloseDate, [FileName] = @FileName, [FilePath] = @FilePath, [DeliveredDate] = @DeliveredDate, [Brand] = @Brand, [Warranty] = @Warranty, [Priority] = @Priority, [OriginCode] = @OriginCode, [ProblemTypeCode] = @ProblemTypeCode, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PostSale].[ServiceCall] WHERE [Id] = @Id";
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
			if (Item.ListServiceCallActivitys?.Count() > 0)
			{
				var list = Item.ListServiceCallActivitys;
				foreach (var item in list) item.IdServiceCall = itemId;
				using (var dal = new ServiceCallActivity(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListServiceCallActivitys = list;
			}
			if (Item.ListServiceCallFiles?.Count() > 0)
			{
				var list = Item.ListServiceCallFiles;
				foreach (var item in list) item.IdServiceCall = itemId;
				using (var dal = new ServiceCallFile(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListServiceCallFiles = list;
			}
			if (Item.ListServiceCallSolutions?.Count() > 0)
			{
				var list = Item.ListServiceCallSolutions;
				foreach (var item in list) item.IdServiceCall = itemId;
				using (var dal = new ServiceCallSolution(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListServiceCallSolutions = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  ServiceCall		
        /// </summary>
        /// <param name="Items">Business object of type ServiceCall para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BET.ServiceCall> Items)
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
        /// 	For use on data access layer at assembly level, return an  ServiceCall type object
        /// </summary>
        /// <param name="Id">Object Identifier ServiceCall</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCall</returns>
        /// <remarks>
        /// </remarks>		
        internal BET.ServiceCall ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ServiceCall de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ServiceCall</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ServiceCall</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BET.ServiceCall> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BET.ServiceCall> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PostSale].[ServiceCall] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BET.ServiceCall> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.PostSale.ServiceCallActivity> lstServiceCallActivitys = null; 
			IEnumerable<BE.PostSale.ServiceCallFile> lstServiceCallFiles = null; 
			IEnumerable<BE.PostSale.ServiceCallSolution> lstServiceCallSolutions = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.PostSale.relServiceCall.ServiceCallActivitys))
				{
					using (var dal = new ServiceCallActivity(Connection))
					{
						lstServiceCallActivitys = dal.List(Keys, "IdServiceCall", Relations);
					}
				}
				if (RelationEnum.Equals(BE.PostSale.relServiceCall.ServiceCallFiles))
				{
					using (var dal = new ServiceCallFile(Connection))
					{
						lstServiceCallFiles = dal.List(Keys, "IdServiceCall", Relations);
					}
				}
				if (RelationEnum.Equals(BE.PostSale.relServiceCall.ServiceCallSolutions))
				{
					using (var dal = new ServiceCallSolution(Connection))
					{
						lstServiceCallSolutions = dal.List(Keys, "IdServiceCall", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstServiceCallActivitys != null)
					{
						Item.ListServiceCallActivitys = lstServiceCallActivitys.Where(x => x.IdServiceCall == Item.Id)?.ToList();
					}
					if (lstServiceCallFiles != null)
					{
						Item.ListServiceCallFiles = lstServiceCallFiles.Where(x => x.IdServiceCall == Item.Id)?.ToList();
					}
					if (lstServiceCallSolutions != null)
					{
						Item.ListServiceCallSolutions = lstServiceCallSolutions.Where(x => x.IdServiceCall == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BET.ServiceCall Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.PostSale.relServiceCall.ServiceCallActivitys))
				{
					using (var dal = new ServiceCallActivity(Connection))
					{
						Item.ListServiceCallActivitys = dal.List(Keys, "IdServiceCall", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.PostSale.relServiceCall.ServiceCallFiles))
				{
					using (var dal = new ServiceCallFile(Connection))
					{
						Item.ListServiceCallFiles = dal.List(Keys, "IdServiceCall", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.PostSale.relServiceCall.ServiceCallSolutions))
				{
					using (var dal = new ServiceCallSolution(Connection))
					{
						Item.ListServiceCallSolutions = dal.List(Keys, "IdServiceCall", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ServiceCall
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCall</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCall> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PostSale].[ServiceCall] ORDER By " + Order;
            IEnumerable<BET.ServiceCall> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ServiceCall
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCall> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PostSale].[ServiceCall] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BET.ServiceCall> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ServiceCall
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCall</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCall> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCall] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BET.ServiceCall> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ServiceCall    	
        /// </summary>
        /// <param name="Id">Object identifier ServiceCall</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCall</returns>
        /// <remarks>
        /// </remarks>    
        public BET.ServiceCall Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCall] WHERE [Id] = @Id";
            BET.ServiceCall Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ServiceCall() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ServiceCall(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ServiceCall(SqlConnection connection) : base(connection) { }

        #endregion

    }
}