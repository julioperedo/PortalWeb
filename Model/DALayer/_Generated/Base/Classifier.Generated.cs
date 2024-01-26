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


namespace DALayer.Base
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Base
    /// Class     : Classifier
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Classifier 
    ///     for the service Base.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Base
    /// </remarks>
    /// <history>
    ///     [DMC]   17/10/2023 14:27:43 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Classifier : DALEntity<BEB.Classifier>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Classifier
        /// </summary>
        /// <param name="Item">Business object of type Classifier </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEB.Classifier Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Base].[Classifier]([Id], [Name], [Description], [Value], [isBase], [IdType], [LogUser], [LogDate]) VALUES(@Id, @Name, @Description, @Value, @isBase, @IdType, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Base].[Classifier] SET [Name] = @Name, [Description] = @Description, [Value] = @Value, [isBase] = @isBase, [IdType] = @IdType, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Base].[Classifier] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
				if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Classifier", 1);
				Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
			long itemId = Item.Id;
			if (Item.ListCharts?.Count() > 0)
			{
				var list = Item.ListCharts;
				foreach (var item in list) item.IdcChartType = itemId;
				using (var dal = new Chart(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListCharts = list;
			}
			if (Item.ListLicenses?.Count() > 0)
			{
				var list = Item.ListLicenses;
				foreach (var item in list) item.IdReason = itemId;
				using (var dal = new DALayer.HumanResources.License(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListLicenses = list;
			}
			if (Item.ListRequest_States?.Count() > 0)
			{
				var list = Item.ListRequest_States;
				foreach (var item in list) item.IdState = itemId;
				using (var dal = new DALayer.HumanResources.Request(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListRequest_States = list;
			}
			if (Item.ListSales?.Count() > 0)
			{
				var list = Item.ListSales;
				foreach (var item in list) item.StateIdc = itemId;
				using (var dal = new DALayer.Online.Sale(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSales = list;
			}
			if (Item.ListSaleDetails?.Count() > 0)
			{
				var list = Item.ListSaleDetails;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Online.SaleDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSaleDetails = list;
			}
			if (Item.ListSaleStates?.Count() > 0)
			{
				var list = Item.ListSaleStates;
				foreach (var item in list) item.StateIdc = itemId;
				using (var dal = new DALayer.Online.SaleState(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListSaleStates = list;
			}
			if (Item.ListTempSaleDetails?.Count() > 0)
			{
				var list = Item.ListTempSaleDetails;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Online.TempSaleDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTempSaleDetails = list;
			}
			if (Item.ListDocuments?.Count() > 0)
			{
				var list = Item.ListDocuments;
				foreach (var item in list) item.TypeIdc = itemId;
				using (var dal = new DALayer.Product.Document(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListDocuments = list;
			}
			if (Item.ListOpenBoxs?.Count() > 0)
			{
				var list = Item.ListOpenBoxs;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Product.OpenBox(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListOpenBoxs = list;
			}
			if (Item.ListOpenBoxHistorys?.Count() > 0)
			{
				var list = Item.ListOpenBoxHistorys;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Product.OpenBoxHistory(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListOpenBoxHistorys = list;
			}
			if (Item.ListPrices?.Count() > 0)
			{
				var list = Item.ListPrices;
				foreach (var item in list) item.IdSudsidiary = itemId;
				using (var dal = new DALayer.Product.Price(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPrices = list;
			}
			if (Item.ListPriceHistorys?.Count() > 0)
			{
				var list = Item.ListPriceHistorys;
				foreach (var item in list) item.IdSudsidiary = itemId;
				using (var dal = new DALayer.Product.PriceHistory(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPriceHistorys = list;
			}
			if (Item.ListPriceOffers?.Count() > 0)
			{
				var list = Item.ListPriceOffers;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Product.PriceOffer(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPriceOffers = list;
			}
			if (Item.ListPromoBannerItems?.Count() > 0)
			{
				var list = Item.ListPromoBannerItems;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Product.PromoBannerItem(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListPromoBannerItems = list;
			}
			if (Item.ListRequest_Subsidiarys?.Count() > 0)
			{
				var list = Item.ListRequest_Subsidiarys;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Product.Request(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListRequest_Subsidiarys = list;
			}
			if (Item.ListVolumePricings?.Count() > 0)
			{
				var list = Item.ListVolumePricings;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Product.VolumePricing(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVolumePricings = list;
			}
			if (Item.ListVolumePricingHistorys?.Count() > 0)
			{
				var list = Item.ListVolumePricingHistorys;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Product.VolumePricingHistory(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVolumePricingHistorys = list;
			}
			if (Item.ListQuoteDetailPricess?.Count() > 0)
			{
				var list = Item.ListQuoteDetailPricess;
				foreach (var item in list) item.IdSubsidiary = itemId;
				using (var dal = new DALayer.Sales.QuoteDetailPrices(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListQuoteDetailPricess = list;
			}
			if (Item.ListTransport_Destinations?.Count() > 0)
			{
				var list = Item.ListTransport_Destinations;
				foreach (var item in list) item.DestinationId = itemId;
				using (var dal = new DALayer.Sales.Transport(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTransport_Destinations = list;
			}
			if (Item.ListTransport_Sources?.Count() > 0)
			{
				var list = Item.ListTransport_Sources;
				foreach (var item in list) item.SourceId = itemId;
				using (var dal = new DALayer.Sales.Transport(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTransport_Sources = list;
			}
			if (Item.ListTransport_Transporters?.Count() > 0)
			{
				var list = Item.ListTransport_Transporters;
				foreach (var item in list) item.TransporterId = itemId;
				using (var dal = new DALayer.Sales.Transport(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTransport_Transporters = list;
			}
			if (Item.ListTransport_Types?.Count() > 0)
			{
				var list = Item.ListTransport_Types;
				foreach (var item in list) item.TypeId = itemId;
				using (var dal = new DALayer.Sales.Transport(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListTransport_Types = list;
			}
			if (Item.ListUserRequests?.Count() > 0)
			{
				var list = Item.ListUserRequests;
				foreach (var item in list) item.StateIdc = itemId;
				using (var dal = new DALayer.Security.UserRequest(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserRequests = list;
			}
			if (Item.ListUserRequestDetails?.Count() > 0)
			{
				var list = Item.ListUserRequestDetails;
				foreach (var item in list) item.StateIdc = itemId;
				using (var dal = new DALayer.Security.UserRequestDetail(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListUserRequestDetails = list;
			}
			if (Item.ListVisitReceptions?.Count() > 0)
			{
				var list = Item.ListVisitReceptions;
				foreach (var item in list) item.IdReason = itemId;
				using (var dal = new DALayer.Visits.VisitReception(Connection))
				{
					dal.Save(ref list);
				}
				Item.ListVisitReceptions = list;
			}
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Classifier		
        /// </summary>
        /// <param name="Items">Business object of type Classifier para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEB.Classifier> Items)
        {
			long lastId, currentId = 1;
			int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0); 
			if (quantity > 0)
			{
				lastId = GenID("Classifier", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Classifier type object
        /// </summary>
        /// <param name="Id">Object Identifier Classifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Classifier</returns>
        /// <remarks>
        /// </remarks>		
        internal BEB.Classifier ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Classifier de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Classifier</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Classifier</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEB.Classifier> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEB.Classifier> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Base].[Classifier] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEB.Classifier> Items, params Enum[] Relations)
        {
			IEnumerable<long> Keys;
			IEnumerable<BE.Base.Chart> lstCharts = null; 
			IEnumerable<BE.HumanResources.License> lstLicenses = null; 
			IEnumerable<BE.HumanResources.Request> lstRequest_States = null; 
			IEnumerable<BE.Online.Sale> lstSales = null; 
			IEnumerable<BE.Online.SaleDetail> lstSaleDetails = null; 
			IEnumerable<BE.Online.SaleState> lstSaleStates = null; 
			IEnumerable<BE.Online.TempSaleDetail> lstTempSaleDetails = null; 
			IEnumerable<BE.Product.Document> lstDocuments = null; 
			IEnumerable<BE.Product.OpenBox> lstOpenBoxs = null; 
			IEnumerable<BE.Product.OpenBoxHistory> lstOpenBoxHistorys = null; 
			IEnumerable<BE.Product.Price> lstPrices = null; 
			IEnumerable<BE.Product.PriceHistory> lstPriceHistorys = null; 
			IEnumerable<BE.Product.PriceOffer> lstPriceOffers = null; 
			IEnumerable<BE.Product.PromoBannerItem> lstPromoBannerItems = null; 
			IEnumerable<BE.Product.Request> lstRequest_Subsidiarys = null; 
			IEnumerable<BE.Product.VolumePricing> lstVolumePricings = null; 
			IEnumerable<BE.Product.VolumePricingHistory> lstVolumePricingHistorys = null; 
			IEnumerable<BE.Sales.QuoteDetailPrices> lstQuoteDetailPricess = null; 
			IEnumerable<BE.Sales.Transport> lstTransport_Destinations = null; 
			IEnumerable<BE.Sales.Transport> lstTransport_Sources = null; 
			IEnumerable<BE.Sales.Transport> lstTransport_Transporters = null; 
			IEnumerable<BE.Sales.Transport> lstTransport_Types = null; 
			IEnumerable<BE.Security.UserRequest> lstUserRequests = null; 
			IEnumerable<BE.Security.UserRequestDetail> lstUserRequestDetails = null; 
			IEnumerable<BE.Visits.VisitReception> lstVisitReceptions = null; 
			IEnumerable<BE.Base.ClassifierType> lstTypes = null;

            foreach (Enum RelationEnum in Relations)
            {
				Keys = from i in Items select i.Id;
				if (RelationEnum.Equals(BE.Base.relClassifier.Charts))
				{
					using (var dal = new Chart(Connection))
					{
						lstCharts = dal.List(Keys, "IdcChartType", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Licenses))
				{
					using (var dal = new DALayer.HumanResources.License(Connection))
					{
						lstLicenses = dal.List(Keys, "IdReason", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Request_States))
				{
					using (var dal = new DALayer.HumanResources.Request(Connection))
					{
						lstRequest_States = dal.List(Keys, "IdState", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Sales))
				{
					using (var dal = new DALayer.Online.Sale(Connection))
					{
						lstSales = dal.List(Keys, "StateIdc", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.SaleDetails))
				{
					using (var dal = new DALayer.Online.SaleDetail(Connection))
					{
						lstSaleDetails = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.SaleStates))
				{
					using (var dal = new DALayer.Online.SaleState(Connection))
					{
						lstSaleStates = dal.List(Keys, "StateIdc", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.TempSaleDetails))
				{
					using (var dal = new DALayer.Online.TempSaleDetail(Connection))
					{
						lstTempSaleDetails = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Documents))
				{
					using (var dal = new DALayer.Product.Document(Connection))
					{
						lstDocuments = dal.List(Keys, "TypeIdc", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.OpenBoxs))
				{
					using (var dal = new DALayer.Product.OpenBox(Connection))
					{
						lstOpenBoxs = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.OpenBoxHistorys))
				{
					using (var dal = new DALayer.Product.OpenBoxHistory(Connection))
					{
						lstOpenBoxHistorys = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Prices))
				{
					using (var dal = new DALayer.Product.Price(Connection))
					{
						lstPrices = dal.List(Keys, "IdSudsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.PriceHistorys))
				{
					using (var dal = new DALayer.Product.PriceHistory(Connection))
					{
						lstPriceHistorys = dal.List(Keys, "IdSudsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.PriceOffers))
				{
					using (var dal = new DALayer.Product.PriceOffer(Connection))
					{
						lstPriceOffers = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.PromoBannerItems))
				{
					using (var dal = new DALayer.Product.PromoBannerItem(Connection))
					{
						lstPromoBannerItems = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Request_Subsidiarys))
				{
					using (var dal = new DALayer.Product.Request(Connection))
					{
						lstRequest_Subsidiarys = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.VolumePricings))
				{
					using (var dal = new DALayer.Product.VolumePricing(Connection))
					{
						lstVolumePricings = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.VolumePricingHistorys))
				{
					using (var dal = new DALayer.Product.VolumePricingHistory(Connection))
					{
						lstVolumePricingHistorys = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.QuoteDetailPricess))
				{
					using (var dal = new DALayer.Sales.QuoteDetailPrices(Connection))
					{
						lstQuoteDetailPricess = dal.List(Keys, "IdSubsidiary", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Destinations))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						lstTransport_Destinations = dal.List(Keys, "DestinationId", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Sources))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						lstTransport_Sources = dal.List(Keys, "SourceId", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Transporters))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						lstTransport_Transporters = dal.List(Keys, "TransporterId", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Types))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						lstTransport_Types = dal.List(Keys, "TypeId", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.UserRequests))
				{
					using (var dal = new DALayer.Security.UserRequest(Connection))
					{
						lstUserRequests = dal.List(Keys, "StateIdc", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.UserRequestDetails))
				{
					using (var dal = new DALayer.Security.UserRequestDetail(Connection))
					{
						lstUserRequestDetails = dal.List(Keys, "StateIdc", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.VisitReceptions))
				{
					using (var dal = new DALayer.Visits.VisitReception(Connection))
					{
						lstVisitReceptions = dal.List(Keys, "IdReason", Relations);
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Type))
				{
					using(var dal = new ClassifierType(Connection))
					{
						Keys = (from i in Items select i.IdType).Distinct();
						lstTypes = dal.ReturnChild(Keys, Relations)?.ToList();
					}
				}
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
					if (lstCharts != null)
					{
						Item.ListCharts = lstCharts.Where(x => x.IdcChartType == Item.Id)?.ToList();
					}
					if (lstLicenses != null)
					{
						Item.ListLicenses = lstLicenses.Where(x => x.IdReason == Item.Id)?.ToList();
					}
					if (lstRequest_States != null)
					{
						Item.ListRequest_States = lstRequest_States.Where(x => x.IdState == Item.Id)?.ToList();
					}
					if (lstSales != null)
					{
						Item.ListSales = lstSales.Where(x => x.StateIdc == Item.Id)?.ToList();
					}
					if (lstSaleDetails != null)
					{
						Item.ListSaleDetails = lstSaleDetails.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstSaleStates != null)
					{
						Item.ListSaleStates = lstSaleStates.Where(x => x.StateIdc == Item.Id)?.ToList();
					}
					if (lstTempSaleDetails != null)
					{
						Item.ListTempSaleDetails = lstTempSaleDetails.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstDocuments != null)
					{
						Item.ListDocuments = lstDocuments.Where(x => x.TypeIdc == Item.Id)?.ToList();
					}
					if (lstOpenBoxs != null)
					{
						Item.ListOpenBoxs = lstOpenBoxs.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstOpenBoxHistorys != null)
					{
						Item.ListOpenBoxHistorys = lstOpenBoxHistorys.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstPrices != null)
					{
						Item.ListPrices = lstPrices.Where(x => x.IdSudsidiary == Item.Id)?.ToList();
					}
					if (lstPriceHistorys != null)
					{
						Item.ListPriceHistorys = lstPriceHistorys.Where(x => x.IdSudsidiary == Item.Id)?.ToList();
					}
					if (lstPriceOffers != null)
					{
						Item.ListPriceOffers = lstPriceOffers.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstPromoBannerItems != null)
					{
						Item.ListPromoBannerItems = lstPromoBannerItems.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstRequest_Subsidiarys != null)
					{
						Item.ListRequest_Subsidiarys = lstRequest_Subsidiarys.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstVolumePricings != null)
					{
						Item.ListVolumePricings = lstVolumePricings.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstVolumePricingHistorys != null)
					{
						Item.ListVolumePricingHistorys = lstVolumePricingHistorys.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstQuoteDetailPricess != null)
					{
						Item.ListQuoteDetailPricess = lstQuoteDetailPricess.Where(x => x.IdSubsidiary == Item.Id)?.ToList();
					}
					if (lstTransport_Destinations != null)
					{
						Item.ListTransport_Destinations = lstTransport_Destinations.Where(x => x.DestinationId == Item.Id)?.ToList();
					}
					if (lstTransport_Sources != null)
					{
						Item.ListTransport_Sources = lstTransport_Sources.Where(x => x.SourceId == Item.Id)?.ToList();
					}
					if (lstTransport_Transporters != null)
					{
						Item.ListTransport_Transporters = lstTransport_Transporters.Where(x => x.TransporterId == Item.Id)?.ToList();
					}
					if (lstTransport_Types != null)
					{
						Item.ListTransport_Types = lstTransport_Types.Where(x => x.TypeId == Item.Id)?.ToList();
					}
					if (lstUserRequests != null)
					{
						Item.ListUserRequests = lstUserRequests.Where(x => x.StateIdc == Item.Id)?.ToList();
					}
					if (lstUserRequestDetails != null)
					{
						Item.ListUserRequestDetails = lstUserRequestDetails.Where(x => x.StateIdc == Item.Id)?.ToList();
					}
					if (lstVisitReceptions != null)
					{
						Item.ListVisitReceptions = lstVisitReceptions.Where(x => x.IdReason == Item.Id)?.ToList();
					}
					if (lstTypes != null)
					{
						Item.Type = (from i in lstTypes where i.Id == Item.IdType select i).FirstOrDefault();
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
        protected override void LoadRelations(ref BEB.Classifier Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
				long[] Keys = new[] { Item.Id };
				if (RelationEnum.Equals(BE.Base.relClassifier.Charts))
				{
					using (var dal = new Chart(Connection))
					{
						Item.ListCharts = dal.List(Keys, "IdcChartType", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Licenses))
				{
					using (var dal = new DALayer.HumanResources.License(Connection))
					{
						Item.ListLicenses = dal.List(Keys, "IdReason", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Request_States))
				{
					using (var dal = new DALayer.HumanResources.Request(Connection))
					{
						Item.ListRequest_States = dal.List(Keys, "IdState", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Sales))
				{
					using (var dal = new DALayer.Online.Sale(Connection))
					{
						Item.ListSales = dal.List(Keys, "StateIdc", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.SaleDetails))
				{
					using (var dal = new DALayer.Online.SaleDetail(Connection))
					{
						Item.ListSaleDetails = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.SaleStates))
				{
					using (var dal = new DALayer.Online.SaleState(Connection))
					{
						Item.ListSaleStates = dal.List(Keys, "StateIdc", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.TempSaleDetails))
				{
					using (var dal = new DALayer.Online.TempSaleDetail(Connection))
					{
						Item.ListTempSaleDetails = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Documents))
				{
					using (var dal = new DALayer.Product.Document(Connection))
					{
						Item.ListDocuments = dal.List(Keys, "TypeIdc", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.OpenBoxs))
				{
					using (var dal = new DALayer.Product.OpenBox(Connection))
					{
						Item.ListOpenBoxs = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.OpenBoxHistorys))
				{
					using (var dal = new DALayer.Product.OpenBoxHistory(Connection))
					{
						Item.ListOpenBoxHistorys = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Prices))
				{
					using (var dal = new DALayer.Product.Price(Connection))
					{
						Item.ListPrices = dal.List(Keys, "IdSudsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.PriceHistorys))
				{
					using (var dal = new DALayer.Product.PriceHistory(Connection))
					{
						Item.ListPriceHistorys = dal.List(Keys, "IdSudsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.PriceOffers))
				{
					using (var dal = new DALayer.Product.PriceOffer(Connection))
					{
						Item.ListPriceOffers = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.PromoBannerItems))
				{
					using (var dal = new DALayer.Product.PromoBannerItem(Connection))
					{
						Item.ListPromoBannerItems = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Request_Subsidiarys))
				{
					using (var dal = new DALayer.Product.Request(Connection))
					{
						Item.ListRequest_Subsidiarys = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.VolumePricings))
				{
					using (var dal = new DALayer.Product.VolumePricing(Connection))
					{
						Item.ListVolumePricings = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.VolumePricingHistorys))
				{
					using (var dal = new DALayer.Product.VolumePricingHistory(Connection))
					{
						Item.ListVolumePricingHistorys = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.QuoteDetailPricess))
				{
					using (var dal = new DALayer.Sales.QuoteDetailPrices(Connection))
					{
						Item.ListQuoteDetailPricess = dal.List(Keys, "IdSubsidiary", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Destinations))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						Item.ListTransport_Destinations = dal.List(Keys, "DestinationId", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Sources))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						Item.ListTransport_Sources = dal.List(Keys, "SourceId", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Transporters))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						Item.ListTransport_Transporters = dal.List(Keys, "TransporterId", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Transport_Types))
				{
					using (var dal = new DALayer.Sales.Transport(Connection))
					{
						Item.ListTransport_Types = dal.List(Keys, "TypeId", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.UserRequests))
				{
					using (var dal = new DALayer.Security.UserRequest(Connection))
					{
						Item.ListUserRequests = dal.List(Keys, "StateIdc", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.UserRequestDetails))
				{
					using (var dal = new DALayer.Security.UserRequestDetail(Connection))
					{
						Item.ListUserRequestDetails = dal.List(Keys, "StateIdc", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.VisitReceptions))
				{
					using (var dal = new DALayer.Visits.VisitReception(Connection))
					{
						Item.ListVisitReceptions = dal.List(Keys, "IdReason", Relations)?.ToList();
					}
				}
				if (RelationEnum.Equals(BE.Base.relClassifier.Type))
				{
					using (var dal = new ClassifierType(Connection))
					{
						Item.Type = dal.ReturnMaster(Item.IdType, Relations);
					}
				}
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Classifier
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Classifier</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Classifier> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Base].[Classifier] ORDER By " + Order;
            IEnumerable<BEB.Classifier> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Classifier
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Classifier> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Base].[Classifier] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEB.Classifier> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Classifier
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Classifier</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEB.Classifier> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Classifier] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEB.Classifier> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Classifier    	
        /// </summary>
        /// <param name="Id">Object identifier Classifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Classifier</returns>
        /// <remarks>
        /// </remarks>    
        public BEB.Classifier Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Base].[Classifier] WHERE [Id] = @Id";
            BEB.Classifier Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Classifier() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Classifier(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Classifier(SqlConnection connection) : base(connection) { }

        #endregion

    }
}