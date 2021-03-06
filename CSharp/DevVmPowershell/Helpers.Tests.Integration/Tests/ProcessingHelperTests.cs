﻿using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration.Tests
{
	//These tests must be run in a specific order
	[TestFixture]
	public class ProcessingHelperTests
	{
		private IProcessingHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);
			IRestHelper restHelper = new RestHelper();
			Sut = new ProcessingHelper(connectionHelper, restHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test, Order(10)]
		[TestCase(true)]
		public async Task CreateProcessingSourceLocationChoiceTestAsync(bool expectedResult)
		{
			//Arrange
			bool result = await Sut.CreateProcessingSourceLocationChoiceAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(20)]
		[TestCase(true)]
		public async Task AddProcessingSourceLocationChoiceToDefaultResourcePoolAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(30)]
		[TestCase(true)]
		public async Task CreateWorkerManagerServerAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.CreateWorkerManagerServerAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(40)]
		[TestCase(true)]
		public async Task AddWorkerManagerServerToDefaultResourcePoolAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.AddWorkerManagerServerToDefaultResourcePoolAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(45)]
		[TestCase(true)]
		public async Task UpdateWorkerServerForProcessingAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			Thread.Sleep(30);
			bool result = await Sut.UpdateWorkerServerForProcessingAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(50)]
		[TestCase(true)]
		public async Task AddWorkerServerToDefaultResourcePoolAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.AddWorkerServerToDefaultResourcePoolAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(60)]
		[TestCase(true)]
		public async Task RemoveWorkerServerFromDefaultResourcePoolAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.RemoveWorkerServerFromDefaultResourcePoolAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(70)]
		[TestCase(true)]
		public async Task RemoveWorkerManagerServerFromDefaultResourcePoolAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.RemoveWorkerManagerServerFromDefaultResourcePoolAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(100)]
		[TestCase(true)]
		public async Task DeleteWorkerManagerServerAsyncTest(bool expectedResult)
		{
			//Arrange

			//Act
			bool result = await Sut.DeleteWorkerManagerServerAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(1000)]
		[TestCase(true)]
		public async Task FullSetupAndUpdateDefaultResourcePoolAsyncTest(bool expectedResult)
		{
			//Arrange
			// No API exists yet to remove Processing Source Locations.  You'll have to manually delete it from the Default Resource Pool and from the Choice List

			//Act
			bool result = await Sut.FullSetupAndUpdateDefaultResourcePoolAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test, Order(1010)]
		[TestCase(true)]
		public async Task FullResetAsyncTest(bool expectedResult)
		{
			//Arrange
			// No API exists yet to remove Processing Source Locations.  You'll have to manually delete it from the Default Resource Pool and from the Choice List

			//Act
			bool result = await Sut.FullResetAsync();

			//Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}