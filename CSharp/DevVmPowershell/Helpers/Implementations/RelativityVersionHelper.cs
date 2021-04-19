using Helpers.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
				string responseContent = await response.Content.ReadAsStringAsync();
				throw new Exception($"Error Getting Instance Relativity Version. [{nameof(responseContent)}: {responseContent}]");
			}
			string relativityVersion = await response.Content.ReadAsStringAsync();
			relativityVersion = relativityVersion.Replace("\\", "").Replace("\"", ""); // Returned string has wrapped characters and this just cleans it up before comparing.
			if (relativityVersion != installerRelativityVersion)
			{
				throw new Exception($"Installed Relativity Version ({relativityVersion}) and Installer Version ({installerRelativityVersion}) are not the same");
			}
		}
	}
}
