using System;
using System.Collections.Generic;
using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class DisclaimerAcceptanceHelperTests
	{
		private IDisclaimerAcceptanceHelper Sut { get; set; }
		private ISqlRunner SqlRunner { get; set; }
		private ISqlHelper SqlHelper { get; set; }
		private IWorkspaceHelper WorkspaceHelper { get; set; }
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

			Sut = new DisclaimerAcceptanceHelper(connectionHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			SqlRunner = new SqlRunner(connectionHelper);
			SqlHelper = new SqlHelper(SqlRunner);
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, SqlHelper);
			ApplicationInstallHelper = new ApplicationInstallHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void AddDisclaimerConfigurationTest()
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
			bool installationSuccess = ApplicationInstallHelper.InstallApplicationFromApplicationLibrary(workspaceName, Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid);
			if (!installationSuccess)
			{
				WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
				throw new Exception("Failed to Install Disclaimer Acceptance Log Application in Workspace");
			}

			// Act / Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimerConfiguration(workspaceName));
			Assert.IsTrue(Sut.CheckIfDisclaimerConfigurationRDOExists(workspaceArtifactId));


			//Cleanup
			WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
		}

		[Test]
		public void AddDisclaimerTest()
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
			bool installationSuccess = ApplicationInstallHelper.InstallApplicationFromApplicationLibrary(workspaceName, Constants.DisclaimerAcceptance.ApplicationGuids.ApplicationGuid);
			if (!installationSuccess)
			{
				WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
				throw new Exception("Failed to Install Disclaimer Acceptance Log Application in Workspace");
			}

			Sut.AddDisclaimerConfiguration(workspaceName);

			// Act / Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimer(workspaceName));
			Assert.IsTrue(Sut.CheckIfDisclaimerRDOExists(workspaceArtifactId));

			//Cleanup
			WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
		}
	}
}
