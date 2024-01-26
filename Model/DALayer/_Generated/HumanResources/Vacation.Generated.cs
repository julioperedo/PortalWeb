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
    /// Class     : Vacation
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Vacation 
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
    public partial class Vacation : DALEntity<BEH.Vacation>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Vacation
        /// </summary>
        /// <param name="Item">Business object of type Vacation </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.Vacation Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[Vacation]([IdRequest], [IdLicense], [FromDate], [FromDatePeriod], [ToDate], [ToDatePeriod], [Days], [LogUser], [LogDate]) VALUES(@IdRequest, @IdLicense, @FromDate, @FromDatePeriod, @ToDate, @ToDatePeriod, @Days, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[Vacation] SET [IdRequest] = @IdRequest, [IdLicense] = @IdLicense, [FromDate] = @FromDate, [FromDatePeriod] = @FromDatePeriod, [ToDate] = @ToDate, [ToDatePeriod] = @ToDatePeriod, [Days] = @Days, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[Vacation] WHERE [Id] = @Id";
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
			if (Item.ListVacationReplacements?.Count() > 0)
			{
				var list = Item.ListVacationReplacements;
				foreach (var item in list) item.IdVacation = itemId;
				using (var dal = new VacationReplacement(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVacationReplacements = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Vacation		
        /// </summary>
        /// <param name="Items">Business object of type Vacation para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.Vacation> Items)
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
        /// 	For use on data access layer at assembly level, return an  Vacation type object
        /// </summary>
        /// <param name="Id">Object Identifier Vacation</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Vacation</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.Vacation ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Vacation de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Vacation</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Vacation</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.Vacation> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.Vacation> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[Vacation] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.Vacation> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.VacationReplacement> lstVacationReplacements = null; 
			IEnumerable<BE.HumanResources.License> lstLicenses = null;
			IEnumerable<BE.HumanResources.Request> lstRequests = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.HumanResources.relVacation.VacationReplacements))
				{
					using (var dal = new VacationReplacement(Connection))
					{
						lstVacationReplacements = dal.List(Keys, "IdVacation", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relVacation.License))
				{
					using(var dal = new License(Connection))
					{
						Keys = (from i in Items where i.IdLicense.HasValue select i.IdLicense.Value).Distinct();
						lstLicenses = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relVacation.Request))
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
					if (lstVacationReplacements != null)
					{
						Item.ListVacationReplacements = lstVacationReplacements.Where(x => x.IdVacation == Item.Id)?.ToList();
					}
					if (lstLicenses != null)
					{
						Item.License = (from i in lstLicenses where i.Id == Item.IdLicense select i).FirstOrDefault();
					}					if (lstRequests != null)
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
        protected override void LoadRelations(ref BEH.Vacation Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.HumanResources.relVacation.VacationReplacements))
				{
					using (var dal = new VacationReplacement(Connection))
					{
						Item.ListVacationReplacements = dal.List(Keys, "IdVacation", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relVacation.License))
				{
					using (var dal = new License(Connection))
					{
						if (Item.IdLicense.HasValue)
						{
							Item.License = dal.ReturnMaster(Item.IdLicense.Value, Relations);
						}
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relVacation.Request))
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
        /// 	Return an object Collection of Vacation
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Vacation</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Vacation> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[Vacation] ORDER By " + Order;
            IEnumerable<BEH.Vacation> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Vacation
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Vacation> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[Vacation] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.Vacation> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Vacation
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Vacation</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.Vacation> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[Vacation] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.Vacation> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Vacation    	
        /// </summary>
        /// <param name="Id">Object identifier Vacation</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Vacation</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.Vacation Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[Vacation] WHERE [Id] = @Id";
            BEH.Vacation Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Vacation() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Vacation(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Vacation(SqlConnection connection) : base(connection) { }

        #endregion

    }
}