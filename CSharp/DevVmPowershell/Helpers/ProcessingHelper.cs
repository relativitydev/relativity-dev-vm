using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ResourcePool;
using Relativity.Services.ResourceServer;
using Relativity.Services.ServiceProxy;
using Relativity.Services.WorkerStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Choice = kCura.Relativity.Client.DTOs.Choice;
using ChoiceRef = Relativity.Services.Choice.ChoiceRef;

namespace Helpers
{
	public class ProcessingHelper : IProcessingHelper
	{
		private ServiceFactory ServiceFactory { get; }
		public ProcessingHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		/// <summary>
		/// Runs through every step needed to create Objects and setup to the Default Resource Pool.  Objects created and added to Default Pool are a Processing Source Location, a Worker Server Manager, and a Worker Server
		/// </summary>
		/// <returns></returns>
		public async Task<bool> FullSetupAndUpdateDefaultResourcePoolAsync()
		{
			bool wasSetupComplete = false;

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Starting ({nameof(CreateProcessingSourceLocationChoice)})");
			wasSetupComplete = CreateProcessingSourceLocationChoice();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished ({nameof(CreateProcessingSourceLocationChoice)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Starting ({nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)})");
			wasSetupComplete = await AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished ({nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Starting ({nameof(CreateWorkerManagerServerAsync)})");
			wasSetupComplete = await CreateWorkerManagerServerAsync();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished ({nameof(CreateWorkerManagerServerAsync)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Sleeping for 30 seconds auto creation of Worker Server");
			Thread.Sleep(30000);
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished sleeping for auto creation of Worker Server");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Starting ({nameof(UpdateWorkerServerForProcessingAsync)})");
			wasSetupComplete = await UpdateWorkerServerForProcessingAsync();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished ({nameof(UpdateWorkerServerForProcessingAsync)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Starting ({nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)})");
			wasSetupComplete = await AddWorkerManagerServerToDefaultResourcePoolAsync();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished ({nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Starting ({nameof(AddWorkerServerToDefaultResourcePoolAsync)})");
			wasSetupComplete = await AddWorkerServerToDefaultResourcePoolAsync();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished ({nameof(AddWorkerServerToDefaultResourcePoolAsync)}) Result: {wasSetupComplete}");


			return wasSetupComplete;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<bool> FullResetAsync()
		{
			bool wasReset = false;

			Console.WriteLine($"{nameof(FullResetAsync)} - Starting ({nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)})");
			wasReset = await RemoveWorkerServerFromDefaultResourcePoolAsync();
			Console.WriteLine($"{nameof(FullResetAsync)} - Finished ({nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)}) Result: {wasReset}");

			Console.WriteLine($"{nameof(FullResetAsync)} - Starting ({nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)})");
			wasReset = await RemoveWorkerServerFromDefaultResourcePoolAsync();
			Console.WriteLine($"{nameof(FullResetAsync)} - Finished ({nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)}) Result: {wasReset}");

			Console.WriteLine($"{nameof(FullResetAsync)} - Starting ({nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)})");
			wasReset = await RemoveWorkerManagerServerFromDefaultResourcePoolAsync();
			Console.WriteLine($"{nameof(FullResetAsync)} - Finished ({nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)}) Result: {wasReset}");

			Console.WriteLine($"{nameof(FullResetAsync)} - Starting ({nameof(DeleteWorkerManagerServerAsync)})");
			wasReset = await DeleteWorkerManagerServerAsync();
			Console.WriteLine($"{nameof(FullResetAsync)} - Finished ({nameof(DeleteWorkerManagerServerAsync)}) Result: {wasReset}");


			return wasReset;
		}

		/// <summary>
		/// Creates a Processing Source Location Choice and makes sure it doesn't already exist.
		/// </summary>
		/// <returns></returns>
		public bool CreateProcessingSourceLocationChoice()
		{
			bool wasChoiceCreated = false;

			Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Creating Processing Source Location Choice ({Constants.Processing.ChoiceName})");

			Choice choice = new Choice
			{
				Name = Constants.Processing.ChoiceName,
				ChoiceTypeID = Constants.Processing.ChoiceTypeID,
				Order = 1
			};

			kCura.Relativity.Client.TextCondition cond = new kCura.Relativity.Client.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = kCura.Relativity.Client.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ChoiceName
			};

			using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
			{
				rsapiClient.APIOptions = new APIOptions(-1);

				kCura.Relativity.Client.DTOs.QueryResultSet<Choice> choiceQuery = rsapiClient.Repositories.Choice.Query(new Query<Choice>() { Condition = cond });

				if (choiceQuery.Success && choiceQuery.TotalCount > 0)
				{
					Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Failed to create Processing Source Location Choice ({Constants.Processing.ChoiceName}) as it already exists");
					wasChoiceCreated = true;
					return wasChoiceCreated;
				}

				WriteResultSet<Choice> result = rsapiClient.Repositories.Choice.Create(choice);

				if (result.Success)
				{
					choiceQuery = rsapiClient.Repositories.Choice.Query(new Query<Choice>() { Condition = cond });
					if (choiceQuery.Success && choiceQuery.TotalCount > 0)
					{
						wasChoiceCreated = true;
						Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Successfully created Processing Source Location Choice ({Constants.Processing.ChoiceName})");
					}
					else
					{
						Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Failed to create Processing Source Location Choice ({Constants.Processing.ChoiceName})");
					}

				}
				else
				{
					Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Failed to create Processing Source Location Choice ({Constants.Processing.ChoiceName})");
				}
			}

			return wasChoiceCreated;
		}

		/// <summary>
		/// Add Processing Source Location choice to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync()
		{
			bool wasChoiceAddedToPool = false;

			Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Adding Processing Source Location Choice to Default Resource Pool");

			// Setup for checking Resource Pools
			Relativity.Services.TextCondition conditionPool = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.StartsWith,
				Value = Constants.Processing.DefaultPool
			};

			Relativity.Services.Query queryPool = new Relativity.Services.Query()
			{
				Condition = conditionPool.ToQueryString(),
			};

			// Setup for checking choice
			Relativity.Services.TextCondition conditionChoice = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ChoiceName
			};

			Relativity.Services.Objects.DataContracts.QueryRequest queryChoice = new Relativity.Services.Objects.DataContracts.QueryRequest()
			{
				Condition = conditionChoice.ToQueryString().Replace(@"\", @"\\"),
				ObjectType = new ObjectTypeRef() { ArtifactTypeID = Constants.Processing.ChoiceArtifactTypeId },
				Fields = new List<FieldRef>
				{
					new FieldRef
					{
						Name = Constants.Processing.ChoiceFieldRef
					}
				}
			};

			using (IObjectManager objectManager = ServiceFactory.CreateProxy<IObjectManager>())
			using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
			{
				// Check for Default Resource Pool
				ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(queryPool);

				Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Checking if Default Resource Pool exists");
				if (resultPools.Success && resultPools.TotalCount > 0)
				{
					ResourcePoolRef defaultResourcePoolRef = new ResourcePoolRef(resultPools.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID);

					// Check if Processing Server Location already added to Default Resource Pool
					Relativity.Services.Objects.DataContracts.QueryResult resultChoice = await objectManager.QueryAsync(Constants.Processing.WorkspaceId, queryChoice, 1, 10);

					Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Checking if Processing Source Location Choice exists");
					if (resultChoice.TotalCount > 0)
					{
						ChoiceRef choice = new ChoiceRef(resultChoice.Objects.First().ArtifactID);

						List<ChoiceRef> resultProcessingSourceLocations = await resourcePoolManager.GetProcessingSourceLocationsAsync(defaultResourcePoolRef);

						// Check if Processing Source Location was not already added
						if (!resultProcessingSourceLocations.Exists(x => x.ArtifactID == choice.ArtifactID))
						{
							await resourcePoolManager.AddProcessingSourceLocationAsync(choice, defaultResourcePoolRef);

							resultProcessingSourceLocations = await resourcePoolManager.GetProcessingSourceLocationsAsync(defaultResourcePoolRef);

							// Confirm if it was added
							if (resultProcessingSourceLocations.Exists(x => x.ArtifactID == choice.ArtifactID))
							{
								wasChoiceAddedToPool = true;
								Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Added Processing Source Location Choice to Default Resource Pool");
							}
						}
						else
						{
							Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Failed to add Processing Source Location Choice to Default Resource Pool as it already exists within the pool");
							wasChoiceAddedToPool = true;
							return wasChoiceAddedToPool;
						}
					}
					else
					{
						Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Processing Source Location Choice does not exist");
						wasChoiceAddedToPool = true;
					}

				}
				else
				{
					Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Default Resource Pool does not exist");
				}
			}

			return wasChoiceAddedToPool;
		}

		/// <summary>
		/// Create Worker Manager Server if it doesn't already exist, which also automatically creates a Worker Server
		/// </summary>
		/// <returns></returns>
		public async Task<bool> CreateWorkerManagerServerAsync()
		{
			bool wasWorkerManagerServerCreated = false;

			Console.WriteLine($"{nameof(CreateWorkerManagerServerAsync)} - Creating Worker Manager Server ({Constants.Processing.ResourceServerName})");

			// Setup for checking if Worker Manager Server exists
			Relativity.Services.TextCondition conditionServer = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryServer = new Relativity.Services.Query()
			{
				Condition = conditionServer.ToQueryString()
			};

			// The Worker Manager Server to be created
			WorkerManagerServer server = new WorkerManagerServer()
			{
				Name = Constants.Processing.ResourceServerName,
				IsDefault = true,
				InventoryPriority = 100,
				DiscoveryPriority = 100,
				PublishPriority = 100,
				ImageOnTheFlyPriority = 1,
				MassImagingPriority = 100,
				SingleSaveAsPDFPriority = 100,
				MassSaveAsPDFPriority = 100,
				URL = Constants.Processing.ResourceServerUrl
			};

			using (IWorkerServerManager workerServerManager = ServiceFactory.CreateProxy<IWorkerServerManager>())
			{
				WorkerServerQueryResultSet queryResult = await workerServerManager.QueryAsync(queryServer);

				if (queryResult.Success && queryResult.TotalCount == 0)
				{
					int workerServerArtifactId = await workerServerManager.CreateSingleAsync(server);
					Console.WriteLine($"{nameof(CreateWorkerManagerServerAsync)} - Worker Server Artifact ID: {workerServerArtifactId}");

					queryResult = await workerServerManager.QueryAsync(queryServer);

					if (queryResult.Success && queryResult.TotalCount > 0)
					{
						wasWorkerManagerServerCreated = true;
						Console.WriteLine($"{nameof(CreateWorkerManagerServerAsync)} - Successfully created Worker Manager Server ({Constants.Processing.ResourceServerName})");
					}
				}
				else if (queryResult.Success && queryResult.TotalCount > 0)
				{
					Console.WriteLine($"{nameof(CreateWorkerManagerServerAsync)} - Failed to create Worker Manager Server ({Constants.Processing.ResourceServerName}) as it already exists");
					wasWorkerManagerServerCreated = true;
				}
				else
				{
					Console.WriteLine($"{nameof(CreateWorkerManagerServerAsync)} - Failed to create and check for Worker Manager Server ({Constants.Processing.ResourceServerName})");
				}
			}
			return wasWorkerManagerServerCreated;
		}

		/// <summary>
		/// Delete Worker Manager Server from the Instance
		/// </summary>
		/// <returns></returns>
		public async Task<bool> DeleteWorkerManagerServerAsync()
		{
			bool wasDeleted = false;

			Console.WriteLine($"{nameof(DeleteWorkerManagerServerAsync)} - Creating Worker Manager Server ({Constants.Processing.ResourceServerName})");

			// Setup for checking if Worker Manager Server exists
			Relativity.Services.TextCondition conditionServer = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryServer = new Relativity.Services.Query()
			{
				Condition = conditionServer.ToQueryString()
			};

			using (IWorkerServerManager workerServerManager = ServiceFactory.CreateProxy<IWorkerServerManager>())
			{
				WorkerServerQueryResultSet queryResult = await workerServerManager.QueryAsync(queryServer);

				if (queryResult.Success && queryResult.Results.Exists(x => x.Artifact.ServerType.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)))
				{
					await workerServerManager.DeleteSingleAsync(queryResult.Results.Find(x => x.Artifact.ServerType.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID);

					queryResult = await workerServerManager.QueryAsync(queryServer);

					if (queryResult.Success && !queryResult.Results.Exists(x => x.Artifact.ServerType.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)))
					{
						wasDeleted = true;
						Console.WriteLine($"{nameof(DeleteWorkerManagerServerAsync)} - Successfully deleted Worker Manager Server ({Constants.Processing.ResourceServerName})");
					}
				}
				else if (queryResult.Success && queryResult.TotalCount == 0)
				{
					Console.WriteLine($"{nameof(DeleteWorkerManagerServerAsync)} - Failed to delete Worker Manager Server ({Constants.Processing.ResourceServerName}) as it doesn't exist");
					wasDeleted = true;
				}
				else
				{
					Console.WriteLine($"{nameof(DeleteWorkerManagerServerAsync)} - Failed to delete and check for Worker Manager Server ({Constants.Processing.ResourceServerName})");
				}
			}
			return wasDeleted;
		}

		public async Task<bool> UpdateWorkerServerForProcessingAsync()
		{
			bool wasUpdated = false;

			Console.WriteLine($"{nameof(UpdateWorkerServerForProcessingAsync)} - Updating Worker Server ({Constants.Processing.WorkerServer}) for to be ready for Processing");

			// Setup for checking if Worker Server exists
			Relativity.Services.TextCondition conditionWorker = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryWorker = new Relativity.Services.Query()
			{
				Condition = conditionWorker.ToQueryString()
			};

			using (IWorkerStatusService workerStatusService = ServiceFactory.CreateProxy<IWorkerStatusService>())
			using (IResourceServerManager resourceServerManager = ServiceFactory.CreateProxy<IResourceServerManager>())
			{
				// Make sure the Worker Server actually exists and then add it
				ResourceServerQueryResultSet queryResult = await resourceServerManager.QueryAsync(queryWorker);

				if (queryResult.Success && queryResult.Results.Exists(x => x.Artifact.ServerType.Name.Equals(Constants.Processing.WorkerServer, StringComparison.OrdinalIgnoreCase)))
				{
					WorkerCategory[] categories = new WorkerCategory[]
					{
						WorkerCategory.NativeImaging,
						WorkerCategory.BasicImaging,
						WorkerCategory.SaveAsPDF,
						WorkerCategory.Processing
					};
					int workerServerArtifactId = queryResult.Results
						.Find(x => x.Artifact.ServerType.Name.Equals(Constants.Processing.WorkerServer,
							StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID;

					await workerStatusService.UpdateCategoriesOnWorkerAsync(workerServerArtifactId, categories);
					wasUpdated = true;
					Console.WriteLine($"{nameof(UpdateWorkerServerForProcessingAsync)} - Successfully updated Worker Server ({Constants.Processing.ResourceServerName})");
				}
				else
				{
					Console.WriteLine($"{nameof(UpdateWorkerServerForProcessingAsync)} - Failed to update Worker Server ({Constants.Processing.ResourceServerName}) as it doesn't exist");
				}
			}

			return wasUpdated;
		}

		/// <summary>
		/// Add Worker Manager Server to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddWorkerManagerServerToDefaultResourcePoolAsync()
		{
			bool wasWorkerManagerServerAddedToDefaultPool = false;

			Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Adding Worker Manager Server ({Constants.Processing.ResourceServerName}) to the Default Resource Pool");

			// Setup for checking Resource Pools
			Relativity.Services.TextCondition conditionPool = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.StartsWith,
				Value = Constants.Processing.DefaultPool
			};

			Relativity.Services.Query queryPool = new Relativity.Services.Query()
			{
				Condition = conditionPool.ToQueryString(),
			};

			// Setup for checking if Worker Manager Server exists
			Relativity.Services.TextCondition conditionServer = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryServer = new Relativity.Services.Query()
			{
				Condition = conditionServer.ToQueryString()
			};

			using (IWorkerServerManager workerServerManager = ServiceFactory.CreateProxy<IWorkerServerManager>())
			using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
			{
				// Check for Default Resource Pool
				ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(queryPool);

				Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Checking if Default Resource Pool exists");
				if (resultPools.Success && resultPools.TotalCount > 0)
				{
					ResourcePoolRef defaultPoolRef = new ResourcePoolRef(resultPools.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID);

					List<ResourceServerRef> resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

					// Check to make sure the Worker Manager Server was not already added
					if (!resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)))
					{
						// Make sure the Worker Manager Server actually exists and then add it
						WorkerServerQueryResultSet queryResult = await workerServerManager.QueryAsync(queryServer);
						if (queryResult.Success && queryResult.TotalCount > 0)
						{
							WorkerManagerServer workerManagerServer = queryResult.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.ResourceServerName,
								StringComparison.OrdinalIgnoreCase) && x.Artifact.URL.Equals(Constants.Processing.ResourceServerUrl,
									StringComparison.OrdinalIgnoreCase)).Artifact;

							ResourceServerRef workerServerRef = new ResourceServerRef()
							{
								ArtifactID = workerManagerServer.ArtifactID,
								Name = workerManagerServer.Name,
								ServerType = new ResourceServerTypeRef()
								{
									ArtifactID = workerManagerServer.ServerType.ArtifactID,
									Name = workerManagerServer.ServerType.Name
								}
							};

							await resourcePoolManager.AddServerAsync(workerServerRef, defaultPoolRef);

							resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

							if (resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)))
							{
								wasWorkerManagerServerAddedToDefaultPool = true;
								Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Successfully added Worker Manager Server to Default Resource Pool");
							}
							else
							{
								wasWorkerManagerServerAddedToDefaultPool = false;
								Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Failed to add Worker Manager Server to Default Resource Pool.");
							}
						}
						else
						{
							Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Failed to add Worker Manager Server to Default Resource Pool as the Worker Manager Server does not exist");
						}
					}
					else
					{
						wasWorkerManagerServerAddedToDefaultPool = true;
						Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Failed to add Worker Manager Server to Default Resource Pool as it already exists within the pool");
					}

				}
			}

			return wasWorkerManagerServerAddedToDefaultPool;
		}


