using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface ISmokeTestHelper
	{
		Task<bool> WaitForSmokeTestToCompleteAsync(string workspaceName, int timeoutValueInMinutes);
	}
}
