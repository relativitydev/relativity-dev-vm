using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class RelativityVersionHelperTests
	{
		private IRelativityVersionHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);
			IRestHelper restHelper = new RestHelper();
			Sut = new RelativityVersionHelper(connectionHelper, restHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void ConfirmInstallerAndInstanceRelativityVersionAreEqualTest()
		{
			// Arrange
			string installerRelativityVersion = TestConstants.INSTANCE_RELATIVITY_VERSION;

			// Act / Assert
			Assert.DoesNotThrow(() =>
			{
				Sut.ConfirmInstallerAndInstanceRelativityVersionAreEqual(installerRelativityVersion);
			});
		}
	}
}
