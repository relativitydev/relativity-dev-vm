using Helpers;
using System.Management.Automation;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Reset, "ElasticSearchYmlFile")]
	public class ResetElasticSearchYmlFile : BaseModule
	{
		protected override void ProcessRecordCode()
		{
			IYmlFileHelper ymlFileHelper = new YmlFileHelper();

			// Update Elastic Search Yml File
			ymlFileHelper.UpdateElasticSearchYml();
		}
	}
}
