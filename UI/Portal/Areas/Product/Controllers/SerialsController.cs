using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Portal.Controllers;
using BCA = BComponents.SAP;
using BEA = BEntities.SAP;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Portal.Areas.Product.Controllers
{
	[Area("Product")]
	[Authorize]
	public class SerialsController : BaseController
	{

		#region Constructores

		public SerialsController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { }

		#endregion

		#region GETs

		public IActionResult Index()
		{
			if (IsAllowed(this))
			{
				ViewBag.CardCode = CardCode;
				return View(HomeCardCode == CardCode ? "Index" : "IndexClient");
			}
			else
			{
				return RedirectToAction("Denied");
			}
		}

		public ActionResult Filter(string CardCode, string ItemCode, DateTime? InitDate, DateTime? FinalDate, string Serial, int? DocNumber)
		{
			string message = "";
			try
			{
				//if (HomeCardCode == base.CardCode)
				//{
				var items = GetItems(CardCode, ItemCode, InitDate, FinalDate, Serial, DocNumber);
				return Json(new { message, items });
				//}
				//else { 

				//}                
			}
			catch (Exception ex)
			{
				message = GetError(ex);
			}
			return Json(new { message });
		}

		public ActionResult ExportExcel(string CardCode, string ItemCode, DateTime? InitDate, DateTime? FinalDate, string Serial, int? DocNumber)
		{
			var lstItems = GetItems(CardCode, ItemCode, InitDate, FinalDate, Serial, DocNumber);
			using ExcelPackage objExcel = new();
			ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
			FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

			wsMain.Name = "Resumen Seriales";
			wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
			wsMain.Cells.Style.Fill.PatternColor.SetColor(System.Drawing.Color.White);
			wsMain.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
			wsMain.Cells.Style.Font.Size = 9;

			wsMain.Cells[4, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
			wsMain.Cells[1, 1].Value = "CONSULTA DE SERIALES";
			wsMain.Cells[1, 1].Style.Font.Size = 12;
			wsMain.Cells[1, 1].Style.Font.Bold = true;
			wsMain.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			wsMain.Cells[1, 1, 1, 11].Merge = true;

			var imgLogo = wsMain.Drawings.AddPicture("logo", logo);
			imgLogo.SetPosition(5, 5);

			wsMain.Cells[6, 1].Value = "Sucursal";
			wsMain.Cells[6, 2].Value = "Cod. CLiente";
			wsMain.Cells[6, 3].Value = "Cliente";
			wsMain.Cells[6, 4].Value = "F. Nota";
			wsMain.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			wsMain.Cells[6, 5].Value = "# Nota";
			wsMain.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			wsMain.Cells[6, 6].Value = "Tipo";

			wsMain.Cells[6, 1, 6, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
			wsMain.Cells[6, 1, 6, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DimGray);
			wsMain.Cells[6, 1, 6, 8].Style.Font.Color.SetColor(System.Drawing.Color.White);
			wsMain.Cells[6, 1, 6, 8].Style.Font.Bold = true;

			int intFila;

			if (lstItems != null && lstItems.Any())
			{
				intFila = 7;
				foreach (var item in lstItems)
				{
					wsMain.Cells[intFila, 1].Value = item.Subsidiary;
					wsMain.Cells[intFila, 2].Value = item.ClientCode;
					wsMain.Cells[intFila, 3].Value = item.ClientName;
					wsMain.Cells[intFila, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					wsMain.Cells[intFila, 4].Style.Numberformat.Format = "dd/MM/yyyy";
					wsMain.Cells[intFila, 4].Value = item.Date;
					wsMain.Cells[intFila, 5].Value = item.Number;
					wsMain.Cells[intFila, 6].Value = item.DocTypeDesc;
					intFila++;

					wsMain.Cells[intFila, 2].Value = "Cod. Producto";
					wsMain.Cells[intFila, 3, intFila, 5].Merge = true;
					wsMain.Cells[intFila, 3].Value = "Producto";
					wsMain.Cells[intFila, 7].Value = "Cantidad";
					wsMain.Cells[intFila, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					wsMain.Cells[intFila, 8].Value = "Seriales";
					wsMain.Cells[intFila, 2, intFila, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
					wsMain.Cells[intFila, 2, intFila, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
					wsMain.Cells[intFila, 2, intFila, 8].Style.Font.Bold = true;
					wsMain.Cells[intFila, 2, intFila + item.Items.Sum(i => i.Count), 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
					intFila++;

					foreach (var detail in item.Items)
					{
						wsMain.Cells[intFila, 2].Value = detail.ItemCode;
						wsMain.Cells[intFila, 3, intFila, 5].Merge = true;
						wsMain.Cells[intFila, 3].Value = detail.Name;
						wsMain.Cells[intFila, 7].Value = detail.Count;
						foreach (var serial in detail.Serials)
						{
							wsMain.Cells[intFila, 8].Value = serial;
							intFila++;
						}
						//intFila++;
					}
				}
			}

			wsMain.Cells.AutoFitColumns();
			//wsMain.Cells.Style.WrapText = true;
			wsMain.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
			wsMain.View.FreezePanes(7, 1);
			wsMain.PrinterSettings.LeftMargin = 0.2m;
			wsMain.PrinterSettings.RightMargin = 0.2m;
			wsMain.PrinterSettings.RepeatRows = wsMain.Cells["1:6"];
			wsMain.PrinterSettings.RepeatColumns = wsMain.Cells["A:H"];
			//.PrinterSettings.Orientation = eOrientation.Landscape

			string strFileName = "Resumen-Seriales-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".xlsx";
			byte[] objData = objExcel.GetAsByteArray();
			objExcel.Dispose();
			return File(objData, "application/xlsx", strFileName);
		}

		#endregion

		#region Private Methods

		private IEnumerable<Models.SaleNote> GetItems(string CardCode, string ItemCode, DateTime? InitDate, DateTime? FinalDate, string Serial, int? DocNumber)
		{
			IEnumerable<BEA.Serial> lstItems = new List<BEA.Serial>();
			BCA.Serial bcSerial = new();
			List<Field> lstFilters = new();
			if (InitDate.HasValue) lstFilters.Add(new Field("DocDate", InitDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
			if (FinalDate.HasValue) lstFilters.Add(new Field("DocDate", FinalDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
			if (!string.IsNullOrWhiteSpace(CardCode)) lstFilters.Add(new Field { Name = "ClientCode", Value = CardCode.Trim() });
			if (!string.IsNullOrWhiteSpace(ItemCode)) lstFilters.Add(new Field { Name = "ItemCode", Value = ItemCode.Trim(), Operator = Operators.Likes });
			if (!string.IsNullOrWhiteSpace(Serial)) lstFilters.Add(new Field("LOWER(SerialNumber)", Serial.Trim().ToLower(), Operators.Likes));
			if (DocNumber.HasValue) lstFilters.Add(new Field("DocNum", DocNumber.Value));

			CompleteFilters(ref lstFilters);
			lstItems = bcSerial.List(lstFilters, "DocDate, DocNum");

			var clients = GetClientsList("");

			var lstNotes = from i in lstItems
						   group i by new { i.Subsidiary, i.DocNum, i.DocDate, i.DocType, i.ClientCode, i.ClientName } into g
						   select new Models.SaleNote
						   {
							   Subsidiary = g.Key.Subsidiary,
							   ClientCode = g.Key.ClientCode,
							   ClientName = g.Key.ClientName,
							   Number = g.Key.DocNum,
							   Date = g.Key.DocDate,
							   DocType = g.Key.DocType,
							   DocTypeDesc = GetTypeDesc(g.Key.DocType),
							   IsOwn = clients.Any(c => c.CardCode == g.Key.ClientCode),
							   Items = (from d in g
										group d by new { d.ItemCode, d.ItemName } into sg
										select new Models.SaleNoteDetail
										{
											ItemCode = sg.Key.ItemCode,
											Name = sg.Key.ItemName,
											SerialsResume = string.Join("; ", (from sd in sg select sd.SerialNumber).ToArray()),
											Serials = (from sd in sg select sd.SerialNumber).ToList()
										}).ToList()
						   };
			return lstNotes;
		}

		private string GetTypeDesc(int Type)
		{
			string name = Type switch
			{
				13 => "Nota de Venta",
				14 => "Nota de Crédito",
				15 => "Nota de Entrega",
				18 => "Factura a Proveedores",
				19 => "Nota Crédito a Proveedores",
				_ => "Otro",
			};
			return name;
		}

		#endregion

	}
}