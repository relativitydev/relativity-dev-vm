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
using System.Net.Http;
using System.Threading.Tasks;
using Helpers.RequestModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Implementations
{
	public class AgentHelper : IAgentHelper
	{
		private ServiceFactory ServiceFactory { get; }
		private string InstanceAddress { get; }
		private string AdminUsername { get; }
		private string AdminPassword { get; }

		public AgentHelper(IConnectionHelper connectionHelper, string instanceAddress, string adminUsername, string adminPassword)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
			InstanceAddress = instanceAddress;
			AdminUsername = adminUsername;
			AdminPassword = adminPassword;
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
			HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
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
			HttpResponseMessage response = RestHelper.MakePostAsync(httpClient, url, request).Result;
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

		private async Task<List<AgentTypeResponse>> GetAgentTypesInInstanceAsync()
		{
			using (Relativity.Services.Interfaces.Agent.IAgentManager agentManager = ServiceFactory.CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>())
			{
				List<AgentTypeResponse> agentTypeResponseList = await agentManager.GetAgentTypesAsync(Constants.EDDS_WORKSPACE_ARTIFACT_ID);
				Console.WriteLine($"Total Agent Types in Instance: {agentTypeResponseList.Count}");
				return agentTypeResponseList;
			}
		}

		private async Task<List<AgentServerResponse>> GetAgentServersForAgentTypeAsync(int agentTypeArtifactId)
		{
			using (Relativity.Services.Interfaces.Agent.IAgentManager agentManager = ServiceFactory.CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>())
			{
				List<AgentServerResponse> agentServerResponseList = await agentManager.GetAvailableAgentServersAsync(Constants.EDDS_WORKSPACE_ARTIFACT_ID, agentTypeArtifactId);
				Console.WriteLine($"Total Available Agent Servers for Agent Type: {agentServerResponseList.Count}");
				return agentServerResponseList;
			}
		}

		private async Task CreateAgentAsync(int agentTypeArtifactId, int agentServerArtifactId, decimal defaultInterval, int defaultLoggingLevel)
		{
			AgentRequest newAgentRequest = new AgentRequest
			{
				Enabled = Constants.Agents.ENABLE_AGENT,
				Interval = defaultInterval,
				Keywords = Constants.Agents.KEYWORDS,
				Notes = Constants.Agents.NOTES,
				LoggingLevel = defaultLoggingLevel,
				AgentType = new Securable<ObjectIdentifier>(
					new ObjectIdentifier
					{
						ArtifactID = agentTypeArtifactId
					}),
				AgentServer = new Securable<ObjectIdentifier>(
					new ObjectIdentifier
					{
						ArtifactID = agentServerArtifactId
					})
			};

			using (Relativity.Services.Interfaces.Agent.IAgentManager agentManager = ServiceFactory.CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>())
			{
				int newAgentArtifactId = await agentManager.CreateAsync(Constants.EDDS_WORKSPACE_ARTIFACT_ID, newAgentRequest);
				Console.WriteLine($"{nameof(newAgentArtifactId)}: {newAgentArtifactId}");
			}
		}

		private async Task DeleteAgentAsync(int agentArtifactId)
		{
			using (Relativity.Services.Interfaces.Agent.IAgentManager agentManager = ServiceFactory.CreateProxy<Relativity.Services.Interfaces.Agent.IAgentManager>())
			{
				try
				{
					await agentManager.DeleteAsync(Constants.EDDS_WORKSPACE_ARTIFACT_ID, agentArtifactId);
				}
				catch (Exception ex)
				{
					throw new Exception($"An error occured when deleting Agent. [{nameof(agentArtifactId)}: {agentArtifactId}]", ex);
				}
			}
		}

		public async Task<int> CreateAgentsInRelativityApplicationAsync(string applicationName)
		{
			try
			{
				int numberOfAgentsCreated = 0;

				//Query all Agent Types in the Instance
				List<AgentTypeResponse> agentTypesInInstance = await GetAgentTypesInInstanceAsync();

				//Filter Agent Types from Relativity application
				List<AgentTypeResponse> agentTypesInApplication = agentTypesInInstance.Where(x => x.ApplicationName.Equals(applicationName)).ToList();

				//Create Agents if not already exists
				foreach (AgentTypeResponse agentTypeResponse in agentTypesInApplication)
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
						List<AgentServerResponse> agentServersForAgentType = await GetAgentServersForAgentTypeAsync(agentTypeArtifactId);
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
				List<AgentTypeResponse> agentTypesInInstance = await GetAgentTypesInInstanceAsync();

				//Filter Agent Types from Relativity application
				List<AgentTypeResponse> agentTypesInApplication = agentTypesInInstance.Where(x => x.ApplicationName.Equals(applicationName)).ToList();

				//Create Agents if not already exists
				foreach (AgentTypeResponse agentTypeResponse in agentTypesInApplication)
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
				List<AgentTypeResponse> agentTypesInInstance = await GetAgentTypesInInstanceAsync();

				//Check if Agent already exists
				bool doesAgentExists = await CheckIfAtLeastSingleInstanceOfAgentExistsAsync(agentName);

				if (doesAgentExists)
				{
					Console.WriteLine($"Agent already exists. Skipped creation. [{nameof(agentName)}:{agentName}]");
				}
				else
				{
					AgentTypeResponse agentTypeResponse = agentTypesInInstance.Find(x => x.Name.Equals(agentName, StringComparison.OrdinalIgnoreCase));

					int agentTypeArtifactId = agentTypeResponse.ArtifactID;
					decimal defaultInterval = agentTypeResponse.DefaultInterval ?? Constants.Agents.AGENT_INTERVAL;
					int defaultLoggingLevel = agentTypeResponse.DefaultLoggingLevel ?? (int)Constants.Agents.AGENT_LOGGING_LEVEL;

					//Query Agent Server for Agent Type
					List<AgentServerResponse> agentServersForAgentType = await GetAgentServersForAgentTypeAsync(agentTypeArtifactId);
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
