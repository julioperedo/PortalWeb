using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BEntities.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class ShoppingCartController : BaseController
    {
        #region Constructores

        public ShoppingCartController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
            _path = Path.Combine(rootDirectory, "wwwroot", "files", "salesOnline");
        }

        #endregion

        #region Global Variables

        string _path;// = "~/Files/SalesOnline";

        #endregion

        #region GETs

        /*public IActionResult Index()
        {
            return View();
        }*/

        public ActionResult Edit()
        {
            string message = "";
            try
            {
                long IdUser = UserCode;
                BCO.Sale bcSale = new();
                BEO.Sale beSale = bcSale.SearchCurrent(IdUser, BEO.relSale.SaleDetails, BEO.relSaleDetail.Product, BEO.relSaleDetail.Subsidiary);
                BCA.Client bcClient = new();
                BEA.Client beClient = bcClient.Search(CardCode);
                if (beSale == null)
                {
                    beSale = new BEO.Sale { Id = 0, IdUser = IdUser, Name = beClient.CardName, Address = beClient.Address, SellerCode = beClient.SellerCode, SellerName = beClient.SellerName.Trim() };
                }
                beSale.SellerCode = beClient.SellerCode;
                beSale.SellerName = beClient.SellerName.Trim();
                Models.ShoppingCart cart = new(beSale);

                if (beSale.ListSaleDetails?.Count > 0)
                {
                    string strItemCodes = string.Join(",", (from i in beSale.ListSaleDetails where i.Product != null select $"'{i.Product.ItemCode}'").ToArray());
                    BCA.ProductStock bcInventory = new();
                    var filters = new List<Field>
                    {
                        new Field("Stock", 0, Operators.HigherThan), new Field ("LOWER(ItemCode)", strItemCodes.ToLower(), Operators.In), new Field("Blocked", "N"),
                        new Field(LogicalOperators.And ), new Field (LogicalOperators.And)
                    };
                    IEnumerable<BEA.ProductStock> lstInventory = bcInventory.List(filters, "1");

                    BCB.Classifier bcClassifier = new();
                    filters = new List<Field> { new Field("IdType", (long)BEE.Classifiers.Subsidiary) };
                    IEnumerable<BEB.Classifier> lstSubsidiaries = bcClassifier.List(filters, "1");

                    string strCodes = string.Join(",", (from i in beSale.ListSaleDetails where i.IdProduct != 0 select i.IdProduct).ToArray());
                    BCP.Price bcPrice = new();
                    filters = new List<Field> { new Field("IdProduct", strCodes, Operators.In) };
                    IEnumerable<BEP.Price> lstPrices = bcPrice.List(filters, "1");


                    BCP.PriceOffer bcOffer = new();
                    string today = DateTime.Today.ToString("yyyy-MM-dd");
                    filters = new List<Field> { new Field("IdProduct", strCodes, Operators.In), new Field("Enabled", 1), new Field($"ISNULL(Since, '{today}')", today, Operators.LowerOrEqualThan), new Field($"ISNULL(Until, '{today}')", today, Operators.HigherOrEqualThan) };
                    CompleteFilters(ref filters);
                    IEnumerable<BEP.PriceOffer> offers = bcOffer.List(filters, "Price DESC") ?? Enumerable.Empty<BEP.PriceOffer>();

                    foreach (var item in cart.Details)
                    {
                        if (item.IdSubsidiary.HasValue)
                        {
                            var beSubsidiary = (from i in lstSubsidiaries where i.Id == item.IdSubsidiary.Value select i).FirstOrDefault();
                            item.Stock = (from i in lstInventory
                                          where i.Subsidiary.ToLower() == beSubsidiary.Name.ToLower() & i.ItemCode.ToLower() == item.Product.ItemCode.ToLower() & i.Blocked == "N"
                                          select beSubsidiary.Name.ToLower() == "iquique" ? i.Stock : i.Available2).Sum();
                            var beTempPrice = lstPrices.FirstOrDefault(x => x.IdProduct == item.IdProduct & x.IdSudsidiary == item.IdProduct);
                            var offer = offers.FirstOrDefault(x => x.IdProduct == item.IdProduct & x.IdSubsidiary == item.IdSubsidiary);
                            item.Price = offer?.Price ?? beTempPrice?.Regular ?? 0;
                        }
                        var lstStock = from i in lstInventory
                                       where i.ItemCode.ToLower() == item.Product.ItemCode.ToLower()
                                       group i by i.Subsidiary into g
                                       select new BEA.ProductStock { Subsidiary = g.Key, Stock = g.Sum(d => d.Stock), Available2 = g.Sum(d => d.Available2) };
                        item.DataExtra = (from s in lstSubsidiaries
                                          join i in lstStock on s.Name.ToLower() equals i.Subsidiary.ToLower() into jStock
                                          from ls in jStock.DefaultIfEmpty()
                                          join p in (from ip in lstPrices where ip.IdProduct == item.IdProduct select ip).ToList() on s.Id equals p.IdSudsidiary
                                          select new Models.ShoppingCartDetailExtra
                                          {
                                              Id = s.Id,
                                              Price = offers.Any(x => x.IdProduct == p.IdProduct & x.IdSubsidiary == s.Id & x.Price > 0) ? offers.First(x => x.IdProduct == p.IdProduct & x.IdSubsidiary == s.Id & x.Price > 0).Price : p.Regular,
                                              Stock = (s.Name.ToLower() == "iquique" ? ls?.Stock : ls?.Available2) ?? 0
                                          }).ToList();
                    }
                }

                return Json(new { message, cart });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        [HttpPost]
        public IActionResult Edit(BEO.Sale Item)
        {
            string message = "";
            try
            {
                long id = SaveSale(Item);
                return Json(new { message, id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public async Task<IActionResult> Send(BEO.Sale Item)
        {
            string message = "";
            try
            {
                Item.StateIdc = (long)BEE.States.SaleOnline.Sent;
                Item.Code = GetCode();
                SaveSale(Item);
                var (sent, strMessage) = await SendMailAsync(Item);
                if (!sent)
                {
                    message = $"No se ha podido enviar el correo a las partes. <br />{strMessage}";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult Delete(long Id)
        {
            string message = "";
            try
            {
                BCO.Sale bcSale = new BCO.Sale();
                BEO.Sale beSale = bcSale.Search(Id);
                beSale.StatusType = BEntities.StatusType.Update;
                beSale.StateIdc = (long)BEE.States.SaleOnline.Canceled;
                if (beSale.Code == null)
                {
                    beSale.Code = GetCode();
                }
                beSale.LogUser = UserCode;
                beSale.LogDate = DateTime.Now;

                bcSale.Save(ref beSale);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult AddItem(long IdProduct, int Quantity, decimal Price, bool OpenBox, long IdSubsidiary, string Warehouse)
        {
            string message = "";
            try
            {
                long IdUser = UserCode;
                BCO.Sale bcSale = new BCO.Sale();
                BEO.Sale beSale = bcSale.SearchCurrent(IdUser, BEO.relSale.SaleDetails);

                if (beSale == null)
                {
                    BCA.Client bcClient = new BCA.Client();
                    BEA.Client beClient = bcClient.Search(CardCode);
                    beSale = new BEO.Sale
                    {
                        StatusType = BEntities.StatusType.Insert,
                        IdUser = IdUser,
                        Name = beClient.CardName ?? "",
                        Address = beClient.Address ?? "",
                        SellerCode = beClient.SellerCode,
                        SellerName = beClient.SellerName.Trim(),
                        LogUser = IdUser,
                        LogDate = DateTime.Now,
                        StateIdc = (long)BEE.States.SaleOnline.Created,
                        ListSaleDetails = new List<BEO.SaleDetail>()
                    };
                }

                if ((from d in beSale.ListSaleDetails where d.IdProduct == IdProduct & d.IdSubsidiary == IdSubsidiary select d).Count() == 0)
                {
                    var detail = new BEO.SaleDetail
                    {
                        StatusType = BEntities.StatusType.Insert,
                        IdProduct = IdProduct,
                        IdSubsidiary = IdSubsidiary,
                        Quantity = Quantity,
                        Price = Price,
                        OpenBox = OpenBox,
                        Warehouse = Warehouse ?? "",
                        LogUser = IdUser,
                        LogDate = DateTime.Now
                    };
                    beSale.ListSaleDetails.Add(detail);
                    beSale.Total = (from d in beSale.ListSaleDetails select d.Quantity * d.Price).Sum();
                    bcSale.Save(ref beSale);
                }
                else
                {
                    message = "Ya se ha agregado ese item al Carrito de Compras";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult FinishSale(BEO.Sale Item)
        {
            string message = "";
            try
            {
                Item.StateIdc = (long)BEE.States.SaleOnline.Finished;
                SaveSale(Item);
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost]
        public IActionResult DeleteBill(long Id)
        {
            string message = "";
            try
            {
                BCO.SaleFiles bcFile = new BCO.SaleFiles();
                BEO.SaleFiles beFile = bcFile.Search(Id);
                beFile.StatusType = BEntities.StatusType.Delete;
                bcFile.Save(ref beFile);

                var physicalPath = Path.Combine(_path, beFile.Name);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
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

        private long SaveSale(BEO.Sale Item)
        {
            long IdUser = UserCode;
            DateTime objDate = DateTime.Now;
            BCO.Sale bcSale = new();
            BEO.Sale beSale;
            if (Item.Id == 0)
            {
                BCA.Client bcClient = new();
                BEA.Client beClient = bcClient.Search(CardCode);
                beSale = new BEO.Sale { StatusType = BEntities.StatusType.Insert, StateIdc = (long)BEE.States.SaleOnline.Created, IdUser = IdUser, SellerCode = beClient.SellerCode, SellerName = beClient.SellerName };
            }
            else
            {
                beSale = bcSale.Search(Item.Id, BEO.relSale.SaleDetails);
                beSale.StatusType = BEntities.StatusType.Update;
            }
            beSale.Name = Item.Name?.Trim();
            beSale.Address = Item.Address?.Trim();
            beSale.Commentaries = Item.Commentaries?.Trim();
            beSale.ClientSaleNote = Item.ClientSaleNote;
            beSale.WithDropShip = Item.WithDropShip;
            beSale.LogUser = IdUser;
            beSale.LogDate = objDate;

            if (beSale.ListSaleDetails == null)
            {
                beSale.ListSaleDetails = new List<BEO.SaleDetail>();
            }
            if (Item.ListSaleDetails == null)
            {
                Item.ListSaleDetails = new List<BEO.SaleDetail>();
            }
            foreach (var item in beSale.ListSaleDetails)
            {
                BEO.SaleDetail beItem = (from d in Item.ListSaleDetails where d.IdProduct == item.IdProduct select d).FirstOrDefault();
                if (beItem == null)
                {
                    item.StatusType = BEntities.StatusType.Delete;
                }
                else
                {
                    item.StatusType = BEntities.StatusType.Update;
                    item.Quantity = beItem.Quantity;
                    item.Warehouse = beItem.Warehouse ?? "";
                    item.IdSubsidiary = beItem.IdSubsidiary;
                    item.Price = beItem.Price;
                    item.LogUser = IdUser;
                    item.LogDate = objDate;
                }
            }
            foreach (var item in Item.ListSaleDetails)
            {
                if (!(from d in beSale.ListSaleDetails select d.IdProduct).Contains(item.IdProduct))
                {
                    item.StatusType = BEntities.StatusType.Insert;
                    item.Warehouse = item.Warehouse ?? "";
                    item.LogUser = IdUser;
                    item.LogDate = objDate;
                    beSale.ListSaleDetails.Add(item);
                }
            }
            beSale.Total = (from d in beSale.ListSaleDetails where d.StatusType != BEntities.StatusType.Delete select d.Quantity * d.Price).Sum();
            bcSale.Save(ref beSale);
            return beSale.Id;
        }

        private async Task<(bool, string)> SendMailAsync(BEO.Sale Item)
        {
            bool boReturn = false;
            string strSubject = "Carrito de Compras", message = "";
            StringBuilder sb = new();

            List<Field> lstFilter = new() { new Field("Id", string.Join(",", Item.ListSaleDetails.Select(x => x.IdProduct)), Operators.In) };
            BCP.Product bcProduct = new();
            IEnumerable<BEP.Product> lstProducts = bcProduct.List(lstFilter, "1");

            BCB.Classifier bcClassifier = new();
            List<BEB.Classifier> lstSubsidiaries = bcClassifier.List((long)BEE.Classifiers.Subsidiary, "1");

            BCA.Client bcClient = new();
            BEA.Client beClient = bcClient.Search(CardCode);

            BCS.User bcUser = new();
            BES.User beUser = bcUser.Search(UserCode), beSeller = bcUser.SearchSeller(Item.SellerCode);

            sb.AppendLine(@"<html>");
            sb.AppendLine(@"<head>");
            sb.AppendLine(@"<meta charset=""utf-8"">");
            sb.AppendLine(@"<meta name=""viewport"" content=""width=device-width"">");
            sb.AppendLine(@"<meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">");
            sb.AppendLine(@"<meta name=""x-apple-disable-message-reformatting"">");
            sb.AppendLine(@"<title>Solicitud</title>");
            sb.AppendLine(@"<!-- Web Font / @font-face -->");
            sb.AppendLine(@"<!-- Desktop Outlook chokes on web font references and defaults to Times New Roman, so we force a safe fallback font. -->");
            sb.AppendLine(@"<!--[if mso]>");
            sb.AppendLine(@"        <style>");
            sb.AppendLine(@"            * { font-family: sans-serif !important; }");
            sb.AppendLine(@"        </style>");
            sb.AppendLine(@"        <![endif]-->");
            sb.AppendLine(@"<!-- End Web Font / @font-face -->");
            sb.AppendLine(@"<!-- CSS Reset -->");
            sb.AppendLine(@"");
            sb.AppendLine(@"<style type=""text/css"">");
            sb.AppendLine(@"@import 'https://fonts.googleapis.com/css?family=Lato:400,700,900&subset=latin-ext';");
            sb.AppendLine(@"body { color: #666; font-family: 'Lato', sans-serif; background-color: #E9E9E9; font-size: 11px; }");
            sb.AppendLine(@".table { width: 650px; margin: 10px 5px 10px 5px; border-spacing: 0; border-collapse: collapse; }");
            sb.AppendLine(@".table thead td { padding: 4px; background-color: #3E3C4E; color: #FFF; }");
            sb.AppendLine(@".table .right { text-align: right; }");
            sb.AppendLine(@".table tfoot td { background-color: #3E3C4E; color: #FFF; }");
            sb.AppendLine(@".footer { background-color: #3e3c4e; color: #EEE; text-align: center; font-size: 0.8em; border-radius: 0 0 5px 5px; padding: 6px; }");
            sb.AppendLine(@"p { font-size: 0.85em; margin: 6px; }");
            sb.AppendLine(@"</style>");
            sb.AppendLine(@"</head>");
            sb.AppendLine(@"<body valign=""middle"" style=""margin: 0 auto; padding: 0; height: 100%; width: 100%; text-align: center; background-repeat: no-repeat;"">");
            sb.AppendLine(@"<div style=""max-width: 680px; margin: auto;"">");
            sb.AppendLine(@"  <div style=""background-color: #FFF; margin: 10px; border-radius: 5px; text-align:left;""> <img src=""http://www.dmc.bo/img/logo3.png"" alt=""DMC S.A."" style=""margin: 8px;"" height=""70"" />");
            sb.AppendLine($@"    <div style=""background-color: #CADFF7; color: #0C4278;padding: 10px;""> <strong>Cliente:</strong> ({beClient.CardCode}) {beClient.CardName} <br />");
            sb.AppendLine($@"      <strong>Nombre Entrega:</strong> {Item.Name} <br />");
            sb.AppendLine($@"      <strong>Direcci&oacute;n Entrega:</strong> {Item.Address} <br />");
            sb.AppendLine($@"      <strong>Comentarios:</strong> {Item.Commentaries} <br />");
            sb.AppendLine($@"      <strong>Orden de Compra Cliente:</strong> {Item.ClientSaleNote} <br />");
            sb.AppendLine($@"      <strong>Vendedor:</strong> {beSeller.Name} <br />");
            //if(Item.WithDropShip) {
            //    sb.AppendLine($@"      <strong>Requiere DropShip:</strong> SI </div>");
            //} else {
            //    sb.AppendLine($@"      <strong>Requiere DropShip:</strong> NO </div>");
            //}
            sb.AppendLine(@"    <div style=""background-color: #FFF;"">");
            sb.AppendLine(@"      <table class=""table"">");
            sb.AppendLine(@"        <thead>");
            sb.AppendLine(@"          <tr>");
            sb.AppendLine(@"            <td>C&oacute;digo</td>");
            sb.AppendLine(@"            <td>Nombre</td>");
            sb.AppendLine(@"            <td>Sucursal</td>");
            //sb.AppendLine(@"            <td>Almac&eacute;n</td>");
            sb.AppendLine(@"            <td class=""right"">Cantidad</td>");
            sb.AppendLine(@"            <td class=""right"">Precio</td>");
            sb.AppendLine(@"            <td class=""right"">Subtotal</td>");
            sb.AppendLine(@"          </tr>");
            sb.AppendLine(@"        </thead>");
            sb.AppendLine(@"        <tbody>");

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

                wsMain.Cells[4, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                wsMain.Cells[6, 1].Value = "Código";
                wsMain.Cells[6, 2].Value = "Nombre";
                wsMain.Cells[6, 3].Value = "Sucursal";
                wsMain.Cells[6, 4].Value = "Cantidad";
                wsMain.Cells[6, 5].Value = "Precio";
                wsMain.Cells[6, 5].Value = "Subtotal";

                int row = 7;
                foreach (var detail in Item.ListSaleDetails)
                {
                    BEP.Product beProduct = lstProducts.FirstOrDefault(x => x.Id == detail.IdProduct);
                    BEB.Classifier beSubsidiary = lstSubsidiaries.FirstOrDefault(x => x.Id == detail.IdSubsidiary);
                    if (beProduct != null)
                    {
                        sb.AppendLine(@"          <tr>");
                        sb.AppendLine($@"            <td>{beProduct.ItemCode}</td>");
                        sb.AppendLine($@"            <td>{beProduct.Name}</td>");
                        sb.AppendLine($@"            <td>{beSubsidiary.Name}</td>");
                        //sb.AppendLine($@"            <td>{detail.Warehouse}</td>");
                        sb.AppendLine($@"            <td class=""right"">{detail.Quantity}</td>");
                        sb.AppendLine($@"            <td class=""right"">{detail.Price:#,##0.00}</td>");
                        sb.AppendLine($@"            <td class=""right"">{detail.Price * detail.Quantity:#,##0.00}</td>");
                        sb.AppendLine(@"          </tr>");

                        wsMain.Cells[row, 1].Value = beProduct.ItemCode;
                        wsMain.Cells[row, 2].Value = beProduct.Name;
                        wsMain.Cells[row, 3].Value = beSubsidiary.Name;
                        wsMain.Cells[row, 4].Value = detail.Quantity;
                        wsMain.Cells[row, 5].Value = detail.Price;
                        wsMain.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
                        wsMain.Cells[row, 6].Formula = $"D{row}*E{row}";
                        wsMain.Cells[row, 6].Style.Numberformat.Format = "#,##0.00";
                        row++;
                    }
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
            sb.AppendLine(@"        </tbody>");
            sb.AppendLine(@"        <tfoot>");
            sb.AppendLine(@"          <tr>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            //sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine(@"            <td></td>");
            sb.AppendLine($@"            <td class=""right"">{(from d in Item.ListSaleDetails select d.Price * d.Quantity).Sum():#,##0.00}</td>");
            sb.AppendLine(@"          </tr>");
            sb.AppendLine(@"        </tfoot>");
            sb.AppendLine(@"      </table>");
            sb.AppendLine(@"      <p>Su solicitud ha sido enviada en espera de ser atendida por su Ejecutivo de Cuenta, el cual se pondr&aacute; en contacto con usted para su confirmaci&oacute;n y ser procesada.<br />");
            sb.AppendLine(@"      <b>Validez de la Cotizaci&oacute;n:</b> 3 d&iacute;as<br /><b>Nota:</b>En esta cotizaci&oacute;n se puede incrementar productos y cantidades.<br />Los descuentos por vol&uacute;men de compra ser&aacute;n confirmados por su Ejecutivo de Cuenta al procesar la orden.</p>");
            sb.AppendLine(@"    </div>");
            sb.AppendLine(@"    <div class=""footer"">Este correo es generado por un servicio autom&aacute;tico, por favor no responda.</div>");
            sb.AppendLine(@"  </div>");
            sb.AppendLine(@"</div>");
            sb.AppendLine(@"</body>");
            sb.AppendLine(@"</html>");

            List<MailAddress> lstTo = new List<MailAddress>(), lstCopies = new List<MailAddress>();
            if ((beUser != null && !string.IsNullOrWhiteSpace(beUser.EMail)) & (beSeller != null && !string.IsNullOrWhiteSpace(beSeller.EMail)))
            {
                if (!IsEMailBlacklisted(beUser.EMail))
                {
                    lstTo.Add(new MailAddress(beUser.EMail, beUser.Name));
                    lstTo.Add(new MailAddress(beSeller.EMail, beSeller.Name));
                    //lstTo.Add(new MailAddress("julio.peredo@dmc.bo", "Julio"));

                    List<Field> filters = new()
                    {
                        new Field("SellerCode", Item.SellerCode),
                        new Field("InitialDate", DateTime.Today.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan),
                        new Field("FinalDate", DateTime.Today.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan),
                        new Field (LogicalOperators.And),
                        new Field (LogicalOperators.And)
                    };
                    BCF.Replace bcReplace = new();
                    IEnumerable<BEF.Replace> replaces = bcReplace.List(filters, "1");
                    if (replaces?.Count() > 0)
                    {
                        filters = new List<Field> { new Field("SellerCode", string.Join(",", replaces.Select(x => $"'{x.ReplaceCode}'").Distinct()), Operators.In) };
                        BCS.UserData bcData = new();
                        IEnumerable<BES.UserData> userDatas = bcData.List(filters, "1", BES.relUserData.User);
                        if (userDatas?.Count() > 0)
                        {
                            foreach (var item in userDatas)
                            {
                                if (IsValidEmail(item.User?.EMail))
                                {
                                    lstCopies.Add(new MailAddress(item.User.EMail, item.User.Name));
                                }
                            }
                        }
                    }

                    await SendMailAsync(strSubject, sb.ToString(), lstTo, lstCopies, null, null, files);
                    boReturn = true;
                }
                else
                {
                    message = $"Su correo {beUser.EMail} está en nuestra lista negra, por favor comuníquese on su Ejecutivo de Ventas";
                }
            }
            return (boReturn, message);
        }

        private string GetCode()
        {
            string strCode = $"{CardCode.Replace("-", "")}-{DateTime.Now.Year}";
            BCO.Sale bcSale = new();
            List<Field> lstFilter = new()
            {
                new Field ("Code", strCode, Operators.Likes),
                new Field ("IdUser", UserCode),
                new Field (LogicalOperators.And)
            };

            IEnumerable<BEO.Sale> lstSales = bcSale.List(lstFilter, "Code DESC");
            if (lstSales?.Count() > 0)
            {
                string lastCode = lstSales.First().Code;
                if (!string.IsNullOrWhiteSpace(lastCode))
                {
                    long intCode = long.Parse(lastCode.Split('-').Last()) + 1;
                    strCode = CardCode.Replace("-", "") + "-" + intCode.ToString().PadLeft(4, '0');
                }
                else
                {
                    strCode = CardCode.Replace("-", "") + "-" + DateTime.Now.Year.ToString() + "0001";
                }
            }
            else
            {
                strCode = CardCode.Replace("-", "") + "-" + DateTime.Now.Year.ToString() + "0001";
            }

            return strCode;
        }

        #endregion

    }
}
