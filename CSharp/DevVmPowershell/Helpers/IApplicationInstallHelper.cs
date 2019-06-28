using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public interface IApplicationInstallHelper
	{
		bool InstallApplicationFromRapFile(string workspaceName, string filePath);
		bool InstallApplicationFromApplicationLibrary(string workspaceName, string applicationGuid);
	}
}
