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
    /// Class     : Document
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Document 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   28/07/2022 13:40:20 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Document : DALEntity<BEP.Document>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Document
        /// </summary>
        /// <param name="Item">Business object of type Document </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.Document Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[Document]([IdProduct], [TypeIdc], [Name], [Description], [ReleaseDate], [Enabled], [LogUser], [LogDate]) VALUES(@IdProduct, @TypeIdc, @Name, @Description, @ReleaseDate, @Enabled, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[Document] SET [IdProduct] = @IdProduct, [TypeIdc] = @TypeIdc, [Name] = @Name, [Description] = @Description, [ReleaseDate] = @ReleaseDate, [Enabled] = @Enabled, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[Document] WHERE [Id] = @Id";
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
			if (Item.ListDocumentFiles?.Count() > 0)
			{
				var list = Item.ListDocumentFiles;
				foreach (var item in list) item.IdDocument = itemId;
				using (var dal = new DocumentFile(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListDocumentFiles = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Document		
        /// </summary>
        /// <param name="Items">Business object of type Document para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.Document> Items)
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
        /// 	For use on data access layer at assembly level, return an  Document type object
        /// </summary>
        /// <param name="Id">Object Identifier Document</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Document</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.Document ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Document de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Document</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Document</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.Document> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.Document> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[Document] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.Document> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.DocumentFile> lstDocumentFiles = null; 
			IEnumerable<BE.Product.Product> lstProducts = null;
			IEnumerable<BE.Base.Classifier> lstTypes = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Product.relDocument.DocumentFiles))
				{
					using (var dal = new DocumentFile(Connection))
					{
						lstDocumentFiles = dal.List(Keys, "IdDocument", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Product.relDocument.Product))
				{
					using(var dal = new Product(Connection))
					{
						Keys = (from i in Items select i.IdProduct).Distinct();
						lstProducts = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Product.relDocument.Type))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.TypeIdc).Distinct();
						lstTypes = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstDocumentFiles != null)
					{
						Item.ListDocumentFiles = lstDocumentFiles.Where(x => x.IdDocument == Item.Id)?.ToList();
					}
					if (lstProducts != null)
					{
						Item.Product = (from i in lstProducts where i.Id == Item.IdProduct select i).FirstOrDefault();
					}					if (lstTypes != null)
					{
						Item.Type = (from i in lstTypes where i.Id == Item.TypeIdc select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.Document Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Product.relDocument.DocumentFiles))
				{
					using (var dal = new DocumentFile(Connection))
					{
						Item.ListDocumentFiles = dal.List(Keys, "IdDocument", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Product.relDocument.Product))
				{
					using (var dal = new Product(Connection))
					{
						Item.Product = dal.ReturnMaster(Item.IdProduct, Relations);
					}
				}				if (RelationEnum.Equals(BE.Product.relDocument.Type))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.Type = dal.ReturnMaster(Item.TypeIdc, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Document
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Document</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Document> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[Document] ORDER By " + Order;
            IEnumerable<BEP.Document> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Document
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Document> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[Document] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.Document> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Document
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Document</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Document> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[Document] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.Document> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Document    	
        /// </summary>
        /// <param name="Id">Object identifier Document</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Document</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.Document Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[Document] WHERE [Id] = @Id";
            BEP.Document Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Document() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Document(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Document(SqlConnection connection) : base(connection) { }

        #endregion

    }
}