using Helpers.Interfaces;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers.RequestModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Implementations
{
	public class WorkspaceHelper : IWorkspaceHelper
	{
		private ServiceFactory ServiceFactory { get; }
		public ISqlHelper SqlHelper { get; set; }
		private string InstanceAddress { get; }
		private string AdminUsername { get; }
		private string AdminPassword { get; }
		private string Workspace_Rest_Url = "Relativity.REST/api/Relativity.Workspaces/workspace";

		public WorkspaceHelper(IConnectionHelper connectionHelper, ISqlHelper sqlHelper, string instanceAddress, string adminUsername, string adminPassword)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
			SqlHelper = sqlHelper;
			InstanceAddress = instanceAddress;
			AdminUsername = adminUsername;
			AdminPassword = adminPassword;
		}

		public async Task<int> CreateSingleWorkspaceAsync(string workspaceTemplateName, string workspaceName, bool enableDataGrid)
		{
			// Query for the RelativityOne Quick Start Template
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

			// Create the workspace 
			int workspaceArtifactId = await CreateWorkspaceAsync(templateWorkspaceArtifactId, workspaceName, enableDataGrid);
			return workspaceArtifactId;
		}

		public async Task DeleteAllWorkspacesAsync(string workspaceName)
		{
			List<int> workspaceArtifactIds = await WorkspaceQueryAsync(workspaceName);
			if (workspaceArtifactIds.Count > 0)
			{
				Console.WriteLine("Deleting all Workspaces");
				foreach (int workspaceArtifactId in workspaceArtifactIds)
				{
					DeleteSingleWorkspace(workspaceArtifactId);
				}
				Console.WriteLine("Deleted all Workspaces!");
			}
		}

		public void DeleteSingleWorkspace(int workspaceArtifactId)
		{
			Console.WriteLine("Deleting Workspace");

			try
			{
				string url = $"{Workspace_Rest_Url}/{workspaceArtifactId}";
				HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
				HttpResponseMessage response = RestHelper.MakeDelete(httpClient, url);
				if (!response.IsSuccessStatusCode)
				{
					throw new Exception("Failed to Delete Workspace");
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
				HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
				int statusID = (await GetEligibleStatuses(httpClient)).First();
				int matterID = (await QueryEligibleMatters(httpClient)).First();
				string defaultDownloadHandlerUrl = await GetDefaultDownloadHandlerUrl(httpClient);
				int resourcePoolID = (await GetEligibleResourcePools(httpClient)).First();
				int fileRepositoryID = (await GetEligibleFileRepositories(httpClient, resourcePoolID)).First();
				int cacheLocationID = (await GetEligibleCacheLocations(httpClient, resourcePoolID)).First();
				int sqlServerID = (await GetEligibleSqlServers(httpClient, resourcePoolID)).First();
				int sqlFullTextLanguage = await GetDefaultSqlFullTextLanguage(httpClient);
				int templateID = (await QueryEligibleTemplates(httpClient)).First();

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

				StringContent createPayload = new StringContent(JsonConvert.SerializeObject(createPayloadObject), Encoding.UTF8, "application/json");
				HttpResponseMessage createResponse = await httpClient.PostAsync($"{Workspace_Rest_Url}/", createPayload);
				if (!createResponse.IsSuccessStatusCode)
				{
					throw new Exception("Failed to Create Workspace");
				}
				string resultString = await createResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				int workspaceArtifactId = result.ArtifactID;

				Console.WriteLine($"Workspace ArtifactId: {workspaceArtifactId}");
				Console.WriteLine("Created new Workspace!");

				if (enableDataGrid)
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

					StringContent updatePayload = new StringContent(JsonConvert.SerializeObject(updatePayloadObject), Encoding.UTF8, "application/json");
					HttpResponseMessage updateResponse = await httpClient.PutAsync($"{Workspace_Rest_Url}/{workspaceArtifactId}", updatePayload);
					if (!updateResponse.IsSuccessStatusCode)
					{
						throw new Exception("Failed to Update Workspace to Enable Data Grid");
					}
					Console.WriteLine("Updated workspace to be Data Grid Enabled");
				}

				return workspaceArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when creating Workspace", ex);
			}
		}

		private async Task<List<int>> WorkspaceQueryAsync(string workspaceName)
		{
			Console.WriteLine($"Querying for Workspaces [Name: {workspaceName}]");

			try
			{
				using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = Constants.EDDS_WORKSPACE_ARTIFACT_ID;

					TextCondition textCondition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, workspaceName);
					Query<Workspace> workspaceQuery = new Query<Workspace>
					{
						Fields = FieldValue.AllFields,
						Condition = textCondition
					};

					QueryResultSet<Workspace> workspaceQueryResultSet = await Task.Run(() => rsapiClient.Repositories.Workspace.Query(workspaceQuery));

					if (!workspaceQueryResultSet.Success || workspaceQueryResultSet.Results == null)
					{
						throw new Exception("Failed to query Workspaces");
					}

					List<int> workspaceArtifactIds = workspaceQueryResultSet.Results.Select(x => x.Artifact.ArtifactID).ToList();

					Console.WriteLine($"Queried for Workspaces! [Count: {workspaceArtifactIds.Count}]");

					return workspaceArtifactIds;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when querying Workspaces", ex);
			}
		}

		public async Task<int> GetWorkspaceCountQueryAsync(string workspaceName)
		{
			Console.WriteLine($"Querying for Workspace Name: {workspaceName}");

			try
			{
				using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = Constants.EDDS_WORKSPACE_ARTIFACT_ID;

					TextCondition textCondition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, workspaceName);
					Query<Workspace> workspaceQuery = new Query<Workspace>
					{
						Fields = FieldValue.NoFields,
						Condition = textCondition
					};

					QueryResultSet<Workspace> workspaceQueryResultSet = await Task.Run(() => rsapiClient.Repositories.Workspace.Query(workspaceQuery));

					if (!workspaceQueryResultSet.Success || workspaceQueryResultSet.Results == null)
					{
						throw new Exception("Failed to query Workspaces");
					}

					List<int> workspaceArtifactIds = workspaceQueryResultSet.Results.Select(x => x.Artifact.ArtifactID).ToList();

					int workspaceCount = workspaceArtifactIds.Count;
					Console.WriteLine($"Queried for Workspaces! [Count: {workspaceCount}]");

					return workspaceCount;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when querying for Workspace count", ex);
			}
		}

		public async Task<int> GetFirstWorkspaceArtifactIdQueryAsync(string workspaceName)
		{
			Console.WriteLine($"Querying for Workspace Name: {workspaceName}");

			try
			{
				using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = Constants.EDDS_WORKSPACE_ARTIFACT_ID;

					TextCondition textCondition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, workspaceName);
					Query<Workspace> workspaceQuery = new Query<Workspace>
					{
						Fields = FieldValue.NoFields,
						Condition = textCondition
					};

					QueryResultSet<Workspace> workspaceQueryResultSet = await Task.Run(() => rsapiClient.Repositories.Workspace.Query(workspaceQuery));

					if (!workspaceQueryResultSet.Success || workspaceQueryResultSet.Results == null)
					{
						throw new Exception("Failed to query Workspaces");
					}

					List<int> workspaceArtifactIds = workspaceQueryResultSet.Results.Select(x => x.Artifact.ArtifactID).ToList();

					Console.WriteLine($"Queried for Workspaces! [Count: {workspaceArtifactIds.Count}]");

					if (workspaceArtifactIds.Count == 0)
					{
						throw new Exception($"No workspace exists! [{nameof(workspaceName)}: {workspaceName}]");
					}

					int firstWorkspaceArtifactId = workspaceArtifactIds.FirstOrDefault();
					return firstWorkspaceArtifactId;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when querying Workspaces", ex);
			}
		}

		#region Workspace Rest Helpers

		private async Task<List<int>> GetArtifactIDList(HttpClient httpClient, string url)
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

		private async Task<List<int>> QueryArtifactIDList(HttpClient httpClient, string url)
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

		public async Task<List<int>> GetEligibleStatuses(HttpClient httpClient)
		{
			return await GetArtifactIDList(httpClient, $"{Workspace_Rest_Url}/eligible-statuses");
		}

		public async Task<List<int>> GetEligibleResourcePools(HttpClient httpClient)
		{
			return await GetArtifactIDList(httpClient, $"{Workspace_Rest_Url}/eligible-resource-pools");
		}

		public async Task<List<int>> GetEligibleFileRepositories(HttpClient httpClient, int resourcePoolID)
		{
			return await GetArtifactIDList(httpClient, $"{Workspace_Rest_Url}/eligible-resource-pools/{resourcePoolID}/eligible-file-repositories");
		}

		public async Task<List<int>> GetEligibleCacheLocations(HttpClient httpClient, int resourcePoolID)
		{
			return await GetArtifactIDList(httpClient, $"{Workspace_Rest_Url}/eligible-resource-pools/{resourcePoolID}/eligible-cache-locations");
		}

		public async Task<List<int>> GetEligibleSqlServers(HttpClient httpClient, int resourcePoolID)
		{
			return await GetArtifactIDList(httpClient, $"{Workspace_Rest_Url}/eligible-resource-pools/{resourcePoolID}/eligible-sql-servers");
		}

		public async Task<List<int>> QueryEligibleMatters(HttpClient httpClient)
		{
			return await QueryArtifactIDList(httpClient, $"{Workspace_Rest_Url}/query-eligible-matters");
		}

		public async Task<List<int>> QueryEligibleTemplates(HttpClient httpClient)
		{
			return await QueryArtifactIDList(httpClient, $"{Workspace_Rest_Url}/query-eligible-templates");
		}

		public async Task<int> GetDefaultSqlFullTextLanguage(HttpClient httpClient)
		{
			HttpResponseMessage response = await httpClient.GetAsync($"{Workspace_Rest_Url}/eligible-sql-full-text-languages");
			string resultString = await response.Content.ReadAsStringAsync();
			dynamic languages = JObject.Parse(resultString) as JObject;
			int defaultLanguageID = languages.DefaultLanguageLcid;
			return defaultLanguageID;
		}

		public async Task<string> GetDefaultDownloadHandlerUrl(HttpClient httpClient)
		{
			HttpResponseMessage response = await httpClient.GetAsync($"{Workspace_Rest_Url}/default-download-handler-url");
			string result = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<string>(result);
		}

		#endregion
	}
}
