using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Helpers
{
	public interface ISqlRunner
	{
		IConnectionHelper ConnectionHelper { get; }

		T ExecuteSqlStatementAsScalar<T>(string sqlDatabaseName, string sqlStatement, List<SqlParameter> sqlParameters, int timeout = 30);
		T ExecuteSqlStatementAsScalar<T>(string sqlDatabaseName, string sqlStatement);
		DataTable ExecuteSqlStatementAsDataTable(string sqlDatabaseName, string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS);
		DataTable ExecuteSqlStatementAsDataTable(string sqlDatabaseName, string sqlStatement);
		DataSet ExecuteSqlStatementAsDataSet(string sqlDatabaseName, string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS);
		DataSet ExecuteSqlStatementAsDataSet(string sqlDatabaseName, string sqlStatement);
		int ExecuteNonQuerySqlStatement(string sqlDatabaseName, string sqlStatement, List<SqlParameter> sqlParameters, int timeout = Constants.Connection.Sql.DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS);
		int ExecuteNonQuerySqlStatement(string sqlDatabaseName, string sqlStatement);
	}
}
