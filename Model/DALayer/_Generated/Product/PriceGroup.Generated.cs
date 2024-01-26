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
    /// Class     : PriceGroup
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type PriceGroup 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   10/8/2023 10:15:34 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class PriceGroup : DALEntity<BEP.PriceGroup>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type PriceGroup
        /// </summary>
        /// <param name="Item">Business object of type PriceGroup </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.PriceGroup Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[PriceGroup]([Name], [Description], [Percentage], [LogUser], [LogDate]) VALUES(@Name, @Description, @Percentage, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[PriceGroup] SET [Name] = @Name, [Description] = @Description, [Percentage] = @Percentage, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[PriceGroup] WHERE [Id] = @Id";
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
			if (Item.ListPriceGroupClients?.Count() > 0)
			{
				var list = Item.ListPriceGroupClients;
				foreach (var item in list) item.IdGroup = itemId;
				using (var dal = new PriceGroupClient(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPriceGroupClients = list;
			}
			if (Item.ListPriceGroupLines?.Count() > 0)
			{
				var list = Item.ListPriceGroupLines;
				foreach (var item in list) item.IdGroup = itemId;
				using (var dal = new PriceGroupLine(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPriceGroupLines = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  PriceGroup		
        /// </summary>
        /// <param name="Items">Business object of type PriceGroup para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.PriceGroup> Items)
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
        /// 	For use on data access layer at assembly level, return an  PriceGroup type object
        /// </summary>
        /// <param name="Id">Object Identifier PriceGroup</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PriceGroup</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.PriceGroup ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto PriceGroup de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a PriceGroup</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo PriceGroup</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.PriceGroup> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.PriceGroup> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[PriceGroup] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.PriceGroup> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.PriceGroupClient> lstPriceGroupClients = null; 
			IEnumerable<BE.Product.PriceGroupLine> lstPriceGroupLines = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Product.relPriceGroup.PriceGroupClients))
				{
					using (var dal = new PriceGroupClient(Connection))
					{
						lstPriceGroupClients = dal.List(Keys, "IdGroup", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relPriceGroup.PriceGroupLines))
				{
					using (var dal = new PriceGroupLine(Connection))
					{
						lstPriceGroupLines = dal.List(Keys, "IdGroup", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstPriceGroupClients != null)
					{
						Item.ListPriceGroupClients = lstPriceGroupClients.Where(x => x.IdGroup == Item.Id)?.ToList();
					}
					if (lstPriceGroupLines != null)
					{
						Item.ListPriceGroupLines = lstPriceGroupLines.Where(x => x.IdGroup == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEP.PriceGroup Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Product.relPriceGroup.PriceGroupClients))
				{
					using (var dal = new PriceGroupClient(Connection))
					{
						Item.ListPriceGroupClients = dal.List(Keys, "IdGroup", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relPriceGroup.PriceGroupLines))
				{
					using (var dal = new PriceGroupLine(Connection))
					{
						Item.ListPriceGroupLines = dal.List(Keys, "IdGroup", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of PriceGroup
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PriceGroup</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroup> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[PriceGroup] ORDER By " + Order;
            IEnumerable<BEP.PriceGroup> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of PriceGroup
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroup> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[PriceGroup] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.PriceGroup> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of PriceGroup
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PriceGroup</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroup> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PriceGroup] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.PriceGroup> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type PriceGroup    	
        /// </summary>
        /// <param name="UserId">Object identifier PriceGroup</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PriceGroup</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.PriceGroup Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PriceGroup] WHERE [Id] = @Id";
            BEP.PriceGroup Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public PriceGroup() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public PriceGroup(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal PriceGroup(SqlConnection connection) : base(connection) { }

        #endregion

    }
}