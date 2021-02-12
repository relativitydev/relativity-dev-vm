using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration.Tests
{
	public class SmokeTestHelperTests
	{
		private ISqlHelper SqlHelper { get; set; }
		private IWorkspaceHelper WorkspaceHelper { get; set; }
		private IRetryLogicHelper RetryLogicHelper { get; set; }
		private IApplicationInstallHelper ApplicationInstallHelper { get; set; }
		private IAgentHelper AgentHelper { get; set; }
		private IImportApiHelper ImportApiHelper { get; set; }
		private ISmokeTestHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);
			ISqlRunner sqlRunner = new SqlRunner(connectionHelper);
			SqlHelper = new SqlHelper(sqlRunner);
			IRestHelper restHelper = new RestHelper();
			RetryLogicHelper = new RetryLogicHelper();
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, restHelper, SqlHelper);
			ApplicationInstallHelper = new ApplicationInstallHelper(connectionHelper, restHelper, WorkspaceHelper, RetryLogicHelper);
			AgentHelper = new AgentHelper(connectionHelper, restHelper);
			ImportApiHelper = new ImportApiHelper(connectionHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			Sut = new SmokeTestHelper(connectionHelper, restHelper, RetryLogicHelper, WorkspaceHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
			WorkspaceHelper = null;
			ApplicationInstallHelper = null;
			AgentHelper = null;
			ImportApiHelper = null;
		}

		[Test]
		public async Task WaitForSmokeTestToCompleteAsyncTest()
		{
			int workspaceArtifactId = 0;
			const string workspaceName = "Smoke Test Helper Workspace";
			const string applicationName = "Smoke Test";
			try
			{
				//Arrange

				//Delete Workspace with Smoke Test Installed
				List<int> workspacesWhereApplicationIsInstalled = SqlHelper.RetrieveWorkspacesWhereApplicationIsInstalled(new Guid(Constants.SmokeTest.Guids.ApplicationGuid));
				if (workspacesWhereApplicationIsInstalled.Count > 0)
				{
					foreach (int workspaceId in workspacesWhereApplicationIsInstalled)
					{
						WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceId).Wait();
					}
				}

				//Delete Smoke Test Agents if they exist
				AgentHelper.DeleteAgentsInRelativityApplicationAsync(applicationName).Wait();

				//Create Workspace
				workspaceArtifactId = WorkspaceHelper
					.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, true).Result;

				//Install Smoke Test Application in Workspace
				string binFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				if (string.IsNullOrWhiteSpace(binFolderPath))
				{
					throw new Exception($"{nameof(binFolderPath)} is invalid.");
				}
				string rapLocation = Path.Combine(binFolderPath, TestConstants.SMOKE_TEST_APP_FILE_PATH);
				bool installationResult = await ApplicationInstallHelper.InstallApplicationFromRapFileAsync(workspaceName, rapLocation);
				if (!installationResult)
				{
					throw new Exception($"Smoke Test Application failed to Install in a workspace ({workspaceName})");
				}

				//Install Processing Application in Workspace
				bool processingInstallationResult = await ApplicationInstallHelper.InstallApplicationFromApplicationLibraryAsync(workspaceName, Constants.Processing.Guid);
				if (!processingInstallationResult)
				{
					throw new Exception($"Processing Application failed to Install in a workspace ({workspaceName})");
				}

				//Create Smoke Test Agents
				bool smokeTestRunnerAgentCreated =
					AgentHelper.AddAgentToRelativityByNameAsync("Smoke Test Runner Agent").Result;
				bool smokeTestAnalysisAgentCreated =
					AgentHelper.AddAgentToRelativityByNameAsync("Smoke Test Analysis Agent").Result;
				if (!smokeTestRunnerAgentCreated && !smokeTestAnalysisAgentCreated)
				{
					throw new Exception("Failed to Create Smoke Test Agents");
				}

				//Import Imaged Documents
				int numberOfImagedDocuments =
					ImportApiHelper.AddDocumentsToWorkspace(workspaceArtifactId, "image", 100, "").Result;
				if (numberOfImagedDocuments <= 0)
				{
					throw new Exception("Failed to Import Imaged Documents to the workspace");
				}

				//Act
				bool result = await Sut.WaitForSmokeTestToCompleteAsync(workspaceName, Constants.Waiting.SMOKE_TEST_HELPER_MAX_WAIT_TIME_IN_MINUTES);

				//Assert
				Assert.IsTrue(result);
			}
			finally
			{
				//Cleanup

				//Delete Smoke Test Agents if they exist
				AgentHelper.DeleteAgentsInRelativityApplicationAsync(applicationName).Wait();

				//Delete Workspace
				WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId).Wait();
			}
		}
	}
}
