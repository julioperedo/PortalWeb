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


namespace DALayer.PiggyBank
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : PiggyBank
    /// Class     : ClaimedPrize
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ClaimedPrize 
    ///     for the service PiggyBank.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PiggyBank
    /// </remarks>
    /// <history>
    ///     [DMC]   24/10/2023 16:45:32 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ClaimedPrize : DALEntity<BEI.ClaimedPrize>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ClaimedPrize
        /// </summary>
        /// <param name="Item">Business object of type ClaimedPrize </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEI.ClaimedPrize Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PiggyBank].[ClaimedPrize]([IdPrize], [IdUser], [ClaimDate], [Quantity], [Points], [LogUser], [LogDate]) VALUES(@IdPrize, @IdUser, @ClaimDate, @Quantity, @Points, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PiggyBank].[ClaimedPrize] SET [IdPrize] = @IdPrize, [IdUser] = @IdUser, [ClaimDate] = @ClaimDate, [Quantity] = @Quantity, [Points] = @Points, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PiggyBank].[ClaimedPrize] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  ClaimedPrize		
        /// </summary>
        /// <param name="Items">Business object of type ClaimedPrize para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEI.ClaimedPrize> Items)
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
        /// 	For use on data access layer at assembly level, return an  ClaimedPrize type object
        /// </summary>
        /// <param name="Id">Object Identifier ClaimedPrize</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClaimedPrize</returns>
        /// <remarks>
        /// </remarks>		
        internal BEI.ClaimedPrize ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ClaimedPrize de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ClaimedPrize</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ClaimedPrize</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEI.ClaimedPrize> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEI.ClaimedPrize> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PiggyBank].[ClaimedPrize] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEI.ClaimedPrize> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.PiggyBank.Prizes> lstPrizes = null;
			IEnumerable<BE.PiggyBank.User> lstUsers = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PiggyBank.relClaimedPrize.Prize))
				{
					using(var dal = new Prizes(Connection))
					{
						Keys = (from i in Items select i.IdPrize).Distinct();
						lstPrizes = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.PiggyBank.relClaimedPrize.User))
				{
					using(var dal = new User(Connection))
					{
						Keys = (from i in Items select i.IdUser).Distinct();
						lstUsers = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstPrizes != null)
					{
						Item.Prize = (from i in lstPrizes where i.Id == Item.IdPrize select i).FirstOrDefault();
					}					if (lstUsers != null)
					{
						Item.User = (from i in lstUsers where i.Id == Item.IdUser select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEI.ClaimedPrize Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.PiggyBank.relClaimedPrize.Prize))
				{
					using (var dal = new Prizes(Connection))
					{
						Item.Prize = dal.ReturnMaster(Item.IdPrize, Relations);
					}
				}				if (RelationEnum.Equals(BE.PiggyBank.relClaimedPrize.User))
				{
					using (var dal = new User(Connection))
					{
						Item.User = dal.ReturnMaster(Item.IdUser, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ClaimedPrize
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClaimedPrize</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.ClaimedPrize> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PiggyBank].[ClaimedPrize] ORDER By " + Order;
            IEnumerable<BEI.ClaimedPrize> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ClaimedPrize
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.ClaimedPrize> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PiggyBank].[ClaimedPrize] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEI.ClaimedPrize> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ClaimedPrize
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClaimedPrize</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.ClaimedPrize> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[ClaimedPrize] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEI.ClaimedPrize> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ClaimedPrize    	
        /// </summary>
        /// <param name="Id">Object identifier ClaimedPrize</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClaimedPrize</returns>
        /// <remarks>
        /// </remarks>    
        public BEI.ClaimedPrize Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[ClaimedPrize] WHERE [Id] = @Id";
            BEI.ClaimedPrize Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ClaimedPrize() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ClaimedPrize(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ClaimedPrize(SqlConnection connection) : base(connection) { }

        #endregion

    }
}