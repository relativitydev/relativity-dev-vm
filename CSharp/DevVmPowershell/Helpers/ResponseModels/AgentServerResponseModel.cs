using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ResponseModels
{
	public class AgentServerResponseModel
	{
		public string Type { get; set; }
		public int ProcessorCores { get; set; }
		public int NumberOfAgents { get; set; }
		public int ArtifactID { get; set; }
		public string Name { get; set; }
	}
}
