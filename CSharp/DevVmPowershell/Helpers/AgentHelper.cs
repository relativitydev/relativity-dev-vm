using Relativity.Services.Interfaces.Agent.Models;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers
{
	public class AgentHelper : IAgentHelper
	{
		private ServiceFactory ServiceFactory { get; }

		public AgentHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
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

			QueryRequest agentQueryRequest = new QueryRequest
			{
				ObjectType = new ObjectTypeRef
				{
					Name = Constants.Agents.AGENT_OBJECT_TYPE
				},
				Fields = new List<FieldRef>
				{
					new FieldRef
					{
						Name = Constants.Agents.AGENT_FIELD_NAME
					}
				},
				Condition = $"(('{Constants.Agents.AGENT_FIELD_NAME}' LIKE '{agentName}'))"
			};
			using (IObjectManager objectManager = ServiceFactory.CreateProxy<IObjectManager>())
			{
				QueryResult agentQueryResult = await objectManager.QueryAsync(
					Constants.EDDS_WORKSPACE_ARTIFACT_ID,
					agentQueryRequest,
					1,
					3);

				if (agentQueryResult.Objects.Count > 0)
				{
					agentArtifactIds.AddRange(agentQueryResult.Objects.Select(x => x.ArtifactID).ToList());
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
	}
}
