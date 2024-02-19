using BEntities;
using BEntities.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUglify.JavaScript.Syntax;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using BCA = BComponents.SAP;
using BCP = BComponents.Product;
using BCS = BComponents.Security;
using BEP = BEntities.Product;

namespace Portal.Areas.Product.Controllers
{
    [Area("Product")]
    [Authorize]
    public class LoanController : BaseController
    {
        #region Constructors

        public LoanController(IConfiguration Configuration, IWebHostEnvironment HEnviroment) : base(Configuration, HEnviroment) { }

        #endregion

        #region GETs

        public IActionResult Index()
        {
            ViewData["UserCode"] = UserCode;
            ViewData["UserName"] = UserName;
            ViewData["CardCode"] = CardCode;
            ViewData["CardName"] = CardName;
            ViewData["Permission"] = GetPermission("PrestamoEquipos") > 0 ? "Y" : "N";
            return View();
        }

        public IActionResult Filter(string CardCode, DateTime? StartDate, DateTime? EndDate, string StateCodes, string Product)
        {
            string message = "";
            try
            {
                BCP.Loan bcLoan = new();
                List<Field> filters = new();
                if (!string.IsNullOrEmpty(CardCode))
                {
                    filters.Add(new("LOWER(u.CardCode)", CardCode.ToLower()));
                }
                if (StartDate != null && StartDate.HasValue)
                {
                    filters.Add(new("FinalDate", StartDate.Value.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan));
                }
                if (EndDate != null && EndDate.HasValue)
                {
                    filters.Add(new("InitialDate", EndDate.Value.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan));
                }
                if (!string.IsNullOrEmpty(StateCodes))
                {
                    filters.Add(new("State", StateCodes, Operators.In));
                }
                if (!string.IsNullOrEmpty(Product))
                {
                    filters.AddRange(new[] {
                        Field.New("LOWER(p.Name)", Product.ToLower(), Operators.Likes), Field.New("LOWER(p.Description)", Product.ToLower(), Operators.Likes), Field.New("LOWER(p.ItemCode)", Product.ToLower(), Operators.Likes),
                        Field.LogicalOr(), Field.LogicalOr()
                    });
                }
                CompleteFilters(ref filters);
                var loans = bcLoan.ListExtended(filters, "1", BEP.relLoan.Product, BEP.relLoan.User);

                if (loans?.Count() > 0)
                {
                    string clientCodes = string.Join(",", loans.Select(x => $"'{x.User.CardCode.ToLower()}'"));
                    BCA.Client bcClient = new();
                    filters = new() { new("LOWER(CardCode)", clientCodes, Operators.In) };
                    var clients = bcClient.ListShort(filters, "1");

                    var items = from l in loans
                                join c in clients on l.User.CardCode.ToLower() equals c.CardCode.ToLower()
                                select new { l.Id, l.IdUser, UserName = l.User.Name, c.CardCode, c.CardName, l.IdProduct, ProductName = l.Product.Name, l.Product.ItemCode, l.RequestDate, l.Quantity, l.InitialDate, l.FinalDate, l.State, StateName = GetStateName(l.State), l.Comments };

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

        public IActionResult DemoProducts()
        {
            string message = "";
            try
            {
                BCA.ProductStock bcStock = new();
                var stockItems = bcStock.ListForLoan();
                string codes = string.Join(",", stockItems.Select(x => $"'{x.ItemCode.ToLower()}'"));

                BCP.Product bcProduct = new();
                List<Field> filters = new() { new("LOWER(ItemCode)", codes, Operators.In) };
                var products = bcProduct.List(filters, "1");

                var items = from p in products
                            join s in stockItems on p.ItemCode.ToLower() equals s.ItemCode.ToLower()
                            select new { p.Id, p.ItemCode, p.Name, s.Category, s.Subcategory, s.Brand, s.Stock };

                return Json(new { message, items });
            }
            catch (System.Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        #endregion

        #region POSTs

        [HttpPost()]
        public IActionResult Edit(BEP.Loan Item)
        {
            string message = "";
            try
            {
                Item.StatusType = Item.Id > 0 ? StatusType.Update : StatusType.Insert;
                Item.IdUser = Item.IdUser > 0 ? Item.IdUser : UserCode;
                Item.State = Item.Id > 0 ? Item.State : "R";
                Item.LogUser = UserCode;
                Item.LogDate = DateTime.Now;

                BCP.Loan bc = new();
                bc.Save(ref Item);

                SendMail(Item);

                return Json(new { message, id = Item.Id });
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost()]
        public IActionResult Delete(long Id)
        {
            string message = "";
            try
            {
                BCP.Loan bc = new();
                var item = bc.Search(Id);
                if (item != null)
                {
                    if (item.State == "R")
                    {
                        item.StatusType = StatusType.Delete;
                        bc.Save(ref item);
                    }
                    else
                    {
                        message = "La solicitud no se puede eliminar porque ya fue aprobada.";
                    }
                }
                else
                {
                    message = "No se ha encontrado la solicitud que desea eliminar.";
                }
            }
            catch (Exception ex)
            {
                message = GetError(ex);
            }
            return Json(new { message });
        }

        [HttpPost()]
        public IActionResult ChangeState(long Id, string State)
        {
            string message = "";
            try
            {
                BCP.Loan bc = new();
                var item = bc.Search(Id, BEP.relLoan.Product);
                if (item != null)
                {
                    List<Field> filters = new() {
                        new("IdProduct", item.IdProduct),
                        new("State", "'A','D'", Operators.In),
                        new("InitialDate", item.InitialDate.ToString("yyyy-MM-dd"), Operators.LowerOrEqualThan),
                        new("FinalDate", item.FinalDate.ToString("yyyy-MM-dd"), Operators.HigherOrEqualThan)
                    };
                    CompleteFilters(ref filters);
                    var temp = bc.List(filters, "1");

                    BCA.ProductStock bcStock = new();
                    var stockItems = bcStock.ListForLoan();

                    int available = stockItems?.Where(x => x.ItemCode.ToLower() == item.Product.ItemCode.ToLower()).Sum(x => x.Stock) ?? 0,
                        requested = temp?.Sum(x => x.Quantity) ?? 0;

                    if (requested >= available & State == "A")
                    {
                        message = "No se puede aprobar la solicitud porque todas las unidades están comprometidas para el periodo seleccionado.";
                    }
                    else
                    {
                        item.State = State;
                        item.LogUser = UserCode;
                        item.LogDate = DateTime.Now;
                        item.StatusType = StatusType.Update;

                        bc.Save(ref item);
                        SendMail(item);
                    }
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

        private static string GetStateName(string Code) => Code switch { "R" => "Solicitado", "A" => "Aprobado", "D" => "Entregado", "F" => "Terminado y devuelto", "C" => "Cancelado", _ => "Ninguno" };

        private void SendMail(BEP.Loan Item)
        {
            BCS.User bcUser = new();
            var user = bcUser.Search(Item.IdUser);

            BCP.Product bcProduct = new();
            var product = bcProduct.Search(Item.IdProduct);

            string strSubject = Item.State switch
            {
                "R" => "Solicitud creada",
                "A" => "Solicitud aprobada",
                "D" => "Producto de la solitud entregado",
                "F" => "El producto de la solicitud ha sido devuelto",
                _ => "Solicitud cancelada"
            };
            string body = Item.State switch
            {
                "R" => "ha sido creada correctamente, nos comunicaremos con Ud. cuando haya sido evaluada",
                "A" => "ha sido aprobada, nos comunicaremos con Ud. para coordinar una fecha y hacer la entrega del equipo",
                "D" => "fue procesada y el producto entregado",
                "F" => "ha terminado y el producto devuelto de forma correcta",
                _ => "ha sido cancelada"
            };
            StringBuilder sb = new();
            sb.AppendLine(@"	<style> ");
            sb.AppendLine(@"		body { background-color: #FFF; font-family: Verdana, Geneva, sans-serif; font-size: 12px; } ");
            sb.AppendLine(@"		img { margin: 20px 15px; }");
            sb.AppendLine(@"		td { padding: 0 8px; line-height: 18px; }");
            sb.AppendLine(@"	</style>");
            sb.AppendLine(@"	<div style=""background-color: #DDD; margin: 10px; padding: 15px; border-radius: 15px;"" >");
            sb.AppendLine(@"		<table style=""background-color: #DDD; margin: 10px; width: 97%; font-size: 12px; border-collapse: collapse;"">");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td style=""height:130px"">");
            sb.AppendLine($@"					<br /><img src=""http://www.dmc.bo/img/logo3.png"" class=""logo"" height=""70"" />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"			<tr>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"				<td>");
            sb.AppendLine($@"				    <p>Estimado Cliente: <strong>{user.Name}</strong><p/>");
            sb.AppendLine($@"                    <p>Su solictud del producto <b>{product.ItemCode}</b> {product.Name} {body}.</p>");
            sb.AppendLine(@"					<p>Atentamente<br />El equipo de DMC</p><br />");
            sb.AppendLine(@"				</td>");
            sb.AppendLine(@"                <td style=""width: 20px;"">&nbsp;</td>");
            sb.AppendLine(@"			</tr>");
            sb.AppendLine(@"		</table>");
            sb.AppendLine(@"	</div>");

            List<MailAddress> lstTo = new(), lstCopies = new(), lstBlindCopies = new();
            if (IsDevelopmentMode)
            {
                lstTo.Add(new MailAddress("julio.peredo@dmc.bo", "Julio Peredo"));
            }
            else
            {
                lstTo.Add(new MailAddress(user.EMail, user.Name));
                FillCustomCopies("Product", "Loan", "SendMail", ref lstCopies, ref lstBlindCopies);
            }
            _ = SendMailAsync(strSubject, sb.ToString(), lstTo, lstCopies, lstBlindCopies);
        }

        #endregion
    }
}
