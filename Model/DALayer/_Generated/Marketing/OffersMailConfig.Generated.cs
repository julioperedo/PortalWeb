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


namespace DALayer.Marketing
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Marketing
    /// Class     : OffersMailConfig
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type OffersMailConfig 
    ///     for the service Marketing.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Marketing
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:39 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class OffersMailConfig : DALEntity<BEM.OffersMailConfig>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type OffersMailConfig
        /// </summary>
        /// <param name="Item">Business object of type OffersMailConfig </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEM.OffersMailConfig Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Marketing].[OffersMailConfig]([IdLine], [WeekDay], [LogUser], [LogDate]) VALUES(@IdLine, @WeekDay, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Marketing].[OffersMailConfig] SET [IdLine] = @IdLine, [WeekDay] = @WeekDay, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Marketing].[OffersMailConfig] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  OffersMailConfig		
        /// </summary>
        /// <param name="Items">Business object of type OffersMailConfig para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEM.OffersMailConfig> Items)
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
        /// 	For use on data access layer at assembly level, return an  OffersMailConfig type object
        /// </summary>
        /// <param name="Id">Object Identifier OffersMailConfig</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type OffersMailConfig</returns>
        /// <remarks>
        /// </remarks>		
        internal BEM.OffersMailConfig ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto OffersMailConfig de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a OffersMailConfig</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo OffersMailConfig</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEM.OffersMailConfig> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEM.OffersMailConfig> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Marketing].[OffersMailConfig] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEM.OffersMailConfig> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.Line> lstLines = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Marketing.relOffersMailConfig.Line))
				{
					using(var dal = new Product.Line(Connection))
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
					if (lstLines != null)
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
        protected override void LoadRelations(ref BEM.OffersMailConfig Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Marketing.relOffersMailConfig.Line))
				{
					using (var dal = new Product.Line(Connection))
					{
						Item.Line = dal.ReturnMaster(Item.IdLine, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of OffersMailConfig
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type OffersMailConfig</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEM.OffersMailConfig> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Marketing].[OffersMailConfig] ORDER By " + Order;
            IEnumerable<BEM.OffersMailConfig> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of OffersMailConfig
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEM.OffersMailConfig> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Marketing].[OffersMailConfig] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEM.OffersMailConfig> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of OffersMailConfig
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type OffersMailConfig</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEM.OffersMailConfig> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Marketing].[OffersMailConfig] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEM.OffersMailConfig> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type OffersMailConfig    	
        /// </summary>
        /// <param name="Id">Object identifier OffersMailConfig</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type OffersMailConfig</returns>
        /// <remarks>
        /// </remarks>    
        public BEM.OffersMailConfig Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Marketing].[OffersMailConfig] WHERE [Id] = @Id";
            BEM.OffersMailConfig Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public OffersMailConfig() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public OffersMailConfig(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal OffersMailConfig(SqlConnection connection) : base(connection) { }

        #endregion

    }
}