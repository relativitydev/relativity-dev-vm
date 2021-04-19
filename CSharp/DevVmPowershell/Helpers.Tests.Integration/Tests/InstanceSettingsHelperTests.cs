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
			IRestHelper restHelper = new RestHelper();
			Sut = new InstanceSettingsHelper(connectionHelper, restHelper);
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
					Sut.DeleteInstanceSettingAsync(existingInstanceSettingId).Wait();
				}

				// Act
				_createdInstanceSettingId = Sut.CreateInstanceSettingAsync(section, name, description, value).Result;

				// Assert
				Assert.True(_createdInstanceSettingId != 0);
			}
			finally
			{
				if (_createdInstanceSettingId != 0)
				{
					Sut.DeleteInstanceSettingAsync(_createdInstanceSettingId).Wait();
				}
			}
		}

		[Test]
		public void UpdateInstanceSettingTest()
		{
			// Arrange
			string section = "Relativity.DataGrid";
			string name = "DataGridEndPoint";
			string description = "";
			string initialValue = "initial_value";
			string value = "new_value";

			var existingInstanceSettingId = Sut.GetInstanceSettingArtifactIdByName(name, section);
			if (existingInstanceSettingId == 0)
			{
				Sut.CreateInstanceSettingAsync(name, section, description, initialValue).Wait();
			}

			try
			{
				// Act
				bool success = Sut.UpdateInstanceSettingValueAsync(name, section, value).Result;
				string instanceSettingValue = Sut.GetInstanceSettingValue(name, section);

				// Assert
				Assert.AreEqual(value, instanceSettingValue);
			}
			finally
			{
				Sut.UpdateInstanceSettingValueAsync(name, section, "").Wait();
			}
		}

		[Test]
		public void GetInstanceSettingArtifactIdByName()
		{
			// Arrange
			string section = "Relativity.DataGrid";
			string name = "DataGridEndPoint";
			string description = "";
			string value = "value";

			var existingInstanceSettingId = Sut.GetInstanceSettingArtifactIdByName(name, section);
			if (existingInstanceSettingId == 0)
			{
				Sut.CreateInstanceSettingAsync(name, section, description, value).Wait();
			}


			// Act
			int instanceSettingId = Sut.GetInstanceSettingArtifactIdByName(name, section);

			// Assert
			Assert.AreNotEqual(instanceSettingId, 0);
		}
	}
}
