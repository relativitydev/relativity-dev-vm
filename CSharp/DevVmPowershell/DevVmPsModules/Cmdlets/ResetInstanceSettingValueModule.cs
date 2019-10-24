using Helpers.Implementations;
using Helpers.Interfaces;
using System;
using System.Management.Automation;

namespace DevVmPsModules.Cmdlets
{
	[Cmdlet(VerbsCommon.Reset, "InstanceSettingValue")]
	public class ResetInstanceSettingValueModule : BaseModule
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
			HelpMessage = "Username of the Relativity Sql Account")]
		public string SqlAdminUserName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 4,
			HelpMessage = "Password of the Relativity Sql Account")]
		public string SqlAdminPassword { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 5,
			HelpMessage = "Name of the Instance Setting")]
		public string Name { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 6,
			HelpMessage = "Section of the Instance Setting")]
		public string Section { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 7,
			HelpMessage = "New Value of the Instance Setting")]
		public string NewValue { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: RelativityInstanceName,
				relativityAdminUserName: RelativityAdminUserName,
				relativityAdminPassword: RelativityAdminPassword,
				sqlAdminUserName: SqlAdminUserName,
				sqlAdminPassword: SqlAdminPassword);
			IInstanceSettingsHelper instanceSettingsHelper = new InstanceSettingsHelper(connectionHelper);

			// Update Instance Setting Value
			instanceSettingsHelper.UpdateInstanceSettingValue(Name, Section, NewValue);
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

			if (string.IsNullOrWhiteSpace(Name))
			{
				throw new ArgumentNullException(nameof(Name), $"{nameof(Name)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(Section))
			{
				throw new ArgumentNullException(nameof(Section), $"{nameof(Section)} cannot be NULL or Empty.");
			}
		}
	}
}
