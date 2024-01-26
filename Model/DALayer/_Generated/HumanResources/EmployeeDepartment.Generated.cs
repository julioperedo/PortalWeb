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
    /// Class     : EmployeeDepartment
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type EmployeeDepartment 
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
    public partial class EmployeeDepartment : DALEntity<BEH.EmployeeDepartment>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type EmployeeDepartment
        /// </summary>
        /// <param name="Item">Business object of type EmployeeDepartment </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.EmployeeDepartment Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[EmployeeDepartment]([IdEmployee], [IdDepartment], [IdManager], [LogUser], [LogDate]) VALUES(@IdEmployee, @IdDepartment, @IdManager, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[EmployeeDepartment] SET [IdEmployee] = @IdEmployee, [IdDepartment] = @IdDepartment, [IdManager] = @IdManager, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[EmployeeDepartment] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  EmployeeDepartment		
        /// </summary>
        /// <param name="Items">Business object of type EmployeeDepartment para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.EmployeeDepartment> Items)
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
        /// 	For use on data access layer at assembly level, return an  EmployeeDepartment type object
        /// </summary>
        /// <param name="Id">Object Identifier EmployeeDepartment</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type EmployeeDepartment</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.EmployeeDepartment ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto EmployeeDepartment de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a EmployeeDepartment</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo EmployeeDepartment</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.EmployeeDepartment> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.EmployeeDepartment> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[EmployeeDepartment] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.EmployeeDepartment> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.Employee> lstManagers = null;
			IEnumerable<BE.HumanResources.Department> lstDepartments = null;
			IEnumerable<BE.HumanResources.Employee> lstEmployees = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relEmployeeDepartment.Manager))
				{
					using(var dal = new Employee(Connection))
					{
						Keys = (from i in Items select i.IdManager).Distinct();
						lstManagers = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relEmployeeDepartment.Department))
				{
					using(var dal = new Department(Connection))
					{
						Keys = (from i in Items select i.IdDepartment).Distinct();
						lstDepartments = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relEmployeeDepartment.Employee))
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
					if (lstManagers != null)
					{
						Item.Manager = (from i in lstManagers where i.Id == Item.IdManager select i).FirstOrDefault();
					}					if (lstDepartments != null)
					{
						Item.Department = (from i in lstDepartments where i.Id == Item.IdDepartment select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEH.EmployeeDepartment Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relEmployeeDepartment.Manager))
				{
					using (var dal = new Employee(Connection))
					{
						Item.Manager = dal.ReturnMaster(Item.IdManager, Relations);
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relEmployeeDepartment.Department))
				{
					using (var dal = new Department(Connection))
					{
						Item.Department = dal.ReturnMaster(Item.IdDepartment, Relations);
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relEmployeeDepartment.Employee))
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
        /// 	Return an object Collection of EmployeeDepartment
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type EmployeeDepartment</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.EmployeeDepartment> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[EmployeeDepartment] ORDER By " + Order;
            IEnumerable<BEH.EmployeeDepartment> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of EmployeeDepartment
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.EmployeeDepartment> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[EmployeeDepartment] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.EmployeeDepartment> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of EmployeeDepartment
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type EmployeeDepartment</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.EmployeeDepartment> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[EmployeeDepartment] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.EmployeeDepartment> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type EmployeeDepartment    	
        /// </summary>
        /// <param name="Id">Object identifier EmployeeDepartment</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type EmployeeDepartment</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.EmployeeDepartment Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[EmployeeDepartment] WHERE [Id] = @Id";
            BEH.EmployeeDepartment Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public EmployeeDepartment() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public EmployeeDepartment(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal EmployeeDepartment(SqlConnection connection) : base(connection) { }

        #endregion

    }
}