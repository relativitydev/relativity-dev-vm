using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IDisclaimerAcceptanceHelper
	{
		Task AddDisclaimerConfigurationAsync(string workspaceName);
		Task<bool> CheckIfDisclaimerConfigurationRdoExistsAsync(int workspaceId);
		Task AddDisclaimerAsync(string workspaceName);
		Task<bool> CheckIfDisclaimerRdoExistsAsync(int workspaceId);
	}
}
