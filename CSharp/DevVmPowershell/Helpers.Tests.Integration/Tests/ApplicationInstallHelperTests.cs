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
		private IRetryLogicHelper RetryLogicHelper { get; set; }
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
			RetryLogicHelper = new RetryLogicHelper();
			Sut = new ApplicationInstallHelper(connectionHelper, restHelper, WorkspaceHelper, RetryLogicHelper, TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
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
			string workspaceName = $"{nameof(InstallApplicationFromARapFileTest)}";
			const bool enableDataGrid = false;

			try
			{
				// Arrange
				string binFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				if (string.IsNullOrWhiteSpace(binFolderPath))
				{
					throw new Exception($"{nameof(binFolderPath)} is invalid.");
				}
				string rapLocation = Path.Combine(binFolderPath, TestConstants.SAMPLE_APPLICATION_FILE_PATH);

				//Cleanup
				await WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName);

				//Create Workspace
				await WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

				// Act
				bool installationResult = await Sut.InstallApplicationFromRapFileAsync(workspaceName, rapLocation);

				// Assert
				Assert.That(installationResult, Is.True);

			}
			catch (Exception ex)
			{
				Assert.Fail("InstallApplicationFromARapFileTest Failed");
			}
			finally
			{
				//Cleanup
				await WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName);
			}
		}

		[Test]
		public async Task InstallApplicationFromTheApplicationLibraryTest()
		{
			string workspaceName = $"{nameof(InstallApplicationFromTheApplicationLibraryTest)}";
			const bool enableDataGrid = false;

			try
			{
				// Arrange
				string applicationGuid = Constants.ApplicationGuids.SimpleFileUploadGuid;

				//Cleanup
				await WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName);

				//Create Workspace
				await WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, enableDataGrid); //To Test this method, make sure the Template Workspace exists

				// Act
				bool installationResult = await Sut.InstallApplicationFromApplicationLibraryAsync(workspaceName, applicationGuid);

				// Assert
				Assert.That(installationResult, Is.True);

			}
			catch (Exception ex)
			{
				Assert.Fail("InstallApplicationFromTheApplicationLibraryTest Failed");
			}
			finally
			{
				//Cleanup
				await WorkspaceHelper.DeleteAllWorkspacesAsync(workspaceName);
			}
		}
	}
}
