using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Helpers.Interfaces;
using Relativity.Services.InstanceDetails;
using Relativity.Services.ServiceProxy;

namespace Helpers.Implementations
{
	public class RelativityVersionHelper : IRelativityVersionHelper
	{
		private string InstanceAddress { get; }
		private string AdminUsername { get; }
		private string AdminPassword { get; }
		private IRestHelper RestHelper { get; set; }

		public RelativityVersionHelper(IRestHelper restHelper, string instanceAddress, string adminUsername, string adminPassword)
		{
			InstanceAddress = instanceAddress;
			AdminUsername = adminUsername;
			AdminPassword = adminPassword;
			RestHelper = restHelper;
		}

		public async Task ConfirmInstallerAndInstanceRelativityVersionAreEqual(string installerRelativityVersion)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
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
