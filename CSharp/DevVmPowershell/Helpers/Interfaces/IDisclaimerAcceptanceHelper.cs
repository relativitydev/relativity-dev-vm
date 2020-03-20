namespace Helpers.Interfaces
{
	public interface IDisclaimerAcceptanceHelper
	{
		void AddDisclaimerConfiguration(string workspaceName);
		bool CheckIfDisclaimerConfigurationRDOExists(int workspaceId);
		void AddDisclaimer(string workspaceName);
		bool CheckIfDisclaimerRDOExists(int workspaceId);
	}
}
