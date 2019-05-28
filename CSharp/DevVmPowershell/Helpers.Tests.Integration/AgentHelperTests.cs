using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration
{
	[TestFixture]
	public class AgentHelperTests
	{
		private IAgentHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);
			Sut = new AgentHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public async Task CreateAgentsInRelativityApplicationAsyncTest()
		{
			//Arrange
			await Sut.DeleteAgentsInRelativityApplicationAsync(TestConstants.TEST_APPLICATION_NAME);

			//Wait for the Agents to be deleted
			const int maxWaitTimeInMilliSeconds = Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000;
			const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
			int currentWaitTimeInMilliSeconds = 0;
			int numberOfAgentsExists = 0;

			while (currentWaitTimeInMilliSeconds < maxWaitTimeInMilliSeconds && numberOfAgentsExists != 0)
			{
				Thread.Sleep(sleepTimeInMilliSeconds);

				numberOfAgentsExists = await Sut.CheckIfAtLeastSingleInstanceOfAgentExistsInRelativityApplicationAsync(TestConstants.TEST_APPLICATION_NAME);

				currentWaitTimeInMilliSeconds += sleepTimeInMilliSeconds;
			}


			//Act
			int numberOfAgentsCreated = await Sut.CreateAgentsInRelativityApplicationAsync(TestConstants.TEST_APPLICATION_NAME); //To Test this method, make sure the agent in the Test Application doesn't exist

			//Assert
			Assert.That(numberOfAgentsCreated, Is.GreaterThan(0));
		}

		[Test]
		public async Task DeleteAgentsInRelativityApplicationAsyncTest()
		{
			//Arrange
			await Sut.CreateAgentsInRelativityApplicationAsync(TestConstants.TEST_APPLICATION_NAME);

			//Act
			int numberOfAgentsDeleted = await Sut.DeleteAgentsInRelativityApplicationAsync(TestConstants.TEST_APPLICATION_NAME); //To Test this method, make sure the agent in the Test Application exist

			//Assert
			Assert.That(numberOfAgentsDeleted, Is.GreaterThan(0));
		}
	}
}
