using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;

namespace Portal.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize]
    public class PivotStockController : BaseController
    {
        #region Variables Globales
        private readonly IConfiguration config;
        #endregion

        #region Constructores

        public PivotStockController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env) { config = configuration; }

        #endregion

        #region GETs
        public IActionResult Index()
        {
            if (IsAllowed(this))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Denied");
            }
        }

        public ActionResult Filter(string Regionals, string Warehouses, string Categories, string Subcategories, string Lines, string ProductManagers)
        {
            string message = "";
            var options = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };
            try
            {
                var lstItems = GetItems(Regionals, Warehouses, Categories, Subcategories, Lines, ProductManagers);
                var items = lstItems.Select(i => new
                {
                    Sucursal = i.Subsidiary,
                    Almacen = i.Warehouse,
                    Item = i.ItemCode,
                    Descripcion = i.ItemName,
                    Categoria = i.Category,
                    Subcategoria = i.Subcategory,
                    GP = i.ProductManager,
                    Linea = i.Line,
                    i.Stock,
                    Costo = i.TotalReal,
                    Marca = i.Brand
                });
                return Json(new { message, items }, options);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Export(string Regionals, string Warehouses, string Categories, string Subcategories, string Lines, string ProductManagers)
        {
            var lstItems = GetItems(Regionals, Warehouses, Categories, Subcategories, Lines, ProductManagers);

            using (ExcelPackage excel = new ExcelPackage())
            {
                //create 2 WorkSheets. One for the source data and one for the Pivot table
                ExcelWorksheet wsPivot = excel.Workbook.Worksheets.Add("Pivot");
                ExcelWorksheet wsData = excel.Workbook.Worksheets.Add("Data");

                int column = 1, row = 1;
                wsData.Cells[row, column++].Value = "Sucursal";
                wsData.Cells[row, column++].Value = "Almacen";
                wsData.Cells[row, column++].Value = "Linea";
                wsData.Cells[row, column++].Value = "Marca";
                wsData.Cells[row, column++].Value = "Categoria";
                wsData.Cells[row, column++].Value = "Subcategoria";
                wsData.Cells[row, column++].Value = "GProducto";
                wsData.Cells[row, column++].Value = "ItemCode";
                wsData.Cells[row, column++].Value = "Item";
                wsData.Cells[row, column++].Value = "Stock";
                wsData.Cells[row++, column++].Value = "Costo";

                foreach (var item in lstItems)
                {
                    column = 1;
                    wsData.Cells[row, column++].Value = item.Subsidiary;
                    wsData.Cells[row, column++].Value = item.Warehouse;
                    wsData.Cells[row, column++].Value = item.Line;
                    wsData.Cells[row, column++].Value = item.Brand;
                    wsData.Cells[row, column++].Value = item.Category;
                    wsData.Cells[row, column++].Value = item.Subcategory;
                    wsData.Cells[row, column++].Value = item.ProductManager;
                    wsData.Cells[row, column++].Value = item.ItemCode;
                    wsData.Cells[row, column++].Value = item.ItemName;
                    wsData.Cells[row, column++].Value = item.Stock;
                    wsData.Cells[row++, column++].Value = item.TotalReal;
                }

                var dataRange = wsData.Cells[wsData.Dimension.Address];

                //create the pivot table
                var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A1"], dataRange, "PivotTable");

                //label field
                pivotTable.RowFields.Add(pivotTable.Fields["GProducto"]);
                pivotTable.RowFields.Add(pivotTable.Fields["Linea"]);
                pivotTable.RowFields.Add(pivotTable.Fields["Marca"]);
                pivotTable.RowFields.Add(pivotTable.Fields["Item"]);
                pivotTable.RowFields.Add(pivotTable.Fields["ItemCode"]);
                pivotTable.DataOnRows = false;

                var field = pivotTable.DataFields.Add(pivotTable.Fields["Stock"]);
                field.Name = "Stock";
                field.Function = DataFieldFunctions.Sum;
                field.Format = "#,##0.00";

                field = pivotTable.DataFields.Add(pivotTable.Fields["Costo"]);
                field.Function = DataFieldFunctions.Sum;
                field.Name = "Costo";
                field.Format = "#,##0.00";

                pivotTable.ColumnFields.Add(pivotTable.Fields["Sucursal"]);

                string strFileName = $"Pivot-Saldos-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
                byte[] objData = excel.GetAsByteArray();
                excel.Dispose();
                return File(objData, "application/xlsx", strFileName);
            }
        }

        #endregion

        #region POSTs

        #endregion

        #region Private Methods

        private IEnumerable<BEA.ProductStock> GetItems(string Regionals, string Warehouses, string Categories, string Subcategories, string Lines, string ProductManagers)
        {
            BCA.ProductStock bcInventory = new BCA.ProductStock();
            List<Field> lstFilter = new List<Field>();
            if (!string.IsNullOrWhiteSpace(Regionals))
            {
                lstFilter.Add(new Field("LOWER(Subsidiary)", Regionals.ToLower(), Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(Warehouses))
            {
                lstFilter.Add(new Field("LOWER(Warehouse)", Warehouses.ToLower(), Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(Lines))
            {
                lstFilter.Add(new Field("LOWER(Line)", Lines.ToLower(), Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(Categories))
            {
                lstFilter.Add(new Field("LOWER(Category)", Categories.ToLower(), Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(Subcategories))
            {
                lstFilter.Add(new Field("LOWER(Subcategory)", Subcategories.ToLower(), Operators.In));
            }
            if (!string.IsNullOrWhiteSpace(ProductManagers))
            {
                lstFilter.Add(new Field("LOWER(Productmanager)", ProductManagers.ToLower()));
            }
            CompleteFilters(ref lstFilter);
            IEnumerable<BEA.ProductStock> lstItems = bcInventory.ListBalance(lstFilter, "1");
            return lstItems;
        }

        #endregion

    }
}