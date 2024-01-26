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


namespace DALayer.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Product
    /// Class     : OpenBoxHistory
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type OpenBoxHistory 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:44 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class OpenBoxHistory : DALEntity<BEP.OpenBoxHistory>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type OpenBoxHistory
        /// </summary>
        /// <param name="Item">Business object of type OpenBoxHistory </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.OpenBoxHistory Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[OpenBoxHistory]([IdProduct], [IdSubsidiary], [Comments], [Quantity], [Price], [Enabled], [LogUser], [LogDate], [LogAction]) VALUES(@IdProduct, @IdSubsidiary, @Comments, @Quantity, @Price, @Enabled, @LogUser, @LogDate, @LogAction) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[OpenBoxHistory] SET [IdProduct] = @IdProduct, [IdSubsidiary] = @IdSubsidiary, [Comments] = @Comments, [Quantity] = @Quantity, [Price] = @Price, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate, [LogAction] = @LogAction WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[OpenBoxHistory] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  OpenBoxHistory		
        /// </summary>
        /// <param name="Items">Business object of type OpenBoxHistory para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.OpenBoxHistory> Items)
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
        /// 	For use on data access layer at assembly level, return an  OpenBoxHistory type object
        /// </summary>
        /// <param name="Id">Object Identifier OpenBoxHistory</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type OpenBoxHistory</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.OpenBoxHistory ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto OpenBoxHistory de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a OpenBoxHistory</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo OpenBoxHistory</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.OpenBoxHistory> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.OpenBoxHistory> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[OpenBoxHistory] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.OpenBoxHistory> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.Product> lstProducts = null;
			IEnumerable<BE.Base.Classifier> lstSubsidiarys = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Product.relOpenBoxHistory.Product))
				{
					using(var dal = new Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Product.relOpenBoxHistory.Subsidiary))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.IdSubsidiary).Distinct();
						lstSubsidiarys = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstProducts != null)
					{
						Item.Product = (from i in lstProducts where i.Id == Item.IdProduct select i).FirstOrDefault();
					}					if (lstSubsidiarys != null)
					{
						Item.Subsidiary = (from i in lstSubsidiarys where i.Id == Item.IdSubsidiary select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.OpenBoxHistory Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Product.relOpenBoxHistory.Product))
				{
					using (var dal = new Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}				if (RelationEnum.Equals(BE.Product.relOpenBoxHistory.Subsidiary))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.Subsidiary = dal.ReturnMaster(Item.IdSubsidiary, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of OpenBoxHistory
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type OpenBoxHistory</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.OpenBoxHistory> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[OpenBoxHistory] ORDER By " + Order;
            IEnumerable<BEP.OpenBoxHistory> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of OpenBoxHistory
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.OpenBoxHistory> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[OpenBoxHistory] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.OpenBoxHistory> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of OpenBoxHistory
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type OpenBoxHistory</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.OpenBoxHistory> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[OpenBoxHistory] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.OpenBoxHistory> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type OpenBoxHistory    	
        /// </summary>
        /// <param name="Id">Object identifier OpenBoxHistory</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type OpenBoxHistory</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.OpenBoxHistory Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[OpenBoxHistory] WHERE [Id] = @Id";
            BEP.OpenBoxHistory Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public OpenBoxHistory() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public OpenBoxHistory(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal OpenBoxHistory(SqlConnection connection) : base(connection) { }

        #endregion

    }
}