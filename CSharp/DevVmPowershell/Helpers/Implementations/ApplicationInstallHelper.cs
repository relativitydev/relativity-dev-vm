﻿using Helpers.Interfaces;
using Newtonsoft.Json;
using Relativity.Services.Exceptions;
using Relativity.Services.Interfaces.LibraryApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class ApplicationInstallHelper : IApplicationInstallHelper
	{
		private IConnectionHelper ConnectionHelper { get; }
		private IWorkspaceHelper WorkspaceHelper { get; }
		private IRestHelper RestHelper { get; }
		private IRetryLogicHelper RetryLogicHelper { get; }

		public ApplicationInstallHelper(IConnectionHelper connectionHelper, IRestHelper restHelper, IWorkspaceHelper workspaceHelper, IRetryLogicHelper retryLogicHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
			WorkspaceHelper = workspaceHelper;
			RetryLogicHelper = retryLogicHelper;
		}


		public async Task<bool> InstallApplicationFromRapFileAsync(string workspaceName, string filePath)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);

			// Need to install to library before we install to a workspace
			HttpResponseMessage updateResponseMessage = await UploadLibraryApplicationAsync(httpClient, filePath);

			if (updateResponseMessage.IsSuccessStatusCode)
			{
				Console.WriteLine("Successfully installed application to the library.");
			}
			else
			{
				string responseContent = await updateResponseMessage.Content.ReadAsStringAsync();
				Console.WriteLine($"Failed to install application to the library. [{nameof(responseContent)}: {responseContent}]");
				return false;
			}

			string updateContent = updateResponseMessage.Content.ReadAsStringAsync().Result;
			dynamic updateResult = JsonConvert.DeserializeObject<dynamic>(updateContent);
			string applicationGuid = updateResult.ApplicationIdentifier.Guids.First.Value;

			// Retry until we get the applicationId from the library
			int applicationId = RetryLogicHelper
				.RetryFunction<int>(Constants.Connection.RestUrlEndpoints.ApplicationInstall.retryCount,
					Constants.Connection.RestUrlEndpoints.ApplicationInstall.retryDelay,
					() => GetApplicationLibraryIdAsync(applicationGuid).Result);

			// Now install the application to the workspace
			bool wasInstalledOnWorkspace = await InstallApplicationOnWorkspaceAsync(httpClient, workspaceName, applicationId);
			if (wasInstalledOnWorkspace)
			{
				Console.WriteLine("Successfully installed application into workspace.");
				return true;
			}
			else
			{
				Console.WriteLine("Failed to install application into workspace.");
				return false;
			}
		}

		public async Task<bool> InstallApplicationFromApplicationLibraryAsync(string workspaceName, string applicationGuid)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);

			int applicationId = GetApplicationLibraryIdAsync(applicationGuid).Result;

			// Now install the application to the workspace
			bool wasInstalledOnWorkspace = await InstallApplicationOnWorkspaceAsync(httpClient, workspaceName, applicationId);
			if (wasInstalledOnWorkspace)
			{
				Console.WriteLine("Successfully installed application into workspace.");
				return true;
			}
			else
			{
				Console.WriteLine("Failed to install application into workspace.");
				return false;
			}
		}

		private async Task<List<LibraryApplicationResponse>> ReadAllLibraryApplicationAsync()
		{
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			HttpResponseMessage httpResponse = await RestHelper.MakeGetAsync(httpClient, Constants.Connection.RestUrlEndpoints.ApplicationInstall.readAllLibraryApplicationUrl);

			string content = await httpResponse.Content.ReadAsStringAsync();
			List<LibraryApplicationResponse> response = JsonConvert.DeserializeObject<List<LibraryApplicationResponse>>(content);

			return response;
		}

		private async Task<bool> DoesLibraryApplicationExistAsync(string applicationGuid)
		{
			List<LibraryApplicationResponse> allApps = await ReadAllLibraryApplicationAsync();

			return allApps.Exists(x => x.Guids.Contains(new Guid(applicationGuid)));
		}

		private async Task<int> GetApplicationLibraryIdAsync(string applicationGuid)
		{
			if (!await DoesLibraryApplicationExistAsync(applicationGuid))
			{
				throw new ValidationException("Library application does not exist");
			}

			List<LibraryApplicationResponse> allApps = await ReadAllLibraryApplicationAsync();
			return allApps.Find(x => x.Guids.Contains(new Guid(applicationGuid))).ArtifactID;
		}

		private async Task<HttpResponseMessage> UploadLibraryApplicationAsync(HttpClient httpClient, string filePath)
		{
			string fullFileName = Path.GetFileName(filePath);
			FileStream fileStream = File.OpenRead(filePath);

			var tempUpdateJsonRequest = new
			{
				request = new { IgnoreVersion = false, CreateIfMissing = true, RefreshCustomPages = true, FileName = fullFileName }
			};
			string updateJsonRequest = JsonConvert.SerializeObject(tempUpdateJsonRequest);

			MultipartFormDataContent form = new MultipartFormDataContent();
			string contentType = "application/octet-stream";
			form.Add(RestHelper.CreateFileContent(fileStream, fullFileName, contentType), Constants.Connection.RestUrlEndpoints.ApplicationInstall.uploadFileKeyName, fullFileName);
			form.Headers.Add("X-CSRF-Header", "-");
			form.Add(new StringContent(updateJsonRequest, Encoding.Unicode, "application/json"), "request");


			HttpResponseMessage updateResponse = await RestHelper.MakePutAsync(httpClient, Constants.Connection.RestUrlEndpoints.ApplicationInstall.updateLibraryApplicationUrl, form);
			return updateResponse;
		}

		private async Task<bool> InstallApplicationOnWorkspaceAsync(HttpClient httpClient, string workspaceName, int applicationId)
		{
			int workspaceId = WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName).Result;
			var tempInstallJsonRequest = new
			{
				request = new { WorkspaceIDs = new[] { workspaceId }, UnlockAllApplications = false }
			};
			string installJsonRequest = JsonConvert.SerializeObject(tempInstallJsonRequest);

			string installEndPoint = string.Format(Constants.Connection.RestUrlEndpoints.ApplicationInstall.installWorkspaceApplicationUrl, applicationId);
			HttpResponseMessage installResponse = await RestHelper.MakePostAsync(httpClient, installEndPoint, installJsonRequest);
			return installResponse.IsSuccessStatusCode;
		}
	}
}
