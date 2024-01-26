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
    /// Class     : DocumentFile
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type DocumentFile 
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
    public partial class DocumentFile : DALEntity<BEP.DocumentFile>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type DocumentFile
        /// </summary>
        /// <param name="Item">Business object of type DocumentFile </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.DocumentFile Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[DocumentFile]([IdDocument], [FileURL], [LogUser], [LogDate]) VALUES(@IdDocument, @FileURL, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[DocumentFile] SET [IdDocument] = @IdDocument, [FileURL] = @FileURL, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[DocumentFile] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  DocumentFile		
        /// </summary>
        /// <param name="Items">Business object of type DocumentFile para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.DocumentFile> Items)
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
        /// 	For use on data access layer at assembly level, return an  DocumentFile type object
        /// </summary>
        /// <param name="Id">Object Identifier DocumentFile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type DocumentFile</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.DocumentFile ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto DocumentFile de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a DocumentFile</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo DocumentFile</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.DocumentFile> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.DocumentFile> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[DocumentFile] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.DocumentFile> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Product.Document> lstDocuments = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Product.relDocumentFile.Document))
				{
					using(var dal = new Document(Connection))
					{
						Keys = (from i in Items select i.IdDocument).Distinct();
						lstDocuments = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstDocuments != null)
					{
						Item.Document = (from i in lstDocuments where i.Id == Item.IdDocument select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEP.DocumentFile Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Product.relDocumentFile.Document))
				{
					using (var dal = new Document(Connection))
					{
						Item.Document = dal.ReturnMaster(Item.IdDocument, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of DocumentFile
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type DocumentFile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.DocumentFile> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[DocumentFile] ORDER By " + Order;
            IEnumerable<BEP.DocumentFile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of DocumentFile
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.DocumentFile> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[DocumentFile] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.DocumentFile> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of DocumentFile
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type DocumentFile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.DocumentFile> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[DocumentFile] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.DocumentFile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type DocumentFile    	
        /// </summary>
        /// <param name="Id">Object identifier DocumentFile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type DocumentFile</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.DocumentFile Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[DocumentFile] WHERE [Id] = @Id";
            BEP.DocumentFile Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public DocumentFile() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public DocumentFile(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal DocumentFile(SqlConnection connection) : base(connection) { }

        #endregion

    }
}