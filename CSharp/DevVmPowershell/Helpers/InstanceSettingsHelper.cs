using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;
using Relativity.Services.ServiceProxy;

namespace Helpers
{
	public class InstanceSettingsHelper : IInstanceSettingsHelper
	{
		private ServiceFactory ServiceFactory { get; }

		public InstanceSettingsHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		public void UpdateInstanceSettings(string section, string name, string newValue)
		{
			
		}
	}
}
