using Helpers;
using System;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.New, "Workspace")]
	public class WorkspaceModule : BaseModule
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
			HelpMessage = "Name of the workspace to create")]
		public string WorkspaceName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 3,
			HelpMessage = "Indicator to enable DataGrid in the workspace")]
		public bool EnableDataGrid { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);
			IAgentHelper agentHelper = new AgentHelper(connectionHelper);

			//Create Agents for all Applications
			foreach (string applicationName in ApplicationNames)
			{
				//Create Agents for Application
				string trimmedApplicationName = applicationName.Trim();
				agentHelper.CreateAgentsInRelativityApplicationAsync(trimmedApplicationName).Wait();
			}
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

			if (ApplicationNames.Length == 0)
			{
				throw new ArgumentException($"{nameof(RelativityAdminPassword)} cannot be an Empty Array.", nameof(RelativityAdminPassword));
			}

			foreach (string applicationName in ApplicationNames)
			{
				if (string.IsNullOrWhiteSpace(applicationName))
				{
					throw new ArgumentNullException(nameof(applicationName), $"{nameof(applicationName)} cannot be NULL or Empty.");
				}
			}
		}
	}
}
