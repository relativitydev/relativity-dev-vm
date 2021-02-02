using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.RequestModels
{
	public class InstanceSettingManagerUpdateRequest
	{
		public InstanceSetting instanceSetting { get; set; }
	}

	public class InstanceSetting
	{
		public int ArtifactId { get; set; }
		public string Value { get; set; }
	}
}
