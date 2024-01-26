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
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;


namespace DALayer.HumanResources
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : HumanResources
    /// Class     : Request
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Request 
    ///     for the service HumanResources.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service HumanResources
    /// </remarks>
    /// <history>
    ///     [DMC]   4/12/2023 14:06:20 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Request : DALEntity<BEH.Request>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Request
        /// </summary>
        /// <param name="Item">Business object of type Request </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.Request Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[Request]([IdEmployee], [RequestDate], [IdType], [IdState], [Comments], [RejectComments], [ExternalCode], [LogUser], [LogDate]) VALUES(@IdEmployee, @RequestDate, @IdType, @IdState, @Comments, @RejectComments, @ExternalCode, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[Request] SET [IdEmployee] = @IdEmployee, [RequestDate] = @RequestDate, [IdType] = @IdType, [IdState] = @IdState, [Comments] = @Comments, [RejectComments] = @RejectComments, [ExternalCode] = @ExternalCode, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[Request] WHERE [Id] = @Id";
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
			if (Item.ListHomeOffices?.Count() > 0)
			{
				var list = Item.ListHomeOffices;
				foreach (var item in list) item.IdRequest = itemId;
				using (var dal = new HomeOffice(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListHomeOffices = list;
			}
			if (Item.ListLicenses?.Count() > 0)
			{
				var list = Item.ListLicenses;
				foreach (var item in list) item.IdRequest = itemId;
				using (var dal = new License(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListLicenses = list;
			}
			if (Item.ListTravels?.Count() > 0)
			{
				var list = Item.ListTravels;
				foreach (var item in list) item.IdRequest = itemId;
				using (var dal = new Travel(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTravels = list;
			}
			if (Item.ListVacations?.Count() > 0)
			{
				var list = Item.ListVacations;
				foreach (var item in list) item.IdRequest = itemId;
				using (var dal = new Vacation(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVacations = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Request		
        /// </summary>
        /// <param name="Items">Business object of type Request para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.Request> Items)
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
        /// 	For use on data access layer at assembly level, return an  Request type object
        /// </summary>
        /// <param name="Id">Object Identifier Request</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Request</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.Request ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Request de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Request</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Request</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.Request> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.Request> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[Request] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.Request> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.HomeOffice> lstHomeOffices = null; 
			IEnumerable<BE.HumanResources.License> lstLicenses = null; 
			IEnumerable<BE.HumanResources.Travel> lstTravels = null; 
			IEnumerable<BE.HumanResources.Vacation> lstVacations = null; 
			IEnumerable<BE.Base.Classifier> lstStates = null;
			IEnumerable<BE.HumanResources.Employee> lstEmployees = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.HumanResources.relRequest.HomeOffices))
				{
					using (var dal = new HomeOffice(Connection))
					{
						lstHomeOffices = dal.List(Keys, "IdRequest", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.Licenses))
				{
					using (var dal = new License(Connection))
					{
						lstLicenses = dal.List(Keys, "IdRequest", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.Travels))
				{
					using (var dal = new Travel(Connection))
					{
						lstTravels = dal.List(Keys, "IdRequest", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.Vacations))
				{
					using (var dal = new Vacation(Connection))
					{
						lstVacations = dal.List(Keys, "IdRequest", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.State))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.IdState).Distinct();
						lstStates = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relRequest.Employee))
				{
					using(var dal = new Employee(Connection))
					{
						Keys = (from i in Items select i.IdEmployee).Distinct();
						lstEmployees = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstHomeOffices != null)
					{
						Item.ListHomeOffices = lstHomeOffices.Where(x => x.IdRequest == Item.Id)?.ToList();
					}
					if (lstLicenses != null)
					{
						Item.ListLicenses = lstLicenses.Where(x => x.IdRequest == Item.Id)?.ToList();
					}
					if (lstTravels != null)
					{
						Item.ListTravels = lstTravels.Where(x => x.IdRequest == Item.Id)?.ToList();
					}
					if (lstVacations != null)
					{
						Item.ListVacations = lstVacations.Where(x => x.IdRequest == Item.Id)?.ToList();
					}
					if (lstStates != null)
					{
						Item.State = (from i in lstStates where i.Id == Item.IdState select i).FirstOrDefault();
					}					if (lstEmployees != null)
					{
						Item.Employee = (from i in lstEmployees where i.Id == Item.IdEmployee select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEH.Request Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.HumanResources.relRequest.HomeOffices))
				{
					using (var dal = new HomeOffice(Connection))
					{
						Item.ListHomeOffices = dal.List(Keys, "IdRequest", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.Licenses))
				{
					using (var dal = new License(Connection))
					{
						Item.ListLicenses = dal.List(Keys, "IdRequest", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.Travels))
				{
					using (var dal = new Travel(Connection))
					{
						Item.ListTravels = dal.List(Keys, "IdRequest", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.Vacations))
				{
					using (var dal = new Vacation(Connection))
					{
						Item.ListVacations = dal.List(Keys, "IdRequest", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relRequest.State))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.State = dal.ReturnMaster(Item.IdState, Relations);
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relRequest.Employee))
				{
					using (var dal = new Employee(Connection))
					{
						Item.Employee = dal.ReturnMaster(Item.IdEmployee, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Request
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Request</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Request> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[Request] ORDER By " + Order;
            IEnumerable<BEH.Request> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Request
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Request> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[Request] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.Request> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Request
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Request</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Request> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[Request] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.Request> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Request    	
        /// </summary>
        /// <param name="Id">Object identifier Request</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Request</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.Request Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[Request] WHERE [Id] = @Id";
            BEH.Request Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Request() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Request(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Request(SqlConnection connection) : base(connection) { }

        #endregion

    }
}