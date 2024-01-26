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


namespace DALayer.Staff
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Staff
    /// Class     : Department
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Department 
    ///     for the service Staff.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Staff
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:51 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Department : DALEntity<BEF.Department>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Department
        /// </summary>
        /// <param name="Item">Business object of type Department </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEF.Department Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Staff].[Department]([Id], [Name], [Order], [LogUser], [LogDate]) VALUES(@Id, @Name, @Order, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Staff].[Department] SET [Name] = @Name, [Order] = @Order, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Staff].[Department] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Department", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListMembers?.Count() > 0)
			{
				var list = Item.ListMembers;
				foreach (var item in list) item.IdDepartment = itemId;
				using (var dal = new Member(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListMembers = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Department		
        /// </summary>
        /// <param name="Items">Business object of type Department para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEF.Department> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Department", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Department type object
        /// </summary>
        /// <param name="Id">Object Identifier Department</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Department</returns>
        /// <remarks>
        /// </remarks>		
        internal BEF.Department ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Department de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Department</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Department</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEF.Department> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEF.Department> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Staff].[Department] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEF.Department> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Staff.Member> lstMembers = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Staff.relDepartment.Members))
				{
					using (var dal = new Member(Connection))
					{
						lstMembers = dal.List(Keys, "IdDepartment", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstMembers != null)
					{
						Item.ListMembers = lstMembers.Where(x => x.IdDepartment == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEF.Department Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Staff.relDepartment.Members))
				{
					using (var dal = new Member(Connection))
					{
						Item.ListMembers = dal.List(Keys, "IdDepartment", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Department
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Department</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Department> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Staff].[Department] ORDER By " + Order;
            IEnumerable<BEF.Department> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Department
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Department> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Staff].[Department] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEF.Department> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Department
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Department</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEF.Department> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Staff].[Department] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEF.Department> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Department    	
        /// </summary>
        /// <param name="Id">Object identifier Department</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Department</returns>
        /// <remarks>
        /// </remarks>    
        public BEF.Department Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Staff].[Department] WHERE [Id] = @Id";
            BEF.Department Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Department() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Department(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Department(SqlConnection connection) : base(connection) { }

        #endregion

    }
}