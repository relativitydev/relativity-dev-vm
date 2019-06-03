using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Get, "Salutation")]
	public class SampleModule : BaseModule
	{
		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Name to get salutation for")]
		[Alias("Person", "FirstName")]
		public string[] Name { get; set; }

		protected override void ProcessRecordCode()
		{
			foreach (string name in Name)
			{
				WriteVerbose("Creating salutation for " + name);
				string salutation = "Hello, " + name;
				WriteObject(salutation);
			}
		}
	}
}
