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
    /// Class     : AcceleratorLotExcluded
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type AcceleratorLotExcluded 
    ///     for the service Kbytes.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Kbytes
    /// </remarks>
    /// <history>
    ///     [DMC]   8/2/2024 16:31:05 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class AcceleratorLotExcluded : DALEntity<BEK.AcceleratorLotExcluded>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type AcceleratorLotExcluded
        /// </summary>
        /// <param name="Item">Business object of type AcceleratorLotExcluded </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEK.AcceleratorLotExcluded Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Kbytes].[AcceleratorLotExcluded]([IdAccelerator], [CardCode], [LogUser], [LogDate]) VALUES(@IdAccelerator, @CardCode, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Kbytes].[AcceleratorLotExcluded] SET [IdAccelerator] = @IdAccelerator, [CardCode] = @CardCode, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Kbytes].[AcceleratorLotExcluded] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  AcceleratorLotExcluded		
        /// </summary>
        /// <param name="Items">Business object of type AcceleratorLotExcluded para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEK.AcceleratorLotExcluded> Items)
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
        /// 	For use on data access layer at assembly level, return an  AcceleratorLotExcluded type object
        /// </summary>
        /// <param name="Id">Object Identifier AcceleratorLotExcluded</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type AcceleratorLotExcluded</returns>
        /// <remarks>
        /// </remarks>		
        internal BEK.AcceleratorLotExcluded ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto AcceleratorLotExcluded de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a AcceleratorLotExcluded</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo AcceleratorLotExcluded</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEK.AcceleratorLotExcluded> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEK.AcceleratorLotExcluded> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Kbytes].[AcceleratorLotExcluded] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEK.AcceleratorLotExcluded> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Kbytes.AcceleratorLot> lstAccelerators = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relAcceleratorLotExcluded.Accelerator))
				{
					using(var dal = new AcceleratorLot(Connection))
					{
						Keys = (from i in Items select i.IdAccelerator).Distinct();
						lstAccelerators = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstAccelerators != null)
					{
						Item.Accelerator = (from i in lstAccelerators where i.Id == Item.IdAccelerator select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEK.AcceleratorLotExcluded Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Kbytes.relAcceleratorLotExcluded.Accelerator))
				{
					using (var dal = new AcceleratorLot(Connection))
					{
						Item.Accelerator = dal.ReturnMaster(Item.IdAccelerator, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of AcceleratorLotExcluded
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type AcceleratorLotExcluded</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.AcceleratorLotExcluded> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Kbytes].[AcceleratorLotExcluded] ORDER By " + Order;
            IEnumerable<BEK.AcceleratorLotExcluded> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of AcceleratorLotExcluded
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.AcceleratorLotExcluded> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Kbytes].[AcceleratorLotExcluded] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEK.AcceleratorLotExcluded> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of AcceleratorLotExcluded
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type AcceleratorLotExcluded</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEK.AcceleratorLotExcluded> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[AcceleratorLotExcluded] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEK.AcceleratorLotExcluded> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type AcceleratorLotExcluded    	
        /// </summary>
        /// <param name="Id">Object identifier AcceleratorLotExcluded</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type AcceleratorLotExcluded</returns>
        /// <remarks>
        /// </remarks>    
        public BEK.AcceleratorLotExcluded Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Kbytes].[AcceleratorLotExcluded] WHERE [Id] = @Id";
            BEK.AcceleratorLotExcluded Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public AcceleratorLotExcluded() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public AcceleratorLotExcluded(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal AcceleratorLotExcluded(SqlConnection connection) : base(connection) { }

        #endregion

    }
}