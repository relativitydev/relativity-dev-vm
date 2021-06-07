using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class WorkspaceHelperTests
	{
		private IWorkspaceHelper Sut { get; set; }

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
			ISqlHelper sqlHelper = new SqlHelper(sqlRunner);
			IRestHelper restHelper = new RestHelper();
			ILogService logService = new LogService();
			IRetryLogicHelper retryLogicHelper = new RetryLogicHelper();
			Sut = new WorkspaceHelper(logService, connectionHelper, restHelper, sqlHelper, retryLogicHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test, Order(10)]
		public async Task CreateSingleWorkspaceAsyncTest()
		{
			//Arrange
			string workspaceName = $"{nameof(CreateSingleWorkspaceAsyncTest)}";
			const bool enableDataGrid = false;

			//Cleanup
			await Sut.DeleteAllWorkspacesAsync(workspaceName);

			//Act
			int workspaceArtifactId = await Sut.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			//Assert
			Assert.That(workspaceArtifactId, Is.GreaterThan(0));

			//Cleanup
			await Sut.DeleteAllWorkspacesAsync(workspaceName);
		}

		[Test, Order(20)]
		public async Task CreateDataGridWorkspaceAsyncTest()
		{
			//Arrange
			string workspaceName = $"{nameof(CreateDataGridWorkspaceAsyncTest)}";
			const bool enableDataGrid = true;

			//Cleanup
			await Sut.DeleteAllWorkspacesAsync(workspaceName);

			//Act
			int workspaceArtifactId = await Sut.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			//Assert
			Assert.That(workspaceArtifactId, Is.GreaterThan(0));
			await Sut.DeleteSingleWorkspaceAsync(workspaceArtifactId);

			//Cleanup
			await Sut.DeleteAllWorkspacesAsync(workspaceName);
		}

		[Test, Order(30)]
		public async Task DeleteAllWorkspacesAsyncTest()
		{
			//Arrange
			string workspaceName = $"{nameof(DeleteAllWorkspacesAsyncTest)}";
			const bool enableDataGrid = false;

			//Create the workspace to delete
			await Sut.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			//Act
			await Sut.DeleteAllWorkspacesAsync(workspaceName);

			//Assert
			int workspaceCount = await Sut.GetWorkspaceCountQueryAsync(workspaceName);
			Assert.That(workspaceCount, Is.EqualTo(0));
		}

		[Test, Order(40)]
		public async Task DeleteSingleWorkspaceAsyncTest()
		{
			//Arrange
			string workspaceName = $"{nameof(DeleteSingleWorkspaceAsyncTest)}";
			const bool enableDataGrid = false;

			//Cleanup
			await Sut.DeleteAllWorkspacesAsync(workspaceName);

			int workspaceArtifactId = await Sut.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			//Act
			await Sut.DeleteSingleWorkspaceAsync(workspaceArtifactId);

			//Assert
			int workspaceCount = await Sut.GetWorkspaceCountQueryAsync(workspaceName);
			Assert.That(workspaceCount, Is.EqualTo(0));
		}

		[Test, Order(50)]
		public async Task GetWorkspaceArtifactIdTest()
		{
			//Arrange
			string workspaceName = $"{nameof(DeleteSingleWorkspaceAsyncTest)}";
			const bool enableDataGrid = false;

			//Cleanup
			await Sut.DeleteAllWorkspacesAsync(workspaceName);

			int workspaceArtifactId = await Sut.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			//Act
			int result = await Sut.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName);

			//Assert
			Assert.That(workspaceArtifactId > 0);
			Assert.AreEqual(workspaceArtifactId, result);

			//Cleanup
			await Sut.DeleteAllWorkspacesAsync(workspaceName);
		}
	}
}
