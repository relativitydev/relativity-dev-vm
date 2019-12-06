using System;
using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using Renci.SshNet;
using Exception = kCura.Notification.Exception;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class SqlHelperTests
	{
		private ISqlHelper Sut { get; set; }
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
			ISqlRunner sqlRunner = new SqlRunner(connectionHelper);
			Sut = new SqlHelper(sqlRunner);
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, Sut);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void DeleteAllErrorsTest()
		{
			// Act
			bool result = Sut.DeleteAllErrors(Constants.Connection.Sql.EDDS_DATABASE);
			int errorsCount = Sut.GetErrorsCount(Constants.Connection.Sql.EDDS_DATABASE);

			// Assert
			Assert.True(errorsCount == 0);
		}

		[Test]
		public void GetFileShareResourceServerArtifactIdTest()
		{
			// Act
			int fileShareResourceServerArtifactId = Sut.GetFileShareResourceServerArtifactId(Constants.Connection.Sql.EDDS_DATABASE);

			// Assert
			Assert.That(fileShareResourceServerArtifactId, Is.GreaterThan(0));
		}

		[Test]
		public void EnableDataGridOnExtractedTextTest()
		{
			var workspaceName = "EnableDataGridOnExtractedTextTest";
			var workspaceId = 0;
			try
			{
				// Arrange
				workspaceId = WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, true).Result;

				// Act
				// Assert
				Assert.DoesNotThrow(() => Sut.EnableDataGridOnExtractedText(Constants.Connection.Sql.EDDS_DATABASE, workspaceName));
			}
			finally
			{
				WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceId);
			}

		}

		[Test]
		public void CreateOrAlterShrinkDbProcTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.That(Sut.CreateOrAlterShrinkDbProc(Constants.Connection.Sql.EDDS_DATABASE), Is.True);
		}

		[Test]
		public void RunShrinkDbProcTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.That(Sut.RunShrinkDbProc(Constants.Connection.Sql.EDDS_DATABASE), Is.True);
		}

		[Test]
		public void InsertRsmfViewerOverrideTest()
		{
			// Arrange

			// Act

			// Assert
			Assert.DoesNotThrow(() => Sut.InsertRsmfViewerOverride(Constants.Connection.Sql.EDDS_DATABASE));
		}
	}
}
