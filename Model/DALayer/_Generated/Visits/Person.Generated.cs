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
    /// Class     : Person
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Person 
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
    public partial class Person : DALEntity<BEV.Person>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Person
        /// </summary>
        /// <param name="Item">Business object of type Person </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEV.Person Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Visits].[Person]([Id], [DocumentId], [FirstName], [LastName], [Phone], [LogUser], [LogDate]) VALUES(@Id, @DocumentId, @FirstName, @LastName, @Phone, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Visits].[Person] SET [DocumentId] = @DocumentId, [FirstName] = @FirstName, [LastName] = @LastName, [Phone] = @Phone, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Visits].[Person] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Person", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListUserPersons?.Count() > 0)
			{
				var list = Item.ListUserPersons;
				foreach (var item in list) item.IdPerson = itemId;
				using (var dal = new DALayer.Security.UserPerson(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserPersons = list;
			}
			if (Item.ListPictures?.Count() > 0)
			{
				var list = Item.ListPictures;
				foreach (var item in list) item.IdPerson = itemId;
				using (var dal = new Picture(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPictures = list;
			}
			if (Item.ListVisits?.Count() > 0)
			{
				var list = Item.ListVisits;
				foreach (var item in list) item.VisitorId = itemId;
				using (var dal = new Visit(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVisits = list;
			}
			if (Item.ListVisitReceptions?.Count() > 0)
			{
				var list = Item.ListVisitReceptions;
				foreach (var item in list) item.VisitorId = itemId;
				using (var dal = new VisitReception(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVisitReceptions = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Person		
        /// </summary>
        /// <param name="Items">Business object of type Person para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEV.Person> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Person", quantity);
				currentId = lastId - quantity + 1;
				if (lastId <= 0) throw new Exception("No se puede generar el identificador " + this.GetType().FullName);
			}

            for (int i = 0; i < Items.Count; i++)
            {
                var Item = Items[i];
                if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = currentId++;
                Save(ref Item);
                Items[i] = Item;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 	For use on data access layer at assembly level, return an  Person type object
        /// </summary>
        /// <param name="Id">Object Identifier Person</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Person</returns>
        /// <remarks>
        /// </remarks>		
        internal BEV.Person ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Person de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Person</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Person</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEV.Person> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEV.Person> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Visits].[Person] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEV.Person> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Security.UserPerson> lstUserPersons = null; 
			IEnumerable<BE.Visits.Picture> lstPictures = null; 
			IEnumerable<BE.Visits.Visit> lstVisits = null; 
			IEnumerable<BE.Visits.VisitReception> lstVisitReceptions = null; 

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Visits.relPerson.UserPersons))
				{
					using (var dal = new DALayer.Security.UserPerson(Connection))
					{
						lstUserPersons = dal.List(Keys, "IdPerson", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Visits.relPerson.Pictures))
				{
					using (var dal = new Picture(Connection))
					{
						lstPictures = dal.List(Keys, "IdPerson", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Visits.relPerson.Visits))
				{
					using (var dal = new Visit(Connection))
					{
						lstVisits = dal.List(Keys, "VisitorId", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Visits.relPerson.VisitReceptions))
				{
					using (var dal = new VisitReception(Connection))
					{
						lstVisitReceptions = dal.List(Keys, "VisitorId", Relations);
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstUserPersons != null)
					{
						Item.ListUserPersons = lstUserPersons.Where(x => x.IdPerson == Item.Id)?.ToList();
					}
					if (lstPictures != null)
					{
						Item.ListPictures = lstPictures.Where(x => x.IdPerson == Item.Id)?.ToList();
					}
					if (lstVisits != null)
					{
						Item.ListVisits = lstVisits.Where(x => x.VisitorId == Item.Id)?.ToList();
					}
					if (lstVisitReceptions != null)
					{
						Item.ListVisitReceptions = lstVisitReceptions.Where(x => x.VisitorId == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEV.Person Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Visits.relPerson.UserPersons))
				{
					using (var dal = new DALayer.Security.UserPerson(Connection))
					{
						Item.ListUserPersons = dal.List(Keys, "IdPerson", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Visits.relPerson.Pictures))
				{
					using (var dal = new Picture(Connection))
					{
						Item.ListPictures = dal.List(Keys, "IdPerson", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Visits.relPerson.Visits))
				{
					using (var dal = new Visit(Connection))
					{
						Item.ListVisits = dal.List(Keys, "VisitorId", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Visits.relPerson.VisitReceptions))
				{
					using (var dal = new VisitReception(Connection))
					{
						Item.ListVisitReceptions = dal.List(Keys, "VisitorId", Relations)?.ToList();
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Person
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Person</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEV.Person> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Visits].[Person] ORDER By " + Order;
            IEnumerable<BEV.Person> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Person
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEV.Person> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Visits].[Person] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEV.Person> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Person
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Person</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEV.Person> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Visits].[Person] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEV.Person> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Person    	
        /// </summary>
        /// <param name="Id">Object identifier Person</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Person</returns>
        /// <remarks>
        /// </remarks>    
        public BEV.Person Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Visits].[Person] WHERE [Id] = @Id";
            BEV.Person Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Person() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Person(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Person(SqlConnection connection) : base(connection) { }

        #endregion

    }
}