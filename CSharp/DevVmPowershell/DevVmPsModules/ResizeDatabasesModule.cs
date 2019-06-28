﻿using Helpers;
using System;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Resize, "Databases")]
	public class ResizeDatabasesModule : BaseModule
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
			HelpMessage = "Username of the Relativity Sql Account")]
		public string SqlAdminUserName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 2,
			HelpMessage = "Password of the Relativity Sql Account")]
		public string SqlAdminPassword { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			ISqlHelper sqlHelper = new SqlHelper(RelativityInstanceName, SqlAdminUserName, SqlAdminPassword);

			sqlHelper.RunShrinkDbProc();
		}

		private void ValidateInputArguments()
		{
			if (string.IsNullOrWhiteSpace(RelativityInstanceName))
			{
				throw new ArgumentNullException(nameof(RelativityInstanceName), $"{nameof(RelativityInstanceName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(SqlAdminUserName))
			{
				throw new ArgumentNullException(nameof(SqlAdminUserName), $"{nameof(SqlAdminUserName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(SqlAdminPassword))
			{
				throw new ArgumentNullException(nameof(SqlAdminPassword), $"{nameof(SqlAdminPassword)} cannot be NULL or Empty.");
			}
		}
	}
}
