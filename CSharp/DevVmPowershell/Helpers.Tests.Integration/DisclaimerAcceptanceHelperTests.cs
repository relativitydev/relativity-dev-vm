using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class DisclaimerAcceptanceHelperTests
	{
		private IDisclaimerAcceptanceHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);

			Sut = new DisclaimerAcceptanceHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void AddDisclaimerConfigurationTest()
		{
			// Arrange
			string workspaceName = "Sample Workspace";

			// Act

			// Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimerConfiguration(workspaceName));
		}

		[Test]
		public void AddDisclaimerTest()
		{
			// Arrange
			string workspaceName = "Sample Workspace";

			// Act

			// Assert
			Assert.DoesNotThrow(() => Sut.AddDisclaimer(workspaceName));
		}
	}
}
