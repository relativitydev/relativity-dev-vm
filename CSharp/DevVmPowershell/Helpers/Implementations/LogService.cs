using Helpers.Interfaces;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Compact;
using System;
using System.IO;

namespace Helpers.Implementations
{
	public class LogService : ILogService
	{
		private readonly Logger _logger;

		public LogService()
		{
			var tempLogFilePath = GetTempLogFilePath();

			// Logging is based on Serilog (https://github.com/serilog/serilog/wiki/Configuration-Basics)
			_logger =
				new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Debug() // Will be written to Visual Studio Output window
					.WriteTo.Console()
					.WriteTo.File(
						new CompactJsonFormatter(),
						tempLogFilePath,
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true) // A new log file will be created for every single day
					.CreateLogger();

			_logger.Debug("Logger configured");
			_logger.Debug("Log file path --> {tempLogFilePath}", tempLogFilePath);
		}

		private string GetTempLogFilePath()
		{
			/*
			 * A new log file will be created for every single day
			 * Log file will be located in the Temp folder. Temp folder would be located at "C:\Users\<insert_windows_username_here>\AppData\Local\Temp"
			 * Abstracted username Temp folder path is %USERPROFILE%\AppData\Local\Temp
			 * Example Log file path: "C:\Users\john.doe\AppData\Local\Temp\DevVmPowerShellModule_Logs__20210407.log"
			 */
			string tempFolderPath = Path.GetTempPath();
			string tempLogFilePath = Path.Combine(tempFolderPath, Constants.Logs.LogFileName);
			return tempLogFilePath;
		}

		public void LogVerbose(string message, params object[] properties)
		{
			_logger.Verbose(message);
			foreach (object property in properties)
			{
				_logger.Verbose($"{nameof(property)}", property);
			}
		}

		public void LogDebug(string message, params object[] properties)
		{
			_logger.Debug(message);
			foreach (object property in properties)
			{
				_logger.Debug($"{nameof(property)}", property);
			}
		}

		public void LogInformation(string message, params object[] properties)
		{
			_logger.Information(message);
			foreach (object property in properties)
			{
				_logger.Information($"{nameof(property)}", property);
			}
		}

		public void LogWarning(string message, params object[] properties)
		{
			_logger.Warning(message);
			foreach (object property in properties)
			{
				_logger.Warning($"{nameof(property)}", property);
			}
		}

		public void LogError(string message, Exception exception, params object[] properties)
		{
			_logger.Error(exception, message);
			foreach (object property in properties)
			{
				_logger.Error($"{nameof(property)}", property);
			}
		}

		public void LogFatal(string message, Exception exception, params object[] properties)
		{
			_logger.Fatal(exception, message);
			foreach (object property in properties)
			{
				_logger.Fatal($"{nameof(property)}", property);
			}
		}
	}
}
