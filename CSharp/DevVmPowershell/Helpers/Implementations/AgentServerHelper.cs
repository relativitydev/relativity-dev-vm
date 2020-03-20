using Helpers.Interfaces;
using Relativity.Services;
using Relativity.Services.ResourcePool;
using Relativity.Services.ResourceServer;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class AgentServerHelper : IAgentServerHelper
	{
		private ServiceFactory ServiceFactory { get; }

		public AgentServerHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		/// <summary>
		/// Add Agent Server to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddAgentServerToDefaultResourcePoolAsync()
		{
			bool wasAgentServerAddedToDefaultPool = false;

			Console.WriteLine(
				$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Adding Agent Server ({Constants.AgentServer.AgentServerName}) to the Default Resource Pool");

			// Setup for checking Resource Pools
			TextCondition conditionPool = new TextCondition()
			{
				Field = Constants.AgentServer.NameField,
				Operator = TextConditionEnum.StartsWith,
				Value = Constants.AgentServer.DefaultPool
			};

			Query queryPool = new Query()
			{
				Condition = conditionPool.ToQueryString(),
			};

			// Setup for checking if Agent Server exists
			TextCondition conditionAgent = new TextCondition()
			{
				Field = Constants.AgentServer.NameField,
				Operator = TextConditionEnum.EqualTo,
				Value = Constants.AgentServer.ResourceServerName
			};

			Query queryAgent = new Query()
			{
				Condition = conditionAgent.ToQueryString()
			};

			try
			{
				using (IResourceServerManager resourceServerManager = ServiceFactory.CreateProxy<IResourceServerManager>())
				using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
				{
					// Check for Default Resource Pool
					ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(queryPool);

					Console.WriteLine($"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Checking if Default Resource Pool exists");

					if (resultPools.Success && resultPools.TotalCount > 0)
					{
						ResourcePoolRef defaultPoolRef = new ResourcePoolRef(resultPools.Results.Find(x =>
								x.Artifact.Name.Equals(Constants.AgentServer.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact
							.ArtifactID);

						List<ResourceServerRef> resultServers =
							await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);


						// Check to make sure the Agent Server was not already added
						if (!resultServers.Exists(x =>
							x.ServerType.Name.Equals(Constants.AgentServer.AgentServerName, StringComparison.OrdinalIgnoreCase)))
						{
							// Make sure the Agent Server actually exists and then add it
							ResourceServerQueryResultSet queryResult = await resourceServerManager.QueryAsync(queryAgent);

							if (queryResult.Success && queryResult.TotalCount > 0)
							{
								ResourceServer agentServer = queryResult.Results.Find(x =>
									x.Artifact.ServerType.Name.Equals(Constants.AgentServer.AgentServerName,
										StringComparison.OrdinalIgnoreCase)).Artifact;

								ResourceServerRef agentServerRef = new ResourceServerRef()
								{
									ArtifactID = agentServer.ArtifactID,
									Name = agentServer.Name,
									ServerType = new ResourceServerTypeRef()
									{
										ArtifactID = agentServer.ServerType.ArtifactID,
										Name = agentServer.ServerType.Name
									}
								};

								await resourcePoolManager.AddServerAsync(agentServerRef, defaultPoolRef);

								resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

								if (resultServers.Exists(x =>
									x.ServerType.Name.Equals(Constants.AgentServer.AgentServerName, StringComparison.OrdinalIgnoreCase)))
								{
									wasAgentServerAddedToDefaultPool = true;
									Console.WriteLine($"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Successfully added Agent Server to Default Resource Pool");
								}
								else
								{
									wasAgentServerAddedToDefaultPool = false;
									Console.WriteLine($"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to add Agent Server to Default Resource Pool.");
								}
							}
							else
							{
								Console.WriteLine($"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to add Agent Server to Default Resource Pool as the Agent Server does not exist");
							}
						}
						else
						{
							wasAgentServerAddedToDefaultPool = true;
							Console.WriteLine($"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to add Agent Server to Default Resource Pool as it already exists within the pool");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to add Agent Server to Default Resource Pool: {ex.Message}");
				throw;
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

			Console.WriteLine(
				$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Removing Agent Server ({Constants.AgentServer.AgentServerName}) from the Default Resource Pool");

			// Setup for checking Resource Pools
			TextCondition conditionPool = new TextCondition()
			{
				Field = Constants.AgentServer.NameField,
				Operator = TextConditionEnum.StartsWith,
				Value = Constants.AgentServer.DefaultPool
			};

			Query queryPool = new Query()
			{
				Condition = conditionPool.ToQueryString(),
			};

			// Setup for checking if Agent Server exists
			TextCondition conditionAgent = new TextCondition()
			{
				Field = Constants.AgentServer.NameField,
				Operator = TextConditionEnum.EqualTo,
				Value = Constants.AgentServer.ResourceServerName
			};

			Query queryAgent = new Query()
			{
				Condition = conditionAgent.ToQueryString()
			};


			try
			{
				using (IResourceServerManager resourceServerManager = ServiceFactory.CreateProxy<IResourceServerManager>())
				using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
				{
					// Check for Default Resource Pool
					ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(queryPool);

					Console.WriteLine(
						$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Checking if Default Resource Pool exists");
					if (resultPools.Success && resultPools.TotalCount > 0)
					{
						ResourcePoolRef defaultPoolRef = new ResourcePoolRef(resultPools.Results.Find(x =>
								x.Artifact.Name.Equals(Constants.AgentServer.DefaultPool, StringComparison.OrdinalIgnoreCase))
							.Artifact
							.ArtifactID);

						List<ResourceServerRef> resultServers =
							await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

						// Check to make sure the Agent Server was added so we can remove it
						if (resultServers.Exists(x =>
							x.ServerType.Name.Equals(Constants.AgentServer.AgentServerName, StringComparison.OrdinalIgnoreCase)))
						{
							// Make sure the Agent Server actually exists and then remove it
							ResourceServerQueryResultSet queryResult = await resourceServerManager.QueryAsync(queryAgent);
							if (queryResult.Success && queryResult.TotalCount > 0)
							{
								ResourceServer agentServer = queryResult.Results.Find(x =>
									x.Artifact.ServerType.Name.Equals(Constants.AgentServer.AgentServerName,
										StringComparison.OrdinalIgnoreCase)).Artifact;

								ResourceServerRef agentServerRef = new ResourceServerRef()
								{
									ArtifactID = agentServer.ArtifactID,
									Name = agentServer.Name,
									ServerType = new ResourceServerTypeRef()
									{
										ArtifactID = agentServer.ServerType.ArtifactID,
										Name = agentServer.ServerType.Name
									}
								};

								await resourcePoolManager.RemoveServerAsync(agentServerRef, defaultPoolRef);

								resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

								if (!resultServers.Exists(x =>
									x.ServerType.Name.Equals(Constants.AgentServer.AgentServerName,
										StringComparison.OrdinalIgnoreCase)))
								{
									wasAgentServerRemovedFromDefaultPool = true;
									Console.WriteLine(
										$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Successfully removed Agent Server from Default Resource Pool");
								}
								else
								{
									wasAgentServerRemovedFromDefaultPool = false;
									Console.WriteLine(
										$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to remove Agent Server from Default Resource Pool.");
								}
							}
							else
							{
								Console.WriteLine(
									$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to remove Agent Server from Default Resource Pool as the Agent Server does not exist");
							}
						}
						else
						{
							wasAgentServerRemovedFromDefaultPool = true;
							Console.WriteLine(
								$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to remove Agent Server from Default Resource Pool as it doesn't exist within the pool");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"{nameof(AddAgentServerToDefaultResourcePoolAsync)} - Failed to remove Agent Server from Default Resource Pool: {ex.Message}");
				throw;
			}


			return wasAgentServerRemovedFromDefaultPool;
		}
	}
}