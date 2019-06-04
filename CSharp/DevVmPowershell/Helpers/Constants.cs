using Relativity.Services.Agent;

namespace Helpers
{
	public class Constants
	{
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
			public const int EDDS_WORKSPACE_ARTIFACT_ID = -1;
			public const bool ENABLE_AGENT = true;
			public const int AGENT_INTERVAL = 20;
			public const Agent.LoggingLevelEnum AGENT_LOGGING_LEVEL = Agent.LoggingLevelEnum.All;
		}

		public class CommonFields
		{
			public const string ControlNumber = "Control Number";
			public const string FilePath = "file path";
			public const string FolderName = "folder name";
			public const string FileName = "File Name";
			public const string ParentDocId = "Parent Document ID";
			public const string Bates = "Bates";
			public const string Doc = "Doc";
			public const string File = "File";
		}

		public class CommonArtifactIds
		{
			public const int ControlNumber = 1003667;
		}

		public class FileTypes
		{
			public const string Documents = "documents";
			public const string Images = "images";
		}
	}
}
