using DevVmPsModules.CustomExceptions;
using kCura.Relativity.Client.DTOs;
using System.Linq;

namespace Helpers
{
	public class ApplicationHelper : IApplicationHelper
	{
		public int GetRelativityApplicationArtifactId(kCura.Relativity.Client.IRSAPIClient rsapiClient, int applicationName)
		{
			RelativityApplication relativityApplicationDto = new RelativityApplication();
			ResultSet<RelativityApplication> relativityApplicationReadResultSet = rsapiClient.Repositories.RelativityApplication.Read(relativityApplicationDto);
			if (relativityApplicationReadResultSet == null || !relativityApplicationReadResultSet.Success || relativityApplicationReadResultSet.Results == null || relativityApplicationReadResultSet.Results.Count <= 0)
			{
				throw new DevVmPowerShellModuleException($"An error occured when querying for Relativity Application ArtifactId [{nameof(applicationName)}: {applicationName}]");
			}

			int applicationArtifactId = relativityApplicationReadResultSet.Results.First().Artifact.ArtifactID;
			return applicationArtifactId;
		}
	}
}
