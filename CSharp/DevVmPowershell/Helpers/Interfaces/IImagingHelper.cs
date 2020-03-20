using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IImagingHelper
	{
		Task ImageAllDocumentsInWorkspaceAsync(int workspaceArtifactId);
		Task<int> CreateImagingProfileAsync(int workspaceArtifactId);
		Task<int> CreateImagingSetAsync(int workspaceArtifactId, int savedSearchArtifactId, int imagingProfileArtifactId);
		Task RunImagingJobAsync(int workspaceArtifactId, int imagingSetArtifactId);
		Task WaitForImagingJobToCompleteAsync(int workspaceArtifactId, int imagingSetArtifactId);
		Task<int> CreateKeywordSearchAsync(int workspaceArtifactId);
		Task<bool> CheckThatAllDocumentsInWorkspaceAreImaged(int workspaceArtifactId);
	}
}
