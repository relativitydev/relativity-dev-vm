using Helpers;
using System;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.New, "ProcessingSourceLocationChoice")]
	public class ProcessingCreateSourceLocationChoiceModule : BaseModule
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

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);

			IProcessingHelper processingHelper = new ProcessingHelper(connectionHelper);

			processingHelper.CreateProcessingSourceLocationChoice();
		}

		private void ValidateInputArguments()
		{
			if (string.IsNullOrWhiteSpace(RelativityInstanceName))
			{
				throw new ArgumentNullException(nameof(RelativityInstanceName),
					$"{nameof(RelativityInstanceName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(RelativityAdminUserName))
			{
				throw new ArgumentNullException(nameof(RelativityAdminUserName),
					$"{nameof(RelativityAdminUserName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(RelativityAdminPassword))
			{
				throw new ArgumentNullException(nameof(RelativityAdminPassword),
					$"{nameof(RelativityAdminPassword)} cannot be NULL or Empty.");
			}
		}
	}
}
