using System.Threading.Tasks;

namespace Helpers
{
	public interface IWorkspaceHelper
	{
		Task<int> CreateWorkspaceAsync(string workspaceTemplateName, string workspaceName, bool enableDataGrid);
		Task DeleteAllWorkspacesAsync(string workspaceName);
		Task DeleteWorkspaceAsync(int workspaceArtifactId);
		int GetWorkspaceId(string workspaceName);
	}
}