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
    /// Class     : Travel
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Travel 
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
    public partial class Travel : DALEntity<BEH.Travel>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Travel
        /// </summary>
        /// <param name="Item">Business object of type Travel </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.Travel Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[Travel]([IdRequest], [FromDate], [FromDatePeriod], [ToDate], [ToDatePeriod], [Destiny], [LogUser], [LogDate]) VALUES(@IdRequest, @FromDate, @FromDatePeriod, @ToDate, @ToDatePeriod, @Destiny, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[Travel] SET [IdRequest] = @IdRequest, [FromDate] = @FromDate, [FromDatePeriod] = @FromDatePeriod, [ToDate] = @ToDate, [ToDatePeriod] = @ToDatePeriod, [Destiny] = @Destiny, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[Travel] WHERE [Id] = @Id";
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
			if (Item.ListTravelReplacements?.Count() > 0)
			{
				var list = Item.ListTravelReplacements;
				foreach (var item in list) item.IdTravel = itemId;
				using (var dal = new TravelReplacement(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTravelReplacements = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Travel		
        /// </summary>
        /// <param name="Items">Business object of type Travel para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.Travel> Items)
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
        /// 	For use on data access layer at assembly level, return an  Travel type object
        /// </summary>
        /// <param name="Id">Object Identifier Travel</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Travel</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.Travel ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Travel de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Travel</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Travel</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.Travel> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.Travel> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[Travel] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.Travel> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.TravelReplacement> lstTravelReplacements = null; 
			IEnumerable<BE.HumanResources.Request> lstRequests = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.HumanResources.relTravel.TravelReplacements))
				{
					using (var dal = new TravelReplacement(Connection))
					{
						lstTravelReplacements = dal.List(Keys, "IdTravel", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relTravel.Request))
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
					if (lstTravelReplacements != null)
					{
						Item.ListTravelReplacements = lstTravelReplacements.Where(x => x.IdTravel == Item.Id)?.ToList();
					}
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
        protected override void LoadRelations(ref BEH.Travel Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.HumanResources.relTravel.TravelReplacements))
				{
					using (var dal = new TravelReplacement(Connection))
					{
						Item.ListTravelReplacements = dal.List(Keys, "IdTravel", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relTravel.Request))
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
        /// 	Return an object Collection of Travel
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Travel</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Travel> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[Travel] ORDER By " + Order;
            IEnumerable<BEH.Travel> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Travel
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Travel> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[Travel] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.Travel> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Travel
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Travel</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Travel> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[Travel] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.Travel> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Travel    	
        /// </summary>
        /// <param name="Id">Object identifier Travel</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Travel</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.Travel Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[Travel] WHERE [Id] = @Id";
            BEH.Travel Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Travel() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Travel(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Travel(SqlConnection connection) : base(connection) { }

        #endregion

    }
}