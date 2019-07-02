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
			
			Sut = new ImagingHelper(connectionHelper);
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
			// Assert
			Assert.DoesNotThrow(() => Sut.ImageAllDocumentsInWorkspaceAsync(workspaceArtifactId).Wait());
		}
	}
}
