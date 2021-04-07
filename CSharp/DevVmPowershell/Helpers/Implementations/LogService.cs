using Helpers.Interfaces;
using Serilog;
using Serilog.Core;
using System.IO;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class LogService : ILogService
	{
		public async Task<Logger> GetLoggerAsync()
		{
			string tempFolderPath = Path.GetTempPath(); // Temp Path would be "C:\Users\<insert_windows_username_here>\AppData\Local\Temp" or %USERPROFILE%\AppData\Local\Temp
			string tempLogFilePath = Path.Combine(tempFolderPath, "DevVmPowerShellModule_Logs_.log");

			Logger logger = await Task.Run(() =>
				new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Debug() // Will be written to Visual Studio Output window
					.WriteTo.Console()
					.WriteTo.File(tempLogFilePath, rollingInterval: RollingInterval.Day)
					.CreateLogger());

			logger.Debug("Logger configured");

			return logger;
		}
	}
}
