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


namespace DALayer.Kbytes
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Kbytes
    /// Class     : Awards
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Awards 
    ///     for the service Kbytes.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Kbytes
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:37 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Awards : DALEntity<BEK.Awards>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Awards
        /// </summary>
        /// <param name="Item">Business object of type Awards </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.Awards Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[Awards]([Name], [Points], [LogUser], [LogDate]) VALUES(@Name, @Points, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[Awards] SET [Name] = @Name, [Points] = @Points, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[Awards] WHERE [Id] = @Id";
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
			if (Item.ListClaimedAwards?.Count() > 0)
			{
				var list = Item.ListClaimedAwards;
				foreach (var item in list) item.IdAward = itemId;
				using (var dal = new ClaimedAward(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListClaimedAwards = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Awards		
        /// </summary>
        /// <param name="Items">Business object of type Awards para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.Awards> Items)
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
        /// 	For use on data access layer at assembly level, return an  Awards type object
        /// </summary>
        /// <param name="Id">Object Identifier Awards</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Awards</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.Awards ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Awards de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Awards</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Awards</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.Awards> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.Awards> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[Awards] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.Awards> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.ClaimedAward> lstClaimedAwards = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Kbytes.relAwards.ClaimedAwards))
				{
					using (var dal = new ClaimedAward(Connection))
					{
						lstClaimedAwards = dal.List(Keys, "IdAward", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstClaimedAwards != null)
					{
						Item.ListClaimedAwards = lstClaimedAwards.Where(x => x.IdAward == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEK.Awards Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Kbytes.relAwards.ClaimedAwards))
				{
					using (var dal = new ClaimedAward(Connection))
					{
						Item.ListClaimedAwards = dal.List(Keys, "IdAward", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Awards
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Awards</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.Awards> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[Awards] ORDER By " + Order;
            IEnumerable<BEK.Awards> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Awards
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.Awards> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[Awards] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.Awards> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Awards
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Awards</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.Awards> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[Awards] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.Awards> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Awards    	
        /// </summary>
        /// <param name="Id">Object identifier Awards</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Awards</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.Awards Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[Awards] WHERE [Id] = @Id";
            BEK.Awards Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Awards() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Awards(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Awards(SqlConnection connection) : base(connection) { }

        #endregion

    }
}