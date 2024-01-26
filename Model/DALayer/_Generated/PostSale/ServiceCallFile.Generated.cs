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


namespace DALayer.PostSale
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : PostSale
    /// Class     : ServiceCallFile
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ServiceCallFile 
    ///     for the service PostSale.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PostSale
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:42 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ServiceCallFile : DALEntity<BET.ServiceCallFile>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ServiceCallFile
        /// </summary>
        /// <param name="Item">Business object of type ServiceCallFile </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BET.ServiceCallFile Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PostSale].[ServiceCallFile]([IdServiceCall], [FileName]) VALUES(@IdServiceCall, @FileName) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PostSale].[ServiceCallFile] SET [IdServiceCall] = @IdServiceCall, [FileName] = @FileName WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PostSale].[ServiceCallFile] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ServiceCallFile		
        /// </summary>
        /// <param name="Items">Business object of type ServiceCallFile para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BET.ServiceCallFile> Items)
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
        /// 	For use on data access layer at assembly level, return an  ServiceCallFile type object
        /// </summary>
        /// <param name="Id">Object Identifier ServiceCallFile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCallFile</returns>
        /// <remarks>
        /// </remarks>		
        internal BET.ServiceCallFile ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ServiceCallFile de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ServiceCallFile</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ServiceCallFile</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BET.ServiceCallFile> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BET.ServiceCallFile> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PostSale].[ServiceCallFile] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BET.ServiceCallFile> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.PostSale.ServiceCall> lstServiceCalls = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PostSale.relServiceCallFile.ServiceCall))
				{
					using(var dal = new ServiceCall(Connection))
					{
						Keys = (from i in Items select i.IdServiceCall).Distinct();
						lstServiceCalls = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstServiceCalls != null)
					{
						Item.ServiceCall = (from i in lstServiceCalls where i.Id == Item.IdServiceCall select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BET.ServiceCallFile Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PostSale.relServiceCallFile.ServiceCall))
				{
					using (var dal = new ServiceCall(Connection))
					{
						Item.ServiceCall = dal.ReturnMaster(Item.IdServiceCall, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ServiceCallFile
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCallFile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallFile> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PostSale].[ServiceCallFile] ORDER By " + Order;
            IEnumerable<BET.ServiceCallFile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ServiceCallFile
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallFile> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PostSale].[ServiceCallFile] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BET.ServiceCallFile> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ServiceCallFile
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCallFile</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallFile> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCallFile] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BET.ServiceCallFile> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ServiceCallFile    	
        /// </summary>
        /// <param name="Id">Object identifier ServiceCallFile</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCallFile</returns>
        /// <remarks>
        /// </remarks>    
        public BET.ServiceCallFile Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCallFile] WHERE [Id] = @Id";
            BET.ServiceCallFile Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ServiceCallFile() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ServiceCallFile(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ServiceCallFile(SqlConnection connection) : base(connection) { }

        #endregion

    }
}