﻿using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class DisclaimerAcceptanceHelperTests
	{
		private IDisclaimerAcceptanceHelper Sut { get; set; }
		private ISqlRunner SqlRunner { get; set; }
		private ISqlHelper SqlHelper { get; set; }
		private IWorkspaceHelper WorkspaceHelper { get; set; }
		private IRetryLogicHelper RetryLogicHelper { get; set; }
		private IApplicationInstallHelper ApplicationInstallHelper { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);
			IRestHelper restHelper = new RestHelper();
			SqlRunner = new SqlRunner(connectionHelper);
			SqlHelper = new SqlHelper(SqlRunner);
			ILogService logService = new LogService();
			WorkspaceHelper = new WorkspaceHelper(logService, connectionHelper, restHelper, SqlHelper);
			RetryLogicHelper = new RetryLogicHelper();
			ApplicationInstallHelper = new ApplicationInstallHelper(connectionHelper, restHelper, WorkspaceHelper, RetryLogicHelper);
			Sut = new DisclaimerAcceptanceHelper(connectionHelper, restHelper, WorkspaceHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public async Task AddDisclaimerConfigurationTest()
		{
			// Arrange
			string workspaceName = "Disclaimer Test Workspace";

			//Delete Workspace with Disclaimer Acceptance Installed
			List<int> workspacesWhereApplicationIsInstalled = SqlHelper.RetrieveWorkspacesWhereApplicationIsInstalled(new Guid(Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid));
			if (workspacesWhereApplicationIsInstalled.Count > 0)
			{
				foreach (int workspaceId in workspacesWhereApplicationIsInstalled)
				{
					await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceId);
				}
			}
			//Create New Workspace
			int workspaceArtifactId = await WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false);
			//Install Disclaimer Acceptance Log in Workspace
			bool installationSuccess = await ApplicationInstallHelper.InstallApplicationFromApplicationLibraryAsync(workspaceName, Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid);
			if (!installationSuccess)
			{
				await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
				throw new Exception("Failed to Install Disclaimer Acceptance Log Application in Workspace");
			}

			Thread.Sleep(60000); // Sleep to give time for Application Objects to be Created

			// Act / Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimerConfigurationAsync(workspaceName).Wait());
			Assert.IsTrue(Sut.CheckIfDisclaimerConfigurationRdoExistsAsync(workspaceArtifactId).Result);

			//Cleanup
			await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
		}

		[Test]
		public async Task AddDisclaimerTest()
		{
			// Arrange
			string workspaceName = "Disclaimer Test Workspace";

			//Delete Workspace with Disclaimer Acceptance Installed
			List<int> workspacesWhereApplicationIsInstalled = SqlHelper.RetrieveWorkspacesWhereApplicationIsInstalled(new Guid(Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid));
			if (workspacesWhereApplicationIsInstalled.Count > 0)
			{
				foreach (int workspaceId in workspacesWhereApplicationIsInstalled)
				{
					await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceId);
				}
			}
			//Create New Workspace
			int workspaceArtifactId = await WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false);
			//Install Disclaimer Acceptance Log in Workspace
			bool installationSuccess = await ApplicationInstallHelper.InstallApplicationFromApplicationLibraryAsync(workspaceName, Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid);
			if (!installationSuccess)
			{
				await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
				throw new Exception("Failed to Install Disclaimer Acceptance Log Application in Workspace");
			}

			Thread.Sleep(60000); // Sleep to give time for Application Objects to be Created

			await Sut.AddDisclaimerConfigurationAsync(workspaceName);

			// Act / Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimerAsync(workspaceName).Wait());
			Assert.IsTrue(Sut.CheckIfDisclaimerRdoExistsAsync(workspaceArtifactId).Result);

			//Cleanup
			await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
		}
	}
}
