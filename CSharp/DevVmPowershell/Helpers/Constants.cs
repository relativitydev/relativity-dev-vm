using Relativity.Imaging.Services.Interfaces;
using Relativity.Services.Agent;
using System;
using kCura.Vendor.Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Helpers
{
	public class Constants
	{
		public const int EDDS_WORKSPACE_ARTIFACT_ID = -1;
		public const int OBJECT_TYPE_TYPE_ARTIFACT_ID = 25;
		public const int LAYOUT_TYPE_ARTIFACT_ID = 16;
		public const int FIELD_TYPE_ARTIFACT_ID = 14;

		public class Connection
		{
			public const string PROTOCOL = "http";

			public class Sql
			{
				public const int DEFAULT_SQL_RUNNER_TIMEOUT_IN_SECONDS = 120;
				public const string CONNECTION_STRING_PERSIST_SECURITY_INFO = "False";
				public const string CONNECTION_STRING_PACKET_SIZE = "4096";
				public const string CONNECTION_STRING_CONNECT_TIMEOUT_DEFAULT = "120";
				public const string EDDS_DATABASE = "EDDS";
			}

			public class RestUrlEndpoints
			{
				public class InstanceSettings
				{
					public const string EndpointUrl = "Relativity.REST/api/Relativity.InstanceSettings/workspace/-1/instancesettings/";
				}

				public class ObjectManager
				{
					public const string QueryUrl = "Relativity.REST/api/Relativity.Objects/workspace/-1/object/query";
					public const string QuerySlimUrl = "Relativity.REST/api/Relativity.Objects/workspace/-1/object/queryslim";
					public const string ReadUrl = "Relativity.REST/api/Relativity.Objects/workspace/-1/object/read";
				}

				public class ProcessingManager
				{
					public const string ProcessingSourceCreateUrl = "Relativity.REST/api/relativity-object-model/v1/workspaces/-1/choices/";
				}

				public class WorkspaceManager
				{
					public const string EndpointUrl = "Relativity.REST/api/Relativity.Workspaces/workspace";
				}

				public class ApplicationInstall
				{
					public const int retryCount = 6;
					public const int retryDelay = 10;
					public const string uploadFileKeyName = "rapStream";

					public const string updateLibraryApplicationUrl = "Relativity.REST/api/relativity-environment/v1/workspace/-1/libraryapplications";
					public const string readAllLibraryApplicationUrl = "Relativity.REST/api/relativity-environment/v1/workspace/-1/libraryapplications";
					public const string installWorkspaceApplicationUrl = "Relativity.REST/api/Relativity.LibraryApplications/workspace/-1/libraryapplications/{0}/install";
				}
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

		public class InstanceSetting
		{
			public const string INSTANCE_SETTING_OBJECT_TYPE = "InstanceSetting";
		}

		public class Workspace
		{
			public const string DEFAULT_WORKSPACE_TEMPLATE_NAME = "New Case Template";
		}

		public class Waiting
		{
			public const int SMOKE_TEST_HELPER_MAX_WAIT_TIME_IN_MINUTES = 10;
			public const int SMOKE_TEST_HELPER_SLEEP_TIME_IN_SECONDS = 10;
			public const int SMOKE_TEST_HELPER_RETRY_COUNT = 10;
			public const int IMAGING_HELPER_RETRY_COUNT = 60;
			public const int IMAGING_HELPER_RETRY_DELAY = 10;
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
			public const string ExtractedText = "Extracted Text";
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
			public const string ProcessingSourceLocationFieldGuid = "A18E6EF9-2684-4CB5-9FBD-F4AC9524A8AF";
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
			public const string JAVA_INSTALL_PATH = @"C:\Program Files\Java";
			public const string KCURA_JAVA_HOME = "KCURA_JAVA_HOME";
			public const string JAVA_HOME = "JAVA_HOME";
		}

		public class Imaging
		{
			public class Profile
			{
				public const string NAME = "Sample Imaging Profile";
				public const int IMAGE_OUTPUT_DPI = 100;
				public const ImageFormat BASIC_IMAGE_FORMAT = ImageFormat.Jpeg;
				public const ImageSize IMAGE_SIZE = ImageSize.A4;
				public const ImagingMethod IMAGING_METHOD = ImagingMethod.Basic;
			}

			public class Set
			{
				public const string NAME = "Sample Imaging Set";
				public const string EMAIL_NOTIFICATION_RECIPIENTS = "";
			}

			public class Job
			{
				public const bool QC_ENABLED = false;
			}
		}

		public class Guids
		{
			public class Fields
			{
				public class ImagingSet
				{
					public static Guid Status = new Guid("030747E3-E154-4DF1-BD10-CF6C9734D10A");
				}
			}
		}

		public class Search
		{
			public class KeywordSearch
			{
				public const string OWNER = "Public";
				public const string NAME = "All Documents";
				public const string FIELD_EDIT = "Edit";
				public const string FIELD_FILE_ICON = "File Icon";
				public const string FIELD_CONTROL_NUMBER = "Control Number";
				public const string CONDITION_FIELD_EXTRACTED_TEXT = "Control Number";
				public const string NOTES = "Search for Imaging Set";
			}
		}
		public class Module
		{
			public const int MaxAttemptCount = 5;
			public const int TimeInSecondsBetweenRetry = 5;
		}

		public class DisclaimerAcceptance
		{
			public class ApplicationGuids
			{
				public const string ApplicationGuid = "87E45C02-2D97-41A6-834C-E6AAA905EA85";
			}
			public class ObjectNames
			{
				public const string Disclaimer = "Disclaimer";
				public const string DisclaimerSolutionConfiguration = "Disclaimer Solution Configuration";
			}
			public class ObjectGuids
			{
				public const string Disclaimer = "3F24FC94-F118-44FA-9A40-078D3B92FDB4";
				public const string DisclaimerSolutionConfiguration = "A0365CEC-2657-4683-8A88-AFAB12724405";
			}

			public class DisclaimerSolutionConfigurationFieldGuids
			{
				public const string Name = "A289CD88-C19E-4713-B752-146AEFFD2BE8";
				public const string Enabled = "DB58FDCD-7445-40F0-B3C4-510EF4180652";
				public const string AllowAccessOnError = "9112A06E-7205-4E42-AD70-287C83C0D893";
			}

			public class DisclaimerFieldGuids
			{
				public const string Title = "5A40761E-0FBB-421E-A433-465F9A03642E";
				public const string Text = "4B49715B-ECE6-4F41-948D-27123E85BE22";
				public const string Order = "EBDC5E76-79C2-4EBA-BBBF-1AA5CC47E19A";
				public const string Enabled = "7E61ADE6-7208-4403-8A88-A6DC1DFAFF68";
				public const string AllUsers = "1A535483-CBE5-4ED4-BD63-16DE7BA605D4";
			}

			public class LayoutNames
			{
				public const string DisclaimerLayout = "Disclaimer Layout";
				public const string DisclaimerSolutionConfigurationLayout = "Disclaimer Solution Configuration Layout";
			}

			public const string DisclaimerValue = @"
			<h2>Welcome to the RelativityDevVm!</h2>
			<p>The&nbsp;RelativityDevVm&nbsp;has been designed to help developers test the functionality of their Relativity applications.</p>
			<h4>This&nbsp;DevVm&nbsp;is intended to be used for:</h4>
			<ul>
			<li>Basic Relativity Development such as creating applications with Custom Pages, Agents, Event Handlers, etc.&nbsp;</li>
			<li>Remote Debugging Custom Applications</li>
			<li>The inteded use of the DevVm is meant to be ephemeral. Loading of test data should be automated and there should be no upgrading the DevVm through the Relativity Installer files.</li>
			</ul>
			<h4>Not-Supported:&nbsp;</h4>
			<p>The following are <strong>not</strong> supported in Dev VM.</p>
			<ul>
			<li>Changing the VM name.&nbsp;</li>
			<li>Setting the VM in the cloud like Azure, AWS etc.&nbsp;</li>
			<li>Getting the VM image in a VM format other than Hyper-V.&nbsp;</li>
			<li><span style='color: #ff0000;'><strong>Please note that these VM's should not be used&nbsp;to performance test your applications as system resources are not equivalent to suggested production configurations</strong></span></li>
			</ul>
			<p>(We usually configure our&nbsp;DevVms&nbsp;to 4 cores and 12GB of RAM and can generally develop/debug applications with minimal lag.)</p>
			<h4>Disclaimer:&nbsp;</h4>
			<ul>
			<li>You are responsible for OS updates on the Dev VM&nbsp;</li>
			<li>You should <strong>NOT</strong> put sensitive data on the Dev VM. We can provide a sample data set&nbsp;</li>
			<li>The Dev VM is not to be shared with anyone outside of your organization&nbsp;</li>
			</ul>
			<h4>Unavailable Features:&nbsp;</h4>
			<ul>
			<li>Analytics&nbsp;</li>
			</ul>
			<h4>License:</h4>
			<p>The Dev VM comes with the following licenses.</p>
			<ul>
			<li>Windows Server 2012 R2 Standard
			<ul>
			<li>6-month trial license</li>
			<li>Instructions to update Windows license - <a href = 'https://github.com/relativitydev/relativity-dev-vm/wiki' > https://github.com/relativitydev/relativity-dev-vm/wiki</a></li>
			</ul>
			</li>
			<li>Relativity
			<ul>
			<li>7-day trial license</li>
			<li>Contact<a href='mailto:support@relativity.com'> support@relativity.com</a> to get a new DevVM Developer license</li>
			</ul>
			</li>
			<li>Processing
			<ul>
			<li>7-day trial license</li>
			<li>Contact<a href='mailto:support@relativity.com'> support@relativity.com</a> to get a new DevVM Developer license</li>
			</ul>
			</li>
			<li>SQL Server 2017 Developer edition
			<ul>
			<li>Free for non-production use</li>
			<li>More info on SQL Licensing -<a href = 'http://download.microsoft.com/download/7/8/c/78cdf005-97c1-4129-926b-ce4a6fe92cf5/sql_server_2017_licensing_guide.pdf' > http://download.microsoft.com/download/7/8/c/78cdf005-97c1-4129-926b-ce4a6fe92cf5/sql_server_2017_licensing_guide.pdf</a></li>
			</ul>
			</li>
			</ul>
			<p>For questions related to the&nbsp; RelativityDevVm, we recommend to please refer to the&nbsp;DevVm&nbsp;Category on&nbsp;DevHelp&nbsp;(<a href = 'https://devhelp.relativity.com/c/tools-testing-download-and-tutorials/devvm' > https://devhelp.relativity.com/c/tools-testing-download-and-tutorials/devvm</a>). If you can't find the answer to your question, please create a new post with your question. Someone from our Developer Support group will respond to your question within a few days.&nbsp;</p>
			<p>For a more detailed documentation on how to use the&nbsp;RelativityDevVm, please refer to this link: <a href = 'https://github.com/relativitydev/relativity-dev-vm/blob/master/Documentation/PDF/Relativity%20Dev%20VM%20-%20Pre-built%20VM%20-%20Documentation.pdf' target= '_blank' rel= 'noopener noreferrer' > https://github.com/relativitydev/relativity-dev-vm/blob/master/Documentation/PDF/Relativity%20Dev%20VM%20-%20Pre-built%20VM%20-%20Documentation.pdf</a>&nbsp;</p>";
		}

		public class ErrorMessages
		{
			public class Sql
			{
				public const string SQL_ROLLBACK_ERROR = "An error occurred when doing a SQL rollback.";
				public const string EXECUTE_SQL_STATEMENT_ERROR = "An error occurred when executing SQL statement.";
				public const string EXECUTE_SQL_STATEMENT_AS_SCALAR_ERROR = "An error occurred when executing SQL statement as Scalar.";
				public const string EXECUTE_SQL_STATEMENT_AS_DATATABLE_ERROR = "An error occurred when executing SQL statement as DataTable.";
				public const string EXECUTE_SQL_STATEMENT_AS_DATASET_ERROR = "An error occurred when executing SQL statement as DataSet.";
				public const string EXECUTE_NON_QUERY_SQL_STATEMENT_ERROR = "An error occurred when executing Non-query SQL statement.";
				public const string QUERY_ALL_APPLICATION_QUERY_ERROR = "An error occurred when quering for all Applications.";
				public const string QUERY_ALL_APPLICATION_VERSIONS_FOR_GIVEN_APPLICATION_IDS_ERROR = "An error occurred when querying for the ApplicationVersions for given ApplicationIds.";
				public const string QUERY_APPLICATION_VERSION_NAME_FOR_GIVEN_APPLICATION_VERSION_ID_ERROR = "An error occurred when querying for the ApplicationVersion Name for given ApplicationVersion Id.";
				public const string QUERY_ALL_RELATIVITY_VERSIONS_FOR_GIVEN_APPLICATION_VERSION_IDS_ERROR = "An error occurred when querying for the ApplicationVersions for given ApplicationIds.";

			}
		}

		public class SmokeTest
		{
			public class ObjectNames
			{
				public const string SmokeTestApplicationName = "Smoke Test";
				public const string SmokeTestApplicationObjectTypeName = "Test";
			}
			public class Guids
			{
				public const string ApplicationGuid = "0125C8D4-8354-4D8F-B031-01E73C866C7C";
				public const string TestObjectType = "71ED667F-EE38-4BC7-AB76-6645D8A5587F";

				public class Fields
				{
					public const string Name_FixedLengthText = "9D9F70EF-0AB1-4912-849E-418976EA92CF";
					public const string Status_FixedLengthText = "A057E0A2-73CE-44B1-AF8D-F8B896358446";
					public const string Error_LongText = "D289FEAA-76F4-4E82-86D6-E2E522F42A71";
					public const string ErrorDetails_LongText = "F615298C-EB1E-4892-B8AA-103CEE85888B";
				}
			}
		}

		public class ApplicationGuids
		{
			public const string SimpleFileUploadGuid = "1738CEB6-9546-44A7-8B9B-E64C88E47320";
		}
	}
}
