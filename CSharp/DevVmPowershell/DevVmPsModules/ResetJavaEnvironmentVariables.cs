using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Helpers;

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
