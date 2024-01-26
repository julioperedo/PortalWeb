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


namespace DALayer.Security
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Security
    /// Class     : UserData
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type UserData 
    ///     for the service Security.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Security
    /// </remarks>
    /// <history>
    ///     [DMC]   17/10/2023 14:27:43 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class UserData : DALEntity<BES.UserData>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type UserData
        /// </summary>
        /// <param name="Item">Business object of type UserData </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BES.UserData Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Security].[UserData]([IdUser], [Signature], [SellerCode], [SAPUserId], [SAPUser], [SAPPassword], [IdEmployee], [LogUser], [LogDate]) VALUES(@IdUser, @Signature, @SellerCode, @SAPUserId, @SAPUser, @SAPPassword, @IdEmployee, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Security].[UserData] SET [IdUser] = @IdUser, [Signature] = @Signature, [SellerCode] = @SellerCode, [SAPUserId] = @SAPUserId, [SAPUser] = @SAPUser, [SAPPassword] = @SAPPassword, [IdEmployee] = @IdEmployee, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Security].[UserData] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  UserData		
        /// </summary>
        /// <param name="Items">Business object of type UserData para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BES.UserData> Items)
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
        /// 	For use on data access layer at assembly level, return an  UserData type object
        /// </summary>
        /// <param name="Id">Object Identifier UserData</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserData</returns>
        /// <remarks>
        /// </remarks>		
        internal BES.UserData ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto UserData de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a UserData</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo UserData</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BES.UserData> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BES.UserData> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Security].[UserData] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BES.UserData> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.Employee> lstEmployees = null;
			IEnumerable<BE.Security.User> lstUsers = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserData.Employee))
				{
					using(var dal = new HumanResources.Employee(Connection))
					{
						Keys = (from i in Items where i.IdEmployee.HasValue select i.IdEmployee.Value).Distinct();
						lstEmployees = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Security.relUserData.User))
				{
					using(var dal = new User(Connection))
					{
						Keys = (from i in Items select i.IdUser).Distinct();
						lstUsers = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstEmployees != null)
					{
						Item.Employee = (from i in lstEmployees where i.Id == Item.IdEmployee select i).FirstOrDefault();
					}					if (lstUsers != null)
					{
						Item.User = (from i in lstUsers where i.Id == Item.IdUser select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BES.UserData Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Security.relUserData.Employee))
				{
					using (var dal = new HumanResources.Employee(Connection))
					{
						if (Item.IdEmployee.HasValue)
						{
							Item.Employee = dal.ReturnMaster(Item.IdEmployee.Value, Relations);
						}
					}
				}				if (RelationEnum.Equals(BE.Security.relUserData.User))
				{
					using (var dal = new User(Connection))
					{
						Item.User = dal.ReturnMaster(Item.IdUser, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of UserData
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserData</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserData> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Security].[UserData] ORDER By " + Order;
            IEnumerable<BES.UserData> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of UserData
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserData> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Security].[UserData] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BES.UserData> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of UserData
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UserData</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BES.UserData> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserData] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BES.UserData> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type UserData    	
        /// </summary>
        /// <param name="Id">Object identifier UserData</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UserData</returns>
        /// <remarks>
        /// </remarks>    
        public BES.UserData Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Security].[UserData] WHERE [Id] = @Id";
            BES.UserData Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public UserData() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public UserData(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal UserData(SqlConnection connection) : base(connection) { }

        #endregion

    }
}