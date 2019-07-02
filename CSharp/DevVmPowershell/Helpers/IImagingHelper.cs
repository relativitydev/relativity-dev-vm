using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public interface IImagingHelper
	{
		Task ImageAllDocumentsInWorkspaceAsync(int workspaceArtifactId);
		Task<int> CreateImagingProfileAsync(int workspaceArtifactId);
		Task<int> CreateImagingSetAsync(int workspaceArtifactId, int savedSearchArtifactId, int imagingProfileArtifactId);
		Task RunImagingJobAsync(int workspaceArtifactId, int imagingSetArtifactId);
		Task WaitForImagingJobToCompleteAsync(int workspaceArtifactId, int imagingSetArtifactId);
		Task<int> CreateKeywordSearchAsync(int workspaceArtifactId);
	}
}
