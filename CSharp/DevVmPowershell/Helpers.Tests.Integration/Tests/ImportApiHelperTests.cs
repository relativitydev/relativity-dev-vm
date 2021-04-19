using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class ImportApiHelperTests
	{
		private IImportApiHelper Sut { get; set; }
		private IWorkspaceHelper WorkspaceHelper { get; set; }

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
			Sut = new ImportApiHelper(connectionHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			ILogService logService = new LogService();
			WorkspaceHelper = new WorkspaceHelper(logService, connectionHelper, restHelper, null);

		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
			WorkspaceHelper = null;
		}

		[Test]
		[TestCase(Constants.FileType.Document, 15)]
		[TestCase(Constants.FileType.Image, 15)]
		public void AddDocumentsToWorkspaceTest(string fileType, int numberOfFiles)
		{
			//Arrange
			string workspaceName = "ImportApi Test Workspace";
			CleanupWorkspaceIfItExists(workspaceName);

			int workspaceArtifactId = WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false).Result;

			//Act
			int numberOfFilesImported = Sut.AddDocumentsToWorkspace(WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName).Result, fileType, numberOfFiles, "").Result;

			//Assert
			Assert.That(numberOfFilesImported, Is.EqualTo(numberOfFiles));

			//Cleanup
			WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId).Wait();
		}

		private void CleanupWorkspaceIfItExists(string workspaceName)
		{
			try
			{
				WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName).Wait();
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when cleaning up workspaces (if they exist)", ex);
			}
		}
	}
}
