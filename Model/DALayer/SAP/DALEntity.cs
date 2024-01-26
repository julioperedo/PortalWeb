using BEntities.Filters;
using Dapper;
using Microsoft.Extensions.Configuration;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BE = BEntities;

namespace DALayer.SAP.Hana
{
	[Serializable()]
	abstract public class DALEntity<T> : IDisposable
	{
		#region Enumerator

		protected enum ErrorPolicy : byte
		{
			DALReplace = 1,
			DALWrap = 2,
			DALNew = 3,
			DALThrow = 4
		}

		#endregion

		#region Mustoverride Methods

		abstract protected void LoadRelations(ref T Item, params Enum[] Relations);
		abstract protected void LoadRelations(ref IEnumerable<T> Items, params Enum[] Relations);

		#endregion

		#region Properties

		//public string ErrorMessage { get; set; }
		protected HanaCommand Command { get; set; }
		public HanaConnection Connection { get; set; }

		public string DBSA { get; set; }
		public string DBIQ { get; set; }
		public string DBLA { get; set; }
		public string DBSA_PL { get; set; }

		#endregion

		#region Declaracion de Metodos

		protected HanaDataReader GenericReader(string strQuery)
		{
			strQuery = strQuery.Replace("ISNULL", "IFNULL");
			HanaDataReader Reader = null;
			try
			{
				VerifyConnection();
				HanaCommand CommandList = new(strQuery, Connection);
				Reader = CommandList.ExecuteReader();
			}
			catch (Exception Err)
			{
				this.ErrorHandler(Err);
			}
			return Reader;
		}

		protected async Task<HanaDataReader> GenericReaderAsync(string strQuery)
		{
			strQuery = strQuery.Replace("ISNULL", "IFNULL");
			HanaDataReader Reader = null;
			try
			{
				VerifyConnection();
				HanaCommand CommandList = new(strQuery, Connection);
				Reader = (HanaDataReader)await CommandList.ExecuteReaderAsync();
			}
			catch (Exception Err)
			{
				this.ErrorHandler(Err);
			}
			return Reader;
		}

		private static IEnumerable<T> MapToEnumerable(IDataReader dr)
		{
			IEnumerable<T> list = Enumerable.Empty<T>();
			List<string> columns = new();
			for (int i = 0; i <= dr.FieldCount - 1; i++)
			{
				columns.Add(dr.GetName(i));
			}
			while (dr.Read())
			{
				T obj = Activator.CreateInstance<T>();
				foreach (PropertyInfo prop in obj.GetType().GetProperties())
				{
					if (columns.Contains(prop.Name))
					{
						try
						{
							if (!object.Equals(dr[prop.Name], DBNull.Value))
							{
								if (dr[prop.Name].GetType().Name == "HanaDecimal")
									prop.SetValue(obj, ((HanaDecimal)dr[prop.Name]).ToDecimal(), null);
								else if (dr[prop.Name].GetType().Name == "Byte" | dr[prop.Name].GetType().Name == "System.Byte")
									prop.SetValue(obj, Convert.ToBoolean((byte)dr[prop.Name]), null);
								else
								{
									prop.SetValue(obj, dr[prop.Name], null);
								}
							}
						}
						catch (Exception)
						{
							throw;
						}
					}
				}
				list = list.Concat(new[] { obj });
			}
			return list;
		}

		private static IEnumerable<T1> MapToEnumerable<T1>(IDataReader dr)
		{
			IEnumerable<T1> list = Enumerable.Empty<T1>();
			List<string> columns = new();
			for (int i = 0; i <= dr.FieldCount - 1; i++)
			{
				columns.Add(dr.GetName(i));
			}
			while (dr.Read())
			{
				T1 obj = Activator.CreateInstance<T1>();
				foreach (PropertyInfo prop in obj.GetType().GetProperties())
				{
					if (columns.Contains(prop.Name))
					{
						try
						{
							if (!object.Equals(dr[prop.Name], DBNull.Value))
							{
								if (dr[prop.Name].GetType().Name == "HanaDecimal")
									prop.SetValue(obj, ((HanaDecimal)dr[prop.Name]).ToDecimal(), null);
								else if (dr[prop.Name].GetType().Name == "Byte" | dr[prop.Name].GetType().Name == "System.Byte")
									prop.SetValue(obj, Convert.ToBoolean((byte)dr[prop.Name]), null);
								else
								{
									prop.SetValue(obj, dr[prop.Name], null);
								}
							}
						}
						catch (Exception)
						{
							throw;
						}
					}
				}
				list = list.Concat(new[] { obj });
			}
			return list;
		}

