using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbContextHelper;
using NUnit.Framework;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class SqlHelperTests
	{
		private ISqlHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			Sut = new SqlHelper(TestConstants.RELATIVITY_INSTANCE_NAME, TestConstants.SQL_USER_NAME, TestConstants.SQL_PASSWORD);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public void DeleteAllErrorsTest()
		{
			// Act
			bool result = Sut.DeleteAllErrors();

			// Assert
			Assert.That(result, Is.True);
		}
	}
}
