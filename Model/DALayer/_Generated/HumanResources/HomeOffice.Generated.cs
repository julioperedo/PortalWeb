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
using BEN = BEntities.Campaign;


namespace DALayer.HumanResources
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : HumanResources
    /// Class     : HomeOffice
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type HomeOffice 
    ///     for the service HumanResources.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service HumanResources
    /// </remarks>
    /// <history>
    ///     [DMC]   4/12/2023 14:06:20 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class HomeOffice : DALEntity<BEH.HomeOffice>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type HomeOffice
        /// </summary>
        /// <param name="Item">Business object of type HomeOffice </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.HomeOffice Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[HomeOffice]([IdRequest], [FromDate], [FromDatePeriod], [ToDate], [ToDatePeriod], [LogUser], [LogDate]) VALUES(@IdRequest, @FromDate, @FromDatePeriod, @ToDate, @ToDatePeriod, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[HomeOffice] SET [IdRequest] = @IdRequest, [FromDate] = @FromDate, [FromDatePeriod] = @FromDatePeriod, [ToDate] = @ToDate, [ToDatePeriod] = @ToDatePeriod, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[HomeOffice] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  HomeOffice		
        /// </summary>
        /// <param name="Items">Business object of type HomeOffice para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.HomeOffice> Items)
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
        /// 	For use on data access layer at assembly level, return an  HomeOffice type object
        /// </summary>
        /// <param name="Id">Object Identifier HomeOffice</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type HomeOffice</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.HomeOffice ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto HomeOffice de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a HomeOffice</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo HomeOffice</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.HomeOffice> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.HomeOffice> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[HomeOffice] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.HomeOffice> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.Request> lstRequests = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relHomeOffice.Request))
				{
					using(var dal = new Request(Connection))
					{
						Keys = (from i in Items select i.IdRequest).Distinct();
						lstRequests = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstRequests != null)
					{
						Item.Request = (from i in lstRequests where i.Id == Item.IdRequest select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEH.HomeOffice Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relHomeOffice.Request))
				{
					using (var dal = new Request(Connection))
					{
						Item.Request = dal.ReturnMaster(Item.IdRequest, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of HomeOffice
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type HomeOffice</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.HomeOffice> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[HomeOffice] ORDER By " + Order;
            IEnumerable<BEH.HomeOffice> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of HomeOffice
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.HomeOffice> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[HomeOffice] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.HomeOffice> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of HomeOffice
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type HomeOffice</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.HomeOffice> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[HomeOffice] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.HomeOffice> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type HomeOffice    	
        /// </summary>
        /// <param name="Id">Object identifier HomeOffice</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type HomeOffice</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.HomeOffice Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[HomeOffice] WHERE [Id] = @Id";
            BEH.HomeOffice Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public HomeOffice() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public HomeOffice(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal HomeOffice(SqlConnection connection) : base(connection) { }

        #endregion

    }
}