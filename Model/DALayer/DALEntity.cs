using BEntities.Filters;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BE = BEntities;

namespace DALayer
{
    [Serializable()]
    abstract public class DALEntity<T> : IDisposable
    {
        #region Enumerator

        #endregion

        #region Variables

        #endregion

        #region Mustoverride Methods

        abstract protected void LoadRelations(ref T Item, params Enum[] Relations);
        abstract protected void LoadRelations(ref IEnumerable<T> Items, params Enum[] Relations);

        #endregion

        #region Properties

        public string ErrorMessage { get; set; }
        protected SqlConnection Connection { get; set; }
        //protected SqlTransaction Transaction { get; set; }

        #endregion

        #region Declaracion de Metodos Para Generar IDs

        protected long GenID(string strTableName, int IdQuantity)
        {
            long lngGenId = 0;
            var p = new DynamicParameters();
            p.Add("Table", strTableName);
            p.Add("CounterId", IdQuantity);
            p.Add("Id", lngGenId, DbType.Int64, ParameterDirection.Output);
            _ = Connection.QueryFirstOrDefault<Int64>("dbo.GeneratorId", p, commandType: CommandType.StoredProcedure);
            lngGenId = p.Get<Int64>("Id");
            return lngGenId;
        }

        #endregion

        #region Declaracion de Metodos

