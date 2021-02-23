using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ResponseModels
{
	public class AgentTypeResponseModel
  {
		public string ApplicationName { get; set; }
		public string CompanyName { get; set; }
		public decimal? DefaultInterval { get; set; }
		public int? DefaultLoggingLevel { get; set; }
		public int ArtifactID { get; set; }
		public string Name { get; set; }
  }
}
