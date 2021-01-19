using Helpers.Implementations;
using Helpers.Interfaces;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration.Tests
{
	[TestFixture]
	public class AgentHelperTests
	{
		private IAgentHelper Sut { get; set; }

		[SetUp]
		public void SetUp()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				relativityInstanceName: TestConstants.RELATIVITY_INSTANCE_NAME,
				relativityAdminUserName: TestConstants.RELATIVITY_ADMIN_USER_NAME,
				relativityAdminPassword: TestConstants.RELATIVITY_ADMIN_PASSWORD,
				sqlAdminUserName: TestConstants.SQL_USER_NAME,
				sqlAdminPassword: TestConstants.SQL_PASSWORD);
			IRestHelper restHelper = new RestHelper();
			Sut = new AgentHelper(connectionHelper, restHelper, TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME, TestConstants.RELATIVITY_ADMIN_PASSWORD);
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

			//Act
			int numberOfAgentsCreated = await Sut.CreateAgentsInRelativityApplicationAsync(TestConstants.TEST_APPLICATION_NAME); //To Test this method, make sure the agent in the Test Application doesn't exist

			//Assert
			Assert.That(numberOfAgentsCreated, Is.GreaterThan(0));
		}

		[Test]
		public async Task DeleteAgentsInRelativityApplicationAsyncTest()
		{
			//Arrange

			//Act
			int numberOfAgentsDeleted = await Sut.DeleteAgentsInRelativityApplicationAsync(TestConstants.TEST_APPLICATION_NAME); //To Test this method, make sure the agent in the Test Application exist

			//Assert
			Assert.That(numberOfAgentsDeleted, Is.GreaterThan(0));
		}

		[Test]
		[TestCase(TestConstants.AGENT_NAME)]
		public async Task AddAgentToRelativityByNameAsyncTest(string agentName)
		{
			//Arrange

			//Act
			bool wasAdded = await Sut.AddAgentToRelativityByNameAsync(agentName); //To Test this method, make sure the agent in the Test Application exist

			//Assert
			Assert.That(wasAdded, Is.EqualTo(true));
		}

		[Test]
		[TestCase(TestConstants.AGENT_NAME)]
		public async Task RemoveAgentFromRelativityByNameAsyncTest(string agentName)
		{
			//Arrange

			//Act
			bool wasDeleted = await Sut.RemoveAgentFromRelativityByNameAsync(agentName); //To Test this method, make sure the agent in the Test Application exist

			//Assert
			Assert.That(wasDeleted, Is.EqualTo(true));
		}
	}
}
