using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using Portal.Controllers;
using ReflectionMagic;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Areas.Reports.Controllers
{
	[Area("Reports")]
	[Authorize]
	public class PivotSalesController : BaseController
	{
		#region Variables Globales
		private readonly IConfiguration config;
		#endregion

		#region Constructores

		public PivotSalesController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { config = configuration; }

		#endregion

		#region GETs

		public IActionResult Index()
		{
			if (IsAllowed(this))
			{
				BCS.UserPivotConfig bcConfig = new();
				BES.UserPivotConfig beConfig = bcConfig.SearchByUser(UserCode);
				ViewBag.ConfigPivot = "";
				ViewBag.CardName = CardCode;

				int seeAllClientes = GetPermission("SeeAllClients"), seeMargin = GetPermission("VerMargen");
				BCS.UserData bcData = new();
				var data = bcData.SearchByUser(UserCode);
				ViewBag.Permission = new { SeeAllClients = seeAllClientes > 0, SeeMargin = seeMargin > 0, SellerCode = data?.SellerCode ?? "" };

				if (beConfig?.Id > 0)
				{
					ViewBag.ConfigPivot = $"{beConfig.Rows};{beConfig.Columns};{beConfig.Values}";
				}
				return View(CardCode == HomeCardCode ? "Index" : "IndexClient");
			}
			else
			{
				return RedirectToAction("Denied");
			}
		}

		public IActionResult GetSubcategories2(string Categories)
		{
			string message = "";
			IEnumerable<BEA.Item> items = Enumerable.Empty<BEA.Item>();
			try
			{
				BCA.Subcategory bcSubCategory = new();
				items = bcSubCategory.ListIn(Categories);
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message, items });
		}

		public IActionResult Filter(DateTime? InitialDate, DateTime? FinalDate, string Regionals, string Warehouses, string Categories, string SubCategories, string Lines, string Clients, string SalesMan, string ProductManager, string ItemCode)
		{
			string strMessage = "";
			IEnumerable<BEA.ResumeItem> lstItems = Enumerable.Empty<BEA.ResumeItem>();
			var options = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
			try
			{
				lstItems = GetItems(InitialDate, FinalDate, Regionals, Warehouses, Categories, SubCategories, Lines, Clients, SalesMan, ProductManager, ItemCode);
			}
			catch (Exception ex)
			{
				strMessage = GetError(ex);
			}
			if (HomeCardCode == CardCode)
			{
				int seeMargin = GetPermission("VerMargen");
				if (seeMargin > 0)
				{
					var lstResult = from i in lstItems
									select new
									{
										Sucursal = i.Subsidiary,
										Almacen = i.Warehouse,
										Linea = i.Line,
										Categoria = i.Category,
										Vendedor = i.Seller,
										GerenteProducto = i.ProductManager,
										Cliente = i.ClientName,
										Fecha = i.Date.ToString("yyyy-MM-dd"),
										Factura = i.BillNumber,
										i.ItemCode,
										Item = i.ItemName,
										Ano = i.Year,
										Mes = i.Month.ToString().PadLeft(2, '0'),
										Mes2 = i.Month.ToString().PadLeft(2, '0') + "-" + ToTitle(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i.Month)),
										Dia = i.Day,
										i.Total,
										Margen = i.Margin,
										TotalSinImpuestos = i.TaxlessTotal,
										MargenPorcentage = 0,
										Cantidad = i.Quantity
									};
					return Json(lstResult, options);
				}
				else
				{
					var lstResult = from i in lstItems
									select new
									{
										Sucursal = i.Subsidiary,
										Almacen = i.Warehouse,
										Linea = i.Line,
										Categoria = i.Category,
										Vendedor = i.Seller,
										GerenteProducto = i.ProductManager,
										Cliente = i.ClientName,
										Fecha = i.Date.ToString("yyyy-MM-dd"),
										Factura = i.BillNumber,
										i.ItemCode,
										Item = i.ItemName,
										Ano = i.Year,
										Mes = i.Month.ToString().PadLeft(2, '0'),
										Mes2 = i.Month.ToString().PadLeft(2, '0') + "-" + ToTitle(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i.Month)),
										Dia = i.Day,
										i.Total,
										Cantidad = i.Quantity
									};
					return Json(lstResult, options);
				}
			}
			else
			{
				var lstResult = from i in lstItems
								select new
								{
									Sucursal = i.Subsidiary,
									Almacen = i.Warehouse,
									Linea = i.Line,
									Categoria = i.Category,
									Vendedor = i.Seller,
									Fecha = i.Date.ToString("yyyy-MM-dd"),
									Factura = i.BillNumber,
									i.ItemCode,
									Item = i.ItemName,
									Ano = i.Year,
									Mes = i.Month.ToString().PadLeft(2, '0'),
									Mes2 = i.Month.ToString().PadLeft(2, '0') + "-" + ToTitle(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i.Month)),
									Dia = i.Day,
									i.Total,
									Cantidad = i.Quantity
								};
				return Json(lstResult, options);
			}
		}

		public IActionResult Export(DateTime? InitialDate, DateTime? FinalDate, string Regionals, string Warehouses, string Categories, string SubCategories, string Lines, string Clients, string SalesMan, string ProductManager, string ItemCode)
		{
			var lstItems = GetItems(InitialDate, FinalDate, Regionals, Warehouses, Categories, SubCategories, Lines, Clients, SalesMan, ProductManager, ItemCode);

			using ExcelPackage excel = new();
			bool localUser = CardCode == HomeCardCode;

			//create 2 WorkSheets. One for the source data and one for the Pivot table
			ExcelWorksheet wsPivot = excel.Workbook.Worksheets.Add("Pivot");
			ExcelWorksheet wsData = excel.Workbook.Worksheets.Add("Data");

			int column = 1, row = 1;
			wsData.Cells[row, column++].Value = "Sucursal";
			wsData.Cells[row, column++].Value = "Almacen";
			wsData.Cells[row, column++].Value = "Linea";
			wsData.Cells[row, column++].Value = "Categoria";
			wsData.Cells[row, column++].Value = "Vendedor";
			if (localUser) wsData.Cells[row, column++].Value = "GProducto";
			if (localUser) wsData.Cells[row, column++].Value = "Cliente";
			wsData.Cells[row, column++].Value = "Fecha";
			wsData.Cells[row, column++].Value = "Factura";
			wsData.Cells[row, column++].Value = "ItemCode";
			wsData.Cells[row, column++].Value = "Item";
			wsData.Cells[row, column++].Value = "Ano";
			wsData.Cells[row, column++].Value = "Mes";
			wsData.Cells[row, column++].Value = "Mes2";
			wsData.Cells[row, column++].Value = "Dia";
			wsData.Cells[row, column++].Value = "Total";
			if (localUser) wsData.Cells[row, column++].Value = "Margen";
			if (localUser) wsData.Cells[row, column++].Value = "TotalSI";
			wsData.Cells[row++, column++].Value = "Cantidad";

			foreach (var item in lstItems)
			{
				column = 1;
				wsData.Cells[row, column++].Value = item.Subsidiary;
				wsData.Cells[row, column++].Value = item.Warehouse;
				wsData.Cells[row, column++].Value = item.Line;
				wsData.Cells[row, column++].Value = item.Category;
				wsData.Cells[row, column++].Value = item.Seller;
				if (localUser) wsData.Cells[row, column++].Value = item.ProductManager;
				if (localUser) wsData.Cells[row, column++].Value = item.ClientName;
				wsData.Cells[row, column++].Value = item.Date.ToString("yyyy-MM-dd");
				wsData.Cells[row, column++].Value = item.BillNumber;
				wsData.Cells[row, column++].Value = item.ItemCode;
				wsData.Cells[row, column++].Value = item.ItemName;
				wsData.Cells[row, column++].Value = item.Year;
				wsData.Cells[row, column++].Value = item.Month.ToString().PadLeft(2, '0');
				wsData.Cells[row, column++].Value = item.Month.ToString().PadLeft(2, '0') + "-" + ToTitle(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month));
				wsData.Cells[row, column++].Value = item.Day;
				wsData.Cells[row, column++].Value = item.Total;
				if (localUser) wsData.Cells[row, column++].Value = item.Margin;
				if (localUser) wsData.Cells[row, column++].Value = item.TaxlessTotal;
				wsData.Cells[row++, column++].Value = item.Quantity;
			}

			var dataRange = wsData.Cells[wsData.Dimension.Address];

			//create the pivot table
			var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A1"], dataRange, "PivotTable");

			//label field
			pivotTable.RowFields.Add(pivotTable.Fields["Vendedor"]);
			pivotTable.DataOnRows = false;

			//data fields
			//var field = pivotTable.DataFields.Add(pivotTable.Fields["Column B"]);
			//field.Name = "Count of Column B";
			//field.Function = DataFieldFunctions.Count;

			var field = pivotTable.DataFields.Add(pivotTable.Fields["Total"]);
			field.Name = "Total";
			field.Function = DataFieldFunctions.Sum;
			field.Format = "#,##0.00";

			if (localUser)
			{
				var calcField = AddCalculatedField(pivotTable, "Margen%", "IF(TotalSI>0,(Margen/TotalSI)*100,0)");
				field = pivotTable.DataFields.Add(calcField);
				field.Function = DataFieldFunctions.Sum;
				field.Name = "Margen %";
				field.Format = "#,##0.00";
			}

			//field = pivotTable.DataFields.Add(pivotTable.Fields["Column D"]);
			//field.Name = "Sum of Column D";
			//field.Function = DataFieldFunctions.Sum;
			//field.Format = "€#,##0.00";

			pivotTable.ColumnFields.Add(pivotTable.Fields["Mes2"]);

			string strFileName = $"Pivot-Ventas-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
			byte[] objData = excel.GetAsByteArray();
			excel.Dispose();
			return File(objData, "application/xlsx", strFileName);
		}

		#endregion

		#region POSTs

		[HttpPost()]
		public IActionResult UpdateConfig(string Rows, string Columns, string Values)
		{
			string message = "";
			try
			{
				long IdUser = UserCode;
				BCS.UserPivotConfig bcConfig = new();
				BES.UserPivotConfig beConfig = bcConfig.SearchByUser(IdUser);
				if (beConfig != null && beConfig.Id > 0)
				{
					beConfig.StatusType = BEntities.StatusType.Update;
				}
				else
				{
					beConfig = new BES.UserPivotConfig { StatusType = BEntities.StatusType.Insert, IdUser = IdUser };
				}
				if (beConfig.Rows != Rows | beConfig.Columns != Columns | beConfig.Values != Values)
				{
					beConfig.Rows = Rows;
					beConfig.Columns = Columns;
					beConfig.Values = Values;
					beConfig.LogUser = IdUser;
					beConfig.LogDate = DateTime.Now;

					bcConfig.Save(ref beConfig);
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

		private IEnumerable<BEA.ResumeItem> GetItems(DateTime? InitialDate, DateTime? FinalDate, string Regionals, string Warehouses, string Categories, string SubCategories, string Lines, string Clients, string SalesMan, string ProductManager, string ItemCode)
		{
			BCA.Resume bcSale = new();
			List<Field> lstFilter = new();
			if (InitialDate.HasValue)
			{
				lstFilter.Add(new Field("Date", InitialDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
			}
			if (FinalDate.HasValue)
			{
				lstFilter.Add(new Field("Date", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
			}
			if (!string.IsNullOrEmpty(Regionals))
			{
				lstFilter.Add(new Field("LOWER(Subsidiary)", Regionals.ToLower(), Operators.In));
			}
			if (!string.IsNullOrEmpty(Warehouses))
			{
				lstFilter.Add(new Field("LOWER(Warehouse)", Warehouses.ToLower(), Operators.In));
			}
			if (!string.IsNullOrEmpty(Categories))
			{
				lstFilter.Add(new Field("LOWER(Category)", Categories.ToLower(), Operators.In));
			}
			if (!string.IsNullOrEmpty(SubCategories))
			{
				lstFilter.Add(new Field("LOWER(Subcategory)", SubCategories.ToLower(), Operators.In));
			}
			if (!string.IsNullOrEmpty(Lines))
			{
				BCP.Line bcLine = new();
				List<Field> tempFilters = new() { Field.New("Name", Lines, Operators.In) };
				var tempLines = bcLine.List(tempFilters, "1", BEP.relLine.LineDetails);
				if(tempLines.Any())
				{
					string sapLines = string.Join(",", tempLines.Select(x => string.Join(",", x.ListLineDetails.Select(y => $"'{y.SAPLine.ToLower()}'"))));
					lstFilter.Add(new Field("LOWER(Line)", sapLines, Operators.In));
				}				
			}
			if (!string.IsNullOrWhiteSpace(Clients))
			{
				lstFilter.Add(new Field("LOWER(CodCliente)", Clients.ToLower(), Operators.In));
			}
			if (!string.IsNullOrWhiteSpace(SalesMan))
			{
				lstFilter.Add(new Field("LOWER(Seller)", SalesMan.ToLower()));
			}
			if (!string.IsNullOrWhiteSpace(ProductManager))
			{
				lstFilter.Add(new Field("LOWER(ProductManager)", ProductManager.ToLower()));
			}
			if (!string.IsNullOrWhiteSpace(ItemCode))
			{
				lstFilter.AddRange(new[] { new Field("LOWER(ItemCode)", ItemCode.ToLower(), Operators.Likes), new Field("LOWER(ItemName)", ItemCode.ToLower(), Operators.Likes), new Field(LogicalOperators.Or) });
			}
			CompleteFilters(ref lstFilter);
			var lstItems = bcSale.ResumeByPeriod(lstFilter, "ClienteName") ?? new List<BEA.ResumeItem>();
			return lstItems;
		}

		private ExcelPivotTableField AddCalculatedField(ExcelPivotTable pivotTable, string calcFieldName, string formula)
		{
			var dynamicPivotTable = pivotTable.AsDynamic();
			var maxIndex = pivotTable.Fields.Max(f => f.Index);
			const string schemaMain = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";
			var cacheTopNode = dynamicPivotTable.CacheDefinition.CacheDefinitionXml.SelectSingleNode("//d:cacheFields", dynamicPivotTable.NameSpaceManager);
			cacheTopNode.SetAttribute("count", (int.Parse(cacheTopNode.GetAttribute("count")) + 1).ToString());
			var cacheFieldNode = dynamicPivotTable.CacheDefinition.CacheDefinitionXml.CreateElement("cacheField", schemaMain);
			cacheFieldNode.SetAttribute("name", calcFieldName);
			cacheFieldNode.SetAttribute("databaseField", "0");
			cacheFieldNode.SetAttribute("formula", formula);
			cacheFieldNode.SetAttribute("numFmtId", "0");
			cacheTopNode.AppendChild(cacheFieldNode);

			var topNode = dynamicPivotTable.PivotTableXml.SelectSingleNode("//d:pivotFields", dynamicPivotTable.NameSpaceManager);
			topNode.SetAttribute("count", (int.Parse(topNode.GetAttribute("count")) + 1).ToString());
			XmlElement fieldNode = dynamicPivotTable.PivotTableXml.CreateElement("pivotField", schemaMain);
			fieldNode.SetAttribute("compact", "0");
			fieldNode.SetAttribute("outline", "0");
			fieldNode.SetAttribute("showAll", "0");
			fieldNode.SetAttribute("defaultSubtotal", "0");
			topNode.AppendChild(fieldNode);

			var excelPivotTableFieldType = typeof(ExcelPivotTableField).AsDynamicType();
			var excelPivotTableField = excelPivotTableFieldType.New((XmlNamespaceManager)dynamicPivotTable.NameSpaceManager, fieldNode, (ExcelPivotTable)dynamicPivotTable, maxIndex + 1, maxIndex + 1);
			excelPivotTableField.SetCacheFieldNode(cacheFieldNode);
			dynamicPivotTable.Fields.AddInternal(excelPivotTableField);

			return pivotTable.Fields.First(f => f.Name == calcFieldName);
		}

		#endregion

	}
}