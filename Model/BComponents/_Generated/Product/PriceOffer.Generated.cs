using BEntities.Filters;
using System;
using System.Collections.Generic;
using System.Transactions;
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

using DAL = DALayer.Product;

namespace BComponents.Product 
{
	/// -----------------------------------------------------------------------------
	/// Project   : BComponents
	/// NameSpace : Product
	/// Class     : PriceOffer
	/// Service  :  Product
	/// -----------------------------------------------------------------------------
	/// <summary>
	///    This business component validates business rules for business component Product 
	///    for the services Product
	/// </summary>
	/// <remarks>
	///    Business component for service Product
	/// </remarks>
	/// <history>
	///   [DMC]   10/1/2023 13:36:54 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PriceOffer : BCEntity 
	{

		#region Search Methods 

		/// <summary>
		///     Search for business objects of type PriceOffer
		/// </summary>
		/// <param name="Id">Object identifier PriceOffer</param>
		/// <param name="Relations">relationship enumetators</param>
		/// <returns>An object of type PriceOffer</returns>
		/// <remarks>
		///     To get relationship objects, suply relationship enumetators
		/// </remarks>
		public BEP.PriceOffer Search(long Id, params Enum[] Relations) 
		{
			BEP.PriceOffer Item = null;
			try 
			{
				using (DAL.PriceOffer dal = new()) 
				{
					Item = dal.Search(Id, Relations);
				}
			} 
			catch(Exception ex) 
			{
				base.ErrorHandler(ex);
			}
			return Item;
		}

		#endregion

		#region List Methods 

		/// <summary>
		///     Search for collection business objects of type PriceOffer
		/// </summary>
		/// <param name="Order">Property column to specify collection order</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>Object collection of type PriceOffer</returns>
		/// <remarks>
		///     To get relationship objects, suply relationship enumetators
		/// </remarks>
		public IEnumerable<BEP.PriceOffer> List(string Order, params Enum[] Relations) 
		{
			try 
			{
				IEnumerable<BEP.PriceOffer> Items;
				using (DAL.PriceOffer dal = new()) 
				{
					Items = dal.List(Order, Relations);
				}
				return Items;
			} 
			catch(Exception ex) 
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEP.PriceOffer> List(List<Field> FilterList, string Order, params Enum[] Relations) 
		{
			try 
			{
				IEnumerable<BEP.PriceOffer> Items = null;
				using (DAL.PriceOffer dal = new()) 
				{
					Items = dal.List(FilterList, Order, Relations);
				}
				return Items;
			} 
			catch(Exception ex) 
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		#endregion

		#region Save Methods 

		/// <summary>
		///     Saves data object of type 
		/// </summary>
		/// <param name="Item">Object type PriceOffer</param>     
		/// <remarks>
		/// </remarks>
		public void Save(ref BEP.PriceOffer Item) 
		{
			this.ErrorCollection.Clear();
			if (this.Validate(Item)) 
			{
				try 
				{
					using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
					{
						using(DAL.PriceOffer dal = new()) 
						{
							dal.Save(ref Item);
						}
						BusinessTransaction.Complete();
					}
				} 
				catch(Exception ex) 
				{
					base.ErrorHandler(ex);
				}
			} 
			else 
			{
				base.ErrorHandler(new BCException(this.ErrorCollection));
			}
		}

		/// <summary>
		///     Saves data object of type PriceOffer
		/// </summary>
		/// <param name="Items">Object type PriceOffer</param>
		/// <remarks>
		/// </remarks>
		public void Save(ref IList<BEP.PriceOffer> Items) 
		{
			try 
			{
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction()) 
				{
					using (DAL.PriceOffer dal = new()) 
					{
						dal.Save(ref Items);
					}
					BusinessTransaction.Complete();
				}
			} 
			catch(Exception ex) 
			{
				throw ex;
			}
		}

		/// <summary>
		///     Validates a business object before save it 
		/// </summary>
		/// <param name="Item">Object Type PriceOffer</param>
		/// <returns>True: if object were validated</returns>
		/// <remarks>
		/// </remarks>
		internal bool Validate(BEP.PriceOffer Item) 
		{
			bool bolOk = true;
			if (Item.StatusType != BE.StatusType.NoAction) 
			{
				if (Item.StatusType != BE.StatusType.Insert) 
				{
					if (Item.Id == 0) 
					{ 
						base.ErrorCollection.Add("No se ha proporcionado el Identificador"); 
						bolOk = false; 
					}

				}
			}
			return bolOk;
		}

		#endregion

		#region Constructors 

		/// <summary>
		///  Default Constructors 
		/// </summary>
		/// <remarks>
		/// </remarks>
		public PriceOffer() : base() { }

		#endregion

	}
}