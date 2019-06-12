using NUnit.Framework;
using System.Threading;
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

		[Test, Order(45)]
		[TestCase(true)]
		public async Task UpdateWorkerServerForProcessingTest(bool expectedResult)
		{
			//Arrange

			//Act
			Thread.Sleep(30);
			bool result = await Sut.UpdateWorkerServerForProcessing();

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

		[Test, Order(60)]
		[TestCase(true)]
		public async Task RemoveWorkerServerFromDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.RemoveWorkerServerFromDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(70)]
		[TestCase(true)]
		public async Task RemoveWorkerManagerServerFromDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.RemoveWorkerManagerServerFromDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(80)]
		[TestCase(true)]
		public async Task RemoveProcessingSourceLocationChoiceToDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange
			// No actual API to remove Processing Source Location yet.  You'll have to do this manually on your instance

			//Act
			bool result = await Sut.RemoveProcessingSourceLocationChoiceToDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		//[Test, Order(90)]
		//[TestCase(true)]
		//public async Task DeleteWorkerServerTest(bool expectedResult)
		//{
		//	//Arrange

		//	//Act
		//	bool result = await Sut.DeleteWorkerServer();

		//	//Assert
		//	Assert.That(result, Is.EqualTo(expectedResult));
		//}

		[Test, Order(100)]
		[TestCase(true)]
		public async Task DeleteWorkerManagerServerTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.DeleteWorkerManagerServer();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(110)]
		[TestCase(true)]
		public void DeleteProcessingSourceLocationChoiceTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = Sut.DeleteProcessingSourceLocationChoice();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(1000)]
		[TestCase(true)]
		public async Task FullSetupAndUpdateDefaultResourcePoolTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.FullSetupAndUpdateDefaultResourcePool();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(1010)]
		[TestCase(true)]
		public async Task FullResetTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.FullReset();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
