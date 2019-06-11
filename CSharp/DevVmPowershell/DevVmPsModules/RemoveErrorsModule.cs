using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Remove, "Errors")]
	public class RemoveErrorsModule : BaseModule
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
		public string SqlUserName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 2,
			HelpMessage = "Password of the Relativity Sql Account")]
		public string SqlPassword { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			ISqlHelper sqlHelper = new SqlHelper(RelativityInstanceName, SqlUserName, SqlPassword);

			// Delete all Errors
			sqlHelper.DeleteAllErrors();
		}

		private void ValidateInputArguments()
		{
			if (string.IsNullOrWhiteSpace(RelativityInstanceName))
			{
				throw new ArgumentNullException(nameof(RelativityInstanceName), $"{nameof(RelativityInstanceName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(SqlUserName))
			{
				throw new ArgumentNullException(nameof(SqlUserName), $"{nameof(SqlUserName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(SqlPassword))
			{
				throw new ArgumentNullException(nameof(SqlPassword), $"{nameof(SqlPassword)} cannot be NULL or Empty.");
			}
		}
	}
}
