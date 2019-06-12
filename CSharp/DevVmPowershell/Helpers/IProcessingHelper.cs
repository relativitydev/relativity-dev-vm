using System.Threading.Tasks;

namespace Helpers
{
	public interface IProcessingHelper
	{
		bool CreateProcessingSourceLocationChoice();
		Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePool();
		Task<bool> CreateWorkerManagerServer();
		Task<bool> DeleteWorkerManagerServer();
		Task<bool> UpdateWorkerServerForProcessing();
		Task<bool> AddWorkerManagerServerToDefaultResourcePool();
		Task<bool> AddWorkerServerToDefaultResourcePool();
		Task<bool> RemoveWorkerManagerServerFromDefaultResourcePool();
		Task<bool> RemoveWorkerServerFromDefaultResourcePool();
		Task<bool> FullSetupAndUpdateDefaultResourcePool();
		Task<bool> FullReset();
	}
}