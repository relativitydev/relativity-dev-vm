using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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

			// Act
			Sut.UpdateJavaEnvironmentVariables();

			// Assert
		}
	}
}
