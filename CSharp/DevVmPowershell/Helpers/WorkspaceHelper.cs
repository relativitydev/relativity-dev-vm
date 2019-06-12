using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers
{
	public class WorkspaceHelper : IWorkspaceHelper
	{
		private ServiceFactory ServiceFactory { get; }

		public WorkspaceHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		public async Task<int> CreateWorkspaceAsync(string workspaceTemplateName, string workspaceName, bool enableDataGrid)
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
					await DeleteWorkspaceAsync(workspaceArtifactId);
				}
				Console.WriteLine("Deleted all Workspaces!");
			}
		}

		public async Task DeleteWorkspaceAsync(int workspaceArtifactId)
		{
			Console.WriteLine("Deleting Workspace");

			try
			{
				using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
				{
					await Task.Run(() => rsapiClient.Repositories.Workspace.DeleteSingle(workspaceArtifactId));

					Console.WriteLine("Deleted Workspace!");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when deleting Workspace", ex);
			}
		}

		private async Task<int> CreateWorkspaceAsync(int templateWorkspaceArtifactId, string workspaceName, bool enableDataGrid)
		{
			Console.WriteLine("Creating new Workspace");

			try
			{
				const string workspaceCreationFailErrorMessage = "Failed to create new workspace";

				using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = Constants.EDDS_WORKSPACE_ARTIFACT_ID;

					//Create the workspace object and apply any desired properties.
					Workspace newWorkspace = enableDataGrid
						? CreateDataGridWorkspaceDto(workspaceName)
						: CreateNonDataGridWorkspaceDto(workspaceName);

					ProcessOperationResult processOperationResult = await Task.Run(() => rsapiClient.Repositories.Workspace.CreateAsync(templateWorkspaceArtifactId, newWorkspace));

					if (!processOperationResult.Success)
					{
						throw new Exception(workspaceCreationFailErrorMessage);
					}

					ProcessInformation processInformation = await Task.Run(() => rsapiClient.GetProcessState(rsapiClient.APIOptions, processOperationResult.ProcessID));

					const int maxTimeInMilliseconds = Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000;
					const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
					int currentWaitTimeInMilliseconds = 0;

					while ((currentWaitTimeInMilliseconds < maxTimeInMilliseconds) && (processInformation.State != ProcessStateValue.Completed))
					{
						Thread.Sleep(sleepTimeInMilliSeconds);

						processInformation = await Task.Run(() => rsapiClient.GetProcessState(rsapiClient.APIOptions, processOperationResult.ProcessID));

						currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
					}

					int? workspaceArtifactId = processInformation.OperationArtifactIDs.FirstOrDefault();
					if (workspaceArtifactId == null)
					{
						throw new Exception(workspaceCreationFailErrorMessage);
					}

					Console.WriteLine($"Workspace ArtifactId: {workspaceArtifactId.Value}");
					Console.WriteLine("Created new Workspace!");

					return workspaceArtifactId.Value;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Workspace", ex);
			}
		}

		private static Workspace CreateNonDataGridWorkspaceDto(string workspaceName)
		{
			Workspace newWorkspace = new Workspace
			{
				Name = workspaceName,
				Accessible = true
			};
			return newWorkspace;
		}

		private static Workspace CreateDataGridWorkspaceDto(string workspaceName)
		{
			Workspace newWorkspace = new Workspace
			{
				Name = workspaceName,
				Accessible = true,
				EnableDataGrid = true,
				DefaultDataGridLocation = "
				//DatabaseLocation = Constants.Workspace.DATABASE_LOCATION
			};
			return newWorkspace;
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
	}
}
