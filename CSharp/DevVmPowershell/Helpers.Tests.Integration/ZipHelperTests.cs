using NUnit.Framework;
using System;
using System.IO;
using File = System.IO.File;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class ZipHelperTests
	{
		private IZipHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			Sut = new ZipHelper();
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void ZipFolderTest()
		{
			// Arrange
			string binFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			if (string.IsNullOrWhiteSpace(binFolderPath))
			{
				throw new Exception($"{nameof(binFolderPath)} is invalid.");
			}
			string sourceFolderPath = Path.Combine(binFolderPath, TestConstants.TEST_ZIP_FILES_FOLDER_PATH); // Enter a path to a valid source folder
			string destinationZipFilePath = Path.Combine(binFolderPath, "test.zip"); // Enter a desired path to create your zip file not in the same folder as the sourceFolderPath

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.ZipFolder(sourceFolderPath, destinationZipFilePath));
			File.Delete(destinationZipFilePath);
		}
	}
}
