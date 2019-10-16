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
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);

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
