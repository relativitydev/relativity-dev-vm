using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Threading;

namespace DevVmPsModules
{
	public class BaseModule : PSCmdlet
	{
		protected virtual void BeginProcessingCode()
		{

		}

		protected virtual void ProcessRecordCode()
		{

		}

		protected virtual void EndProcessingCode()
		{

		}

		protected override void BeginProcessing()
		{
			try
			{
				WriteVerbose($"Start - {nameof(BeginProcessing)} method");

				base.BeginProcessing();
				BeginProcessingCode();
			}
			catch (Exception ex)
			{
				string formattedExceptionMessage = ErrorMessageHelper.FormatErrorMessage(ex);
				Exception exception = new Exception($"An error occured in {nameof(BeginProcessing)} method. ErrorMessage: {formattedExceptionMessage}. StackTrace: {ex}", ex);
				string errorId = $"{nameof(BeginProcessing)}";
				const ErrorCategory errorCategory = ErrorCategory.DeviceError;
				ErrorRecord errorRecord = new ErrorRecord(exception, errorId, errorCategory, null);
				WriteError(errorRecord);
			}
			finally
			{
				WriteVerbose($"End - {nameof(BeginProcessing)} method");
			}
		}

		protected override void ProcessRecord()
		{
			try
			{
				WriteVerbose($"Start - {nameof(ProcessRecord)} method");

				base.ProcessRecord();
				RetryProcessRecordCode(ProcessRecordCode, TimeSpan.FromSeconds(5), 5);
			}
			catch (Exception ex)
			{
				CreateErrorLogFile(ex);
				string formattedExceptionMessage = ErrorMessageHelper.FormatErrorMessage(ex);
				Exception exception = new Exception($"An error occured in {nameof(ProcessRecord)} method. ErrorMessage: {formattedExceptionMessage}. StackTrace: {ex}", ex);
				string errorId = $"{nameof(ProcessRecord)}";
				const ErrorCategory errorCategory = ErrorCategory.DeviceError;
				ErrorRecord errorRecord = new ErrorRecord(exception, errorId, errorCategory, null);
				WriteError(errorRecord);
			}
			finally
			{
				WriteVerbose($"End - {nameof(ProcessRecord)} method");
			}
		}

		protected override void EndProcessing()
		{
			try
			{
				WriteVerbose($"Start - {nameof(EndProcessing)} method");

				base.EndProcessing();
				EndProcessingCode();
			}
			catch (Exception ex)
			{
				string formattedExceptionMessage = ErrorMessageHelper.FormatErrorMessage(ex);
				Exception exception = new Exception($"An error occured in {nameof(EndProcessing)} method. ErrorMessage: {formattedExceptionMessage}. StackTrace: {ex}", ex);
				string errorId = $"{nameof(EndProcessing)}";
				const ErrorCategory errorCategory = ErrorCategory.DeviceError;
				ErrorRecord errorRecord = new ErrorRecord(exception, errorId, errorCategory, null);
				WriteError(errorRecord);
			}
			finally
			{
				WriteVerbose($"End - {nameof(EndProcessing)} method");
			}
		}

		private static void CreateErrorLogFile(Exception exception)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			Console.WriteLine($"{nameof(currentDirectory)}: {currentDirectory}");
			string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			string errorLogFileName = $"DevVmPowerShellModule_ErrorLog_{currentDateTime}.txt";
			string errorLogFileFullPath = Path.Combine(currentDirectory, errorLogFileName);

			//Delete File if it already exists
			if (File.Exists(errorLogFileFullPath))
			{
				File.Delete(errorLogFileFullPath);
			}

			//Create File
			using (StreamWriter streamWriter = File.CreateText(errorLogFileFullPath))
			{
				string errorMessage = exception.Message;
				string completeErrorMessage = ErrorMessageHelper.FormatErrorMessage(exception);
				string stackTrace = exception.StackTrace;
				streamWriter.WriteLine($"ERROR MESSAGE:\n{errorMessage}");
				streamWriter.WriteLine();
				streamWriter.WriteLine($"COMPLETE ERROR MESSAGE:\n{completeErrorMessage}");
				streamWriter.WriteLine();
				streamWriter.WriteLine($"STACK TRACE:\n{stackTrace}");
			}
		}

		/// <summary>
		/// Retry logic directed at ProcessRecordCode
		/// https://stackoverflow.com/questions/1563191/cleanest-way-to-write-retry-logic
		/// </summary>
		/// <param name="action"></param>
		/// <param name="timeBetweenRetry"></param>
		/// <param name="maxAttemptCount"></param>
		protected void RetryProcessRecordCode(Action action, TimeSpan timeBetweenRetry, int maxAttemptCount)
		{
			List<Exception> exceptions = new List<Exception>();

			for (int attempted = 0; attempted < maxAttemptCount; attempted++)
			{
				try
				{
					if (attempted > 0)
					{
						WriteVerbose($"Waiting before next attempt - {nameof(action)} method.  Attempt: {attempted + 1}/{maxAttemptCount}");
						Thread.Sleep(timeBetweenRetry);
					}

					Console.WriteLine($"Start - {action.Method.Name} method.  Attempt: {attempted + 1}/{maxAttemptCount}");

					base.ProcessRecord();
					action();

					Console.WriteLine($"Finished - {action.Method.Name} method on attempt: {attempted + 1}/{maxAttemptCount}");

					return;
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
			}
			throw new AggregateException(exceptions);
		}
	}
}
