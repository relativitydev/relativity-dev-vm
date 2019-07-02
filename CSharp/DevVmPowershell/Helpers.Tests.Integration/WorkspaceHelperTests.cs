using NUnit.Framework;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class WorkspaceHelperTests
	{
		private IWorkspaceHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);
			ISqlHelper sqlHelper = new SqlHelper(TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.SQL_USER_NAME, TestConstants.SQL_PASSWORD);
			Sut = new WorkspaceHelper(connectionHelper, sqlHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public async Task CreateWorkspaceAsyncTest()
		{
			//Arrange
			const string workspaceName = "ABC";
			bool enableDataGrid = false;

			//Act
			int workspaceArtifactId = await Sut.CreateWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			//Assert
			Assert.That(workspaceArtifactId, Is.GreaterThan(0));
			await Sut.DeleteWorkspaceAsync(workspaceArtifactId);
		}

		[Test]
		public async Task CreateDataGridWorkspaceAsyncTest()
		{
			//Arrange
			const string workspaceName = "ABC";
			bool enableDataGrid = true;

			//Act
			int workspaceArtifactId = await Sut.CreateWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			//Assert
			Assert.That(workspaceArtifactId, Is.GreaterThan(0));
			await Sut.DeleteWorkspaceAsync(workspaceArtifactId);
		}

		[Test]
		public void DeleteAllWorkspacesAsyncTest()
		{
			//Arrange
			const string workspaceName = "ABC";

			//Act
			//Assert
			Assert.DoesNotThrow(async () => await Sut.DeleteAllWorkspacesAsync(workspaceName)); //To Test this method, make sure the workspace(s) you are trying to delete exists
		}

		[Test]
		public void DeleteWorkspaceAsyncTest()
		{
			//Arrange
			const int workspaceArtifactId = 123;

			//Act
			//Assert
			Assert.DoesNotThrow(async () => await Sut.DeleteWorkspaceAsync(workspaceArtifactId)); //To Test this method, make sure the workspace you are trying to delete exists
		}

		[Test]
		public void GetWorkspaceIdTest()
		{
			//Arrange
			string workspaceName = TestConstants.RELATIVITY_WORKSPACE_NAME;

			//Act
			int workspaceId = Sut.GetWorkspaceId(workspaceName);

			//Assert
			Assert.That(workspaceId > 0);
		}
	}
}
