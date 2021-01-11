using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using Relativity.Services.Agent;

namespace Helpers.Tests.Integration.Tests
{
	public class SmokeTestHelperTests
	{
		private ISqlHelper SqlHelper { get; set; }
		private IWorkspaceHelper WorkspaceHelper { get; set; }
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
			RestHelper restHelper = new RestHelper();
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, restHelper, SqlHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			ApplicationInstallHelper = new ApplicationInstallHelper(connectionHelper);
			AgentHelper = new AgentHelper(connectionHelper, restHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			ImportApiHelper = new ImportApiHelper(connectionHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			Sut = new SmokeTestHelper(connectionHelper);
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
		public void WaitForSmokeTestToCompleteAsyncTest()
		{
			int workspaceArtifactId = 0;
			const string workspaceName = "Smoke Test Helper Workspace";
			const string applicationName = "Smoke Test";
			try
			{
				//Arrange

				//Delete Workspace with Disclaimer Acceptance Installed
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
				bool installationResult =
					ApplicationInstallHelper.InstallApplicationFromApplicationLibrary(workspaceName,
						Constants.SmokeTest.Guids.ApplicationGuid);
				if (!installationResult)
				{
					throw new Exception("Smoke Test Application failed to Install");
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
				bool result = Sut.WaitForSmokeTestToComplete(workspaceName, 10);

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
