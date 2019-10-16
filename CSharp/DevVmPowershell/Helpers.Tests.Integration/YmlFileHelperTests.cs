using NUnit.Framework;
using System;
using System.IO;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class YmlFileHelperTests
	{
		private IYmlFileHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
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
			const string parentDataGridDirectory = @"C:\RelativityDataGrid";

			//Cleanup
			if (Directory.Exists(parentDataGridDirectory))
			{
				Directory.Delete(parentDataGridDirectory, true);
			}

			//Create folders if they doesn't already exist
			Directory.CreateDirectory(parentDataGridDirectory);
			Directory.CreateDirectory($@"{parentDataGridDirectory}\elasticsearch-main");
			Directory.CreateDirectory($@"{parentDataGridDirectory}\elasticsearch-main\config");

			string binFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			if (string.IsNullOrWhiteSpace(binFolderPath))
			{
				throw new Exception($"{nameof(binFolderPath)} is invalid.");
			}
			string sourceFilePath = Path.Combine(binFolderPath, TestConstants.ELASTIC_SEARCH_YML_FILE_PATH); // Enter a path to a valid yml file
			const string destinationFilePath = @"C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch.yml";
			File.Copy(sourceFilePath, destinationFilePath, true);

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.UpdateElasticSearchYml()); // To test this method make sure that the yml file exists at C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch.yml

			//Cleanup
			if (File.Exists(destinationFilePath))
			{
				File.Delete(destinationFilePath);
			}
			if (Directory.Exists(parentDataGridDirectory))
			{
				Directory.Delete(parentDataGridDirectory, true);
			}
		}
	}
}
