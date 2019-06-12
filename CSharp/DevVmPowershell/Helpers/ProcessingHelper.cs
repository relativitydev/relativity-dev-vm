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
		public async Task<bool> FullSetupAndUpdateDefaultResourcePool()
		{
			bool wasSetupComplete = false;

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Starting ({nameof(CreateProcessingSourceLocationChoice)})");
			wasSetupComplete = CreateProcessingSourceLocationChoice();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Finished ({nameof(CreateProcessingSourceLocationChoice)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Starting ({nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)})");
			wasSetupComplete = await AddProcessingSourceLocationChoiceToDefaultResourcePool();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Finished ({nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Starting ({nameof(CreateWorkerManagerServer)})");
			wasSetupComplete = await CreateWorkerManagerServer();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Finished ({nameof(CreateWorkerManagerServer)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Sleeping for 30 seconds auto creation of Worker Server");
			Thread.Sleep(30000);
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Finished sleeping for auto creation of Worker Server");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Starting ({nameof(UpdateWorkerServerForProcessing)})");
			wasSetupComplete = await UpdateWorkerServerForProcessing();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Finished ({nameof(UpdateWorkerServerForProcessing)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Starting ({nameof(AddWorkerManagerServerToDefaultResourcePool)})");
			wasSetupComplete = await AddWorkerManagerServerToDefaultResourcePool();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Finished ({nameof(AddWorkerManagerServerToDefaultResourcePool)}) Result: {wasSetupComplete}");

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Starting ({nameof(AddWorkerServerToDefaultResourcePool)})");
			wasSetupComplete = await AddWorkerServerToDefaultResourcePool();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePool)} - Finished ({nameof(AddWorkerServerToDefaultResourcePool)}) Result: {wasSetupComplete}");


			return wasSetupComplete;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<bool> FullReset()
		{
			bool wasReset = false;

			Console.WriteLine($"{nameof(FullReset)} - Starting ({nameof(RemoveWorkerServerFromDefaultResourcePool)})");
			wasReset = await RemoveWorkerServerFromDefaultResourcePool();
			Console.WriteLine($"{nameof(FullReset)} - Finished ({nameof(RemoveWorkerServerFromDefaultResourcePool)}) Result: {wasReset}");

			Console.WriteLine($"{nameof(FullReset)} - Starting ({nameof(DeleteProcessingSourceLocationChoice)})");
			wasReset = DeleteProcessingSourceLocationChoice();
			Console.WriteLine($"{nameof(FullReset)} - Finished ({nameof(DeleteProcessingSourceLocationChoice)}) Result: {wasReset}");

			Console.WriteLine($"{nameof(FullReset)} - Starting ({nameof(RemoveWorkerServerFromDefaultResourcePool)})");
			wasReset = await RemoveWorkerServerFromDefaultResourcePool();
			Console.WriteLine($"{nameof(FullReset)} - Finished ({nameof(RemoveWorkerServerFromDefaultResourcePool)}) Result: {wasReset}");

			Console.WriteLine($"{nameof(FullReset)} - Starting ({nameof(RemoveWorkerManagerServerFromDefaultResourcePool)})");
			wasReset = await RemoveWorkerManagerServerFromDefaultResourcePool();
			Console.WriteLine($"{nameof(FullReset)} - Finished ({nameof(RemoveWorkerManagerServerFromDefaultResourcePool)}) Result: {wasReset}");

			Console.WriteLine($"{nameof(FullReset)} - Starting ({nameof(DeleteWorkerManagerServer)})");
			wasReset = await DeleteWorkerManagerServer();
			Console.WriteLine($"{nameof(FullReset)} - Finished ({nameof(DeleteWorkerManagerServer)}) Result: {wasReset}");


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
		/// Deletes a Processing Source Location Choice and makes sure it doesn't already exist.
		/// </summary>
		/// <returns></returns>
		public bool DeleteProcessingSourceLocationChoice()
		{
			bool wasChoiceDeleted = false;

			Console.WriteLine($"{nameof(DeleteProcessingSourceLocationChoice)} - Deleting Processing Source Location Choice ({Constants.Processing.ChoiceName})");

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
					WriteResultSet<Choice> result = rsapiClient.Repositories.Choice.Delete(choiceQuery.Results.First().Artifact);

					if (result.Success)
					{
						choiceQuery = rsapiClient.Repositories.Choice.Query(new Query<Choice>() { Condition = cond });
						if (choiceQuery.Success && choiceQuery.TotalCount == 0)
						{
							wasChoiceDeleted = true;
							Console.WriteLine($"{nameof(DeleteProcessingSourceLocationChoice)} - Successfully deleted Processing Source Location Choice ({Constants.Processing.ChoiceName})");
						}
						else
						{
							Console.WriteLine($"{nameof(DeleteProcessingSourceLocationChoice)} - Failed to create Processing Source Location Choice ({Constants.Processing.ChoiceName})");
						}

					}
					else
					{
						Console.WriteLine($"{nameof(DeleteProcessingSourceLocationChoice)} - Failed to delete Processing Source Location Choice ({Constants.Processing.ChoiceName})");
					}
				}
			}

			return wasChoiceDeleted;
		}

		/// <summary>
		/// Add Processing Source Location choice to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePool()
		{
			bool wasChoiceAddedToPool = false;

			Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Adding Processing Source Location Choice to Default Resource Pool");

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

				Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Checking if Default Resource Pool exists");
				if (resultPools.Success && resultPools.TotalCount > 0)
				{
					ResourcePoolRef defaultResourcePoolRef = new ResourcePoolRef(resultPools.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact.ArtifactID);

					// Check if Processing Server Location already added to Default Resource Pool
					Relativity.Services.Objects.DataContracts.QueryResult resultChoice = await objectManager.QueryAsync(Constants.Processing.WorkspaceId, queryChoice, 1, 10);

					Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Checking if Processing Source Location Choice exists");
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
								Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Added Processing Source Location Choice to Default Resource Pool");
							}
						}
						else
						{
							Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Failed to add Processing Source Location Choice to Default Resource Pool as it already exists within the pool");
							wasChoiceAddedToPool = true;
							return wasChoiceAddedToPool;
						}
					}
					else
					{
						Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Processing Source Location Choice does not exist");
						wasChoiceAddedToPool = true;
					}

				}
				else
				{
					Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Default Resource Pool does not exist");
				}
			}

			return wasChoiceAddedToPool;
		}

		/// <summary>
		/// Create Worker Manager Server if it doesn't already exist, which also automatically creates a Worker Server
		/// </summary>
		/// <returns></returns>
		public async Task<bool> CreateWorkerManagerServer()
		{
			bool wasWorkerManagerServerCreated = false;

			Console.WriteLine($"{nameof(CreateWorkerManagerServer)} - Creating Worker Manager Server ({Constants.Processing.ResourceServerName})");

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
					Console.WriteLine($"{nameof(CreateWorkerManagerServer)} - Worker Server Artifact ID: {workerServerArtifactId}");

					queryResult = await workerServerManager.QueryAsync(queryServer);

					if (queryResult.Success && queryResult.TotalCount > 0)
					{
						wasWorkerManagerServerCreated = true;
						Console.WriteLine($"{nameof(CreateWorkerManagerServer)} - Successfully created Worker Manager Server ({Constants.Processing.ResourceServerName})");
					}
				}
				else if (queryResult.Success && queryResult.TotalCount > 0)
				{
					Console.WriteLine($"{nameof(CreateWorkerManagerServer)} - Failed to create Worker Manager Server ({Constants.Processing.ResourceServerName}) as it already exists");
					wasWorkerManagerServerCreated = true;
				}
				else
				{
					Console.WriteLine($"{nameof(CreateWorkerManagerServer)} - Failed to create and check for Worker Manager Server ({Constants.Processing.ResourceServerName})");
				}
			}
			return wasWorkerManagerServerCreated;
		}

		/// <summary>
		/// Delete Worker Manager Server from the Instance
		/// </summary>
		/// <returns></returns>
		public async Task<bool> DeleteWorkerManagerServer()
		{
			bool wasDeleted = false;

			Console.WriteLine($"{nameof(DeleteWorkerManagerServer)} - Creating Worker Manager Server ({Constants.Processing.ResourceServerName})");

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
						Console.WriteLine($"{nameof(DeleteWorkerManagerServer)} - Successfully deleted Worker Manager Server ({Constants.Processing.ResourceServerName})");
					}
				}
				else if (queryResult.Success && queryResult.TotalCount == 0)
				{
					Console.WriteLine($"{nameof(DeleteWorkerManagerServer)} - Failed to delete Worker Manager Server ({Constants.Processing.ResourceServerName}) as it doesn't exist");
					wasDeleted = true;
				}
				else
				{
					Console.WriteLine($"{nameof(DeleteWorkerManagerServer)} - Failed to delete and check for Worker Manager Server ({Constants.Processing.ResourceServerName})");
				}
			}
			return wasDeleted;
		}

		public async Task<bool> UpdateWorkerServerForProcessing()
		{
			bool wasUpdated = false;

			Console.WriteLine($"{nameof(UpdateWorkerServerForProcessing)} - Updating Worker Server ({Constants.Processing.WorkerServer}) for to be ready for Processing");

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
					Console.WriteLine($"{nameof(UpdateWorkerServerForProcessing)} - Successfully updated Worker Server ({Constants.Processing.ResourceServerName})");
				}
				else
				{
					Console.WriteLine($"{nameof(UpdateWorkerServerForProcessing)} - Failed to update Worker Server ({Constants.Processing.ResourceServerName}) as it doesn't exist");
				}
			}

			return wasUpdated;
		}

		/// <summary>
		/// Add Worker Manager Server to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddWorkerManagerServerToDefaultResourcePool()
		{
			bool wasWorkerManagerServerAddedToDefaultPool = false;

			Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePool)} - Adding Worker Manager Server ({Constants.Processing.ResourceServerName}) to the Default Resource Pool");

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

				Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePool)} - Checking if Default Resource Pool exists");
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
								Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePool)} - Successfully added Worker Manager Server to Default Resource Pool");
							}
							else
							{
								wasWorkerManagerServerAddedToDefaultPool = false;
								Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePool)} - Failed to add Worker Manager Server to Default Resource Pool.");
							}
						}
						else
						{
							Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePool)} - Failed to add Worker Manager Server to Default Resource Pool as the Worker Manager Server does not exist");
						}
					}
					else
					{
						wasWorkerManagerServerAddedToDefaultPool = true;
						Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePool)} - Failed to add Worker Manager Server to Default Resource Pool as it already exists within the pool");
					}

				}
			}

			return wasWorkerManagerServerAddedToDefaultPool;
		}


		/// <summary>
		/// Add Worker Server to Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AddWorkerServerToDefaultResourcePool()
		{
			bool wasWorkerServerAddedToDefaultPool = false;

			Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePool)} - Adding Worker Server ({Constants.Processing.WorkerServer}) to the Default Resource Pool");

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

				Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePool)} - Checking if Default Resource Pool exists");
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
								Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePool)} - Successfully added Worker Server to Default Resource Pool");
							}
							else
							{
								wasWorkerServerAddedToDefaultPool = false;
								Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePool)} - Failed to add Worker Server to Default Resource Pool.");
							}

						}
						else
						{
							Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePool)} - Failed to add Worker Server to Default Resource Pool as the Worker Server does not exist");
						}
					}
					else
					{
						wasWorkerServerAddedToDefaultPool = true;
						Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePool)} - Failed to add Worker Server to Default Resource Pool as it already exists within the pool");
					}

				}
			}

			return wasWorkerServerAddedToDefaultPool;
		}

		/// <summary>
		/// Removing Worker Manager Server from Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> RemoveWorkerManagerServerFromDefaultResourcePool()
		{
			bool wasRemoved = false;

			Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePool)} - Removing Worker Manager Server ({Constants.Processing.ResourceServerName}) remove the Default Resource Pool");

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

				Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePool)} - Checking if Default Resource Pool exists");
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
								Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePool)} - Successfully removed Worker Manager Server from Default Resource Pool");
							}
							else
							{
								wasRemoved = false;
								Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePool)} - Failed to removed Worker Manager Server from Default Resource Pool.");
							}
						}
						else
						{
							wasRemoved = true;
							Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePool)} - Failed to removed Worker Manager Server from Default Resource Pool as the Worker Manager Server does not exist");
						}
					}
					else
					{
						wasRemoved = true;
						Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePool)} - Failed to removed Worker Manager Server from Default Resource Pool as it is not within the pool");
					}

				}
			}

			return wasRemoved;
		}

		/// <summary>
		/// Removing Worker Server from Default Resource Pool
		/// </summary>
		/// <returns></returns>
		public async Task<bool> RemoveWorkerServerFromDefaultResourcePool()
		{
			bool wasRemoved = false;

			Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePool)} - Deleting Worker Server ({Constants.Processing.ResourceServerName})");

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

				Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePool)} - Checking if Default Resource Pool exists");
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
								Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePool)} - Successfully removed Worker Server from Default Resource Pool");
							}
							else
							{
								wasRemoved = false;
								Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePool)} - Failed to remove Worker Server from Default Resource Pool");
							}

						}
						else
						{
							wasRemoved = true;
							Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePool)} - Failed to remove Worker Server from Default Resource Pool as the Worker Server does not exist");
						}
					}
					else
					{
						wasRemoved = true;
						Console.WriteLine($"{nameof(RemoveWorkerServerFromDefaultResourcePool)} - Failed to remove Worker Server from Default Resource Pool as it already is not within the pool");
					}

				}
			}


			return wasRemoved;
		}

	} // End of class
}