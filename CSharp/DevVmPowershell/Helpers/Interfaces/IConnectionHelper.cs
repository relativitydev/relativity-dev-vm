using kCura.Relativity.ImportAPI;
using Relativity.Services.ServiceProxy;
using System.Data.SqlClient;

namespace Helpers.Interfaces
{
	public interface IConnectionHelper
	{
		string RelativityInstanceName { get; set; }
		string RelativityAdminUserName { get; set; }
		string RelativityAdminPassword { get; set; }
		string SqlAdminUserName { get; set; }
		string SqlAdminPassword { get; set; }
		ServiceFactory GetServiceFactory(string protocol = Constants.Connection.PROTOCOL);
		ImportAPI GetImportApi();
		SqlConnection GetSqlConnection(string sqlDatabaseName, string connectionTimeOut);
	}
}