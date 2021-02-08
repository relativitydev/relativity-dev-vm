using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IApplicationInstallHelper
	{
		Task<bool> InstallApplicationFromRapFileAsync(string workspaceName, string filePath);
		Task<bool> InstallApplicationFromApplicationLibraryAsync(string workspaceName, string applicationGuid);
	}
}
