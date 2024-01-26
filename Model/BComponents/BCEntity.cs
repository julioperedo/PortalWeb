using System;
using System.Configuration;
using System.Collections.Generic;
using System.Transactions;
using BE = BEntities;
using System.Security.Cryptography;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BComponents
{
    public abstract class BCEntity
    {

        #region Enumeradores

        #endregion

        #region Declaracion de Variables

        protected List<string> ErrorCollection = new List<string>();

        #endregion

        #region Definición de las propiedades

        public List<string> ErrorMessage
        {
            get { return this.ErrorCollection; }
        }

        #endregion

        #region Declaracion de Metodos

        protected TransactionScope GenerateBusinessTransaction(bool IsAsync = false)
        {
            TransactionOptions tsOptions = new()
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = new TimeSpan(0, 10, 0)
            };
            if (IsAsync)
            {
                return new TransactionScope(TransactionScopeOption.Required, tsOptions, TransactionScopeAsyncFlowOption.Enabled);
            }
            else
            {
                return new TransactionScope(TransactionScopeOption.Required, tsOptions);
            }
        }

        protected void LogError(Exception ex1)
        {
            //System.Diagnostics.EventLog _Log = new System.Diagnostics.EventLog();
            //_Log.Source = "Portal";
            //_Log.Log = "Portal";
            //string strMessage = "";
            //try
            //{
            //    if (ex1 != null)
            //    {
            //        strMessage = ex1.Message;
            //        Exception e = ex1.InnerException;
            //        while (e != null)
            //        {
            //            strMessage += Environment.NewLine + e.Message;
            //            e = e.InnerException;
            //        }
            //    }
            //    _Log.WriteEntry(strMessage, EventLogEntryType.Error);

            //}
            //catch (Exception)
            //{
            //}
        }

        protected void ErrorHandler(Exception exnException)
        {
            LogError(exnException);
            throw exnException;
        }

        protected void ErrorHandler(string strMessage)
        {
            BCException newException = new(strMessage);
            LogError(newException);
            throw newException;
        }

        #endregion

        #region Definición de Constructores

        protected BCEntity()
        {
            //var currentDir = IISHelper.GetContentRoot(); //Directory.GetCurrentDirectory() Antes de 2.2 funcionaba
            //var builder = new ConfigurationBuilder()
            //     .SetBasePath(currentDir) // requires Microsoft.Extensions.Configuration.Json
            //     .AddJsonFile("appsettings.json") // requires Microsoft.Extensions.Configuration.Json
            //     .AddEnvironmentVariables(); // requires Microsoft.Extensions.Configuration.EnvironmentVariables
            //_ = builder.Build();
        }

        #endregion

    }
}
