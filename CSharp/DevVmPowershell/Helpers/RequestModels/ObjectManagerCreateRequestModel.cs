using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.RequestModels
{
	public class ObjectManagerCreateRequestModel
	{
		public request Request { get; set; }
		public OperationOptions OperationOptions { get; set; }
	}

	public class request
	{
		public objectType ObjectType { get; set; }
		public parentObject ParentObject { get; set; }
		public object[] FieldValues { get; set; }
	}

	public class objectType
	{
		public int ArtifactTypeID { get; set; }
	}

	public class parentObject
	{
		public int ArtifactID { get; set; }
	}

	public class OperationOptions
	{
		public CallingContext CallingContext { get; set; }
	}

	public class CallingContext
	{
		public Layout Layout { get; set; }
	}

	public class Layout
	{
		public int ArtifactID { get; set; }
	}
}
