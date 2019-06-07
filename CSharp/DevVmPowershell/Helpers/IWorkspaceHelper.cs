using System.Threading.Tasks;

namespace Helpers
{
	public interface IWorkspaceHelper
	{
		Task<int> CreateWorkspaceAsync(string workspaceTemplateName, string workspaceName);
		Task DeleteAllWorkspacesAsync(string workspaceName);
		Task DeleteWorkspaceAsync(int workspaceArtifactId);
	}
}