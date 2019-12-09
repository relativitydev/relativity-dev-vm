﻿using System;
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
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, SqlHelper);
			ApplicationInstallHelper = new ApplicationInstallHelper(connectionHelper);
			AgentHelper = new AgentHelper(connectionHelper);
			ImportApiHelper = new ImportApiHelper(connectionHelper);
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
			try
			{
				//Arrange
				const string workspaceName = "Smoke Test Helper Workspace";
				const string applicationName = "Smoke Test";

				//Delete Workspace with Disclaimer Acceptance Installed
				List<int> workspacesWhereApplicationIsInstalled = SqlHelper.RetrieveWorkspacesWhereApplicationIsInstalled(new Guid(Constants.SmokeTest.Guids.ApplicationGuid));
				if (workspacesWhereApplicationIsInstalled.Count > 0)
				{
					foreach (int workspaceId in workspacesWhereApplicationIsInstalled)
					{
						WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceId).Wait();
					}
				}

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

				//Delete Smoke Test Agents if they exist
				AgentHelper.DeleteAgentsInRelativityApplicationAsync(applicationName).Wait();

				Thread.Sleep(15000);

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
				bool result = Sut.WaitForSmokeTestToComplete(workspaceName);

				//Assert
				Assert.IsTrue(result);
			}
			finally
			{
				//Cleanup
				WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId).Wait();
			}
		}
	}
}
