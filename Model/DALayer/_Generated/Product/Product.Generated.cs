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


namespace DALayer.Product
{
    /// -----------------------------------------------------------------------------
    /// Project   : DALayer
    /// NameSpace : Product
    /// Class     : Product
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     This data access component saves business object information of type Product 
    ///     for the service Product.
    /// </summary>
    /// <remarks>
    ///     Data access layer for the service Product
    /// </remarks>
    /// <history>
    ///     [DMC]   15/2/2024 13:33:52 Created
    /// </history>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public partial class Product : DALEntity<BEP.Product>
    {

        #region Save Methods

        /// <summary>
        /// 	Saves business information object of type Product
        /// </summary>
        /// <param name="Item">Business object of type Product </param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref BEP.Product Item)
        {
            string strQuery = "";
            if (Item.StatusType == BE.StatusType.Insert)
            {
                strQuery = "INSERT INTO [Product].[Product]([Id], [Name], [Description], [Commentaries], [ItemCode], [Link], [Warranty], [Consumables], [ExtraComments], [Enabled], [ShowAlways], [ShowInWeb], [ShowTeamOnly], [ImageURL], [Line], [Category], [SubCategory], [Brand], [BrandCode], [Editable], [EnabledDate], [IsDigital], [Cost], [LogUser], [LogDate]) VALUES(@Id, @Name, @Description, @Commentaries, @ItemCode, @Link, @Warranty, @Consumables, @ExtraComments, @Enabled, @ShowAlways, @ShowInWeb, @ShowTeamOnly, @ImageURL, @Line, @Category, @SubCategory, @Brand, @BrandCode, @Editable, @EnabledDate, @IsDigital, @Cost, @LogUser, @LogDate)";
            }
            else if (Item.StatusType == BE.StatusType.Update)
            {
                strQuery = "UPDATE [Product].[Product] SET [Name] = @Name, [Description] = @Description, [Commentaries] = @Commentaries, [ItemCode] = @ItemCode, [Link] = @Link, [Warranty] = @Warranty, [Consumables] = @Consumables, [ExtraComments] = @ExtraComments, [Enabled] = @Enabled, [ShowAlways] = @ShowAlways, [ShowInWeb] = @ShowInWeb, [ShowTeamOnly] = @ShowTeamOnly, [ImageURL] = @ImageURL, [Line] = @Line, [Category] = @Category, [SubCategory] = @SubCategory, [Brand] = @Brand, [BrandCode] = @BrandCode, [Editable] = @Editable, [EnabledDate] = @EnabledDate, [IsDigital] = @IsDigital, [Cost] = @Cost, [LogUser] = @LogUser, [LogDate] = @LogDate WHERE [Id] = @Id";
            }
            else if (Item.StatusType == BE.StatusType.Delete)
            {
                strQuery = "DELETE FROM [Product].[Product] WHERE [Id] = @Id";
            }

            if (Item.StatusType != BE.StatusType.NoAction)
            {
                if (Item.StatusType == BE.StatusType.Insert & Item.Id <= 0) Item.Id = GenID("Product", 1);
                Connection.Execute(strQuery, Item);
                Item.StatusType = BE.StatusType.NoAction;
            }
            long itemId = Item.Id;
            if (Item.ListAcceleratorLots?.Count() > 0)
            {
                var list = Item.ListAcceleratorLots;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new DALayer.Kbytes.AcceleratorLot(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListAcceleratorLots = list;
            }
            if (Item.ListClientNoteDetails?.Count() > 0)
            {
                var list = Item.ListClientNoteDetails;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new DALayer.Kbytes.ClientNoteDetail(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListClientNoteDetails = list;
            }
            if (Item.ListSaleDetails?.Count() > 0)
            {
                var list = Item.ListSaleDetails;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new DALayer.Online.SaleDetail(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListSaleDetails = list;
            }
            if (Item.ListTempSaleDetails?.Count() > 0)
            {
                var list = Item.ListTempSaleDetails;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new DALayer.Online.TempSaleDetail(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListTempSaleDetails = list;
            }
            if (Item.ListDocuments?.Count() > 0)
            {
                var list = Item.ListDocuments;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new Document(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListDocuments = list;
            }
            if (Item.ListLoans?.Count() > 0)
            {
                var list = Item.ListLoans;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new Loan(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListLoans = list;
            }
            if (Item.ListOpenBoxs?.Count() > 0)
            {
                var list = Item.ListOpenBoxs;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new OpenBox(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListOpenBoxs = list;
            }
            if (Item.ListOpenBoxHistorys?.Count() > 0)
            {
                var list = Item.ListOpenBoxHistorys;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new OpenBoxHistory(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListOpenBoxHistorys = list;
            }
            if (Item.ListPrices?.Count() > 0)
            {
                var list = Item.ListPrices;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new Price(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListPrices = list;
            }
            if (Item.ListPriceExternalSites?.Count() > 0)
            {
                var list = Item.ListPriceExternalSites;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new PriceExternalSite(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListPriceExternalSites = list;
            }
            if (Item.ListPriceHistorys?.Count() > 0)
            {
                var list = Item.ListPriceHistorys;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new PriceHistory(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListPriceHistorys = list;
            }
            if (Item.ListPriceOffers?.Count() > 0)
            {
                var list = Item.ListPriceOffers;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new PriceOffer(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListPriceOffers = list;
            }
            if (Item.ListPromoBannerItems?.Count() > 0)
            {
                var list = Item.ListPromoBannerItems;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new PromoBannerItem(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListPromoBannerItems = list;
            }
            if (Item.ListPromoBannerTriggers?.Count() > 0)
            {
                var list = Item.ListPromoBannerTriggers;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new PromoBannerTrigger(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListPromoBannerTriggers = list;
            }
            if (Item.ListRelatedProduct_Products?.Count() > 0)
            {
                var list = Item.ListRelatedProduct_Products;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new RelatedProduct(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListRelatedProduct_Products = list;
            }
            if (Item.ListRelatedProduct_Relateds?.Count() > 0)
            {
                var list = Item.ListRelatedProduct_Relateds;
                foreach (var item in list) item.IdRelated = itemId;
                using (var dal = new RelatedProduct(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListRelatedProduct_Relateds = list;
            }
            if (Item.ListRequests?.Count() > 0)
            {
                var list = Item.ListRequests;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new Request(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListRequests = list;
            }
            if (Item.ListVolumePricings?.Count() > 0)
            {
                var list = Item.ListVolumePricings;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new VolumePricing(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListVolumePricings = list;
            }
            if (Item.ListVolumePricingHistorys?.Count() > 0)
            {
                var list = Item.ListVolumePricingHistorys;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new VolumePricingHistory(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListVolumePricingHistorys = list;
            }
            if (Item.ListQuoteDetails?.Count() > 0)
            {
                var list = Item.ListQuoteDetails;
                foreach (var item in list) item.IdProduct = itemId;
                using (var dal = new DALayer.Sales.QuoteDetail(Connection))
                {
                    dal.Save(ref list);
                }
                Item.ListQuoteDetails = list;
            }
        }

        /// <summary>
        /// 	Saves a collection business information object of type  Product		
        /// </summary>
        /// <param name="Items">Business object of type Product para Save</param>    
        /// <remarks>
        /// </remarks>
        public void Save(ref IList<BEP.Product> Items)
        {
            long lastId, currentId = 1;
            int quantity = Items.Count(i => i.StatusType == BE.StatusType.Insert & i.Id <= 0);
            if (quantity > 0)
            {
                lastId = GenID("Product", quantity);
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
        /// 	For use on data access layer at assembly level, return an  Product type object
        /// </summary>
        /// <param name="Id">Object Identifier Product</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Product</returns>
        /// <remarks>
        /// </remarks>		
        internal BEP.Product ReturnMaster(long Id, params Enum[] Relations)
        {
            return Search(Id, Relations);
        }

        /// <summary>
        /// 	Devuelve un objeto Product de tipo uno a uno con otro objeto
        /// </summary>
        /// <param name="Keys">Los identificadores de los objetos relacionados a Product</param>
        /// <param name="Relations">Enumerador de Relations a retorar</param>
        /// <returns>Un Objeto de tipo Product</returns>
        /// <remarks>
        /// </remarks>	
        internal IEnumerable<BEP.Product> ReturnChild(IEnumerable<long> Keys, params Enum[] Relations)
        {
            IEnumerable<BEP.Product> Items = null;
            if (Keys.Count() > 0)
            {
                string strQuery = $"SELECT * FROM [Product].[Product] WHERE Id IN ( {string.Join(",", Keys)} ) ";
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
        protected override void LoadRelations(ref IEnumerable<BEP.Product> Items, params Enum[] Relations)
        {
            IEnumerable<long> Keys;
            IEnumerable<BE.Kbytes.AcceleratorLot> lstAcceleratorLots = null;
            IEnumerable<BE.Kbytes.ClientNoteDetail> lstClientNoteDetails = null;
            IEnumerable<BE.Online.SaleDetail> lstSaleDetails = null;
            IEnumerable<BE.Online.TempSaleDetail> lstTempSaleDetails = null;
            IEnumerable<BE.Product.Document> lstDocuments = null;
            IEnumerable<BE.Product.Loan> lstLoans = null;
            IEnumerable<BE.Product.OpenBox> lstOpenBoxs = null;
            IEnumerable<BE.Product.OpenBoxHistory> lstOpenBoxHistorys = null;
            IEnumerable<BE.Product.Price> lstPrices = null;
            IEnumerable<BE.Product.PriceExternalSite> lstPriceExternalSites = null;
            IEnumerable<BE.Product.PriceHistory> lstPriceHistorys = null;
            IEnumerable<BE.Product.PriceOffer> lstPriceOffers = null;
            IEnumerable<BE.Product.PromoBannerItem> lstPromoBannerItems = null;
            IEnumerable<BE.Product.PromoBannerTrigger> lstPromoBannerTriggers = null;
            IEnumerable<BE.Product.RelatedProduct> lstRelatedProduct_Products = null;
            IEnumerable<BE.Product.RelatedProduct> lstRelatedProduct_Relateds = null;
            IEnumerable<BE.Product.Request> lstRequests = null;
            IEnumerable<BE.Product.VolumePricing> lstVolumePricings = null;
            IEnumerable<BE.Product.VolumePricingHistory> lstVolumePricingHistorys = null;
            IEnumerable<BE.Sales.QuoteDetail> lstQuoteDetails = null;

            foreach (Enum RelationEnum in Relations)
            {
                Keys = from i in Items select i.Id;
                if (RelationEnum.Equals(BE.Product.relProduct.AcceleratorLots))
                {
                    using (var dal = new DALayer.Kbytes.AcceleratorLot(Connection))
                    {
                        lstAcceleratorLots = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.ClientNoteDetails))
                {
                    using (var dal = new DALayer.Kbytes.ClientNoteDetail(Connection))
                    {
                        lstClientNoteDetails = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.SaleDetails))
                {
                    using (var dal = new DALayer.Online.SaleDetail(Connection))
                    {
                        lstSaleDetails = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.TempSaleDetails))
                {
                    using (var dal = new DALayer.Online.TempSaleDetail(Connection))
                    {
                        lstTempSaleDetails = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Documents))
                {
                    using (var dal = new Document(Connection))
                    {
                        lstDocuments = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Loans))
                {
                    using (var dal = new Loan(Connection))
                    {
                        lstLoans = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.OpenBoxs))
                {
                    using (var dal = new OpenBox(Connection))
                    {
                        lstOpenBoxs = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.OpenBoxHistorys))
                {
                    using (var dal = new OpenBoxHistory(Connection))
                    {
                        lstOpenBoxHistorys = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Prices))
                {
                    using (var dal = new Price(Connection))
                    {
                        lstPrices = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PriceExternalSites))
                {
                    using (var dal = new PriceExternalSite(Connection))
                    {
                        lstPriceExternalSites = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PriceHistorys))
                {
                    using (var dal = new PriceHistory(Connection))
                    {
                        lstPriceHistorys = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PriceOffers))
                {
                    using (var dal = new PriceOffer(Connection))
                    {
                        lstPriceOffers = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PromoBannerItems))
                {
                    using (var dal = new PromoBannerItem(Connection))
                    {
                        lstPromoBannerItems = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PromoBannerTriggers))
                {
                    using (var dal = new PromoBannerTrigger(Connection))
                    {
                        lstPromoBannerTriggers = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.RelatedProduct_Products))
                {
                    using (var dal = new RelatedProduct(Connection))
                    {
                        lstRelatedProduct_Products = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.RelatedProduct_Relateds))
                {
                    using (var dal = new RelatedProduct(Connection))
                    {
                        lstRelatedProduct_Relateds = dal.List(Keys, "IdRelated", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Requests))
                {
                    using (var dal = new Request(Connection))
                    {
                        lstRequests = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.VolumePricings))
                {
                    using (var dal = new VolumePricing(Connection))
                    {
                        lstVolumePricings = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.VolumePricingHistorys))
                {
                    using (var dal = new VolumePricingHistory(Connection))
                    {
                        lstVolumePricingHistorys = dal.List(Keys, "IdProduct", Relations);
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.QuoteDetails))
                {
                    using (var dal = new DALayer.Sales.QuoteDetail(Connection))
                    {
                        lstQuoteDetails = dal.List(Keys, "IdProduct", Relations);
                    }
                }
            }

            if (Relations.GetLength(0) > 0)
            {
                foreach (var Item in Items)
                {
                    if (lstAcceleratorLots != null)
                    {
                        Item.ListAcceleratorLots = lstAcceleratorLots.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstClientNoteDetails != null)
                    {
                        Item.ListClientNoteDetails = lstClientNoteDetails.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstSaleDetails != null)
                    {
                        Item.ListSaleDetails = lstSaleDetails.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstTempSaleDetails != null)
                    {
                        Item.ListTempSaleDetails = lstTempSaleDetails.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstDocuments != null)
                    {
                        Item.ListDocuments = lstDocuments.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstLoans != null)
                    {
                        Item.ListLoans = lstLoans.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstOpenBoxs != null)
                    {
                        Item.ListOpenBoxs = lstOpenBoxs.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstOpenBoxHistorys != null)
                    {
                        Item.ListOpenBoxHistorys = lstOpenBoxHistorys.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstPrices != null)
                    {
                        Item.ListPrices = lstPrices.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstPriceExternalSites != null)
                    {
                        Item.ListPriceExternalSites = lstPriceExternalSites.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstPriceHistorys != null)
                    {
                        Item.ListPriceHistorys = lstPriceHistorys.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstPriceOffers != null)
                    {
                        Item.ListPriceOffers = lstPriceOffers.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstPromoBannerItems != null)
                    {
                        Item.ListPromoBannerItems = lstPromoBannerItems.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstPromoBannerTriggers != null)
                    {
                        Item.ListPromoBannerTriggers = lstPromoBannerTriggers.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstRelatedProduct_Products != null)
                    {
                        Item.ListRelatedProduct_Products = lstRelatedProduct_Products.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstRelatedProduct_Relateds != null)
                    {
                        Item.ListRelatedProduct_Relateds = lstRelatedProduct_Relateds.Where(x => x.IdRelated == Item.Id)?.ToList();
                    }
                    if (lstRequests != null)
                    {
                        Item.ListRequests = lstRequests.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstVolumePricings != null)
                    {
                        Item.ListVolumePricings = lstVolumePricings.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstVolumePricingHistorys != null)
                    {
                        Item.ListVolumePricingHistorys = lstVolumePricingHistorys.Where(x => x.IdProduct == Item.Id)?.ToList();
                    }
                    if (lstQuoteDetails != null)
                    {
                        Item.ListQuoteDetails = lstQuoteDetails.Where(x => x.IdProduct == Item.Id)?.ToList();
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
        protected override void LoadRelations(ref BEP.Product Item, params Enum[] Relations)
        {
            foreach (Enum RelationEnum in Relations)
            {
                long[] Keys = new[] { Item.Id };
                if (RelationEnum.Equals(BE.Product.relProduct.AcceleratorLots))
                {
                    using (var dal = new DALayer.Kbytes.AcceleratorLot(Connection))
                    {
                        Item.ListAcceleratorLots = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.ClientNoteDetails))
                {
                    using (var dal = new DALayer.Kbytes.ClientNoteDetail(Connection))
                    {
                        Item.ListClientNoteDetails = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.SaleDetails))
                {
                    using (var dal = new DALayer.Online.SaleDetail(Connection))
                    {
                        Item.ListSaleDetails = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.TempSaleDetails))
                {
                    using (var dal = new DALayer.Online.TempSaleDetail(Connection))
                    {
                        Item.ListTempSaleDetails = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Documents))
                {
                    using (var dal = new Document(Connection))
                    {
                        Item.ListDocuments = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Loans))
                {
                    using (var dal = new Loan(Connection))
                    {
                        Item.ListLoans = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.OpenBoxs))
                {
                    using (var dal = new OpenBox(Connection))
                    {
                        Item.ListOpenBoxs = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.OpenBoxHistorys))
                {
                    using (var dal = new OpenBoxHistory(Connection))
                    {
                        Item.ListOpenBoxHistorys = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Prices))
                {
                    using (var dal = new Price(Connection))
                    {
                        Item.ListPrices = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PriceExternalSites))
                {
                    using (var dal = new PriceExternalSite(Connection))
                    {
                        Item.ListPriceExternalSites = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PriceHistorys))
                {
                    using (var dal = new PriceHistory(Connection))
                    {
                        Item.ListPriceHistorys = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PriceOffers))
                {
                    using (var dal = new PriceOffer(Connection))
                    {
                        Item.ListPriceOffers = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PromoBannerItems))
                {
                    using (var dal = new PromoBannerItem(Connection))
                    {
                        Item.ListPromoBannerItems = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.PromoBannerTriggers))
                {
                    using (var dal = new PromoBannerTrigger(Connection))
                    {
                        Item.ListPromoBannerTriggers = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.RelatedProduct_Products))
                {
                    using (var dal = new RelatedProduct(Connection))
                    {
                        Item.ListRelatedProduct_Products = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.RelatedProduct_Relateds))
                {
                    using (var dal = new RelatedProduct(Connection))
                    {
                        Item.ListRelatedProduct_Relateds = dal.List(Keys, "IdRelated", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.Requests))
                {
                    using (var dal = new Request(Connection))
                    {
                        Item.ListRequests = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.VolumePricings))
                {
                    using (var dal = new VolumePricing(Connection))
                    {
                        Item.ListVolumePricings = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.VolumePricingHistorys))
                {
                    using (var dal = new VolumePricingHistory(Connection))
                    {
                        Item.ListVolumePricingHistorys = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
                if (RelationEnum.Equals(BE.Product.relProduct.QuoteDetails))
                {
                    using (var dal = new DALayer.Sales.QuoteDetail(Connection))
                    {
                        Item.ListQuoteDetails = dal.List(Keys, "IdProduct", Relations)?.ToList();
                    }
                }
            }
        }

        #endregion

        #region List Methods

        /// <summary>
        /// 	Return an object Collection of Product
        /// </summary>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Product</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Product> List(string Order, params Enum[] Relations)
        {
            string strQuery = "SELECT * FROM [Product].[Product] ORDER By " + Order;
            IEnumerable<BEP.Product> Items = SQLList(strQuery, Relations);
            return Items;
        }

        /// <summary>
        /// 	Return an object Collection of Product
        /// </summary>
        /// <param name="FilterList">Filter List </param>
        /// <param name="Order">Object order property column </param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type ClassifierType</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Product> List(List<Field> FilterList, string Order, params Enum[] Relations)
        {
            StringBuilder sbQuery = new StringBuilder();
            string filter = GetFilterString(FilterList?.ToArray());

            sbQuery.AppendLine("SELECT   * ");
            sbQuery.AppendLine("FROM    [Product].[Product] ");
            if (filter != "") sbQuery.AppendLine($"WHERE   {filter} ");
            sbQuery.AppendLine($"ORDER By {Order}");

            IEnumerable<BEP.Product> Items = SQLList(sbQuery.ToString(), Relations);
            return Items;
        }

        /// <summary>
        ///     Return an object Collection of Product
        /// </summary>
        /// <param name="Keys">Object Identifier</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>A Collection of type Product</returns>
        /// <remarks>
        /// </remarks>
        public IEnumerable<BEP.Product> List(IEnumerable<long> Keys, string Column, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[Product] WHERE {Column} IN ( {string.Join(",", Keys)} ) ";
            IEnumerable<BEP.Product> Items = SQLList(strQuery, Relations);
            return Items;
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 	Search an object of type Product    	
        /// </summary>
        /// <param name="Id">Object identifier Product</param>
        /// <param name="Relations">Relationship enumerator</param>
        /// <returns>An object of type Product</returns>
        /// <remarks>
        /// </remarks>    
        public BEP.Product Search(long Id, params Enum[] Relations)
        {
            string strQuery = $"SELECT * FROM [Product].[Product] WHERE [Id] = @Id";
            BEP.Product Item = SQLSearch(strQuery, new { @Id = Id }, Relations);
            return Item;
        }

        #endregion

        #region Constructors

        public Product() : base() { }

        /// <summary>
        /// El constructor por defecto que crear una instancia de la base de datos 
        /// utilizando el Factory Pattern
        /// </summary>
        /// <remarks>
        ///  La instancia de la Base de datos se pasa al constructor
        ///	</remarks>   
        public Product(string ConnectionName) : base(ConnectionName) { }

        /// <summary>
        /// Constructor que crea la instancia del la base de datos utilizando
        /// el Factory pattern
        /// </summary>
        /// <remarks></remarks>
        internal Product(SqlConnection connection) : base(connection) { }

        #endregion

    }
}