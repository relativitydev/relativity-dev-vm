using System.Management.Automation;
using System.Threading.Tasks;

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

		protected override async Task ProcessRecordAsync()
		{
			await Task.Run(() =>
			{
				foreach (string name in Name)
				{
					WriteVerbose("Creating salutation for " + name);
					string salutation = "Hello, " + name;
					WriteObject(salutation);
				}
			});
		}
	}
}
