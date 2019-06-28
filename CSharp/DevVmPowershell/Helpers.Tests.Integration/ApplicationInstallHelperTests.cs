﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Helpers.Tests.Integration;
using NUnit.Framework;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class ApplicationInstallHelperTests
	{
		private IApplicationInstallHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);
			Sut = new ApplicationInstallHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void InstallApplicationFromARapFileTest()
		{
			// Arrange
			string rapLocation = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(@"bin\Debug\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll", TestConstants.APPLICATION_FILE_PATH);
			string workspaceName = "Sample Data Grid Workspace";

			// Act
			bool installationResult = Sut.InstallApplicationFromRapFile(workspaceName, rapLocation);

			// Assert
			Assert.That(installationResult, Is.True);
		}

		[Test]
		public void InstallApplicationFromTheApplicationLibraryTest()
		{
			// Arrange
			string applicationGuid = "0125C8D4-8354-4D8F-B031-01E73C866C7C"; // Guid of the Smoke Test Application

			// Act
			bool installationResult = Sut.InstallApplicationFromApplicationLibrary(TestConstants.RELATIVITY_WORKSPACE_NAME, applicationGuid);
			
			// Assert
			Assert.That(installationResult, Is.True);
		}
	}
}
