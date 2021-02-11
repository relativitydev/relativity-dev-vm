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
			IRestHelper restHelper = new RestHelper();
			Sut = new RelativityVersionHelper(restHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
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
