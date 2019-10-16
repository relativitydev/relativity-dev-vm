using Helpers;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Reset, "JavaEnvironmentVariables")]
	public class ResetJavaEnvironmentVariables : BaseModule
	{
		protected override void ProcessRecordCode()
		{
			IEnvironmentVariableHelper environmentVariableHelper = new EnvironmentVariableHelper();

			// Update Java Environment Variables
			environmentVariableHelper.UpdateJavaEnvironmentVariables();
		}
	}
}
