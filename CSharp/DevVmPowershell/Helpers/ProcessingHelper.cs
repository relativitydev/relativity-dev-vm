using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ResourcePool;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
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
		/// Creates a Processing Source Location Choice and makes sure it doesn't already exist.
		/// </summary>
		/// <returns></returns>
		public bool CreateProcessingSourceLocationChoice()
		{
			bool wasChoiceCreated = false;

			Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Creating Processing Source Location Choice ({Constants.Processing.ChoiceName})");

			Choice choice = new Choice();
			choice.Name = Constants.Processing.ChoiceName;
			choice.ChoiceTypeID = Constants.Processing.ChoiceTypeID;
			choice.Order = 1;

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
					wasChoiceCreated = true;
					Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Successfully created Processing Source Location Choice ({Constants.Processing.ChoiceName})");
				}
				else
				{
					Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoice)} - Failed to create Processing Source Location Choice ({Constants.Processing.ChoiceName})");
				}
			}

			return wasChoiceCreated;
		}


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

						// Add Processing Server Location to Default Resource Pool
						if (!resultProcessingSourceLocations.Exists(x => x.ArtifactID == choice.ArtifactID))
						{
							Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Adding Processing Source Location Choice to Default Resource Pool");
							await resourcePoolManager.AddProcessingSourceLocationAsync(choice, defaultResourcePoolRef);
							wasChoiceAddedToPool = true;
						}
						else
						{
							Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Failed to add Processing Source Location Choice to Default Resource Pool as it already exists");
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
		/// NOTE: This class uses private calls (IResourcePoolManager)
		/// </summary>
		/// <returns></returns>
		//public async Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePool()
		//{
		//	bool wasChoiceAddedToPool = false;

		//	Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePool)} - Adding Processing Source Location Choice to Default Resource Pool");

		//	Relativity.Services.TextCondition condition = new Relativity.Services.TextCondition()
		//	{
		//		Field = Constants.Processing.NameField,
		//		Operator = Relativity.Services.TextConditionEnum.StartsWith,
		//		Value = Constants.Processing.DefaultPool
		//	};

		//	Relativity.Services.Query query = new Relativity.Services.Query()
		//	{
		//		Condition = condition.ToQueryString(),

		//	};

		//	using (IResourcePoolManager resourcePoolManager = ServiceFactory.CreateProxy<IResourcePoolManager>())
		//	{
		//		// Check for Default Resource Pool
		//		ResourcePoolQueryResultSet resultPools = await resourcePoolManager.QueryAsync(query);

		//		// Check for Worker Server
		//		if (resultPools.Success && resultPools.TotalCount > 0)
		//		{
		//			List<ChoiceRef> resultsServers = await resourcePoolManager.GetResourceServerTypeChoicesAsync();

		//			if (resultsServers.Exists(x => x.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)))
		//			{
		//				// Add Processing Server to Default Resource Pool
		//				ResourceServer resourceServer = new ResourceServer()
		//				{
		//					ArtifactID = 99999999,
		//					ServerType = new ResourceServerTypeRef()
		//					{
		//						ArtifactID = resultsServers.Find(x => x.Name.Equals(Constants.Processing.WorkerManagerServer, StringComparison.OrdinalIgnoreCase)).ArtifactID
		//					}
		//				};
		//				//ResourcePool resourcePool = new ResourcePool()
		//				//{

		//				//};
		//				ResourcePool defaultResourcePool = resultPools.Results.Find(x => x.Artifact.Name.Equals(Constants.Processing.DefaultPool, StringComparison.OrdinalIgnoreCase)).Artifact;

		//				Task resultNewServer = resourcePoolManager.AddServerAsync(resourceServer, defaultResourcePool);

		//				if (resultNewServer.IsCompleted)
		//				{
		//					ChoiceRef processingSourceLocation = new ChoiceRef() { };
		//					resourcePoolManager.AddProcessingSourceLocationAsync(processingSourceLocation, defaultResourcePool);
		//				}
		//			}
		//		}
		//	}

		//	return wasChoiceAddedToPool;
		//}
	}
}