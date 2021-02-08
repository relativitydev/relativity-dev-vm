﻿using Helpers.Implementations;
using Helpers.Interfaces;
using System;
using System.Management.Automation;

namespace DevVmPsModules.Cmdlets
{
	[Cmdlet(VerbsCommon.New, "Agents")]
	public class AgentModule : BaseModule
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
			HelpMessage = "Comma separated list of Relativity Application Names to create agents")]
		public string[] ApplicationNames { get; set; }

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
			IRestHelper restHelper = new RestHelper();
			IAgentHelper agentHelper = new AgentHelper(connectionHelper, restHelper, RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);

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

			if (string.IsNullOrWhiteSpace(SqlAdminUserName))
			{
				throw new ArgumentNullException(nameof(SqlAdminUserName), $"{nameof(SqlAdminUserName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(SqlAdminPassword))
			{
				throw new ArgumentNullException(nameof(SqlAdminPassword), $"{nameof(SqlAdminPassword)} cannot be NULL or Empty.");
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
