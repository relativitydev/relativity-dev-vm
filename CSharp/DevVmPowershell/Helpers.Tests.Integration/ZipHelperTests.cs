using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
			string sourceFolderPath = @"C:\Users\aaron.gilbert\Documents\TempFolder";
			string destinationFolderPath = @"C:\Users\aaron.gilbert\Documents\TempFolder\test.zip";

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.ZipFolder(sourceFolderPath, destinationFolderPath));
		}
	}
}
