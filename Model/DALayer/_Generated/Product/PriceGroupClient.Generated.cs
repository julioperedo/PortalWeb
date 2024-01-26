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


namespace DALayer.Product
{
	/// -----------------------------------------------------------------------------
	/// Project   : DALayer
	/// NameSpace : Product
	/// Class     : PriceGroupClient
	/// -----------------------------------------------------------------------------
	/// <summary>
	///     This data access component saves business object information of type PriceGroupClient 
	///     for the service Product.
	/// </summary>
	/// <remarks>
	///     Data access layer for the service Product
	/// </remarks>
	/// <history>
	///     [DMC]   2/8/2023 11:58:00 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PriceGroupClient : DALEntity<BEP.PriceGroupClient>
	{

		#region Save Methods

		/// <summary>
		/// 	Saves business information object of type PriceGroupClient
		/// </summary>
		/// <param name="Item">Business object of type PriceGroupClient </param>    
		/// <remarks>
		/// </remarks>
		public void Save(ref BEP.PriceGroupClient Item)
		{
			string strQuery = "";
			if (Item.StatusType == BE.StatusType.Insert)
			{
				strQuery = "INSERT INTO [Product].[PriceGroupClient]([CardCode], [IdGroup], [LogUser], [LogDate]) VALUES(@CardCode, @IdGroup, @LogUser, @LogDate)";
			}
			else if (Item.StatusType == BE.StatusType.Update)
			{
				strQuery = "UPDATE [Product].[PriceGroupClient] SET [IdGroup] = @IdGroup, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [CardCode] = @CardCode";
			}
			else if (Item.StatusType == BE.StatusType.Delete)
			{
				strQuery = "DELETE FROM [Product].[PriceGroupClient] WHERE [CardCode] = @CardCode";
			}

			if (Item.StatusType != BE.StatusType.NoAction)
			{
				Connection.Execute(strQuery, Item);
				Item.StatusType = BE.StatusType.NoAction;
			}
		}

		/// <summary>
		/// 	Saves a collection business information object of type  PriceGroupClient		
		/// </summary>
		/// <param name="Items">Business object of type PriceGroupClient para Save</param>    
		/// <remarks>
		/// </remarks>
		public void Save(ref IList<BEP.PriceGroupClient> Items)
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
		/// 	For use on data access layer at assembly level, return an  PriceGroupClient type object
		/// </summary>
		/// <param name="Id">Object Identifier PriceGroupClient</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>An object of type PriceGroupClient</returns>
		/// <remarks>
		/// </remarks>		
		internal BEP.PriceGroupClient ReturnMaster(string CardCode, params Enum[] Relations)
		{
			return Search(CardCode, Relations);
		}

		/// <summary>
		/// 	Devuelve un objeto PriceGroupClient de tipo uno a uno con otro objeto
		/// </summary>
		/// <param name="Keys">Los identificadores de los objetos relacionados a PriceGroupClient</param>
		/// <param name="Relations">Enumerador de Relations a retorar</param>
		/// <returns>Un Objeto de tipo PriceGroupClient</returns>
		/// <remarks>
		/// </remarks>	
		internal IEnumerable<BEP.PriceGroupClient> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
		{
			IEnumerable<BEP.PriceGroupClient> Items = null;
			if (Keys.Count() > 0)
			{
				string strQuery = $"SELECT * FROM [Product].[PriceGroupClient] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
		protected override void LoadRelations(ref IEnumerable<BEP.PriceGroupClient> Items, params Enum[] Relations)
		{
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.PriceGroup> lstGroups = null;

			foreach (Enum RelationEnum in Relations)
			{
				if (RelationEnum.Equals(BE.Product.relPriceGroupClient.Group))
				{
					using (var dal = new PriceGroup(Connection))
					{
						Keys = (from i in Items select i.IdGroup).Distinct();
						lstGroups = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
			}

			if (Relations.GetLength(0) > 0)
			{
				foreach (var Item in Items)
				{
					if (lstGroups != null)
					{
						Item.Group = (from i in lstGroups where i.Id == Item.IdGroup select i).FirstOrDefault();
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
		protected override void LoadRelations(ref BEP.PriceGroupClient Item, params Enum[] Relations)
		{
			foreach (Enum RelationEnum in Relations)
			{
				if (RelationEnum.Equals(BE.Product.relPriceGroupClient.Group))
				{
					using (var dal = new PriceGroup(Connection))
					{
						Item.Group = dal.ReturnMaster(Item.IdGroup, Relations);
					}
				}
			}
		}

		#endregion

		#region List Methods

		/// <summary>
		/// 	Return an object Collection of PriceGroupClient
		/// </summary>
		/// <param name="Order">Object order property column </param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type PriceGroupClient</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEP.PriceGroupClient> List(string Order, params Enum[] Relations)
		{
			string strQuery = "SELECT * FROM [Product].[PriceGroupClient] ORDER By " + Order;
			IEnumerable<BEP.PriceGroupClient> Items = SQLList(strQuery, Relations);
			return Items;
		}

		/// <summary>
		/// 	Return an object Collection of PriceGroupClient
		/// </summary>
		/// <param name="FilterList">Filter List </param>
		/// <param name="Order">Object order property column </param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type ClassifierType</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEP.PriceGroupClient> List(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			StringBuilder sbQuery = new StringBuilder();
			string filter = GetFilterString(FilterList?.ToArray());

			sbQuery.AppendLine("SELECT   * ");
			sbQuery.AppendLine("FROM    [Product].[PriceGroupClient] ");
			if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
			sbQuery.AppendLine($"ORDER By {Order}");

			IEnumerable<BEP.PriceGroupClient> Items = SQLList(sbQuery.ToString(), Relations);
			return Items;
		}

		/// <summary>
		///     Return an object Collection of PriceGroupClient
		/// </summary>
		/// <param name="Keys">Object Identifier</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>A Collection of type PriceGroupClient</returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<BEP.PriceGroupClient> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Product].[PriceGroupClient] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
			IEnumerable<BEP.PriceGroupClient> Items = SQLList(strQuery, Relations);
			return Items;
		}

		#endregion

		#region Search Methods

		/// <summary>
		/// 	Search an object of type PriceGroupClient    	
		/// </summary>
		/// <param name="Id">Object identifier PriceGroupClient</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>An object of type PriceGroupClient</returns>
		/// <remarks>
		/// </remarks>    
		public BEP.PriceGroupClient Search(string CardCode, params Enum[] Relations)
		{
			string strQuery = $"SELECT * FROM [Product].[PriceGroupClient] WHERE [CardCode] = @CardCode";
			BEP.PriceGroupClient Item = SQLSearch(strQuery, new { @CardCode = CardCode }, Relations);
			return Item;
		}

		#endregion

		#region Constructors

		public PriceGroupClient() : base() { }

		/// <summary>
		/// El constructor por defecto que crear una instancia de la base de datos 
		/// utilizando el Factory Pattern
		/// </summary>
		/// <remarks>
		///  La instancia de la Base de datos se pasa al constructor
		///	</remarks>   
		public PriceGroupClient(string ConnectionName) : base(ConnectionName) { }

		/// <summary>
		/// Constructor que crea la instancia del la base de datos utilizando
		/// el Factory pattern
		/// </summary>
		/// <remarks></remarks>
		internal PriceGroupClient(SqlConnection connection) : base(connection) { }

		#endregion

	}
}