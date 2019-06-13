using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class YmlFileHelperTests
	{
		private IYmlFileHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			//IConnectionHelper connectionHelper = new ConnectionHelper(
			//	TestConstants.RELATIVITY_INSTANCE_NAME,
			//	TestConstants.RELATIVITY_ADMIN_USER_NAME,
			//	TestConstants.RELATIVITY_ADMIN_PASSWORD);
			//Sut = new ApplicationInstallHelper(connectionHelper);
			Sut = new YmlFileHelper();
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void UpdateElasticSearchYmlTest()
		{
			// Arrange

			// Act
			Sut.UpdateElasticSearchYml();

			// Assert
		}
	}
}
