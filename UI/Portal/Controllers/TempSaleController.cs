using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUglify.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using BCA = BComponents.SAP;
using BCB = BComponents.Base;
using BCF = BComponents.Staff;
using BCO = BComponents.Online;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEA = BEntities.SAP;
using BEB = BEntities.Base;
using BEE = BEntities.Enums;
using BEF = BEntities.Staff;
using BEO = BEntities.Online;
using BEP = BEntities.Product;
using BES = BEntities.Security;

namespace Portal.Controllers
{
    public class TempSaleController : BaseController
    {
        #region Constructores

        public TempSaleController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            //_path = Path.Combine(rootDirectory, "wwwroot", "files", "salesOnline");
        }

        #endregion

        #region GETs

        // GET: Product/TempSale
        /*public IActionResult Index()
        {
            return View();
        }*/

        public IActionResult Edit()
        {
            string message = "";
            try
            {
                long userId = UserCode;
                List<Field> filters = new() { new Field { Name = "IdUser", Value = userId } };
                BCO.TempSale bcSale = new();
                IEnumerable<BEO.TempSale> lstSales = bcSale.List(filters, "1", BEO.relTempSale.TempSaleDetails, BEO.relTempSaleDetail.Product, BEO.relTempSaleDetail.Subsidiary);
                BEO.TempSale beTemp = lstSales?.FirstOrDefault() ?? new BEO.TempSale { Id = 0, ListTempSaleDetails = new List<BEO.TempSaleDetail>() };
                Models.OnlineSale beSale = new(beTemp);
                if (beSale.Details?.Count > 0)
                {
                    string strItemCodes = string.Join(",", (from i in beSale.Details where i.Product != null select $"'{i.Product.ItemCode}'").ToArray()),
                        productIds = string.Join(",", beSale.Details.Select(x => $"'{x.IdProduct}'"));

                    BCA.ProductStock bcInventory = new();
                    filters = new List<Field> {
                        new Field("Stock", 0, Operators.HigherThan), new Field("ItemCode", strItemCodes, Operators.In), new Field("Blocked", "N"),
                        new Field(LogicalOperators.And), new Field(LogicalOperators.And)
                    };
                    IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(filters, "1");

                    BCB.Classifier bcClassifier = new();
                    filters = new List<Field> { new Field { Name = "IdType", Value = (long)BEE.Classifiers.Subsidiary } };
                    IEnumerable<BEB.Classifier> lstSubsidiaries = bcClassifier.List(filters, "1");

                    string strCodes = string.Join(",", (from i in beSale.Details where i.IdProduct != 0 select i.IdProduct).ToArray());
                    BCP.Price bcPrice = new();
                    filters = new List<Field> { new Field { Name = "IdProduct", Value = strCodes, Operator = Operators.In } };
                    IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1");

                    BCP.PriceOffer bcOffer = new();
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    filters = new List<Field> { new Field("IdProduct", productIds, Operators.In), new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) };
                    CompleteFilters(ref filters);
                    IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

                    foreach (var item in beSale.Details)
                    {
                        if (item.IdSubsidiary.HasValue)
                        {
                            var beSubsidiary = (from i in lstSubsidiaries where i.Id == item.IdSubsidiary.Value select i).FirstOrDefault();
                            item.Stock = (from i in lstInventory
                                          where i.Subsidiary.ToLower() == beSubsidiary.Name.ToLower() & i.ItemCode.ToLower() == item.Product.ItemCode.ToLower() & i.Blocked == "N"
                                          select beSubsidiary.Name.ToLower() == "iquique" ? i.Stock : i.Available2).Sum();
                        }
                        var lstStock = (from i in lstInventory
                                        where i.ItemCode.ToLower() == item.Product.ItemCode.ToLower()
                                        group i by i.Subsidiary into g
                                        select new BEA.ProductStock { Subsidiary = g.Key, Stock = g.Sum(d => d.Stock), Available2 = g.Sum(d => d.Available2) }).ToList();
                        item.DataExtra = (from s in lstSubsidiaries
                                          join i in lstStock on s.Name.ToLower() equals i.Subsidiary.ToLower() into jStock
                                          from ls in jStock.DefaultIfEmpty()
                                          join p in (from ip in lstPrices where ip.IdProduct == item.IdProduct select ip).ToList() on s.Id equals p.IdSudsidiary
                                          select new Models.OnlineSaleDetailExtra
                                          {
                                              Id = s.Id,
                                              Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.IdProduct == p.IdProduct & x.Price > 0).Price : p.Regular,
                                              Stock = (s.Name.ToLower() == "iquique" ? ls?.Stock : ls?.Available2) ?? 0
                                          }).ToList();
                    }
                }
                return Json(new { message, item = beSale });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        public IActionResult GetSubsidiariesByProduct(long ProductId)
        {
            BCP.Price bcPrice = new();
            List<Field> filters = new() { new Field("IdProduct", ProductId) };
            IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1", BEP.relPrice.Sudsidiary);

            BCP.PriceOffer bcOffer = new();
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            filters.AddRange(new[] { new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) });
            CompleteFilters(ref filters);
            IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

