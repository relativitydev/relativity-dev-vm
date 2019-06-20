using Helpers;
using System;
using System.IO;
using System.Management.Automation;

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
				ProcessRecordCode();
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
	}
}
