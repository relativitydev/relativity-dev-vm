using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Helpers
{
	public class SqlRunner : ISqlRunner
	{
		private enum SqlQueryType
		{
			Scalar = 0,
			DataTable = 1,
			NonQuery = 2,
			DataSet = 3
		}
		private IDbConnection SqlConnection { get; }

		public SqlRunner(IDbConnection sqlConnection)
		{
			SqlConnection = sqlConnection;
		}

		public void Dispose()
		{
			if (SqlConnection != null)
			{
				if (SqlConnection.State != ConnectionState.Closed)
				{
					SqlConnection.Close();
				}
				SqlConnection.Dispose();
			}
		}

		private object ExecuteSqlStatement(SqlQueryType sqlQueryType, string sqlStatement, List<SqlParameter> sqlParameters, int timeout)
		{
			try
			{
				if (SqlConnection.State != ConnectionState.Open)
				{
					SqlConnection.Open();
				}

				IDbCommand sqlCommand = SqlConnection.CreateCommand();
				IDbTransaction sqlTransaction = SqlConnection.BeginTransaction();
				sqlCommand.Connection = SqlConnection;
				sqlCommand.Transaction = sqlTransaction;
				sqlCommand.CommandTimeout = timeout;
				sqlCommand.CommandText = sqlStatement;

				if (sqlParameters != null && sqlParameters.ToList().Any())
				{
					foreach (SqlParameter sqlParameter in sqlParameters)
					{
						sqlCommand.Parameters.Add(sqlParameter);
					}
				}

				try
				{
					switch (sqlQueryType)
					{
						case SqlQueryType.Scalar:
							return ProcessScalar(sqlCommand, sqlTransaction);

						case SqlQueryType.DataTable:
							return ProcessDataTable(sqlCommand, sqlTransaction);

						case SqlQueryType.NonQuery:
							return ProcessNonQuery(sqlCommand, sqlTransaction);

						case SqlQueryType.DataSet:
							return ProcessDataSet(sqlCommand, sqlTransaction);

						default:
							throw new Exception($"Not a valid {nameof(sqlQueryType)} [Value: {sqlQueryType}]");
					}
				}
				catch (Exception ex1)
				{
					// Attempt to roll back the transaction
					try
					{
						sqlTransaction.Rollback();
					}
					catch (Exception ex2)
					{
						// This catch block will handle any errors that may have occurred on the server that would cause the rollback to fail, such as a closed connection.
						throw new Exception(Constants.ErrorMessages.Sql.SQL_ROLLBACK_ERROR, ex2);
					}

					throw new Exception(Constants.ErrorMessages.Sql.EXECUTE_SQL_STATEMENT_ERROR, ex1);
				}
				finally
				{
					SqlConnection.Close();
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteSqlStatement)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(sqlStatement))
				{
					throw new ArgumentNullException(nameof(sqlStatement));
				}

				if (timeout < 1)
				{
					throw new ArgumentException($"{nameof(timeout)} [Value: {timeout}] is not valid. '{nameof(timeout)}' should be greater than zero.");
				}

				try
				{
					T returnValue = (T)ExecuteSqlStatement(SqlQueryType.Scalar, sqlStatement, sqlParameters, timeout);
					return returnValue;
				}
				catch (Exception ex)
				{
					throw new Exception(Constants.ErrorMessages.Sql.EXECUTE_SQL_STATEMENT_AS_SCALAR_ERROR, ex);
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteSqlStatementAsScalar)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement)
		{
			try
			{
				return ExecuteSqlStatementAsScalar<T>(sqlStatement, null);
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteSqlStatementAsScalar)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(sqlStatement))
				{
					throw new ArgumentNullException(nameof(sqlStatement));
				}

				if (timeout < 1)
				{
					throw new ArgumentException($"{nameof(timeout)} [Value: {timeout}] is not valid. '{nameof(timeout)}' should be greater than zero.");
				}

				try
				{
					DataTable returnValue = (DataTable)ExecuteSqlStatement(SqlQueryType.DataTable, sqlStatement, sqlParameters, timeout);
					return returnValue;
				}
				catch (Exception ex)
				{
					throw new Exception(Constants.ErrorMessages.Sql.EXECUTE_SQL_STATEMENT_AS_DATATABLE_ERROR, ex);
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteSqlStatementAsDataTable)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(sqlStatement))
				{
					throw new ArgumentNullException(nameof(sqlStatement));
				}

				if (timeout < 1)
				{
					throw new ArgumentException($"{nameof(timeout)} [Value: {timeout}] is not valid. '{nameof(timeout)}' should be greater than zero.");
				}

				try
				{
					DataSet returnValue = (DataSet)ExecuteSqlStatement(SqlQueryType.DataSet, sqlStatement, sqlParameters, timeout);
					return returnValue;
				}
				catch (Exception ex)
				{
					throw new Exception(Constants.ErrorMessages.Sql.EXECUTE_SQL_STATEMENT_AS_DATATABLE_ERROR, ex);
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteSqlStatementAsDataSet)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement)
		{
			try
			{
				return ExecuteSqlStatementAsDataTable(sqlStatement, null);
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteSqlStatementAsDataTable)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement)
		{
			try
			{
				return ExecuteSqlStatementAsDataSet(sqlStatement, null);
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteSqlStatementAsDataSet)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public int ExecuteNonQuerySqlStatement(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(sqlStatement))
				{
					throw new ArgumentNullException(nameof(sqlStatement));
				}

				if (timeout < 1)
				{
					throw new ArgumentException($"{nameof(timeout)} [Value: {timeout}] is not valid. '{nameof(timeout)}' should be greater than zero.");
				}

				try
				{
					int returnValue = (int)ExecuteSqlStatement(SqlQueryType.NonQuery, sqlStatement, sqlParameters, timeout);
					return returnValue;
				}
				catch (Exception ex)
				{
					throw new Exception(Constants.ErrorMessages.Sql.EXECUTE_NON_QUERY_SQL_STATEMENT_ERROR, ex);
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteNonQuerySqlStatement)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		public int ExecuteNonQuerySqlStatement(string sqlStatement)
		{
			try
			{
				return ExecuteNonQuerySqlStatement(sqlStatement, null);
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ExecuteNonQuerySqlStatement)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		private object ProcessScalar(IDbCommand sqlCommand, IDbTransaction sqlTransaction)
		{
			try
			{
				//Execute SQL statement   
				object returnObject = sqlCommand.ExecuteScalar();

				//Attempt to commit the transaction
				sqlTransaction.Commit();

				return returnObject;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ProcessScalar)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		private DataTable ProcessDataTable(IDbCommand sqlCommand, IDbTransaction sqlTransaction)
		{
			try
			{
				//Execute SQL statement   
				IDataReader dataReader = sqlCommand.ExecuteReader();
				DataTable dataTable = new DataTable();
				dataTable.Load(dataReader);
				dataReader.Dispose();

				//Attempt to commit the transaction
				sqlTransaction.Commit();

				return dataTable;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ProcessDataTable)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		private DataSet ProcessDataSet(IDbCommand sqlCommand, IDbTransaction sqlTransaction)
		{
			try
			{
				//Execute SQL statement   
				IDataReader dataReader = sqlCommand.ExecuteReader();
				DataSet dataSet = new DataSet();
				while (!dataReader.IsClosed)
				{
					dataSet.Tables.Add().Load(dataReader);
				}

				dataReader.Dispose();

				//Attempt to commit the transaction
				sqlTransaction.Commit();

				return dataSet;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ProcessDataSet)}' method";
				throw new Exception(errorMessage, ex);
			}
		}

		private int ProcessNonQuery(IDbCommand sqlCommand, IDbTransaction sqlTransaction)
		{
			try
			{
				//Execute SQL statement   
				int returnInt = sqlCommand.ExecuteNonQuery();

				//Attempt to commit the transaction
				sqlTransaction.Commit();

				return returnInt;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in '{nameof(ProcessNonQuery)}' method";
				throw new Exception(errorMessage, ex);
			}
		}
	}
}
