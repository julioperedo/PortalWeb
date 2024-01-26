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


namespace DALayer.PiggyBank
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : PiggyBank
    /// Class     : UsedProduct
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type UsedProduct 
    ///     for the service PiggyBank.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PiggyBank
    /// </remarks>
    /// <history>
    ///     [DMC]   26/7/2023 11:16:21 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class UsedProduct : DALEntity<BEI.UsedProduct>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type UsedProduct
        /// </summary>
        /// <param name="Item">Business object of type UsedProduct </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEI.UsedProduct Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PiggyBank].[UsedProduct]([ItemCode], [LogUser], [LogDate]) VALUES(@ItemCode, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PiggyBank].[UsedProduct] SET [ItemCode] = @ItemCode, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PiggyBank].[UsedProduct] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  UsedProduct		
        /// </summary>
        /// <param name="Items">Business object of type UsedProduct para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEI.UsedProduct> Items)
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
        /// 	For use on data access layer at assembly level, return an  UsedProduct type object
        /// </summary>
        /// <param name="Id">Object Identifier UsedProduct</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UsedProduct</returns>
        /// <remarks>
        /// </remarks>		
        internal BEI.UsedProduct ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto UsedProduct de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a UsedProduct</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo UsedProduct</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEI.UsedProduct> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEI.UsedProduct> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PiggyBank].[UsedProduct] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEI.UsedProduct> Items, params Enum[] Relations)
        {

            foreach (Enum RelationEnum in Relations)
            {

            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {

                }
            }
        }

        /// <summary>
        /// Load Relationship of an Object
        /// </summary>
        /// <param name="Item">Given Object</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <remarks></remarks>
        protected override void LoadRelations(ref BEI.UsedProduct Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of UsedProduct
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UsedProduct</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.UsedProduct> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PiggyBank].[UsedProduct] ORDER By " + Order;
            IEnumerable<BEI.UsedProduct> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of UsedProduct
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.UsedProduct> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PiggyBank].[UsedProduct] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEI.UsedProduct> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of UsedProduct
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type UsedProduct</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.UsedProduct> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[UsedProduct] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEI.UsedProduct> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type UsedProduct    	
        /// </summary>
        /// <param name="Id">Object identifier UsedProduct</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type UsedProduct</returns>
        /// <remarks>
        /// </remarks>    
        public BEI.UsedProduct Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[UsedProduct] WHERE [Id] = @Id";
            BEI.UsedProduct Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public UsedProduct() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public UsedProduct(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal UsedProduct(SqlConnection connection) : base(connection) { }

        #endregion

    }
}