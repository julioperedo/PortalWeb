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


namespace DALayer.Campaign
{
	/// -----------------------------------------------------------------------------
	/// Project   : DALayer
	/// NameSpace : Campaign
	/// Class     : User
	/// -----------------------------------------------------------------------------
	/// <summary>
	///     This data access component saves business object information of type User 
	///     for the service Campaign.
	/// </summary>
	/// <remarks>
	///     Data access layer for the service Campaign
	/// </remarks>
	/// <history>
	///     [DMC]   27/7/2023 15:06:01 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class User : DALEntity<BEN.User>
	{

		#region Save Methods

		/// <summary>
		/// 	Saves business information object of type User
		/// </summary>
		/// <param name="Item">Business object of type User </param>    
		/// <remarks>
		/// </remarks>
		public void Save(ref BEN.User Item)
		{
			string strQuery = "";
			if (Item.StatusType == BE.StatusType.Insert)
			{
				strQuery = "INSERT INTO [Campaign].[User]([Name], [StoreName], [EMail], [Password], [Enabled], [City], [Address], [Phone], [IdCampaign], [LogUser], [LogDate]) VALUES(@Name, @StoreName, @EMail, @Password, @Enabled, @City, @Address, @Phone, @IdCampaign, @LogUser, @LogDate) SELECT @@IDENTITY";
			}
			else if (Item.StatusType == BE.StatusType.Update)
			{
				strQuery = "UPDATE [Campaign].[User] SET [Name] = @Name, [StoreName] = @StoreName, [EMail] = @EMail, [Password] = @Password, [Enabled] = @Enabled, [City] = @City, [Address] = @Address, [Phone] = @Phone, [IdCampaign] = @IdCampaign, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
			}
			else if (Item.StatusType == BE.StatusType.Delete)
			{
				strQuery = "DELETE FROM [Campaign].[User] WHERE [Id] = @Id";
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
			if (Item.ListSerials?.Count() > 0)
			{
				var list = Item.ListSerials;
				foreach (var item in list) item.IdUser = itemId;
				using (var dal = new Serial(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSerials = list;
			}
		}

		/// <summary>
		/// 	Saves a collection business information object of type  User		
		/// </summary>
		/// <param name="Items">Business object of type User para Save</param>    
		/// <remarks>
		/// </remarks>
		public void Save(ref IList<BEN.User> Items)
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
		/// 	For use on data access layer at assembly level, return an  User type object
		/// </summary>
		/// <param name="Id">Object Identifier User</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>An object of type User</returns>
		/// <remarks>
		/// </remarks>		
		internal BEN.User ReturnMaster(long Id, params Enum[] Relations)
		{
			return Search(Id, Relations);
		}

		/// <summary>
		/// 	Devuelve un objeto User de tipo uno a uno con otro objeto
		/// </summary>
		/// <param name="Keys">Los identificadores de los objetos relacionados a User</param>
		/// <param name="Relations">Enumerador de Relations a retorar</param>
		/// <returns>Un Objeto de tipo User</returns>
		/// <remarks>
		/// </remarks>	
		internal IEnumerable<BEN.User> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
		{
			IEnumerable<BEN.User> Items = null;
			if (Keys.Count() > 0)
			{
				string strQuery = $"SELECT * FROM [Campaign].[User] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
		protected override void LoadRelations(ref IEnumerable<BEN.User> Items, params Enum[] Relations)
		{
			IEnumerable<long> Keys;
			IEnumerable<BE.Campaign.Serial> lstSerials = null; 
			IEnumerable<BE.Campaign.Info> lstCampaigns = null;

			foreach (Enum RelationEnum in Relations)
			{
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Campaign.relUser.Serials))
				{
					using (var dal = new Serial(Connection))
					{
						lstSerials = dal.List(Keys, "IdUser", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Campaign.relUser.Campaign))
				{
					using(var dal = new Info(Connection))
					{
						Keys = (from i in Items select i.IdCampaign).Distinct();
						lstCampaigns = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
			}

			if (Relations.GetLength(0) > 0)
			{
				foreach (var Item in Items)
				{
					if (lstSerials != null)
					{
						Item.ListSerials = lstSerials.Where(x => x.IdUser == Item.Id)?.ToList();
					}
					if (lstCampaigns != null)
					{
						Item.Campaign = (from i in lstCampaigns where i.Id == Item.IdCampaign select i).FirstOrDefault();
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
		protected override void LoadRelations(ref BEN.User Item, params Enum[] Relations)
		{
			foreach (Enum RelationEnum in Relations)
			{
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Campaign.relUser.Serials))
				{
					using (var dal = new Serial(Connection))
					{
						Item.ListSerials = dal.List(Keys, "IdUser", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Campaign.relUser.Campaign))
				{
					using (var dal = new Info(Connection))
					{
						Item.Campaign = dal.ReturnMaster(Item.IdCampaign, Relations);
					}
				}
			}
		}

		#endregion

		#region List Methods

		/// <summary>
		/// 	Return an object Collection of User
		/// </summary>
		/// <param name="Order">Object order property column </param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type User</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEN.User> List(string Order, params Enum[] Relations)
		{
			string strQuery = "SELECT * FROM [Campaign].[User] ORDER By " + Order;
			IEnumerable<BEN.User> Items = SQLList(strQuery, Relations);
			return Items;
		}

		/// <summary>
		/// 	Return an object Collection of User
		/// </summary>
		/// <param name="FilterList">Filter List </param>
		/// <param name="Order">Object order property column </param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type ClassifierType</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEN.User> List(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			StringBuilder sbQuery = new StringBuilder();
			string filter = GetFilterString(FilterList?.ToArray());

			sbQuery.AppendLine("SELECT   * ");
			sbQuery.AppendLine("FROM    [Campaign].[User] ");
			if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
			sbQuery.AppendLine($"ORDER By {Order}");

			IEnumerable<BEN.User> Items = SQLList(sbQuery.ToString(), Relations);
			return Items;
		}

		/// <summary>
		///     Return an object Collection of User
		/// </summary>
		/// <param name="Keys">Object Identifier</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type User</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEN.User> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Campaign].[User] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
			IEnumerable<BEN.User> Items = SQLList(strQuery, Relations);
			return Items;
		}

		#endregion

		#region Search Methods

		/// <summary>
		/// 	Search an object of type User    	
		/// </summary>
		/// <param name="Id">Object identifier User</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>An object of type User</returns>
		/// <remarks>
		/// </remarks>    
		public BEN.User Search(long Id, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Campaign].[User] WHERE [Id] = @Id";
			BEN.User Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
			return Item;
		}

		#endregion

		#region Constructors

		public User() : base() { }

		/// <summary>
		/// El constructor por defecto que crear una instancia de la base de datos 
		/// utilizando el Factory Pattern
		/// </summary>
		/// <remarks>
		///  La instancia de la Base de datos se pasa al constructor
		///	</remarks>   
		public User(string ConnectionName) : base(ConnectionName) { }

		/// <summary>
		/// Constructor que crea la instancia del la base de datos utilizando
		/// el Factory pattern
		/// </summary>
		/// <remarks></remarks>
		internal User(SqlConnection connection) : base(connection) { }

		#endregion

	}
}