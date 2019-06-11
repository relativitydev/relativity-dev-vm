using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.InstanceSetting;
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
			using (IInstanceSettingManager instanceSettingManager = ServiceFactory.CreateProxy<IInstanceSettingManager>())
			{
				Query query = new Query();
				//instanceSettingManager.QueryAsync()
			}
		}
	}
}
