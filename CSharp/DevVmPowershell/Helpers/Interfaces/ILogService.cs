using Serilog.Core;
using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface ILogService
	{
		Task<Logger> GetLoggerAsync();
	}
}
