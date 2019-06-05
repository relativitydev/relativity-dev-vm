using Helpers;
using System;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.New, "ImportApi")]
	public class ImportApiModule : BaseModule
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
			HelpMessage = "Protocol (Http or Https) of the Relativity Instance")]
		public string RelativityProtocol { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 2,
			HelpMessage = "Username of the Relativity Admin")]
		public string RelativityAdminUserName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 3,
			HelpMessage = "Password of the Relativity Admin")]
		public string RelativityAdminPassword { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 4,
			HelpMessage = "Relativity Workspace ID to receive the files")]
		public int WorkspaceId { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 5,
			HelpMessage = "Either documents or images")]
		public string FileType { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 6,
			HelpMessage = "How many documents or images you want to upload")]
		public int FileCount { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);

			string webServiceUrl = $@"{RelativityProtocol}://{RelativityInstanceName}/relativitywebapi/";

			IImportApiHelper importApi = new ImportApiHelper(connectionHelper, RelativityAdminUserName, RelativityAdminPassword, webServiceUrl);

			// Add documents for each Workspace ID specified
			try
			{
				importApi.AddDocumentsToWorkspace(WorkspaceId, FileType, FileCount);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

		}

		private void ValidateInputArguments()
		{
			if (string.IsNullOrWhiteSpace(RelativityInstanceName))
			{
				throw new ArgumentNullException(nameof(RelativityInstanceName), $"{nameof(RelativityInstanceName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(RelativityProtocol))
			{
				throw new ArgumentNullException(nameof(RelativityProtocol), $"{nameof(RelativityProtocol)} cannot be NULL or Empty.");
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

			if (string.IsNullOrWhiteSpace(FileType) && (FileType.ToLower().Contains("documents") || FileType.ToLower().Contains("images")))
			{
				throw new ArgumentNullException(nameof(FileType), $"{nameof(FileType)} cannot be NULL or Empty.");
			}

			if (!FileType.ToLower().Contains("documents") && !FileType.ToLower().Contains("images"))
			{
				throw new ArgumentNullException(nameof(FileType), $"{nameof(FileType)} must be either documents or images.");
			}

			if (FileCount < 0)
			{
				throw new ArgumentNullException(nameof(FileCount), $"{nameof(FileCount)} cannot be less than 0.");
			}
		}
	}
}
