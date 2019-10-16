using NUnit.Framework;
using System;
using System.IO;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class EnvironmentVariableTests
	{
		private IEnvironmentVariableHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			Sut = new EnvironmentVariableHelper();
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void UpdateJavaEnvironmentVariables()
		{
			// Arrange
			bool toDelete1 = CreateEnvironmentVariableIfDoesNotExist(TestConstants.ENVIRONMENT_VARIABLE_KCURA_JAVA_HOME);
			bool toDelete2 = CreateEnvironmentVariableIfDoesNotExist(TestConstants.ENVIRONMENT_VARIABLE_JAVA_HOME);

			//Setup Java Install folder
			//Create folders if they doesn't already exist
			Directory.CreateDirectory(TestConstants.JAVA_INSTALL_PATH);
			string javaVersionFolderPath = Path.Combine(TestConstants.JAVA_INSTALL_PATH, @"JavaVersionFolder");
			Directory.CreateDirectory(javaVersionFolderPath);

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.UpdateJavaEnvironmentVariables());

			//Cleanup
			DeleteEnvironmentVariableIfWeCreatedIt(TestConstants.ENVIRONMENT_VARIABLE_KCURA_JAVA_HOME, toDelete1);
			DeleteEnvironmentVariableIfWeCreatedIt(TestConstants.ENVIRONMENT_VARIABLE_JAVA_HOME, toDelete2);
			System.IO.Directory.Delete(javaVersionFolderPath, true);
		}

		private static void DeleteEnvironmentVariableIfWeCreatedIt(string environmentVariableName, bool toDelete)
		{
			// If we've created it, now delete it.
			if (toDelete)
			{
				Environment.SetEnvironmentVariable(environmentVariableName, null, EnvironmentVariableTarget.Machine);

				// Confirm the deletion.
				if (Environment.GetEnvironmentVariable(environmentVariableName) == null)
				{
					Console.WriteLine("Test1 has been deleted.");
				}
			}
		}

		private static bool CreateEnvironmentVariableIfDoesNotExist(string environmentVariableName)
		{
			// Check whether the environment variable exists.
			string value = Environment.GetEnvironmentVariable(environmentVariableName, EnvironmentVariableTarget.Machine);
			bool toDelete = false;

			// If necessary, create it.
			if (value == null)
			{
				Environment.SetEnvironmentVariable(environmentVariableName, "");
				toDelete = true;
			}

			return toDelete;
		}
	}
}
