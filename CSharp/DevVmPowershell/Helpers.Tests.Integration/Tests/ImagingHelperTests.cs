﻿using System;
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
			ISqlRunner sqlRunner = new SqlRunner(connectionHelper);
			ISqlHelper sqlHelper = new SqlHelper(sqlRunner);
			WorkspaceHelper = new WorkspaceHelper(connectionHelper, sqlHelper);
			Sut = new ImagingHelper(connectionHelper);
			ImportApiHelper = new ImportApiHelper(connectionHelper);
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

			//Create Workspace
			int workspaceArtifactId = WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false).Result;

			//Import Documents
			int numberImported = ImportApiHelper.AddDocumentsToWorkspace(workspaceArtifactId, "document", 100, "").Result;
			if (numberImported == 0)
			{
				await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
				throw new Exception("Failed to Import Documents to the Workspace");
			}

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.ImageAllDocumentsInWorkspaceAsync(workspaceArtifactId).Wait());

			//Cleanup
			await WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
		}
	}
}
