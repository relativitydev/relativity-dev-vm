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

		[Test]
		[TestCase(true)]
		public async Task CreateWorkerManagerServerTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool wasWorkerManagerServerCreated = await Sut.CreateWorkerManagerServer();

			//Assert
			Assert.That(wasWorkerManagerServerCreated, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(true)]
		public async Task AddWorkerManagerServerToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool wasWorkerManagerServerAddedToPool = await Sut.AddWorkerManagerServerToDefaultResourcePool();

			//Assert
			Assert.That(wasWorkerManagerServerAddedToPool, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(true)]
		public async Task AddWorkerServerToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool wasWorkerServerAddedToPool = await Sut.AddWorkerServerToDefaultResourcePool();

			//Assert
			Assert.That(wasWorkerServerAddedToPool, Is.EqualTo(expectedResult));
		}

		[Test]
		[TestCase(true)]
		public async Task AddAgentServerToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool wasAgentServerAddedToPool = await Sut.AddAgentServerToDefaultResourcePool();

			//Assert
			Assert.That(wasAgentServerAddedToPool, Is.EqualTo(expectedResult));
		}
	}
}
