using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.RequestModels
{
	public class ObjectManagerQueryRequestModel
	{
		public Request request { get; set; }
		public int start { get; set; }
		public int length { get; set; }
	}

	public class Request
	{
		public ObjectType objectType { get; set; }
		public object[] fields { get; set; }
		public string condition { get; set; }
		public object[] sorts { get; set; }
	}

	public class ObjectType
	{
		public string Name { get; set; }
	}
}
