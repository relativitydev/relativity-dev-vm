// ReSharper disable ExpressionIsAlwaysNull
using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace DevVmPsModules
{
	public class BaseModule : PSCmdlet
	{
		protected virtual async Task BeginProcessingAsync()
		{
			await Task.Run(() => base.BeginProcessing());
		}

		protected virtual async Task ProcessRecordAsync()
		{
			await Task.Run(() => base.ProcessRecord());
		}

		protected virtual async Task EndProcessingAsync()
		{
			await Task.Run(() => base.EndProcessing());
		}

		protected override void BeginProcessing()
		{
			try
			{
				WriteVerbose($"Start - {nameof(BeginProcessing)} method");

				BeginProcessingAsync().Wait();
			}
			catch (Exception ex)
			{
				Exception exception = new Exception($"An error occured in {nameof(BeginProcessing)} method when writing Salutation", ex);
				string errorId = $"{nameof(BeginProcessing)}";
				const ErrorCategory errorCategory = ErrorCategory.DeviceError;
				object targetObject = null;
				ErrorRecord errorRecord = new ErrorRecord(exception, errorId, errorCategory, targetObject);
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

				ProcessRecordAsync().Wait();
			}
			catch (Exception ex)
			{
				Exception exception = new Exception($"An error occured in {nameof(ProcessRecord)} method when writing Salutation", ex);
				string errorId = $"{nameof(ProcessRecord)}";
				const ErrorCategory errorCategory = ErrorCategory.DeviceError;
				object targetObject = null;
				ErrorRecord errorRecord = new ErrorRecord(exception, errorId, errorCategory, targetObject);
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

				EndProcessingAsync().Wait();
			}
			catch (Exception ex)
			{
				Exception exception = new Exception($"An error occured in {nameof(EndProcessing)} method when writing Salutation", ex);
				string errorId = $"{nameof(EndProcessing)}";
				const ErrorCategory errorCategory = ErrorCategory.DeviceError;
				object targetObject = null;
				ErrorRecord errorRecord = new ErrorRecord(exception, errorId, errorCategory, targetObject);
				WriteError(errorRecord);
			}
			finally
			{
				WriteVerbose($"End - {nameof(EndProcessing)} method");
			}
		}
	}
}
