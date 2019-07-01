using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.Imaging.Services.Interfaces;
using Relativity.Services.Search;
using Relativity.Services.ServiceProxy;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class ImagingHelperTests
	{
		private IImagingHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);
			ServiceFactory serviceFactory = connectionHelper.GetServiceFactory();

			IImagingProfileManager imagingProfileManager = serviceFactory.CreateProxy<IImagingProfileManager>();
			IImagingSetManager imagingSetManager = serviceFactory.CreateProxy<IImagingSetManager>();
			IImagingJobManager imagingJobManager = serviceFactory.CreateProxy<IImagingJobManager>();
			IRSAPIClient rsapiClient = serviceFactory.CreateProxy<IRSAPIClient>();
			IKeywordSearchManager keywordSearchManager = serviceFactory.CreateProxy<IKeywordSearchManager>();

			Sut = new ImagingHelper(imagingProfileManager, imagingSetManager, imagingJobManager, rsapiClient, keywordSearchManager);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void ImagingTest()
		{
			// Arrange
			int workspaceArtifactId = 1017386;

			// Act
			int savedSearchArtifactId = Sut.CreateKeywordSearchAsync(workspaceArtifactId).Result;
			int imagingProfileArtifactId = Sut.CreateImagingProfileAsync(workspaceArtifactId).Result;
			int imagingSetArtifactId = Sut.CreateImagingSetAsync(workspaceArtifactId, savedSearchArtifactId, imagingProfileArtifactId).Result;
			Sut.RunImagingJobAsync(workspaceArtifactId, imagingSetArtifactId).Wait();

			// Assert
			Assert.That(imagingProfileArtifactId > 0);
			Assert.That(imagingSetArtifactId > 0);
			Assert.DoesNotThrow(() => Sut.WaitForImagingJobToCompleteAsync(workspaceArtifactId, imagingSetArtifactId).Wait());
		}
	}
}
