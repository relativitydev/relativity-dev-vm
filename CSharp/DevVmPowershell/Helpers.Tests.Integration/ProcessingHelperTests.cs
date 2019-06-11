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

		[Test, Order(10)]
		[TestCase(true)]
		public void CreateProcessingSourceLocationChoiceTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = Sut.CreateProcessingSourceLocationChoice();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(20)]
		[TestCase(true)]
		public async Task AddProcessingSourceLocationChoiceToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.AddProcessingSourceLocationChoiceToDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(30)]
		[TestCase(true)]
		public async Task CreateWorkerManagerServerTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.CreateWorkerManagerServer();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(40)]
		[TestCase(true)]
		public async Task AddWorkerManagerServerToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.AddWorkerManagerServerToDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(50)]
		[TestCase(true)]
		public async Task AddWorkerServerToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.AddWorkerServerToDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(1000)]
		[TestCase(true)]
		public async Task FullSetupAndUpdateDefaultResourcePool(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.FullSetupAndUpdateDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
