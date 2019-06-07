using System.Threading.Tasks;

namespace Helpers
{
	public interface IProcessingHelper
	{
		bool CreateProcessingSourceLocationChoice();
		Task<bool> AddProcessingSourceLocationChoiceToDefaultResourcePool();
	}
}
