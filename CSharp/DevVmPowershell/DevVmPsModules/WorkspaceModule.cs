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
			Position = 4,
			HelpMessage = "Indicator to enable DataGrid in the workspace")]
		public string EnableDataGrid { get; set; }

		private bool _enableDataGrid;

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);
			IWorkspaceHelper workspaceHelper = new WorkspaceHelper(connectionHelper);

			//Create Workspace
			workspaceHelper.CreateWorkspaceAsync(Constants.Workspace.DEFAULT_WORKSPACE_TEMPLATE_NAME, WorkspaceName, _enableDataGrid).Wait();
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

			if (string.IsNullOrWhiteSpace(EnableDataGrid))
			{
				throw new ArgumentNullException(nameof(EnableDataGrid), $"{nameof(EnableDataGrid)} cannot be NULL or Empty.");
			}

			if (!bool.TryParse(EnableDataGrid, out _enableDataGrid))
			{
				throw new ArgumentNullException(nameof(EnableDataGrid), $"{nameof(EnableDataGrid)} is not a valid value. [Valid values: True or False]");
			}
		}
	}
}
