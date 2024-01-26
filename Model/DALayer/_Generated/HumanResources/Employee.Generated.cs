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
	/// Class     : Employee
	/// -----------------------------------------------------------------------------
	/// <summary>
	///     This data access component saves business object information of type Employee 
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
	public partial class Employee : DALEntity<BEH.Employee>
	{

		#region Save Methods

		/// <summary>
		/// 	Saves business information object of type Employee
		/// </summary>
		/// <param name="Item">Business object of type Employee </param>    
		/// <remarks>
		/// </remarks>
		public void Save(ref BEH.Employee Item)
		{
			string strQuery = "";
			if (Item.StatusType == BE.StatusType.Insert)
			{
				strQuery = "INSERT INTO [HumanResources].[Employee]([Id], [Name], [ShortName], [Position], [HierarchyLevel], [Mail], [Photo], [Phone], [Enabled], [LogUser], [LogDate]) VALUES(@Id, @Name, @ShortName, @Position, @HierarchyLevel, @Mail, @Photo, @Phone, @Enabled, @LogUser, @LogDate)";
			}
			else if (Item.StatusType == BE.StatusType.Update)
			{
				strQuery = "UPDATE [HumanResources].[Employee] SET [Name] = @Name, [ShortName] = @ShortName, [Position] = @Position, [HierarchyLevel] = @HierarchyLevel, [Mail] = @Mail, [Photo] = @Photo, [Phone] = @Phone, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
			}
			else if (Item.StatusType == BE.StatusType.Delete)
			{
				strQuery = "DELETE FROM [HumanResources].[Employee] WHERE [Id] = @Id";
			}

			if (Item.StatusType != BE.StatusType.NoAction)
			{
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Employee", 1);
				Connection.Execute(strQuery, Item);
				Item.StatusType = BE.StatusType.NoAction;
			}
			long itemId = Item.Id;
			if (Item.ListEmployeeDepartment_Managers?.Count() > 0)
			{
				var list = Item.ListEmployeeDepartment_Managers;
				foreach (var item in list) item.IdManager = itemId;
				using (var dal = new EmployeeDepartment(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListEmployeeDepartment_Managers = list;
			}
			if (Item.ListEmployeeDepartment_Employees?.Count() > 0)
			{
				var list = Item.ListEmployeeDepartment_Employees;
				foreach (var item in list) item.IdEmployee = itemId;
				using (var dal = new EmployeeDepartment(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListEmployeeDepartment_Employees = list;
			}
			if (Item.ListRequests?.Count() > 0)
			{
				var list = Item.ListRequests;
				foreach (var item in list) item.IdEmployee = itemId;
				using (var dal = new Request(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListRequests = list;
			}
			if (Item.ListTravelReplacements?.Count() > 0)
			{
				var list = Item.ListTravelReplacements;
				foreach (var item in list) item.IdReplacement = itemId;
				using (var dal = new TravelReplacement(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTravelReplacements = list;
			}
			if (Item.ListVacationReplacements?.Count() > 0)
			{
				var list = Item.ListVacationReplacements;
				foreach (var item in list) item.IdReplacement = itemId;
				using (var dal = new VacationReplacement(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVacationReplacements = list;
			}
			if (Item.ListUserDatas?.Count() > 0)
			{
				var list = Item.ListUserDatas;
				foreach (var item in list) item.IdEmployee = itemId;
				using (var dal = new DALayer.Security.UserData(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserDatas = list;
			}
		}

		/// <summary>
		/// 	Saves a collection business information object of type  Employee		
		/// </summary>
		/// <param name="Items">Business object of type Employee para Save</param>    
		/// <remarks>
		/// </remarks>
		public void Save(ref IList<BEH.Employee> Items)
		{
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0);
			if (quantity > 0)
			{
				lastId = GenID("Employee", quantity);
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
		/// 	For use on data access layer at assembly level, return an  Employee type object
		/// </summary>
		/// <param name="Id">Object Identifier Employee</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>An object of type Employee</returns>
		/// <remarks>
		/// </remarks>		
		internal BEH.Employee ReturnMaster(long Id, params Enum[] Relations)
		{
			return Search(Id, Relations);
		}

		/// <summary>
		/// 	Devuelve un objeto Employee de tipo uno a uno con otro objeto
		/// </summary>
		/// <param name="Keys">Los identificadores de los objetos relacionados a Employee</param>
		/// <param name="Relations">Enumerador de Relations a retorar</param>
		/// <returns>Un Objeto de tipo Employee</returns>
		/// <remarks>
		/// </remarks>	
		internal IEnumerable<BEH.Employee> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
		{
			IEnumerable<BEH.Employee> Items = null;
			if (Keys.Count() > 0)
			{
				string strQuery = $"SELECT * FROM [HumanResources].[Employee] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
		protected override void LoadRelations(ref IEnumerable<BEH.Employee> Items, params Enum[] Relations)
		{
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.EmployeeDepartment> lstEmployeeDepartment_Managers = null;
			IEnumerable<BE.HumanResources.EmployeeDepartment> lstEmployeeDepartment_Employees = null;
			IEnumerable<BE.HumanResources.Request> lstRequests = null;
			IEnumerable<BE.HumanResources.TravelReplacement> lstTravelReplacements = null;
			IEnumerable<BE.HumanResources.VacationReplacement> lstVacationReplacements = null;
			IEnumerable<BE.Security.UserData> lstUserDatas = null;

			foreach (Enum RelationEnum in Relations)
			{
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.EmployeeDepartment_Managers))
				{
					using (var dal = new EmployeeDepartment(Connection))
					{
						lstEmployeeDepartment_Managers = dal.List(Keys, "IdManager", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.EmployeeDepartment_Employees))
				{
					using (var dal = new EmployeeDepartment(Connection))
					{
						lstEmployeeDepartment_Employees = dal.List(Keys, "IdEmployee", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.Requests))
				{
					using (var dal = new Request(Connection))
					{
						lstRequests = dal.List(Keys, "IdEmployee", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.TravelReplacements))
				{
					using (var dal = new TravelReplacement(Connection))
					{
						lstTravelReplacements = dal.List(Keys, "IdReplacement", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.VacationReplacements))
				{
					using (var dal = new VacationReplacement(Connection))
					{
						lstVacationReplacements = dal.List(Keys, "IdReplacement", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.UserDatas))
				{
					using (var dal = new DALayer.Security.UserData(Connection))
					{
						lstUserDatas = dal.List(Keys, "IdEmployee", Relations);
					}
				}
			}

			if (Relations.GetLength(0) > 0)
			{
				foreach (var Item in Items)
				{
					if (lstEmployeeDepartment_Managers != null)
					{
						Item.ListEmployeeDepartment_Managers = lstEmployeeDepartment_Managers.Where(x => x.IdManager == Item.Id)?.ToList();
					}
					if (lstEmployeeDepartment_Employees != null)
					{
						Item.ListEmployeeDepartment_Employees = lstEmployeeDepartment_Employees.Where(x => x.IdEmployee == Item.Id)?.ToList();
					}
					if (lstRequests != null)
					{
						Item.ListRequests = lstRequests.Where(x => x.IdEmployee == Item.Id)?.ToList();
					}
					if (lstTravelReplacements != null)
					{
						Item.ListTravelReplacements = lstTravelReplacements.Where(x => x.IdReplacement == Item.Id)?.ToList();
					}
					if (lstVacationReplacements != null)
					{
						Item.ListVacationReplacements = lstVacationReplacements.Where(x => x.IdReplacement == Item.Id)?.ToList();
					}
					if (lstUserDatas != null)
					{
						Item.ListUserDatas = lstUserDatas.Where(x => x.IdEmployee == Item.Id)?.ToList();
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
		protected override void LoadRelations(ref BEH.Employee Item, params Enum[] Relations)
		{
			foreach (Enum RelationEnum in Relations)
			{
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.EmployeeDepartment_Managers))
				{
					using (var dal = new EmployeeDepartment(Connection))
					{
						Item.ListEmployeeDepartment_Managers = dal.List(Keys, "IdManager", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.EmployeeDepartment_Employees))
				{
					using (var dal = new EmployeeDepartment(Connection))
					{
						Item.ListEmployeeDepartment_Employees = dal.List(Keys, "IdEmployee", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.Requests))
				{
					using (var dal = new Request(Connection))
					{
						Item.ListRequests = dal.List(Keys, "IdEmployee", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.TravelReplacements))
				{
					using (var dal = new TravelReplacement(Connection))
					{
						Item.ListTravelReplacements = dal.List(Keys, "IdReplacement", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.VacationReplacements))
				{
					using (var dal = new VacationReplacement(Connection))
					{
						Item.ListVacationReplacements = dal.List(Keys, "IdReplacement", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relEmployee.UserDatas))
				{
					using (var dal = new DALayer.Security.UserData(Connection))
					{
						Item.ListUserDatas = dal.List(Keys, "IdEmployee", Relations)?.ToList();
					}
				}
			}
		}

		#endregion

		#region List Methods

		/// <summary>
		/// 	Return an object Collection of Employee
		/// </summary>
		/// <param name="Order">Object order property column </param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type Employee</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEH.Employee> List(string Order, params Enum[] Relations)
		{
			string strQuery = "SELECT * FROM [HumanResources].[Employee] ORDER By " + Order;
			IEnumerable<BEH.Employee> Items = SQLList(strQuery, Relations);
			return Items;
		}

		/// <summary>
		/// 	Return an object Collection of Employee
		/// </summary>
		/// <param name="FilterList">Filter List </param>
		/// <param name="Order">Object order property column </param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type ClassifierType</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEH.Employee> List(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			StringBuilder sbQuery = new StringBuilder();
			string filter = GetFilterString(FilterList?.ToArray());

			sbQuery.AppendLine("SELECT   * ");
			sbQuery.AppendLine("FROM    [HumanResources].[Employee] ");
			if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
			sbQuery.AppendLine($"ORDER By {Order}");

			IEnumerable<BEH.Employee> Items = SQLList(sbQuery.ToString(), Relations);
			return Items;
		}

		/// <summary>
		///     Return an object Collection of Employee
		/// </summary>
		/// <param name="Keys">Object Identifier</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type Employee</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEH.Employee> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [HumanResources].[Employee] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
			IEnumerable<BEH.Employee> Items = SQLList(strQuery, Relations);
			return Items;
		}

		#endregion

		#region Search Methods

		/// <summary>
		/// 	Search an object of type Employee    	
		/// </summary>
		/// <param name="Id">Object identifier Employee</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>An object of type Employee</returns>
		/// <remarks>
		/// </remarks>    
		public BEH.Employee Search(long Id, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [HumanResources].[Employee] WHERE [Id] = @Id";
			BEH.Employee Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
			return Item;
		}

		#endregion

		#region Constructors

		public Employee() : base() { }

		/// <summary>
		/// El constructor por defecto que crear una instancia de la base de datos 
		/// utilizando el Factory Pattern
		/// </summary>
		/// <remarks>
		///  La instancia de la Base de datos se pasa al constructor
		///	</remarks>   
		public Employee(string ConnectionName) : base(ConnectionName) { }

		/// <summary>
		/// Constructor que crea la instancia del la base de datos utilizando
		/// el Factory pattern
		/// </summary>
		/// <remarks></remarks>
		internal Employee(SqlConnection connection) : base(connection) { }

		#endregion

	}
}