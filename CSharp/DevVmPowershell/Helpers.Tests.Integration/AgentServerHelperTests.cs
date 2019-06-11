using NUnit.Framework;
using System.Threading.Tasks;

namespace Helpers.Tests.Integration
{
	public class AgentServerHelperTests
	{
		private IAgentServerHelper Sut { get; set; }

		[SetUp]
		public void Setup()
		{
			IConnectionHelper connectionHelper = new ConnectionHelper(
				TestConstants.RELATIVITY_INSTANCE_NAME,
				TestConstants.RELATIVITY_ADMIN_USER_NAME,
				TestConstants.RELATIVITY_ADMIN_PASSWORD);

			Sut = new AgentServerHelper(connectionHelper);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
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
