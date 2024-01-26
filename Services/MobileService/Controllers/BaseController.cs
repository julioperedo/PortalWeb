using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using BEntities.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MobileService.Controllers
{
    public class BaseController : ControllerBase
    {
        protected string GetError(Exception ex)
        {
            string strMessage = "";
            string strType = ex.GetType().FullName;
            switch (strType)
            {
                //    Case GetType(BComponents.BCException)
                //        If CType(ex, BComponents.BCException).ErrorCollection.Count > 0 Then
                //            strMessage = String.Join(Environment.NewLine, CType(ex, BComponents.BCException).ErrorCollection)
                //        Else
                //            strMessage = ex.Message
                //        End If
                case "System.Data.SqlClient.SqlException":
                    System.Data.SqlClient.SqlException ex1 = (System.Data.SqlClient.SqlException)ex;
                    if (ex1.Errors.Count > 0)
                    {
                        switch (ex1.Errors[0].Number)
                        {
                            case 547:
                                strMessage = "El registro no puede ser eliminado porque tiene dependencias.";
                                break;
                            case 2601:
                            case 2627:
                                strMessage = "Violación de la llave primaria o la llave no ha sido especificada apropiadamente.";
                                break;
                            default:
                                //strMessage = string.Join(Environment.NewLine, (from e in ex1.Errors select e.Message).ToArray);
                                for (int i = 0; i < ex1.Errors.Count; i++)
                                {
                                    strMessage += Environment.NewLine + ex1.Errors[i].Message;
                                }
                                break;
                        }
                    }
                    break;
                default:
                    strMessage = ex.Message;
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        strMessage += Environment.NewLine + ex.Message;
                    }
                    break;
            }
            return strMessage;
        }

        protected string ToTitle(string message)
        {
            string result = "";
            if (message != null && message.Trim() != "")
            {
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                result = myTI.ToTitleCase(message.ToLower());
            }
            return result;
        }

        protected void CompleteFilters(ref List<Field> list)
        {
            int operantors = (from i in list where i.LogicalOperator != LogicalOperators.None select i).Count();
            int parts = (from i in list where i.LogicalOperator == LogicalOperators.None select i).Count();
            for (int i = 1; i < (parts - operantors); i++)
            {
                list.Add(new Field { LogicalOperator = LogicalOperators.And });
            }
        }

        protected string SetHTMLSafe(string Data)
        {
            string strReturn = "";
            if (Data != null)
            {
                strReturn = Data.Replace("&amp;lt;", "<").Replace("&lt;", "<").Replace("&amp;gt;", ">").Replace("&gt;", ">").Replace("&amp;nbsp;", "&nbsp;")
                    .Replace("&amp;aacute;", "&aacute;").Replace("&amp;eacute;", "&eacute;").Replace("&amp;iacute;", "&iacute;").Replace("&amp;oacute;", "&oacute;").Replace("&amp;uacute;", "&uacute;")
                    .Replace("&amp;ntilde;", "&ntilde;").Replace("&amp;amp;", "&").Replace("á", "&aacute;").Replace("é", "&eacute;").Replace("í", "&iacute;").Replace("ó", "&oacute;").Replace("ú", "&uacute;").Replace("ñ", "&ntilde;")
                    .Replace("Á", "&Aacute;").Replace("É", "&Eacute;").Replace("Í", "&Iacute;").Replace("Ó", "&Oacute;").Replace("Ú", "&Uacute;").Replace("Ñ", "&Ntilde;");
            }
            return strReturn;
        }
    }
}
