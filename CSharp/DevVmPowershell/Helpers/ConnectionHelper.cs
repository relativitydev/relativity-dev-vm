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
		public string RelativityDatabase;

		public ConnectionHelper(string relativityInstanceName, string relativityAdminUserName, string relativityAdminPassword)
		{
			this.RelativityInstanceName = relativityInstanceName;
			this.RelativityAdminUserName = relativityAdminUserName;
			this.RelativityAdminPassword = relativityAdminPassword;
		}

		public ConnectionHelper(string relativityInstanceName, string relativityAdminUserName, string relativityAdminPassword, string relativityDatabase)
		{
			this.RelativityInstanceName = relativityInstanceName;
			this.RelativityAdminUserName = relativityAdminUserName;
			this.RelativityAdminPassword = relativityAdminPassword;
			this.RelativityDatabase = relativityDatabase;
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
		/// Leave the connectionTimeout to blank if you want to use the default (30 seconds)
		/// </summary>
		/// <param name="connectionTimeOut"></param>
		/// <returns></returns>
		public SqlConnection GetSqlConnection(string connectionTimeOut = Constants.Connection.Sql.ConnectionString_ConnectTimeoutDefault)
		{
			string connectionString = $"data source={RelativityInstanceName};initial catalog={RelativityDatabase};persist security info={Constants.Connection.Sql.ConnectionString_PersistSecurityInfo};user id={RelativityAdminUserName};password={RelativityAdminPassword};packet size={Constants.Connection.Sql.ConnectionString_PacketSize};connect timeout={connectionTimeOut};";

			return new SqlConnection(connectionString);
		}
	}
}