		private static T MapToEntity(IDataReader dr)
		{
			T obj = default;
			List<string> columns = new();
			for (int i = 0; i <= dr.FieldCount - 1; i++)
			{
				columns.Add(dr.GetName(i));
			}
			while (dr.Read())
			{
				obj = Activator.CreateInstance<T>();
				foreach (PropertyInfo prop in obj.GetType().GetProperties())
				{
					if (columns.Contains(prop.Name))
					{
						if (!object.Equals(dr[prop.Name], DBNull.Value))
						{
							if (dr[prop.Name].GetType().Name == "HanaDecimal")
								prop.SetValue(obj, ((HanaDecimal)dr[prop.Name]).ToDecimal(), null);
							else if (dr[prop.Name].GetType().Name == "Byte" | dr[prop.Name].GetType().Name == "System.Byte")
								prop.SetValue(obj, Convert.ToBoolean((byte)dr[prop.Name]), null);
							else
							{
								prop.SetValue(obj, dr[prop.Name], null);
							}
						}
					}
				}
			}
			return obj;
		}

		private static T1 MapToEntity<T1>(IDataReader dr)
		{
			T1 obj = default;
			List<string> columns = new();
			for (int i = 0; i <= dr.FieldCount - 1; i++)
			{
				columns.Add(dr.GetName(i));
			}
			while (dr.Read())
			{
				obj = Activator.CreateInstance<T1>();
				foreach (PropertyInfo prop in obj.GetType().GetProperties())
				{
					if (columns.Contains(prop.Name))
					{
						if (!object.Equals(dr[prop.Name], DBNull.Value))
						{
							if (dr[prop.Name].GetType().Name == "HanaDecimal")
								prop.SetValue(obj, ((HanaDecimal)dr[prop.Name]).ToDecimal(), null);
							else if (dr[prop.Name].GetType().Name == "Byte" | dr[prop.Name].GetType().Name == "System.Byte")
								prop.SetValue(obj, Convert.ToBoolean((byte)dr[prop.Name]), null);
							else
							{
								prop.SetValue(obj, dr[prop.Name], null);
							}
						}
					}
				}
			}
			return obj;
		}

