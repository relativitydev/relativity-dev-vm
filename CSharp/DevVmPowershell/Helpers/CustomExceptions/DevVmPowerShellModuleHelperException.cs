﻿namespace Helpers.CustomExceptions
{
	public class DevVmPowerShellModuleHelperException : System.Exception
	{
		public DevVmPowerShellModuleHelperException()
		{
		}

		public DevVmPowerShellModuleHelperException(string message)
			: base(message)
		{
		}

		public DevVmPowerShellModuleHelperException(string message, System.Exception inner)
			: base(message, inner)
		{
		}

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client.
		protected DevVmPowerShellModuleHelperException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
	}
}
