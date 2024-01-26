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


namespace DALayer.Sales
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Sales
    /// Class     : Transport
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Transport 
    ///     for the service Sales.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Sales
    /// </remarks>
    /// <history>
    ///     [DMC]   7/3/2022 18:16:47 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Transport : DALEntity<BEL.Transport>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Transport
        /// </summary>
        /// <param name="Item">Business object of type Transport </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEL.Transport Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Sales].[Transport]([DocNumber], [Date], [TransporterId], [SourceId], [DestinationId], [DeliveryTo], [Observations], [Weight], [QuantityPieces], [RemainingAmount], [TypeId], [WithCopies], [Sent], [LogUser], [LogDate]) VALUES(@DocNumber, @Date, @TransporterId, @SourceId, @DestinationId, @DeliveryTo, @Observations, @Weight, @QuantityPieces, @RemainingAmount, @TypeId, @WithCopies, @Sent, @LogUser, @LogDate) SELECT @@IDENTITY";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Sales].[Transport] SET [DocNumber] = @DocNumber, [Date] = @Date, [TransporterId] = @TransporterId, [SourceId] = @SourceId, [DestinationId] = @DestinationId, [DeliveryTo] = @DeliveryTo, [Observations] = @Observations, [Weight] = @Weight, [QuantityPieces] = @QuantityPieces, [RemainingAmount] = @RemainingAmount, [TypeId] = @TypeId, [WithCopies] = @WithCopies, [Sent] = @Sent, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Sales].[Transport] WHERE [Id] = @Id";
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
			if (Item.ListTransportDetails?.Count() > 0)
			{
				var list = Item.ListTransportDetails;
				foreach (var item in list) item.TransportId = itemId;
				using (var dal = new TransportDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTransportDetails = list;
			}
			if (Item.ListTransportSents?.Count() > 0)
			{
				var list = Item.ListTransportSents;
				foreach (var item in list) item.IdTransport = itemId;
				using (var dal = new TransportSent(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTransportSents = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Transport		
        /// </summary>
        /// <param name="Items">Business object of type Transport para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEL.Transport> Items)
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
        /// 	For use on data access layer at assembly level, return an  Transport type object
        /// </summary>
        /// <param name="Id">Object Identifier Transport</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Transport</returns>
        /// <remarks>
        /// </remarks>		
        internal BEL.Transport ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Transport de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Transport</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Transport</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEL.Transport> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEL.Transport> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Sales].[Transport] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEL.Transport> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Sales.TransportDetail> lstTransportDetails = null; 
			IEnumerable<BE.Sales.TransportSent> lstTransportSents = null; 
			IEnumerable<BE.Base.Classifier> lstDestinations = null;
			IEnumerable<BE.Base.Classifier> lstSources = null;
			IEnumerable<BE.Base.Classifier> lstTransporters = null;
			IEnumerable<BE.Base.Classifier> lstTypes = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Sales.relTransport.TransportDetails))
				{
					using (var dal = new TransportDetail(Connection))
					{
						lstTransportDetails = dal.List(Keys, "TransportId", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Sales.relTransport.TransportSents))
				{
					using (var dal = new TransportSent(Connection))
					{
						lstTransportSents = dal.List(Keys, "IdTransport", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Sales.relTransport.Destination))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.DestinationId).Distinct();
						lstDestinations = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Sales.relTransport.Source))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.SourceId).Distinct();
						lstSources = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Sales.relTransport.Transporter))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.TransporterId).Distinct();
						lstTransporters = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}				if (RelationEnum.Equals(BE.Sales.relTransport.Type))
				{
					using(var dal = new Base.Classifier(Connection))
					{
						Keys = (from i in Items select i.TypeId).Distinct();
						lstTypes = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstTransportDetails != null)
					{
						Item.ListTransportDetails = lstTransportDetails.Where(x => x.TransportId == Item.Id)?.ToList();
					}
					if (lstTransportSents != null)
					{
						Item.ListTransportSents = lstTransportSents.Where(x => x.IdTransport == Item.Id)?.ToList();
					}
					if (lstDestinations != null)
					{
						Item.Destination = (from i in lstDestinations where i.Id == Item.DestinationId select i).FirstOrDefault();
					}					if (lstSources != null)
					{
						Item.Source = (from i in lstSources where i.Id == Item.SourceId select i).FirstOrDefault();
					}					if (lstTransporters != null)
					{
						Item.Transporter = (from i in lstTransporters where i.Id == Item.TransporterId select i).FirstOrDefault();
					}					if (lstTypes != null)
					{
						Item.Type = (from i in lstTypes where i.Id == Item.TypeId select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEL.Transport Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Sales.relTransport.TransportDetails))
				{
					using (var dal = new TransportDetail(Connection))
					{
						Item.ListTransportDetails = dal.List(Keys, "TransportId", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Sales.relTransport.TransportSents))
				{
					using (var dal = new TransportSent(Connection))
					{
						Item.ListTransportSents = dal.List(Keys, "IdTransport", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Sales.relTransport.Destination))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.Destination = dal.ReturnMaster(Item.DestinationId, Relations);
					}
				}				if (RelationEnum.Equals(BE.Sales.relTransport.Source))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.Source = dal.ReturnMaster(Item.SourceId, Relations);
					}
				}				if (RelationEnum.Equals(BE.Sales.relTransport.Transporter))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.Transporter = dal.ReturnMaster(Item.TransporterId, Relations);
					}
				}				if (RelationEnum.Equals(BE.Sales.relTransport.Type))
				{
					using (var dal = new Base.Classifier(Connection))
					{
						Item.Type = dal.ReturnMaster(Item.TypeId, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Transport
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Transport</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.Transport> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Sales].[Transport] ORDER By " + Order;
            IEnumerable<BEL.Transport> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Transport
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.Transport> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Sales].[Transport] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEL.Transport> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Transport
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Transport</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEL.Transport> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[Transport] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEL.Transport> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Transport    	
        /// </summary>
        /// <param name="Id">Object identifier Transport</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Transport</returns>
        /// <remarks>
        /// </remarks>    
        public BEL.Transport Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Sales].[Transport] WHERE [Id] = @Id";
            BEL.Transport Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Transport() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Transport(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Transport(SqlConnection connection) : base(connection) { }

        #endregion

    }
}