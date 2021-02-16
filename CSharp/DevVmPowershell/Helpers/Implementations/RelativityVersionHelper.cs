using System;
using System.Net.Http;
using System.Threading.Tasks;
using Helpers.Interfaces;

namespace Helpers.Implementations
{
	public class RelativityVersionHelper : IRelativityVersionHelper
	{
		private IConnectionHelper ConnectionHelper { get; }
		private IRestHelper RestHelper { get; set; }

		public RelativityVersionHelper(IConnectionHelper connectionHelper, IRestHelper restHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
		}

		public async Task ConfirmInstallerAndInstanceRelativityVersionAreEqualAsync(string installerRelativityVersion)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.InstanceDetailsService.EndpointUrl, "");
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Error Getting Instance Relativity Version");
			}
			string relativityVersion = await response.Content.ReadAsStringAsync();
			if (relativityVersion != installerRelativityVersion)
			{
				throw new Exception("Installed Relativity Version and Installer Version are not the same");
			}
		}
	}
}
