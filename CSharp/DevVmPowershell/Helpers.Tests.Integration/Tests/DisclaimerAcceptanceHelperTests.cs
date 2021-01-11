using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
			RestHelper restHelper = new RestHelper();
			Sut = new DisclaimerAcceptanceHelper(connectionHelper, restHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			SqlRunner = new SqlRunner(connectionHelper);
			SqlHelper = new SqlHelper(SqlRunner);
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, restHelper, SqlHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			RetryLogicHelper = new RetryLogicHelper();
			ApplicationInstallHelper = new ApplicationInstallHelper(connectionHelper, restHelper, WorkspaceHelper, RetryLogicHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
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
					WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceId).Wait();
				}
			}
			//Create New Workspace
			int workspaceArtifactId = WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false).Result;
			//Install Disclaimer Acceptance Log in Workspace
			bool installationSuccess = await ApplicationInstallHelper.InstallApplicationFromApplicationLibraryAsync(workspaceName, Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid);
			if (!installationSuccess)
			{
				WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId).Wait();
				throw new Exception("Failed to Install Disclaimer Acceptance Log Application in Workspace");
			}

			// Act / Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimerConfigurationAsync(workspaceName).Wait());
			Assert.IsTrue(Sut.CheckIfDisclaimerConfigurationRdoExists(workspaceArtifactId));


			//Cleanup
			WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId).Wait();
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
					WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceId).Wait();
				}
			}
			//Create New Workspace
			int workspaceArtifactId = WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false).Result;
			//Install Disclaimer Acceptance Log in Workspace
			bool installationSuccess = await ApplicationInstallHelper.InstallApplicationFromApplicationLibraryAsync(workspaceName, Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid);
			if (!installationSuccess)
			{
				WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId).Wait();
				throw new Exception("Failed to Install Disclaimer Acceptance Log Application in Workspace");
			}

			Sut.AddDisclaimerConfigurationAsync(workspaceName).Wait();

			// Act / Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimerAsync(workspaceName).Wait());
			Assert.IsTrue(Sut.CheckIfDisclaimerRdoExists(workspaceArtifactId));

			//Cleanup
			WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId).Wait();
		}
	}
}
