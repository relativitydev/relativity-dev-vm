using System;
using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class ImagingHelperTests
	{
		private IWorkspaceHelper WorkspaceHelper { get; set; }
		private IImagingHelper Sut { get; set; }
		private IImportApiHelper ImportApiHelper { get; set; }

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
			ISqlRunner sqlRunner = new SqlRunner(connectionHelper);
			ISqlHelper sqlHelper = new SqlHelper(sqlRunner);
			IRetryLogicHelper retryLogicHelper = new RetryLogicHelper();
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, restHelper, sqlHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			Sut = new ImagingHelper(connectionHelper, restHelper, retryLogicHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			ImportApiHelper = new ImportApiHelper(connectionHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public async Task ImagingTest()
		{
			// Arrange
			const string workspaceName = "Imaging Test Workspace";
			CleanupWorkspaceIfItExists(workspaceName);

			//Create Workspace
			int workspaceArtifactId = await WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false);

			//Import Documents
			int numberImported = await ImportApiHelper.AddDocumentsToWorkspace(workspaceArtifactId, "document", 100, "");
			if (numberImported == 0)
			{
				await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
				throw new Exception("Failed to Import Documents to the Workspace");
			}

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.ImageAllDocumentsInWorkspaceAsync(workspaceArtifactId).Wait());
			Assert.IsTrue(Sut.CheckThatAllDocumentsInWorkspaceAreImaged(workspaceArtifactId).Result);

			//Cleanup
			await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
		}

		private void CleanupWorkspaceIfItExists(string workspaceName)
		{
			try
			{
				WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName).Wait();
			}
			catch (Exception ex)
			{
				//Workspace Does Not Exist
			}
		}
	}
}
