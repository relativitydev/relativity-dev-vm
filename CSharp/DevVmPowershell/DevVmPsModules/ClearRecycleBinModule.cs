using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace DevVmPsModules
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
