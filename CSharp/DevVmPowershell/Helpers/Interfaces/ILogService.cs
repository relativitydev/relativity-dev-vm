using System;

namespace Helpers.Interfaces
{
	public interface ILogService
	{
		/// <summary>
		/// Logs a Verbose message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="properties">Properties list to log</param>
		void LogVerbose(string message, params object[] properties);

		/// <summary>
		/// Logs a Debug message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="properties">Properties list to log</param>
		void LogDebug(string message, params object[] properties);

		/// <summary>
		/// Logs an Information message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="properties">Properties list to log</param>
		void LogInformation(string message, params object[] properties);

		/// <summary>
		/// Logs a Warning message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="properties">Properties list to log</param>
		void LogWarning(string message, params object[] properties);

		/// <summary>
		/// Logs an Error message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="exception">The exception to log</param>
		/// <param name="properties">Properties list to log</param>
		void LogError(string message, Exception exception, params object[] properties);

		/// <summary>
		/// Logs a Fatal message
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="exception">The exception to log</param>
		/// <param name="properties">Properties list to log</param>
		void LogFatal(string message, Exception exception, params object[] properties);
	}
}
