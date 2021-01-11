using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class ApplicationInstallHelperTests
	{
		private IWorkspaceHelper WorkspaceHelper { get; set; }
		private IApplicationInstallHelper Sut { get; set; }

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
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, restHelper, sqlHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
			Sut = new ApplicationInstallHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			WorkspaceHelper = null;
			Sut = null;
		}

		[Test]
		public async Task InstallApplicationFromARapFileTest()
		{
			// Arrange
			string binFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			if (string.IsNullOrWhiteSpace(binFolderPath))
			{
				throw new Exception($"{nameof(binFolderPath)} is invalid.");
			}
			string rapLocation = Path.Combine(binFolderPath, TestConstants.SAMPLE_APPLICATION_FILE_PATH);
			string workspaceName = $"{nameof(InstallApplicationFromARapFileTest)}";
			const bool enableDataGrid = false;

			//Cleanup
			await WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName);

			//Create Workspace
			await WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

			// Act
			bool installationResult = Sut.InstallApplicationFromRapFile(workspaceName, rapLocation);

			// Assert
			Assert.That(installationResult, Is.True);

			//Cleanup
			await WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName);
		}

		[Test]
		public void InstallApplicationFromTheApplicationLibraryTest()
		{
			// Arrange
			string applicationGuid = "0125C8D4-8354-4D8F-B031-01E73C866C7C"; // Guid of the Smoke Test Application

			// Act
			bool installationResult = Sut.InstallApplicationFromApplicationLibrary(TestConstants.SAMPLE_DATA_GRID_WORKSPACE_NAME, applicationGuid);

			// Assert
			Assert.That(installationResult, Is.True);
		}
	}
}
