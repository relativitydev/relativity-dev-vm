using Helpers.Interfaces;
using Relativity.Services.Interfaces.Agent.Models;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Net.Http;
using System.Threading.Tasks;
using Helpers.RequestModels;
using Helpers.ResponseModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Implementations
{
	public class AgentHelper : IAgentHelper
	{
		private IConnectionHelper ConnectionHelper { get; set; }
		private IRestHelper RestHelper { get; set; }

		public AgentHelper(IConnectionHelper connectionHelper, IRestHelper restHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
		}

		private async Task<bool> CheckIfAtLeastSingleInstanceOfAgentExistsAsync(string agentName)
		{
			List<int> agentArtifactIds = await GetAgentArtifactIdsAsync(agentName);

			bool doesAgentExists = agentArtifactIds.Count > 0;
			return doesAgentExists;
		}

		private async Task<List<int>> GetAgentArtifactIdsAsync(string agentName)
		{
			List<int> agentArtifactIds = new List<int>();
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			string url = $"Relativity.REST/api/Relativity.Objects/workspace/{Constants.EDDS_WORKSPACE_ARTIFACT_ID}/object/query";
			ObjectManagerQueryRequestModel objectManagerQueryRequestModel = new ObjectManagerQueryRequestModel
			{
				request = new Request
				{
					objectType = new Helpers.RequestModels.ObjectType
					{
						Name = Constants.Agents.AGENT_OBJECT_TYPE
					},
					fields = new object[]
					{
						new
						{
							Name = Constants.Agents.AGENT_FIELD_NAME
						}
					},
					condition = $"(('{Constants.Agents.AGENT_FIELD_NAME}' LIKE '{agentName}'))"
				},
				start = 1,
				length = 3
			};
			string request = JsonConvert.SerializeObject(objectManagerQueryRequestModel);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, url, request);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Query for Agent Artifact Ids");
			}
			string result = await response.Content.ReadAsStringAsync();
			JObject jObject = JObject.Parse(result);
			int totalCount = jObject["TotalCount"].Value<int>();
			if (totalCount > 0)
			{
				foreach(var agent in jObject["Objects"])
				{
					agentArtifactIds.Add(agent["ArtifactID"].Value<int>());
				}
			}

			return agentArtifactIds;
		}

		private async Task<List<AgentTypeResponseModel>> GetAgentTypesInInstanceAsync()
		{
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			HttpResponseMessage response = await RestHelper.MakeGetAsync(httpClient, Constants.Connection.RestUrlEndpoints.AgentType.EndpointUrl);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Get Agent Types in Instance");
			}
			string resultString = await response.Content.ReadAsStringAsync();
			dynamic result = JsonConvert.DeserializeObject<dynamic>(resultString);
			List<AgentTypeResponseModel> agentTypeResponseList = new List<AgentTypeResponseModel>();
			foreach (dynamic obj in result)
			{
				agentTypeResponseList.Add(new AgentTypeResponseModel()
				{
					ApplicationName = obj["ApplicationName"].ToString(),
					CompanyName = obj["CompanyName"].ToString(),
					DefaultInterval = Convert.ToDecimal(obj["DefaultInterval"].ToString()),
					DefaultLoggingLevel = Convert.ToInt32(obj["DefaultLoggingLevel"].ToString()),
					ArtifactID = Convert.ToInt32(obj["ArtifactID"].ToString()),
					Name = obj["Name"].ToString()
				});
			}

			return agentTypeResponseList;
		}

		private async Task<List<AgentServerResponseModel>> GetAgentServersForAgentTypeAsync(int agentTypeArtifactId)
		{
			string url =
				Constants.Connection.RestUrlEndpoints.AgentType.GetAgentServersForAgentTypeEndpointUrl.Replace(
					"{agentTypeArtifactId}", agentTypeArtifactId.ToString());
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			HttpResponseMessage response = await RestHelper.MakeGetAsync(httpClient, url);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Get Available Agent Servers for Agent Type");
			}
			string resultString = await response.Content.ReadAsStringAsync();
			dynamic result = JsonConvert.DeserializeObject<dynamic>(resultString);
			List<AgentServerResponseModel> agentServerResponseList = new List<AgentServerResponseModel>();
			foreach (dynamic obj in result)
			{
				agentServerResponseList.Add(new AgentServerResponseModel()
				{
					Type = obj["Type"].ToString(),
					ProcessorCores = Convert.ToInt32(obj["ProcessorCores"].ToString()),
					NumberOfAgents = Convert.ToInt32(obj["NumberOfAgents"].ToString()),
					ArtifactID = Convert.ToInt32(obj["ArtifactID"].ToString()),
					Name = obj["Name"].ToString()
				});
			}

			return agentServerResponseList;
		}

		private async Task CreateAgentAsync(int agentTypeArtifactId, int agentServerArtifactId, decimal defaultInterval, int defaultLoggingLevel)
		{
			var agentCreateRequestModel = new
			{
				AgentRequest = new
				{
					AgentType = new
					{
						Value = new
						{
							ArtifactID = agentTypeArtifactId
						}
					},
					AgentServer = new
					{
						Value = new
						{
							ArtifactID = agentServerArtifactId
						}
					},
					Enabled = Constants.Agents.ENABLE_AGENT,
					Interval = defaultInterval,
					Keywords = Constants.Agents.KEYWORDS,
					Notes = Constants.Agents.NOTES,
					LoggingLevel = defaultLoggingLevel
				}
			};

			string createPayload = JsonConvert.SerializeObject(agentCreateRequestModel);
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.Agent.EndpointUrl, createPayload);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Create Agent");
			}
		}

		private async Task DeleteAgentAsync(int agentArtifactId)
		{
			string url =
				Constants.Connection.RestUrlEndpoints.Agent.DeleteEndpointUrl.Replace("{agentArtifactId}",
					agentArtifactId.ToString());
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			HttpResponseMessage response = await RestHelper.MakeDeleteAsync(httpClient, url);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to Delete Agent : {agentArtifactId}");
			}
		}

		public async Task<int> CreateAgentsInRelativityApplicationAsync(string applicationName)
		{
			try
			{
				int numberOfAgentsCreated = 0;

				//Query all Agent Types in the Instance
				List<AgentTypeResponseModel> agentTypesInInstance = await GetAgentTypesInInstanceAsync();

				//Filter Agent Types from Relativity application
				List<AgentTypeResponseModel> agentTypesInApplication = agentTypesInInstance.Where(x => x.ApplicationName.Equals(applicationName)).ToList();

				//Create Agents if not already exists
				foreach (AgentTypeResponseModel agentTypeResponse in agentTypesInApplication)
				{
					string agentName = agentTypeResponse.Name;
					int agentTypeArtifactId = agentTypeResponse.ArtifactID;
					decimal defaultInterval = agentTypeResponse.DefaultInterval ?? Constants.Agents.AGENT_INTERVAL;
					int defaultLoggingLevel = agentTypeResponse.DefaultLoggingLevel ?? (int)Constants.Agents.AGENT_LOGGING_LEVEL;

					//Check if Agent already exists
					bool doesAgentExists = await CheckIfAtLeastSingleInstanceOfAgentExistsAsync(agentName);

					if (doesAgentExists)
					{
						Console.WriteLine($"Agent already exists. Skipped creation. [{nameof(agentName)}:{agentName}]");
					}
					else
					{
						//Query Agent Server for Agent Type
						List<AgentServerResponseModel> agentServersForAgentType = await GetAgentServersForAgentTypeAsync(agentTypeArtifactId);
						int firstAgentServerArtifactId = agentServersForAgentType.First().ArtifactID;

						//Create Single Agent
						await CreateAgentAsync(agentTypeArtifactId, firstAgentServerArtifactId, defaultInterval, defaultLoggingLevel);
						Console.WriteLine($"Agent Created. [{nameof(agentName)}: {agentName}]");

						numberOfAgentsCreated++;
					}
				}

				return numberOfAgentsCreated;
			}
			catch (Exception ex)
			{
				throw new Exception($"An error occured when creating Agents in Relativity Application. [{nameof(applicationName)}: {applicationName}]", ex);
			}
		}

		public async Task<int> DeleteAgentsInRelativityApplicationAsync(string applicationName)
		{
			try
			{
				int numberOfAgentsDeleted = 0;

				//Query all Agent Types in the Instance
				List<AgentTypeResponseModel> agentTypesInInstance = await GetAgentTypesInInstanceAsync();

				//Filter Agent Types from Relativity application
				List<AgentTypeResponseModel> agentTypesInApplication = agentTypesInInstance.Where(x => x.ApplicationName.Equals(applicationName)).ToList();

				//Create Agents if not already exists
				foreach (AgentTypeResponseModel agentTypeResponse in agentTypesInApplication)
				{
					string agentName = agentTypeResponse.Name;

					//Check if Agent already exists
					bool doesAgentExists = await CheckIfAtLeastSingleInstanceOfAgentExistsAsync(agentName);

					if (doesAgentExists)
					{
						//Query Agent Artifact Ids
						List<int> agentArtifactIds = await GetAgentArtifactIdsAsync(agentName);

						//Delete All Agents
						foreach (int agentArtifactId in agentArtifactIds)
						{
							//Delete Single Agent
							await DeleteAgentAsync(agentArtifactId);
							Console.WriteLine($"Agent Deleted. [{nameof(agentName)}: {agentName}]");

							numberOfAgentsDeleted++;
						}
					}
					else
					{
						Console.WriteLine($"Agent doesn't exists. Skipped deletion. [{nameof(agentName)}:{agentName}]");
					}
				}

				return numberOfAgentsDeleted;
			}
			catch (Exception ex)
			{
				throw new Exception($"An error occured when deleting Agents in Relativity Application. [{nameof(applicationName)}: {applicationName}]", ex);
			}
		}

		public async Task<bool> AddAgentToRelativityByNameAsync(string agentName)
		{
			try
			{
				bool wasAdded = false;

				//Query all Agent Types in the Instance
				List<AgentTypeResponseModel> agentTypesInInstance = await GetAgentTypesInInstanceAsync();

				//Check if Agent already exists
				bool doesAgentExists = await CheckIfAtLeastSingleInstanceOfAgentExistsAsync(agentName);

				if (doesAgentExists)
				{
					Console.WriteLine($"Agent already exists. Skipped creation. [{nameof(agentName)}:{agentName}]");
				}
				else
				{
					AgentTypeResponseModel agentTypeResponse = agentTypesInInstance.Find(x => x.Name.Equals(agentName, StringComparison.OrdinalIgnoreCase));

					int agentTypeArtifactId = agentTypeResponse.ArtifactID;
					decimal defaultInterval = agentTypeResponse.DefaultInterval ?? Constants.Agents.AGENT_INTERVAL;
					int defaultLoggingLevel = agentTypeResponse.DefaultLoggingLevel ?? (int)Constants.Agents.AGENT_LOGGING_LEVEL;

					//Query Agent Server for Agent Type
					List<AgentServerResponseModel> agentServersForAgentType = await GetAgentServersForAgentTypeAsync(agentTypeArtifactId);
					int firstAgentServerArtifactId = agentServersForAgentType.First().ArtifactID;

					//Create Single Agent
					await CreateAgentAsync(agentTypeArtifactId, firstAgentServerArtifactId, defaultInterval, defaultLoggingLevel);
					Console.WriteLine($"Agent Created. [{nameof(agentName)}: {agentName}]");

					wasAdded = true;
				}

				return wasAdded;
			}
			catch (Exception ex)
			{
				throw new Exception($"An error occured when adding agent [{nameof(agentName)}: {agentName}]", ex);
			}
		}

		public async Task<bool> RemoveAgentFromRelativityByNameAsync(string agentName)
		{
			bool wasRemoved = false;

			//Check if Agent already exists
			bool doesAgentExists = await CheckIfAtLeastSingleInstanceOfAgentExistsAsync(agentName);

			if (doesAgentExists)
			{
				//Query Agent Artifact Ids
				List<int> agentArtifactIds = await GetAgentArtifactIdsAsync(agentName);

				//Delete All Agents
				foreach (int agentArtifactId in agentArtifactIds)
				{
					//Delete Single Agent
					await DeleteAgentAsync(agentArtifactId);
					Console.WriteLine($"Agent Deleted. [{nameof(agentName)}: {agentName}]");

					wasRemoved = true;
				}
			}
			else
			{
				Console.WriteLine($"Agent does not exist. Skipped removal. [{nameof(agentName)}:{agentName}]");
			}

			return wasRemoved;
		}
	}
}
