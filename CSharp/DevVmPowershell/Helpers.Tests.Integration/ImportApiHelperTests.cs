using NUnit.Framework;

namespace Helpers.Tests.Integration
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
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);

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
		[TestCase(TestConstants.RELATIVITY_WORKSPACE_NAME, Constants.FileType.Document, 15)]
		[TestCase(TestConstants.RELATIVITY_WORKSPACE_NAME, Constants.FileType.Image, 15)]
		public void/*async Task*/ AddDocumentsToWorkspaceTest(string workspaceName, string fileType, int numberOfFiles)
		{
			//Arrange

			//Act
			int numberOfFilesImported = Sut.AddDocumentsToWorkspace(WorkspaceHelper.GetFirstWorkspaceIdQueryAsync(workspaceName).Result, fileType, numberOfFiles, "").Result;

			//Assert
			Assert.That(numberOfFilesImported, Is.EqualTo(numberOfFiles));
		}
	}
}