		/// <summary>
		/// Add Worker Server to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddWorkerServerToDefaultResourcePoolAsync()
		{
			bool wasWorkerServerAddedToDefaultPool = false;

			Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Adding Worker Server ({Constants.Processing.WorkerServer}) to the Default Resource Pool");

			// Setup for checking Resource Pools
			Relativity.Services.TextCondition conditionPool = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.StartsWith,
				Value = Constants.Processing.DefaultPool
			};

			Relativity.Services.Query queryPool = new Relativity.Services.Query()
			{
				Condition = conditionPool.ToQueryString(),
			};

			// Setup for checking if Worker Manager Server exists
			Relativity.Services.TextCondition conditionServer = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryServer = new Relativity.Services.Query()
			{
				Condition = conditionServer.ToQueryString()
			};

			// Setup for checking if Worker Server exists
			Relativity.Services.TextCondition conditionWorker = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryWorker = new Relativity.Services.Query()
			{
				Condition = conditionWorker.ToQueryString()
			};

			using (IResourceServerManager resourceServerManager = ServiceFactory.CreateProxy<IResourceServerManager>())
			using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
			{
				// Check for Default Resource Pool
				ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(queryPool);

				Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Checking if Default Resource Pool exists");
				if (resultPools.Success && resultPools.TotalCount > 0)
				{
					ResourcePoolRef defaultPoolRef = new ResourcePoolRef(resultPools.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID);

					List<ResourceServerRef> resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

					// Check to make sure the Worker Server was not already added
					if (!resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerServer, StringComparison.OrdinalIgnoreCase)))
					{
						// Make sure the Worker Server actually exists and then add it
						ResourceServerQueryResultSet queryResult = await resourceServerManager.QueryAsync(queryWorker);

						if (queryResult.Success && queryResult.TotalCount > 0)
						{
							ResourceServer workerServer = queryResult.Results.Find(x => x.Artifact.ServerType.Name.Equals(Constants.Processing.WorkerServer, StringComparison.OrdinalIgnoreCase)).Artifact;

							ResourceServerRef workerServerRef = new ResourceServerRef()
							{
								ArtifactID = workerServer.ArtifactID,
								Name = workerServer.Name,
								ServerType = new ResourceServerTypeRef()
								{
									ArtifactID = workerServer.ServerType.ArtifactID,
									Name = workerServer.ServerType.Name
								}
							};

							await resourcePoolManager.AddServerAsync(workerServerRef, defaultPoolRef);

							resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

							if (resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerServer, StringComparison.OrdinalIgnoreCase)))
							{
								wasWorkerServerAddedToDefaultPool = true;
								Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Successfully added Worker Server to Default Resource Pool");
							}
							else
							{
								wasWorkerServerAddedToDefaultPool = false;
								Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Failed to add Worker Server to Default Resource Pool.");
							}

						}
						else
						{
							Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Failed to add Worker Server to Default Resource Pool as the Worker Server does not exist");
						}
					}
					else
					{
						wasWorkerServerAddedToDefaultPool = true;
						Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Failed to add Worker Server to Default Resource Pool as it already exists within the pool");
					}

				}
			}

			return wasWorkerServerAddedToDefaultPool;
		}

		/// <summary>
		/// Removing Worker Manager Server from Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> RemoveWorkerManagerServerFromDefaultResourcePoolAsync()
		{
			bool wasRemoved = false;

			Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Removing Worker Manager Server ({Constants.Processing.ResourceServerName}) remove the Default Resource Pool");

			// Setup for checking Resource Pools
			Relativity.Services.TextCondition conditionPool = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.StartsWith,
				Value = Constants.Processing.DefaultPool
			};

			Relativity.Services.Query queryPool = new Relativity.Services.Query()
			{
				Condition = conditionPool.ToQueryString(),
			};

			// Setup for checking if Worker Manager Server exists
			Relativity.Services.TextCondition conditionServer = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryServer = new Relativity.Services.Query()
			{
				Condition = conditionServer.ToQueryString()
			};

			using (IWorkerServerManager workerServerManager = ServiceFactory.CreateProxy<IWorkerServerManager>())
			using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
			{
				// Check for Default Resource Pool
				ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(queryPool);

				Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Checking if Default Resource Pool exists");
				if (resultPools.Success && resultPools.TotalCount > 0)
				{
					ResourcePoolRef defaultPoolRef = new ResourcePoolRef(resultPools.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID);

					List<ResourceServerRef> resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

					// Check to make sure the Worker Manager Server is there
					if (resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)))
					{
						// Make sure the Worker Manager Server actually exists and then remove it
						WorkerServerQueryResultSet queryResult = await workerServerManager.QueryAsync(queryServer);
						if (queryResult.Success && queryResult.TotalCount > 0)
						{
							WorkerManagerServer workerManagerServer = queryResult.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.ResourceServerName,
								StringComparison.OrdinalIgnoreCase) && x.Artifact.URL.Equals(Constants.Processing.ResourceServerUrl,
									StringComparison.OrdinalIgnoreCase)).Artifact;

							ResourceServerRef workerServerRef = new ResourceServerRef()
							{
								ArtifactID = workerManagerServer.ArtifactID,
								Name = workerManagerServer.Name,
								ServerType = new ResourceServerTypeRef()
								{
									ArtifactID = workerManagerServer.ServerType.ArtifactID,
									Name = workerManagerServer.ServerType.Name
								}
							};

							await resourcePoolManager.RemoveServerAsync(workerServerRef, defaultPoolRef);

							resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

							if (!resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)))
							{
								wasRemoved = true;
								Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Successfully removed Worker Manager Server from Default Resource Pool");
							}
							else
							{
								wasRemoved = false;
								Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to removed Worker Manager Server from Default Resource Pool.");
							}
						}
						else
						{
							wasRemoved = true;
							Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to removed Worker Manager Server from Default Resource Pool as the Worker Manager Server does not exist");
						}
					}
					else
					{
						wasRemoved = true;
						Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to removed Worker Manager Server from Default Resource Pool as it is not within the pool");
					}

				}
			}

			return wasRemoved;
		}

		/// <summary>
		/// Removing Worker Server from Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> RemoveWorkerServerFromDefaultResourcePoolAsync()
		{
			bool wasRemoved = false;

			Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)} - Deleting Worker Server ({Constants.Processing.ResourceServerName})");

			// Setup for checking Resource Pools
			Relativity.Services.TextCondition conditionPool = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.StartsWith,
				Value = Constants.Processing.DefaultPool
			};

			Relativity.Services.Query queryPool = new Relativity.Services.Query()
			{
				Condition = conditionPool.ToQueryString(),
			};

			// Setup for checking if Worker Manager Server exists
			Relativity.Services.TextCondition conditionServer = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryServer = new Relativity.Services.Query()
			{
				Condition = conditionServer.ToQueryString()
			};

			// Setup for checking if Worker Server exists
			Relativity.Services.TextCondition conditionWorker = new Relativity.Services.TextCondition()
			{
				Field = Constants.Processing.NameField,
				Operator = Relativity.Services.TextConditionEnum.EqualTo,
				Value = Constants.Processing.ResourceServerName
			};

			Relativity.Services.Query queryWorker = new Relativity.Services.Query()
			{
				Condition = conditionWorker.ToQueryString()
			};


			using (IResourceServerManager resourceServerManager = ServiceFactory.CreateProxy<IResourceServerManager>())
			using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
			{
				// Check for Default Resource Pool
				ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(queryPool);

				Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)} - Checking if Default Resource Pool exists");
				if (resultPools.Success && resultPools.TotalCount > 0)
				{
					ResourcePoolRef defaultPoolRef = new ResourcePoolRef(resultPools.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID);

					List<ResourceServerRef> resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

					// Check to make sure the Worker Server is there
					if (resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerServer, StringComparison.OrdinalIgnoreCase)))
					{
						// Make sure the Worker Server actually exists and then remove it from the pool
						ResourceServerQueryResultSet queryResult = await resourceServerManager.QueryAsync(queryWorker);

						if (queryResult.Success && queryResult.TotalCount > 0)
						{
							ResourceServer workerServer = queryResult.Results.Find(x => x.Artifact.ServerType.Name.Equals(Constants.Processing.WorkerServer, StringComparison.OrdinalIgnoreCase)).Artifact;

							ResourceServerRef workerServerRef = new ResourceServerRef()
							{
								ArtifactID = workerServer.ArtifactID,
								Name = workerServer.Name,
								ServerType = new ResourceServerTypeRef()
								{
									ArtifactID = workerServer.ServerType.ArtifactID,
									Name = workerServer.ServerType.Name
								}
							};

							await resourcePoolManager.RemoveServerAsync(workerServerRef, defaultPoolRef);

							resultServers = await resourcePoolManager.RetrieveResourceServersAsync(defaultPoolRef);

							if (!resultServers.Exists(x => x.ServerType.Name.Equals(Constants.Processing.WorkerServer, StringComparison.OrdinalIgnoreCase)))
							{
								wasRemoved = true;
								Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)} - Successfully removed Worker Server from Default Resource Pool");
							}
							else
							{
								wasRemoved = false;
								Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)} - Failed to remove Worker Server from Default Resource Pool");
							}

						}
						else
						{
							wasRemoved = true;
							Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)} - Failed to remove Worker Server from Default Resource Pool as the Worker Server does not exist");
						}
					}
					else
					{
						wasRemoved = true;
						Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePoolAsync)} - Failed to remove Worker Server from Default Resource Pool as it already is not within the pool");
					}

				}
			}


			return wasRemoved;
		}

	} // End of class
}