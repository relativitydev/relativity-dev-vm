using System;
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
				Exception exception = new Exception($"An error occured in {nameof(BeginProcessing)} method when writing Salutation", ex);
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
				Exception exception = new Exception($"An error occured in {nameof(ProcessRecord)} method when writing Salutation", ex);
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
				Exception exception = new Exception($"An error occured in {nameof(EndProcessing)} method when writing Salutation", ex);
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
	}
}
