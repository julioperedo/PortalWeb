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
    /// Class     : UserRequestDetail
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type UserRequestDetail 
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
    public partial class UserRequestDetail : DALEntity<BES.UserRequestDetail>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type UserRequestDetail
        /// </summary>
        /// <param name="Item">Business object of type UserRequestDetail </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.UserRequestDetail Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[UserRequestDetail]([IdRequest], [StateIdc], [LogUser], [LogDate]) VALUES(@IdRequest, @StateIdc, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[UserRequestDetail] SET [IdRequest] = @IdRequest, [StateIdc] = @StateIdc, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[UserRequestDetail] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  UserRequestDetail		
        /// </summary>
        /// <param name="Items">Business object of type UserRequestDetail para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.UserRequestDetail> Items)
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
        /// 	For use on data access layer at assembly level, return an  UserRequestDetail type object
        /// </summary>
        /// <param name="Id">Object Identifier UserRequestDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserRequestDetail</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.UserRequestDetail ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto UserRequestDetail de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a UserRequestDetail</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo UserRequestDetail</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.UserRequestDetail> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.UserRequestDetail> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[UserRequestDetail] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.UserRequestDetail> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Security.UserRequest> lstRequests = null;
			IEnumerable<BE.Base.Classifier> lstStates = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserRequestDetail.Request))
				{
					using(var dal = new UserRequest(Connection))
					{
						Keys = (from i in Items select i.IdRequest).Distinct();
						lstRequests = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Security.relUserRequestDetail.State))
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
					if (lstRequests != null)
					{
						Item.Request = (from i in lstRequests where i.Id == Item.IdRequest select i).FirstOrDefault();
					}					if (lstStates != null)
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
        protected override void LoadRelations(ref BES.UserRequestDetail Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserRequestDetail.Request))
				{
					using (var dal = new UserRequest(Connection))
					{
						Item.Request = dal.ReturnMaster(Item.IdRequest, Relations);
					}
				}				if (RelationEnum.Equals(BE.Security.relUserRequestDetail.State))
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
        /// 	Return an object Collection of UserRequestDetail
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserRequestDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserRequestDetail> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[UserRequestDetail] ORDER By " + Order;
            IEnumerable<BES.UserRequestDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of UserRequestDetail
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserRequestDetail> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[UserRequestDetail] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.UserRequestDetail> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of UserRequestDetail
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserRequestDetail</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserRequestDetail> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserRequestDetail] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.UserRequestDetail> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type UserRequestDetail    	
        /// </summary>
        /// <param name="Id">Object identifier UserRequestDetail</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserRequestDetail</returns>
        /// <remarks>
        /// </remarks>    
        public BES.UserRequestDetail Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserRequestDetail] WHERE [Id] = @Id";
            BES.UserRequestDetail Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public UserRequestDetail() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public UserRequestDetail(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal UserRequestDetail(SqlConnection connection) : base(connection) { }

        #endregion

    }
}