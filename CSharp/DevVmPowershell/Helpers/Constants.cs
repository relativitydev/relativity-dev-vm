using Relativity.Services.Agent;

namespace Helpers
{
	public class Constants
	{
		public const int EDDS_WORKSPACE_ARTIFACT_ID = -1;

		public class Connection
		{
			public const string PROTOCOL = "http";
		}
		public class Agents
		{
			public const string AGENT_OBJECT_TYPE = "Agent";
			public const string AGENT_FIELD_NAME = "Name";
			public const string RELATIVITY_DEV_VM = "RelativityDevVm";
			public const string KEYWORDS = RELATIVITY_DEV_VM;
			public const string NOTES = RELATIVITY_DEV_VM;
			public const bool ENABLE_AGENT = true;
			public const int AGENT_INTERVAL = 20;
			public const Agent.LoggingLevelEnum AGENT_LOGGING_LEVEL = Agent.LoggingLevelEnum.All;
		}

		public class Workspace
		{
			public const string DEFAULT_WORKSPACE_TEMPLATE_NAME = "New Case Template";
			public const bool ACCESSIBLE = true;
			public const string DATABASE_LOCATION = @"ST002EDDS.T002.ctus010000.relativity.one,24331";
		}

		public class Waiting
		{
			public const int MAX_WAIT_TIME_IN_MINUTES = 10;
			public const int SLEEP_TIME_IN_SECONDS = 15;
		}
	}
}
