using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using kCura.Relativity.Client;
using Relativity.Imaging.Services.Interfaces;
using Relativity.Services.Search;
using Relativity.Services.ServiceProxy;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Add, "ImagesForAllDocuments")]
	public class AddImagesForAllDocumentsModule : BaseModule
	{
		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Name of the Relativity Instance")]
		public string RelativityInstanceName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 1,
			HelpMessage = "Username of the Relativity Admin")]
		public string RelativityAdminUserName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 2,
			HelpMessage = "Password of the Relativity Admin")]
		public string RelativityAdminPassword { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 3,
			HelpMessage = "Workspace Name that you want to install the application in")]
		public string WorkspaceName { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IConnectionHelper connectionHelper = new ConnectionHelper(RelativityInstanceName, RelativityAdminUserName, RelativityAdminPassword);
			ServiceFactory serviceFactory = connectionHelper.GetServiceFactory();
			IImagingProfileManager imagingProfileManager = serviceFactory.CreateProxy<IImagingProfileManager>();
			IImagingSetManager imagingSetManager = serviceFactory.CreateProxy<IImagingSetManager>();
			IImagingJobManager imagingJobManager = serviceFactory.CreateProxy<IImagingJobManager>();
			IRSAPIClient rsapiClient = serviceFactory.CreateProxy<IRSAPIClient>();
			IKeywordSearchManager keywordSearchManager = serviceFactory.CreateProxy<IKeywordSearchManager>();

			IImagingHelper imagingHelper = new ImagingHelper(imagingProfileManager, imagingSetManager, imagingJobManager, rsapiClient, keywordSearchManager);
			IWorkspaceHelper workspaceHelper = new WorkspaceHelper(connectionHelper, null);

			// Run Imaging Job
			int workspaceArtifactId = workspaceHelper.GetWorkspaceId(WorkspaceName);
			int savedSearchArtifactId = imagingHelper.CreateKeywordSearchAsync(workspaceArtifactId).Result;
			int imagingProfileArtifactId = imagingHelper.CreateImagingProfileAsync(workspaceArtifactId).Result;
			int imagingSetArtifactId = imagingHelper.CreateImagingSetAsync(workspaceArtifactId, savedSearchArtifactId, imagingProfileArtifactId).Result;
			imagingHelper.RunImagingJobAsync(workspaceArtifactId, imagingSetArtifactId).Wait();
			imagingHelper.WaitForImagingJobToCompleteAsync(workspaceArtifactId, imagingSetArtifactId).Wait();
		}

		private void ValidateInputArguments()
		{
			if (string.IsNullOrWhiteSpace(RelativityInstanceName))
			{
				throw new ArgumentNullException(nameof(RelativityInstanceName), $"{nameof(RelativityInstanceName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(RelativityAdminUserName))
			{
				throw new ArgumentNullException(nameof(RelativityAdminUserName), $"{nameof(RelativityAdminUserName)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(RelativityAdminPassword))
			{
				throw new ArgumentNullException(nameof(RelativityAdminPassword), $"{nameof(RelativityAdminPassword)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(WorkspaceName))
			{
				throw new ArgumentNullException(nameof(WorkspaceName), $"{nameof(WorkspaceName)} cannot be NULL or Empty.");
			}
		}
	}
}
