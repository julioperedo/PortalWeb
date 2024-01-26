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
    /// Class     : License
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type License 
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
    public partial class License : DALEntity<BEH.License>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type License
        /// </summary>
        /// <param name="Item">Business object of type License </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.License Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[License]([IdRequest], [Date], [InitialTime], [FinalTime], [IdReason], [ReasonDescription], [LogUser], [LogDate]) VALUES(@IdRequest, @Date, @InitialTime, @FinalTime, @IdReason, @ReasonDescription, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[License] SET [IdRequest] = @IdRequest, [Date] = @Date, [InitialTime] = @InitialTime, [FinalTime] = @FinalTime, [IdReason] = @IdReason, [ReasonDescription] = @ReasonDescription, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[License] WHERE [Id] = @Id";
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
			if (Item.ListVacations?.Count() > 0)
			{
				var list = Item.ListVacations;
				foreach (var item in list) item.IdLicense = itemId;
				using (var dal = new Vacation(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVacations = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  License		
        /// </summary>
        /// <param name="Items">Business object of type License para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.License> Items)
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
        /// 	For use on data access layer at assembly level, return an  License type object
        /// </summary>
        /// <param name="Id">Object Identifier License</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type License</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.License ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto License de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a License</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo License</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.License> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.License> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[License] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.License> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.Vacation> lstVacations = null; 
			IEnumerable<BE.Base.Classifier> lstReasons = null;
			IEnumerable<BE.HumanResources.Request> lstRequests = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.HumanResources.relLicense.Vacations))
				{
					using (var dal = new Vacation(Connection))
					{
						lstVacations = dal.List(Keys, "IdLicense", Relations);
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relLicense.Reason))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items where i.IdReason.HasValue select i.IdReason.Value).Distinct();
						lstReasons = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relLicense.Request))
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
					if (lstVacations != null)
					{
						Item.ListVacations = lstVacations.Where(x => x.IdLicense == Item.Id)?.ToList();
					}
					if (lstReasons != null)
					{
						Item.Reason = (from i in lstReasons where i.Id == Item.IdReason select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEH.License Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.HumanResources.relLicense.Vacations))
				{
					using (var dal = new Vacation(Connection))
					{
						Item.ListVacations = dal.List(Keys, "IdLicense", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.HumanResources.relLicense.Reason))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						if (Item.IdReason.HasValue)
						{
							Item.Reason = dal.ReturnMaster(Item.IdReason.Value, Relations);
						}
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relLicense.Request))
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
        /// 	Return an object Collection of License
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type License</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.License> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[License] ORDER By " + Order;
            IEnumerable<BEH.License> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of License
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.License> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[License] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.License> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of License
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type License</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.License> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[License] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.License> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type License    	
        /// </summary>
        /// <param name="Id">Object identifier License</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type License</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.License Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[License] WHERE [Id] = @Id";
            BEH.License Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public License() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public License(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal License(SqlConnection connection) : base(connection) { }

        #endregion

    }
}