using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Helpers
{
	public interface ISqlRunner : IDisposable
	{
		T ExecuteSqlStatementAsScalar<T>(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = 30);
		T ExecuteSqlStatementAsScalar<T>(string sqlStatement);
		DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS);
		DataTable ExecuteSqlStatementAsDataTable(string sqlStatement);
		DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS);
		DataSet ExecuteSqlStatementAsDataSet(string sqlStatement);
		int ExecuteNonQuerySqlStatement(string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS);
		int ExecuteNonQuerySqlStatement(string sqlStatement);
	}
}
