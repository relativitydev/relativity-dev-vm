using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;

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

			Sut = new ImportApiHelper(connectionHelper);
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, null);

		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
			WorkspaceHelper = null;
		}

		[Test]
		[TestCase(TestConstants.SAMPLE_DATA_GRID_WORKSPACE_NAME, Constants.FileType.Document, 15)]
		[TestCase(TestConstants.SAMPLE_DATA_GRID_WORKSPACE_NAME, Constants.FileType.Image, 15)]
		public void/*async Task*/ AddDocumentsToWorkspaceTest(string workspaceName, string fileType, int numberOfFiles)
		{
			//Arrange

			//Act
			int numberOfFilesImported = Sut.AddDocumentsToWorkspace(WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName).Result, fileType, numberOfFiles, "").Result;

			//Assert
			Assert.That(numberOfFilesImported, Is.EqualTo(numberOfFiles));
		}
	}
}
