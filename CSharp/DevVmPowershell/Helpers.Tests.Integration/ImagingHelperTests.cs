using NUnit.Framework;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class ImagingHelperTests
	{
		private IWorkspaceHelper WorkspaceHelper { get; set; }
		private IImagingHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);
			ISqlRunner sqlRunner = new SqlRunner(connectionHelper);
			ISqlHelper sqlHelper = new SqlHelper(sqlRunner);
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, sqlHelper);
			Sut = new ImagingHelper(connectionHelper);
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
			//Arrange
			const string workspaceName = TestConstants.SAMPLE_DATA_GRID_WORKSPACE_NAME;

			//Act
			int workspaceArtifactId = await WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName);

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.ImageAllDocumentsInWorkspaceAsync(workspaceArtifactId).Wait());

			//Cleanup
			await WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName);
		}
	}
}
