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


namespace DALayer.Kbytes
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Kbytes
    /// Class     : ClaimedAward
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type ClaimedAward 
    ///     for the service Kbytes.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Kbytes
    /// </remarks>
    /// <history>
    ///     [DMC]   2/2/2024 14:27:47 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class ClaimedAward : DALEntity<BEK.ClaimedAward>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type ClaimedAward
        /// </summary>
        /// <param name="Item">Business object of type ClaimedAward </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.ClaimedAward Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[ClaimedAward]([IdAward], [CardCode], [ClaimDate], [Quantity], [Points], [LogUser], [LogDate]) VALUES(@IdAward, @CardCode, @ClaimDate, @Quantity, @Points, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[ClaimedAward] SET [IdAward] = @IdAward, [CardCode] = @CardCode, [ClaimDate] = @ClaimDate, [Quantity] = @Quantity, [Points] = @Points, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[ClaimedAward] WHERE [Id] = @Id";
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
			if (Item.ListStatusDetails?.Count() > 0)
			{
				var list = Item.ListStatusDetails;
				foreach (var item in list) item.IdAward = itemId;
				using (var dal = new StatusDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListStatusDetails = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  ClaimedAward		
        /// </summary>
        /// <param name="Items">Business object of type ClaimedAward para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.ClaimedAward> Items)
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
        /// 	For use on data access layer at assembly level, return an  ClaimedAward type object
        /// </summary>
        /// <param name="Id">Object Identifier ClaimedAward</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClaimedAward</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.ClaimedAward ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto ClaimedAward de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a ClaimedAward</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo ClaimedAward</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.ClaimedAward> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.ClaimedAward> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[ClaimedAward] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.ClaimedAward> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.StatusDetail> lstStatusDetails = null; 
			IEnumerable<BE.Kbytes.Awards> lstAwards = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Kbytes.relClaimedAward.StatusDetails))
				{
					using (var dal = new StatusDetail(Connection))
					{
						lstStatusDetails = dal.List(Keys, "IdAward", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relClaimedAward.Award))
				{
					using(var dal = new Awards(Connection))
					{
						Keys = (from i in Items select i.IdAward).Distinct();
						lstAwards = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstStatusDetails != null)
					{
						Item.ListStatusDetails = lstStatusDetails.Where(x => x.IdAward == Item.Id)?.ToList();
					}
					if (lstAwards != null)
					{
						Item.Award = (from i in lstAwards where i.Id == Item.IdAward select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEK.ClaimedAward Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Kbytes.relClaimedAward.StatusDetails))
				{
					using (var dal = new StatusDetail(Connection))
					{
						Item.ListStatusDetails = dal.List(Keys, "IdAward", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Kbytes.relClaimedAward.Award))
				{
					using (var dal = new Awards(Connection))
					{
						Item.Award = dal.ReturnMaster(Item.IdAward, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of ClaimedAward
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClaimedAward</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClaimedAward> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[ClaimedAward] ORDER By " + Order;
            IEnumerable<BEK.ClaimedAward> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of ClaimedAward
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClaimedAward> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[ClaimedAward] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.ClaimedAward> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of ClaimedAward
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClaimedAward</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.ClaimedAward> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClaimedAward] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.ClaimedAward> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type ClaimedAward    	
        /// </summary>
        /// <param name="Id">Object identifier ClaimedAward</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type ClaimedAward</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.ClaimedAward Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[ClaimedAward] WHERE [Id] = @Id";
            BEK.ClaimedAward Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public ClaimedAward() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public ClaimedAward(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal ClaimedAward(SqlConnection connection) : base(connection) { }

        #endregion

    }
}