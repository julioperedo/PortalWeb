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
    /// Class     : RelatedProduct
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type RelatedProduct 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   7/9/2022 16:11:28 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class RelatedProduct : DALEntity<BEP.RelatedProduct>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type RelatedProduct
        /// </summary>
        /// <param name="Item">Business object of type RelatedProduct </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.RelatedProduct Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[RelatedProduct]([IdProduct], [IdRelated], [LogUser], [LogDate]) VALUES(@IdProduct, @IdRelated, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[RelatedProduct] SET [IdProduct] = @IdProduct, [IdRelated] = @IdRelated, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[RelatedProduct] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  RelatedProduct		
        /// </summary>
        /// <param name="Items">Business object of type RelatedProduct para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.RelatedProduct> Items)
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
        /// 	For use on data access layer at assembly level, return an  RelatedProduct type object
        /// </summary>
        /// <param name="Id">Object Identifier RelatedProduct</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type RelatedProduct</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.RelatedProduct ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto RelatedProduct de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a RelatedProduct</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo RelatedProduct</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.RelatedProduct> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.RelatedProduct> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[RelatedProduct] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.RelatedProduct> Items, params Enum[] Relations)
        {
            IEnumerable<long> Keys;
            IEnumerable<BE.Product.Product> lstProducts = null;
            IEnumerable<BE.Product.Product> lstRelateds = null;

            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BE.Product.relRelatedProduct.Product))
                {
                    using (var dal = new Product(Connection))
                    {
                        Keys = (from i in Items select i.IdProduct).Distinct();
                        lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relRelatedProduct.Related))
                {
                    using (var dal = new Product(Connection))
                    {
                        Keys = (from i in Items select i.IdRelated).Distinct();
                        lstRelateds = dal.ReturnChild(Keys, Relations)?.ToList();
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
                    }
                    if (lstRelateds != null)
                    {
                        Item.Related = (from i in lstRelateds where i.Id == Item.IdRelated select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.RelatedProduct Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BE.Product.relRelatedProduct.Product))
                {
                    using (var dal = new Product(Connection))
                    {
                        Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relRelatedProduct.Related))
                {
                    using (var dal = new Product(Connection))
                    {
                        Item.Related = dal.ReturnMaster(Item.IdRelated, Relations);
                    }
                }
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of RelatedProduct
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type RelatedProduct</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.RelatedProduct> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[RelatedProduct] ORDER By " + Order;
            IEnumerable<BEP.RelatedProduct> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of RelatedProduct
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.RelatedProduct> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[RelatedProduct] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.RelatedProduct> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of RelatedProduct
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type RelatedProduct</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.RelatedProduct> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[RelatedProduct] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.RelatedProduct> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type RelatedProduct    	
        /// </summary>
        /// <param name="Id">Object identifier RelatedProduct</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type RelatedProduct</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.RelatedProduct Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[RelatedProduct] WHERE [Id] = @Id";
            BEP.RelatedProduct Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public RelatedProduct() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public RelatedProduct(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal RelatedProduct(SqlConnection connection) : base(connection) { }

        #endregion

    }
}