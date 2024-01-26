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


namespace DALayer.Online
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Online
    /// Class     : SaleFiles
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type SaleFiles 
    ///     for the service Online.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Online
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:40 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class SaleFiles : DALEntity<BEO.SaleFiles>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type SaleFiles
        /// </summary>
        /// <param name="Item">Business object of type SaleFiles </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEO.SaleFiles Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Online].[SaleFiles]([Id], [IdSale], [Type], [Name], [LogUser], [LogDate]) VALUES(@Id, @IdSale, @Type, @Name, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Online].[SaleFiles] SET [IdSale] = @IdSale, [Type] = @Type, [Name] = @Name, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Online].[SaleFiles] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("SaleFiles", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  SaleFiles		
        /// </summary>
        /// <param name="Items">Business object of type SaleFiles para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEO.SaleFiles> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("SaleFiles", quantity);
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
        /// 	For use on data access layer at assembly level, return an  SaleFiles type object
        /// </summary>
        /// <param name="Id">Object Identifier SaleFiles</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SaleFiles</returns>
        /// <remarks>
        /// </remarks>		
        internal BEO.SaleFiles ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto SaleFiles de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a SaleFiles</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo SaleFiles</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEO.SaleFiles> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEO.SaleFiles> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Online].[SaleFiles] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEO.SaleFiles> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Online.Sale> lstSales = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relSaleFiles.Sale))
				{
					using(var dal = new Sale(Connection))
					{
						Keys = (from i in Items select i.IdSale).Distinct();
						lstSales = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstSales != null)
					{
						Item.Sale = (from i in lstSales where i.Id == Item.IdSale select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEO.SaleFiles Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Online.relSaleFiles.Sale))
				{
					using (var dal = new Sale(Connection))
					{
						Item.Sale = dal.ReturnMaster(Item.IdSale, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of SaleFiles
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SaleFiles</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleFiles> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Online].[SaleFiles] ORDER By " + Order;
            IEnumerable<BEO.SaleFiles> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of SaleFiles
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleFiles> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Online].[SaleFiles] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEO.SaleFiles> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of SaleFiles
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type SaleFiles</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEO.SaleFiles> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[SaleFiles] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEO.SaleFiles> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type SaleFiles    	
        /// </summary>
        /// <param name="Id">Object identifier SaleFiles</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type SaleFiles</returns>
        /// <remarks>
        /// </remarks>    
        public BEO.SaleFiles Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Online].[SaleFiles] WHERE [Id] = @Id";
            BEO.SaleFiles Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public SaleFiles() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public SaleFiles(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal SaleFiles(SqlConnection connection) : base(connection) { }

        #endregion

    }
}