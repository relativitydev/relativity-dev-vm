using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public interface IInstanceSettingsHelper
	{
		int CreateInstanceSetting(string name, string section, string description, string value);
		bool UpdateInstanceSettingValue(string name, string section, string newValue);
	}
}
