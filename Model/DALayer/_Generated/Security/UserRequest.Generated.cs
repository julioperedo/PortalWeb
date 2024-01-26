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


namespace DALayer.Security
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Security
    /// Class     : UserRequest
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type UserRequest 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:51 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class UserRequest : DALEntity<BES.UserRequest>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type UserRequest
        /// </summary>
        /// <param name="Item">Business object of type UserRequest </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.UserRequest Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[UserRequest]([FullName], [ClientName], [Position], [City], [EMail], [Phone], [CardCode], [RequestDate], [StateIdc], [Comments], [LogUser], [LogDate]) VALUES(@FullName, @ClientName, @Position, @City, @EMail, @Phone, @CardCode, @RequestDate, @StateIdc, @Comments, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[UserRequest] SET [FullName] = @FullName, [ClientName] = @ClientName, [Position] = @Position, [City] = @City, [EMail] = @EMail, [Phone] = @Phone, [CardCode] = @CardCode, [RequestDate] = @RequestDate, [StateIdc] = @StateIdc, [Comments] = @Comments, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[UserRequest] WHERE [Id] = @Id";
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
			if (Item.ListUserRequestDetails?.Count() > 0)
			{
				var list = Item.ListUserRequestDetails;
				foreach (var item in list) item.IdRequest = itemId;
				using (var dal = new UserRequestDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserRequestDetails = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  UserRequest		
        /// </summary>
        /// <param name="Items">Business object of type UserRequest para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.UserRequest> Items)
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
        /// 	For use on data access layer at assembly level, return an  UserRequest type object
        /// </summary>
        /// <param name="Id">Object Identifier UserRequest</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserRequest</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.UserRequest ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto UserRequest de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a UserRequest</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo UserRequest</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.UserRequest> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.UserRequest> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[UserRequest] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.UserRequest> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Security.UserRequestDetail> lstUserRequestDetails = null; 
			IEnumerable<BE.Base.Classifier> lstStates = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Security.relUserRequest.UserRequestDetails))
				{
					using (var dal = new UserRequestDetail(Connection))
					{
						lstUserRequestDetails = dal.List(Keys, "IdRequest", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Security.relUserRequest.State))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.StateIdc).Distinct();
						lstStates = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstUserRequestDetails != null)
					{
						Item.ListUserRequestDetails = lstUserRequestDetails.Where(x => x.IdRequest == Item.Id)?.ToList();
					}
					if (lstStates != null)
					{
						Item.State = (from i in lstStates where i.Id == Item.StateIdc select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BES.UserRequest Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Security.relUserRequest.UserRequestDetails))
				{
					using (var dal = new UserRequestDetail(Connection))
					{
						Item.ListUserRequestDetails = dal.List(Keys, "IdRequest", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Security.relUserRequest.State))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.State = dal.ReturnMaster(Item.StateIdc, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of UserRequest
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserRequest</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserRequest> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[UserRequest] ORDER By " + Order;
            IEnumerable<BES.UserRequest> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of UserRequest
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserRequest> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[UserRequest] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.UserRequest> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of UserRequest
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserRequest</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserRequest> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserRequest] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.UserRequest> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type UserRequest    	
        /// </summary>
        /// <param name="Id">Object identifier UserRequest</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserRequest</returns>
        /// <remarks>
        /// </remarks>    
        public BES.UserRequest Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserRequest] WHERE [Id] = @Id";
            BES.UserRequest Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public UserRequest() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public UserRequest(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal UserRequest(SqlConnection connection) : base(connection) { }

        #endregion

    }
}