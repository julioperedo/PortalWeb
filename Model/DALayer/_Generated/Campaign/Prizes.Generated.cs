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


namespace DALayer.Campaign
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Campaign
    /// Class     : Prizes
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Prizes 
    ///     for the service Campaign.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Campaign
    /// </remarks>
    /// <history>
    ///     [DMC]   28/7/2023 15:42:24 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Prizes : DALEntity<BEN.Prizes>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Prizes
        /// </summary>
        /// <param name="Item">Business object of type Prizes </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEN.Prizes Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Campaign].[Prizes]([Name], [Description], [ImageUrl], [Enabled], [Points], [IdCampaign], [LogUser], [LogDate]) VALUES(@Name, @Description, @ImageUrl, @Enabled, @Points, @IdCampaign, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Campaign].[Prizes] SET [Name] = @Name, [Description] = @Description, [ImageUrl] = @ImageUrl, [Enabled] = @Enabled, [Points] = @Points, [IdCampaign] = @IdCampaign, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Campaign].[Prizes] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  Prizes		
        /// </summary>
        /// <param name="Items">Business object of type Prizes para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEN.Prizes> Items)
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
        /// 	For use on data access layer at assembly level, return an  Prizes type object
        /// </summary>
        /// <param name="Id">Object Identifier Prizes</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Prizes</returns>
        /// <remarks>
        /// </remarks>		
        internal BEN.Prizes ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Prizes de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Prizes</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Prizes</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEN.Prizes> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEN.Prizes> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Campaign].[Prizes] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEN.Prizes> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Campaign.Info> lstCampaigns = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Campaign.relPrizes.Campaign))
				{
					using(var dal = new Info(Connection))
					{
						Keys = (from i in Items select i.IdCampaign).Distinct();
						lstCampaigns = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstCampaigns != null)
					{
						Item.Campaign = (from i in lstCampaigns where i.Id == Item.IdCampaign select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEN.Prizes Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Campaign.relPrizes.Campaign))
				{
					using (var dal = new Info(Connection))
					{
						Item.Campaign = dal.ReturnMaster(Item.IdCampaign, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Prizes
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Prizes</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEN.Prizes> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Campaign].[Prizes] ORDER By " + Order;
            IEnumerable<BEN.Prizes> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Prizes
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEN.Prizes> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Campaign].[Prizes] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEN.Prizes> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Prizes
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Prizes</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEN.Prizes> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Campaign].[Prizes] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEN.Prizes> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Prizes    	
        /// </summary>
        /// <param name="Id">Object identifier Prizes</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Prizes</returns>
        /// <remarks>
        /// </remarks>    
        public BEN.Prizes Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Campaign].[Prizes] WHERE [Id] = @Id";
            BEN.Prizes Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Prizes() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Prizes(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Prizes(SqlConnection connection) : base(connection) { }

        #endregion

    }
}