using System;

namespace Helpers.Interfaces
{
	public interface ILogService
	{
		void LogVerbose(string message, params object[] properties);
		void LogDebug(string message, params object[] properties);
		void LogInformation(string message, params object[] properties);
		void LogWarning(string message, params object[] properties);
		void LogError(string message, Exception exception, params object[] properties);
		void LogFatal(string message, Exception exception, params object[] properties);
	}
}
