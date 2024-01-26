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
    /// Class     : Info
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Info 
    ///     for the service Campaign.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Campaign
    /// </remarks>
    /// <history>
    ///     [DMC]   27/7/2023 15:05:58 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Info : DALEntity<BEN.Info>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Info
        /// </summary>
        /// <param name="Item">Business object of type Info </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEN.Info Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Campaign].[Info]([Name], [Description], [LogUser], [LogDate]) VALUES(@Name, @Description, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Campaign].[Info] SET [Name] = @Name, [Description] = @Description, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Campaign].[Info] WHERE [Id] = @Id";
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
			if (Item.ListCategorys?.Count() > 0)
			{
				var list = Item.ListCategorys;
				foreach (var item in list) item.IdCampaign = itemId;
				using (var dal = new Category(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListCategorys = list;
			}
			if (Item.ListPrizess?.Count() > 0)
			{
				var list = Item.ListPrizess;
				foreach (var item in list) item.IdCampaign = itemId;
				using (var dal = new Prizes(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPrizess = list;
			}
			if (Item.ListUsers?.Count() > 0)
			{
				var list = Item.ListUsers;
				foreach (var item in list) item.IdCampaign = itemId;
				using (var dal = new User(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUsers = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Info		
        /// </summary>
        /// <param name="Items">Business object of type Info para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEN.Info> Items)
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
        /// 	For use on data access layer at assembly level, return an  Info type object
        /// </summary>
        /// <param name="Id">Object Identifier Info</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Info</returns>
        /// <remarks>
        /// </remarks>		
        internal BEN.Info ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Info de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Info</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Info</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEN.Info> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEN.Info> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Campaign].[Info] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEN.Info> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Campaign.Category> lstCategorys = null; 
			IEnumerable<BE.Campaign.Prizes> lstPrizess = null; 
			IEnumerable<BE.Campaign.User> lstUsers = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Campaign.relInfo.Categorys))
				{
					using (var dal = new Category(Connection))
					{
						lstCategorys = dal.List(Keys, "IdCampaign", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Campaign.relInfo.Prizess))
				{
					using (var dal = new Prizes(Connection))
					{
						lstPrizess = dal.List(Keys, "IdCampaign", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Campaign.relInfo.Users))
				{
					using (var dal = new User(Connection))
					{
						lstUsers = dal.List(Keys, "IdCampaign", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstCategorys != null)
					{
						Item.ListCategorys = lstCategorys.Where(x => x.IdCampaign == Item.Id)?.ToList();
					}
					if (lstPrizess != null)
					{
						Item.ListPrizess = lstPrizess.Where(x => x.IdCampaign == Item.Id)?.ToList();
					}
					if (lstUsers != null)
					{
						Item.ListUsers = lstUsers.Where(x => x.IdCampaign == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEN.Info Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Campaign.relInfo.Categorys))
				{
					using (var dal = new Category(Connection))
					{
						Item.ListCategorys = dal.List(Keys, "IdCampaign", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Campaign.relInfo.Prizess))
				{
					using (var dal = new Prizes(Connection))
					{
						Item.ListPrizess = dal.List(Keys, "IdCampaign", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Campaign.relInfo.Users))
				{
					using (var dal = new User(Connection))
					{
						Item.ListUsers = dal.List(Keys, "IdCampaign", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Info
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Info</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEN.Info> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Campaign].[Info] ORDER By " + Order;
            IEnumerable<BEN.Info> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Info
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEN.Info> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Campaign].[Info] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEN.Info> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Info
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Info</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEN.Info> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Campaign].[Info] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEN.Info> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Info    	
        /// </summary>
        /// <param name="Id">Object identifier Info</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Info</returns>
        /// <remarks>
        /// </remarks>    
        public BEN.Info Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Campaign].[Info] WHERE [Id] = @Id";
            BEN.Info Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Info() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Info(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Info(SqlConnection connection) : base(connection) { }

        #endregion

    }
}