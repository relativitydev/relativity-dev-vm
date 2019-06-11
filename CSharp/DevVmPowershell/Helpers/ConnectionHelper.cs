using kCura.Relativity.ImportAPI;
using Relativity.Services.ServiceProxy;
using System;
using Relativity.API;

namespace Helpers
{
	public class ConnectionHelper : IConnectionHelper
	{
		private ServiceFactory _serviceFactory;
		public string RelativityInstanceName;
		public string RelativityAdminUserName;
		public string RelativityAdminPassword;

		public ConnectionHelper(string relativityInstanceName, string relativityAdminUserName, string relativityAdminPassword)
		{
			this.RelativityInstanceName = relativityInstanceName;
			this.RelativityAdminUserName = relativityAdminUserName;
			this.RelativityAdminPassword = relativityAdminPassword;
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

		public IDBContext GetDbContext(int workspaceId)
		{
			IHelper helper = _serviceFactory.CreateProxy<IHelper>();
			IDBContext dbContext = helper.GetDBContext(workspaceId);
			return dbContext;
		}
	}
}