		protected IEnumerable<T> SQLList(string Query, params Enum[] Relations) //where T : BE.BEntity
		{
			IEnumerable<T> items = Enumerable.Empty<T>();
			IDataReader dtrReader = null;
			try
			{
				dtrReader = GenericReader(Query);
				items = MapToEnumerable(dtrReader);
				if (items != null && items.Any()) LoadRelations(ref items, Relations);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
			finally
			{
				DisposeReader(ref dtrReader);
				if (Relations.Length <= 0) DisposeConnection();
			}
			return items;
		}

		protected async Task<IEnumerable<T>> SQLListAsync(string Query, params Enum[] Relations) //where T : BE.BEntity
		{
			IEnumerable<T> items = Enumerable.Empty<T>();
			IDataReader dtrReader = null;
			try
			{
				dtrReader = await GenericReaderAsync(Query);
				items = MapToEnumerable(dtrReader);
				if (items != null && items.Any()) LoadRelations(ref items, Relations);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
			finally
			{
				DisposeReader(ref dtrReader);
				if (Relations.Length <= 0) DisposeConnection();
			}
			return items;
		}

		protected IEnumerable<T1> SQLList<T1>(string Query, params Enum[] relations) //where T : BE.BEntity
		{
			IEnumerable<T1> items = Enumerable.Empty<T1>();
			IDataReader dtrReader = null;
			try
			{
				dtrReader = GenericReader(Query);
				items = MapToEnumerable<T1>(dtrReader);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
			finally
			{
				DisposeReader(ref dtrReader);
				DisposeConnection();
			}
			return items;
		}

		protected T SQLSearch(string strQuery, params Enum[] Relations) //where T : BE.BEntity, new()
		{
			IDataReader reader = null;
			T BEEntidad = default;
			try
			{
				reader = GenericReader(strQuery);
				BEEntidad = MapToEntity(reader);
				if (BEEntidad != null) LoadRelations(ref BEEntidad, Relations);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
			finally
			{
				DisposeReader(ref reader);
				DisposeConnection();
			}
			return BEEntidad;
		}

		protected T1 SQLSearch<T1>(string strQuery, params Enum[] Relations) //where T : BE.BEntity, new()
		{
			IDataReader reader = null;
			T1 BEEntidad = default;
			try
			{
				reader = GenericReader(strQuery);
				BEEntidad = MapToEntity<T1>(reader);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
			finally
			{
				DisposeReader(ref reader);
				DisposeConnection();
			}
			return BEEntidad;
		}

		protected object Value(string strQuery)
		{
			object objResult = null;
			HanaCommand cmdCommand = new(strQuery, Connection);
			try
			{
				VerifyConnection();
				objResult = cmdCommand.ExecuteScalar();
			}
			catch (Exception Err)
			{
				this.ErrorHandler(Err);
			}
			return objResult;
		}

		protected void DisposeReader(ref IDataReader dtrReader)
		{
			try
			{
				if (dtrReader != null)
				{
					if (!dtrReader.IsClosed)
						dtrReader.Close();
					dtrReader.Dispose();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
			dtrReader = null;
		}

		protected void DisposeConnection()
		{
			try
			{
				if (this.Connection != null)
				{
					if (this.Connection.State != ConnectionState.Closed)
						this.Connection.Close();
					this.Connection.Dispose();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
			this.Connection = null;
		}

		protected void DisposeCommand()
		{
			Command?.Dispose();
			Command = null;
		}

		protected void ErrorHandler(Exception exnException)
		{
			//Dim rethrow As Boolean = ExceptionPolicy.HandleException(exnException, Policy.ToString)
			//If (rethrow) Then
			throw exnException;
			//End If
		}

		//public static IEnumerable<T> Add(this IEnumerable<T> e, T value)
		//{
		//	foreach (var cur in e)
		//	{
		//		yield return cur;
		//	}
		//	yield return value;
		//}

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

		protected string GetOrder(string OrderBy)
		{
			if (string.IsNullOrWhiteSpace(OrderBy))
			{
				return "1";
			}
			else
			{
				return string.Join(",", OrderBy.Trim().Split(',').Select(x => x.Trim().All(char.IsDigit) ? x : $@"{FixNameHana(x.Trim())}"));
			}

		}

		#endregion

		#region List Filters

		protected string GetFilter(params Field[] FilterFields)
		{
			string filters = "1 = 1";
			var dicOperators = GetOperators();
			List<string> sentences = new();

			if (FilterFields?.GetUpperBound(0) >= 0)
			{
				for (int intPos = 0; intPos <= FilterFields.GetUpperBound(0); intPos++)
				{
					Field current = FilterFields[intPos];
					if (current.LogicalOperator == LogicalOperators.None)
					{
						if (!string.IsNullOrWhiteSpace(current.Name))
						{
							if (current.Value != null | current.Operator == Operators.IsNull | current.Operator == Operators.IsNotNull)
							{
								if (current.Operator == Operators.IsNull | current.Operator == Operators.IsNotNull)
									sentences.Add(string.Format(dicOperators[current.Operator], FixNameHana(current.Name)));
								else
								{
									string currentValue = current.Value.ToString();
									if (!IsNumber(current.Value))
									{
										if (current.Operator == Operators.Likes | current.Operator == Operators.NotLikes)
										{
											currentValue = $"'%{current.Value}%'";
										}
										else if (current.Operator == Operators.In | current.Operator == Operators.NotIn)
										{
											if (current.Value.GetType().IsArray | current.Value.GetType().IsGenericType)
											{
												if (current.Value.GetType().FullName.Contains("Int32"))
												{
													currentValue = string.Join(",", (from x in (IEnumerable<int>)current.Value select x));
												}
												else if (current.Value.GetType().FullName.Contains("Int64"))
												{
													currentValue = string.Join(",", (from x in (IEnumerable<long>)current.Value select x));
												}
												else
												{
													currentValue = string.Join(",", (from x in (IEnumerable<string>)current.Value select $"'{x}'"));
												}
											}
										}
										else
										{
											currentValue = $"'{current.Value}'";
										}
									}
									sentences.Add(string.Format(dicOperators[current.Operator], FixNameHana(current.Name), currentValue));
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

		private static Dictionary<Operators, String> GetOperators()
		{
			var dic = new Dictionary<Operators, String> {
				{ Operators.Equal, " {0} = {1} " },
				{ Operators.Different, " {0} <> {1} " },
				{ Operators.HigherOrEqualThan, " {0} >= {1} " },
				{ Operators.HigherThan, " {0} > {1} " },
				{ Operators.Likes, " {0} LIKE {1} " },
				{ Operators.LowerOrEqualThan, " {0} <= {1} " },
				{ Operators.LowerThan, " {0} < {1} " },
				{ Operators.NotLikes, " {0} NOT LIKE {1} " },
				{ Operators.In, " {0} IN ( {1} ) " },
				{ Operators.NotIn, " {0} NOT IN ( {1} ) " },
				{ Operators.IsNull, " {0} IS NULL " },
				{ Operators.IsNotNull, " {0} IS NOT NULL " }
			};
			return dic;
		}

		private static string FixNameHana(string Name)
		{
			string[] reservedWords = { "LOWER(", "UPPER(", "IFNULL(", "CAST(", " AS DATE", " AS INT", " AS VARCHAR", "RTRIM(", "LTRIM(", " ASC", " DESC", "(", ")", "+", "-", "*", "/", "||", "," };
			string strName = System.Text.RegularExpressions.Regex.Replace(Name, @"\d+(?=[^(]*\))", ""); //quito los numeros dentro de paréntesis
			foreach (var item in reservedWords)
			{
				strName = strName.Replace(item, "");
			}
			strName = System.Text.RegularExpressions.Regex.Replace(strName, " {2,}", " ");  //Quito todos los espacios innecesarios dejando un solo espacio
			var words = strName.Split(' ');
			string result = Name;
			foreach (var item in words.Distinct())
			{
				if (!item.StartsWith("'") & !item.EndsWith("'") & !item.Trim().All(char.IsDigit))
				{
					result = item.Contains('.') ? result.Replace(item.Split('.').Last(), $@"""{item.Split('.').Last()}""") : result.Replace(item, $@"""{item}""");
				}
			}
			return result;
		}

		private static bool IsNumber(object value)
		{
			return value is sbyte
					|| value is byte
					|| value is short
					|| value is ushort
					|| value is int
					|| value is uint
					|| value is long
					|| value is ulong
					|| value is float
					|| value is double
					|| value is decimal;
		}

		#endregion

		#region Constructors

		protected DALEntity()
		{
			GenerateConnection("SAPHana");
		}

		protected DALEntity(string ConnectionName)
		{
			GenerateConnection(ConnectionName);
		}

		public DALEntity(HanaConnection connection)
		{
			if (connection == null) GenerateConnection("SAPHana");
			else Connection = connection;
		}

		private void GenerateConnection(string ConnectionName)
		{
			try
			{
				var currentDir = IISHelper.GetContentRoot() ?? Environment.CurrentDirectory; //Directory.GetCurrentDirectory() Antes de 2.2 funcionaba
				var builder = new ConfigurationBuilder()
					 .SetBasePath(currentDir) // requires Microsoft.Extensions.Configuration.Json
					 .AddJsonFile("appsettings.json") // requires Microsoft.Extensions.Configuration.Json
					 .AddJsonFile("appsettings.Development.json")
					 .AddEnvironmentVariables(); // requires Microsoft.Extensions.Configuration.EnvironmentVariables
				IConfigurationRoot Configuration = builder.Build();
				Connection = new HanaConnection(Configuration.GetConnectionString(ConnectionName));
				var sapSettings = Configuration.GetSection("SAPSettings").GetChildren();
				DBSA = sapSettings.FirstOrDefault(x => x.Key == "DBSA")?.Value ?? "DMC_SA";
				DBIQ = sapSettings.FirstOrDefault(x => x.Key == "DBIQ")?.Value ?? "DMC_IQUIQUE";
				DBLA = sapSettings.FirstOrDefault(x => x.Key == "DBLA")?.Value ?? "DMC_LATINAMERICA";
				DBSA_PL = sapSettings.FirstOrDefault(x => x.Key == "DBSA_PL")?.Value ?? "PL_DMC_SA";
			}
			catch (Exception ex)
			{
				ErrorHandler(ex);
			}
		}

		private void VerifyConnection()
		{
			if (Connection == null) GenerateConnection("SAPHana");
			if (Connection.State != ConnectionState.Open) Connection.Open();
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
					//if(sqlConection != null && sqlConection.State != ConnectionState.Closed) {
					if (Connection != null && Connection.State != ConnectionState.Closed)
						this.DisposeConnection();
				}
			}

			// TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			// TODO: set large fields to null.
			this.disposedValue = true;
		}

		// TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
		//Protected Overrides Sub Finalize()
		//    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
		//    Dispose(False)
		//    MyBase.Finalize()
		//End Sub

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
