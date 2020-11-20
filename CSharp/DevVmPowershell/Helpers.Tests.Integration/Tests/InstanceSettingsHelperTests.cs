using Helpers.Implementations;
using Helpers.Interfaces;
using kCura.Notification;
using NUnit.Framework;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class InstanceSettingsHelperTests
	{
		private IInstanceSettingsHelper Sut { get; set; }
		private int _createdInstanceSettingId;

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);

			Sut = new InstanceSettingsHelper(connectionHelper, TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void CreateInstanceSettingTest()
		{
			try
			{
				// Arrange
				string section = "TestInstanceSetting";
				string name = "TestInstanceSetting";
				string description = "";
				string value = "Test";
				_createdInstanceSettingId = 0;

				var existingInstanceSettingId = Sut.GetInstanceSettingArtifactIdByName(name, section);
				if (existingInstanceSettingId != 0)
				{
					Sut.DeleteInstanceSetting(existingInstanceSettingId);
				}

				// Act
				_createdInstanceSettingId = Sut.CreateInstanceSetting(section, name, description, value);

				// Assert
				Assert.True(_createdInstanceSettingId != 0);
			}
			finally
			{
				if (_createdInstanceSettingId != 0)
				{
					Sut.DeleteInstanceSetting(_createdInstanceSettingId);
				}
			}
		}

		[Test]
		public void UpdateInstanceSettingTest()
		{
			// Arrange
			string section = "Relativity.DataGrid";
			string name = "DataGridEndPoint";
			string value = "new_value";

			try
			{
				// Act
				bool success = Sut.UpdateInstanceSettingValue(name, section, value);
				string instanceSettingValue = Sut.GetInstanceSettingValue(name, section);

				// Assert
				Assert.AreEqual(value, instanceSettingValue);
			}
			finally
			{
				Sut.UpdateInstanceSettingValue(name, section, "");
			}
		}

		[Test]
		public void GetInstanceSettingArtifactIdByName()
		{
			// Arrange
			string section = "Relativity.DataGrid";
			string name = "DataGridEndPoint";

			// Act
			int instanceSettingId = Sut.GetInstanceSettingArtifactIdByName(name, section);

			// Assert
			Assert.AreNotEqual(instanceSettingId, 0);
		}
	}
}
