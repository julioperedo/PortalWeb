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
using BEH = BEntities.HumanResources;
using BEI = BEntities.PiggyBank;
using BEN = BEntities.Campaign;

using DAL = DALayer.Product;

namespace BComponents.Product
{
	/// -----------------------------------------------------------------------------
	/// Project   : BComponents
	/// NameSpace : Product
	/// Class     : PriceGroupClient
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
	///   [DMC]   2/8/2023 11:58:00 Created
	/// </history>
	/// -----------------------------------------------------------------------------
	[Serializable()]
	public partial class PriceGroupClient : BCEntity
	{

		#region Search Methods 

		/// <summary>
		///     Search for business objects of type PriceGroupClient
		/// </summary>
		/// <param name="Id">Object identifier PriceGroupClient</param>
		/// <param name="Relations">relationship enumetators</param>
		/// <returns>An object of type PriceGroupClient</returns>
		/// <remarks>
		///     To get relationship objects, suply relationship enumetators
		/// </remarks>
		public BEP.PriceGroupClient Search(string CardCode, params Enum[] Relations)
		{
			BEP.PriceGroupClient Item = null;
			try
			{
				using (DAL.PriceGroupClient dal = new())
				{
					Item = dal.Search(CardCode, Relations);
				}
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
			return Item;
		}

		#endregion

		#region List Methods 

		/// <summary>
		///     Search for collection business objects of type PriceGroupClient
		/// </summary>
		/// <param name="Order">Property column to specify collection order</param>
		/// <param name="Relations">Relationship enumerator</param>
		/// <returns>Object collection of type PriceGroupClient</returns>
		/// <remarks>
		///     To get relationship objects, suply relationship enumetators
		/// </remarks>
		public IEnumerable<BEP.PriceGroupClient> List(string Order, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEP.PriceGroupClient> Items;
				using (DAL.PriceGroupClient dal = new())
				{
					Items = dal.List(Order, Relations);
				}
				return Items;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEP.PriceGroupClient> List(List<Field> FilterList, string Order, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEP.PriceGroupClient> Items = null;
				using (DAL.PriceGroupClient dal = new())
				{
					Items = dal.List(FilterList, Order, Relations);
				}
				return Items;
			}
			catch (Exception ex)
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
		/// <param name="Item">Object type PriceGroupClient</param>     
		/// <remarks>
		/// </remarks>
		public void Save(ref BEP.PriceGroupClient Item)
		{
			this.ErrorCollection.Clear();
			if (this.Validate(Item))
			{
				try
				{
					using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction())
					{
						using (DAL.PriceGroupClient dal = new())
						{
							dal.Save(ref Item);
						}
						BusinessTransaction.Complete();
					}
				}
				catch (Exception ex)
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
		///     Saves data object of type PriceGroupClient
		/// </summary>
		/// <param name="Items">Object type PriceGroupClient</param>
		/// <remarks>
		/// </remarks>
		public void Save(ref IList<BEP.PriceGroupClient> Items)
		{
			try
			{
				using (TransactionScope BusinessTransaction = base.GenerateBusinessTransaction())
				{
					using (DAL.PriceGroupClient dal = new())
					{
						dal.Save(ref Items);
					}
					BusinessTransaction.Complete();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		///     Validates a business object before save it 
		/// </summary>
		/// <param name="Item">Object Type PriceGroupClient</param>
		/// <returns>True: if object were validated</returns>
		/// <remarks>
		/// </remarks>
		internal bool Validate(BEP.PriceGroupClient Item)
		{
			bool bolOk = true;
			if (Item.StatusType != BE.StatusType.NoAction)
			{
				if (Item.StatusType != BE.StatusType.Insert)
				{

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
		public PriceGroupClient() : base() { }

		#endregion

	}
}