using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class RecycleBinTests
	{
		private IRecycleBinHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			Sut = new RecycleBinHelper();
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void EmptyRecycleBinTest()
		{
			// Arrange

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.EmptyRecycleBin());
		}
	}
}
