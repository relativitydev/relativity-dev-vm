﻿using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class InstanceSettingsHelperTests
	{
		private IInstanceSettingsHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);

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
			Sut.DeleteInstanceSetting(createdInstanceSettingId);
		}

		[Test]
		public void UpdateInstanceSettingTest()
		{
			// Arrange
			string section = "Relativity.DataGrid";
			string name = "DataGridEndPoint";
			string value = " ";

			// Act
			bool success = Sut.UpdateInstanceSettingValue(name, section, value);
			string instanceSettingValue = Sut.GetInstanceSettingValue(name, section);

			// Assert
			Assert.AreEqual(value, instanceSettingValue);
		}
	}
}