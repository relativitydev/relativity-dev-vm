using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IDisclaimerAcceptanceHelper
	{
		Task AddDisclaimerConfigurationAsync(string workspaceName);
		bool CheckIfDisclaimerConfigurationRdoExists(int workspaceId);
		Task AddDisclaimerAsync(string workspaceName);
		bool CheckIfDisclaimerRdoExists(int workspaceId);
	}
}
