using BEntities.Filters;
using DocumentFormat.OpenXml.ExtendedProperties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BEA = BEntities.SAP;
using DALH = DALayer.SAP.Hana;

namespace BComponents.SAP
{
	[Serializable()]
	public class Resume : BCEntity
	{
		#region Search Methods

		#endregion

		#region List Methods

		public IEnumerable<BEA.Resume> ResumeSaleByPeriod(DateTime InitialDate, DateTime FinalDate, List<string> Lines, List<string> Sellers, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Resume> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeSaleByPeriod(InitialDate, FinalDate, Lines, Sellers, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.ResumePeriod> ResumeSaleByPeriod2(DateTime InitialDate, DateTime FinalDate, List<string> Lines, List<string> Sellers)
		{
			try
			{
				IEnumerable<BEA.ResumePeriod> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeSaleByPeriod2(InitialDate, FinalDate, Lines, Sellers);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.ResumeBySeller> ResumeSaleBySeller(DateTime InitialDate, DateTime FinalDate)
		{
			try
			{
				IEnumerable<BEA.ResumeBySeller> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeSaleBySeller(InitialDate, FinalDate);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.ResumeBySeller> ResumeSaleBySellerByMonth(DateTime InitialDate, DateTime FinalDate)
		{
			try
			{
				IEnumerable<BEA.ResumeBySeller> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeSaleBySellerByMonth(InitialDate, FinalDate);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.ResumeItem> ResumeByPeriod(List<Field> FilterList, string Order)
		{
			try
			{
				IEnumerable<BEA.ResumeItem> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeByPeriod(FilterList);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Resume> ResumeStock(List<string> Lines, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Resume> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeStock(Lines, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Resume> ResumeStock(string Subsidiary, string Warehouse, string Division, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Resume> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeStock(Subsidiary, Warehouse, Division, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}


		public IEnumerable<BEA.Resume> AuthorizedOrders(List<string> Lines, List<string> Sellers, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Resume> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.AuthorizedOrders(Lines, Sellers, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Resume> OpenAmounts(List<string> Lines, List<string> Sellers, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Resume> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.OpenAmounts(Lines, Sellers, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.ResumeByClient> ResumeClientByPeriod(DateTime InitialDate, DateTime FinalDate, List<string> Lines, List<string> Sellers)
		{
			try
			{
				IEnumerable<BEA.ResumeByClient> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeClientByPeriod(InitialDate, FinalDate, Lines, Sellers);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		public IEnumerable<BEA.Resume> ResumeOpenDeliveryNotes(List<string> Lines, List<string> Sellers, params Enum[] Relations)
		{
			try
			{
				IEnumerable<BEA.Resume> BECollection = default;
				using (DALH.Resume DALObject = new())
				{
					BECollection = DALObject.ResumeOpenDeliveryNotes(Lines, Sellers, Relations);
				}
				return BECollection;
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
				return null;
			}
		}

		#endregion

		#region Constructors

		public Resume() : base() { }

		#endregion
	}
}
