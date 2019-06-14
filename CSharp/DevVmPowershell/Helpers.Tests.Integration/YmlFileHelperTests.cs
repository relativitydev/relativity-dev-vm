using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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

			// Act
			// Assert
			Assert.DoesNotThrow(() => Sut.UpdateElasticSearchYml()); // To test this method make sure that the yml file exists at C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch.yml
		}
	}
}
