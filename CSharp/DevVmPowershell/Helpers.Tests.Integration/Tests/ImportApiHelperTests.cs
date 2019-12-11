﻿using System;
using Helpers.Implementations;
using Helpers.Interfaces;
using kCura.Relativity.ImportAPI.Data;
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
		[TestCase(Constants.FileType.Document, 15)]
		[TestCase(Constants.FileType.Image, 15)]
		public void AddDocumentsToWorkspaceTest(string fileType, int numberOfFiles)
		{
			//Arrange
			string workspaceName = "ImportApi Test Workspace";
			CleanupWorkspaceIfItExists(workspaceName);

			int workspaceArtifactId = WorkspaceHelper.CreateSingleWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, workspaceName, false).Result;

			//Act
			int numberOfFilesImported = Sut.AddDocumentsToWorkspace(WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName).Result, fileType, numberOfFiles, "").Result;

			//Assert
			Assert.That(numberOfFilesImported, Is.EqualTo(numberOfFiles));

			//Cleanup
			WorkspaceHelper.DeleteSingleWorkspaceAsync(workspaceArtifactId);
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
