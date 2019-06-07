using NUnit.Framework;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class ProcessingHelperTests
	{
		private IProcessingHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);

			Sut = new ProcessingHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		[TestCase(true)]
		public void CreateProcessingSourceLocationChoiceTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool wasSourceLocationChoiceCreated = Sut.CreateProcessingSourceLocationChoice();

			//Assert
			Assert.That(wasSourceLocationChoiceCreated, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(true)]
		public async Task AddProcessingSourceLocationChoiceToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool wasSourceLocationChoiceCreated = await Sut.AddProcessingSourceLocationChoiceToDefaultResourcePool();

			//Assert
			Assert.That(wasSourceLocationChoiceCreated, Is.EqualTo(expectedResult));
		}
	}
}
