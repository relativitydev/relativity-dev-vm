using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.RequestModels
{
	public class InstanceSettingManagerCreateRequest
	{
		public instanceSetting instanceSetting { get; set; }
	}

	public class instanceSetting
	{
		public string Name { get; set; }
		public string Section { get; set; }
		public string Machine { get; set; }
		public string ValueType { get; set; }
		public string Value { get; set; }
		public string InitialValue { get; set; }
		public bool Encrypted { get; set; }
		public string Description { get; set; }
		public string Keywords { get; set; }
		public string Notes { get; set; }
	}
}
