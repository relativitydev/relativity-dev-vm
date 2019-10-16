using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IAgentServerHelper
	{
		Task<bool> AddAgentServerToDefaultResourcePoolAsync();
		Task<bool> RemoveAgentServerFromDefaultResourcePoolAsync();
	}
}
