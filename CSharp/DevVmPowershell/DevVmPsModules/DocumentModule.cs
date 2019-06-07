using Helpers;
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
			HelpMessage = "Relativity Workspace ID to receive the files")]
		public int WorkspaceId { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 4,
			HelpMessage = "Either document or image")]
		public string FileType { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 5,
			HelpMessage = "How many documents or images you want to upload")]
		public int FileCount { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);

			IImportApiHelper importApi = new ImportApiHelper(connectionHelper);

			// Add documents for each Workspace ID specified
			importApi.AddDocumentsToWorkspace(WorkspaceId, FileType, FileCount);
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

			if (WorkspaceId <= 0)
			{
				throw new ArgumentException(nameof(WorkspaceId), $"{nameof(WorkspaceId)} cannot be less than or equal to 0.");
			}

			if (string.IsNullOrWhiteSpace(FileType))
			{
				throw new ArgumentNullException(nameof(FileType), $"{nameof(FileType)} cannot be NULL or Empty.");
			}

			if (!FileType.ToLower().Equals("document", StringComparison.OrdinalIgnoreCase) && !FileType.ToLower().Equals("image", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentNullException(nameof(FileType), $"{nameof(FileType)} must be either document or image.");
			}

			if (FileCount < 0)
			{
				throw new ArgumentNullException(nameof(FileCount), $"{nameof(FileCount)} cannot be less than 0.");
			}
		}
	}
}
