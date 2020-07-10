using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Interfaces;
using Relativity.Services.InstanceDetails;
using Relativity.Services.ServiceProxy;

namespace Helpers.Implementations
{
	public class RelativityVersionHelper : IRelativityVersionHelper
	{
		private ServiceFactory ServiceFactory { get; }

		public RelativityVersionHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		public void ConfirmInstallerAndInstanceRelativityVersionAreEqual(string installerRelativityVersion)
		{
			try
			{
				using (IInstanceDetailsManager instanceDetailsManager = ServiceFactory.CreateProxy<IInstanceDetailsManager>())
				{
					string relativityVersion = instanceDetailsManager.GetRelativityVersionAsync().Result;
					if (relativityVersion != installerRelativityVersion)
					{
						throw new Exception("Installed Relativity Version and Installer Version are not the same");
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error Getting Instance Relativity Version", ex);
			}
		}
	}
}
