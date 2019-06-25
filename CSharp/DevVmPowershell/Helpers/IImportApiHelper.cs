using System.Threading.Tasks;

namespace Helpers
{
	public interface IImportApiHelper
	{
		Task<int> AddDocumentsToWorkspace(int workspaceId, string fileType, int count, string resourceFolderPath);
		Task<int> GetFirstWorkspaceIdQueryAsync(string workspaceName);
	}
}