            var items = from p in lstPrices
                        where p.Regular > 0
                        select new { Id = p.IdSudsidiary, Name = ToTitle(p.Sudsidiary.Name), Price = offers.Any(x => x.IdSubsidiary == p.IdSudsidiary & x.Price > 0) ? offers.First(x => x.IdSubsidiary == p.IdSudsidiary & x.Price > 0).Price : p.Regular };

            return Json(items);
        }

        //public ActionResult GetWarehouses(long ProductId, string ItemCode, string Subsidiary)
        //{
        //    BCP.WarehouseAllowed bcWharehouse = new BCP.WarehouseAllowed();
        //    List<BEP.WarehouseAllowed> lstWarehouses = bcWharehouse.List("Name");

        //    BCA.ProductStock bcInventory = new BCA.ProductStock();
        //    List<Field> lstFilter = new List<Field> {
        //        new Field { Name = Subsidiary.ToLower() == "iquique" ? "Stock" : "Disponible2", Value = 0, Operator = Operators.HigherThan },
        //        new Field { Name = "ItemCode", Value = ItemCode },
        //        new Field { Name = "LOWER(Sucursal)", Value = Subsidiary.ToLower() },
        //        new Field { LogicalOperator = LogicalOperators.And },
        //        new Field { LogicalOperator = LogicalOperators.And }
        //    };
        //    List<BEA.ProductStock> lstInventory = bcInventory.ListShort(lstFilter, "1");

        //    var lstItems = (from i in lstInventory
        //                    where (from a in lstWarehouses select a.Name.ToLower()).Contains(i.Almacen.ToLower())
        //                    select new { Id = toTitle(i.Almacen), Name = toTitle(i.Almacen) }).ToList();

        //    BCB.Classifier bcClassifier = new BCB.Classifier();
        //    lstFilter = new List<Field> {
        //        new Field { Name = "IdType", Value = (long)BEE.Classifiers.Subsidiary },
        //        new Field { Name = "LOWER(Name)", Value = Subsidiary.ToLower() },
        //        new Field { LogicalOperator = LogicalOperators.And }
        //    };
        //    BEB.Classifier beSubsidiary = bcClassifier.List(lstFilter, "1").FirstOrDefault() ?? new BEB.Classifier();

        //    BCP.Price bcPrice = new BCP.Price();
        //    lstFilter = new List<Field> {
        //        new Field { Name = "IdProduct", Value = ProductId },
        //        new Field { Name = "IdSudsidiary", Value = beSubsidiary.Id },
        //        new Field { LogicalOperator = LogicalOperators.And }
        //    };
        //    BEP.Price bePrice = bcPrice.List(lstFilter, "1").FirstOrDefault() ?? new BEP.Price();

        //    return Json(new { Warehouses = lstItems, Price = bePrice.Offer.HasValue && bePrice.Offer.Value > 0 ? bePrice.Offer.Value : bePrice.Regular });
        //}

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult AddItem(long IdProduct, int Quantity, decimal Price, bool OpenBox, long? IdSubsidiary, string Warehouse)
        {
            string message = "";
            try
            {
                long userId = UserCode;
                DateTime now = DateTime.Now;
                BCO.TempSale bcSale = new();
                BCO.TempSaleDetail bcDetail = new();

                List<Field> lstFilters = new() { new Field("IdUser", userId) };
                IEnumerable<BEO.TempSale> lstSales = bcSale.List(lstFilters, "1", BEO.relTempSale.TempSaleDetails);
                BEO.TempSale beSale = lstSales?.FirstOrDefault();
                if (beSale == null)
                {
                    BCS.User bcUser = new();
                    BES.User beUser = bcUser.Search(userId);
                    beSale = new BEO.TempSale { StatusType = StatusType.Insert, IdUser = userId, Name = beUser.Name, EMail = beUser.EMail, ListTempSaleDetails = new List<BEO.TempSaleDetail>(), LogUser = userId, LogDate = now };
                    bcSale.Save(ref beSale);
                }
                if (!(from d in beSale.ListTempSaleDetails where d.IdProduct == IdProduct & d.IdSubsidiary == IdSubsidiary select d).Any())
                {
                    BEO.TempSaleDetail beDetail = new()
                    {
                        StatusType = StatusType.Insert,
                        IdSale = beSale.Id,
                        IdProduct = IdProduct,
                        Quantity = Quantity,
                        Price = Price,
                        OpenBox = OpenBox,
                        IdSubsidiary = IdSubsidiary,
                        Warehouse = Warehouse ?? "",
                        LogUser = userId,
                        LogDate = now
                    };
                    bcDetail.Save(ref beDetail);
                }
                else
                {
                    message = "Ese producto ya est&aacute; en el carrito.";
                }
            }
            catch (Exception ex)
            {
                message = "Se ha producido un error al agregar el producto, por favor intente nuevamente en un momento. <br />" + GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(List<BEO.TempSaleDetail> Items)
        {
            string message = "";
            try
            {
                await SaveTempSaleAsync(Items ?? new List<BEO.TempSaleDetail>(), false, false);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete()
        {
            string message = "";
            try
            {
                DeleteItems();
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public async Task<IActionResult> SendTempSale(List<BEO.TempSaleDetail> Items, bool WithComments)
        {
            string message = "";
            try
            {
                await SaveTempSaleAsync(Items, true, WithComments);
                DeleteItems();
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region Private Methods

        private async Task SaveTempSaleAsync(List<BEO.TempSaleDetail> Items, bool SendEmail, bool WithComments)
        {
            long userId = UserCode;
            DateTime now = DateTime.Now;
            BCO.TempSale bcSale = new();

            List<Field> lstFilters = new() { new Field("IdUser", userId) };
            IEnumerable<BEO.TempSale> lstSales = bcSale.List(lstFilters, "1", BEO.relTempSale.TempSaleDetails);
            BEO.TempSale beSale = lstSales?.FirstOrDefault();
            if (beSale == null)
            {
                BCS.User bcUser = new();
                BES.User beUser = bcUser.Search(userId);
                beSale = new BEO.TempSale { StatusType = StatusType.Insert, IdUser = userId, Name = beUser.Name, EMail = beUser.EMail, ListTempSaleDetails = new List<BEO.TempSaleDetail>(), LogUser = userId, LogDate = now };
            }
            beSale.ListTempSaleDetails.ForEach(i => i.StatusType = StatusType.Delete);
            foreach (var item in Items)
            {
                BEO.TempSaleDetail beTemp = beSale.ListTempSaleDetails.FirstOrDefault(x => x.Id == item.Id);
                if (beTemp == null)
                {
                    beTemp = new BEO.TempSaleDetail { IdProduct = item.IdProduct };
                    beSale.ListTempSaleDetails.Add(beTemp);
                }
                beTemp.StatusType = beTemp.Id == 0 ? StatusType.Insert : StatusType.Update;
                beTemp.Quantity = item.Quantity;
                beTemp.Price = item.Price;
                beTemp.IdSubsidiary = item.IdSubsidiary;
                beTemp.Warehouse = item.Warehouse ?? "";
                beTemp.LogUser = userId;
                beTemp.LogDate = now;
            }
            bcSale.Save(ref beSale);

            if (SendEmail)
            {
                beSale = bcSale.Search(beSale.Id, BEO.relTempSale.TempSaleDetails, BEO.relTempSaleDetail.Product, BEO.relTempSaleDetail.Subsidiary, BEP.relProduct.Prices, BEP.relProduct.PriceOffers);
                await SendByMailAsync(beSale, WithComments);
            }
        }

        private async Task SendByMailAsync(BEO.TempSale Quote, bool WithComments)
        {
            //Por ahora aca, pero debería ser una solución global
            var cul = CultureInfo.CreateSpecificCulture("es-BO");
            cul.NumberFormat.NumberDecimalSeparator = ".";
            cul.NumberFormat.NumberGroupSeparator = ",";
            System.Threading.Thread.CurrentThread.CurrentCulture = cul;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cul;

            StringBuilder sb = new();
            sb.AppendLine(@"<html>");
            sb.AppendLine(@" <head>");
            sb.AppendLine(@"     <style>");
            sb.AppendLine(@"     table { border-collapse: collapse; }");
            sb.AppendLine(@"      .image { width: 30%; padding: 2px 0 2px 2px; text-align: center; } ");
            sb.AppendLine(@"      .image img { width: 200px; } ");
            sb.AppendLine(@"      .client { font-size:  1.1em; font-weight: 600; } ");
            sb.AppendLine(@"      td { font-size: 11.5px; } ");
            sb.AppendLine(@"      tr:nth-child(even) {background: #CCC} ");
            sb.AppendLine(@"     .header { background-color: #444; } ");
            sb.AppendLine(@"     </style>");
            sb.AppendLine(@"	<!--[if(mso)|(IE)]>");
            sb.AppendLine(@"	<style type=""text/css"">");
            sb.AppendLine(@"		table {border-collapse: collapse !important; mso-table-lspace:0pt !important; mso-table-rspace:0pt !important;}");
            sb.AppendLine(@"		table, div, td {font-family: 'Montserrat', Verdana, Geneva, sans-serif !important;}");
            sb.AppendLine(@"	</style>");
            sb.AppendLine(@"	<![endif]-->");
            sb.AppendLine(@"	<!--[if mso]>");
            sb.AppendLine(@"		<style type=""text/css"">");
            sb.AppendLine(@"			ol, ul {margin-left: 25px !important; margin-top: 0 !important;}");
            sb.AppendLine(@"		</style>");
            sb.AppendLine(@"		<xml>");
            sb.AppendLine(@"		  <o:OfficeDocumentSettings>");
            sb.AppendLine(@"			<o:AllowPNG/>");
            sb.AppendLine(@"			<o:PixelsPerInch>96</o:PixelsPerInch>");
            sb.AppendLine(@"		 </o:OfficeDocumentSettings>");
            sb.AppendLine(@"		</xml>");
            sb.AppendLine(@"		<![endif]-->");
            sb.AppendLine(@" </head>");
            sb.AppendLine(@" <body>");
            sb.AppendLine(@"     <div style='font-family: Verdana, Geneva, sans-serif; font-size: 12px; color: #666;'>");
            sb.AppendLine(@"        <div class='header'>");
            sb.AppendLine(@"            <img src=""http://portal.dmc.bo/images/logocot.png"" alt=""Cotizacion"" />");
            sb.AppendLine(@"        </div>");
            sb.AppendLine(@"        <p>Estimado Cliente<br />");
            sb.AppendLine($@"           <span class='client'>{Quote.Name}<span> </p>");
            //if (!string.IsNullOrWhiteSpace(Quote.Header))
            //{
            //    sb.AppendLine($"     <p>{Quote.Header}</p>");
            //}

            var files = new List<Attachment>();
            Stream stream;
            using (ExcelPackage objExcel = new())
            {
                ExcelWorksheet wsMain = objExcel.Workbook.Worksheets.Add("Main");
                FileInfo logo = new(Path.Combine(rootDirectory, "wwwroot", "images", "logo2.jpg"));

                wsMain.Name = "Pedido";
                wsMain.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                wsMain.Cells.Style.Fill.PatternColor.SetColor(Color.White);
                wsMain.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
                wsMain.Cells.Style.Font.Size = 9;

                wsMain.Cells[4, 1].Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
                wsMain.Cells[6, 1].Value = "Código";
                wsMain.Cells[6, 2].Value = "Nombre";
                wsMain.Cells[6, 3].Value = "Sucursal";
                wsMain.Cells[6, 4].Value = "Cantidad";
                wsMain.Cells[6, 5].Value = "Precio";
                wsMain.Cells[6, 5].Value = "Subtotal";

                int row = 7;
                sb.AppendLine(@"        <table style='width: 95%; font-family: Verdana, Geneva, sans-serif; font-size: 11px; color: #666;'>");
                foreach (var item in Quote.ListTempSaleDetails)
                {
                    string style = Quote.ListTempSaleDetails.IndexOf(item) % 2 == 1 ? " style='background-color: #DCDCDC;'" : "";
                    string logo2 = "http://portal.dmc.bo/images/products/" + item.Product.ImageURL;
                    sb.AppendLine($@"     <tr{style}>");
                    sb.AppendLine(@"        <td class='image' width='200' style='width: 200px; max-height: 200px;'>");
                    sb.AppendLine($@"          <img src='{logo2}' width='200' style='width: 200px; max-height: 200px;' />");
                    sb.AppendLine(@"        </td>");
                    sb.AppendLine(@"        <td style='padding-left: 5px;'>");
                    if (!string.IsNullOrWhiteSpace(item.Product.Link))
                    {
                        sb.AppendLine($@"     <a href='{item.Product.Link.Trim()}' target='_blank' style='color: #337AB7; text-decoration: none;'><strong><span style='font-size: 13.5px;'>{item.Product.ItemCode}</span></strong></a><br />");
                    }
                    else
                    {
                        sb.AppendLine($@"     <strong><span style='font-size: 13.5px;'>{item.Product.ItemCode}</span></strong><br />");
                    }
                    sb.AppendLine($@"     <span style='font-size: 12.5px;'>{item.Product.Name}</span><br />");
                    sb.AppendLine(item.Product.Description);
                    if (WithComments & !string.IsNullOrWhiteSpace(item.Product.Commentaries))
                    {
                        sb.AppendLine(item.Product.Commentaries.Replace("\r", "<br />"));
                    }
                    sb.AppendLine(@"   </td>");
                    sb.AppendLine(@"   <td style='width: 20%; padding-top: 8px; padding-left: 8px;'>");
                    sb.AppendLine(@"         <div>");
                    sb.AppendLine($@"             <span style='font-weight: 600; text-align: right; width: 100%;'>{item.Subsidiary.Name}</span>");
                    string fob = item.Subsidiary.Name.ToLower() == "santa cruz" ? "" : "FOB ";
                    if (item.Price > 0)
                    {
                        sb.AppendLine($@"             <br /><span style='font-weight: 600; text-align: right; width: 100%;'>{fob}{item.Price:#,##0.00} $Us</span>");
                        if (item.Product.ListPrices?.Count(x => x.IdSudsidiary == item.IdSubsidiary) > 0)
                        {
                            //var price = item.Product.ListPrices.FirstOrDefault(x => x.IdSudsidiary == item.IdSubsidiary);
                            var price = item.Product.CurrentOffers.FirstOrDefault(x => x.IdSubsidiary == item.IdSubsidiary);
                            if (!string.IsNullOrWhiteSpace(price?.Description))
                            {
                                sb.AppendLine($@"             <br /><span style='color: #f84444;font-weight: 600; text-align: right; width: 100%;'>{price.Description}</span>");
                            }
                        }
                    }
                    if (item.Quantity > 0)
                    {
                        sb.AppendLine($@"             <br /><span style='font-weight: 600; text-align: right; width: 100%;'>{item.Quantity} unidad(es)</span>");
                    }
                    sb.AppendLine(@"         </div>");
                    sb.AppendLine(@"   </td>");
                    sb.AppendLine(@"</tr>");
                    sb.AppendLine(@"<tr>");
                    sb.AppendLine(@"<td colspan='3'></td>");
                    sb.AppendLine(@"</tr>");

                    wsMain.Cells[row, 1].Value = item.Product.ItemCode;
                    wsMain.Cells[row, 2].Value = item.Product.Name;
                    wsMain.Cells[row, 3].Value = item.Subsidiary.Name;
                    wsMain.Cells[row, 4].Value = item.Quantity;
                    wsMain.Cells[row, 5].Value = item.Price;
                    wsMain.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
                    wsMain.Cells[row, 6].Formula = $"D{row}*E{row}";
                    wsMain.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
                    row++;
                }
                wsMain.Cells[row, 6].Formula = $"SUM(F7:F{row - 1})";
                wsMain.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
                wsMain.Cells.AutoFitColumns();
                wsMain.Cells.Style.WrapText = true;

                var file = objExcel.GetAsByteArray();
                stream = new MemoryStream(file);
                objExcel.Dispose();
            }
            files.Add(new Attachment(stream, "Detalle.xlsx"));

            sb.AppendLine(@"</table><br />");
            sb.AppendLine(@"<p style='font-size: 0.9em;'>Las cotizaciones emitidas en el presente correo son v&aacute;lidas s&oacute;lo para el mes en curso y est&aacute;n sujetas a cambios de precios, variaci&oacute;n en su disponibilidad y t&eacute;rminos de ofertas, garant&iacute;as y otros aspectos. Por tal raz&oacute;n DMC S.A. no se hace responsable por errores u omisiones que se den en la presente.</p>");
            sb.AppendLine(@"<p style='font-size: 0.9em;'>Para garantizar el env&iacute;o de mercader&iacute;a el mismo d&iacute;a debemos contar con la confirmaci&oacute;n de su dep&oacute;sito/orden de compra hasta las 15:30.</p>");

            long intUserID = UserCode;
            BCS.User bcUser = new();
            BES.User beUser = bcUser.Search(intUserID);
            BCS.UserData bcData = new();
            BES.UserData beData = bcData.SearchByUser(intUserID);
            BCF.Replace bcReplace = new();
            //List<BES.UserData> userDatas;
            //List<Field> filters = new List<Field> {
            //    new Field("SellerCode", beData.SellerCode), new Field("InitialDate", DateTime.Today.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan), new Field("FinalDate", DateTime.Today.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan),
            //    new Field { LogicalOperator= LogicalOperators.And }, new Field { LogicalOperator= LogicalOperators.And }
            //};
            //List<BEF.Replace> replaces = bcReplace.List(filters, "1");

            if (!string.IsNullOrWhiteSpace(beData?.Signature))
            {
                if (beData.Signature.Contains("/Portal/Content/UserData/"))
                {
                    sb.AppendLine(SetHTMLSafe(beData.Signature.Replace("/Portal/Content/UserData/", "http://portal.dmc.bo/images/userdata/")));
                }
                else if (beData.Signature.Contains("/Content/UserData/"))
                {
                    sb.AppendLine(SetHTMLSafe(beData.Signature.Replace("/Content/UserData/", "http://portal.dmc.bo/images/userdata/")));
                }
                else if (beData.Signature.Contains("/Portal/images/userdata/"))
                {
                    sb.AppendLine(SetHTMLSafe(beData.Signature.Replace("/Portal/images/userdata/", "http://portal.dmc.bo/images/userdata/")));
                }
                else if (beData.Signature.Contains("/images/userdata/"))
                {
                    sb.AppendLine(SetHTMLSafe(beData.Signature.Replace("/images/userdata/", "http://portal.dmc.bo/images/userdata/")));
                }
                else
                {
                    sb.AppendLine(SetHTMLSafe(beData.Signature));
                }
            }
            else
            {
                sb.AppendLine("<p>Atentamente</p>");
                if (!string.IsNullOrWhiteSpace(beUser?.Name))
                {
                    sb.AppendLine($"<p>{beUser.Name}</p>");
                }
                else
                {
                    sb.AppendLine("<p>El equipo de DMC</p>");
                }
            }
            sb.AppendLine("     </div>");
            sb.AppendLine(" </body>");
            sb.AppendLine("</html>");

            string strTitle = "Cotización DMC";

            List<MailAddress> lstTo = new(), lstCopy = new();
            MailAddress from = null;
            if (IsDevelopmentMode)
            {
                lstTo.Add(new MailAddress("julio.peredo@dmc.bo", "Julio Peredo"));
            }
            else
            {
                if (IsValidEmail(Quote.EMail)) lstTo.Add(new MailAddress(Quote.EMail, Quote.Name));
                if (!string.IsNullOrWhiteSpace(beUser.EMail) && IsValidEmail(beUser.EMail.Trim()))
                {
                    from = new MailAddress(beUser.EMail, beUser.Name);
                    lstCopy.Add(from);
                }
                //if (replaces?.Count > 0) {
                //    filters = new List<Field> { new Field("SellerCode", string.Join(",", replaces.Select(x => $"'{x.ReplaceCode}'").Distinct()), Operators.In) };
                //    userDatas = bcData.List(filters, "1", BES.relUserData.User);
                //    if (userDatas?.Count > 0) {
                //        foreach (var item in userDatas) {
                //            if (IsValidEmail(item.User?.EMail)) {
                //                lstCopy.Add(new MailAddress(item.User.EMail, item.User.Name));
                //            }
                //        }
                //    }
                //}
            }

            if (lstTo.Count > 0 || lstCopy.Count > 0)
            {
                await SendMailAsync(strTitle, sb.ToString(), lstTo, null, lstCopy, from, files);

                //Guarda constancia del envío
                BCO.QuoteSent bcSent = new();
                BEO.QuoteSent beSent = new() { StatusType = StatusType.Insert, IdUser = intUserID, Name = Quote.Name, EMail = Quote.EMail, Body = sb.ToString(), LogUser = UserCode, LogDate = DateTime.Now };
                bcSent.Save(ref beSent);
            }
        }

        private void DeleteItems()
        {
            long userId = UserCode;
            BCO.TempSale bcSale = new();

            List<Field> lstFilters = new() { new Field { Name = "IdUser", Value = userId } };
            IEnumerable<BEO.TempSale> lstSales = bcSale.List(lstFilters, "1", BEO.relTempSale.TempSaleDetails);
            BEO.TempSale beSale = lstSales?.FirstOrDefault();
            if (beSale != null)
            {
                beSale.ListTempSaleDetails.ForEach(i => i.StatusType = StatusType.Delete);
                bcSale.Save(ref beSale);
            }
        }

        #endregion
    }
}
