using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class InstanceSettingsHelperTests
	{
		private IInstanceSettingsHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);

			Sut = new InstanceSettingsHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void CreateInstanceSettingTest()
		{
			// Arrange
			string section = "TestInstanceSetting";
			string name = "TestInstanceSetting";
			string description = "";
			string value = "Test";

			// Act
			int createdInstanceSettingId = Sut.CreateInstanceSetting(section, name, description, value);

			// Assert
			Assert.True(createdInstanceSettingId > 1);
		}

		[Test]
		public void UpdateInstanceSettingTest()
		{
			// Arrange
			string section = "Relativity.DataGrid";
			string name = "DataGridEndPoint";
			string value = "test";

			// Act
			bool success = Sut.UpdateInstanceSettingValue(section, name, value);

			// Assert
			Assert.That(success, Is.True);
		}
	}
}
