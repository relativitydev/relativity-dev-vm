﻿using Helpers;
using System;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.New, "Documents")]
	public class DocumentModule : BaseModule
	{
		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Name of the Relativity Instance")]
		public string RelativityInstanceName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 1,
			HelpMessage = "Username of the Relativity Admin")]
		public string RelativityAdminUserName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 2,
			HelpMessage = "Password of the Relativity Admin")]
		public string RelativityAdminPassword { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 3,
			HelpMessage = "Relativity Workspace Name to receive the files")]
		public string WorkspaceName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 4,
			HelpMessage = "Either " + Constants.FileType.Document + " or " + Constants.FileType.Image)]
		public string FileType { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 5,
			HelpMessage = "How many files you want to upload")]
		public int FileCount { get; set; }

		[Parameter(
			Mandatory = false,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 6,
			HelpMessage = "Path to the Resource Folder (do not include /Resources in this)")]
		public string ResourceFilePath { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);

			IImportApiHelper importApi = new ImportApiHelper(connectionHelper);

			// Get workspaceId
			int workspaceId = importApi.GetFirstWorkspaceIdQueryAsync(WorkspaceName).Result;

			// Add documents for each Workspace ID specified
			importApi.AddDocumentsToWorkspace(workspaceId, FileType, FileCount, ResourceFilePath).Wait();
		}

		private void ValidateInputArguments()
		{
			if (string.IsNullOrWhiteSpace(RelativityInstanceName))
			{
				throw new ArgumentNullException(nameof(RelativityInstanceName), $"{nameof(RelativityInstanceName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(RelativityAdminUserName))
			{
				throw new ArgumentNullException(nameof(RelativityAdminUserName), $"{nameof(RelativityAdminUserName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(RelativityAdminPassword))
			{
				throw new ArgumentNullException(nameof(RelativityAdminPassword), $"{nameof(RelativityAdminPassword)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(WorkspaceName))
			{
				throw new ArgumentNullException(nameof(WorkspaceName), $"{nameof(WorkspaceName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(FileType))
			{
				throw new ArgumentNullException(nameof(FileType), $"{nameof(FileType)} cannot be NULL or Empty.");
			}

			if (!FileType.Equals(Constants.FileType.Document, StringComparison.OrdinalIgnoreCase) && !FileType.Equals(Constants.FileType.Image, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentNullException(nameof(FileType), $"{nameof(FileType)} must be either {Constants.FileType.Document} or {Constants.FileType.Image}.");
			}

			if (FileCount < 0)
			{
				throw new ArgumentNullException(nameof(FileCount), $"{nameof(FileCount)} cannot be less than 0.");
			}

		}
	}
}
