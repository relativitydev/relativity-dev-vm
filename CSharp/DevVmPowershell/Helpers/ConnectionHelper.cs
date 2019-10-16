using kCura.Relativity.ImportAPI;
using Relativity.Services.ServiceProxy;
using System;
using System.Data.SqlClient;

namespace Helpers
{
	public class ConnectionHelper : IConnectionHelper
	{
		private ServiceFactory _serviceFactory;
		public string RelativityInstanceName;
		public string RelativityAdminUserName;
		public string RelativityAdminPassword;
		public string SqlAdminUserName;
		public string SqlAdminPassword;


		public ConnectionHelper(string relativityInstanceName, string relativityAdminUserName, string relativityAdminPassword, string sqlAdminUserName, string sqlAdminPassword)
		{
			RelativityInstanceName = (string.IsNullOrEmpty(relativityInstanceName)) ? throw new ArgumentNullException(nameof(relativityInstanceName)) : relativityInstanceName;
			RelativityAdminUserName = (string.IsNullOrEmpty(relativityAdminUserName)) ? throw new ArgumentNullException(nameof(relativityAdminUserName)) : relativityAdminUserName;
			RelativityAdminPassword = (string.IsNullOrEmpty(relativityAdminPassword)) ? throw new ArgumentNullException(nameof(relativityAdminPassword)) : relativityAdminPassword;
			SqlAdminUserName = (string.IsNullOrEmpty(sqlAdminUserName)) ? throw new ArgumentNullException(nameof(sqlAdminUserName)) : sqlAdminUserName;
			SqlAdminPassword = (string.IsNullOrEmpty(sqlAdminPassword)) ? throw new ArgumentNullException(nameof(sqlAdminPassword)) : sqlAdminPassword;
		}

		public ServiceFactory GetServiceFactory(string protocol = Constants.Connection.PROTOCOL)
		{
			if (_serviceFactory == null)
			{
				Uri relativityServicesUri = new Uri($"{protocol}://{RelativityInstanceName}/Relativity.Services");
				Uri relativityRestUri = new Uri($"{protocol}://{RelativityInstanceName}/Relativity.Rest/Api");
				UsernamePasswordCredentials usernamePasswordCredentials = new UsernamePasswordCredentials(
					username: RelativityAdminUserName,
					password: RelativityAdminPassword);
				ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
					relativityServicesUri: relativityServicesUri,
					relativityRestUri: relativityRestUri,
					credentials: usernamePasswordCredentials);
				_serviceFactory = new ServiceFactory(
					settings: serviceFactorySettings);
			}

			return _serviceFactory;
		}

		public ImportAPI GetImportApi()
		{
			string webServiceUrl = $@"{Constants.Connection.PROTOCOL}://{RelativityInstanceName}/relativitywebapi/";

			return new ImportAPI(RelativityAdminUserName, RelativityAdminPassword, webServiceUrl);
		}

		/// <summary>
		/// Creates a SqlConnection for when DBContext doesn't give you enough power. 
		/// </summary>
		/// <param name="sqlDatabaseName"></param>
		/// <param name="connectionTimeOut"></param>
		/// <returns></returns>
		public SqlConnection GetSqlConnection(string sqlDatabaseName, string connectionTimeOut)
		{
			string connectionString = $"data source={RelativityInstanceName};initial catalog={sqlDatabaseName};persist security info={Constants.Connection.Sql.CONNECTION_STRING_PERSIST_SECURITY_INFO};user id={SqlAdminUserName};password={SqlAdminPassword};packet size={Constants.Connection.Sql.CONNECTION_STRING_PACKET_SIZE};connect timeout={connectionTimeOut};";

			return new SqlConnection(connectionString);
		}
	}
}
