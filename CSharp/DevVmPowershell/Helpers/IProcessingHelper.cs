using System.Threading.Tasks;

namespace Helpers
{
	public interface IProcessingHelper
	{
		bool CreateProcessingSourceLocationChoice();
		bool DeleteProcessingSourceLocationChoice();
		Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePool();
		Task<bool> CreateWorkerManagerServer();
		Task<bool> DeleteWorkerManagerServer();
		Task<bool> AddWorkerManagerServerToDefaultResourcePool();
		Task<bool> AddWorkerServerToDefaultResourcePool();
		Task<bool> FullSetupAndUpdateDefaultResourcePool();
	}
}