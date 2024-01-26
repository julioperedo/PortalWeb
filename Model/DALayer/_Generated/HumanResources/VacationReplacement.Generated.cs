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
    /// Class     : VacationReplacement
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type VacationReplacement 
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
    public partial class VacationReplacement : DALEntity<BEH.VacationReplacement>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type VacationReplacement
        /// </summary>
        /// <param name="Item">Business object of type VacationReplacement </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEH.VacationReplacement Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [HumanResources].[VacationReplacement]([IdVacation], [IdReplacement], [FromDate], [FromDatePeriod], [ToDate], [ToDatePeriod], [IdState], [LogUser], [LogDate]) VALUES(@IdVacation, @IdReplacement, @FromDate, @FromDatePeriod, @ToDate, @ToDatePeriod, @IdState, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [HumanResources].[VacationReplacement] SET [IdVacation] = @IdVacation, [IdReplacement] = @IdReplacement, [FromDate] = @FromDate, [FromDatePeriod] = @FromDatePeriod, [ToDate] = @ToDate, [ToDatePeriod] = @ToDatePeriod, [IdState] = @IdState, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [HumanResources].[VacationReplacement] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  VacationReplacement		
        /// </summary>
        /// <param name="Items">Business object of type VacationReplacement para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEH.VacationReplacement> Items)
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
        /// 	For use on data access layer at assembly level, return an  VacationReplacement type object
        /// </summary>
        /// <param name="Id">Object Identifier VacationReplacement</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type VacationReplacement</returns>
        /// <remarks>
        /// </remarks>		
        internal BEH.VacationReplacement ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto VacationReplacement de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a VacationReplacement</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo VacationReplacement</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEH.VacationReplacement> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEH.VacationReplacement> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [HumanResources].[VacationReplacement] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEH.VacationReplacement> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.HumanResources.Employee> lstReplacements = null;
			IEnumerable<BE.HumanResources.Vacation> lstVacations = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relVacationReplacement.Replacement))
				{
					using(var dal = new Employee(Connection))
					{
						Keys = (from i in Items select i.IdReplacement).Distinct();
						lstReplacements = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relVacationReplacement.Vacation))
				{
					using(var dal = new Vacation(Connection))
					{
						Keys = (from i in Items select i.IdVacation).Distinct();
						lstVacations = dal.ReturnChild(Keys, Relations)?.ToList();
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
					}					if (lstVacations != null)
					{
						Item.Vacation = (from i in lstVacations where i.Id == Item.IdVacation select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEH.VacationReplacement Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.HumanResources.relVacationReplacement.Replacement))
				{
					using (var dal = new Employee(Connection))
					{
						Item.Replacement = dal.ReturnMaster(Item.IdReplacement, Relations);
					}
				}				if (RelationEnum.Equals(BE.HumanResources.relVacationReplacement.Vacation))
				{
					using (var dal = new Vacation(Connection))
					{
						Item.Vacation = dal.ReturnMaster(Item.IdVacation, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of VacationReplacement
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type VacationReplacement</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.VacationReplacement> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [HumanResources].[VacationReplacement] ORDER By " + Order;
            IEnumerable<BEH.VacationReplacement> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of VacationReplacement
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.VacationReplacement> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [HumanResources].[VacationReplacement] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEH.VacationReplacement> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of VacationReplacement
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type VacationReplacement</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEH.VacationReplacement> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[VacationReplacement] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEH.VacationReplacement> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type VacationReplacement    	
        /// </summary>
        /// <param name="Id">Object identifier VacationReplacement</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type VacationReplacement</returns>
        /// <remarks>
        /// </remarks>    
        public BEH.VacationReplacement Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [HumanResources].[VacationReplacement] WHERE [Id] = @Id";
            BEH.VacationReplacement Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public VacationReplacement() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public VacationReplacement(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal VacationReplacement(SqlConnection connection) : base(connection) { }

        #endregion

    }
}