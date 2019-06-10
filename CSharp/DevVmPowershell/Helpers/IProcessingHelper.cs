using System.Threading.Tasks;

namespace Helpers
{
	public interface IProcessingHelper
	{
		bool CreateProcessingSourceLocationChoice();
		Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePool();
		Task<bool> CreateWorkerManagerServer();
		Task<bool> AddWorkerManagerServerToDefaultResourcePool();
		Task<bool> AddWorkerServerToDefaultResourcePool();
		Task<bool> AddAgentServerToDefaultResourcePool();
	}
}
