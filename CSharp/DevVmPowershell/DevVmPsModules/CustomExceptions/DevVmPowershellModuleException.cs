namespace DevVmPsModules.CustomExceptions
{
	[System.Serializable]
	public class DevVmPowerShellModuleException : System.Exception
	{
		public DevVmPowerShellModuleException()
		{
		}

		public DevVmPowerShellModuleException(string message)
			: base(message)
		{
		}

		public DevVmPowerShellModuleException(string message, System.Exception inner)
			: base(message, inner)
		{
		}

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client.
		protected DevVmPowerShellModuleException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
	}
}
