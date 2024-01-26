using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BEA = BEntities.SAP;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Controllers
{
	[Area("Product")]
	[Authorize]
	public class ArrivalController : BaseController
	{
		#region Global Variables

		#endregion

		#region Constructors

		public ArrivalController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

		#endregion

		#region GETs

		public IActionResult Index()
		{
			if (IsAllowed(this))
			{
				ViewBag.IsLocal = HomeCardCode == CardCode ? "Y" : "N";
				return View();
			}
			else
			{
				return RedirectToAction("Denied");
			}
		}

		public IActionResult Filter(string Filter)
		{
			string message = "";
			try
			{
				bool isLocal = CardCode == HomeCardCode;
				BCA.ProductStock bcStock = new();
				List<Field> filters = new() { Field.New(@"IFNULL(ArrivalDate, '')", "", Operators.Different), Field.New("Stock", 0, Operators.HigherThan), Field.New("Warehouse", "TRANSITO", Operators.Likes) };
				if (!string.IsNullOrEmpty(Filter)) filters.AddRange(new[] { Field.New("ItemCode", Filter, Operators.Likes), Field.New("ItemName", Filter, Operators.Likes), Field.LogicalOr() });
				CompleteFilters(ref filters);
				var stock = bcStock.ListBalance(filters, "1");
				if (stock?.Count() > 0)
				{
					string codes = string.Join(",", stock.Select(x => $"'{x.ItemCode}'").Distinct());

					BCP.Product bcProduct = new();
					var products = bcProduct.ListWithPrices3(codes, "1");

					var items = from x in stock
								where isLocal || products.Any(p => p.ItemCode.ToLower() == x.ItemCode.ToLower())
								select new { x.ItemCode, x.ItemName, x.Line, x.Category, x.Subcategory, Stock = isLocal || x.Stock <= 50 ? x.Stock.ToString() : "+50", Date = x.Date.ToString("yyyy-MM-dd"), ArrivalDate = x.ArrivalDate.Value.ToString("yyyy-MM-dd") };
					return Json(new { message, items });
				}
				else
				{
					return Json(new { message, items = new List<string>() });
				}
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message });
		}

		#endregion

		#region Private Methods

		#endregion
	}
}
