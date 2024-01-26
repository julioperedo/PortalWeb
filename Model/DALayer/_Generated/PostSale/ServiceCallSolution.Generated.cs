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
    /// Class     : ServiceCallSolution
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ServiceCallSolution 
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
    public partial class ServiceCallSolution : DALEntity<BET.ServiceCallSolution>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ServiceCallSolution
        /// </summary>
        /// <param name="Item">Business object of type ServiceCallSolution </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BET.ServiceCallSolution Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PostSale].[ServiceCallSolution]([IdServiceCall], [SAPCode], [ItemCode], [StatusCode], [Status], [OwnerCode], [Owner], [CreateDate], [UpdatedByCode], [UpdatedBy], [UpdateDate], [Subject], [Symtom], [Cause], [Description], [Attachment], [LogUser], [LogDate]) VALUES(@IdServiceCall, @SAPCode, @ItemCode, @StatusCode, @Status, @OwnerCode, @Owner, @CreateDate, @UpdatedByCode, @UpdatedBy, @UpdateDate, @Subject, @Symtom, @Cause, @Description, @Attachment, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PostSale].[ServiceCallSolution] SET [IdServiceCall] = @IdServiceCall, [SAPCode] = @SAPCode, [ItemCode] = @ItemCode, [StatusCode] = @StatusCode, [Status] = @Status, [OwnerCode] = @OwnerCode, [Owner] = @Owner, [CreateDate] = @CreateDate, [UpdatedByCode] = @UpdatedByCode, [UpdatedBy] = @UpdatedBy, [UpdateDate] = @UpdateDate, [Subject] = @Subject, [Symtom] = @Symtom, [Cause] = @Cause, [Description] = @Description, [Attachment] = @Attachment, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PostSale].[ServiceCallSolution] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ServiceCallSolution		
        /// </summary>
        /// <param name="Items">Business object of type ServiceCallSolution para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BET.ServiceCallSolution> Items)
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
        /// 	For use on data access layer at assembly level, return an  ServiceCallSolution type object
        /// </summary>
        /// <param name="Id">Object Identifier ServiceCallSolution</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCallSolution</returns>
        /// <remarks>
        /// </remarks>		
        internal BET.ServiceCallSolution ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ServiceCallSolution de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ServiceCallSolution</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ServiceCallSolution</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BET.ServiceCallSolution> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BET.ServiceCallSolution> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PostSale].[ServiceCallSolution] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BET.ServiceCallSolution> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.PostSale.ServiceCall> lstServiceCalls = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PostSale.relServiceCallSolution.ServiceCall))
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
        protected override void LoadRelations(ref BET.ServiceCallSolution Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PostSale.relServiceCallSolution.ServiceCall))
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
        /// 	Return an object Collection of ServiceCallSolution
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCallSolution</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallSolution> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PostSale].[ServiceCallSolution] ORDER By " + Order;
            IEnumerable<BET.ServiceCallSolution> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ServiceCallSolution
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallSolution> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PostSale].[ServiceCallSolution] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BET.ServiceCallSolution> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ServiceCallSolution
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ServiceCallSolution</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BET.ServiceCallSolution> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCallSolution] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BET.ServiceCallSolution> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ServiceCallSolution    	
        /// </summary>
        /// <param name="Id">Object identifier ServiceCallSolution</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ServiceCallSolution</returns>
        /// <remarks>
        /// </remarks>    
        public BET.ServiceCallSolution Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PostSale].[ServiceCallSolution] WHERE [Id] = @Id";
            BET.ServiceCallSolution Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ServiceCallSolution() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ServiceCallSolution(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ServiceCallSolution(SqlConnection connection) : base(connection) { }

        #endregion

    }
}