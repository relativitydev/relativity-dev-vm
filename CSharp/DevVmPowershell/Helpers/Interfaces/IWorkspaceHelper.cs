using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IWorkspaceHelper
	{
		Task<int> CreateSingleWorkspaceAsync(string workspaceTemplateName, string workspaceName, bool enableDataGrid);
		Task DeleteAllWorkspacesAsync(string workspaceName);
		void DeleteSingleWorkspace(int workspaceArtifactId);
		Task<int> GetWorkspaceCountQueryAsync(string workspaceName);
		Task<int> GetFirstWorkspaceArtifactIdQueryAsync(string workspaceName);
	}
}