namespace AgentsConsole.CustomExceptions
{
	[System.Serializable]
	public class AgentsConsoleException : System.Exception
	{
		public AgentsConsoleException()
		{
		}

		public AgentsConsoleException(string message)
			: base(message)
		{
		}

		public AgentsConsoleException(string message, System.Exception inner)
			: base(message, inner)
		{
		}

		// A constructor is needed for serialization when an
		// exception propagates from a remoting server to the client.
		protected AgentsConsoleException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
	}
}
