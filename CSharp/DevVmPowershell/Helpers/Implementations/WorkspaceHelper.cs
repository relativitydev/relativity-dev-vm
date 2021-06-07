using Helpers.CustomExceptions;
using Helpers.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class WorkspaceHelper : IWorkspaceHelper
	{
		private readonly ILogService _logService;
		private IConnectionHelper ConnectionHelper { get; }
		public ISqlHelper SqlHelper { get; set; }
		private IRestHelper RestHelper { get; set; }
		private IRetryLogicHelper RetryLogicHelper { get; set; }

		public WorkspaceHelper(ILogService logService, IConnectionHelper connectionHelper, IRestHelper restHelper, ISqlHelper sqlHelper, IRetryLogicHelper retryLogicHelper)
		{
			_logService = logService;
			ConnectionHelper = connectionHelper;
			SqlHelper = sqlHelper;
			RestHelper = restHelper;
			RetryLogicHelper = retryLogicHelper;
		}

		public async Task<int> CreateSingleWorkspaceAsync(string workspaceTemplateName, string workspaceName, bool enableDataGrid)
		{
			try
			{
				_logService.LogDebug($"Create Single Workspace with the following input parameters - {workspaceTemplateName}, {workspaceName} and {enableDataGrid}", workspaceTemplateName, workspaceName, enableDataGrid);

				// Query for the Workspace Template
				List<int> workspaceArtifactIds = await WorkspaceQueryAsync(workspaceTemplateName);
				if (workspaceArtifactIds.Count == 0)
				{
					throw new Exception($"Template workspace doesn't exist [Name: {workspaceTemplateName}]");
				}
				if (workspaceArtifactIds.Count > 1)
				{
					throw new Exception($"Multiple Template workspaces exist with the same name [Name: {workspaceTemplateName}]");
				}

				int templateWorkspaceArtifactId = workspaceArtifactIds.First();
				_logService.LogDebug($"Queried for Workspace Template - {templateWorkspaceArtifactId}", templateWorkspaceArtifactId);

				// Create the workspace 
				int workspaceArtifactId = await CreateWorkspaceAsync(templateWorkspaceArtifactId, workspaceName, enableDataGrid);
				_logService.LogDebug($"Workspace created - {workspaceArtifactId}", workspaceArtifactId);

				return workspaceArtifactId;
			}
			catch (Exception ex)
			{
				var errorMessage = $"An error occurred when creating a single workspace [{nameof(workspaceName)}: {workspaceName}]";
				_logService.LogError(errorMessage, ex, workspaceName);
				throw new DevVmPowerShellModuleHelperException(errorMessage, ex);
			}
		}

		public async Task DeleteAllWorkspacesAsync(string workspaceName)
		{
			List<int> workspaceArtifactIds = await WorkspaceQueryAsync(workspaceName);
			if (workspaceArtifactIds.Count > 0)
			{
				Console.WriteLine("Deleting all Workspaces");
				foreach (int workspaceArtifactId in workspaceArtifactIds)
				{
					await DeleteSingleWorkspaceAsync(workspaceArtifactId);
				}
				Console.WriteLine("Deleted all Workspaces!");
			}
		}

		public async Task DeleteSingleWorkspaceAsync(int workspaceArtifactId)
		{
			Console.WriteLine("Deleting Workspace");

			try
			{
				string url = $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/{workspaceArtifactId}";
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
				HttpResponseMessage response = await RestHelper.MakeDeleteAsync(httpClient, url);
				if (!response.IsSuccessStatusCode)
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Delete Workspace. [{nameof(responseContent)}: {responseContent}]");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when Deleting Workspace", ex);
			}
		}

		private async Task<int> CreateWorkspaceAsync(int templateWorkspaceArtifactId, string workspaceName, bool enableDataGrid)
		{
			Console.WriteLine("Creating new Workspace");
			try
			{
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
				int statusID = (await GetEligibleStatusesAsync(httpClient)).First();
				int matterID = (await QueryEligibleMattersAsync(httpClient)).First();
				string defaultDownloadHandlerUrl = await GetDefaultDownloadHandlerUrlAsync(httpClient);
				int resourcePoolID = (await GetEligibleResourcePoolsAsync(httpClient)).First();
				int fileRepositoryID = (await GetEligibleFileRepositoriesAsync(httpClient, resourcePoolID)).First();
				int cacheLocationID = (await GetEligibleCacheLocationsAsync(httpClient, resourcePoolID)).First();
				int sqlServerID = (await GetEligibleSqlServersAsync(httpClient, resourcePoolID)).First();
				int sqlFullTextLanguage = await GetDefaultSqlFullTextLanguageAsync(httpClient);
				int templateID = (await QueryEligibleTemplatesAsync(httpClient)).First();

				// Wait for Template workspace to finish upgrading
				await WaitForTemplateWorkspaceToFinishUpgradingAsync(templateID);

				var workspaceArtifactId = await ExecuteCreateWorkspaceAsync(workspaceName, statusID, matterID, defaultDownloadHandlerUrl, resourcePoolID, fileRepositoryID, cacheLocationID, sqlServerID, sqlFullTextLanguage, templateID, httpClient);

				Console.WriteLine($"Workspace ArtifactId: {workspaceArtifactId}");
				Console.WriteLine("Created new Workspace!");

				if (enableDataGrid)
				{
					//Update Workspace to be Data Grid Enabled
					await EnableDataGridOnWorkspaceAsync(workspaceName, statusID, matterID, defaultDownloadHandlerUrl, resourcePoolID, fileRepositoryID, cacheLocationID, sqlServerID, sqlFullTextLanguage, httpClient, workspaceArtifactId);

					//Enable Data Grid on Extracted Text field
					SqlHelper.EnableDataGridOnExtractedText(Constants.Connection.Sql.EDDS_DATABASE, workspaceName);
				}

				return workspaceArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when creating Workspace", ex);
			}
		}

		private async Task<int> ExecuteCreateWorkspaceAsync(string workspaceName, int statusID, int matterID,
			string defaultDownloadHandlerUrl, int resourcePoolID, int fileRepositoryID, int cacheLocationID, int sqlServerID,
			int sqlFullTextLanguage, int templateID, HttpClient httpClient)
		{
			var createPayloadObject = new
			{
				workspaceRequest = new
				{
					Name = workspaceName,
					Status = new { ArtifactID = statusID },
					Matter = new { Secured = false, Value = new { ArtifactID = matterID } },
					DownloadHandlerUrl = defaultDownloadHandlerUrl,
					EnableDataGrid = false,
					ResourcePool = new { Secured = false, Value = new { ArtifactID = resourcePoolID } },
					DefaultFileRepository = new { Secured = false, Value = new { ArtifactID = fileRepositoryID } },
					DefaultCacheLocation = new { Secured = false, Value = new { ArtifactID = cacheLocationID } },
					SqlServer = new { Secured = false, Value = new { ArtifactID = sqlServerID } },
					SqlFullTextLanguage = sqlFullTextLanguage,
					Template = new { Secured = false, Value = new { ArtifactID = templateID } },
				}
			};

			string createPayload = JsonConvert.SerializeObject(createPayloadObject);
			HttpResponseMessage createResponse = await RestHelper.MakePostAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/", createPayload);
			if (!createResponse.IsSuccessStatusCode)
			{
				string responseContent = await createResponse.Content.ReadAsStringAsync();
				throw new Exception($"Failed to Create Workspace. [{nameof(responseContent)}: {responseContent}]");
			}

			string resultString = await createResponse.Content.ReadAsStringAsync();
			dynamic result = JObject.Parse(resultString) as JObject;
			int workspaceArtifactId = result.ArtifactID;
			return workspaceArtifactId;
		}

		private async Task EnableDataGridOnWorkspaceAsync(string workspaceName, int statusID, int matterID,
			string defaultDownloadHandlerUrl, int resourcePoolID, int fileRepositoryID, int cacheLocationID, int sqlServerID,
			int sqlFullTextLanguage, HttpClient httpClient, int workspaceArtifactId)
		{
			Console.WriteLine("Updating workspace to be Data Grid Enabled");

			var updatePayloadObject = new
			{
				workspaceRequest = new
				{
					Name = workspaceName,
					Status = new { ArtifactID = statusID },
					Matter = new { Secured = false, Value = new { ArtifactID = matterID } },
					DownloadHandlerUrl = defaultDownloadHandlerUrl,
					EnableDataGrid = true,
					ResourcePool = new { Secured = false, Value = new { ArtifactID = resourcePoolID } },
					DefaultFileRepository = new { Secured = false, Value = new { ArtifactID = fileRepositoryID } },
					DefaultCacheLocation = new { Secured = false, Value = new { ArtifactID = cacheLocationID } },
					SqlServer = new { Secured = false, Value = new { ArtifactID = sqlServerID } },
					SqlFullTextLanguage = sqlFullTextLanguage,
				}
			};

			string updatePayload = JsonConvert.SerializeObject(updatePayloadObject);
			HttpResponseMessage updateResponse = await RestHelper.MakePutAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/{workspaceArtifactId}", updatePayload);
			if (!updateResponse.IsSuccessStatusCode)
			{
				string responseContent = await updateResponse.Content.ReadAsStringAsync();
				throw new Exception($"Failed updating Workspace to Enable Data Grid. [{nameof(responseContent)}: {responseContent}]");
			}

			Console.WriteLine("Updated workspace to be Data Grid Enabled");
		}

		private async Task<List<int>> WorkspaceQueryAsync(string workspaceName)
		{
			Console.WriteLine($"Querying for Workspaces [Name: {workspaceName}]");

			try
			{
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);

				var queryPayloadObject = new
				{
					request = new
					{
						objectType = new { artifactTypeId = 8 },
						fields = new[]
						{
							new { Name = "Case Artifact ID"},
							new { Name = "Name" },
							new { Name = "Keywords" }
						},
						Condition = $"'Name' == '{workspaceName}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseContent = await queryResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Query for Workspaces. [{nameof(responseContent)}: {responseContent}]");
				}
				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				int totalCount = result.TotalCount;

				List<int> workspaceIds = new List<int>();
				for (int i = 0; i < totalCount; i++)
				{
					int workspaceId = result.Objects[i]["ArtifactID"];
					workspaceIds.Add(workspaceId);
				}
				return workspaceIds;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when querying Workspaces", ex);
			}
		}

		public async Task<int> GetWorkspaceCountQueryAsync(string workspaceName)
		{
			Console.WriteLine($"Querying for Workspace Name: {workspaceName}");

			try
			{
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);

				var queryPayloadObject = new
				{
					request = new
					{
						objectType = new { artifactTypeId = 8 },
						fields = new[]
						{
							new { Name = "Case Artifact ID"},
							new { Name = "Name" },
							new { Name = "Keywords" }
						},
						Condition = $"'Name' == '{workspaceName}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseContent = await queryResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Query for Workspaces. [{nameof(responseContent)}: {responseContent}]");
				}
				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				int totalCount = result.TotalCount;
				return totalCount;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when querying for Workspace count", ex);
			}
		}

		public async Task<int> GetFirstWorkspaceArtifactIdQueryAsync(string workspaceName)
		{
			Console.WriteLine($"Querying for Workspace Name: {workspaceName}");

			try
			{
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);

				var queryPayloadObject = new
				{
					request = new
					{
						objectType = new { artifactTypeId = 8 },
						fields = new[]
						{
							new { Name = "Case Artifact ID"},
							new { Name = "Name" },
							new { Name = "Keywords" }
						},
						Condition = $"'Name' == '{workspaceName}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseContent = await queryResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Query for Workspaces. [{nameof(responseContent)}: {responseContent}]");
				}
				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				int workspaceId = result.Objects[0]["ArtifactID"];
				return workspaceId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when querying Workspaces", ex);
			}
		}

		#region Workspace Rest Helpers

		private async Task<List<int>> GetArtifactIDListAsync(HttpClient httpClient, string url)
		{
			HttpResponseMessage response = await httpClient.GetAsync(url);
			string resultString = await response.Content.ReadAsStringAsync();

			var artifactIDs = new List<int>();

			dynamic objects = JArray.Parse(resultString) as JArray;
			foreach (var obj in objects)
			{
				int artifactID = obj.ArtifactID;
				artifactIDs.Add(artifactID);
			}

			return artifactIDs;
		}

		private async Task<List<int>> QueryArtifactIDListAsync(HttpClient httpClient, string url)
		{
			var queryRequest = new
			{
				request = new
				{
					Fields = new[]
					{
						new { Name = "*" }
					},
					Condition = ""
				},
				start = 1,
				length = 1000
			};

			StringContent payload = new StringContent(JsonConvert.SerializeObject(queryRequest), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await httpClient.PostAsync(url, payload);
			string resultString = await response.Content.ReadAsStringAsync();

			var artifactIDs = new List<int>();

			dynamic result = JObject.Parse(resultString) as JObject;
			foreach (var obj in result.Objects)
			{
				int artifactID = obj.ArtifactID;
				artifactIDs.Add(artifactID);
			}

			return artifactIDs;
		}

		public async Task<List<int>> GetEligibleStatusesAsync(HttpClient httpClient)
		{
			return await GetArtifactIDListAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/eligible-statuses");
		}

		public async Task<List<int>> GetEligibleResourcePoolsAsync(HttpClient httpClient)
		{
			return await GetArtifactIDListAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/eligible-resource-pools");
		}

		public async Task<List<int>> GetEligibleFileRepositoriesAsync(HttpClient httpClient, int resourcePoolID)
		{
			return await GetArtifactIDListAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/eligible-resource-pools/{resourcePoolID}/eligible-file-repositories");
		}

		public async Task<List<int>> GetEligibleCacheLocationsAsync(HttpClient httpClient, int resourcePoolID)
		{
			return await GetArtifactIDListAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/eligible-resource-pools/{resourcePoolID}/eligible-cache-locations");
		}

		public async Task<List<int>> GetEligibleSqlServersAsync(HttpClient httpClient, int resourcePoolID)
		{
			return await GetArtifactIDListAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/eligible-resource-pools/{resourcePoolID}/eligible-sql-servers");
		}

		public async Task<List<int>> QueryEligibleMattersAsync(HttpClient httpClient)
		{
			return await QueryArtifactIDListAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/query-eligible-matters");
		}

		public async Task<List<int>> QueryEligibleTemplatesAsync(HttpClient httpClient)
		{
			return await QueryArtifactIDListAsync(httpClient, $"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/query-eligible-templates");
		}

		public async Task<int> GetDefaultSqlFullTextLanguageAsync(HttpClient httpClient)
		{
			HttpResponseMessage response = await httpClient.GetAsync($"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/eligible-sql-full-text-languages");
			string resultString = await response.Content.ReadAsStringAsync();
			dynamic languages = JObject.Parse(resultString) as JObject;
			int defaultLanguageId = languages.DefaultLanguageLcid;
			return defaultLanguageId;
		}

		public async Task<string> GetDefaultDownloadHandlerUrlAsync(HttpClient httpClient)
		{
			HttpResponseMessage response = await httpClient.GetAsync($"{Constants.Connection.RestUrlEndpoints.WorkspaceManager.EndpointUrl}/default-download-handler-url");
			string result = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<string>(result);
		}

		public async Task WaitForTemplateWorkspaceToFinishUpgradingAsync(int templateID)
		{
			Console.WriteLine("Waiting for Template Workspace to finish upgrading");
			bool upgradeComplete = await WorkspaceUpgradeCompletedAsync(templateID);
			if (!upgradeComplete)
			{
				throw new Exception("Template workspace upgrade failed to complete");
			}
			Console.WriteLine("Template Workspace finished upgrading!");
		}

		private async Task<bool> WorkspaceUpgradeCompletedAsync(int templateId)
		{
			try
			{
				// Retry until workspace has finished upgrading
				bool isWorkspaceStillUpgrading = await RetryLogicHelper
				.RetryFunctionAsync<bool>(Constants.Waiting.WORKSPACE_HELPER_RETRY_COUNT,
				Constants.Waiting.WORKSPACE_HELPER_RETRY_DELAY,
				async () =>
				{
					bool result = SqlHelper.IsWorkspaceUpgrading(templateId);
					if (!result)
					{
						throw new Exception("Workspace still upgrading");
					}
					
					return result;
				});

				return isWorkspaceStillUpgrading;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error when checking for if workspace is still upgrading. [ErrorMessage: {ex}]", ex);
			}
		}

		#endregion
	}
}
