using Helpers.Implementations;
using Helpers.Interfaces;
using System.Management.Automation;

namespace DevVmPsModules.Cmdlets
{
	[Cmdlet(VerbsCommon.Clear, "RecycleBin")]
	public class ClearRecycleBinModule : BaseModule
	{
		protected override void ProcessRecordCode()
		{
			IRecycleBinHelper recycleBinHelper = new RecycleBinHelper();

			// Empty Recycle Bin
			recycleBinHelper.EmptyRecycleBin();
		}
	}
}
