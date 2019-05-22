using System;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Get, "Salutation")]
	public class SampleModule : PSCmdlet
	{
		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Name to get salutation for")]
		[Alias("Person", "FirstName")]
		public string[] Name { get; set; }

		protected override void BeginProcessing()
		{
			try
			{
				WriteVerbose($"Start - {nameof(BeginProcessing)} method");

				base.BeginProcessing();
			}
			catch (Exception ex)
			{
				Exception exception = new Exception($"An error occured in {nameof(BeginProcessing)} method when writing Salutation", ex);
				ErrorRecord errorRecord = new ErrorRecord(exception, $"{nameof(BeginProcessing)}", ErrorCategory.DeviceError, null);
				WriteError(errorRecord);
			}
			finally
			{
				WriteVerbose($"End - {nameof(ProcessRecord)} method");
			}
		}

		protected override void ProcessRecord()
		{
			try
			{
				WriteVerbose($"Start - {nameof(ProcessRecord)} method");

				foreach (string name in Name)
				{
					WriteVerbose("Creating salutation for " + name);
					string salutation = "Hello, " + name;
					WriteObject(salutation);
				}
			}
			catch (Exception ex)
			{
				Exception exception = new Exception($"An error occured in {nameof(ProcessRecord)} method when writing Salutation", ex);
				ErrorRecord errorRecord = new ErrorRecord(exception, $"{nameof(ProcessRecord)}", ErrorCategory.DeviceError, null);
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
			}
			catch (Exception ex)
			{
				Exception exception = new Exception($"An error occured in {nameof(EndProcessing)} method when writing Salutation", ex);
				ErrorRecord errorRecord = new ErrorRecord(exception, $"{nameof(EndProcessing)}", ErrorCategory.DeviceError, null);
				WriteError(errorRecord);
			}
			finally
			{
				WriteVerbose($"End - {nameof(EndProcessing)} method");
			}
		}
	}
}
