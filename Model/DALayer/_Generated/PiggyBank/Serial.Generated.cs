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


namespace DALayer.PiggyBank
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : PiggyBank
    /// Class     : Serial
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Serial 
    ///     for the service PiggyBank.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service PiggyBank
    /// </remarks>
    /// <history>
    ///     [DMC]   26/7/2023 11:16:21 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Serial : DALEntity<BEI.Serial>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Serial
        /// </summary>
        /// <param name="Item">Business object of type Serial </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEI.Serial Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [PiggyBank].[Serial]([IdUser], [SerialNumber], [IsScanned], [RegisterDate], [State], [RejectReason], [CardCode], [CardName], [ItemCode], [ItemName], [Latitude], [Longitude], [Points], [LogUser], [LogDate]) VALUES(@IdUser, @SerialNumber, @IsScanned, @RegisterDate, @State, @RejectReason, @CardCode, @CardName, @ItemCode, @ItemName, @Latitude, @Longitude, @Points, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [PiggyBank].[Serial] SET [IdUser] = @IdUser, [SerialNumber] = @SerialNumber, [IsScanned] = @IsScanned, [RegisterDate] = @RegisterDate, [State] = @State, [RejectReason] = @RejectReason, [CardCode] = @CardCode, [CardName] = @CardName, [ItemCode] = @ItemCode, [ItemName] = @ItemName, [Latitude] = @Latitude, [Longitude] = @Longitude, [Points] = @Points, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [PiggyBank].[Serial] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  Serial		
        /// </summary>
        /// <param name="Items">Business object of type Serial para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEI.Serial> Items)
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
        /// 	For use on data access layer at assembly level, return an  Serial type object
        /// </summary>
        /// <param name="Id">Object Identifier Serial</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Serial</returns>
        /// <remarks>
        /// </remarks>		
        internal BEI.Serial ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Serial de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Serial</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Serial</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEI.Serial> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEI.Serial> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [PiggyBank].[Serial] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEI.Serial> Items, params Enum[] Relations)
        {
            IEnumerable<long> Keys;
            IEnumerable<BE.PiggyBank.User> lstUsers = null;

            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BE.PiggyBank.relSerial.User))
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
                    if (lstUsers != null)
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
        protected override void LoadRelations(ref BEI.Serial Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                if (RelationEnum.Equals(BE.PiggyBank.relSerial.User))
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
        /// 	Return an object Collection of Serial
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Serial</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.Serial> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [PiggyBank].[Serial] ORDER By " + Order;
            IEnumerable<BEI.Serial> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Serial
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.Serial> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [PiggyBank].[Serial] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEI.Serial> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Serial
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Serial</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEI.Serial> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[Serial] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEI.Serial> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Serial    	
        /// </summary>
        /// <param name="Id">Object identifier Serial</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Serial</returns>
        /// <remarks>
        /// </remarks>    
        public BEI.Serial Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [PiggyBank].[Serial] WHERE [Id] = @Id";
            BEI.Serial Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Serial() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Serial(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Serial(SqlConnection connection) : base(connection) { }

        #endregion

    }
}