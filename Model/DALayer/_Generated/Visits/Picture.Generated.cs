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


namespace DALayer.Visits
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Visits
    /// Class     : Picture
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Picture 
    ///     for the service Visits.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Visits
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:52 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Picture : DALEntity<BEV.Picture>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Picture
        /// </summary>
        /// <param name="Item">Business object of type Picture </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEV.Picture Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Visits].[Picture]([Name], [Type], [IdPerson], [LogUser], [LogDate]) VALUES(@Name, @Type, @IdPerson, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Visits].[Picture] SET [Name] = @Name, [Type] = @Type, [IdPerson] = @IdPerson, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Visits].[Picture] WHERE [Id] = @Id";
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
        /// 	Saves a collection business information object of type  Picture		
        /// </summary>
        /// <param name="Items">Business object of type Picture para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEV.Picture> Items)
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
        /// 	For use on data access layer at assembly level, return an  Picture type object
        /// </summary>
        /// <param name="Id">Object Identifier Picture</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Picture</returns>
        /// <remarks>
        /// </remarks>		
        internal BEV.Picture ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Picture de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Picture</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Picture</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEV.Picture> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEV.Picture> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Visits].[Picture] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEV.Picture> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Visits.Person> lstPersons = null;

            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Visits.relPicture.Person))
				{
					using(var dal = new Person(Connection))
					{
						Keys = (from i in Items select i.IdPerson).Distinct();
						lstPersons = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstPersons != null)
					{
						Item.Person = (from i in lstPersons where i.Id == Item.IdPerson select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEV.Picture Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				if (RelationEnum.Equals(BE.Visits.relPicture.Person))
				{
					using (var dal = new Person(Connection))
					{
						Item.Person = dal.ReturnMaster(Item.IdPerson, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Picture
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Picture</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEV.Picture> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Visits].[Picture] ORDER By " + Order;
            IEnumerable<BEV.Picture> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Picture
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEV.Picture> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Visits].[Picture] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEV.Picture> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Picture
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Picture</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEV.Picture> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Visits].[Picture] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEV.Picture> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Picture    	
        /// </summary>
        /// <param name="Id">Object identifier Picture</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Picture</returns>
        /// <remarks>
        /// </remarks>    
        public BEV.Picture Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Visits].[Picture] WHERE [Id] = @Id";
            BEV.Picture Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Picture() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Picture(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Picture(SqlConnection connection) : base(connection) { }

        #endregion

    }
}