using Relativity.Services.Agent;

namespace Helpers
{
	public class Constants
	{
		public const int EDDS_WORKSPACE_ARTIFACT_ID = -1;

		public class Connection
		{
			public const string PROTOCOL = "http";

			public class Sql
			{
				public const string ConnectionString_PersistSecurityInfo = "False";
				public const string ConnectionString_PacketSize = "4096";
				public const string ConnectionString_ConnectTimeoutDefault = "30";
				public const string ConnectionString_ConnectTimeoutLong = "120";
				public const string ConnectionString_DefaultDatabase = "EDDS";
			}
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
		}

		public class Waiting
		{
			public const int MAX_WAIT_TIME_IN_MINUTES = 10;
			public const int SLEEP_TIME_IN_SECONDS = 15;
		}

		public class DocumentCommonFields
		{
			public const string DocumentTypeRef = "Document";
			public const string HasImages = "Has Images";
			public const string HasNative = "Has Native";
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

		public class FileType
		{
			public const string Document = "document";
			public const string Image = "image";
		}

		public class Processing
		{
			public const int WorkspaceId = -1;
			public const string ChoiceName = @"\\RELATIVITYDEVVM\ProcessingSourceLocation";
			public const int ChoiceTypeID = 1000017;
			public const int ChoiceArtifactTypeId = 7;
			public const string ResourceServerName = "RELATIVITYDEVVM";
			public const string ResourceServerUrl = "RELATIVITYDEVVM";
			public const string ChoiceFieldRef = "TextIdentifier";
			public const string NameField = "Name";
			public const string DefaultPool = "Default";
			public const string WorkerManagerServer = "Worker Manager Server";
			public const string WorkerServer = "Worker";
		}

		public class AgentServer
		{
			public const string NameField = "Name";
			public const string ResourceServerName = "RELATIVITYDEVVM";
			public const string AgentServerName = "Agent";
			public const string DefaultPool = "Default";
		}

		public class SqlScripts
		{
			public const string ShrinkDbProcName = "ShrinkDBs";
			public const string SchemaName = "eddsdbo";
			public const string DoesExist = "IF (OBJECT_ID('@@schema.@@objectName') IS NOT NULL) SELECT 1 as does_exist ELSE SELECT 0 as does_exist";
			public const string ExecuteShringDbProc = "EXEC EDDS.eddsdbo.ShrinkDBs";
			public const string CreateOrAlterShrinkDbProc = @"
CREATE PROCEDURE eddsdbo.ShrinkDBs
AS
BEGIN
	SET NOCOUNT ON;
	
	/*************************************************************
		GATHER ALL DATABASES TO SHRINK
	*************************************************************/
	IF (OBJECT_ID('tempdb..#databases') IS NOT NULL) DROP TABLE #databases;
	SELECT			d.name as db_name
			INTO	#databases
	FROM			sys.databases d
	WHERE			d.name LIKE 'EDDS%'
			OR		d.name LIKE 'Invariant%'
	
	
	/*************************************************************
		MAIN CURSOR
	*************************************************************/
	DECLARE @db_name nvarchar(100) = N''
			,@sql nvarchar(max) = N''
	
	DECLARE cursor_mini CURSOR FAST_FORWARD FOR
		SELECT			d.db_name
		FROM			#databases d
	OPEN cursor_mini
	FETCH NEXT FROM cursor_mini INTO @db_name
	WHILE @@FETCH_STATUS = 0
	BEGIN
		PRINT N'Running on....' + @db_name
		
		-- Truncate the log by changing the database recovery model to SIMPLE.
		SET @sql = N'
			USE @@DB_NAME
			ALTER DATABASE @@DB_NAME
			SET RECOVERY SIMPLE;
		'
		SET @sql = REPLACE(@sql, N'@@DB_NAME', @db_name)
		EXEC (@sql)
		
		-- Shrink the truncated log file to 1 GB.
		SET @sql = N'
			USE @@DB_NAME
			DBCC SHRINKFILE (@@DB_NAME, 1);
		'
		SET @sql = REPLACE(@sql, N'@@DB_NAME', @db_name)
		EXEC (@sql)
		
	FETCH NEXT FROM cursor_mini
	INTO @db_name
	END
	CLOSE cursor_mini
	DEALLOCATE cursor_mini
END
";
    }
    
		public class YmlFile
		{
			public const string YmlFilePath = @"C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch.yml";
			public const string TempYmlFilePath = @"C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch_temp.yml";
			public const string OriginalYmlFilePath = @"C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch_original.yml";
			public const string DiscoveryZenPingUnicastHosts = "discovery.zen.ping.unicast.hosts";
			public const string DiscoveryZenPingUnicastHostsValue = "discovery.zen.ping.unicast.hosts: [\"RELATIVITYDEVVM\"]";
			public const string ActionDestructiveRequiresName = "action.destructive_requires_name";
			public const string ActionDestructiveRequiresNameValue = "action.destructive_requires_name: false";
			public const string NetworkHost = "network.host";
			public const string NetworkHostValue = "network.host: RELATIVITYDEVVM";
			public const string ShieldEnabled = "shield.enabled";
			public const string ShieldEnabledValue = "shield.enabled: false";
			public const string PublicJWKsUrl = "publicJWKsUrl";
		}
		
		public class EnvironmentVariables
		{
			public const string JavaPath = @"C:\Program Files\Java";
			public const string KcuraJavaHome = "KCURA_JAVA_HOME";
			public const string JavaHome = "JAVA_HOME";
		}
	}
}
