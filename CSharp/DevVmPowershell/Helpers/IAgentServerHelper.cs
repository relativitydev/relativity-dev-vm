using System.Threading.Tasks;

namespace Helpers
{
	public interface IAgentServerHelper
	{
		Task<bool> AddAgentServerToDefaultResourcePool();
	}
}