        protected IEnumerable<T> SQLList(string strQuery, params Enum[] Relations) //where T : BE.BEntity
        {
            IEnumerable<T> items = default;
            try
            {
                items = Connection.Query<T>(strQuery);
                if (items?.Count() > 0) LoadRelations(ref items, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return items;
        }

        protected IEnumerable<T> SQLList(string strQuery, object parameters, params Enum[] Relations) //where T : BE.BEntity
        {
            IEnumerable<T> items = default;
            try
            {
                items = Connection.Query<T>(strQuery, parameters);
                if (items?.Count() > 0) LoadRelations(ref items, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return items;
        }

        protected async Task<IEnumerable<T>> SQLListAsync(string strQuery, params Enum[] Relations) //where T : BE.BEntity
        {
            IEnumerable<T> items = default;
            try
            {
                items = (await Connection.QueryAsync<T>(strQuery));
                if (items?.Count() > 0) LoadRelations(ref items, Relations);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return items;
        }

        protected async Task<IEnumerable<T>> SQLListAsync(string strQuery, object parameters, params Enum[] Relations) //where T : BE.BEntity
        {
            IEnumerable<T> items = default;
            try
            {
                items = (await Connection.QueryAsync<T>(strQuery, parameters));
                if (items?.Count() > 0) LoadRelations(ref items, Relations);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return items;
        }

        protected T SQLSearch(string strQuery, object parameters, params Enum[] Relations) //where T : BE.BEntity, new()
        {
            T item = default;
            try
            {
                item = Connection.QueryFirstOrDefault<T>(strQuery, parameters);
                if (item != null) LoadRelations(ref item, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return item;
        }

        protected T SQLSearch(string strQuery, params Enum[] Relations) //where T : BE.BEntity, new()
        {
            T item = default;
            try
            {
                item = Connection.QueryFirstOrDefault<T>(strQuery);
                if (item != null) LoadRelations(ref item, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return item;
        }

        protected async Task<T> SQLSearchAsync(string strQuery, object parameters, params Enum[] Relations) //where T : BE.BEntity, new()
        {
            T item = default;
            try
            {
                item = await Connection.QueryFirstOrDefaultAsync<T>(strQuery, parameters);
                if (item != null) LoadRelations(ref item, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return item;
        }

        protected async Task<T> SQLSearchAsync(string strQuery, params Enum[] Relations) //where T : BE.BEntity, new()
        {
            T item = default;
            try
            {
                item = await Connection.QueryFirstOrDefaultAsync<T>(strQuery);
                if (item != null) LoadRelations(ref item, Relations);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            finally
            {
                if (Relations.Length <= 0) DisposeConnection();
            }
            return item;
        }

        protected T1 SQLScalar<T1>(string Query)
        {
            T1 item = default;
            try
            {
                item = Connection.ExecuteScalar<T1>(Query);
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
            finally
            {
                DisposeConnection();
            }
            return item;
        }

        protected void DisposeConnection()
        {
            try
            {
                if (this.Connection != null)
                {
                    if (this.Connection.State != ConnectionState.Closed)
                    {
                        this.Connection.Close();
                    }
                    this.Connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
            this.Connection = null;
        }

        protected void ErrorHandler(Exception exnException)
        {
            //Dim rethrow As Boolean = ExceptionPolicy.HandleException(exnException, Policy.ToString)
            //If (rethrow) Then
            throw exnException;
            //End If
        }

        #endregion

        #region Special Methods

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

        #endregion

        #region List Filters

        protected (string, DynamicParameters) GetFilter(params Field[] FilterFields)
        {
            string filters = "";
            DynamicParameters parameters = new DynamicParameters();
            var dicOperators = GetOperators();
            List<string> sentences = new List<string>();

            if (FilterFields?.GetUpperBound(0) >= 0)
            {
                int paramCount = 0;
                for (int intPos = 0; intPos <= FilterFields.GetUpperBound(0); intPos++)
                {
                    Field current = FilterFields[intPos];
                    if (current.LogicalOperator == LogicalOperators.None)
                    {
                        if (!string.IsNullOrWhiteSpace(current.Name))
                        {
                            if (current.Value != null | current.Operator == Operators.IsNull | current.Operator == Operators.IsNotNull)
                            {
                                string paramName = $"param{paramCount++}";
                                if (current.Operator == Operators.IsNull | current.Operator == Operators.IsNotNull)
                                {
                                    sentences.Add(string.Format(dicOperators[current.Operator], current.Name));
                                }
                                else
                                {
                                    if (current.Operator == Operators.In | current.Operator == Operators.NotIn)
                                    {
                                        if (current.Value is Array)
                                        {
                                            sentences.Add(string.Format(dicOperators[current.Operator], current.Name, paramName));
                                        }
                                        else
                                        {
                                            sentences.Add(string.Format(dicOperators[current.Operator].Replace("@", ""), current.Name, $"( {current.Value} )"));
                                        }
                                    }
                                    else
                                    {
                                        sentences.Add(string.Format(dicOperators[current.Operator], current.Name, paramName));
                                    }
                                }
                                if (current.Value != null)
                                {
                                    if (current.Operator == Operators.Likes | current.Operator == Operators.NotLikes)
                                    {
                                        parameters.Add(paramName, "%" + current.Value.ToString().Replace("[", "[[]").Replace("%", "[%]") + "%");
                                    }
                                    else if (current.Operator == Operators.In | current.Operator == Operators.NotIn)
                                    {
                                        if (current.Value is Array)
                                        {
                                            parameters.Add(paramName, (Array)current.Value);
                                        }
                                    }
                                    else
                                    {
                                        parameters.Add(paramName, current.Value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (sentences.Count > 1)
                        {
                            string sOperator = current.LogicalOperator == LogicalOperators.And ? "AND" : "OR";
                            string operatingTwo = sentences.Last();
                            sentences.RemoveAt(sentences.Count - 1);
                            string operatingOne = sentences.Last();
                            sentences.RemoveAt(sentences.Count - 1);

                            sentences.Add(string.Format(" ( {0} {1} {2} ) ", operatingOne, sOperator, operatingTwo));
                        }
                    }
                }
                filters = sentences.First() ?? "1 = 1";
            }
            return (filters, parameters);
        }

        protected string GetFilterString(params Field[] FilterFields)
        {
            string filters = "";
            var dicOperators = GetOperators();
            List<string> sentences = new();

            if (FilterFields?.GetUpperBound(0) >= 0)
            {
                //int paramCount = 0;
                for (int intPos = 0; intPos <= FilterFields.GetUpperBound(0); intPos++)
                {
                    Field current = FilterFields[intPos];
                    if (current.LogicalOperator == LogicalOperators.None)
                    {
                        if (!string.IsNullOrWhiteSpace(current.Name))
                        {
                            if (current.Value != null | current.Operator == Operators.IsNull | current.Operator == Operators.IsNotNull)
                            {
                                //string paramName = $"param{paramCount++}";
                                if (current.Operator == Operators.IsNull | current.Operator == Operators.IsNotNull)
                                {
                                    sentences.Add(string.Format(dicOperators[current.Operator], current.Name));
                                }
                                else
                                {
                                    if (current.Operator == Operators.In | current.Operator == Operators.NotIn)
                                    {
                                        if (current.Value is Array)
                                        {
                                            string value = "";
                                            if (((IEnumerable)current.Value).Cast<object>().First().GetType().Name == "String")
                                            {
                                                value = string.Join(",", ((IEnumerable)current.Value).Cast<object>().Select(x => $"'{x}'"));
                                            }
                                            else
                                            {
                                                value = string.Join(",", ((IEnumerable)current.Value).Cast<object>().Select(x => x));
                                            }
                                            sentences.Add(string.Format(dicOperators[current.Operator].Replace("@", ""), current.Name, $"( {value} )"));
                                        }
                                        else
                                        {
                                            sentences.Add(string.Format(dicOperators[current.Operator].Replace("@", ""), current.Name, $"( {current.Value} )"));
                                        }
                                    }
                                    else
                                    {
                                        //string value = current.Value.GetType().Name == "String" ? (current.Operator == Operators.Likes || current.Operator == Operators.NotLikes ? $"'%{current.Value}%'" : $"'{current.Value}'") : current.Value.ToString();
                                        string value;
                                        if (current.Value.GetType().Name == "String")
                                        {
                                            value = current.Operator == Operators.Likes || current.Operator == Operators.NotLikes ? $"'%{current.Value}%'" : $"'{current.Value}'";
                                        }
                                        else if (current.Value.GetType().Name == "Boolean")
                                        {
                                            value = (Boolean)current.Value == true ? "1" : "0";
                                        }
                                        else
                                        {
                                            value = current.Value.ToString();
                                        }
                                        sentences.Add(string.Format(dicOperators[current.Operator].Replace("@", ""), current.Name, value));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (sentences.Count > 1)
                        {
                            string sOperator = current.LogicalOperator == LogicalOperators.And ? "AND" : "OR";
                            string operatingTwo = sentences.Last();
                            sentences.RemoveAt(sentences.Count - 1);
                            string operatingOne = sentences.Last();
                            sentences.RemoveAt(sentences.Count - 1);

                            sentences.Add(string.Format(" ( {0} {1} {2} ) ", operatingOne, sOperator, operatingTwo));
                        }
                    }
                }
                filters = sentences.First() ?? "1 = 1";
            }
            return filters;
        }

        private Dictionary<Operators, String> GetOperators()
        {
            var dic = new Dictionary<Operators, String> {
                { Operators.Equal, " {0} = @{1} " },
                { Operators.Different, " {0} <> @{1} " },
                { Operators.HigherOrEqualThan, " {0} >= @{1} " },
                { Operators.HigherThan, " {0} > @{1} " },
                { Operators.Likes, " {0} LIKE @{1} " },
                { Operators.LowerOrEqualThan, " {0} <= @{1} " },
                { Operators.LowerThan, " {0} < @{1} " },
                { Operators.NotLikes, " {0} NOT LIKE @{1} " },
                { Operators.In, " {0} IN @{1} " },
                { Operators.NotIn, " {0} NOT IN @{1} " },
                { Operators.IsNull, " {0} IS NULL " },
                { Operators.IsNotNull, " {0} IS NOT NULL " }
            };

            return dic;
        }

        #endregion

        #region Constructors

        protected DALEntity()
        {
            GenerateConnection("Negocio");
        }

        protected DALEntity(string ConnectionName)
        {
            GenerateConnection(ConnectionName);
        }

        public DALEntity(SqlConnection connection)
        {
            this.Connection = connection;
        }

        private void GenerateConnection(string ConnectionName)
        {
            try
            {
                var currentDir = IISHelper.GetContentRoot() ?? System.Environment.CurrentDirectory; //Directory.GetCurrentDirectory() Antes de 2.2 funcionaba
                IConfigurationRoot Configuration;
                var builder = new ConfigurationBuilder()
                     .SetBasePath(currentDir) // requires Microsoft.Extensions.Configuration.Json
                     .AddJsonFile("appsettings.json") // requires Microsoft.Extensions.Configuration.Json
                     .AddJsonFile("appsettings.Development.json", optional: true)
                     .AddEnvironmentVariables(); // requires Microsoft.Extensions.Configuration.EnvironmentVariables
                Configuration = builder.Build();

                Connection = new SqlConnection(Configuration.GetConnectionString(ConnectionName));
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        #endregion

        #region IDisposable Support

        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (Connection != null && Connection.State != ConnectionState.Closed)
                    {
                        this.DisposeConnection();
                    }
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            // TODO: set large fields to null.
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
