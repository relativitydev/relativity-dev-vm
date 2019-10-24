namespace Helpers.Interfaces
{
	public interface IApplicationInstallHelper
	{
		bool InstallApplicationFromRapFile(string workspaceName, string filePath);
		bool InstallApplicationFromApplicationLibrary(string workspaceName, string applicationGuid);
	}
}
