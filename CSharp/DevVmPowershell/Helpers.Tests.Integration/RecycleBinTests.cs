using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Helpers.Tests.Integration
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
