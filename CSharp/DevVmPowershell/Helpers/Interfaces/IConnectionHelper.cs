using kCura.Relativity.ImportAPI;
using Relativity.Services.ServiceProxy;
using System.Data.SqlClient;

namespace Helpers.Interfaces
{
	public interface IConnectionHelper
	{
		ServiceFactory GetServiceFactory(string protocol = Constants.Connection.PROTOCOL);

		ImportAPI GetImportApi();

		SqlConnection GetSqlConnection(string sqlDatabaseName, string connectionTimeOut);
	}
}