using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IProcessingHelper
	{
		bool CreateProcessingSourceLocationChoice();
		Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePoolAsync();
		Task<bool> CreateWorkerManagerServerAsync();
		Task<bool> DeleteWorkerManagerServerAsync();
		Task<bool> UpdateWorkerServerForProcessingAsync();
		Task<bool> AddWorkerManagerServerToDefaultResourcePoolAsync();
		Task<bool> AddWorkerServerToDefaultResourcePoolAsync();
		Task<bool> RemoveWorkerManagerServerFromDefaultResourcePoolAsync();
		Task<bool> RemoveWorkerServerFromDefaultResourcePoolAsync();
		Task<bool> FullSetupAndUpdateDefaultResourcePoolAsync();
		Task<bool> FullResetAsync();
	}
}