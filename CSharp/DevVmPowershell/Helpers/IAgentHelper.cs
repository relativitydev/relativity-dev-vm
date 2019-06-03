using System.Threading.Tasks;

namespace Helpers
{
	public interface IAgentHelper
	{
		Task<int> CreateAgentsInRelativityApplicationAsync(string applicationName);
		Task<int> DeleteAgentsInRelativityApplicationAsync(string applicationName);
	}
}