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
    /// Class     : PriceGroupLine
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type PriceGroupLine 
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
    public partial class PriceGroupLine : DALEntity<BEP.PriceGroupLine>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type PriceGroupLine
        /// </summary>
        /// <param name="Item">Business object of type PriceGroupLine </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.PriceGroupLine Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[PriceGroupLine]([IdLine], [IdGroup], [Percentage], [LogUser], [LogDate]) VALUES(@IdLine, @IdGroup, @Percentage, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[PriceGroupLine] SET [IdLine] = @IdLine, [IdGroup] = @IdGroup, [Percentage] = @Percentage, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[PriceGroupLine] WHERE [Id] = @Id";
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
			if (Item.ListPriceGroupLineClients?.Count() > 0)
			{
				var list = Item.ListPriceGroupLineClients;
				foreach (var item in list) item.IdGroupLine = itemId;
				using (var dal = new PriceGroupLineClient(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPriceGroupLineClients = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  PriceGroupLine		
        /// </summary>
        /// <param name="Items">Business object of type PriceGroupLine para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.PriceGroupLine> Items)
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
        /// 	For use on data access layer at assembly level, return an  PriceGroupLine type object
        /// </summary>
        /// <param name="Id">Object Identifier PriceGroupLine</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PriceGroupLine</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.PriceGroupLine ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto PriceGroupLine de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a PriceGroupLine</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo PriceGroupLine</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.PriceGroupLine> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.PriceGroupLine> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[PriceGroupLine] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.PriceGroupLine> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.PriceGroupLineClient> lstPriceGroupLineClients = null; 
			IEnumerable<BE.Product.PriceGroup> lstGroups = null;
			IEnumerable<BE.Product.Line> lstLines = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Product.relPriceGroupLine.PriceGroupLineClients))
				{
					using (var dal = new PriceGroupLineClient(Connection))
					{
						lstPriceGroupLineClients = dal.List(Keys, "IdGroupLine", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relPriceGroupLine.Group))
				{
					using(var dal = new PriceGroup(Connection))
					{
						Keys = (from i in Items select i.IdGroup).Distinct();
						lstGroups = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Product.relPriceGroupLine.Line))
				{
					using(var dal = new Line(Connection))
					{
						Keys = (from i in Items select i.IdLine).Distinct();
						lstLines = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstPriceGroupLineClients != null)
					{
						Item.ListPriceGroupLineClients = lstPriceGroupLineClients.Where(x => x.IdGroupLine == Item.Id)?.ToList();
					}
					if (lstGroups != null)
					{
						Item.Group = (from i in lstGroups where i.Id == Item.IdGroup select i).FirstOrDefault();
					}					if (lstLines != null)
					{
						Item.Line = (from i in lstLines where i.Id == Item.IdLine select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.PriceGroupLine Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Product.relPriceGroupLine.PriceGroupLineClients))
				{
					using (var dal = new PriceGroupLineClient(Connection))
					{
						Item.ListPriceGroupLineClients = dal.List(Keys, "IdGroupLine", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relPriceGroupLine.Group))
				{
					using (var dal = new PriceGroup(Connection))
					{
						Item.Group = dal.ReturnMaster(Item.IdGroup, Relations);
					}
				}				if (RelationEnum.Equals(BE.Product.relPriceGroupLine.Line))
				{
					using (var dal = new Line(Connection))
					{
						Item.Line = dal.ReturnMaster(Item.IdLine, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of PriceGroupLine
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PriceGroupLine</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroupLine> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[PriceGroupLine] ORDER By " + Order;
            IEnumerable<BEP.PriceGroupLine> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of PriceGroupLine
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroupLine> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[PriceGroupLine] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.PriceGroupLine> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of PriceGroupLine
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type PriceGroupLine</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.PriceGroupLine> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PriceGroupLine] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.PriceGroupLine> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type PriceGroupLine    	
        /// </summary>
        /// <param name="Id">Object identifier PriceGroupLine</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type PriceGroupLine</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.PriceGroupLine Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[PriceGroupLine] WHERE [Id] = @Id";
            BEP.PriceGroupLine Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public PriceGroupLine() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public PriceGroupLine(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal PriceGroupLine(SqlConnection connection) : base(connection) { }

        #endregion

    }
}