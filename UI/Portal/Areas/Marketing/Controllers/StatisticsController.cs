using BEntities.Filters;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.Table.PivotTable;
using OfficeOpenXml;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BCY = BComponents.PiggyBank;
using BEY = BEntities.PiggyBank;

namespace Portal.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Authorize]
    public class StatisticsController : BaseController
    {
        #region Constructors

        public StatisticsController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Filter()
        {
            string message = "";
            try
            {
                BCY.Serial bcSerial = new();
                List<Field> filters = new() { new Field("State", "V") };
                var serials = bcSerial.List(filters, "1", BEY.relSerial.User);
                var items = serials.Select(x => new { x.Id, x.IdUser, x.RegisterDate.Date, Quarter = GetQuarter(x.RegisterDate), x.CardCode, x.CardName, x.ItemCode, x.ItemName, x.Points, UserName = x.User.Name, x.User.StoreName, City = ToTitle(x.User.City.Trim()), x.SerialNumber, x.User.Address, x.User.EMail, x.User.Phone });

                BCY.Prizes bcPrize = new();
                var prizes = bcPrize.List("1");
                int minPrize = prizes?.Where(x => x.Enabled).Min(x => x.Points) ?? 0;

                BCY.ClaimedPrize bcClaimedPrize = new();
                var claimedPrizes  = bcClaimedPrize.List("1", BEY.relClaimedPrize.Prize, BEY.relClaimedPrize.User);
                var claimed = claimedPrizes.Select(x => new { x.Id, x.IdPrize, x.IdUser, x.ClaimDate.Date, x.Quantity, x.Points, UserName = x.User.Name, x.User.StoreName, City = ToTitle(x.User.City.Trim()), x.User.Address, x.User.EMail, x.User.Phone, PrizeName = x.Prize.Name, Quarter = GetQuarter(x.ClaimDate) });

                return Json(new { message, items, minPrize, claimed });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult Export()
        {
            BCY.Serial bcSerial = new();
            List<Field> filters = new() { new Field("State", "V") };
            var lstItems = bcSerial.List(filters, "1", BEY.relSerial.User);

            BCY.ClaimedPrize bcClaimedPrize = new();
            var claimedPrizes = bcClaimedPrize.List("1", BEY.relClaimedPrize.Prize, BEY.relClaimedPrize.User);

            using ExcelPackage excel = new();
            bool localUser = CardCode == HomeCardCode;

            //create 2 WorkSheets. One for the source data and one for the Pivot table
            ExcelWorksheet wsPivot = excel.Workbook.Worksheets.Add("Pivot");
            ExcelWorksheet wsData = excel.Workbook.Worksheets.Add("Data");

            int column = 1, row = 1;
            wsData.Cells[row, column++].Value = "Serial";
            wsData.Cells[row, column++].Value = "Ciudad";
            wsData.Cells[row, column++].Value = "Usuario";
            wsData.Cells[row, column++].Value = "CodCliente";
            wsData.Cells[row, column++].Value = "Cliente";
            wsData.Cells[row, column++].Value = "Fecha";
            wsData.Cells[row, column++].Value = "Puntos";
            //wsData.Cells[row, column++].Value = "Usado";
            //wsData.Cells[row, column++].Value = "TotalPuntos";
            wsData.Cells[row, column++].Value = "CodProducto";
            wsData.Cells[row, column++].Value = "Producto";
            wsData.Cells[row, column++].Value = "Tienda";
            wsData.Cells[row, column++].Value = "Direccion";
            wsData.Cells[row, column++].Value = "Correo";
            wsData.Cells[row, column++].Value = "Fecha";
            wsData.Cells[row, column++].Value = "Año";
            wsData.Cells[row, column++].Value = "Trimestre";
            wsData.Cells[row, column++].Value = "Mes";
            wsData.Cells[row, column++].Value = "Dia";
            wsData.Cells[row++, column++].Value = "Telefono";

            foreach (var item in lstItems)
            {
                column = 1;
                wsData.Cells[row, column++].Value = item.SerialNumber;
                wsData.Cells[row, column++].Value = ToTitle(item.User.City.Trim());
                wsData.Cells[row, column++].Value = ToTitle(item.User.Name);
                wsData.Cells[row, column++].Value = item.CardCode.ToUpper();
                wsData.Cells[row, column++].Value = item.CardName;
                wsData.Cells[row, column++].Value = item.RegisterDate.ToString("yyyy-MM-dd");
                wsData.Cells[row, column++].Value = item.Points;
                wsData.Cells[row, column++].Value = 0;
                wsData.Cells[row, column++].Value = item.Points;
                wsData.Cells[row, column++].Value = item.ItemCode.ToUpper();
                wsData.Cells[row, column++].Value = item.ItemName;
                wsData.Cells[row, column++].Value = ToTitle(item.User.StoreName);
                wsData.Cells[row, column++].Value = ToTitle(item.User.Address);
                wsData.Cells[row, column++].Value = ToTitle(item.User.EMail);
                wsData.Cells[row, column++].Value = item.RegisterDate.Date;
                wsData.Cells[row, column++].Value = item.RegisterDate.Year;
                wsData.Cells[row, column++].Value = $"Q{GetQuarter(item.RegisterDate)}";
                wsData.Cells[row, column++].Value = item.RegisterDate.Month.ToString("D2");
                wsData.Cells[row, column++].Value = item.RegisterDate.Day.ToString("D2");
                wsData.Cells[row++, column++].Value = ToTitle(item.User.Phone);
            }

            foreach (var item in claimedPrizes)
            {
                column = 1;
                wsData.Cells[row, column++].Value = "";
                wsData.Cells[row, column++].Value = ToTitle(item.User.City.Trim());
                wsData.Cells[row, column++].Value = ToTitle(item.User.Name);
                wsData.Cells[row, column++].Value = "";
                wsData.Cells[row, column++].Value = "";
                wsData.Cells[row, column++].Value = item.ClaimDate.ToString("yyyy-MM-dd");
                wsData.Cells[row, column++].Value = 0;
                wsData.Cells[row, column++].Value = item.Points;
                wsData.Cells[row, column++].Value = -item.Points;
                wsData.Cells[row, column++].Value = "";
                wsData.Cells[row, column++].Value = "";
                wsData.Cells[row, column++].Value = ToTitle(item.User.StoreName);
                wsData.Cells[row, column++].Value = ToTitle(item.User.Address);
                wsData.Cells[row, column++].Value = ToTitle(item.User.EMail);
                wsData.Cells[row, column++].Value = item.ClaimDate.Date;
                wsData.Cells[row, column++].Value = item.ClaimDate.Year;
                wsData.Cells[row, column++].Value = $"Q{GetQuarter(item.ClaimDate)}";
                wsData.Cells[row, column++].Value = item.ClaimDate.Month.ToString("D2");
                wsData.Cells[row, column++].Value = item.ClaimDate.Day.ToString("D2");
                wsData.Cells[row++, column++].Value = ToTitle(item.User.Phone);
            }

            var dataRange = wsData.Cells[wsData.Dimension.Address];

            //create the pivot table
            var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A1"], dataRange, "PivotTable");

            //label field
            pivotTable.RowFields.Add(pivotTable.Fields["Ciudad"]);
            pivotTable.RowFields.Add(pivotTable.Fields["Usuario"]);
            pivotTable.DataOnRows = false;

            var fieldPoints = pivotTable.DataFields.Add(pivotTable.Fields["Puntos"]);
            fieldPoints.Name = "Puntos";
            fieldPoints.Function = DataFieldFunctions.Sum;
            fieldPoints.Format = "#,##0";

            //var fieldSerial = pivotTable.DataFields.Add(pivotTable.Fields["Serial"]);
            //fieldSerial.Name = "Serial";
            //fieldSerial.Function = DataFieldFunctions.Count;
            //fieldSerial.Format = "#,##0";

            //var fieldUsed = pivotTable.DataFields.Add(pivotTable.Fields["Usado"]);
            //fieldUsed.Name = "Usado";
            //fieldUsed.Function = DataFieldFunctions.Count;
            //fieldUsed.Format = "#,##0";

            //var fieldTotal = pivotTable.DataFields.Add(pivotTable.Fields["TotalPuntos"]);
            //fieldTotal.Name = "TotalPuntos";
            //fieldTotal.Function = DataFieldFunctions.Count;
            //fieldTotal.Format = "#,##0";

            //pivotTable.ColumnFields.Add(pivotTable.Fields["Serial"]);

            string strFileName = $"Pivot-Alcancia-{DateTime.Now:yyyyMMdd-HHmm}.xlsx";
            byte[] objData = excel.GetAsByteArray();
            excel.Dispose();
            return File(objData, "application/xlsx", strFileName);
        }

        #endregion

        #region Private Methods

        private int GetQuarter(DateTime Item) => (Item.Month - 1) / 3 + 1;

        #endregion
    }
}
