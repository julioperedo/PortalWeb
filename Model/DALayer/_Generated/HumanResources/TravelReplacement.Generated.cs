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
    /// Class     : TravelReplacement
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type TravelReplacement 
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
    public partial class TravelReplacement : DALEntity<BEH.TravelReplacement>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type TravelReplacement
        /// </summary>
        /// <param name="Item">Business object of type TravelReplacement </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.TravelReplacement Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[TravelReplacement]([IdTravel], [IdReplacement], [FromDate], [FromDatePeriod], [ToDate], [ToDatePeriod], [IdState], [LogUser], [LogDate]) VALUES(@IdTravel, @IdReplacement, @FromDate, @FromDatePeriod, @ToDate, @ToDatePeriod, @IdState, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[TravelReplacement] SET [IdTravel] = @IdTravel, [IdReplacement] = @IdReplacement, [FromDate] = @FromDate, [FromDatePeriod] = @FromDatePeriod, [ToDate] = @ToDate, [ToDatePeriod] = @ToDatePeriod, [IdState] = @IdState, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[TravelReplacement] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  TravelReplacement		
        /// </summary>
        /// <param name="Items">Business object of type TravelReplacement para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.TravelReplacement> Items)
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
        /// 	For use on data access layer at assembly level, return an  TravelReplacement type object
        /// </summary>
        /// <param name="Id">Object Identifier TravelReplacement</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TravelReplacement</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.TravelReplacement ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto TravelReplacement de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a TravelReplacement</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo TravelReplacement</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.TravelReplacement> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.TravelReplacement> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[TravelReplacement] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.TravelReplacement> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.Employee> lstReplacements = null;
			IEnumerable<BE.HumanResources.Travel> lstTravels = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relTravelReplacement.Replacement))
				{
					using(var dal = new Employee(Connection))
					{
						Keys = (from i in Items select i.IdReplacement).Distinct();
						lstReplacements = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relTravelReplacement.Travel))
				{
					using(var dal = new Travel(Connection))
					{
						Keys = (from i in Items select i.IdTravel).Distinct();
						lstTravels = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstReplacements != null)
					{
						Item.Replacement = (from i in lstReplacements where i.Id == Item.IdReplacement select i).FirstOrDefault();
					}					if (lstTravels != null)
					{
						Item.Travel = (from i in lstTravels where i.Id == Item.IdTravel select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEH.TravelReplacement Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relTravelReplacement.Replacement))
				{
					using (var dal = new Employee(Connection))
					{
						Item.Replacement = dal.ReturnMaster(Item.IdReplacement, Relations);
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relTravelReplacement.Travel))
				{
					using (var dal = new Travel(Connection))
					{
						Item.Travel = dal.ReturnMaster(Item.IdTravel, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of TravelReplacement
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TravelReplacement</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.TravelReplacement> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[TravelReplacement] ORDER By " + Order;
            IEnumerable<BEH.TravelReplacement> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of TravelReplacement
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.TravelReplacement> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[TravelReplacement] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.TravelReplacement> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of TravelReplacement
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type TravelReplacement</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.TravelReplacement> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[TravelReplacement] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.TravelReplacement> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type TravelReplacement    	
        /// </summary>
        /// <param name="Id">Object identifier TravelReplacement</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type TravelReplacement</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.TravelReplacement Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[TravelReplacement] WHERE [Id] = @Id";
            BEH.TravelReplacement Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public TravelReplacement() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public TravelReplacement(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal TravelReplacement(SqlConnection connection) : base(connection) { }

        #endregion

    }
}