using Helpers.Interfaces;
using Relativity.Services.ServiceProxy;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Implementations
{
	public class AgentServerHelper : IAgentServerHelper
	{
		private IConnectionHelper ConnectionHelper { get; }
		private IRestHelper RestHelper { get; }

		public AgentServerHelper(IConnectionHelper connectionHelper, IRestHelper restHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
		}

		/// <summary>
		/// Add Agent Server to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddAgentServerToDefaultResourcePoolAsync()
		{
			bool wasAgentServerAddedToDefaultPool = false;
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			
			// Get Default Resource Pool Artifact Id
			int defaultResourcePoolArtifactId = await GetDefaultResourcePoolArtifactIdAsync(httpClient);
			if (defaultResourcePoolArtifactId != -1)
			{
				// Get Agent Server Type Artifact Id
				int agentServerTypeArtifactId = await GetAgentServerTypeArtifactIdAsync(httpClient);
				if (agentServerTypeArtifactId != -1)
				{
					// Get Agent Server Artifact Id
					int agentServerArtifactId = await GetAgentServerArtifactIdAsync(httpClient);
					if (agentServerArtifactId != -1)
					{
						// Add Agent Server to Default Resource Pool
						await CallAddAgentServerToDefaultResourcePoolAsync(httpClient, agentServerArtifactId,
							agentServerTypeArtifactId, defaultResourcePoolArtifactId);
						wasAgentServerAddedToDefaultPool = true;
					}
				}
			}

			return wasAgentServerAddedToDefaultPool;
		}

		/// <summary>
		/// Remove Agent Server from Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> RemoveAgentServerFromDefaultResourcePoolAsync()
		{
			bool wasAgentServerRemovedFromDefaultPool = false;
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);

			// Get Default Resource Pool Artifact Id
			int defaultResourcePoolArtifactId = await GetDefaultResourcePoolArtifactIdAsync(httpClient);
			if (defaultResourcePoolArtifactId != -1)
			{
				// Get Agent Server Type Artifact Id
				int agentServerTypeArtifactId = await GetAgentServerTypeArtifactIdAsync(httpClient);
				if (agentServerTypeArtifactId != -1)
				{
					// Get Agent Server Artifact Id
					int agentServerArtifactId = await GetAgentServerArtifactIdAsync(httpClient);
					if (agentServerArtifactId != -1)
					{
						await CallRemoveAgentServerFromDefaultResourcePoolAsync(httpClient, agentServerArtifactId, agentServerTypeArtifactId, defaultResourcePoolArtifactId);
						wasAgentServerRemovedFromDefaultPool = true;
					}
				}
			}

			return wasAgentServerRemovedFromDefaultPool;
		}

		private async Task<int> GetDefaultResourcePoolArtifactIdAsync(HttpClient httpClient)
		{
			// Query for Default Resource Pool
			var resourcePoolQueryPayload = new
			{
				Query = new
				{
					Condition = "'Name' STARTSWITH 'Default'"
				}
			};
			string poolQuery = JsonConvert.SerializeObject(resourcePoolQueryPayload);
			HttpResponseMessage queryPoolResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.QueryEndpointUrl, poolQuery);
			if (!queryPoolResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to query for Default Resource Pool");
			}
			string queryPoolResultString = await queryPoolResponse.Content.ReadAsStringAsync();
			dynamic queryPoolResult = JObject.Parse(queryPoolResultString) as JObject;
			if (Convert.ToInt32(queryPoolResult.TotalCount) > 0)
			{
				return Convert.ToInt32(queryPoolResult.Results[0]["Artifact"]["ArtifactID"].ToString());
			}
			else
			{
				return -1;
			}
		}

		private async Task<int> GetAgentServerTypeArtifactIdAsync(HttpClient httpClient)
		{
			int agentServerTypeArtifactId = -1;
			HttpResponseMessage serverTypeQueryResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.ResourceServerTypeQueryEndpointUrl, "");
			if (!serverTypeQueryResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to query for Agent Server Types");
			}
			string queryServerTypesResultString = await serverTypeQueryResponse.Content.ReadAsStringAsync();
			dynamic queryServerTypesResult = JsonConvert.DeserializeObject<dynamic>(queryServerTypesResultString);
			foreach (dynamic obj in queryServerTypesResult)
			{
				if (obj["Name"].ToString() == "Agent")
				{
					agentServerTypeArtifactId = Convert.ToInt32(obj["ArtifactID"].ToString());
					break;
				}
			}

			return agentServerTypeArtifactId;
		}

		private async Task<int> GetAgentServerArtifactIdAsync(HttpClient httpClient)
		{
			int agentServerArtifactId = -1;
			var resourceServerQueryPayload = new
			{
				Query = new
				{
					Condition = ""
				}
			};
			string serverQuery = JsonConvert.SerializeObject(resourceServerQueryPayload);
			HttpResponseMessage queryServerResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourceServer.QueryEndpointUrl, serverQuery);
			if (!queryServerResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to query for Agent Resource Server");
			}
			string queryServerResultString = await queryServerResponse.Content.ReadAsStringAsync();
			dynamic queryServerResult = JObject.Parse(queryServerResultString) as JObject;
			if (Convert.ToInt32(queryServerResult.TotalCount) > 0)
			{
				foreach (dynamic obj in queryServerResult.Results)
				{
					if (obj["Artifact"]["ServerType"]["Name"].ToString() == "Agent")
					{
						agentServerArtifactId = Convert.ToInt32(obj["Artifact"]["ArtifactID"].ToString());
						break;
					}
				}
			}

			return agentServerArtifactId;
		}

		private async Task CallAddAgentServerToDefaultResourcePoolAsync(HttpClient httpClient, int agentServerArtifactId, int agentServerTypeArtifactId, int defaultResourcePoolArtifactId)
		{
			var addResourceServerPayload = new
			{
				resourceServer = new
				{
					ArtifactID = agentServerArtifactId,
					ServerType = new
					{
						ArtifactID = agentServerTypeArtifactId
					}
				},
				resourcePool = new
				{
					ArtifactID = defaultResourcePoolArtifactId
				}
			};
			string addResourceServer = JsonConvert.SerializeObject(addResourceServerPayload);
			HttpResponseMessage addServerResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.AddServerEndpointUrl, addResourceServer);
			if (!addServerResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to add the Agent Server to the Default Resource Pool");
			}
		}

		private async Task CallRemoveAgentServerFromDefaultResourcePoolAsync(HttpClient httpClient, int agentServerArtifactId, int agentServerTypeArtifactId, int defaultResourcePoolArtifactId)
		{
			var removeAgentServerResourcePoolPayload = new
			{
				resourceServer = new
				{
					ArtifactID = agentServerArtifactId,
					ServerType = new
					{
						ArtifactID = agentServerTypeArtifactId
					}
				},
				resourcePool = new
				{
					ArtifactID = defaultResourcePoolArtifactId
				}
			};
			string removeAgentServer = JsonConvert.SerializeObject(removeAgentServerResourcePoolPayload);
			HttpResponseMessage removeAgentServerResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.RemoveServerUrl, removeAgentServer);
			if (!removeAgentServerResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to remove the Agent Server from the Default Resource Pool");
			}
		}
	}
}