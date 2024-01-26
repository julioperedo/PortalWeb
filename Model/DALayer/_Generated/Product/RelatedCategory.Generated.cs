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


namespace DALayer.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Product
    /// Class     : RelatedCategory
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type RelatedCategory 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   7/9/2022 16:09:18 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class RelatedCategory : DALEntity<BEP.RelatedCategory>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type RelatedCategory
        /// </summary>
        /// <param name="Item">Business object of type RelatedCategory </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.RelatedCategory Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[RelatedCategory]([Category], [Related], [LogUser], [LogDate]) VALUES(@Category, @Related, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[RelatedCategory] SET [Category] = @Category, [Related] = @Related, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[RelatedCategory] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  RelatedCategory		
        /// </summary>
        /// <param name="Items">Business object of type RelatedCategory para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.RelatedCategory> Items)
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
        /// 	For use on data access layer at assembly level, return an  RelatedCategory type object
        /// </summary>
        /// <param name="Id">Object Identifier RelatedCategory</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type RelatedCategory</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.RelatedCategory ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto RelatedCategory de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a RelatedCategory</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo RelatedCategory</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.RelatedCategory> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.RelatedCategory> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[RelatedCategory] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.RelatedCategory> Items, params Enum[] Relations)
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
        protected override void LoadRelations(ref BEP.RelatedCategory Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {

            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of RelatedCategory
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type RelatedCategory</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.RelatedCategory> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[RelatedCategory] ORDER By " + Order;
            IEnumerable<BEP.RelatedCategory> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of RelatedCategory
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.RelatedCategory> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[RelatedCategory] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.RelatedCategory> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of RelatedCategory
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type RelatedCategory</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.RelatedCategory> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[RelatedCategory] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.RelatedCategory> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type RelatedCategory    	
        /// </summary>
        /// <param name="Id">Object identifier RelatedCategory</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type RelatedCategory</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.RelatedCategory Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[RelatedCategory] WHERE [Id] = @Id";
            BEP.RelatedCategory Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public RelatedCategory() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public RelatedCategory(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal RelatedCategory(SqlConnection connection) : base(connection) { }

        #endregion

    }
}