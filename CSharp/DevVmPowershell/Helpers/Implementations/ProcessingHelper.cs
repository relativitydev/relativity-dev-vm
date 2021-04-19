using Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Implementations
{
	public class ProcessingHelper : IProcessingHelper
	{
		private IConnectionHelper ConnectionHelper { get; }
		private IRestHelper RestHelper { get; set; }
		public ProcessingHelper(IConnectionHelper connectionHelper, IRestHelper restHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
		}

		/// <summary>
		/// Runs through every step needed to create Objects and setup to the Default Resource Pool.  Objects created and added to Default Pool are a Processing Source Location, a Worker Server Manager, and a Worker Server
		/// </summary>
		/// <returns></returns>
		public async Task<bool> FullSetupAndUpdateDefaultResourcePoolAsync()
		{
			bool wasSetupComplete = false;

			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Starting ({nameof(CreateProcessingSourceLocationChoiceAsync)})");
			wasSetupComplete = await CreateProcessingSourceLocationChoiceAsync();
			Console.WriteLine($"{nameof(FullSetupAndUpdateDefaultResourcePoolAsync)} - Finished ({nameof(CreateProcessingSourceLocationChoiceAsync)}) Result: {wasSetupComplete}");

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
		public async Task<bool> CreateProcessingSourceLocationChoiceAsync()
		{
			bool wasChoiceCreated = false;

			Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoiceAsync)} - Creating Processing Source Location Choice ({Constants.Processing.ChoiceName})");

			string url = Constants.Connection.RestUrlEndpoints.ProcessingManager.ProcessingSourceCreateUrl;
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			var createPayloadObject = new
			{

				choiceRequest = new
				{
					Field = new { Guids = new List<Guid>{Guid.Parse(Constants.Processing.ProcessingSourceLocationFieldGuid)}},
					Name = Constants.Processing.ChoiceName,
					Order = 100,
					Keywords = "DevVM Processing Source Location",
					Notes = "",
					RelativityApplications = new string[] { },
				}
			};
			string createPayload = JsonConvert.SerializeObject(createPayloadObject);
		  HttpResponseMessage createResponse = await RestHelper.MakePostAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.ProcessingManager.ProcessingSourceCreateUrl}/", createPayload);
		  if (!createResponse.IsSuccessStatusCode)
		  {
			  string responseContent = await createResponse.Content.ReadAsStringAsync();
			  throw new Exception($"Failed to Create Processing Source Location. [{nameof(responseContent)}: {responseContent}]");
		  }
		  string resultString = await createResponse.Content.ReadAsStringAsync();
		  int processingChoiceId = Int32.Parse(resultString);
		  if (processingChoiceId < 0)
			{
				wasChoiceCreated = false;
				Console.WriteLine($"{nameof(CreateProcessingSourceLocationChoiceAsync)} - Failed to create Processing Source Location Choice ({Constants.Processing.ChoiceName})");
			}
			else
			{
				wasChoiceCreated = true;
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

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Get Default Resource Pool Artifact Id
			int defaultResourcePoolArtifactId = await GetDefaultResourcePoolArtifactIdAsync(httpClient);
			if (defaultResourcePoolArtifactId != -1)
			{
				// Get Processing Choice Location Id
				int processingChoiceId = await GetProcessingSourceLocationArtifactIdAsync(httpClient);
				if (processingChoiceId != -1)
				{
					// Check if Processing Source Location Already Exists in Default Resource Pool
					if (!(await DoesProcessingSourceLocationAlreadyExistForDefaultResourcePoolAsync(httpClient,
						defaultResourcePoolArtifactId, processingChoiceId)))
					{
						// Add Processing Source Location to Default Resource Pool
						await CallAddProcessingSourceLocationToDefaultResourcePoolAsync(httpClient, defaultResourcePoolArtifactId, processingChoiceId);
						Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Added Processing Source Location Choice to Default Resource Pool");
					}
					else
					{
						Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Failed to add Processing Source Location Choice to Default Resource Pool as it already exists within the pool");
					}
					wasChoiceAddedToPool = true;
				}
				else
				{
					Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Processing Source Location Choice does not exist");
				}
			}
			else
			{
				Console.WriteLine($"{nameof(AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync)} - Default Resource Pool does not exist");
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

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Check if Worker Manager Server is already exists
			if (await GetWorkerManagerServerArtifactIdAsync(httpClient) == -1)
			{
				// Create Worker Manager Server
				await CallCreateWorkerManagerServerAsync(httpClient);
				Console.WriteLine($"{nameof(CreateWorkerManagerServerAsync)} - Successfully created Worker Manager Server ({Constants.Processing.ResourceServerName})");
				wasWorkerManagerServerCreated = true;
			}
			else
			{
				Console.WriteLine($"{nameof(CreateWorkerManagerServerAsync)} - Failed to create Worker Manager Server ({Constants.Processing.ResourceServerName}) as it already exists");
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

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Check if Worker Manager Server is already exists
			int workerManagerServerId = await GetWorkerManagerServerArtifactIdAsync(httpClient);
			if (workerManagerServerId != -1)
			{
				// Delete Worker Manager Server
				await CallDeleteWorkerManagerServerAsync(httpClient, workerManagerServerId);
				Console.WriteLine($"{nameof(DeleteWorkerManagerServerAsync)} - Successfully deleted Worker Manager Server ({Constants.Processing.ResourceServerName})");
				wasDeleted = true;
			}
			else
			{
				Console.WriteLine($"{nameof(DeleteWorkerManagerServerAsync)} - Failed to delete Worker Manager Server ({Constants.Processing.ResourceServerName}) as it doesn't exist");
			}

			return wasDeleted;
		}

		public async Task<bool> UpdateWorkerServerForProcessingAsync()
		{
			bool wasUpdated = false;

			Console.WriteLine($"{nameof(UpdateWorkerServerForProcessingAsync)} - Updating Worker Server ({Constants.Processing.WorkerServer}) for to be ready for Processing");

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Get Worker Server Artifact Id
			int workerServerArtifactId = await GetWorkerServerArtifactIdAsync(httpClient);
			if (workerServerArtifactId != -1)
			{
				// Enable Processing on Worker Server
				await EnableProcessingOnWorkerServerAsync(httpClient, workerServerArtifactId);
				Thread.Sleep(10000);
				// Update Worker Server for Processing
				await UpdateCategoriesOnWorkerForProcessingAsync(httpClient, workerServerArtifactId);

				wasUpdated = true;
				Console.WriteLine($"{nameof(UpdateWorkerServerForProcessingAsync)} - Successfully updated Worker Server ({Constants.Processing.ResourceServerName})");
			}
			else
			{
				Console.WriteLine($"{nameof(UpdateWorkerServerForProcessingAsync)} - Failed to update Worker Server ({Constants.Processing.ResourceServerName}) as it doesn't exist");
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
			
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Get Default Resource Pool Artifact Id
			int defaultResourcePoolArtifactId = await GetDefaultResourcePoolArtifactIdAsync(httpClient);
			if (defaultResourcePoolArtifactId != -1)
			{
				// Get Worker Manager Server Type Artifact Id
				int workerManagerServerTypeArtifactId = await GetWorkerManagerServerTypeArtifactIdAsync(httpClient);
				if (workerManagerServerTypeArtifactId != -1)
				{
					// Get Worker Manager Server Artifact Id
					int workerManagerServerArtifactId = await GetWorkerManagerServerArtifactIdAsync(httpClient);
					if (workerManagerServerArtifactId != -1)
					{
						// Add Worker Manager Server to Default Resource Pool
						await AddServerToDefaultResourcePoolAsync(httpClient, workerManagerServerArtifactId,
							workerManagerServerTypeArtifactId, defaultResourcePoolArtifactId);
						wasWorkerManagerServerAddedToDefaultPool = true;
						Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Successfully added Worker Manager Server to Default Resource Pool");
					}
					else
					{
						Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Failed to add Worker Manager Server to Default Resource Pool as the Worker Manager Server does not exist");
					}
				}
				else
				{
					Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Failed to get Worker Manager Server Type Id");
				}
			}
			else
			{
				Console.WriteLine($"{nameof(AddWorkerManagerServerToDefaultResourcePoolAsync)} - Failed to get Default Resource Pool Id");
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

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Get Default Resource Pool Artifact Id
			int defaultResourcePoolArtifactId = await GetDefaultResourcePoolArtifactIdAsync(httpClient);
			if (defaultResourcePoolArtifactId != -1)
			{
				// Get Worker Server Type Artifact Id
				int workerServerTypeArtifactId = await GetWorkerServerTypeArtifactIdAsync(httpClient);
				if (workerServerTypeArtifactId != -1)
				{
					// Get Worker Server Artifact Id
					int workerServerArtifactId = await GetWorkerServerArtifactIdAsync(httpClient);
					if (workerServerArtifactId != -1)
					{
						// Add Worker Server to Default Resource Pool
						await AddServerToDefaultResourcePoolAsync(httpClient, workerServerArtifactId,
							workerServerTypeArtifactId, defaultResourcePoolArtifactId);
						wasWorkerServerAddedToDefaultPool = true;
						Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Successfully added Worker Server to Default Resource Pool");
					}
					else
					{
						Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Failed to add Worker Server to Default Resource Pool as the Worker does not exist");
					}
				}
				else
				{
					Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Failed to get Worker Server Type Id");
				}
			}
			else
			{
				Console.WriteLine($"{nameof(AddWorkerServerToDefaultResourcePoolAsync)} - Failed to get Default Resource Pool Id");
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

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Get Default Resource Pool Artifact Id
			int defaultResourcePoolArtifactId = await GetDefaultResourcePoolArtifactIdAsync(httpClient);
			if (defaultResourcePoolArtifactId != -1)
			{
				// Get Worker Manager Server Type Artifact Id
				int workerManagerServerTypeArtifactId = await GetWorkerManagerServerTypeArtifactIdAsync(httpClient);
				if (workerManagerServerTypeArtifactId != -1)
				{
					// Get Worker Manager Server Artifact Id
					int workerManagerServerArtifactId = await GetWorkerManagerServerArtifactIdAsync(httpClient);
					if (workerManagerServerArtifactId != -1)
					{
						// Remove Worker Manager Server from Default Resource Pool
						await RemoveServerFromDefaultResourcePoolAsync(httpClient, workerManagerServerArtifactId, workerManagerServerTypeArtifactId, defaultResourcePoolArtifactId);
						wasRemoved = true;
						Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Successfully removed Worker Manager Server from Default Resource Pool");
					}
					else
					{
						Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to removed Worker Manager Server from Default Resource Pool as the Worker Manager Server does not exist");
					}
				}
				else
				{
					Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to get Worker Manager Server Type Id");
				}
			}
			else
			{
				Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to get Default Resource Pool Id");
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

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			// Get Default Resource Pool Artifact Id
			int defaultResourcePoolArtifactId = await GetDefaultResourcePoolArtifactIdAsync(httpClient);
			if (defaultResourcePoolArtifactId != -1)
			{
				// Get Worker Server Type Artifact Id
				int workerServerTypeArtifactId = await GetWorkerServerTypeArtifactIdAsync(httpClient);
				if (workerServerTypeArtifactId != -1)
				{
					// Get Worker Server Artifact Id
					int workerServerArtifactId = await GetWorkerServerArtifactIdAsync(httpClient);
					if (workerServerArtifactId != -1)
					{
						// Remove Worker Server from Default Resource Pool
						await RemoveServerFromDefaultResourcePoolAsync(httpClient, workerServerArtifactId, workerServerTypeArtifactId, defaultResourcePoolArtifactId);
						wasRemoved = true;
						Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Successfully removed Worker Server from Default Resource Pool");
					}
					else
					{
						Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to removed Worker Server from Default Resource Pool as the Worker Server does not exist");
					}
				}
				else
				{
					Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to get Worker Server Type Id");
				}
			}
			else
			{
				Console.WriteLine($"{nameof(RemoveWorkerManagerServerFromDefaultResourcePoolAsync)} - Failed to get Default Resource Pool Id");
			}

			return wasRemoved;
		}

		#region Private Helpers

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
			string queryPoolResultString = await queryPoolResponse.Content.ReadAsStringAsync();
			if (!queryPoolResponse.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to query for Default Resource Pool : {queryPoolResultString}");
			}
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

		private async Task<int> GetProcessingSourceLocationArtifactIdAsync(HttpClient httpClient)
		{
			var processingChoiceQuery = new
			{
				condition = "'Choice Type ID' == 1000017",
				fields = new object[]
				{
					"*"
				}
			};
			string request = JsonConvert.SerializeObject(processingChoiceQuery);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.Choice.QueryEndpointUrl, request);
			string resultString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Query for Processing Choice Location : {resultString}");
			}
			dynamic result = JObject.Parse(resultString) as JObject;
			if (Convert.ToInt32(result.ResultCount) > 0)
			{
				return Convert.ToInt32(result.Results[0]["Artifact ID"].ToString());
			}
			else
			{
				return -1;
			}
		}

		private async Task<bool> DoesProcessingSourceLocationAlreadyExistForDefaultResourcePoolAsync(HttpClient httpClient,
			int defaultResourcePoolId, int processingChoiceId)
		{
			var getProcessingSourceLocations = new
			{
				resourcePool = new
				{
					ArtifactID = defaultResourcePoolId
				}
			};
			string request = JsonConvert.SerializeObject(getProcessingSourceLocations);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.GetProcessingSourceLocationsUrl, request);
			string resultString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Get Processing Choice Locations for Resource Pool : {resultString}");
			}
			dynamic result = JsonConvert.DeserializeObject<dynamic>(resultString);
		  bool doesProcessingChoiceExist = false;
		  foreach (dynamic obj in result)
		  {
			  if (processingChoiceId == Convert.ToInt32(obj["ArtifactID"]))
			  {
				  doesProcessingChoiceExist = true;
				  break;
			  }
		  }

		  return doesProcessingChoiceExist;
		}

		private async Task CallAddProcessingSourceLocationToDefaultResourcePoolAsync(HttpClient httpClient, int defaultResourcePoolId, int processingSourceId)
		{
			var addProcessingSource = new
			{
				resourcePool = new
				{
					ArtifactID = defaultResourcePoolId
				},
				ProcessingSourceLocation = new
				{
					ArtifactID = processingSourceId
				}
			};
			string request = JsonConvert.SerializeObject(addProcessingSource);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.AddProcessingSourceLocationUrl, request);
			string responseString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Add Processing Choice Location to Default Resource Pool : {responseString}");
			}
		}

		private async Task<int> GetWorkerManagerServerArtifactIdAsync(HttpClient httpClient)
		{
			var workerManagerQuery = new
			{
				Query = new
				{
					Condition = $"'ServerType' == 'Worker Manager Server' AND 'Name' == '{Constants.Processing.ResourceServerName}'",
					Sorts = new object[]{}
				}
			};
			string request = JsonConvert.SerializeObject(workerManagerQuery);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourceServer.QueryEndpointUrl, request);
			string resultString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Query for Worker Manager Server: {resultString}");
			}
			dynamic result = JObject.Parse(resultString) as JObject;
			if (Convert.ToInt32(result.TotalCount) > 0)
			{
				return Convert.ToInt32(result.Results[0]["Artifact"]["ArtifactID"].ToString());
			}
			else
			{
				return -1;
			}
		}

		private async Task CallCreateWorkerManagerServerAsync(HttpClient httpClient)
		{
			var workerManagerServerCreate = new
			{
				resourceServer = new
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
				}
			};
			string request = JsonConvert.SerializeObject(workerManagerServerCreate);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourceServer.WorkerManagerCreateEndpointUrl, request);
			string responseString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Create Worker Manager Server : {responseString}");
			}
		}

		private async Task CallDeleteWorkerManagerServerAsync(HttpClient httpClient, int workerManagerServerArtifactId)
		{
			var workerManagerServerDelete = new
			{
				resourceServerID = workerManagerServerArtifactId
			};
			string request = JsonConvert.SerializeObject(workerManagerServerDelete);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourceServer.WorkerManagerDeleteEndpointUrl, request);
			string responseString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Delete Worker Manager Server : {responseString}");
			}
		}

		private async Task<int> GetWorkerServerArtifactIdAsync(HttpClient httpClient)
		{
			int workerServerArtifactId = -1;
			var resourceServerQueryPayload = new
			{
				Query = new
				{
					Condition = ""
				}
			};
			string serverQuery = JsonConvert.SerializeObject(resourceServerQueryPayload);
			HttpResponseMessage queryServerResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourceServer.QueryEndpointUrl, serverQuery);
			string queryServerResultString = await queryServerResponse.Content.ReadAsStringAsync();
			if (!queryServerResponse.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to query for Worker Resource Server : {queryServerResultString}");
			}
			dynamic queryServerResult = JObject.Parse(queryServerResultString) as JObject;
			if (Convert.ToInt32(queryServerResult.TotalCount) > 0)
			{
				foreach (dynamic obj in queryServerResult.Results)
				{
					if (obj["Artifact"]["ServerType"]["Name"].ToString() == "Worker")
					{
						workerServerArtifactId = Convert.ToInt32(obj["Artifact"]["ArtifactID"].ToString());
						break;
					}
				}
			}

			return workerServerArtifactId;
		}

		private async Task EnableProcessingOnWorkerServerAsync(HttpClient httpClient, int workerServerId)
		{
			var enableProcessingOnWorker = new
			{
				workerArtifactId = workerServerId
			};
			string request = JsonConvert.SerializeObject(enableProcessingOnWorker);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourceServer.EnableProcessingOnWorkerEndpointUrl, request);
			string responseString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Enable Processing on Worker Server : {responseString}");
			}
		}

		private async Task UpdateCategoriesOnWorkerForProcessingAsync(HttpClient httpClient, int workerServerId)
		{
			var updateCategories = new
			{
				workerArtifactId = workerServerId,
				categories = new string[]
				{
					"NativeImaging",
					"BasicImaging",
					"SaveAsPDF",
					"Processing"
				}
			};
			string request = JsonConvert.SerializeObject(updateCategories);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourceServer.UpdateCategoriesOnWorkerEndpointUrl, request);
			string responseString = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to Update Categories on Worker Server : {responseString}");
			}
		}

		private async Task<int> GetWorkerManagerServerTypeArtifactIdAsync(HttpClient httpClient)
		{
			int workerManagerServerTypeArtifactId = -1;
			HttpResponseMessage serverTypeQueryResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.ResourceServerTypeQueryEndpointUrl, "");
			string queryServerTypesResultString = await serverTypeQueryResponse.Content.ReadAsStringAsync();
			if (!serverTypeQueryResponse.IsSuccessStatusCode)
			{
				throw new System.Exception("Failed to query for Worker Manager Server Types : {query}");
			}
			dynamic queryServerTypesResult = JsonConvert.DeserializeObject<dynamic>(queryServerTypesResultString);
			foreach (dynamic obj in queryServerTypesResult)
			{
				if (obj["Name"].ToString() == "Worker Manager Server")
				{
					workerManagerServerTypeArtifactId = Convert.ToInt32(obj["ArtifactID"].ToString());
					break;
				}
			}

			return workerManagerServerTypeArtifactId;
		}

		private async Task<int> GetWorkerServerTypeArtifactIdAsync(HttpClient httpClient)
		{
			int workerServerTypeArtifactId = -1;
			HttpResponseMessage serverTypeQueryResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.ResourceServerTypeQueryEndpointUrl, "");
			string queryServerTypesResultString = await serverTypeQueryResponse.Content.ReadAsStringAsync();
			if (!serverTypeQueryResponse.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to query for Worker Server Types : {queryServerTypesResultString}");
			}
			dynamic queryServerTypesResult = JsonConvert.DeserializeObject<dynamic>(queryServerTypesResultString);
			foreach (dynamic obj in queryServerTypesResult)
			{
				if (obj["Name"].ToString() == "Worker")
				{
					workerServerTypeArtifactId = Convert.ToInt32(obj["ArtifactID"].ToString());
					break;
				}
			}

			return workerServerTypeArtifactId;
		}

		private async Task AddServerToDefaultResourcePoolAsync(HttpClient httpClient, int serverArtifactId, int serverTypeArtifactId, int defaultResourcePoolArtifactId)
		{
			var addResourceServerPayload = new
			{
				resourceServer = new
				{
					ArtifactID = serverArtifactId,
					ServerType = new
					{
						ArtifactID = serverTypeArtifactId
					}
				},
				resourcePool = new
				{
					ArtifactID = defaultResourcePoolArtifactId
				}
			};
			string addResourceServer = JsonConvert.SerializeObject(addResourceServerPayload);
			HttpResponseMessage addServerResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.AddServerEndpointUrl, addResourceServer);
			string responseString = await addServerResponse.Content.ReadAsStringAsync();
			if (!addServerResponse.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to add the Server to the Default Resource Pool : {responseString}");
			}
		}

		private async Task RemoveServerFromDefaultResourcePoolAsync(HttpClient httpClient, int serverArtifactId, int serverTypeArtifactId, int defaultResourcePoolArtifactId)
		{
			var removeServerResourcePoolPayload = new
			{
				resourceServer = new
				{
					ArtifactID = serverArtifactId,
					ServerType = new
					{
						ArtifactID = serverTypeArtifactId
					}
				},
				resourcePool = new
				{
					ArtifactID = defaultResourcePoolArtifactId
				}
			};
			string removeServer = JsonConvert.SerializeObject(removeServerResourcePoolPayload);
			HttpResponseMessage removeServerResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ResourcePool.RemoveServerUrl, removeServer);
			string responseString = await removeServerResponse.Content.ReadAsStringAsync();
			if (!removeServerResponse.IsSuccessStatusCode)
			{
				throw new System.Exception($"Failed to remove the Server from the Default Resource Pool : {responseString}");
			}
		}

		#endregion

	} // End of class
}