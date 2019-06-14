using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Helpers;

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
